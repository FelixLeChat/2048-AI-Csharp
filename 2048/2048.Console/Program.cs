using System;
using System.Diagnostics;
using _2048.AI.Helper;
using _2048.AI.Heuristics;
using _2048.AI.Model.Core;
using _2048.AI.Model.Optimize;
using _2048.AI.Strategy;
using _2048.Model;

namespace _2048.Console
{
    class Program
    {
        private static IBoard CurrentBoard { get; set; } = new OptimizeBoard();
        private static IStrategy CurrentStrategy { get; set; } = new IterativeDeepening();
        private static bool Restart { get; set; } = true;


        static void Main(string[] args)
        {
            // Initialise game settings
            //Initialize(args);

            System.Console.WriteLine("************************************************************");
            System.Console.WriteLine("*      Bienvenue dans le jeu de 2048 version console       *");
            System.Console.WriteLine("************************************************************" + Environment.NewLine);
            while (true)
            {
                PlayGame();
            }

        }

        private static readonly Stopwatch GameTimer = new Stopwatch();
        private static void PlayGame()
        {
            var done = false;
            var notMovedCount = 0;
            IBoard board = new OptimizeBoard();
            board.Initialize();
            BoardHelper.InitializeBoard(board);

            GameTimer.Restart();

            while (!done)
            {
                var direction = CurrentStrategy.GetDirection(board);

                // Move
                if (board.PerformMove(direction))
                {
                    notMovedCount = 0;

                    // Spawn random values
                    BoardHelper.AddRandomCell(board);
                }
                else if(Heuristics.GetEmptyCellCount(board) == 0)
                    notMovedCount++;

                // Check for win/lost
                if (notMovedCount > 6 || Heuristics.IsWon(board))
                    done = true;
            }
            GameTimer.Stop();

            // Write result
            var maxTile = Heuristics.GetMaxValue(board);
            var score = board.GetScore();
            var time = GameTimer.ElapsedMilliseconds;
            
            System.Console.WriteLine("MaxTile = {0}, Score = {1}, Time = {2} ", maxTile, score, time);
        }


        private static void Initialize(string[] args)
        {
            if (args.Length == 1)
            {
                switch (args[0].ToLower())
                {
                    case "dfs":
                        CurrentStrategy = new DeptFirstStrategy();
                        break;
                    case "random":
                        CurrentStrategy = new RandomStrategy();
                        break;
                    // Iterative Deepening
                    case "id":
                        CurrentStrategy = new IterativeDeepening();
                        break;
                }
            }

            if (args.Length == 2)
            {
                switch (args[0].ToLower())
                {
                    case "restart":
                        Restart = true;
                        break;
                    default:
                        Restart = false;
                        break;
                }
            }
        }
    }
}
