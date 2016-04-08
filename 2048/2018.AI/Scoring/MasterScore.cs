using _2048.AI.Model;
using _2048.AI.Model.Core;

namespace _2048.AI.Scoring
{
    public class MasterScore : IOptimizedScore, IScore
    {
        private const double SumWeight = 11.0f;
        private const double EmptyWeight = 270.0f;
        private const double MonotonicityPower = 4.0f;
        private const double MonotonicityWeight = 47.0f;
        private const double MergesWeight = 700.0f;
        private const double SmoothnessWeight = 200.0f;

        /// <summary>
        /// Get the Heiristic score of the given board
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public double GetScore(IBoard board)
        {
            double score = 0.0f;

            // for each collumn
            for (var i = 0; i < board.GetSize(); i++)
            {
                // Heuristic for sum of cells
                score += SumWeight* Heuristics.Heuristics.ToBase2Exp(Heuristics.Heuristics.GetLineScore(board, i));

                // Heuristic for empty cells count
                score += EmptyWeight*Heuristics.Heuristics.CountEmptyCells(board, i);

                // Heuristic for Monotonicity
                score += MonotonicityWeight*Heuristics.Heuristics.GetMonotonicity(board, i, MonotonicityPower);

                // Heuristic for cell Merge count
                score += MergesWeight*Heuristics.Heuristics.GetMergeCount(board, i);
            }

            // Heuritic for smoothness
            score += Heuristics.Heuristics.GetSmoothness(board)*SmoothnessWeight;

            return score;
        }

        /// <summary>
        /// Scoring method for TreeNode Based Strategy
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public double Score(TreeNode node)
        {
            return GetScore(node.Board);
        }
    }
}