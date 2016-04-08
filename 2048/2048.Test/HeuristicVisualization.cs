using Microsoft.VisualStudio.TestTools.UnitTesting;
using _2048.AI.Model.Optimize;

namespace _2048.Test
{
    [TestClass]
    public class HeuristicVisualization
    {
        [TestMethod]
        public void SimpleLeftMerge()
        {
            OptimizeBoard board = new OptimizeBoard();
            board.Initialize();

            board.Board = 0;
            board.InsertValue(0, 0, 64);
            board.InsertValue(1, 0, 32);
            board.InsertValue(2, 0, 32);
            board.InsertValue(3, 0, 32);

            var score1 = board.GetHeuristicEvaluation();
        }
    }
}
