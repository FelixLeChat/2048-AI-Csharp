using _2018.AI.Model;

namespace _2018.AI.Scoring
{
    public interface IScore
    {
        double Score(TreeNode node);
    }
}