using _2018.AI.Model.Core;
using _2018.AI.Model.Optimize;
using _2018.AI.Strategy;

namespace _2048.Console
{
    class Program
    {
        private static IBoard CurrentBoard { get; set; } = new OptimizeBoard();
        private static IStrategy CurrentStrategy { get; set; } = new MasterAlgo();


        static void Main(string[] args)
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
                    case "id":
                        CurrentStrategy = new IterativeDeepening();
                        break;
                }
            }

        }
    }
}
