using _2048.Model;

namespace _2048.WPF.Model
{
    public class TreeNode
    {
        public TreeNode Parent { get; set; }

        public TreeNode Left { get; set; }
        public TreeNode Right { get; set; }
        public TreeNode Up { get; set; }
        public TreeNode Down { get; set; }

        public double Score { get; set; }
        public int Depth { get; set; }
        public GameModel GameModel { get; set; }
    }
}