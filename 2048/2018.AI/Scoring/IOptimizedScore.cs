using _2018.AI.Model.Core;

namespace _2018.AI.Scoring
{
    public interface IOptimizedScore
    {
        float GetScore(IBoard board);
    }
}