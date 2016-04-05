using _2018.AI.Enums;

namespace _2018.AI.Model.Core
{
    public interface IBoard
    {
        bool PerformMove(Direction direction);
        int GetValue(int x, int y);
        int GetSize();
    }
}