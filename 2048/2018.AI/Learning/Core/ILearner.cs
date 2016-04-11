using System.Collections.Generic;
using _2048.AI.Heuristics;

namespace _2048.AI.Learning.Core
{
    /// <summary>
    /// From a current generation, will return the HeuristicFactor for the next generation to tests
    /// </summary>
    public interface ILearner
    {
        List<HeuristicFactor> GetNewGeneration(List<PopulationNode> previousGeneration, int populationSize);

        void SetMakerType(IBastardMaker maker);
    }
}