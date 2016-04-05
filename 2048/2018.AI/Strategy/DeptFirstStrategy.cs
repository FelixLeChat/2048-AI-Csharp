using System;
using System.Collections.Generic;
using System.Linq;
using _2018.AI.Enums;
using _2018.AI.Model;
using _2018.AI.Model.Core;
using _2018.AI.Scoring;

namespace _2018.AI.Strategy
{
    public class DeptFirstStrategy : IStrategy
    {
        public Direction GetDirection(IBoard board)
        {
            var dfs = new DepthFirstSearch(board, new MasterScore());
            return dfs.Search();
        }
    }

    public class DepthFirstSearch
    {
        private readonly Random _random = new Random();
        private readonly Stack<TreeNode> _searchStack;
        private readonly TreeNode _root;
        private readonly IScore _score;

        public DepthFirstSearch(IBoard board, IScore score)
        {
            _root = GetNode(Helper.ObjectExtensions.Copy(board),0);
            _score = score;
            _searchStack = new Stack<TreeNode>();
        }

        private static readonly List<Direction> PassDirection = new List<Direction>();
        public Direction Search()
        {
            Direction direction;

            if (PassDirection.Count > 5)
                PassDirection.Remove(PassDirection[0]);

            var score = new Dictionary<Direction, double>
            {
                {Direction.Left, GetBestScore(_root.Left)},
                {Direction.Up, GetBestScore(_root.Up)},
                {Direction.Right, GetBestScore(_root.Right)},
                {Direction.Down, GetBestScore(_root.Down)}
            };

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
                current = GetNode(current.Board, current.Depth);

                PushIfGood(current.Down);
                PushIfGood(current.Up);
                PushIfGood(current.Left);
                PushIfGood(current.Right);
            }

            return score;
        }

        private void PushIfGood(TreeNode node)
        {
            if (node.Depth >= 3) return;

            // Same score as grandparent
            if (node.Parent?.Parent?.Score <= node.Score) return;

            _searchStack.Push(node);
        }

        private static TreeNode GetNode(IBoard board, int depth)
        {
            var node = new TreeNode { Board = board };

            var downBoard = board.GetCopy();
            downBoard.PerformMove(Direction.Down);
            node.Down = new TreeNode() { Board = downBoard, Parent = node, Depth = depth + 1};

            var upBoard = board.GetCopy();
            upBoard.PerformMove(Direction.Up);
            node.Up = new TreeNode() { Board = upBoard, Parent = node, Depth = depth + 1 };

            var leftBoard = board.GetCopy();
            leftBoard.PerformMove(Direction.Left);
            node.Left = new TreeNode() { Board = leftBoard, Parent = node, Depth = depth + 1 };

            var lightBoard = board.GetCopy();
            lightBoard.PerformMove(Direction.Right);
            node.Right = new TreeNode() { Board = lightBoard, Parent = node, Depth = depth + 1 };

            return node;
        }
    }
}