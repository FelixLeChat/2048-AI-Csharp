using _2048.Model;
using _2048.WPF.Game;

namespace _2048.WPF
{
    public class Defaultstrategy : IStrategy
    {
        private Direction _lastDirection = Direction.Up;
        private Cell[][] _lastCells = null;

        public Direction GetDirection(GameModel board)
        {
            var direction = Direction.Up;

            // First move
            if (_lastCells == null)
            {
                _lastCells = board.Cells;
                return direction;
            }

            // Stuck
            /*if (Helper.Helper.Equal(_lastCells, board.Cells))
            {
                _lastCells = board.Cells;
                 return Direction.Down;
            }*/


            switch (_lastDirection)
            {
                case Direction.Up:
                    _lastDirection = Direction.Right;
                    direction = Direction.Right;
                    break;

                case Direction.Right:
                    _lastDirection = Direction.Left;
                    direction = Direction.Left;
                    break;

                default:
                    _lastDirection = Direction.Up;
                    direction = Direction.Up;
                    break;
            }

            _lastCells = board.Cells;
            return direction;
        }
    }
}