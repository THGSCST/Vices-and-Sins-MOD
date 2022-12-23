using System.IO;
using System.Windows.Controls;

namespace SPRTYL2PNG
{
    internal struct TileSetGroupInformation
    {
        public int Horizontal { get; }
        public int Vertical { get; }
        public int[] TilesIndexes { get; }

        public TileSetGroupInformation(int horizontal, int vertical, int[] tilesIndexes)
        {
            Horizontal = horizontal;
            Vertical = vertical;
            TilesIndexes = tilesIndexes;
        }

        public TileSetGroupInformation(BinaryReader reader)
        {
            Horizontal = reader.ReadInt32();
            Vertical = reader.ReadInt32();
            TilesIndexes = new int[Horizontal * Vertical - Horizontal / 2];

            for (int i = 0; i < TilesIndexes.Length; i++)
                TilesIndexes[i] = reader.ReadInt32();
        }
    }
}