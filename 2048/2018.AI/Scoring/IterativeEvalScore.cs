using System;
using _2018.AI.Model;
using _2018.AI.Model.Core;
using _2048.Model;

namespace _2018.AI.Scoring
{
    public class IterativeEvalScore :IScore
    {
        public double Score(TreeNode node)
        {
            return Eval(node.Board);
        }

        private const double SmoothWeight = 0.1;
        private const double Mono2Weight = 1.0;
        private const double EmptyWeight = 2.7;
        private const double MaxWeight = 1.0;

        private static double Eval(IBoard cells)
        {
            var emptyCells = Helper.CellHelper.GetEmptyCellCount(cells);

            var result = 0.0;
            result += MasterScore.GetSmoothness(cells)*SmoothWeight;
            result += Helper.CellHelper.Monotonicity2(cells)*Mono2Weight;
            result += Math.Log(emptyCells)*EmptyWeight;
            result += Helper.CellHelper.GetMaxValue(cells) * MaxWeight;
            return result;
        }
    }
}