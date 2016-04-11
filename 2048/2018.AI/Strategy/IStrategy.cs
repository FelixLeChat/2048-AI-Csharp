using _2048.AI.Enums;
using _2048.AI.Model.Core;

namespace _2048.AI.Strategy
{
    public interface IStrategy
    {
        Direction GetDirection(IBoard board);
        void SetSearchDepth(int depth);
    }
}