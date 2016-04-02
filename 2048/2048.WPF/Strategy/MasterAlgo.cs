using _2048.Model;
using _2048.WPF.Game;

namespace _2048.WPF
{
    public class MasterAlgo : IStrategy
    {
        public void Initialize(GameModel model)
        {
        }

        public void Ended(ScoreModel score)
        {
        }

        public Direction GetDirection(GameModel model)
        {
            return Direction.Up;
        }
    }
}