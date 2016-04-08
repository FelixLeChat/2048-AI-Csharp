using System;
using _2018.AI.Model;
using _2018.AI.Model.Core;
using static _2018.AI.Heuristics.Heuristics;

namespace _2018.AI.Scoring
{
    public class IterativeEvalScore :IScore
    {
        public double Score(TreeNode node)
        {
            return Eval(node.Board);
        }

        private const double SmoothWeight = 1.0;
        private const double Mono2Weight = 1.5;
        private const double EmptyWeight = 2.0;
        private const double MaxWeight = 1.0;

        private static double Eval(IBoard cells)
        {
            var emptyCells = GetEmptyCellCount(cells);

            var result = 0.0;
            result += GetSmoothness(cells)*SmoothWeight;
            result += Monotonicity2(cells)*Mono2Weight;
            result += emptyCells*EmptyWeight;
            result += GetMaxValue(cells) * MaxWeight;
            return result;
        }
    }
}