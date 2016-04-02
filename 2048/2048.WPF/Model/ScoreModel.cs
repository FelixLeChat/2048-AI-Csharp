using _2048.WPF.Enums;

namespace _2048.WPF.Game
{
    public struct ScoreModel
    {
        public int Score { get; set; } 
        public int MaxTile { get; set; }
        public State State { get; set; }
    }
}