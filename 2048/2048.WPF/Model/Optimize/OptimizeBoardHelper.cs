using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _2048.WPF.Game;
using Board = System.UInt64;
using Row = System.UInt16;


namespace _2048.WPF.Model
{
    /// <summary>
    /// Inspired from https://github.com/nneonneo/2048-ai
    /// </summary>

    public class OptimizeBoardHelper
    {
        private const Board ROW_MASK = 0xFFFF;
        private const Board COL_MASK = 0x000F000F000F000F;


        #region Bit Operation Helper
        private static Board ConvertToColumn(Row row)
        {
            Board tmp = row;
            return (tmp | (tmp << 12) | (tmp << 24) | (tmp << 36)) & COL_MASK;
        }

        private static Row ReverseRow(Row row)
        {
            return (ushort)((row >> 12) | ((row >> 4) & 0x00F0) | ((row << 4) & 0x0F00) | (row << 12));
        }

        // Transpose rows/columns in a board:
        //   0123       048c
        //   4567  -->  159d
        //   89ab       26ae
        //   cdef       37bf
        private static Board Transpose(Board board)
        {
            Board a1 = board & 0xF0F00F0FF0F00F0F;
            Board a2 = board & 0x0000F0F00000F0F0;
            Board a3 = board & 0x0F0F00000F0F0000;
            Board a = a1 | (a2 << 12) | (a3 >> 12);
            Board b1 = a & 0xFF00FF0000FF00FF;
            Board b2 = a & 0x00FF00FF00000000;
            Board b3 = a & 0x00000000FF00FF00;
            return b1 | (b2 >> 24) | (b3 << 24);
        }

        #endregion

        // Pre-calculate all possible result of move for row
        // A row is 16 bits: so 65536 possibility
        #region Lookup Table
        static readonly Row[] _rowLeftTable = new Row[65536];
        static readonly Row[] _rowRightTable = new Row[65536];
        static readonly Board[] _colUpTable = new Board[65536];
        static readonly Board[] _colDownTable = new Board[65536];

        static readonly float[] _scoreTable = new float[65536];

        private void InitLookupTable()
        {
            for (Row row = 0; row < 65536; ++row)
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

                CalculateRowTransformation(row, ref line);

            }
        }

        // TODO: Felix exemple comment calculer score
        // Calculate the score of this row
        private void CalculateRowScore(uint row, int[] line)
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

        // Precalculate the possible transformation (execute move the the left)
        private void CalculateRowTransformation(Row row, ref int[] line)
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
            var reverseResult = ReverseRow(result);
            var reverseRow = ReverseRow(row);

            _rowLeftTable[row] = (ushort)(row ^ result);
            _rowLeftTable[reverseRow] = (ushort)(reverseRow ^ reverseResult);
            _colUpTable[row] = ConvertToColumn(row) ^ ConvertToColumn(result);
            _colDownTable[reverseRow] = ConvertToColumn(reverseRow) ^ ConvertToColumn(reverseResult);


        }

        private Row LineArrayToRow(int[] line)
        {
            Row result = (Row)((line[0] << 0) |
                                (line[1] << 4) |
                                (line[2] << 8) |
                                (line[3] << 12));
            return result;
        }

        #endregion

        public static Board PerformMove(Board board ,Direction direction)
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
                    return 0;
            }
        }

        private static Board MoveUp(Board board)
        {
            Board result = board;
            Board transposed = Transpose(board);
            result ^= _colUpTable[transposed >> 0 & ROW_MASK] << 0;
            result ^= _colUpTable[transposed >> 16 & ROW_MASK] << 4;
            result ^= _colUpTable[transposed >> 32 & ROW_MASK] << 8;
            result ^= _colUpTable[transposed >> 48 & ROW_MASK] << 12;
            return result;
        }
        private static Board MoveDown(Board board)
        {
            Board result = board;
            Board transposed = Transpose(board);
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

        public static void InsertRandomTile(Board board, Board tile)
        {

        }


    }
}
