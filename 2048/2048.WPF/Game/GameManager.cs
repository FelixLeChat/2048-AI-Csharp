using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using _2048.AI.Enums;
using _2048.AI.Helper;
using _2048.AI.Model.Core;
using _2048.AI.Model.Optimize;
using _2048.AI.Strategy;
using _2048.WPF.Model;

namespace _2048.WPF.Game
{
    public class GameManager
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance ?? (_instance = new GameManager());
        private GameManager()
        {
        }

        public ObservableCollection<ScoreModel> ScoreList { get; set; } = new ObservableCollection<ScoreModel>();
        public StatModel Stats { get; set; } = new StatModel();
        private CancellationTokenSource _cancelToken;
        private Stopwatch _moveTimer;
        private Stopwatch _gameTimer;
        public IStrategy Strategy { get; set; }
        public bool Animate { get; set; }
        public bool Restart { get; set; }

        private BoardType BoardType { get; set; }

        // GameGrid a les elements visuels mais mous n'avons besoin que de GameModel pour la simulation rapide
        public GameGrid GameGrid { get; set; }

        public void Init(GameSettings settings)
        {
            switch (settings.GameType)
            {
                case Type.Manual:
                    Strategy = null;
                    break;
                case Type.Random:
                    Strategy = new RandomStrategy();
                    break;
                case Type.MasterAlgo:
                    Strategy = new MasterAlgo();
                    break;
                case Type.DepthFirst:
                    Strategy = new DeptFirstStrategy();
                    break;
                case Type.IterativeDeepening:
                    Strategy =new IterativeDeepening();
                    break;
                case Type.Expectimax:
                    Strategy = new ExpectimaxStrategy();
                    break;
            }

            GameGrid = new GameGrid();
            ScoreList = new ObservableCollection<ScoreModel>();
            Stats = new StatModel();
            Animate = settings.IsAnimated;
            Restart = settings.IsRestart;
            _moveTimer = new Stopwatch();
            _gameTimer = new Stopwatch();

            BoardType = settings.BoardType;
            if (BoardType == BoardType.Optimized)
            {
                OptimizeBoardHelper.InitLookupTable();
            }
        }

        public void RestartGrid()
        {
            GameGrid = new GameGrid();
        }

        public void StartGame()
        {
            if (Strategy == null)
                return;

            // Token for thread cancelation
            _cancelToken = new CancellationTokenSource();
            Task<State>.Factory.StartNew(() =>
            {
                var gameState = State.NotFinished;

                // Stats
                _gameTimer.Restart();

                while (!_cancelToken.IsCancellationRequested)
                {
                    // If no animation to do
                    if (!GameGrid.MoveInProgress)
                    {
                        Application.Current.Dispatcher.Invoke(
                            () =>
                            {
                                // Stats
                                _moveTimer.Restart();
                                Stats.TotalMoveCount++;

                                IBoard board = GameGrid.GameModel;
                                if (BoardType == BoardType.Optimized)
                                    board = Helper.Translate(GameGrid.GameModel);

                                var direction = Strategy.GetDirection(board);
                                // Move
                                if (Animate)
                                    GameGrid.HandleMove(direction);
                                else
                                {
                                    GameGrid.GameModel.PerformMoveAndSpawn(direction);
                                    GameGrid.MoveInProgress = false;
                                    GameGrid.ResetCells();
                                }
                                _moveTimer.Stop();

                                Stats.TotalMoveTime += _moveTimer.ElapsedMilliseconds;
                                
                                // Check for win or loose
                                gameState = GameGrid.CheckForWin();
                                if (gameState != State.NotFinished)
                                {
                                    // Won or lost
                                    _cancelToken.Cancel();
                                    _gameTimer.Stop();
                                }
                            });
                    }
                }
                return gameState;

            }, _cancelToken.Token).ContinueWith(task =>
            {
                Application.Current.Dispatcher.Invoke(
                    () =>
                    {
                        var newScore = new ScoreModel()
                        {
                            MaxTile = Helper.GetMaxTile(GameGrid.GameModel.Cells),
                            Score = GameGrid.Score,
                            State = task.Result
                        };

                        // Do not add to board if not finished
                        if (task.Result == State.NotFinished) return;
                        ScoreList.Add(newScore);

                        #region Stats

                        //Max tile and score
                        if (newScore.MaxTile >= Stats.MaxTile)
                        {
                            if (newScore.Score > Stats.MaxScore)
                            {
                                Stats.MaxScore = newScore.Score;
                                Stats.MaxTile = newScore.MaxTile;
                            }
                        }

                        switch (newScore.MaxTile)
                        {
                            case 128:
                                Stats.Get128++;
                                break;
                            case 256:
                                Stats.Get256++;
                                break;
                            case 512:
                                Stats.Get512++;
                                break;
                            case 1024:
                                Stats.Get1024++;
                                break;
                        }

                        // Gamecount
                        Stats.TotalGamePlayed++;
                        if (task.Result == State.Won) Stats.TotalWins++;
                        else Stats.TotalLosses++;

                        // Time
                        Stats.TotalGameTime += _gameTimer.ElapsedMilliseconds;
                        Stats.AverageGameTime = Stats.TotalGameTime/Stats.TotalGamePlayed;

                        Stats.AverageMoveCount = Stats.TotalMoveCount/Stats.TotalGamePlayed;
                        Stats.AverageMoveTime = Stats.TotalMoveTime / Stats.TotalMoveCount;
                        #endregion

                        //Restart if not force stop
                        if (Restart && !ForceStop)
                        {
                            RestartGrid();
                            StartGame();
                        }
                    });
            });
        }

        private bool ForceStop { get; set; }
        public void StopGame(bool force = false)
        {
            try
            {
                _cancelToken.Cancel();
                _cancelToken.Dispose();
                ForceStop = force;
            }
            catch { }
        }
    }
}