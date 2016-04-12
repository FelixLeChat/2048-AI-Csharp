using System;
using System.Collections.Generic;
using _2048.AI.Heuristics;
using _2048.AI.Learning.Core;
using _2048.AI.Model.Stats;

namespace _2048.AI.Learning
{
    public class ReinforcementMaker : IBastardMaker
    {
        private static readonly Random _rand = new Random();
        private float LearningSpeed { get; set; } = 0.7f;
        private const float MutateProbability = 0.1f;

        public HeuristicFactor MakeBastard(List<PopulationNode> previousGeneration)
        {
            var count = previousGeneration.Count;

            if (count < 2)
            {
                throw new ArgumentException("Must have 2 node to use the reinforcement");
            }

            var firstNode = previousGeneration[_rand.Next(0, previousGeneration.Count)];
            PopulationNode secondNode = null;
            do
            {
                secondNode = previousGeneration[_rand.Next(0, previousGeneration.Count)];
            } while (firstNode == secondNode);


            var reinforceHeuristic = LearnFromChild(firstNode, secondNode);

            if (_rand.NextDouble() < MutateProbability)
            {
                return RandomWalkMaker.RandomInverter(reinforceHeuristic, MutateProbability);
            }
            else
            {
                return reinforceHeuristic;
            }
        }

        private HeuristicFactor LearnFromChild(PopulationNode first, PopulationNode second)
        {
            var firstHeuristic = first.Heuristic;
            var secondHeuristic = second.Heuristic;

            var heuristic = new HeuristicFactor()
            {
                LostPenalty = GetNewValue(first.Stat, firstHeuristic.LostPenalty, second.Stat, secondHeuristic.LostPenalty),
                MonoticityPower = GetNewValue(first.Stat, firstHeuristic.MonoticityPower, second.Stat, secondHeuristic.MonoticityPower),
                MonoticityWeight = GetNewValue(first.Stat, firstHeuristic.MonoticityWeight, second.Stat, secondHeuristic.MonoticityWeight),
                SumPower = GetNewValue(first.Stat, firstHeuristic.SumPower, second.Stat, secondHeuristic.SumPower),
                SumWeight = GetNewValue(first.Stat, firstHeuristic.SumWeight, second.Stat, secondHeuristic.SumWeight),
                MergeWeigth = GetNewValue(first.Stat, firstHeuristic.MergeWeigth, second.Stat, secondHeuristic.MergeWeigth),
                EmptyWeigth = GetNewValue(first.Stat, firstHeuristic.EmptyWeigth, second.Stat, secondHeuristic.EmptyWeigth),
                FillWeigth = GetNewValue(first.Stat, firstHeuristic.FillWeigth, second.Stat, secondHeuristic.FillWeigth),
            };

            return heuristic;
        }

  

        // From the best value (one with better score) we go away from the bad one
        private float GetNewValue(StatModel firstScore, float firstValue, StatModel secondScore, float secondValue)
        {
            bool isFirstBetter = firstScore.CompareTo(secondScore) > 0;

            float newValue = 0;
            if (isFirstBetter)
            {
                // Move from second toward first value
                newValue = firstValue + LearningSpeed * (firstValue - secondValue);
            }
            else
            {
                // Move from first value toward second value
                newValue = secondValue + LearningSpeed * (secondValue - firstValue);
            }
            return newValue;
        }

    }
}