using System;
using System.Collections.Generic;
using System.Linq;
using _2048.Model;
using _2048.WPF.Game;

namespace _2048.WPF
{
    public class DeptFirstStrategy : IStrategy
    {
        public void Initialize(GameModel model)
        {
        }

        public void Ended(ScoreModel score)
        {
        }

        public Direction GetDirection(GameModel model)
        {
            var dfs = new DepthFirstSearch(model);
            return dfs.Search();
        }
    }

    public class TreeNode
    {
        public TreeNode Parent { get; set; }

        public TreeNode Left { get; set; }
        public TreeNode Right { get; set; }
        public TreeNode Up { get; set; }
        public TreeNode Down { get; set; }

        public GameModel GameModel { get; set; }
    }

    public class DepthFirstSearch
    {
        private readonly Random _random = new Random();
        private readonly Stack<TreeNode> _searchStack;
        private readonly TreeNode _root;
        public DepthFirstSearch(GameModel game)
        {
            _root = GetNode(Helper.ObjectExtensions.Copy(game));
            _searchStack = new Stack<TreeNode>();
        }

        private static readonly List<Direction> PassDirection = new List<Direction>();
        public Direction Search()
        {
            if (PassDirection.Count > 5)
                PassDirection.Remove(PassDirection[0]);

            var score = new Dictionary<Direction, int>();

            //if(_root.Left.GameModel.MoveChange)
                score.Add(Direction.Left, GetBestScore(_root.Left));
            //if (_root.Up.GameModel.MoveChange)
                score.Add(Direction.Up, GetBestScore(_root.Up));
            //if (_root.Right.GameModel.MoveChange)
                score.Add(Direction.Right, GetBestScore(_root.Right));
            //if (_root.Down.GameModel.MoveChange)
                score.Add(Direction.Down, GetBestScore(_root.Down));

            if (score.Values.Distinct().Count() == 1 || score.Count == 0 || PassDirection.Distinct().Count() == 1)
            {
                var direction = (Direction)_random.Next((int)Direction.Up, (int)Direction.Right + 1);
                PassDirection.Add(direction);
                return direction;
            }

            var select = score.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            PassDirection.Add(select);
            return select;
        }

        private int GetBestScore(TreeNode node)
        {
            var score = 0;

            _searchStack.Clear();
            _searchStack.Push(node);

            // Search on the stack
            while (_searchStack.Count != 0)
            {
                var current = _searchStack.Pop();

                if (current.GameModel.Score > score)
                    score = current.GameModel.Score;

                // Spawn Childs
                current = GetNode(current.GameModel);

                PushIfGood(current.Down);
                PushIfGood(current.Up);
                PushIfGood(current.Left);
                PushIfGood(current.Right);
            }

            return score;
        }

        private void PushIfGood(TreeNode node)
        {
            // Nothing changed
            if (!node.GameModel.MoveChange) return;

            // Same score as grandparent
            if (node.Parent?.Parent?.GameModel.Score <= node.GameModel.Score) return;

            _searchStack.Push(node);
        }

        private TreeNode GetNode(GameModel game)
        {
            var node = new TreeNode { GameModel = game };
            node.Down = new TreeNode() { GameModel = game.IterateNoRandom(Direction.Down), Parent = node };
            node.Up = new TreeNode() { GameModel = game.IterateNoRandom(Direction.Up), Parent = node };
            node.Left = new TreeNode() { GameModel = game.IterateNoRandom(Direction.Left), Parent = node };
            node.Right = new TreeNode() { GameModel = game.IterateNoRandom(Direction.Right), Parent = node };

            return node;
        }
    }
}