using System;
using System.Collections.Generic;
using _2048.AI.Model.Core;

namespace _2048.AI.Strategy.Core
{
    public static class ExpectimaxHelper
    {
        public struct ChanceEvent
        {
            public int X;
            public int Y;
            public int Value;
        }

        public static List<ChanceEvent> GetChanceEvents(IBoard board)
        {
            var eventList = new List<ChanceEvent>();
            int boardSize = board.GetSize();
            for (var x = 0; x < boardSize; x++)
            {
                for (var y = 0; y < boardSize; y++)
                {
                    if (board.GetValue(x, y) == 0)
                    {
                        ChanceEvent c2;
                        c2.X = x;
                        c2.Y = y;
                        c2.Value = 2;
                        eventList.Add(c2);
                        ChanceEvent c4;
                        c4.X = x;
                        c4.Y = y;
                        c4.Value = 4;
                        eventList.Add(c4);
                    }
                }
            }
            return eventList;
        }

        public static
            float GetEventProbability(IBoard board, ChanceEvent chanceEvent)
        {
            var baseProbs = chanceEvent.Value == 2 ? 0.9f : 0.2f;
            var emptyCellCount = Heuristics.Heuristics.GetEmptyCellCount(board);
            var probability =  (float) Math.Pow(baseProbs, emptyCellCount);
            return probability;
        }

        /// <summary>
        /// Make the ChanceEvent on the game 
        /// i is similar to the seed of the chance event
        /// </summary>
        /// <param name="board"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public static void MakeChanceEvent(IBoard board, ChanceEvent chanceEvent)
        {
            board.SetValue(chanceEvent.X, chanceEvent.Y, chanceEvent.Value);
        }

        public static void UnMakeChanceEvent(IBoard board, ChanceEvent chanceEvent)
        {
            board.SetValue(chanceEvent.X, chanceEvent.Y, 0);
        }
    }
}