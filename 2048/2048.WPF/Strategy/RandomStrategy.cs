using System;
using _2048.Model;
using _2048.WPF.Game;

namespace _2048.WPF
{
    public class RandomStrategy : IStrategy
    {
        public void Initialize(GameModel model)
        {
        }

        public void Ended(ScoreModel score)
        {
        }

        private readonly Random _random = new Random();
        public Direction GetDirection(GameModel board)
        {
            // creates a number between 1 and 12
            return (Direction)_random.Next((int)Direction.Up, (int)Direction.Right+1);
        }
    }
}