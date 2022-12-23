using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SPRTYL2PNG
{
    internal class TileSet : Graphics
    {
        public TileSetGroupInformation[] TileSetGroupInformation { get; set; }
        public bool QuantityHeader { get; }
        public bool UnknowTail { get; }
        public int Size { get; }

        public override int Quantity { get; }

        public TileSet(bool quantityHeader, bool unknowTail, int size, byte[] rawData) : base(rawData)
        {
            QuantityHeader = quantityHeader;
            UnknowTail = unknowTail;
            Size = size;

            if (QuantityHeader)
                Quantity = (rawData.Length - 4) / (Size * Size);
            else if (UnknowTail)
                Quantity = rawData.Length / (Size * Size + 4);
            else
                Quantity = rawData.Length / (Size * Size);
        }

        internal override uint GetUnknowByIndex(int index)
        {
            if (UnknowTail)
            {
                int offsetTail = (1024 + 4) * index + 1024;
                return BitConverter.ToUInt32(RawData, offsetTail);
            }
            else
                return 0;
        }
        internal override int OffsetByIndex(int index)
        {
            int offset = index * Size * Size;
            if (QuantityHeader)
                offset += 4;
            else if (UnknowTail)
                offset += 4 * index;
            return offset;
        }

        internal override int HeightByIndex(int index)
        {
            return Size;
        }

        internal override int WidthByIndex(int index)
        {
            return Size;
        }

        internal BitmapSource ExtractTileGroupBitmap(int index, List<Color> palette, Color transparency)
        {
            
            var tileGroup = TileSetGroupInformation[index];
            palette[0] = transparency;
            int tileIndex = 0;
            int x = 0;
            int y = 0;
            var wbmp = new WriteableBitmap(tileGroup.Horizontal * 32, tileGroup.Vertical * 32, 96, 96, PixelFormats.Indexed8, new BitmapPalette(palette));
            for (int h = 0; h < tileGroup.Horizontal; h++)
            {
                int v = 0;
                if (h % 2 == 0)
                {
                    y = 16;
                    v = 1;
                }
                else
                    y = 0;

                for (; v < tileGroup.Vertical; v++)
                {
                    if (tileGroup.TilesIndexes[tileIndex] != 0)
                    {
                        var tile = this.ExtractBitmap(tileGroup.TilesIndexes[tileIndex], palette, transparency);
                        byte[] pixels = new byte[(int)(tile.Width * tile.Height)];
                        tile.CopyPixels(pixels, 32, 0);
                        wbmp.WritePixels(new Int32Rect(x, y, 32, 32), pixels, 32, 0);
                    }
                    y += 32;
                    tileIndex++;
                }
                x += 32;
            }

            return BitmapFrame.Create(wbmp);
        }
    }
}
