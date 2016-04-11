using System;
using System.Collections.Generic;
using System.Dynamic;
using _2048.AI.Enums;
using _2048.AI.Model.Core;
using Board = System.UInt64;

namespace _2048.AI.Strategy
{
    public class ExpectimaxStrategy : IStrategy, IStateSearch
    {
        private readonly List<Direction> PossibleDirection = new List<Direction>()
        {
            Direction.Up,
            Direction.Down,
            Direction.Left,
            Direction.Right
        };


        // Don't recurse into node in prob is below this threshold
        private const float ProbabilityThreshold = 0.0001f;
        public int MaxDepth { get; set; } = 5;

        public Direction GetDirection(IBoard board)
        {
            float bestScore = 0;
            var bestDirection = Direction.NONE;

            // Get the expected score for each move and keep the best
            foreach (var direction in PossibleDirection)
            {
                var copy = board.GetCopy();
                var hadChanged = copy.PerformMove(direction);
                
                if (hadChanged)
                {
                    var score = ScoreTopLevelMoveNode(copy, MaxDepth);

                    // Prevent getting stuck
                    if (bestScore < 0.01f)
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
        /// Will start the Expectimax search from the first Node
        /// </summary>
        /// <param name="board"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        private float ScoreTopLevelMoveNode(IBoard board, int depth)
        {
            var result = ScoreExpectationNode(board, depth - 1, 1.0f);
            return result;
        }

        ///// <summary>
        ///// Get the expected score from the current board
        ///// </summary>
        ///// <param name="board"></param>
        ///// <param name="depth"></param>
        ///// <returns></returns>
        //public float Expectimax(IBoard board, int depth, float probability, bool skipMove = false)
        //{
        //    if (depth == 0 || probability < ProbabilityThreshold)
        //    {
        //        return (float)board.GetHeuristicEvaluation();
        //    }

        //    float bestScore = 0.0f;

        //    var possibleDirection = skipMove ? SkipMove : PossibleDirection;

        //    // Try all possible move on the current state
        //    foreach (var direction in possibleDirection)
        //    {
        //        var copy = board.GetCopy();
        //        copy.PerformMove(direction);

        //        // Calculate the score of the moved board (expectation*score)
        //        float score = 0;
        //        foreach (var possibleEvent in ExpectimaxHelper.GetChanceEvents(board))
        //        {
        //            ExpectimaxHelper.MakeChanceEvent(copy, possibleEvent);
        //            float eventProbability = ExpectimaxHelper.GetEventProbability(board, possibleEvent);
        //            float tempScore = Expectimax(copy, depth - 1, probability * eventProbability);
        //            ExpectimaxHelper.UnMakeChanceEvent(copy, possibleEvent);
        //            score += eventProbability * tempScore;
        //        }

        //        bestScore = Math.Max(score, bestScore);
        //    }

        //    return bestScore;
        //}

        /// <summary>
        /// Represent a Node where we simulate a move, we will return the best score 
        /// </summary>
        /// <param name="board"></param>
        /// <param name="depth"></param>
        /// <param name="probability"></param>
        /// <returns></returns>
        private float ScoreMoveNode(IBoard board, int depth, float probability)
        {
            float bestScore = 0.0f;

            foreach (var direction in PossibleDirection)
            {
                var copy = board.GetCopy();
                var hadChanged = copy.PerformMove(direction);

                // No score if no changed: maybe it is a lost
                if (hadChanged)
                {
                    var score = ScoreExpectationNode(copy, depth - 1, probability);
                    bestScore = Math.Max(bestScore, score);
                }
            }
            return bestScore;
        }

        /// <summary>
        /// Will try all possible result from the previous move (put 2 and 4 tiles), will return the expected score (average)
        /// </summary>
        /// <param name="currentBoard"></param>
        /// <param name="depth"></param>
        /// <param name="probability"></param>
        /// <returns></returns>
        private float ScoreExpectationNode(IBoard currentBoard, int depth, float probability)
        {
            if (depth <= 0 || probability < ProbabilityThreshold)
            {
                return (float)currentBoard.GetHeuristicEvaluation();
            }

            var emptyCount = currentBoard.CountEmpty();
            probability /= probability;


            // Will put 2 and 4 on all empty case
            float score = 0;
            Board board = currentBoard.GetBitArrayRepresentation();
            Board temp = board;
            Board tile2 = 1;

            while (tile2 > 0)
            {
                if ((temp & 0xf) == 0)
                {
                    score += ScoreMoveNode(currentBoard.GetCopy(board | tile2) , depth - 1, probability * 0.9f) * 0.9f;
                    score += ScoreMoveNode(currentBoard.GetCopy(board | (tile2 << 1)), depth - 1, probability * 0.1f) * 0.1f;
                }
                temp >>= 4;
                tile2 <<= 4;
            }

            score = score/emptyCount;

            return score;
        }

    }
}
