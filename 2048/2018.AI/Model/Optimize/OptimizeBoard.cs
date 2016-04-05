using System;
using System.Diagnostics.Contracts;
using _2018.AI.Enums;
using _2018.AI.Model.Core;
using Board = System.UInt64;

namespace _2018.AI.Model.Optimize
{
    public struct OptimizeBoard : IBoard
    {
        public Board Board { get; set; }

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

        public int GetScore()
        {
            throw new NotImplementedException();
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

        public int GetValue(int x, int y)
        {
            if (x < 0 || x >= GetSize() || y < 0 || y >= GetSize())
            {
                throw new IndexOutOfRangeException("The index in out of range");
            }
            var power = OptimizeBoardHelper.GetValue(Board, x, y);
            return  (power == 0) ? 0 :  1 << OptimizeBoardHelper.GetValue(Board, x, y);

        }

        public void SetValue(int x, int y, int value)
        {
            InsertValue(x, y, (short) Math.Sqrt(value));
        }

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