using System;
using System.Collections.Generic;
using System.Linq;
using _2018.AI.Enums;
using _2018.AI.Model;
using _2018.AI.Model.Core;
using _2018.AI.Scoring;
using _2048.Model;

using static _2018.AI.Heuristics.Heuristics;

namespace _2018.AI.Strategy
{
    public class IterativeDeepening : IStrategy
    {
        private static IScore Scoring { get; } = new IterativeEvalScore();

        public Direction GetDirection(IBoard board)
        {
            return Search(board, 0, - 10000, 10000, 0, 0).Move;
        }

        private static readonly Random Random = new Random();
        private static Result Search(IBoard board, int depth, double alpha, double beta, int positions,int cutoffs)
        {
            //return Direction.Up;
            var bestScore = beta;
            var bestMove = Direction.NONE;

            // try a 2 and 4 in each cell
            var cells = GetEmptyCellPositions(board);
            var scores = new Dictionary<int, List<Tuple<Position, double>>>()
            {
                {2, new List<Tuple<Position, double>>()},
                {4, new List<Tuple<Position, double>>()}
            };

            // for each value (2 and 4)
            foreach (var value in scores)
            {
                // for each cell that is empty
                foreach (var i in cells)
                {
                    var newCells = board.GetCopy();
                    newCells.SetValue(i.X, i.Y, value.Key); // 2 or 4

                    // score of each cell for 3 or 4 value of new brick
                    scores[value.Key].Add(new Tuple<Position, double>(i,
                        GetSmoothness(newCells) + GetIslandCount(newCells)));
                }
            }

            // now just pick out the most annoying moves
            double[] maxScore = {-10000000.0};
            foreach (
                var tuple in from score in scores from tuple in score.Value where tuple.Item2 > maxScore[0] select tuple)
            {
                maxScore[0] = tuple.Item2;
            }


            var candidates = (from score in scores
                from tupple in score.Value
                where Math.Abs(Math.Ceiling(tupple.Item2) - Math.Ceiling(maxScore[0])) < 0.001
                select
                    new Tuple<Position, int>(new Position() {X = tupple.Item1.X, Y = tupple.Item1.Y}, score.Key))
                .ToList();

            // search on each candidate
            foreach (var t in candidates)
            {
                var position = t.Item1;
                var value = t.Item2;

                var newGrid = board.GetCopy();
                newGrid.SetValue(position.X, position.Y, value);

                positions++;
                var result = Search2(newGrid, depth, alpha, bestScore, positions, cutoffs);

                positions = result.Position;
                cutoffs = result.Cutoff;

                if (bestMove == Direction.NONE)
                    bestMove = result.Move;

                if (result.Score<bestScore)
                {
                    bestScore = result.Score;
                    bestMove = result.Move;
                }
                if (bestScore<alpha)
                {
                    cutoffs++;
                    //return new Result() { Move = Direction.Up, Score = alpha, Position = positions, Cutoff = cutoffs };
                }
            }

            if(bestMove == Direction.NONE)
                bestMove = (Direction)Random.Next((int)Direction.Up, (int)Direction.Right + 1);
            return new Result() { Move = bestMove, Score = bestScore, Position = positions, Cutoff = cutoffs };
        }

        private static Result Search2(IBoard board, int depth, double alpha, double beta, int positions, int cutoffs)
        {
            var bestMove = Direction.NONE;
            var bestScore = alpha;

            var values = Enum.GetValues(typeof(Direction)).Cast<Direction>();

            foreach (var dir in values)
            {
                var newGrid = board.GetCopy();

                if (!newGrid.PerformMove(dir)) continue;
                positions++;
                if (IsWon(newGrid))
                {
                    return new Result() { Move = dir, Score = bestScore, Position = positions, Cutoff = cutoffs };
                }
                    
                Result result;
                if (depth == 0)
                {
                    result = new Result() { Move = dir, Score = Scoring.Score(new TreeNode() {Board = newGrid}), Position = positions, Cutoff = cutoffs };
                }
                else {
                    result = Search2(board,depth - 1, bestScore, beta, positions, cutoffs);
                    if (result.Score > 9900)
                    { // win
                        result.Score--; // to slightly penalize higher depth from win
                    }
                    positions = result.Position;
                    cutoffs = result.Cutoff;
                }

                if (result.Score > bestScore)
                {
                    bestScore = result.Score;
                    bestMove = dir;
                }
                if (!(bestScore > beta)) continue;
                cutoffs++;
                return new Result() { Move = bestMove, Score = beta, Position = positions, Cutoff = cutoffs };
            }
            return new Result() { Move = bestMove, Score = bestScore, Position = positions, Cutoff = cutoffs };
        }
    }

    public class Result
    {
        public Direction Move { get; set; }
        public double Score { get; set; }
        public int Position { get; set; }
        public int Cutoff { get; set; }
    }
}