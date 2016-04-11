using System;
using _2048.AI.Enums;
using _2048.AI.Model.Core;

namespace _2048.AI.Strategy
{
    public class RandomStrategy : IStrategy
    {
        private readonly Random _random = new Random();
        public Direction GetDirection(IBoard board)
        {
            // creates a number between 1 and 12
            return (Direction)_random.Next((int)Direction.Up, (int)Direction.Right+1);
        }

        public void SetSearchDepth(int depth)
        {
        }
    }
}