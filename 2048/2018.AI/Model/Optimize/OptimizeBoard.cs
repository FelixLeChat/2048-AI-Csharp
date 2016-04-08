using System;
using _2048.AI.Enums;
using _2048.AI.Helper;
using _2048.AI.Heuristics;
using _2048.AI.Model.Core;
using _2048.AI.Scoring;
using Board = System.UInt64;
using Type = _2048.AI.Enums.Type;

namespace _2048.AI.Model.Optimize
{
    public struct OptimizeBoard : IBoard
    {
        public Board Board { get; set; }
        private IOptimizedScore Scoring { get; set; }

        /// <summary>
        /// Perform move and return if it had changed something
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public bool PerformMove(Direction direction)
        {
            var result = OptimizeBoardHelper.PerformMove(Board, direction);
            var hadChange = result != Board;
            Board = result;
            return hadChange;
        }

        public void Initialize()
        {
            OptimizeBoardHelper.InitLookupTable();
            OptimizeScorer.InitLookupTable(HeuristicFactor.GetSomeHeuristic());
        }

        public double GetHeuristicEvaluation()
        {
            return OptimizeScorer.EvaluateHeuristic(Board) + OptimizeScorer.EvaluateHeuristic(BitArrayHelper.Transpose(Board));
        }

        public int GetScore()
        {
            return (int) OptimizeBoardHelper.GetScore(Board);
        }

        public int GetSize()
        {
            return 4;
        }

        public IBoard GetCopy()
        {
            OptimizeBoard copy = new OptimizeBoard();
            copy.Board = Board;
            return copy;
        }

        public IBoard GetCopy(Board board)
        {
            OptimizeBoard copy = new OptimizeBoard();
            copy.Board = board;
            return copy;
        }

        public ulong GetBitArrayRepresentation()
        {
            return Board;
        }

        public int CountEmpty()
        {
            return BitArrayHelper.CountEmpty(Board);
        }

        public int GetValue(int x, int y)
        {
            if (x < 0 || x >= GetSize() || y < 0 || y >= GetSize())
            {
                throw new IndexOutOfRangeException("The index in out of range");
            }
            var power = OptimizeBoardHelper.GetValue(Board, x, y);
            return  (power == 0) ? 0 :  1 << OptimizeBoardHelper.GetValue(Board, x, y);

        }

        /// <summary>
        /// Insert x^2 tile value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="value"></param>
        public void SetValue(int x, int y, int value)
        {
            var result = 0;
            while ((value >>= 1) > 0) result++;
            InsertValue(x, y, (short) result);
        }

        /// <summary>
        /// Insert a tile value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="value"></param>
        public void InsertValue(int x, int y, short value)
        {
            if (x < 0 || x >= GetSize() || y < 0 || y >= GetSize())
            {
                throw new IndexOutOfRangeException("The index in out of range");
            }
            if (value > 0xff)
            {
                throw new ArgumentOutOfRangeException("The value must fit in a nyblet");
            }

            Board = OptimizeBoardHelper.InsertTile(Board, x, y, value);
        }

        public override string ToString()
        {
           return  OptimizeBoardHelper.ToString(Board);
        }
    }
}