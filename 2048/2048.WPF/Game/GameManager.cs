using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using _2048.WPF.Converter;
using _2048.WPF.Enums;
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
            }

            GameGrid = new GameGrid();
            ScoreList = new ObservableCollection<ScoreModel>();
            Stats = new StatModel();
            Animate = settings.IsAnimated;
            Restart = settings.IsRestart;
            _moveTimer = new Stopwatch();
            _gameTimer = new Stopwatch();
        }

        public void RestartGrid()
        {
            GameGrid = new GameGrid();
        }

        public void StartGame()
        {
            if (Strategy == null)
                return;

            // Initialize Strategy
            Strategy.Initialize(GameGrid.GameModel);


            // Token for thread cancelation
            _cancelToken = new CancellationTokenSource();
            Task<State>.Factory.StartNew(() =>
            {
                var gameState = State.NotFinished;

                // Stats
                _gameTimer.Restart();
                _moveTimer.Restart();

                while (!_cancelToken.IsCancellationRequested)
                {
                    // If no animation to do
                    if (!GameGrid.MoveInProgress)
                    {
                        Application.Current.Dispatcher.Invoke(
                            () =>
                            {
                                // Stats
                                _moveTimer.Start();
                                Stats.TotalMoveCount++;

                                var direction = Strategy.GetDirection(GameGrid.GameModel);
                                // Move
                                if (Animate)
                                    GameGrid.HandleMove(direction);
                                else
                                {
                                    GameGrid.GameModel.PerformMove(direction);
                                    GameGrid.MoveInProgress = false;
                                    GameGrid.ResetCells();
                                }
                                _moveTimer.Stop();

                                Stats.TotalMoveTime += _moveTimer.ElapsedMilliseconds;
                                Stats.AverageMoveTime = Stats.TotalMoveTime/Stats.TotalMoveCount;


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
                            MaxTile = Helper.Helper.GetMaxTile(GameGrid.GameModel.Cells),
                            Score = GameGrid.Score,
                            State = task.Result
                        };
                        // Notice Strategy
                        Strategy.Ended(newScore);

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

                        // Gamecount
                        Stats.TotalGamePlayed++;
                        if (task.Result == State.Won) Stats.TotalWins++;
                        else Stats.TotalLosses++;

                        // Time
                        Stats.TotalGameTime += _gameTimer.ElapsedMilliseconds;
                        Stats.AverageGameTime = Stats.TotalGameTime/Stats.TotalGamePlayed;

                        Stats.AverageMoveCount = Stats.TotalMoveCount/Stats.TotalGamePlayed;
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