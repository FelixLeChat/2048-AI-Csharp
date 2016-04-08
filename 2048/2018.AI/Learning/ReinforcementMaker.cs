using System.Collections.Generic;
using _2048.AI.Heuristics;
using _2048.AI.Learning.Core;

namespace _2048.AI.Learning
{
    public class ReinforcementMaker : IBastardMaker
    {
        public HeuristicFactor MakeBastard(List<PopulationNode> previousGeneration)
        {
            return HeuristicFactor.GetRandomHeuristic();
        }
    }
}