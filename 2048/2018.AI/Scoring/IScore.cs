using _2048.AI.Model;

namespace _2048.AI.Scoring
{
    public interface IScore
    {
        double Score(TreeNode node);
    }
}