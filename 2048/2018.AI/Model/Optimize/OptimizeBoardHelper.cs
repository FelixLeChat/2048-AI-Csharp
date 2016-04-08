using System;
using System.Text;
using _2048.AI.Enums;
using _2048.AI.Helper;
using Board = System.UInt64;
using Row = System.UInt16;


namespace _2048.AI.Model.Optimize
{
    /// <summary>
    /// Inspired from https://github.com/nneonneo/2048-ai
    /// </summary>

    public static class OptimizeBoardHelper
    {
        private static bool IsInitialize { get; set; } = false;

        private const Board ROW_MASK = 0xFFFF;
        private const Board COL_MASK = 0x000F000F000F000F;

        // Pre-calculate all possible result of move for row
        // A row is 16 bits: so 65536 possibility
        #region Lookup Table
        static readonly Row[] _rowLeftTable = new Row[65536];
        static readonly Row[] _rowRightTable = new Row[65536];
        static readonly Board[] _colUpTable = new Board[65536];
        static readonly Board[] _colDownTable = new Board[65536];

        static readonly float[] _scoreTable = new float[65536];


        public static void InitLookupTable()
        {
            if (IsInitialize)
            {
                return;
            }

            for (int row = 0; row < 65536; ++row)
            {
                // Transform the uint of row into [nible1, nible2, nible3, nible 4]
                int[] line = new int[]
                {
                    (row >>  0) & 0xf,
                    (row >>  4) & 0xf,
                    (row >>  8) & 0xf,
                    (row >> 12) & 0xf
                };

                CalculateRowScore(row, line);

                CalculateRowTransformation((Row)row, ref line);
            }

            IsInitialize = true;
        }

        //// Calculate the score of this row
        private static void CalculateRowScore(int row, int[] line)
        {

            float score = 0;
            for (int i = 0; i < 4; ++i)
            {
                int rank = (int)line[i];
                if (rank >= 2)
                {
                    // The score is the total sum of the title and all intermediate merged tiles
                    score += (rank - 1) * (1 << rank);
                }
            }
            _scoreTable[row] = score;
        }

        public static float GetScore(Board board)
        {
            return _scoreTable[(board >> 0) & ROW_MASK] +
                   _scoreTable[(board >> 16) & ROW_MASK] +
                   _scoreTable[(board >> 32) & ROW_MASK] +
                   _scoreTable[(board >> 48) & ROW_MASK];
        }

        // Precalculate the possible transformation (execute move the the left)
        private static void CalculateRowTransformation(Row row, ref int[] line)
        {
            // Precalculate the possible transformation (execute move the the left)
            for (int i = 0; i < 3; ++i)
            {
                int j;

                // Find not empty row for merge
                for (j = i + 1; j < 4; ++j)
                {
                    if (line[j] != 0) break;
                }
                if (j == 4) break; // no more tiles to the right

                if (line[i] == 0)
                {
                    line[i] = line[j];
                    line[j] = 0;
                    i--; // retry this entry
                }
                else if (line[i] == line[j])
                {
                    if (line[i] != 0xf)
                    {
                        /* Pretend that 32768 + 32768 = 32768 (representational limit). */
                        line[i]++;
                    }
                    line[j] = 0;
                }
            }

            var result = LineArrayToRow(line);
            var reverseResult = BitArrayHelper.ReverseRow(result);
            var reverseRow = BitArrayHelper.ReverseRow(row);

            _rowLeftTable[row] = (ushort)(row ^ result);
            _rowRightTable[reverseRow] = (ushort)(reverseRow ^ reverseResult);
            _colUpTable[row] = BitArrayHelper.ConvertToColumn(row) ^ BitArrayHelper.ConvertToColumn(result);
            _colDownTable[reverseRow] = BitArrayHelper.ConvertToColumn(reverseRow) ^ BitArrayHelper.ConvertToColumn(reverseResult);


        }

