using System.Windows.Forms;

namespace SPRTYL2PNG
{
    partial class MainForm
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.openBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveButton = new System.Windows.Forms.ToolStripButton();
            this.openDialogSpriteTileset = new System.Windows.Forms.OpenFileDialog();
            this.flowPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.nameStrip = new System.Windows.Forms.StatusStrip();
            this.spriteNumberLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.spriteSizeLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.spriteContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportAsPNGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.replaceWithPNGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportDialogPNG = new System.Windows.Forms.SaveFileDialog();
            this.openDialogPNG = new System.Windows.Forms.OpenFileDialog();
            this.saveDialogSpriteTileset = new System.Windows.Forms.SaveFileDialog();
            this.toolStrip1.SuspendLayout();
            this.nameStrip.SuspendLayout();
            this.spriteContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openBtn,
            this.toolStripSeparator1,
            this.saveButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1094, 59);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // openBtn
            // 
            this.openBtn.Image = global::SPRTYL2BMP.Properties.Resources.Folder_32x;
            this.openBtn.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.openBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openBtn.Name = "openBtn";
            this.openBtn.Size = new System.Drawing.Size(49, 56);
            this.openBtn.Text = "Open";
            this.openBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.openBtn.Click += new System.EventHandler(this.openBtn_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 59);
            // 
            // saveButton
            // 
            this.saveButton.Enabled = false;
            this.saveButton.Image = global::SPRTYL2BMP.Properties.Resources.Save_32x;
            this.saveButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.saveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(44, 56);
            this.saveButton.Text = "Save";
            this.saveButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // openDialogSpriteTileset
            // 
            this.openDialogSpriteTileset.Filter = "Sprite or Tileset Files|*.spr;*.tyl|Sprite Files|*.spr|Tileset Files|*.tyl";
            // 
            // flowPanel
            // 
            this.flowPanel.AutoScroll = true;
            this.flowPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.flowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowPanel.Location = new System.Drawing.Point(0, 59);
            this.flowPanel.Name = "flowPanel";
            this.flowPanel.Size = new System.Drawing.Size(1094, 382);
            this.flowPanel.TabIndex = 5;
            // 
            // nameStrip
            // 
            this.nameStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.nameStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.spriteNumberLabel,
            this.spriteSizeLabel});
            this.nameStrip.Location = new System.Drawing.Point(0, 441);
            this.nameStrip.Name = "nameStrip";
            this.nameStrip.Size = new System.Drawing.Size(1094, 22);
            this.nameStrip.TabIndex = 6;
            this.nameStrip.Text = "statusStrip1";
            // 
            // spriteNumberLabel
            // 
            this.spriteNumberLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.spriteNumberLabel.Name = "spriteNumberLabel";
            this.spriteNumberLabel.Size = new System.Drawing.Size(4, 16);
            // 
            // spriteSizeLabel
            // 
            this.spriteSizeLabel.Name = "spriteSizeLabel";
            this.spriteSizeLabel.Size = new System.Drawing.Size(0, 16);
            // 
            // spriteContextMenuStrip
            // 
            this.spriteContextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.spriteContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.exportAsPNGToolStripMenuItem,
            this.toolStripSeparator2,
            this.replaceWithPNGToolStripMenuItem});
            this.spriteContextMenuStrip.Name = "spriteContextMenuStrip";
            this.spriteContextMenuStrip.Size = new System.Drawing.Size(206, 82);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(205, 24);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // exportAsPNGToolStripMenuItem
            // 
            this.exportAsPNGToolStripMenuItem.Name = "exportAsPNGToolStripMenuItem";
            this.exportAsPNGToolStripMenuItem.Size = new System.Drawing.Size(205, 24);
            this.exportAsPNGToolStripMenuItem.Text = "Export as PNG...";
            this.exportAsPNGToolStripMenuItem.Click += new System.EventHandler(this.exportAsPNGToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(202, 6);
            // 
            // replaceWithPNGToolStripMenuItem
            // 
            this.replaceWithPNGToolStripMenuItem.Name = "replaceWithPNGToolStripMenuItem";
            this.replaceWithPNGToolStripMenuItem.Size = new System.Drawing.Size(205, 24);
            this.replaceWithPNGToolStripMenuItem.Text = "Replace with PNG...";
            this.replaceWithPNGToolStripMenuItem.Click += new System.EventHandler(this.replaceWithPNGToolStripMenuItem_Click);
            // 
            // exportDialogPNG
            // 
            this.exportDialogPNG.Filter = "PNG|*.png";
            // 
            // openDialogPNG
            // 
            this.openDialogPNG.Filter = "PNG|*.png";
            // 
            // saveDialogSpriteTileset
            // 
            this.saveDialogSpriteTileset.Filter = "Sprite or Tileset Files|*.spr;*.tyl|Sprite Files|*.spr|Tileset Files|*.tyl";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1094, 463);
            this.Controls.Add(this.flowPanel);
            this.Controls.Add(this.nameStrip);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MainForm";
            this.Text = "SPRTYL2PNG";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.nameStrip.ResumeLayout(false);
            this.nameStrip.PerformLayout();
            this.spriteContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton openBtn;
        private System.Windows.Forms.OpenFileDialog openDialogSpriteTileset;
        private System.Windows.Forms.FlowLayoutPanel flowPanel;
        private System.Windows.Forms.StatusStrip nameStrip;
        private System.Windows.Forms.ContextMenuStrip spriteContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportAsPNGToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem replaceWithPNGToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel spriteNumberLabel;
        private System.Windows.Forms.ToolStripStatusLabel spriteSizeLabel;
        private System.Windows.Forms.ToolStripButton saveButton;
        private SaveFileDialog exportDialogPNG;
        private OpenFileDialog openDialogPNG;
        private ToolStripSeparator toolStripSeparator1;
        private SaveFileDialog saveDialogSpriteTileset;
    }
}

