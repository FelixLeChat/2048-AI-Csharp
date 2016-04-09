using _2048.AI.Enums;
using _2048.AI.Model.Core;

namespace _2048.AI.Strategy
{
    public interface IStateSearch
    {
        int MaxDepth { get; set; }
    }
}