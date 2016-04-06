using System;
using _2048.AI.Model.Core;

namespace _2048.AI.Helper
{
    public class BoardHelper
    {
        public static void InitializeBoard(IBoard board)
        {
            board.SetValue(2, 0, 8);
            board.SetValue(2, 2, 4);
            board.SetValue(2, 3, 4);
            board.SetValue(3, 2, 8);
            board.SetValue(3, 3, 2);
        }

        public static void AddRandomCell(IBoard board)
        {
            var emptyCells = Heuristics.Heuristics.GetEmptyCellPositions(board);
            if (emptyCells == null || emptyCells.Count == 0)
                return;

            var randomPosition = emptyCells[Rnd.Next(0, emptyCells.Count - 1)];

            board.SetValue(randomPosition.X, randomPosition.Y, GetRandomStartingNumber());
        }

        private static readonly Random Rnd = new Random();
        private static int GetRandomStartingNumber()
        {
            return Rnd.NextDouble() < 0.9 ? 2 : 4;
        }
    }
}