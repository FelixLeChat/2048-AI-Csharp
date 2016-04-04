using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using _2048.Model;
using _2048.WPF.Game;
using _2048.WPF.Model;
using _2048.WPF.Scoring;

namespace _2048.WPF
{
    public class IterativeDeepening : IStrategy
    {
        public void Initialize(GameModel model)
        {
        }

        public void Ended(ScoreModel score)
        {
        }

        public Direction GetDirection(GameModel model)
        {
            return Search(model, 0, - 10000, 10000, 0, 0).Move;
        }

        private static readonly Random Random = new Random();
        private static Result Search(GameModel model, int depth, double alpha, double beta, int positions,int cutoffs)
        {
            //return Direction.Up;
            var gameCells = model.Cells;
            var bestScore = beta;
            var bestMove = Direction.NONE;
            var result = new Result();

            // try a 2 and 4 in each cell and measure how annoying it is
            // with metrics from eval
            var cells = Helper.CellHelper.GetEmptyCellPositions(gameCells);
            var scores = new Dictionary<int, List<Tuple<Cell, double>>>()
            {
                {2, new List<Tuple<Cell, double>>()},
                {4, new List<Tuple<Cell, double>>()}
            };

            // for each value (2 and 4)
            foreach (var value in scores)
            {
                // for each cell that is empty
                foreach (var i in cells)
                {
                    var newCells = Helper.Helper.DeepClone(gameCells);
                    newCells[i.X][i.Y].Value = value.Key; // 2 or 4

                    // score of each cell for 3 or 4 value of new brick
                    scores[value.Key].Add(new Tuple<Cell, double>(newCells[i.X][i.Y],
                        Helper.CellHelper.GetSmoothness(newCells) + Helper.CellHelper.GetIslandCount(newCells)));
                }
            }

            // now just pick out the most annoying moves
            var maxScore = -10000000.0;
            var maxCell = new Cell(0, 0);
            foreach (
                var tuple in from score in scores from tuple in score.Value where tuple.Item2 > maxScore select tuple)
            {
                maxScore = tuple.Item2;
                maxCell = tuple.Item1;
            }


            var candidates = (from score in scores
                from tupple in score.Value
                where Math.Ceiling(tupple.Item2) == Math.Ceiling(maxScore)
                select
                    new Tuple<Position, int>(new Position() {X = tupple.Item1.X, Y = tupple.Item1.Y}, score.Key))
                .ToList();

            // search on each candidate
            foreach (var t in candidates)
            {
                var position = t.Item1;
                var value = t.Item2;

                var newGrid = Helper.Helper.DeepClone(gameCells);
                newGrid[position.X][position.Y].Value = value;

                positions++;
                result = Search2(model,depth, alpha, bestScore, positions, cutoffs);

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

        private static Result Search2(GameModel model, int depth, double alpha, double beta, int positions, int cutoffs)
        {
            var gameCells = model.Cells;
            var bestMove = Direction.NONE;
            var result = new Result();
            var bestScore = alpha;

            var values = Enum.GetValues(typeof(Direction)).Cast<Direction>(); ;

            foreach (var dir in values)
            {
                var newGrid = Helper.Helper.DeepClone(gameCells);
                var gameModel = new GameModel(4,4);
                gameModel.Cells = newGrid;
                gameModel.Score = model.Score;

                if (gameModel.PerformMove(dir))
                {
                    positions++;
                    if (Helper.CellHelper.IsWon(gameModel.Cells))
                    {
                        return new Result() { Move = dir, Score = bestScore, Position = positions, Cutoff = cutoffs };
                    }
                    

                    if (depth == 0)
                    {
                        result = new Result() { Move = dir, Score = new IterativeEvalScore().Score(new TreeNode() {GameModel = gameModel}), Position = positions, Cutoff = cutoffs };
                    }
                    else {
                        result = Search2(model,depth - 1, bestScore, beta, positions, cutoffs);
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
                    if (bestScore > beta)
                    {
                        cutoffs++;
                        return new Result() { Move = bestMove, Score = beta, Position = positions, Cutoff = cutoffs };
                    }
                }
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