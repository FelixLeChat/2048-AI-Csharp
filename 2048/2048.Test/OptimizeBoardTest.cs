using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _2048.WPF.Model;

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
        public void SimpleMerge()
        {
            OptimizeBoardHelper.InitLookupTable();
            OptimizeBoard board = new OptimizeBoard();
            board.Board = 0;
            board.InsertValue(0, 0, 0x1);
            board.InsertValue(1, 0, 0x1);
            Assert.AreEqual(2, board.GetValue(0, 0));
            Assert.AreEqual(2, board.GetValue(1, 0));
            var s1 = board.ToString();
            board.PerformMove(WPF.Game.Direction.Left);
            var s2 = board.ToString();
            Assert.AreEqual(4, board.GetValue(0, 0));
            Assert.AreEqual(0, board.GetValue(1, 0));
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
            board.PerformMove(WPF.Game.Direction.Left);
            var s2 = board.ToString();
            Assert.AreEqual(4, board.GetValue(0, 0));
            Assert.AreEqual(2, board.GetValue(1, 0));
        }
    }
}
