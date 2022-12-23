using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPRTYL2PNG
{
    internal readonly struct SpriteInformation
    {
        public ushort Width { get; }
        public ushort Height { get; }
        public ushort OffsetX { get; }
        public ushort OffsetY { get; }
        public uint Unknow { get; }
        public int DataOffset { get; }

        public int ByteLenght { get => Width * Height; }

        private SpriteInformation(ushort width, ushort height, ushort offsetX, ushort offsetY, uint unknow, int dataOffset)
        {
            Width = width;
            Height = height;
            OffsetX = offsetX;
            OffsetY = offsetY;
            Unknow = unknow;
            DataOffset = dataOffset;
        }

        public SpriteInformation(BinaryReader reader) : this(reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16(),
                reader.ReadUInt32(), reader.ReadInt32())
        { }
    }
}
