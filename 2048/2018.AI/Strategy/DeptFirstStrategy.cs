using System;
using System.Collections.Generic;
using System.Linq;
using _2048.AI.Enums;
using _2048.AI.Model;
using _2048.AI.Model.Core;

namespace _2048.AI.Strategy
{
    public class DeptFirstStrategy : IStrategy
    {
        public Direction GetDirection(IBoard board)
        {
            var dfs = new DepthFirstSearch(board);
            return dfs.Search();
        }

        private int Depth { get; set; }
        public void SetSearchDepth(int depth)
        {
            Depth = depth;
        }
    }

    public class DepthFirstSearch
    {
        private readonly Random _random = new Random();
        private readonly Stack<TreeNode> _searchStack;
        private readonly TreeNode _root;

        public DepthFirstSearch(IBoard board)
        {
            _root = GetNode(board,0);
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

            // Try move to see if they do something
            var dict = new Dictionary<Direction, bool>();
            var values = Enum.GetValues(typeof(Direction)).Cast<Direction>();
            foreach (var value in values)
            {
                var copy = _root.Board.GetCopy();
                dict.Add(value, copy.PerformMove(value));
            }

            // Move in the direction if there is only one direction to go
            if (dict.Select(x => x.Value).Count() == 1)
                return dict.First(x => x.Value).Key;

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
                current.Score = current.Board.GetHeuristicEvaluation();

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