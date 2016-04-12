using System;

namespace _2048.AI.Model.Stats
{
    public class StatModel  : IComparable
    {
        public int MaxScore{get; set;}
        public int MaxTile { get; set; }
        public int MinTile { get; set; }
        public int AverageScore { get; set; }
        public long TotalGameTime { get; set; }
        public int TotalMoveCount { get; set; }
        public int TotalWins { get; set; }
        public int TotalLosses { get; set; }


        public int CompareTo(object obj)
        {
            var otherStat = obj as StatModel;
            return otherStat != null ? AverageScore.CompareTo(otherStat.AverageScore) : 0;
        }
    }
}