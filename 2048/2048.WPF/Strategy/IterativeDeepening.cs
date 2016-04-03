using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using _2048.Model;
using _2048.WPF.Game;
using _2048.WPF.Model;

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
            return Direction.Up;
        }


        private static Direction Search(Cell[][] gameCells, int depth, double alpha, double beta, int positions, double cutoffs)
        {
            return Direction.Up;
            /*
            var bestScore = beta;
            var bestMove = -1;
            var result;

            // computer's turn, we'll do heavy pruning to keep the branching factor low

            // try a 2 and 4 in each cell and measure how annoying it is
            // with metrics from eval
            var candidates = new List<Position>();
            var cells = Helper.CellHelper.GetEmptyCellPositions(gameCells);
            var scores = new Dictionary<int, List<double>>() {{2, new List<double>()}, {4, new List<double>()}};

            // for each value (2 and 4)
            foreach (var value in scores)
            {
                // for each cell that is empty
                foreach (var i in cells)
                {
                    //scores[value.Key].Add(0);
                    var cell = i;

                    var newCells = Helper.Helper.DeepClone(gameCells);
                    newCells[i.X][i.Y].Value = value.Key;

                    scores[value.Key].Add(Helper.CellHelper.GetSmoothness(newCells) + Helper.CellHelper.GetIslandCount(newCells));
                }
            }

            // now just pick out the most annoying moves
            var maxScore = scores.SelectMany(score => score.Value).Concat(new[] {0.0}).Max();

            foreach (var score in scores)
            {
                for (var i = 0; i< scores[score.Key].Count; i++)
                {
                    var cell 
                if (scores[score.Key][i] == maxScore) {
                    candidates.Add(new Position() {X=});
                }
                }
            }

            // search on each candidate
            for (var i = 0; i<candidates.length; i++) {
                var position = candidates[i].position;
        var value = candidates[i].value;
        var newGrid = this.grid.clone();
        var tile = new Tile(position, value);
        newGrid.insertTile(tile);
                newGrid.playerTurn = true;
                positions++;
                newAI = new AI(newGrid);
        result = newAI.search(depth, alpha, bestScore, positions, cutoffs);
                positions = result.positions;
                cutoffs = result.cutoffs;

                if (result.score<bestScore) {
                bestScore = result.score;
                }
                if (bestScore<alpha) {
                cutoffs++;
                return { move: null, score: alpha, positions: positions, cutoffs: cutoffs };
                }
            }
            }

            return { move: bestMove, score: bestScore, positions: positions, cutoffs: cutoffs };*/
        }
    }
}