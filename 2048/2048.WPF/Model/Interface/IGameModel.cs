using _2048.WPF.Game;

namespace _2048.WPF.Model.Interface
{
    public interface IGameModel
    {
        bool PerformMove(Direction direction);
    }
}