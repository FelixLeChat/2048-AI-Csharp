using System.Collections.Generic;
using _2048.AI.Heuristics;

namespace _2048.AI.Learning.Core
{
    public interface IBastardMaker
    {
        HeuristicFactor MakeBastard(List<PopulationNode> previousGeneration);
    }
}