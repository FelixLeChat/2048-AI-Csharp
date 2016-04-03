using System;
using System.Collections.Generic;
using System.Linq;
using _2048.Model;
using _2048.WPF.Model;

namespace _2048.WPF.Helper
{
    public class CellHelper
    {
        /// <summary>
        /// List of all the empty cells on the given grid
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        public static List<Position> GetEmptyCellPositions(Cell[][] cells)
        {
            var emptyCells = new List<Position>();
            foreach (var col in cells)
            {
                foreach (var cell in col)
                {
                    if (cell.Value == 0)
                        emptyCells.Add(new Position() { X = cell.X, Y = cell.Y });
                }
            }

            return emptyCells;
        }

        /// <summary>
        /// Count of all the empty cell on the grid
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        public static int GetEmptyCellCount(Cell[][] cells)
        {
            return GetEmptyCellPositions(cells).Count;
        }

        /// <summary>
        /// Get the cell with the maximum value (or one of those with the max value)
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        public static Cell GetMaxValue(Cell[][] cells)
        {
            var maxCell = new Cell(0,0){Value = 0};

            foreach (var cell in from col in cells from cell in col where cell.Value > maxCell.Value select cell)
            {
                maxCell = cell;
            }
            return maxCell;
        }


        #region Javascript Translation
        // measures how smooth the grid is (as if the values of the pieces
        // were interpreted as elevations). Sums of the pairwise difference
        // between neighboring tiles (in log space, so it represents the
        // number of merges that need to happen before they can merge). 
        // Note that the pieces can be distant
        private static readonly List<Position> DirectionVectors = new List<Position>()
        {
            new Position() {X=0, Y=-1}, new Position() {X=1,Y=0},
            new Position() {X=0, Y=1}, new Position() {X=-1,Y=0},
        }; 
        public static double GetSmoothness(Cell[][] cells)
        {
            var smoothness = 0.0;
            for (var x = 0; x < 4; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    var cell = cells[x][x];
                    if (cell.Value != 0)
                    {
                        var value = Math.Log(cell.Value) / Math.Log(2);

                        for (var direction = 1; direction <= 2; direction++)
                        {
                            var vector = DirectionVectors[direction];
                            var targetCell = FindFartestPosition(cells, cell, vector).Item2;

                            if (targetCell.Value != 0)
                            {
                                var targetValue = Math.Log(targetCell.Value) / Math.Log(2);
                                smoothness -= Math.Abs(value - targetValue);
                            }
                        }
                    }
                }
            }
            return smoothness;
        }

        private static Tuple<Cell,Cell> FindFartestPosition(Cell[][] cells, Cell cell, Position vector)
        {
            var previous = cell;
            var newCell = cell;

            // Progress towards the vector direction until an obstacle is found
            do
            {
                previous = newCell;

                // next position
                var x = previous.X + vector.X;
                var y = previous.Y + vector.Y;

                // Check X validity
                if (x >= 0 && x <= 3)
                {
                    // check Y validity
                    if (y >= 0 && y <= 3)
                    {
                        newCell = cells[x][y];
                    }
                }
            } while (newCell.Value == 0);

            return new Tuple<Cell, Cell>(previous, newCell);
        }

        // measures how monotonic the grid is. This means the values of the tiles are strictly increasing
        // or decreasing in both the left/right and up/down directions
        public static double Monotonicity2(Cell[][] cells)
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
                    while (next < 4 && cells[x][next].Value == 0)
                    {
                        next++;
                    }
                    if (next >= 4) { next--; }

                    currentValue = cells[x][current].Value != 0 ?
                        Math.Log(cells[x][current].Value) / Math.Log(2) :
                        0;

                    nextValue = cells[x][next].Value != 0 ?
                        Math.Log(cells[x][next].Value) / Math.Log(2) :
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
                    while ( next<4 && cells[next][y].Value == 0)
                    {
                        next++;
                    }
                    if (next>=4) { next--; }

                    currentValue = cells[current][y].Value != 0 ?
                        Math.Log(cells[current][y].Value) / Math.Log(2) :
                        0;

                    nextValue = cells[next][y].Value != 0 ?
                        Math.Log(cells[next][y].Value) / Math.Log(2) :
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
        /// <param name="cells"></param>
        /// <returns></returns>
        public static int GetIslandCount(Cell[][] cells)
        {
            var markedCells = GenerateTuple(cells);
            var islands = 0;

            for (var x = 0; x < 4; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    if (!markedCells[x][y].Item2)
                    {
                        islands++;
                        Mark(markedCells,x, y, markedCells[x][y].Item1.Value);
                    }
                }
            }

            return islands;
        }
        
        private static Tuple<Cell,bool>[][] Mark(Tuple<Cell, bool>[][] cells, int x, int y, int value)
        {
            if (x >= 0 && x <= 3 && y >= 0 && y <= 3 && cells[x][y].Item1.Value == value && !cells[x][y].Item2)
            {
                cells[x][y] = new Tuple<Cell, bool>(cells[x][y].Item1, true);

                foreach (var directionVector in DirectionVectors)
                {
                    Mark(cells, x + directionVector.X, y + directionVector.Y, value);
                }
            }

            return cells;
        }

        private static Tuple<Cell, bool>[][] GenerateTuple(Cell[][] cells)
        {
            var tuple = new Tuple<Cell, bool>[4][];

            for (var i = 0; i < 4; ++i)
            {
                tuple[i] = new Tuple<Cell, bool>[4];
            }

            for (var y = 0; y < 4; ++y)
            {
                for (var x = 0; x < 4; ++x)
                {
                    tuple[x][y] = new Tuple<Cell, bool>(cells[x][y], false);
                }
            }

            return tuple;
        }
        #endregion
    }
}