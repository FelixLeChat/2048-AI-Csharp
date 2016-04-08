using _2048.AI.Enums;
using _2048.AI.Heuristics;
using Board = System.UInt64;

namespace _2048.AI.Model.Core
{
    public interface IBoard
    {
        bool PerformMove(Direction direction);
        void Initialize(HeuristicFactor factors = null);

        double GetHeuristicEvaluation();

        int GetValue(int x, int y);
        void SetValue(int x, int y, int value);

        int GetScore();
        int GetSize();

        IBoard GetCopy();
        IBoard GetCopy(Board board);
        Board GetBitArrayRepresentation();
        int CountEmpty();
    }
}