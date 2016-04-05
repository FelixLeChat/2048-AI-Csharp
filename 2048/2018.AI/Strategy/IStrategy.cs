using _2018.AI.Enums;
using _2018.AI.Model.Core;
using _2048.Model;

namespace _2018.AI.Strategy
{
    public interface IStrategy
    {
        void Initialize(GameModel model);

        Direction GetDirection(IBoard board);
    }
}