        private static Row LineArrayToRow(int[] line)
        {
            Row result = (Row)((line[0] << 0) |
                                (line[1] << 4) |
                                (line[2] << 8) |
                                (line[3] << 12));
            return result;
        }

        #endregion

        public static Board PerformMove(Board board, Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return MoveUp(board);
                    break;
                case Direction.Down:
                    return MoveDown(board);
                    break;
                case Direction.Left:
                    return MoveLeft(board);
                    break;
                case Direction.Right:
                    return MoveRight(board);
                    break;
                default:
                    return board;
            }
        }

        private static Board MoveUp(Board board)
        {
            Board result = board;
            Board transposed = BitArrayHelper.Transpose(board);
            result ^= _colUpTable[transposed >> 0 & ROW_MASK] << 0;
            result ^= _colUpTable[transposed >> 16 & ROW_MASK] << 4;
            result ^= _colUpTable[transposed >> 32 & ROW_MASK] << 8;
            result ^= _colUpTable[transposed >> 48 & ROW_MASK] << 12;
            return result;
        }
        private static Board MoveDown(Board board)
        {
            Board result = board;
            Board transposed = BitArrayHelper.Transpose(board);
            result ^= _colDownTable[transposed >> 0 & ROW_MASK] << 0;
            result ^= _colDownTable[transposed >> 16 & ROW_MASK] << 4;
            result ^= _colDownTable[transposed >> 32 & ROW_MASK] << 8;
            result ^= _colDownTable[transposed >> 48 & ROW_MASK] << 12;
            return result;
        }
        private static Board MoveLeft(Board board)
        {
            Board result = board;
            result ^= (Board)(_rowLeftTable[board >> 0 & ROW_MASK]) << 0;
            result ^= (Board)(_rowLeftTable[board >> 16 & ROW_MASK]) << 16;
            result ^= (Board)(_rowLeftTable[board >> 32 & ROW_MASK]) << 32;
            result ^= (Board)(_rowLeftTable[board >> 48 & ROW_MASK]) << 48;
            return result;
        }
        private static Board MoveRight(Board board)
        {
            Board result = board;
            result ^= (Board)(_rowRightTable[board >> 0 & ROW_MASK]) << 0;
            result ^= (Board)(_rowRightTable[board >> 16 & ROW_MASK]) << 16;
            result ^= (Board)(_rowRightTable[board >> 32 & ROW_MASK]) << 32;
            result ^= (Board)(_rowRightTable[board >> 48 & ROW_MASK]) << 48;
            return result;
        }

        public static Board InsertTile(Board board, int x, int y, short value)
        {
            if (x < 0 || x >= 4 || y < 0 || y >= 4)
            {
                throw new ArgumentOutOfRangeException();
            }

            Board insertBoard = (ulong)(value & 0xf);

            // Move to right column
            insertBoard = insertBoard << (x * 4);
            // Move to right row
            insertBoard = insertBoard << (y * 16);

            return board | insertBoard;
        }



        //public static Board InsertRandomTile(Board board, Board tile)
        //{

        //}

        /// <summary>
        /// Return the nyblet at the 
        /// </summary>
        /// <param name="board"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static short GetValue(Board board, int x, int y)
        {
            if (x < 0 || x >= 4 || y < 0 || y >= 4)
            {
                throw new ArgumentOutOfRangeException();
            }

            // Shift the wanted nyblet to the last column
            board = board >> (x * 4);

            //Shift to the last row
            board = board >> (y * 16);

            // Keep the last nyblet
            board = board & 0xf;

            return (short)board;
        }


        public static string ToString(Board board)
        {
            StringBuilder result = new StringBuilder();
            int i, j;
            for (i = 0; i < 4; i++)
            {
                for (j = 0; j < 4; j++)
                {
                    short powerVal = (short) ((board) & 0xf);
                    result.Append(((powerVal == 0) ? 0 : 1 << powerVal) + " ");
                    board >>= 4;
                }
                result.AppendLine();
            }
            return result.ToString();
        }


    }
}
