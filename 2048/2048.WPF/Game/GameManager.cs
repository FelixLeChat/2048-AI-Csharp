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

        private Task _currentTask;
        private CancellationTokenSource _cancelToken;

        private bool _loop = true;
        public void StartGame(GameGrid grid, IStrategy strategy)
        {
            _loop = true;

            _cancelToken = new CancellationTokenSource();
            _currentTask = Task.Factory.StartNew(() =>
            {
                while (_loop)
                {
                    Application.Current.Dispatcher.Invoke(
                        () =>
                        {
                            if(_loop)
                                _loop = grid.HandleMove(strategy.GetDirection(grid.GameModel));
                        });
                }
            }, _cancelToken.Token);
        }

        public void StopGame()
        {
            _loop = false;
            try
            {
                _cancelToken.Cancel();
                _cancelToken.Dispose();
            }
            catch { }
        }
    }
}