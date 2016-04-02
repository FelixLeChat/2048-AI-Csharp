using System.Collections.Generic;
using _2048.Model;
using _2048.WPF.Enums;
using _2048.WPF.Game;
using _2048.WPF.Helper;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace _2048
{
    public sealed partial class GameGrid
    {
        private const int Rows = 4;
        private const int Cols = 4;

        private readonly GameTile[][] _underlyingTiles;
        public GameModel GameModel { get; set; }

        public ScoreCard ScoreCard { get; set; }

        public int Score
        {
            get
            {
                return GameModel.Score;
            }
        }

        private double GetTileSize()
        {
            return GameCanvas.ActualWidth / Rows;
        }

        public GameGrid()
        {
            InitializeComponent();

            SizeChanged += GameGrid_SizeChanged;

            GameModel = new GameModel(Rows, Cols);

            _underlyingTiles = new GameTile[Cols][];

            for (int i = 0; i < Cols; ++i)
            {
                _underlyingTiles[i] = new GameTile[Rows];
            }

            for (int y = 0; y < Rows; ++y)
            {
                for (int x = 0; x < Cols; ++x)
                {
                    _underlyingTiles[x][y] = new GameTile(x, y);
                    _underlyingTiles[x][y].SetValue(Panel.ZIndexProperty, 0);
                    GameCanvas.Children.Add(_underlyingTiles[x][y]);
                }
            }
            
            ScoreCard = new ScoreCard();
            ScoreCard.SetValue(Grid.RowProperty, 0);
            ScoreCard.SetValue(Grid.ColumnProperty, 0);
            ContentGrid.Children.Add(ScoreCard);

            ScoreCard.Score = 0;
            ScoreCard.Title = "SCORE";
            
            StartGame();
        }

        private void GameGrid_SizeChanged(object sender, SizeChangedEventArgs args)
        {
            for (var y = 0; y < Rows; ++y)
            {
                for (var x = 0; x < Cols; ++x)
                {
                    _underlyingTiles[x][y].Width = GetTileSize();
                    _underlyingTiles[x][y].Height = GetTileSize();
                    _underlyingTiles[x][y].SetValue(Canvas.LeftProperty, x * GetTileSize());
                    _underlyingTiles[x][y].SetValue(Canvas.TopProperty, y * GetTileSize());
                }
            }
        }
        
        private void LoadMap()
        {
            GameModel.Cells[2][0] = new Cell(2, 0) { Value = 8 };
            GameModel.Cells[2][2] = new Cell(2, 2) { Value = 4 };
            GameModel.Cells[2][3] = new Cell(2, 3) { Value = 4 };
            GameModel.Cells[3][2] = new Cell(3, 2) { Value = 8 };
            GameModel.Cells[3][3] = new Cell(3, 3) { Value = 2 };
        }

        private void StartGame()
        {
            LoadMap();
            UpdateUi();
        }

        private void UpdateUi()
        {
            foreach (var cell in GameModel.CellsIterator())
            {
                _underlyingTiles[cell.X][cell.Y].StopAnimations();
            }

            // Set to 0 any underlying tile where MovedFrom != null && !WasDoubled OR newValue == 0
            
            foreach (var cell in GameModel.CellsIterator())
            {
                if ((cell.PreviousPosition != null && !cell.WasMerged) || cell.Value == 0 || cell.WasCreated)
                {
                    _underlyingTiles[cell.X][cell.Y].Value = 0;
                }
            }

            // For each tile where MovedFrom != null
            // Create a new temporary animation tile and animate to move to new location
            var storyboard = new Storyboard();
            var tempTiles = new List<GameTile>();
            for (var y = 0; y < Rows; ++y)
            {
                for (var x = 0; x < Cols; ++x)
                {
                    if (GameModel.Cells[x][y].PreviousPosition != null)
                    {
                        var tempTile = new GameTile(x, y, true);
                        tempTile.Width = GetTileSize();
                        tempTile.Height = GetTileSize();
                        tempTile.SetValue(Panel.ZIndexProperty, 1);
                        tempTiles.Add(tempTile);
                        GameCanvas.Children.Add(tempTile);

                        tempTile.Value = GameModel.Cells[x][y].WasMerged ? GameModel.Cells[x][y].Value / 2 : GameModel.Cells[x][y].Value;

                        var from = GameModel.Cells[x][y].PreviousPosition.X * GetTileSize();
                        var to = x * GetTileSize();
                        var xAnimation = Animation.CreateDoubleAnimation(from, to, 1200000);

                        from = GameModel.Cells[x][y].PreviousPosition.Y * GetTileSize();
                        to = y * GetTileSize();
                        var yAnimation = Animation.CreateDoubleAnimation(from, to, 1200000);

                        Storyboard.SetTarget(xAnimation, tempTile);
                        Storyboard.SetTargetProperty(xAnimation, Animation.CreatePropertyPath("(Canvas.Left)"));

                        Storyboard.SetTarget(yAnimation, tempTile);
                        Storyboard.SetTargetProperty(yAnimation, Animation.CreatePropertyPath("(Canvas.Top)"));

                        storyboard.Children.Add(xAnimation);
                        storyboard.Children.Add(yAnimation);
                    }
                }
            }

            storyboard.Completed += (sender, o) =>
            {
                for (var y = 0; y < Rows; ++y)
                {
                    for (var x = 0; x < Cols; ++x)
                    {
                        _underlyingTiles[x][y].Value = GameModel.Cells[x][y].Value;
                    }
                }

                foreach (var tile in tempTiles)
                {
                    GameCanvas.Children.Remove(tile);
                }

                foreach (var cell in GameModel.CellsIterator())
                {
                    if (cell.WasCreated)
                    {
                        _underlyingTiles[cell.X][cell.Y].BeginNewTileAnimation();
                    }
                    else if (cell.WasMerged)
                    {
                        _underlyingTiles[cell.X][cell.Y].SetValue(Panel.ZIndexProperty, 100);
                        _underlyingTiles[cell.X][cell.Y].BeginDoubledAnimation();
                    }

                    ResetCell(cell);
                }

                MoveInProgress = false;

                // Update the score
                ScoreCard.Score = GameModel.Score;
            };

            storyboard.Begin();
        }

        // Reset state of cells for when animation does not occur
        public void ResetCells()
        {
            foreach (var col in GameModel.Cells)
            {
                foreach (var cell in col)
                {
                    ResetCell(cell);
                }
            }
        }

        private static void ResetCell(Cell cell)
        {
            cell.WasCreated = false;
            cell.WasMerged = false;
            cell.PreviousPosition = null;
        }

        public static bool MoveInProgress { get; set; }
        public void HandleMove(Direction direction)
        {
            if (MoveInProgress)
            {
                return;
            }

            MoveInProgress = true;

            if (GameModel.PerformMove(direction))
            {
                UpdateUi();
            }
            else
            {
                MoveInProgress = false;
            }
        }

        private int _passScore;
        private int _iter;
        public State CheckForWin()
        {
            var state = State.NotFinished;
            
            // Check for full tileset
            if (Helper.IsFullTileSet(GameModel.Cells))
            {
                if (Score == _passScore)
                    _iter++;
                else
                    _iter = 0;

                // Lost if more than 20 iteration with no score modif (no fuse of 2 blocks)
                if(_iter > 20)
                    state = State.Lost;
            }

            _passScore = Score;
            return state;
        }
    }
}
