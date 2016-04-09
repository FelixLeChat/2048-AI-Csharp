using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using _2048.AI.Enums;
using _2048.AI.Helper;
using _2048.AI.Heuristics;
using _2048.AI.Learning;
using _2048.AI.Learning.Core;
using _2048.AI.Model.Core;
using _2048.AI.Model.Optimize;
using _2048.AI.Strategy;
using _2048.WPF.Model;
using StatModel = _2048.AI.Model.Stats.StatModel;


namespace _2048.WPF.Game
{
    public class TrainingManager
    {
        private static TrainingManager _instance;
        public static TrainingManager Instance => _instance ?? (_instance = new TrainingManager());
        private TrainingManager()
        {
        }

        private int FirstGenCount { get; set; }
        private bool LoadPreviousGen { get; set; }
        public void Init(GameSettings settings)
        {
            Reset();

            FirstGenCount = settings.FirstGenCount;
            LoadPreviousGen = settings.LoadPreviousGen;
        }

        public void Reset()
        {
            TrainingList = new ObservableCollection<TrainingModel>();
            TrainingStats = new TrainingStats();
            _population = new List<PopulationNode>();

            _previousPopulation = LoadGenerations() ?? new List<PopulationNode>();
        }

        public ObservableCollection<TrainingModel> TrainingList { get; set; } = new ObservableCollection<TrainingModel>();
        public TrainingStats TrainingStats { get; set; } = new TrainingStats();

        private List<PopulationNode> _previousPopulation { get; set; } = new List<PopulationNode>(); 

        private const int GameIterationInPopulation = 100;
        private const int PopulationInNextGeneration = 5;

        private List<PopulationNode> _population = new List<PopulationNode>();
        private readonly ILearner _learner = new HomebrewLearner();

        public void StartTraining(CancellationTokenSource cancelToken, IStrategy strategy)
        {
            // load previous generations
            if (LoadPreviousGen)
                _population = _previousPopulation;

            // TODO : Add check for more than 2 ?
            if (_population == null)
                _population = new List<PopulationNode>();

            TrainingStats.TotalChilds = _population.Count;
            TrainingStats.CurrentGeneration = 1;

            TrainingStats.IterationToDo = GameIterationInPopulation;

            Task.Factory.StartNew(() =>
            {
                // Simulate infinite generations
                while (!cancelToken.IsCancellationRequested)
                {
                    // Initialize generation
                    List<HeuristicFactor> generation;
                    if(TrainingStats.CurrentGeneration == 1 && FirstGenCount > 0)
                        generation = _learner.GetNewGeneration(_population, FirstGenCount);
                    else
                        generation = _learner.GetNewGeneration(_population, PopulationInNextGeneration);

                    // Simulate all new element in generation
                    foreach (var heuristicFactor in generation)
                    {
                        var iteration = 0;
                        var totalScore = 0;
                        TrainingStats.IterationDone = iteration;
                        var generationStat = new StatModel() {MinTile = 2024};

                        // Play X games
                        while (iteration < GameIterationInPopulation && !cancelToken.IsCancellationRequested)
                        {
                            // Initialize the board with the Heuristic Factors of the current generation
                            IBoard board = new OptimizeBoard();
                            BoardHelper.InitializeBoard(board);
                            board.Initialize(heuristicFactor);

                            // Play Game
                            var gameState = State.NotFinished;
                            while (gameState == State.NotFinished && !cancelToken.IsCancellationRequested)
                            {
                                var direction = strategy.GetDirection(board);
                                var goodMove = board.PerformMove(direction);

                                if(goodMove)
                                    // Spawn random values
                                    BoardHelper.AddRandomCell(board);

                                gameState = CheckForWin(board);
                            }

                            // game finished, update stats
                            var score = board.GetScore();
                            totalScore += score;
                            if (score > generationStat.MaxScore)
                                generationStat.MaxScore = score;

                            var maxTile = Heuristics.GetMaxValue(board);
                            if (maxTile > generationStat.MaxTile)
                                generationStat.MaxTile = maxTile;

                            if (maxTile < generationStat.MinTile)
                                generationStat.MinTile = maxTile;

                            iteration++;
                            TrainingStats.IterationDone = iteration;
                        }


                        if (!cancelToken.IsCancellationRequested)
                        {
                            // Add info to next generation (Stats)
                            _population.Add(new PopulationNode()
                            {
                                Heuristic = heuristicFactor,
                                Stat = generationStat
                            });
                            TrainingStats.TotalChilds = _population.Count;



                            // Visual update
                            Application.Current.Dispatcher.Invoke(
                                () =>
                                {
                                    TrainingList.Add(new TrainingModel()
                                    {
                                        AverageScore = totalScore/GameIterationInPopulation,
                                        MaxScore = generationStat.MaxScore,
                                        MaxTile = generationStat.MaxTile,
                                        MinTile = generationStat.MinTile,

                                        LostPenalty = heuristicFactor.LostPenalty,
                                        MonoticityPower = heuristicFactor.MonoticityPower,
                                        MonoticityWeight = heuristicFactor.MonoticityWeight,
                                        EmptyWeigth = heuristicFactor.EmptyWeigth,
                                        FillWeigth = heuristicFactor.FillWeigth,
                                        MergeWeigth = heuristicFactor.MergeWeigth,
                                        SumPower = heuristicFactor.SumPower,
                                        SumWeight = heuristicFactor.SumWeight
                                    
                                    });
                                });

                            // Save Data
                            SaveGenerations();
                        } 
                    }

                    //New generation all tested with stats
                    TrainingStats.CurrentGeneration++;
                }
            }, cancelToken.Token).ContinueWith(task =>
            {
                SaveGenerations();
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

        private const string SaveFileName = "SavedGenerations.txt";
        private void SaveGenerations()
        {
            var totalGen = _population;
            totalGen.AddRange(_previousPopulation ?? new List<PopulationNode>());

            var generationString = JsonConvert.SerializeObject(totalGen);
            File.WriteAllText(SaveFileName, generationString);
        }

        private List<PopulationNode> LoadGenerations()
        {
            if (!File.Exists(SaveFileName)) return null;

            var content = File.ReadAllText(SaveFileName);
            return JsonConvert.DeserializeObject<List<PopulationNode>>(content);
        }

    }
}