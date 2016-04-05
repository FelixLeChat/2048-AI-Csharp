using System;
using _2018.AI.Enums;
using _2018.AI.Model.Core;
using _2048.Model;

namespace _2018.AI.Strategy
{
    public class RandomStrategy : IStrategy
    {
        public void Initialize(GameModel model)
        {
        }

        private readonly Random _random = new Random();
        public Direction GetDirection(IBoard board)
        {
            // creates a number between 1 and 12
            return (Direction)_random.Next((int)Direction.Up, (int)Direction.Right+1);
        }
    }
}