using System;
using System.Windows;
using System.Windows.Input;
using _2048.AI.Enums;
using _2048.WPF.Game;
using _2048.WPF.Model;
using Type = _2048.AI.Enums.Type;

namespace _2048.WPF.UI
{
    public partial class GameWindow
    {
        private const int MaxGridSize = 600;
        private readonly GameManager _gameManager;
        public static GameWindow Instance;
        private static bool _isTraining;

        public GameWindow(GameSettings settings)
        {
            // Window Init
            InitializeComponent();
            Instance = this;

            _gameManager = GameManager.Instance;

            // New UI grid to display
            ContentGrid.Children.Add(_gameManager.GameGrid);
            SizeChanged += OnSizeChanged;

            // Settup on game type
            if (settings.GameType == Type.Manual)
            {
                KeyDown += MainWindow_KeyDown;
                StopButton.Visibility = Visibility.Collapsed;
                StartButton.Visibility = Visibility.Collapsed;
            }

            //Data Bindings
            ListGameScore.ItemsSource = _gameManager.ScoreList;
            ListTrainingScore.ItemsSource = TrainingManager.Instance.TrainingList;
            TopScorePanel.DataContext = _gameManager.Stats;
            TopTrainingPanel.DataContext = TrainingManager.Instance.TrainingStats;

            // Visual VS Training
            if (_gameManager.IsTraining)
            {
                TopScorePanel.Visibility = Visibility.Collapsed;
                ScoreScroll.Visibility = Visibility.Collapsed;
                ContentGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                TrainingScroll.Visibility = Visibility.Collapsed;
                TopTrainingPanel.Visibility = Visibility.Collapsed;
            }
        }

        #region Manual Entry
        void MainWindow_KeyDown(object sender, KeyEventArgs args)
        {
            Direction? direction = null;
            if (args.Key == Key.Up)
            {
                direction = Direction.Up;
            }
            else if (args.Key == Key.Down)
            {
                direction = Direction.Down;
            }
            else if (args.Key == Key.Left)
            {
                direction = Direction.Left;
            }
            else if (args.Key == Key.Right)
            {
                direction = Direction.Right;
            }

            if (direction != null)
            {
                _gameManager.GameGrid.HandleMove(direction.Value);
            }
        }
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var gridSize = Math.Min(ContentGrid.ActualHeight, ContentGrid.ActualWidth) * 0.9;
            gridSize = Math.Min(gridSize, MaxGridSize);

            _gameManager.GameGrid.Width = gridSize;
            _gameManager.GameGrid.Height = gridSize;
        }
        #endregion

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // Button Visbility
            StopButton.IsEnabled = true;
            RestartButton.IsEnabled = true;
            StartButton.IsEnabled = false;

            // Start Game
            if (!_gameManager.IsTraining)
                _gameManager.StartGame();
            else
            {
                _gameManager.StartTraining();
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            // Button Visbility
            StopButton.IsEnabled = false;
            StartButton.IsEnabled = true;

            // Stop Game
            _gameManager.StopGame(true);
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            // Button visibility
            StopButton.IsEnabled = false;
            StartButton.IsEnabled = true;
            RestartButton.IsEnabled = false;

            // Stop Game
            _gameManager.StopGame();

            // Add new grid to view
            ContentGrid.Children.Remove(_gameManager.GameGrid);
            _gameManager.RestartGrid();
            ContentGrid.Children.Add(_gameManager.GameGrid);
            OnSizeChanged(null, null);
        }

        private void MainMenuButton_Click(object sender, RoutedEventArgs e)
        {
            // Stop Game
            _gameManager.StopGame(true);

            var mainMenu = new GameSettingWindow();
            mainMenu.Show();
            Close();
        }

        private void ScoreScroll_LayoutUpdated(object sender, EventArgs e)
        {
            ScoreScroll.ScrollToBottom();
        }
    }
}
