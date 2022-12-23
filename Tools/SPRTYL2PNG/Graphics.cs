using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace SPRTYL2PNG
{
    public abstract class Graphics
    {
        public abstract int Quantity { get; }
        public byte[] RawData { get; }

        protected Graphics(byte[] rawData)
        {
            RawData = rawData;
        }

        internal abstract int WidthByIndex(int index);
        internal abstract int HeightByIndex(int index);
        internal abstract int OffsetByIndex(int index);
        internal abstract uint GetUnknowByIndex(int index);

        internal int ByteSizeByIndex(int index)
        {
            return WidthByIndex(index) * HeightByIndex(index);
        }

        public BitmapSource ExtractBitmap(int index, List<Color> palette, Color transparency)
        {
            var segment = new ArraySegment<byte>(RawData, OffsetByIndex(index), ByteSizeByIndex(index));
            return Make8bppBitmap(segment.ToArray(), WidthByIndex(index), HeightByIndex(index), palette, transparency);
        }

        public void OverwriteBitmap(int index, BitmapSource bitmap, IList<Color> palette)
        {
            if (bitmap.Format != PixelFormats.Indexed8)
                throw new NotImplementedException("No palette! PNG format must be indexed with 256 colors (8-bits)");

            //Check if palette match
            bool match = bitmap.Palette.Colors.Count == palette.Count;
            for (int i = 1; i < palette.Count && match; i++) //Skip firt color
                if (bitmap.Palette.Colors[i] != palette[i])
                    match = false;

            if (!match)
                throw new InvalidDataException("Color palette does not match");

            if (bitmap.PixelWidth != WidthByIndex(index)
                || bitmap.PixelHeight != HeightByIndex(index))
                throw new ArgumentException("Size mismatch");

            int bSize = ByteSizeByIndex(index);
            int pos = OffsetByIndex(index);
            byte[] pixels = new byte[bSize];
            bitmap.CopyPixels(pixels, bitmap.PixelWidth, 0);

            for (int i = 0; i < bSize; i++)
                RawData[pos + i] = pixels[i];
        }

        private static BitmapSource Make8bppBitmap(byte[] rawImage, int width, int height, List<Color> palette, Color transparency)
        {
            int rawStride = (width * PixelFormats.Indexed8.BitsPerPixel + 7) / 8;
            if (rawImage.Length != rawStride * height)
                throw new ArgumentException("Size mismatch");
            palette[0] = transparency;

            return BitmapSource.Create(width, height, 96, 96, PixelFormats.Indexed8, new BitmapPalette(palette), rawImage, rawStride);
        }
    }
}
