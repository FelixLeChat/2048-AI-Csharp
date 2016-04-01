using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using _2048.WPF.Game;

namespace _2048.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const int MaxGridSize = 600;

        private GameGrid _gameGrid;
        private readonly IStrategy _currentStrategy = new Defaultstrategy();
        private readonly GameManager _gameManager;
        private readonly ObservableCollection<GameEntry> _scoreList = new ObservableCollection<GameEntry>();
        public static MainWindow Instance;

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;

            _gameManager = GameManager.Instance;
            var type = GameType.Manual;
            _gameGrid = new GameGrid();
            ContentGrid.Children.Add(_gameGrid);

            SizeChanged += OnSizeChanged;

            // Manual entry for game
            if(type == GameType.Manual)
                KeyDown += MainWindow_KeyDown;

            ListGameScore.ItemsSource = _scoreList;
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
                _gameGrid.HandleMove(direction.Value);
            }
        }
        #endregion

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var gridSize = Math.Min(ContentGrid.ActualHeight, ContentGrid.ActualWidth) * 0.9;
            gridSize = Math.Min(gridSize, MaxGridSize);

            _gameGrid.Width = gridSize;
            _gameGrid.Height = gridSize;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StopButton.IsEnabled = false;
            StartButton.IsEnabled = true;
            _gameManager.StopGame();
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            _gameManager.StopGame();
            RestartGame();
        }

        public void RestartGame()
        {
            // Add Score to list
            _scoreList.Add(new GameEntry() { Score = _gameGrid.Score });

            // Add new grid to view
            ContentGrid.Children.Remove(_gameGrid);
            _gameGrid = new GameGrid();
            ContentGrid.Children.Add(_gameGrid);
            OnSizeChanged(null, null);

            // Button visibility
            StopButton.IsEnabled = false;
            StartButton.IsEnabled = true;
            RestartButton.IsEnabled = false;
        }

        public void StartGame()
        {
            StopButton.IsEnabled = true;
            RestartButton.IsEnabled = true;
            StartButton.IsEnabled = false;

            _gameManager.StartGame(_gameGrid, _currentStrategy);
        }
    }
}
