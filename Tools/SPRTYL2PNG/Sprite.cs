using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SPRTYL2PNG
{
    public class Sprite
    {
        private FrameInformation[] frames;
        private byte[] rawData;
        private struct FrameInformation
        {
            public ushort bmWidth;
            public ushort bmHeight;
            public int unknow;
            public int unknow2;
            public int datatOffset;
        }

        public int Count { get => frames.Length; }
        public byte[] RawData { get => rawData; }
        public Sprite(Stream stream, bool header)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            BinaryReader bReader = new BinaryReader(stream);

            int spriteQuantity;

            if (header) //Read the sprite header
            {
                spriteQuantity = bReader.ReadInt32();
                if (spriteQuantity < 1)
                    throw new InvalidDataException("Sprite is empty, invalid or has no header");

                frames = new FrameInformation[spriteQuantity];

                for (int i = 0; i < spriteQuantity; i++)
                {
                    frames[i].bmWidth = bReader.ReadUInt16();
                    frames[i].bmHeight = bReader.ReadUInt16();
                    frames[i].unknow = bReader.ReadInt32();
                    frames[i].unknow2 = bReader.ReadInt32();
                    frames[i].datatOffset = bReader.ReadInt32();
                }
            }
            else //Its a tileset, no header. Try to create header.
            {
                int size = 32;
                if (bReader.BaseStream.Length == 71200 || bReader.BaseStream.Length == 5204) size = 20;

                spriteQuantity = (int)bReader.BaseStream.Length / (size * size);
                if (bReader.BaseStream.Length == 3189884) spriteQuantity = (int)bReader.BaseStream.Length / (size * size + 4) - 1;

                frames = new FrameInformation[spriteQuantity];

                for (int i = 0; i < spriteQuantity; i++)
                {
                    frames[i].bmWidth = (ushort)size;
                    frames[i].bmHeight = (ushort)size;
                    frames[i].datatOffset = i * size * size;
                    if (bReader.BaseStream.Length == 3189884)
                    {
                        bReader.BaseStream.Position += size * size;
                        frames[i].unknow = bReader.ReadInt32();
                        frames[i].datatOffset = (int)bReader.BaseStream.Position;
                    }
                    if (bReader.BaseStream.Length == 8196 || bReader.BaseStream.Length == 5204)
                        frames[i].datatOffset += 4;
                }
            }

            int totalBytes = 0;
            foreach (var entry in frames)
                totalBytes += (int)entry.bmHeight * entry.bmWidth;

            bReader.BaseStream.Position = 0;
            rawData = bReader.ReadBytes((int)bReader.BaseStream.Length);
        }

        public BitmapSource ToBitmap(int index, List<Color> palette, Color transparency)
        {
            int total = frames[index].bmHeight * frames[index].bmWidth;
            int pos = frames[index].datatOffset;
            var segment = new ArraySegment<byte>(rawData, pos, total);
            return Make8bppBitmap(segment.ToArray(), frames[index].bmWidth, frames[index].bmHeight, palette, transparency);
        }

        public void Replace(int index, BitmapSource bitmap, IList<Color> palette)
        {
            if (bitmap.Palette == null) throw new NotImplementedException("No palette! PNG not indexed");

            //Check if palette match
            bool match = true;
            if (bitmap.Palette.Colors.Count == palette.Count)
            {
                for (int i = 1; i < palette.Count; i++) //Skip firt color
                    if (bitmap.Palette.Colors[i] != palette[i])
                    {
                        match = false;
                        break;
                    }
            }
            else match = false;

            if (!match)
                throw new InvalidDataException("Color palette does not match");

            int total = frames[index].bmHeight * frames[index].bmWidth;
            int stride = (int)bitmap.PixelWidth * (bitmap.Format.BitsPerPixel / 8);
            if (total != stride)
                throw new ArgumentException("Size mismatch");

            int pos = frames[index].datatOffset;
            byte[] pixels = new byte[bitmap.PixelHeight * stride];
            bitmap.CopyPixels(pixels, stride, 0);

            for (int i = 0; i < total; i++)
                rawData[pos + i] = pixels[i];
        }

        private static BitmapSource Make8bppBitmap(Byte[] rawImage, Int32 width, Int32 height, List<Color> palette, Color transparency)
        {
            PixelFormat pf = PixelFormats.Indexed8;
            int rawStride = (width * pf.BitsPerPixel + 7) / 8;

            if(rawImage.Length != rawStride * height)
                throw new ArgumentException("Size mismatch");

            List<Color> modPalette = new List<Color>(palette);
            var pal = new BitmapPalette(modPalette);
            modPalette[0] = transparency;


            return BitmapSource.Create(width, height, 96, 96, pf, new BitmapPalette(modPalette), rawImage, rawStride);
        }

    }
}
