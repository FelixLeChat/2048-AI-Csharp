using System;
using System.Collections.Generic;
using _2018.AI.Model;
using _2018.AI.Model.Core;
using _2048.Model;

namespace _2018.AI.Helper
{
    public class CellHelper
    {
        /// <summary>
        /// List of all the empty cells on the given grid
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        public static List<Position> GetEmptyCellPositions(IBoard cells)
        {
            var emptyCells = new List<Position>();
            for (int x = 0; x < cells.GetSize(); x++)
            {
                for (int y = 0; y < cells.GetSize(); y++)
                {
                    if(cells.GetValue(x,y)== 0)
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
        /// Get the cell with the maximum value (or one of those with the max value)
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

        // measures how monotonic the grid is. This means the values of the tiles are strictly increasing
        // or decreasing in both the left/right and up/down directions
        public static double Monotonicity2(IBoard cells)
        {
            // scores for all four directions
            var totals = new double[4] {0,0,0,0};
            var currentValue = 0.0;
            var nextValue = 0.0;

            // up/down direction
            for (var x = 0; x < 4; x++)
            {
                var current = 0;
                var next = current + 1;

                while (next < 4)
                {
                    while (next < 4 && cells.GetValue(x,next) == 0)
                    {
                        next++;
                    }
                    if (next >= 4) { next--; }

                    currentValue = cells.GetValue(x,current) != 0 ?
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
            for (var y = 0; y<4; y++) {
                var current = 0;
                var next = current + 1;

                while ( next<4 ) {
                    while ( next<4 && cells.GetValue(next,y) == 0)
                    {
                        next++;
                    }
                    if (next>=4) { next--; }

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
                    else if (nextValue > currentValue) {
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
                        Mark(markedCells, board,x, y, board.GetValue(x,y));
                    }
                }
            }

            return islands;
        }

        private static readonly List<Position> DirectionVectors = new List<Position>()
        {
            new Position() {X=0, Y=-1}, new Position() {X=1,Y=0},
            new Position() {X=0, Y=1}, new Position() {X=-1,Y=0},
        };
        private static Tuple<Position, bool>[][] Mark(Tuple<Position, bool>[][] cells, IBoard board, int x, int y, int value)
        {
            var size = board.GetSize();

            if (x >= 0 && x < size && y >= 0 && y < size && board.GetValue(x,y) == value && !cells[x][y].Item2)
            {
                cells[x][y] = new Tuple<Position, bool>(cells[x][y].Item1, true);

                foreach (var directionVector in DirectionVectors)
                {
                    Mark(cells, board, x + directionVector.X, y + directionVector.Y, value);
                }
            }

            return cells;
        }

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
                    tuple[x][y] = new Tuple<Position, bool>(new Position() {X = x, Y = y}, false);
                }
            }

            return tuple;
        }
    }
}