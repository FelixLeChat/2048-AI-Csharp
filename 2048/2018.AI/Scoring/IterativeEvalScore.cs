using _2048.AI.Model;
using _2048.AI.Model.Core;

namespace _2048.AI.Scoring
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
            var emptyCells = Heuristics.Heuristics.GetEmptyCellCount(cells);

            var result = 0.0;
            result += Heuristics.Heuristics.GetSmoothness(cells)*SmoothWeight;
            result += Heuristics.Heuristics.Monotonicity2(cells)*Mono2Weight;
            result += emptyCells*EmptyWeight;
            result += Heuristics.Heuristics.GetMaxValue(cells) * MaxWeight;
            return result;
        }
    }
}