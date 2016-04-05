using _2048.WPF.Game;

namespace _2048.WPF.Model.Core
{
    public interface IBoard
    {
        bool PerformMove(Direction direction);
        int GetValue(int x, int y);
    }
}