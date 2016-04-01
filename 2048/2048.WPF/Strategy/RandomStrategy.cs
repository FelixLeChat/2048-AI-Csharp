using System;
using _2048.Model;
using _2048.WPF.Game;

namespace _2048.WPF
{
    public class RandomStrategy :IStrategy
    {
        private Random rnd = new Random();
         // creates a number between 1 and 12
        public Direction GetDirection(GameModel board)
        {
            return (Direction)rnd.Next((int)Direction.Up, (int)Direction.Right+1);
        }
    }
}