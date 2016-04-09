using System;
using System.Collections.Generic;
using _2048.AI.Heuristics;
using _2048.AI.Learning.Core;

namespace _2048.AI.Learning
{
    public class RandomWalkMaker : IBastardMaker
    {
        private static readonly Random _rand = new Random();
        private const float MutateProbability = 0.5f;

        public HeuristicFactor MakeBastard(List<PopulationNode> previousGeneration)
        {
            var nodeToMutate = previousGeneration[_rand.Next(0, previousGeneration.Count)];

            return RandomWalk(nodeToMutate.Heuristic, MutateProbability, HeuristicFactor.WeigthIncrementLimit, HeuristicFactor.PowerIncrementLimit);
        }

        public static HeuristicFactor RandomWalk(HeuristicFactor nodeToMutate, float probability, float weightLimit, float powerLimit)
        {
            if(probability < 0.01f || (weightLimit == 0 && powerLimit == 0))
            {
                throw  new ArgumentException("Need some randomness to do random wals");
            }

            HeuristicFactor heuristic;

            do
            {
                heuristic = new HeuristicFactor()
                {
                    LostPenalty = RandomHelper(nodeToMutate.LostPenalty, weightLimit, probability),
                    MonoticityPower = RandomHelper(nodeToMutate.MonoticityPower, powerLimit, probability),
                    MonoticityWeight = RandomHelper(nodeToMutate.MonoticityWeight, weightLimit, probability),
                    SumPower = RandomHelper(nodeToMutate.SumPower, powerLimit, probability),
                    SumWeight = RandomHelper(nodeToMutate.SumWeight, weightLimit, probability),
                    MergeWeigth = RandomHelper(nodeToMutate.MergeWeigth, weightLimit, probability),
                    EmptyWeigth = RandomHelper(nodeToMutate.EmptyWeigth, weightLimit, probability),
                    FillWeigth = RandomHelper(nodeToMutate.FillWeigth, weightLimit, probability),
                };
            } while (heuristic.Equals(nodeToMutate));

            return heuristic;
        }

        private static float RandomHelper(float initialValue, float limit, float probability)
        {
            float incrementValue = (float) ((_rand.NextDouble() - 0.5f)*2*limit);
            return _rand.NextDouble() < probability ? initialValue + incrementValue : initialValue;
        }
    }
}