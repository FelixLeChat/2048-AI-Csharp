using System;
using System.Collections.Generic;
using System.Linq;
using _2018.AI.Enums;
using _2018.AI.Model;
using _2018.AI.Model.Core;
using _2018.AI.Scoring;
using _2048.Model;

namespace _2018.AI.Strategy
{
    public class DeptFirstStrategy : IStrategy
    {
        public void Initialize(GameModel model)
        {
        }

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
            _root = GetNode(Helper.ObjectExtensions.Copy(board));
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
                current = GetNode(current.Board);

                PushIfGood(current.Down);
                PushIfGood(current.Up);
                PushIfGood(current.Left);
                PushIfGood(current.Right);
            }

            return score;
        }

        private void PushIfGood(TreeNode node)
        {
            if (node.Depth >= 7) return;

            // Same score as grandparent
            if (node.Parent?.Parent?.Score <= node.Score) return;

            _searchStack.Push(node);
        }

        private TreeNode GetNode(IBoard board)
        {
            var node = new TreeNode { Board = board };

            var downBoard = Helper.Helper.DeepClone(board);
            downBoard.PerformMove(Direction.Down);
            node.Down = new TreeNode() { Board = downBoard, Parent = node, Depth = node.Depth + 1};

            var upBoard = Helper.Helper.DeepClone(board);
            upBoard.PerformMove(Direction.Up);
            node.Up = new TreeNode() { Board = upBoard, Parent = node, Depth = node.Depth + 1 };

            var leftBoard = Helper.Helper.DeepClone(board);
            leftBoard.PerformMove(Direction.Left);
            node.Left = new TreeNode() { Board = leftBoard, Parent = node, Depth = node.Depth + 1 };

            var RightBoard = Helper.Helper.DeepClone(board);
            RightBoard.PerformMove(Direction.Left);
            node.Right = new TreeNode() { Board = RightBoard, Parent = node, Depth = node.Depth + 1 };

            return node;
        }
    }
}