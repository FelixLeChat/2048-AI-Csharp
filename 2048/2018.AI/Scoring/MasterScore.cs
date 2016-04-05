using System;
using System.Collections.Generic;
using _2018.AI.Model;
using _2018.AI.Model.Core;

namespace _2018.AI.Scoring
{
    public class MasterScore : IOptimizedScore, IScore
    {
        private const float SumWeight = 11.0f;
        private const float EmptyWeight = 270.0f;
        private const float MonotonicityPower = 4.0f;
        private const float MonotonicityWeight = 47.0f;
        private const float MergesWeight = 700.0f;
        private const float SmoothnessWeight = 200.0f;

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
                score += SumWeight* ToBase2Exp(GetLineScore(board, i));

                // Heuristic for empty cells count
                score += EmptyWeight*CountEmptyCells(board, i);

                // Heuristic for Monotonicity
                score += MonotonicityWeight*GetMonotonicity(board, i);

                // Heuristic for cell Merge count
                score += MergesWeight*GetMergeCount(board, i);
            }

            // Heuritic for smoothness
            score += GetSmoothness(board)*SmoothnessWeight;

            return score;
        }

        /// <summary>
        /// Scoring method for TreeNode Based Strategy
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
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
                        Math.Pow(ToBase2Exp(board.GetValue(line, i - 1)), MonotonicityPower)
                        - Math.Pow(ToBase2Exp(board.GetValue(line, i)), MonotonicityPower));
                }
                else {
                    monotonicityRight += (float)(
                        Math.Pow(ToBase2Exp(board.GetValue(line, i)), MonotonicityPower)
                        - Math.Pow(ToBase2Exp(board.GetValue(line, i - 1)), MonotonicityPower));
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

        /// <summary>
        /// Definition in vector form of all the directions
        /// </summary>
        private static readonly List<Position> DirectionVectors = new List<Position>()
        {
            new Position() {X=0, Y=-1}, new Position() {X=1,Y=0},
            new Position() {X=0, Y=1}, new Position() {X=-1,Y=0},
        };

        /// <summary>
        /// Get the smootness of the board (relative difference between value of closest neighbourgs)
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static float GetSmoothness(IBoard board)
        {
            var smoothness = 0.0f;
            for (var x = 0; x < board.GetSize(); x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    var val = board.GetValue(x, y);
                    if (val != 0)
                    {
                        var value = ToBase2Exp(val);

                        for (var direction = 1; direction <= 2; direction++)
                        {
                            var vector = DirectionVectors[direction];
                            var targetCellValue = FindFartestCellValue(board, x, y, vector);

                            if (targetCellValue != 0)
                            {
                                var targetValue = ToBase2Exp(targetCellValue);
                                smoothness -= Math.Abs(value - targetValue);
                            }
                        }
                    }
                }
            }
            return smoothness;
        }

        /// <summary>
        /// Find the next populated cell in the direction of the given vector
        /// </summary>
        /// <param name="board"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        private static int FindFartestCellValue(IBoard board, int x, int y, Position vector)
        {
            var previousX = 0;
            var previousY = 0;

            var newCellX = 0;
            var newCellY = 0;
            var canContinue = true;

            var boardSize = board.GetSize();

            // Progress towards the vector direction until an obstacle is found
            do
            {
                canContinue = true;
                previousX = newCellX;
                previousY = newCellY;

                // next position
                newCellX = previousX + vector.X;
                newCellY = previousY + vector.Y;

                // Check X validity
                if (x >= 0 && x < boardSize)
                {
                    // check Y validity
                    if (y >= 0 && y < boardSize)
                    {
                        newCellX = x;
                        newCellY = y;
                    }
                    else
                        canContinue = false;
                }
                else
                    canContinue = false;
            } while (board.GetValue(newCellX, newCellY) == 0 && canContinue);

            return board.GetValue(newCellX, newCellY);
        }

        /// <summary>
        /// Get the base 2 exponent represented by the given value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static float ToBase2Exp(float value)
        {
            return (float)(Math.Log(value) / Math.Log(2));
        }
    }
}