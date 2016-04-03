using _2048.WPF.Model;

namespace _2048.WPF.Scoring
{
    public interface IScore
    {
        double Score(TreeNode node);
    }
}