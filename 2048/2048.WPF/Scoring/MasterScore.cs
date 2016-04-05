using _2048.WPF.Model.Core;
using Board = System.UInt64;
using Row = System.UInt16;

namespace _2048.WPF.Scoring
{
    public class MasterScore : IOptimizedScore
    {
        private const float SCORE_LOST_PENALTY = 200000.0f;
        private const float SCORE_MONOTONICITY_POWER = 4.0f;
        private const float SCORE_MONOTONICITY_WEIGHT = 47.0f;
        private const float SCORE_SUM_POWER = 3.5f;
        private const float SCORE_SUM_WEIGHT = 11.0f;
        private const float SCORE_MERGES_WEIGHT = 700.0f;
        private const float SCORE_EMPTY_WEIGHT = 270.0f;

        // X => , Y => 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public float GetScore(IBoard board)
        {
        }
    }
}