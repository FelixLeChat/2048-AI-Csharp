using _2048.WPF.Model;
using _2048.WPF.Model.Core;
using Board = System.UInt64;
using Row = System.UInt16;

namespace _2048.WPF.Scoring
{
    public interface IOptimizedScore
    {
        float GetScore(IBoard board);
    }
}