using _2048.WPF.Model;

namespace _2048.WPF.Scoring
{
    public class GameScore : IScore
    {
        public double Score(TreeNode node)
        {
            return node.GameModel.Score;
        }
    }
}