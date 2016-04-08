using _2048.AI.Enums;

namespace _2048.AI.Model.Core
{
    public interface IBoard
    {
        bool PerformMove(Direction direction);
        void Initialize();

        double GetHeuristicEvaluation();

        int GetValue(int x, int y);
        void SetValue(int x, int y, int value);

        int GetScore();
        int GetSize();

        IBoard GetCopy();
    }
}