using System;
using System.Collections.Generic;
using System.Linq;
using _2048.Model;
using _2048.WPF.Game;
using _2048.WPF.Model;
using _2048.WPF.Scoring;

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
            var dfs = new DepthFirstSearch(model, new MasterScore());
            return dfs.Search();
        }
    }

    public class DepthFirstSearch
    {
        private readonly Random _random = new Random();
        private readonly Stack<TreeNode> _searchStack;
        private readonly TreeNode _root;
        private readonly IScore _score;

        public DepthFirstSearch(GameModel game, IScore score)
        {
            _root = GetNode(Helper.ObjectExtensions.Copy(game));
            _score = score;
            _searchStack = new Stack<TreeNode>();
        }

        private static readonly List<Direction> PassDirection = new List<Direction>();
        public Direction Search()
        {
            Direction direction = Direction.Up;

            if (PassDirection.Count > 5)
                PassDirection.Remove(PassDirection[0]);

            var score = new Dictionary<Direction, double>();

            //if(_root.Left.GameModel.MoveChange)
                score.Add(Direction.Left, GetBestScore(_root.Left));
            //if (_root.Up.GameModel.MoveChange)
                score.Add(Direction.Up, GetBestScore(_root.Up));
            //if (_root.Right.GameModel.MoveChange)
                score.Add(Direction.Right, GetBestScore(_root.Right));
            //if (_root.Down.GameModel.MoveChange)
                score.Add(Direction.Down, GetBestScore(_root.Down));

            // All same score
            if (score.Values.Distinct().Count() == 1 || PassDirection.Distinct().Count() == 1)
            {
                direction = (Direction)_random.Next((int)Direction.Up, (int)Direction.Right + 1);
            }
            else
            {
                direction = score.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            }

            
            PassDirection.Add(direction);
            return direction;
        }

        private double GetBestScore(TreeNode node)
        {
            var score = 0.0;

            _searchStack.Clear();
            _searchStack.Push(node);

            // Search on the stack
            while (_searchStack.Count != 0)
            {
                var current = _searchStack.Pop();

                // Call scoring algo
                current.Score = _score.Score(current);

                if (current.Score > score)
                    score = current.Score;

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

            if (node.Depth >= 7) return;

            // Same score as grandparent
            //if (node.Parent?.Parent?.Score <= node.Score) return;

            _searchStack.Push(node);
        }

        private TreeNode GetNode(GameModel game)
        {
            var node = new TreeNode { GameModel = game };
            node.Down = new TreeNode() { GameModel = game.IterateNoRandom(Direction.Down), Parent = node, Depth = node.Depth + 1};
            node.Up = new TreeNode() { GameModel = game.IterateNoRandom(Direction.Up), Parent = node, Depth = node.Depth + 1 };
            node.Left = new TreeNode() { GameModel = game.IterateNoRandom(Direction.Left), Parent = node, Depth = node.Depth + 1 };
            node.Right = new TreeNode() { GameModel = game.IterateNoRandom(Direction.Right), Parent = node, Depth = node.Depth + 1 };

            return node;
        }
    }
}