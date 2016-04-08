using System;
using System.Collections.Generic;
using _2048.AI.Learning.Core;
using System.Linq;
using _2048.AI.Heuristics;

namespace _2048.AI.Learning
{
    /// <summary>
    /// Strategy:
    ///     * Merge parent
    ///     * Random Walk with best
    ///     * Learning from past
    /// </summary>
    public class HomebrewLearner
    {
        private readonly Random _rand = new Random();
        private const int SelectionCount = 2;
        private const int PopulationCount = 5;
        private const int MutateProbability = 10;

        //public List<PopulationNode> GetNewGeneration(List<PopulationNode> previousGeneration)
        //{
        //    previousGeneration.Sort();
        //    var bestNode = previousGeneration.Take(SelectionCount);
        //}

        //private List<PopulationNode> MakeChildrenWithLove(List<PopulationNode> previousGeneration)
        //{
        //    int childToSpawn = PopulationCount - previousGeneration.Count;
        //    previousGeneration.Add(Merge(previousGeneration));
        //}

        private PopulationNode Merge(List<PopulationNode> parentsInOrgy)
        {
            
            int count = parentsInOrgy.Count;

            var heuristic = new HeuristicFactor()
            {
                LostPenalty = parentsInOrgy[_rand.Next(0, count)].Heuristic.LostPenalty,
                MonoticityPower = parentsInOrgy[_rand.Next(0, count)].Heuristic.MonoticityPower,
                MonoticityWeight = parentsInOrgy[_rand.Next(0, count)].Heuristic.MonoticityWeight,
                SumPower = parentsInOrgy[_rand.Next(0, count)].Heuristic.SumPower,
                SumWeight = parentsInOrgy[_rand.Next(0, count)].Heuristic.SumWeight,
                MergeWeigth = parentsInOrgy[_rand.Next(0, count)].Heuristic.MergeWeigth,
                EmptyWeigth = parentsInOrgy[_rand.Next(0, count)].Heuristic.EmptyWeigth,
                FillWeigth = parentsInOrgy[_rand.Next(0, count)].Heuristic.FillWeigth,
            };

            return new PopulationNode()
            {
                Heuristic = heuristic
            };
        }

        //private PopulationNode MakeBastard(PopulationNode nodeToMutate)
        //{
        //    var heuristic = new HeuristicFactor()
        //    {
        //        LostPenalty =_rand.Next(0, 100) < MutateProbability ? nodeToMutate.Heuristic.LostPenalty +  : nodeToMutate.Heuristic.LostPenalty,
        //        MonoticityPower = nodeToMutate.Heuristic.MonoticityPower,
        //        MonoticityWeight = nodeToMutate.Heuristic.MonoticityWeight,
        //        SumPower = nodeToMutate.Heuristic.SumPower,
        //        SumWeight = nodeToMutate.Heuristic.SumWeight,
        //        MergeWeigth = nodeToMutate.Heuristic.MergeWeigth,
        //        EmptyWeigth = nodeToMutate.Heuristic.EmptyWeigth,
        //        FillWeigth = nodeToMutate.Heuristic.FillWeigth,
        //    };

        //    return new PopulationNode()
        //    {
        //        Heuristic = heuristic
        //    };
        //}

    }
}