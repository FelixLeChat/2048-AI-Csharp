using _2018.AI.Enums;
using _2018.AI.Model.Core;
using _2048.Model;

namespace _2018.AI.Strategy
{
    public class MasterAlgo : IStrategy
    {
        public Direction GetDirection(IBoard board)
        {
            return Direction.Up;
        }
    }
}