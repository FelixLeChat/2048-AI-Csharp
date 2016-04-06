using System;
using System.Collections.Generic;
using _2048.AI.Enums;
using _2048.AI.Model.Core;
using _2048.AI.Scoring;
using _2048.AI.Strategy.Core;

namespace _2048.AI.Strategy
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

        private readonly List<Direction> SkipMove = new List<Direction>()
        {
            Direction.NONE
        };

        // Don't recurse into node in prob is below this threshold
        private const float ProbabilityThreshold = 0.001f;
        private int MaxDepth = 3;
        private MasterScore Scorer = new MasterScore();

        public Direction GetDirection(IBoard board)
        {
            float bestScore = 0;
            var bestDirection = Direction.NONE;

            // Get the expected score for each move and keep the best
            foreach (var direction in PossibleDirection)
            {
                var copy = board.GetCopy();
                var hadChanged = copy.PerformMove(direction);
                var score = Expectimax(copy, MaxDepth, 1.0f, true);

                if (hadChanged)
                {
                    // Prevent getting stuck
                    if (bestScore == 0)
                    {
                        bestDirection = direction;
                    }
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestDirection = direction;
                    }
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
        public float Expectimax(IBoard board, int depth, float probability,  bool skipMove = false)
        {
            if (depth == 0 || probability < ProbabilityThreshold)
            {
                return (float)Scorer.GetScore(board);
            }

            float bestScore = 0.0f;

            var possibleDirection = skipMove ? SkipMove : PossibleDirection;

            // Try all possible move on the current state
            foreach (var direction in possibleDirection)
            {
                var copy = board.GetCopy();
                copy.PerformMove(direction);

                // Calculate the score of the moved board (expectation*score)
                float score = 0;
                foreach (var possibleEvent in ExpectimaxHelper.GetChanceEvents(board))
                {
                    ExpectimaxHelper.MakeChanceEvent(copy, possibleEvent);
                    float eventProbability = ExpectimaxHelper.GetEventProbability(board, possibleEvent);
                    float tempScore = Expectimax(copy, depth - 1, probability * eventProbability);
                    ExpectimaxHelper.UnMakeChanceEvent(copy, possibleEvent);
                    score += eventProbability * tempScore;
                }

                bestScore = Math.Max(score, bestScore);
            }

            return bestScore;
        }

    }
}
