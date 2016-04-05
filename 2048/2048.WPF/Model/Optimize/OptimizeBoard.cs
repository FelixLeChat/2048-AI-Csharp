using System;
using _2048.WPF.Game;
using _2048.WPF.Model.Core;
using Board = System.UInt64;

namespace _2048.WPF.Model
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
            return Board != result;
        }

        public int GetSize()
        {
            return 4;
        }

        public int GetValue(int x, int y)
        {
            if (x < 0 || x >= GetSize() || y < 0 || y >= GetSize())
            {
                throw new ArgumentOutOfRangeException();
            }

            int tmp = 0;
            // Shift the wanted nyblet to the last column
            switch (x)
            {
                case 0:
                    tmp = tmp >> 12;
                    break;
                case 1:
                    tmp = tmp >> 8;
                    break;
                case 2:
                    tmp = tmp >> 4;
                    break;

            }

            //Shift to the last row
            switch (y)
            {
                case 0:
                    tmp = tmp >> 48;
                    break;
                case 1:
                    tmp = tmp >> 32;
                    break;
                case 2:
                    tmp = tmp >> 16;
                    break;
            }
            tmp = tmp & 0xf;

            var result = 0x2 << tmp;

            return result;
        }
    }
}