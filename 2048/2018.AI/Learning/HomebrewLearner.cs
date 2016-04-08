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
        IBastardMaker Maker = new ReinforcementMaker();
        private const int SelectionCount = 2;

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
                previousGeneration.Sort();
                var bestNode = previousGeneration.Take(SelectionCount).ToList();

                var newGeneration = MakeChildrenWithLove(bestNode, populationSize);

                return newGeneration;
            }
        }

        private List<HeuristicFactor> MakeChildrenWithLove(List<PopulationNode> previousGeneration, int populationSize)
        {
            var heuristics = new List<HeuristicFactor>();
            while (populationSize-- > 0)
            {
                heuristics.Add(Maker.MakeBastard(previousGeneration));
            }
            return heuristics;
        }



    }
}