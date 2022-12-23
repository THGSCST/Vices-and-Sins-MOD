using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SPRTYL2PNG
{
    public class SpriteSheet : Graphics
    {
        private SpriteInformation[] sprites;

        public override int Quantity { get => sprites.Length; }
        public SpriteSheet(byte[] rawData) : base(rawData)
        {
            if (rawData == null || rawData.Length == 0)
                throw new ArgumentNullException(nameof(rawData));

            var bReader = new BinaryReader(new MemoryStream(rawData));

            int spriteQuantity = bReader.ReadInt32();
            if (spriteQuantity < 1)
                throw new InvalidDataException("Sprite is empty, invalid or has no header");

            sprites = new SpriteInformation[spriteQuantity];

            for (int i = 0; i < spriteQuantity; i++)
                sprites[i] = new SpriteInformation(bReader);
        }

        internal override int WidthByIndex(int index)
        {
            return sprites[index].Width;
        }

        internal override int HeightByIndex(int index)
        {
            return sprites[index].Height;
        }
        internal override int OffsetByIndex(int index)
        {
            return sprites[index].DataOffset;
        }
        internal override uint GetUnknowByIndex(int index)
        {
            return sprites[index].Unknow;
        }

    }
}
