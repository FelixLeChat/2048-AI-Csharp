using System;
using System.Collections.Generic;
using _2048.AI.Model;
using _2048.AI.Model.Core;

namespace _2048.AI.Heuristics
{
    public class Heuristics
    {
        /// <summary>
        /// Definition in vector form of all the directions
        /// </summary>
        private static readonly List<Position> DirectionVectors = new List<Position>()
        {
            new Position() {X=0, Y=-1}, new Position() {X=1,Y=0},
            new Position() {X=0, Y=1}, new Position() {X=-1,Y=0},
        };

        /// <summary>
        /// Get the total value of all the elements in the given line
        /// </summary>
        /// <param name="board"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static double GetLineScore(IBoard board, int line)
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
        public static int CountEmptyCells(IBoard board, int line)
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
        /// <param name="monotonicityPower"></param>
        /// <returns></returns>
        public static double GetMonotonicity(IBoard board, int line, double monotonicityPower)
        {
            double monotonicityLeft = 0.0f;
            double monotonicityRight = 0.0f;

            for (var i = 1; i < 4; ++i)
            {
                if (board.GetValue(line, i - 1) > board.GetValue(line, i))
                {
                    monotonicityLeft += (
                        Math.Pow(ToBase2Exp(board.GetValue(line, i - 1)), monotonicityPower)
                        - Math.Pow(ToBase2Exp(board.GetValue(line, i)), monotonicityPower));
                }
                else {
                    monotonicityRight += (
                        Math.Pow(ToBase2Exp(board.GetValue(line, i)), monotonicityPower)
                        - Math.Pow(ToBase2Exp(board.GetValue(line, i - 1)), monotonicityPower));
                }
            }

            return Math.Min(monotonicityLeft, monotonicityRight);
        }
        public static double GetMonotonicity(IBoard board, double monotonicityPower)
        {
            var size = board.GetSize();
            var total = 0.0;
            for (var i = 0; i < size; i++)
            {
                total += GetMonotonicity(board, i, monotonicityPower);
            }
            return total;
        }

