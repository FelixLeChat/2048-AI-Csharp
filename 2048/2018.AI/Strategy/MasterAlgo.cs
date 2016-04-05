using _2018.AI.Enums;
using _2048.Model;

namespace _2018.AI.Strategy
{
    public class MasterAlgo : IStrategy
    {
        public void Initialize(GameModel model)
        {
        }

        public Direction GetDirection(GameModel model)
        {
            return Direction.Up;
        }
    }
}