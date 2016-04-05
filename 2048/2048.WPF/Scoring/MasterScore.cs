using System;
using _2048.WPF.Model;
using _2048.WPF.Model.Core;

namespace _2048.WPF.Scoring
{
    public class MasterScore : IOptimizedScore, IScore
    {
        private const float SumWeight = 11.0f;
        private const float EmptyWeight = 270.0f;
        private const float MonotonicityPower = 4.0f;
        private const float MonotonicityWeight = 47.0f;
        private const float MergesWeight = 700.0f;

        // X => Line , Y => Collumn

        /// <summary>
        /// Get the Heiristic score of the given board
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public float GetScore(IBoard board)
        {
            var score = 0.0f;

            // for each collumn
            for (var i = 0; i < board.GetSize(); i++)
            {
                // Heuristic for sum of cells
                score -= SumWeight*(float) (Math.Log(GetLineScore(board, i))/Math.Log(2));

                // Heuristic for empty cells count
                score += EmptyWeight*CountEmptyCells(board, i);

                // Heuristic for Monotonicity
                score -= MonotonicityWeight*GetMonotonicity(board, i);

                // Heiristic for cell Merge count
                score += MergesWeight*GetMergeCount(board, i);
            }

            return score;
        }

        public double Score(TreeNode node)
        {
            return GetScore(node.GameModel);
        }

        /// <summary>
        /// Get the total value of all the elements in the given line
        /// </summary>
        /// <param name="board"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private static float GetLineScore(IBoard board, int line)
        {
            var score = 0;

            // foreach line
            for (var i = 0; i < board.GetSize(); i++)
            {
                score += board.GetValue(line, i);
            }
            return score;
        }

        /// <summary>
        /// Get the total count of empty cells in the given line
        /// </summary>
        /// <param name="board"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private static int CountEmptyCells(IBoard board, int line)
        {
            var total = 0;

            // foreach line
            for (var i = 0; i < board.GetSize(); i++)
            {
                if (board.GetValue(line, i) == 0)
                    total++;
            }
            return total;
        }

        /// <summary>
        /// Get the indication of the relative monotonicity of the given line
        /// Value of relative neighboors increments
        /// </summary>
        /// <param name="board"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private static float GetMonotonicity(IBoard board, int line)
        {
            var monotonicityLeft = 0.0f;
            var monotonicityRight = 0.0f;

            for (var i = 1; i < 4; ++i)
            {
                if (board.GetValue(line, i-1) > board.GetValue(line, i))
                {
                    monotonicityLeft +=(float)(
                        Math.Pow((Math.Log(board.GetValue(line, i - 1))/Math.Log(2)), MonotonicityPower)
                        - Math.Pow((Math.Log(board.GetValue(line, i))/Math.Log(2)), MonotonicityPower));
                }
                else {
                    monotonicityRight += (float)(
                        Math.Pow((Math.Log(board.GetValue(line, i)) / Math.Log(2)), MonotonicityPower)
                        - Math.Pow((Math.Log(board.GetValue(line, i -1)) / Math.Log(2)), MonotonicityPower));
                }
            }

            return Math.Min(monotonicityLeft, monotonicityRight);
        }

        /// <summary>
        /// Count the number of cells that can be merge in the given line
        /// </summary>
        /// <param name="board"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        private static int GetMergeCount(IBoard board, int line)
        {
            var merges = 0;
            var previousCellValue = 0;
            var counter = 0;

            for (var i = 0; i < 4; ++i)
            {
                var cellValue = board.GetValue(line,i);

                if (cellValue == 0) continue;

                if (previousCellValue == cellValue)
                {
                    counter++;
                }
                else if (counter > 0)
                {
                    merges += 1 + counter;
                    counter = 0;
                }
                previousCellValue = cellValue;
            }

            if (counter > 0)
            {
                merges += 1 + counter;
            }

            return merges;
        }
    }
}