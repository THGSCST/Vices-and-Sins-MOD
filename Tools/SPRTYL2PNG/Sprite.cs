using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace SPRTYL2PNG
{
    public class Sprite
    {
        public int spriteQuantity;
        public Entry[] entries;
        public byte[] rawDataBitmaps;
        public struct Entry
        {
            public ushort bmWidth;
            public ushort bmHeight;
            public int unknow;
            public int unknow2;
            public int datatOffset;
        }
        
        public Sprite(Stream stream, bool header)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            BinaryReader bReader = new BinaryReader(stream);

            if (header) //Read the sprite header
            {
                spriteQuantity = bReader.ReadInt32();
                if (spriteQuantity < 1)
                    throw new InvalidDataException("Sprite is empty, invalid or has no header");

                entries = new Entry[spriteQuantity];

                for (int i = 0; i < spriteQuantity; i++)
                {
                    entries[i].bmWidth = bReader.ReadUInt16();
                    entries[i].bmHeight = bReader.ReadUInt16();
                    entries[i].unknow = bReader.ReadInt32();
                    entries[i].unknow2 = bReader.ReadInt32();
                    entries[i].datatOffset = bReader.ReadInt32();
                }
            }
            else //Its a tileset, no header. Try to create header.
            {
                int size = 32;
                if (bReader.BaseStream.Length == 71200 || bReader.BaseStream.Length == 5204) size = 20;

                spriteQuantity = (int)bReader.BaseStream.Length / (size * size);
                if (bReader.BaseStream.Length == 3189884) spriteQuantity = (int)bReader.BaseStream.Length / (size * size + 4) - 1;

                entries = new Entry[spriteQuantity];

                for (int i = 0; i < spriteQuantity; i++)
                {
                    entries[i].bmWidth = (ushort)size;
                    entries[i].bmHeight = (ushort)size;
                    entries[i].datatOffset = i * size * size;
                    if (bReader.BaseStream.Length == 3189884)
                    {
                        bReader.BaseStream.Position += size * size;
                        entries[i].unknow = bReader.ReadInt32();
                        entries[i].datatOffset = (int)bReader.BaseStream.Position;
                    }
                    if (bReader.BaseStream.Length == 8196 || bReader.BaseStream.Length == 5204)
                        entries[i].datatOffset += 4;
                }
            }

            int totalBytes = 0;
            foreach (var entry in entries)
                totalBytes += (int) entry.bmHeight * entry.bmWidth;

            bReader.BaseStream.Position = 0;
            rawDataBitmaps = bReader.ReadBytes((int)bReader.BaseStream.Length);
        }

        public Bitmap ToBitmap(int index)
        {
            int total = entries[index].bmHeight * entries[index].bmWidth;
            int pos = entries[index].datatOffset;
            var segment = new ArraySegment<byte>(rawDataBitmaps, pos , total);
            return Make8bppBitmap(segment.ToArray(), entries[index].bmWidth, entries[index].bmHeight, Pallete.Default);  
        }

        public void Replace(int index, Bitmap bitmap)
        {
            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

            // Remove stripe padding and copy the RGB values
            for (int y = 0; y < bmpData.Height; ++y)
            {
                IntPtr mem = (IntPtr)((long)bmpData.Scan0 + y * bmpData.Stride);
                int start = entries[index].datatOffset + y * bmpData.Width;
                Marshal.Copy(mem, rawDataBitmaps, start, bmpData.Width);
            }
            // Unlock the bits.
            bitmap.UnlockBits(bmpData);
        }

        public void Save(string filename)
        {

        }
        public static Bitmap Make8bppBitmap(Byte[] sourceData, Int32 width, Int32 height, Color[] palette)
        {
            Bitmap newImage = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            BitmapData targetData = newImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, newImage.PixelFormat);
            Int32 newDataWidth = ((Image.GetPixelFormatSize(PixelFormat.Format8bppIndexed) * width) + 7) / 8;
            Int32 targetStride = targetData.Stride;
            Int64 scan0 = targetData.Scan0.ToInt64();
            for (Int32 y = 0; y < height; y++)
                Marshal.Copy(sourceData, y * width, new IntPtr(scan0 + y * targetStride), newDataWidth);
            newImage.UnlockBits(targetData);

            ColorPalette pal = newImage.Palette;
            for (Int32 i = 0; i < pal.Entries.Length; i++)
                pal.Entries[i] = palette[i];
            newImage.Palette = pal;
            
            return newImage;
        }

    }
}
