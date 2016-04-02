using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using _2048.WPF.Converter;
using _2048.WPF.Enums;

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
        private CancellationTokenSource _cancelToken;
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
            }

            GameGrid = new GameGrid();
            ScoreList = new ObservableCollection<ScoreModel>();
            Animate = settings.IsAnimated;
            Restart = settings.IsRestart;
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
                var gameState = State.None;

                while (!_cancelToken.IsCancellationRequested)
                {
                    // If no animation to do
                    if (!GameGrid.MoveInProgress)
                    {
                        Application.Current.Dispatcher.Invoke(
                            () =>
                            {
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

                                // Check for win or loose
                                gameState = GameGrid.CheckForWin();
                                if (gameState != State.None)
                                {
                                    // Won or lost
                                    _cancelToken.Cancel();
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
                        ScoreList.Add(new ScoreModel()
                        {
                            MaxTile = Helper.Helper.GetMaxTile(GameGrid.GameModel.Cells),
                            Score = GameGrid.Score,
                            State = task.Result
                        });

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