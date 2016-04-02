using _2048.Model;
using _2048.WPF.Game;

namespace _2048.WPF
{
    public interface IStrategy
    {
        void Initialize(GameModel model);
        void Ended(ScoreModel score);

        Direction GetDirection(GameModel model);
    }
}