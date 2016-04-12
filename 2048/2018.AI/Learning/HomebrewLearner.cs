using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using _2048.AI.Learning.Core;
using _2048.AI.Heuristics;

namespace _2048.AI.Learning
{
    /// <summary>
    /// Strategy:
    ///     * Merge parent
    ///     * Random Walk with best
    ///     * Learning from past
    /// </summary>
    public class HomebrewLearner : ILearner
    {
        public IBastardMaker BastardMaker { get; set; } = new RandomWalkMaker();

        public HomebrewLearner() { }
        public HomebrewLearner(IBastardMaker maker)
        {
            BastardMaker = maker;
        }

        private const int SelectionCount = 5;

        public List<HeuristicFactor> GetNewGeneration(List<PopulationNode> previousGeneration, int populationSize)
        {
            // If no previous generation: generate a random one
            if (previousGeneration == null || !previousGeneration.Any())
            {
                var newGeneration = new List<HeuristicFactor>();
                for (int i = 0; i < populationSize; ++i)
                {
                    newGeneration.Add(HeuristicFactor.GetRandomHeuristic());
                }
                return newGeneration;
            }
            else
            {

                //previousGeneration.Sort();
                //previousGeneration.Reverse();
                //var bestNode = previousGeneration.Take(SelectionCount).ToList();
                var nodeToTake = Math.Min(SelectionCount, previousGeneration.Count);
                var bestNode = previousGeneration.OrderByDescending(x => x.Stat.MaxTile).ThenByDescending(x => x.Stat.MinTile).Take(nodeToTake).ToList();

                var newGeneration = MakeChildrenWithLove(bestNode, populationSize);

                return newGeneration;
            }
        }

        public void SetMakerType(IBastardMaker maker)
        {
            BastardMaker = maker;
        }

        private List<HeuristicFactor> MakeChildrenWithLove(List<PopulationNode> previousGeneration, int populationSize)
        {
            var heuristics = new List<HeuristicFactor>();
            while (populationSize-- > 0)
            {
                heuristics.Add(BastardMaker.MakeBastard(previousGeneration));
            }
            return heuristics;
        }

    }
}