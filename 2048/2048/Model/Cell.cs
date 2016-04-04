using System;

namespace _2048.Model
{
    [Serializable]
    public class Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    [Serializable]
    public class Cell
    {
        public int Value { get; set; }

        public bool WasMerged { get; set; }

        public bool WasCreated { get; set; }

        public Coordinate Position { get; set; }

        public int X
        {
            get
            {
                return Position.X;
            }
            set
            {
                Position.X = value;
            }
        }

        public int Y
        {
            get
            {
                return Position.Y;
            }
            set
            {
                Position.Y = value;
            }
        }

        public Coordinate PreviousPosition { get; set; }
        
        public Cell(int x, int y)
        {
            Position = new Coordinate(x, y);
        }

        public bool IsEmpty()
        {
            return Value == 0;
        }

        public override bool Equals(object obj)
        {
            var cell = obj as Cell;
            if (cell == null)
                return false;

            return Value == cell.Value && Position.X == cell.Position.X && Position.Y == cell.Position.Y;
        }
    }
}
