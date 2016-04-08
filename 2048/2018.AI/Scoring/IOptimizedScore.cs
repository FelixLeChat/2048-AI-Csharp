using _2048.AI.Model.Core;

namespace _2048.AI.Scoring
{
    public interface IOptimizedScore
    {
        double GetScore(IBoard board);
    }
}