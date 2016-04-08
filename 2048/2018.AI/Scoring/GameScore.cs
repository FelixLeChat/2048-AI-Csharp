using _2048.AI.Model;

namespace _2048.AI.Scoring
{
    public class GameScore : IScore
    {
        public double Score(TreeNode node)
        {
            return Heuristics.Heuristics.GetScore(node.Board);
        }
    }
}