using System.Linq;
using _2048.Model;

namespace _2048.WPF.Helper
{
    public class Helper
    {
        public static bool Equal(Cell[][] data1, Cell[][] data2)
        {
            return data1.Rank == data2.Rank &&
                Enumerable.Range(0, data1.Rank).All(dimension => data1.GetLength(dimension) == data2.GetLength(dimension)) &&
                data1.SequenceEqual(data2);
        }

        /// <summary>
        /// Return true is all tiles are populated on the board
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool IsFullTileSet(Cell[][] data)
        {
            return data.SelectMany(col => col).All(cell => !cell.IsEmpty());
        }

        public static int GetMaxTile(Cell[][] data)
        {
            return (from coll in data from cell in coll select cell.Value).Concat(new[] {0}).Max();
        }
    }
}