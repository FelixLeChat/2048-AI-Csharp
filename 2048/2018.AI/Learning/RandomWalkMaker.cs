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

        private static Func<float, float> RandomHelperDel;

        public HeuristicFactor MakeBastard(List<PopulationNode> previousGeneration)
        {
            var nodeToMutate = previousGeneration[_rand.Next(0, previousGeneration.Count)];

            return RandomWalk(nodeToMutate.Heuristic, MutateProbability, HeuristicFactor.WeigthIncrementLimit, HeuristicFactor.PowerIncrementLimit);
        }

        public static HeuristicFactor RandomWalk(HeuristicFactor nodeToMutate, float probability, float weightLimit, float powerLimit)
        {
            if (probability < 0.01f || (weightLimit == 0 && powerLimit == 0))
            {
                throw new ArgumentException("Need some randomness to do random walk");
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
            float incrementValue = (float)((_rand.NextDouble() - 0.5f) * 2 * limit);
            return _rand.NextDouble() < probability ? initialValue + incrementValue : initialValue;
        }


        public static HeuristicFactor RandomInverter(HeuristicFactor nodeToMutate, float probability)
        {
            if (probability < 0.01f)
            {
                throw new ArgumentException("Need some randomness to do random walk");
            }

            Func<float, float> fun = (h) => InverseHelper(h, probability);

            var result = RandomizeHelper(nodeToMutate, fun);
            return result;
        }

        public static HeuristicFactor RandomizeHelper(HeuristicFactor nodeToMutate, Func<float, float> transformer)
        {
            HeuristicFactor heuristic;
            do
            {
                heuristic = new HeuristicFactor()
                {
                    LostPenalty = transformer(nodeToMutate.LostPenalty),
                    MonoticityPower = transformer(nodeToMutate.MonoticityPower),
                    MonoticityWeight = transformer(nodeToMutate.MonoticityWeight),
                    SumPower = transformer(nodeToMutate.SumPower),
                    SumWeight = transformer(nodeToMutate.SumWeight),
                    MergeWeigth = transformer(nodeToMutate.MergeWeigth),
                    EmptyWeigth = transformer(nodeToMutate.EmptyWeigth),
                    FillWeigth = transformer(nodeToMutate.FillWeigth),
                };
            } while (heuristic.Equals(nodeToMutate));

            return heuristic;
        }

        private static float InverseHelper(float initialValue, float probability)
        {
            return _rand.NextDouble() < probability ? initialValue : -initialValue;
        }
    }
}