﻿using System.Collections.ObjectModel;
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
using Type = _2048.AI.Enums.Type;

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
        private CancellationTokenSource _cancelToken = new CancellationTokenSource();
        private Stopwatch _moveTimer;
        private Stopwatch _gameTimer;
        public IStrategy Strategy { get; set; }
        public bool Animate { get; set; }
        public bool Restart { get; set; }
        public bool IsTraining { get; set; }

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
                case Type.IterativeDeepening:
                    Strategy =new IterativeDeepening();
                    break;
                case Type.Expectimax:
                    Strategy = new ExpectimaxStrategy();
                    break;
            }

            Strategy?.SetSearchDepth(settings.Depth);

            IsTraining = settings.GameMode == Mode.Training;

            GameGrid = new GameGrid();
            ScoreList = new ObservableCollection<ScoreModel>();
            Stats = new StatModel();
            Animate = settings.IsAnimated;
            Restart = settings.IsRestart;
            _moveTimer = new Stopwatch();
            _gameTimer = new Stopwatch();

            BoardType = settings.BoardType;
            // Small hack
            if (BoardType == BoardType.Optimized)
            {
                var temp = new OptimizeBoard();
                temp.Initialize();
            }

            TrainingManager.Instance.Init(settings);
        }

        public void RestartGrid()
        {
            GameGrid = new GameGrid();
            TrainingManager.Instance.Reset();
        }

        public void StartGame()
        {
            if (Strategy == null)
                return;

            // Token for thread cancelation
            if (_cancelToken != null)
            {
                _cancelToken.Cancel();
                _cancelToken.Dispose();
            }
            _cancelToken = new CancellationTokenSource();

            Task<State>.Factory.StartNew(() =>
            {
                var gameState = State.NotFinished;

                // Stats
                _gameTimer.Restart();

                while (_cancelToken != null && !_cancelToken.IsCancellationRequested)
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
                                if (gameState == State.Lost)
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
                            case 2048:
                                Stats.Get2048++;
                                break;
                            case 4096:
                                Stats.Get4096++;
                                break;
                            case 8192:
                                Stats.Get8192++;
                                break;
                            case 16384:
                                Stats.Get16384++;
                                break;
                        }

                        // Gamecount
                        Stats.TotalGamePlayed++;
                        if (newScore.MaxTile >= 2048) Stats.TotalWins++;
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

        public void StartTraining()
        {            
            // Token for thread cancelation
            if (_cancelToken != null)
            {
                _cancelToken.Cancel();
                _cancelToken.Dispose();
            }
            _cancelToken = new CancellationTokenSource();

            TrainingManager.Instance.StartTraining(_cancelToken, Strategy);
        }

        private bool ForceStop { get; set; }
        public void StopGame(bool force = false)
        {
            try
            {
                _cancelToken.Cancel();
                _cancelToken.Dispose();
                ForceStop = force;

                _cancelToken = null;
            }
            catch { }
        }
    }
}