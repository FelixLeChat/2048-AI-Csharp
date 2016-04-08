using System;
using System.Collections.Generic;
using _2048.AI.Heuristics;
using _2048.AI.Learning.Core;

namespace _2048.AI.Learning
{
    public class GeneticMaker : IBastardMaker
    {
        private readonly Random _rand = new Random();
        private const float MutateProbability = 0.1f;

        public HeuristicFactor MakeBastard(List<PopulationNode> parentsInOrgy)
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

            if (_rand.NextDouble() < MutateProbability)
            {
                return RandomWalkMaker.RandomWalk(heuristic, MutateProbability, HeuristicFactor.WeigthIncrementLimit,
                    HeuristicFactor.PowerIncrementLimit);
            }
            else
            {
                return heuristic;
            }

        }
    }
}