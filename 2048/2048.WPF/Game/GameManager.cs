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

        private bool _stop;
        private IStrategy _strategy;

        public void StartGame(GameGrid grid, IStrategy strategy)
        {
            _strategy = strategy;
            _stop = false;
            Iterate(grid);
        }

        public void Iterate(GameGrid grid)
        {
            if (_stop || _strategy == null) return;

            Application.Current.Dispatcher.Invoke(
            () =>
            {
                // Move
                grid.HandleMove(_strategy.GetDirection(grid.GameModel));

                // Check for win or loose
                var gameState = grid.CheckForWin();
                if (gameState != GameGrid.State.None)
                {
                    // Won or lost
                }
            });
        }

        public void StopGame()
        {
            _stop = true;
        }
    }
}