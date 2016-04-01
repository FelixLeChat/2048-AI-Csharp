using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace _2048.WPF.Game
{
    public class GameManager
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance ?? (_instance = new GameManager());

        private GameManager()
        {
        }

        private CancellationTokenSource _cancelToken;

        public void StartGame(GameGrid grid, IStrategy strategy)
        {
            _cancelToken = new CancellationTokenSource();
            Task<bool>.Factory.StartNew(() =>
            {
                var gameState = GameGrid.State.None;

                while (!_cancelToken.IsCancellationRequested)
                {
                    // If no animation to do
                    if (!GameGrid.MoveInProgress)
                    {
                        Application.Current.Dispatcher.Invoke(
                            () =>
                            {
                                // Move
                                grid.HandleMove(strategy.GetDirection(grid.GameModel));

                                // Check for win or loose
                                gameState = grid.CheckForWin();
                                if (gameState != GameGrid.State.None)
                                {
                                    // Won or lost
                                    _cancelToken.Cancel();
                                }
                            });
                    }
                }
                return gameState == GameGrid.State.Won;

            }, _cancelToken.Token).ContinueWith(task =>
            {
                Application.Current.Dispatcher.Invoke(
                    () =>
                    {
                        var result = task.Result;
                        //MainWindow.Instance.RestartGame();
                        //MainWindow.Instance.StartGame();
                    });
            });
        }

        public void StopGame()
        {
            try
            {
                _cancelToken.Cancel();
                _cancelToken.Dispose();
            }
            catch { }
        }
    }
}