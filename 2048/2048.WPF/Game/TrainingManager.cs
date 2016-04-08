using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using _2048.AI.Enums;
using _2048.AI.Heuristics;
using _2048.AI.Learning.Core;
using _2048.AI.Model.Core;
using _2048.AI.Model.Optimize;
using _2048.AI.Strategy;
using _2048.AI.Model.Stats;



namespace _2048.WPF.Game
{
    public class TrainingManager
    {
        private static TrainingManager _instance;
        public static TrainingManager Instance => _instance ?? (_instance = new TrainingManager());
        private TrainingManager()
        {
        }


        private const int GameIterationInPopulation = 100;
        private const int PopulationInNextGeneration = 5;

        private readonly List<PopulationNode> _population = new List<PopulationNode>();
        private readonly ILearner _learner;


        public void StartTraining(CancellationTokenSource cancelToken, IStrategy strategy)
        {
            Task.Factory.StartNew(() =>
            {
                // Simulate infinite generations
                while (!cancelToken.IsCancellationRequested)
                {
                    var generation = _learner.GetNewGeneration(_population, PopulationInNextGeneration);

                    // Simulate all new element in generation
                    foreach (var heuristicFactor in generation)
                    {
                        var iteration = 0;
                        var generationStat = new StatModel();

                        // Play X games
                        while (iteration < GameIterationInPopulation && !cancelToken.IsCancellationRequested)
                        {
                            // Initialize the board with the Heuristic Factors of the current generation
                            IBoard board = new OptimizeBoard();
                            board.Initialize(heuristicFactor);

                            // Play Game
                            var gameState = State.NotFinished;
                            while (gameState == State.NotFinished && !cancelToken.IsCancellationRequested)
                            {
                                var direction = strategy.GetDirection(board);
                                board.PerformMove(direction);

                                gameState = CheckForWin(board);
                            }

                            // game finished
                            var score = board.GetScore();
                            if (score > generationStat.MaxScore)
                                generationStat.MaxScore = score;

                            iteration++;
                        }

                        // Add info to next generation
                        _population.Add(new PopulationNode()
                        {
                            Heuristic = heuristicFactor,
                            Stat = generationStat
                        });
                    }
                }
            }, cancelToken.Token).ContinueWith(task =>
            {
                
            });
        }

        private int _passScore;
        private int _iter;
        private State CheckForWin(IBoard board)
        {
            var state = State.NotFinished;

            if(Heuristics.IsWon(board))
                return State.Won;


            // Check for full tileset
            if (Heuristics.CountEmptyCells(board) == 0)
            {
                var score = board.GetScore();

                if (score == _passScore)
                    _iter++;
                else
                    _iter = 0;

                // Lost if more than 20 iteration with no score modif (no fuse of 2 blocks)
                if (_iter > 20)
                    state = State.Lost;

                _passScore = score;
            }
            
            return state;
        }

    }
}