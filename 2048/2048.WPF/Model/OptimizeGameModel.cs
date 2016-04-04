using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _2048.WPF.Game;
using _2048.WPF.Model.Interface;

using Board = System.UInt64;
using Row = System.UInt16;

namespace _2048.WPF.Model
{
    public class OptimizeGameModel : IGameModel
    {
        private const Board ROW_MASK = 0xFFFF;
        private const Board COL_MASK = 0x000F000F000F000F;

       
        static private Board ConvertToColumn(Row row)
        {
            Board tmp = row;
            return (tmp | (tmp << 12) | (tmp << 24) | (tmp << 36)) &COL_MASK;
        }

        static private Row ReverseRow(Row row)
        {
            return (ushort) ((row >> 12) | ((row >> 4) & 0x00F0)  | ((row << 4) & 0x0F00) | (row << 12));
        }

        public bool PerformMove(Direction direction)
        {
            throw new NotImplementedException();
        }
    }
}
