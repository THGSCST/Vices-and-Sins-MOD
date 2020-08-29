using SPRTYL2BMP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace SPRTYL2PNG
{
    public partial class MainForm : Form
    {
        Sprite spr;
        int currentSelection;
        public MainForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        private void openBtn_Click(object sender, EventArgs e)
        {
            if (openDialogSpriteTileset.ShowDialog() == DialogResult.OK)
            {
                saveButton.Enabled = false;

                this.Text = openDialogSpriteTileset.FileName + " - SPRTYL2PNG";
                bool header = Path.GetExtension(openDialogSpriteTileset.FileName).ToUpper() == ".SPR";
                spr = new Sprite(File.OpenRead(openDialogSpriteTileset.FileName), header);

                UpdateFlowPanel();
            }
        }

        private void pictureBox_MouseEnter(object sender, EventArgs e)
        {
            var pic = (SpriteBox)sender;
            currentSelection = pic.Index;
            spriteNumberLabel.Text = "Sprite " + (currentSelection + 1).ToString() + " of " + flowPanel.Controls.Count.ToString();
            spriteSizeLabel.Text = pic.SizeStringCache;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fullPath = Path.GetTempPath() + currentSelection.ToString() + ".png";
            spr.ToBitmap(currentSelection).Save(fullPath, ImageFormat.Png);
            Process.Start(fullPath);
        }

        private void exportAsPNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (exportDialogPNG.ShowDialog() == DialogResult.OK)
                spr.ToBitmap(currentSelection).Save(exportDialogPNG.FileName, ImageFormat.Png);
        }

        private void replaceWithPNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openDialogPNG.ShowDialog() == DialogResult.OK)
            {
                PngBitmapDecoder pngDec = new PngBitmapDecoder(new Uri(openDialogPNG.FileName),
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.OnDemand);
                BmpBitmapEncoder bmpEnc = new BmpBitmapEncoder();
                bmpEnc.Frames.Add(pngDec.Frames[0]);
                Bitmap bmp = null;
                using (MemoryStream stream = new MemoryStream())
                {
                    bmpEnc.Save(stream);
                    bmp = new Bitmap(stream);
                }
                pngDec = null;
                bmpEnc = null;
                spr.Replace(currentSelection, bmp);

                UpdateFlowPanel();

                saveButton.Enabled = true;
                this.Text += "*";
            }
        }

        private void UpdateFlowPanel()
        {
            flowPanel.Visible = false;
            flowPanel.Controls.Clear();
            for (int i = 0; i < spr.entries.Count(); i++)
            {
                var pic = new SpriteBox(i, spr);

                pic.ContextMenuStrip = spriteContextMenuStrip;

                pic.MouseEnter += new System.EventHandler(this.pictureBox_MouseEnter);
                flowPanel.Controls.Add(pic);
            }
            flowPanel.Visible = true;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            saveDialogSpriteTileset.FileName = openDialogSpriteTileset.FileName;
            saveDialogSpriteTileset.InitialDirectory = Path.GetDirectoryName(openDialogSpriteTileset.FileName);
            if (saveDialogSpriteTileset.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(saveDialogSpriteTileset.FileName, spr.rawDataBitmaps);
                this.Text = this.Text.Remove(this.Text.Length - 1);
            }
        }
    }
}
