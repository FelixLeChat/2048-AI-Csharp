using _2048.AI.Enums;
using _2048.AI.Model.Core;

namespace _2048.AI.Strategy
{
    public class MasterAlgo : IStrategy
    {
        public Direction GetDirection(IBoard board)
        {
            return Direction.Up;
        }
    }
}