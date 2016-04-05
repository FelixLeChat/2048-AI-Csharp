using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using _2018.AI.Enums;
using _2018.AI.Model.Core;
using _2018.AI.Strategy.Core;
using _2048.Model;

namespace _2018.AI.Strategy
{
    public class ExpectimaxStrategy : IStrategy
    {
        private readonly List<Direction> PossibleDirection = new List<Direction>()
        {
            Direction.Up,
            Direction.Down, 
            Direction.Left,
            Direction.Right
        };

        private int MaxDepth = 3;

        public Direction GetDirection(IBoard board)
        {
            float bestScore = 0;
            var bestDirection = Direction.NONE;

            // Get the expected score for each move and keep the best
            foreach (var direction in PossibleDirection)
            {
                var copy = board.GetCopy();
                copy.PerformMove(direction);
                var score = Expectimax(copy, MaxDepth);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestDirection = direction;
                }
            }

            return bestDirection;
        }

        /// <summary>
        /// Get the expected score from the current board
        /// </summary>
        /// <param name="board"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public float Expectimax(IBoard board, int depth)
        {
            if (depth == 0)
            {
                board.GetScore();
            }

            float bestScore = 0.0f;

            // Try all possible move on the current state
            foreach (var direction in PossibleDirection)
            {
                var copy = board.GetCopy();
                copy.PerformMove(direction);

                // Calculate the score of the moved board (expectation*score)
                float score = 0;
                foreach (var possibleEvent in ExpectimaxHelper.GetChanceEvents(board))
                {
                    ExpectimaxHelper.MakeChanceEvent(board, possibleEvent);
                    float tempScore = Expectimax(board, depth - 1);
                    ExpectimaxHelper.UnMakeChanceEvent(board, possibleEvent);
                    score += ExpectimaxHelper.GetEventProbability(board, possibleEvent) * tempScore;
                }

                bestScore = Math.Max(score, bestScore);
            }

            return bestScore;
        }

    }
}
