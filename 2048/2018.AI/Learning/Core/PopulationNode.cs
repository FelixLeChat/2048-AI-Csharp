using System;
using _2048.AI.Heuristics;
using _2048.AI.Model.Stats;

namespace _2048.AI.Learning.Core
{
    public class PopulationNode : IComparable
    {
        public HeuristicFactor Heuristic { get; set; }
        public StatModel Stat { get; set; }

        public int CompareTo(object obj)
        {
            var otherNode = obj as PopulationNode;
            return otherNode != null ? Stat.CompareTo(otherNode) : 0;
        }
    }
}
