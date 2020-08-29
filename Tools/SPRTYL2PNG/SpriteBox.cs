using SPRTYL2PNG;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SPRTYL2BMP
{
    public partial class SpriteBox : Control
    {
        public int Index { get; }
        public string SizeStringCache { get; }

        public override Color BackColor { get => Color.White; }
        public override ImageLayout BackgroundImageLayout { get => ImageLayout.None; }

        public SpriteBox(int index, Sprite sprite)
        {
            Index = index;

            Bitmap bmp8 = sprite.ToBitmap(index);
            // Convert 8bpp to 32bppP
            this.BackgroundImage = new Bitmap(bmp8.Width, bmp8.Height, PixelFormat.Format32bppPArgb);
            using (var gr = Graphics.FromImage(this.BackgroundImage))
                gr.DrawImage(bmp8, new Rectangle(0, 0, bmp8.Width, bmp8.Height));

            this.Size = BackgroundImage.Size;

            SizeStringCache = this.Size.ToString();

            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

    }
}
