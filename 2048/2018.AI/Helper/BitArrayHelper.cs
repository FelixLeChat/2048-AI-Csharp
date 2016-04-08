
using Board = System.UInt64;
using Row = System.UInt16;

namespace _2048.AI.Helper
{
    public static class BitArrayHelper
    {
        public const Board ROW_MASK = 0xFFFF;
        public const Board COL_MASK = 0x000F000F000F000F;

        #region Bit Operation Helper
        public static Board ConvertToColumn(Row row)
        {
            Board tmp = row;
            return (tmp | (tmp << 12) | (tmp << 24) | (tmp << 36)) & COL_MASK;
        }

        public static Row ReverseRow(Row row)
        {
            return (ushort)((row >> 12) | ((row >> 4) & 0x00F0) | ((row << 4) & 0x0F00) | (row << 12));
        }

        // Transpose rows/columns in a board:
        //   0123       048c
        //   4567  -->  159d
        //   89ab       26ae
        //   cdef       37bf
        public static Board Transpose(Board board)
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

        public static int CountEmpty(Board x)
        {
            x |= (x >> 2) & 0x3333333333333333;
            x |= (x >> 1);
            x = ~x & 0x1111111111111111;
            // At this point each nibble is:
            //  0 if the original nibble was non-zero
            //  1 if the original nibble was zero
            // Next sum them all
            x += x >> 32;
            x += x >> 16;
            x += x >> 8;
            x += x >> 4; // this can overflow to the next nibble if there were 16 empty positions
            return (int) (x & 0xf);
        }

        #endregion
    }
}