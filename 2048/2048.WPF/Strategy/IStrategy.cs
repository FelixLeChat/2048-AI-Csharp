using _2048.Model;
using _2048.WPF.Game;

namespace _2048.WPF
{
    public interface IStrategy
    {
        Direction GetDirection(GameModel board);
    }
}