        /// <summary>
        /// Count the number of cells that can be merge in the given line
        /// </summary>
        /// <param name="board"></param>
        /// <param name="line"></param>
        /// <returns></returns>
        public static int GetMergeCount(IBoard board, int line)
        {
            var merges = 0;
            var previousCellValue = 0;
            var counter = 0;

            for (var i = 0; i < 4; ++i)
            {
                var cellValue = board.GetValue(line, i);

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
        /// Get the smootness of the board (relative difference between value of closest neighbourgs)
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static double GetSmoothness(IBoard board)
        {
            double smoothness = 0.0f;
            for (var x = 0; x < board.GetSize(); x++)
            {
                for (var y = 0; y < board.GetSize(); y++)
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
            var newCellX = 0;
            var newCellY = 0;
            bool canContinue;

            var boardSize = board.GetSize();

            // Progress towards the vector direction until an obstacle is found
            do
            {
                canContinue = true;
                var previousX = newCellX;
                var previousY = newCellY;

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
        public static double ToBase2Exp(double value)
        {
            if (value <= 0)
                return 0;
            return (Math.Log(value) / Math.Log(2));
        }

        /// <summary>
        /// Score of the current board
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static int GetScore(IBoard board)
        {
            return board.GetScore();
        }

        /// <summary>
        /// List of all the empty cells on the given grid
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        public static List<Position> GetEmptyCellPositions(IBoard cells)
        {
            var emptyCells = new List<Position>();
            for (var x = 0; x < cells.GetSize(); x++)
            {
                for (var y = 0; y < cells.GetSize(); y++)
                {
                    if (cells.GetValue(x, y) == 0)
                        emptyCells.Add(new Position() { X = x, Y = y });
                }
            }
            return emptyCells;
        }

        /// <summary>
        /// Count of all the empty cell on the grid
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        public static int GetEmptyCellCount(IBoard cells)
        {
            return GetEmptyCellPositions(cells).Count;
        }

        /// <summary>
        /// Get the maximum value of cells in the board
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        public static int GetMaxValue(IBoard cells)
        {
            var max = 0;

            for (var x = 0; x < cells.GetSize(); x++)
            {
                for (var y = 0; y < cells.GetSize(); y++)
                {
                    if (cells.GetValue(x, y) > max)
                        max = cells.GetValue(x, y);
                }
            }
            return max;
        }

        /// <summary>
        /// Get if the game is won
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        public static bool IsWon(IBoard cells)
        {
            var max = GetMaxValue(cells);
            return max >= 2048;
        }

        /// <summary>
        /// Get the monotonicity of the grid (strictly increasing values up/down and Left/right)
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        public static double Monotonicity2(IBoard cells)
        {
            // scores for all four directions
            var totals = new double[] { 0, 0, 0, 0 };
            double currentValue;
            double nextValue;

            // up/down direction
            for (var x = 0; x < 4; x++)
            {
                var current = 0;
                var next = current + 1;

                while (next < 4)
                {
                    while (next < 4 && cells.GetValue(x, next) == 0)
                    {
                        next++;
                    }
                    if (next >= 4) { next--; }

                    currentValue = cells.GetValue(x, current) != 0 ?
                        Math.Log(cells.GetValue(x, current)) / Math.Log(2) :
                        0;

                    nextValue = cells.GetValue(x, next) != 0 ?
                        Math.Log(cells.GetValue(x, next)) / Math.Log(2) :
                        0;

                    if (currentValue > nextValue)
                    {
                        totals[0] += nextValue - currentValue;
                    }
                    else if (nextValue > currentValue)
                    {
                        totals[1] += currentValue - nextValue;
                    }
                    current = next;
                    next++;
                }
            }

            // left/right direction
            for (var y = 0; y < 4; y++)
            {
                var current = 0;
                var next = current + 1;

                while (next < 4)
                {
                    while (next < 4 && cells.GetValue(next, y) == 0)
                    {
                        next++;
                    }
                    if (next >= 4) { next--; }

                    currentValue = cells.GetValue(current, y) != 0 ?
                        Math.Log(cells.GetValue(current, y)) / Math.Log(2) :
                        0;

                    nextValue = cells.GetValue(next, y) != 0 ?
                        Math.Log(cells.GetValue(next, y)) / Math.Log(2) :
                        0;

                    if (currentValue > nextValue)
                    {
                        totals[2] += nextValue - currentValue;
                    }
                    else if (nextValue > currentValue)
                    {
                        totals[3] += currentValue - nextValue;
                    }
                    current = next;
                    next++;
                }
            }
            return Math.Max(totals[0], totals[1]) + Math.Max(totals[2], totals[3]);
        }

        /// <summary>
        /// Count the number of isolated groups
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public static int GetIslandCount(IBoard board)
        {
            var markedCells = GenerateTuple(board);
            var islands = 0;

            for (var x = 0; x < 4; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    if (!markedCells[x][y].Item2)
                    {
                        islands++;
                        Mark(markedCells, board, x, y, board.GetValue(x, y));
                    }
                }
            }

            return islands;
        }

        /// <summary>
        /// Mark cells with the given values that are near
        /// </summary>
        /// <param name="cells"></param>
        /// <param name="board"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="value"></param>
        private static void Mark(IReadOnlyList<Tuple<Position, bool>[]> cells, IBoard board, int x, int y, int value)
        {
            if (cells == null) return;
            var size = board.GetSize();

            if (x < 0 || x >= size || y < 0 || y >= size || board.GetValue(x, y) != value || cells[x][y].Item2) return;

            cells[x][y] = new Tuple<Position, bool>(cells[x][y].Item1, true);

            foreach (var directionVector in DirectionVectors)
            {
                Mark(cells, board, x + directionVector.X, y + directionVector.Y, value);
            }
        }

        /// <summary>
        /// Generate tuple values for marking
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        private static Tuple<Position, bool>[][] GenerateTuple(IBoard cells)
        {
            var tuple = new Tuple<Position, bool>[4][];

            var size = cells.GetSize();

            for (var i = 0; i < size; ++i)
            {
                tuple[i] = new Tuple<Position, bool>[4];
            }

            for (var y = 0; y < size; ++y)
            {
                for (var x = 0; x < size; ++x)
                {
                    tuple[x][y] = new Tuple<Position, bool>(new Position() { X = x, Y = y }, false);
                }
            }

            return tuple;
        }
    }
}