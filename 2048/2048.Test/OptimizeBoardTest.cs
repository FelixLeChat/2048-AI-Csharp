using Microsoft.VisualStudio.TestTools.UnitTesting;
using _2018.AI.Enums;
using _2018.AI.Model.Core;
using _2018.AI.Model.Optimize;

namespace _2048.Test
{
    [TestClass]
    public class OptimizeBoardTest
    {
        [TestMethod]
        public void Insertion()
        {
            OptimizeBoard board = new OptimizeBoard();
            board.Board = 0;
            board.InsertValue(0, 0, 0x1);
            var result = board.GetValue(0, 0);
            Assert.AreEqual(2, result);

        }

        [TestMethod]
        public void SimpleLeftMerge()
        {
            OptimizeBoardHelper.InitLookupTable();
            OptimizeBoard board = new OptimizeBoard();
            board.Board = 0;
            board.InsertValue(0, 0, 0x1);
            board.InsertValue(1, 0, 0x1);
            Assert.AreEqual(2, board.GetValue(0, 0));
            Assert.AreEqual(2, board.GetValue(1, 0));
            var s1 = board.ToString();
            board.PerformMove(Direction.Left);
            var s2 = board.ToString();
            Assert.AreEqual(4, board.GetValue(0, 0));
            Assert.AreEqual(0, board.GetValue(1, 0));
        }


        // 0 0 8 0 
        // 0 0 0 0 
        // 0 0 4 8 
        // 0 0 4 2 
        // UP
        // 0 0 8 8 
        // 0 0 8 2 
        // 0 0 0 0 
        // 0 0 0 0 
        [TestMethod]
        public void MoveUp()
        {
            OptimizeBoardHelper.InitLookupTable();
            IBoard board = new OptimizeBoard();
            board.SetValue(2, 0, 8);
            board.SetValue(2, 2, 4);
            board.SetValue(2, 3, 4);
            board.SetValue(3, 2, 8);
            board.SetValue(3, 3, 2);
            var s1 = board.ToString();
            board.PerformMove(Direction.Up);
            var s2 = board.ToString();
            Assert.AreEqual(8, board.GetValue(2, 0));
            Assert.AreEqual(8, board.GetValue(2, 1));
            Assert.AreEqual(8, board.GetValue(3, 0));
            Assert.AreEqual(2, board.GetValue(3, 1));
        }

        [TestMethod]
        public void HalfMerge()
        {
            OptimizeBoardHelper.InitLookupTable();
            OptimizeBoard board = new OptimizeBoard();
            board.Board = 0;
            board.InsertValue(0, 0, 0x1);
            board.InsertValue(1, 0, 0x1);
            board.InsertValue(2, 0, 0x1);
            Assert.AreEqual(2, board.GetValue(0, 0));
            Assert.AreEqual(2, board.GetValue(1, 0));
            var s1 = board.ToString();
            board.PerformMove(Direction.Left);
            var s2 = board.ToString();
            Assert.AreEqual(4, board.GetValue(0, 0));
            Assert.AreEqual(2, board.GetValue(1, 0));
        }
    }
}
