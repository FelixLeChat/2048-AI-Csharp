using _2018.AI.Model;

namespace _2018.AI.Scoring
{
    public class GameScore : IScore
    {
        public double Score(TreeNode node)
        {
            return node.Board.GetScore();
        }
    }
}