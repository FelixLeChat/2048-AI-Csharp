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

        public int GetValue(int x, int y)
        {
            return 0;
        }
    }
}