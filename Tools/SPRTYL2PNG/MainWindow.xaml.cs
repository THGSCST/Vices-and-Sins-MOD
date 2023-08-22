using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace SPRTYL2PNG
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        string lastOpenedFile;
        Graphics loadedGraphics;
        Color[] transparencyColor = new Color[]{ Colors.Magenta, Colors.Black, Colors.White };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void UpdatePreviewPanel()
        {
            previewPanel.Children.Clear();
            previewTilePanel.Children.Clear();

            if (zoomCombo.SelectedIndex > 0)
                RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            else RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.Fant);

            var spriteImage = new Image[loadedGraphics.Quantity];
            for (int i = 0; i < spriteImage.Length; i++)
            {
                spriteImage[i] = new Image();

                spriteImage[i].Margin = new Thickness(marginsStroke.SelectedIndex * 4);

                spriteImage[i].Stretch = Stretch.None;
                spriteImage[i].Tag = i;
                spriteImage[i].MouseDown += (sender, args) =>
                {
                    selectedTile.Source = ((Image)sender).Source;
                    selectedSpriteIndex.Content = ((Image)sender).Tag;
                    selectedSpriteSize.Content = selectedTile.Source.Width.ToString() + ", " + selectedTile.Source.Height.ToString();
                    selectedSpriteUnknow.Content = loadedGraphics.GetUnknowByIndex((int)((Image)sender).Tag);
                    bottomBar.IsEnabled = true;
                    replaceButton.IsEnabled = true;
                };
                spriteImage[i].SnapsToDevicePixels = true;
                spriteImage[i].LayoutTransform = new ScaleTransform(zoomCombo.SelectedIndex + 1, zoomCombo.SelectedIndex + 1);
                spriteImage[i].Source = loadedGraphics.ExtractBitmap(i, GamePalettes.ByIndex(palettePicker.SelectedIndex), transparencyColor[transparencyColorPicker.SelectedIndex]);
                previewPanel.Children.Add(spriteImage[i]);
            }

            TileSet tg = null;

            if(loadedGraphics is TileSet)
               tg = (TileSet)loadedGraphics;

            if (tg != null && tg.TileSetGroupInformation != null)
            {
                tileColumnDef.Width = new GridLength(250);
                var spriteTileImage = new Image[tg.TileSetGroupInformation.Length];
                for (int i = 0; i < spriteTileImage.Length; i++)
                {
                    spriteTileImage[i] = new Image();

                    spriteTileImage[i].Margin = new Thickness(marginsStroke.SelectedIndex * 4);

                    spriteTileImage[i].Stretch = Stretch.None;
                    spriteTileImage[i].Tag = i;
                    spriteTileImage[i].MouseDown += (sender, args) =>
                    {
                        selectedTile.Source = ((Image)sender).Source;
                        selectedSpriteIndex.Content = ((Image)sender).Tag;
                        selectedSpriteSize.Content = selectedTile.Source.Width.ToString() + ", " + selectedTile.Source.Height.ToString();
                        selectedSpriteUnknow.Content = tg.GetUnknowByIndex((int)((Image)sender).Tag);
                        bottomBar.IsEnabled = true;
                        replaceButton.IsEnabled = false;
                    };
                    spriteTileImage[i].SnapsToDevicePixels = true;
                    spriteTileImage[i].LayoutTransform = new ScaleTransform(zoomCombo.SelectedIndex + 1, zoomCombo.SelectedIndex + 1);
                    spriteTileImage[i].Source = tg.ExtractTileGroupBitmap(i, GamePalettes.ByIndex(palettePicker.SelectedIndex), transparencyColor[transparencyColorPicker.SelectedIndex]);
                    previewTilePanel.Children.Add(spriteTileImage[i]);
                }
            }
            else tileColumnDef.Width = new GridLength(0);
        }

        private void Open(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Sprite or Tileset Files|*.spr;*.tyl|Sprite Files|*.spr|Tileset Files|*.tyl";
            if (openFileDialog.ShowDialog() == true)
            {
                var fileName = Path.GetFileName(openFileDialog.FileName.ToLower());
                bottomBar.IsEnabled = false;
                previewOptionsMenu.IsEnabled = true;
                exportButton.IsEnabled = true;
                lastOpenedFile = openFileDialog.FileName.ToLower();
                this.Title = openFileDialog.FileName + " - SPRTYL2PNG";

                if(fileName.EndsWith(".spr"))
                    loadedGraphics = new SpriteSheet(File.ReadAllBytes(openFileDialog.FileName));

                else if (fileName.EndsWith(".tyl"))
                {
                    int size = 32;
                    bool hasHeader = false , hasUnknowTail = false;

                    switch (fileName)
                    {
                        case "medborders.tyl":
                            size = 20; hasHeader = true;
                            break;
                        case "mediumroofs.tyl":
                            size = 20;
                            break;
                        case "lowborders.tyl":
                            hasHeader = true;
                            break;
                        case "locations.tyl":
                            hasUnknowTail = true;
                            break;
                    }

                    loadedGraphics = new TileSet(hasHeader, hasUnknowTail, size, File.ReadAllBytes(openFileDialog.FileName));

                    if(fileName == "locations.tyl") // If true, load locations.dat
                    {
                        string path = openFileDialog.FileName.ToLower().Replace(".tyl", ".dat");
                        using (BinaryReader br = new BinaryReader(File.OpenRead(path)))
                        {
                            br.BaseStream.Position = 60;
                            int groupCount = (int)br.ReadInt16() * 4;
                            TileSetGroupInformation[] tsgi = new TileSetGroupInformation[groupCount];
           
                            for (int i = 0; i < groupCount; i++)
                            {
                                if (i != 0 && (i & 3) == 0) //Mulltiple of 4, not zero
                                    br.BaseStream.Position++;

                                tsgi[i] = new TileSetGroupInformation(br);
                            }

                            ((TileSet)loadedGraphics).TileSetGroupInformation = tsgi;
                        }

                                             
                    }

                }

                UpdatePreviewPanel();
            }
        }

        private void SaveAs(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.FileName = lastOpenedFile;
            saveFileDialog.Filter = "Sprite Files|*.spr|Tileset Files|*.tyl";
            saveFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(lastOpenedFile);
            if (lastOpenedFile.EndsWith(".tyl")) saveFileDialog.FilterIndex = 1;
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllBytes(saveFileDialog.FileName, loadedGraphics.RawData);
                this.Title = this.Title.Remove(this.Title.Length - 1);
            }
        }
        private void previewOptionsChanged(object sender, SelectionChangedEventArgs e)
        {
            if (previewPanel != null)
                UpdatePreviewPanel();
        }

        private void ExportPNGSprite(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog exportFileDialog = new Microsoft.Win32.SaveFileDialog();
            exportFileDialog.Filter = "PNG-8|*.png";
            if (exportFileDialog.ShowDialog() == true)
            {
                //Create a 8bit PNG with transparency

                //var bitmap = loadedGraphics.ExtractBitmap(int.Parse(selectedSpriteIndex.Content.ToString()), GamePalettes.ByIndex(palettePicker.SelectedIndex), Color.FromArgb(0,0,0,0));
                var bitmap = selectedTile.Source;
                using (var fileStream = new FileStream(exportFileDialog.FileName, FileMode.Create))
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmap as BitmapSource));
                    encoder.Save(fileStream);
                }
            }
        }

        private void ImportPNGSprite(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "PNG-8|*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                PngBitmapDecoder pngDec = new PngBitmapDecoder(new System.Uri(openFileDialog.FileName),
                BitmapCreateOptions.None,
                BitmapCacheOption.Default);

                BitmapSource bs = pngDec.Frames[0];

                try
                {
                    loadedGraphics.OverwriteBitmap(int.Parse(selectedSpriteIndex.Content.ToString()), bs, GamePalettes.ByIndex(palettePicker.SelectedIndex));
                }
                catch (NotImplementedException)
                {
                    MessageBox.Show(
                    "No support for 32bits PNGs! Only indexed 8bits PNGs can be used.",
                    "PNG FORMAT ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (InvalidDataException)
                {
                    MessageBox.Show(
                    "PNG color palette does not match. The PNG-8 shall be indexed and palette must be identical to the game graphics.",
                    "PALETTE ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (ArgumentException)
                {
                    MessageBox.Show(
                    "PNG and sprite size are different. Both need to be the same size.",
                    "DIFFERENT SIZE ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                UpdatePreviewPanel();

                openButton.IsEnabled = false;
                saveAsButton.IsEnabled = true;
                this.Title += "*";
            }

        }

        private void ExportAll(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog exportFileDialog = new Microsoft.Win32.SaveFileDialog();
            exportFileDialog.Filter = "PNG-8|*.png";
            exportFileDialog.Title = "All frames will be exported";
            exportFileDialog.FileName = "X.png";
            if (exportFileDialog.ShowDialog() == true)
            {
                for (int i = 0; i < loadedGraphics.Quantity; i++)
                {
                    string exportName = System.IO.Path.GetDirectoryName(exportFileDialog.FileName) + "\\" + i.ToString() + ".PNG";
                    //Create a 8bit PNG with transparency
                    var bitmap = loadedGraphics.ExtractBitmap(i, GamePalettes.ByIndex(palettePicker.SelectedIndex), Color.FromArgb(0, 0, 0, 0));
                    using (var fileStream = new FileStream(exportName, FileMode.Create))
                    {
                        BitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(bitmap));
                        encoder.Save(fileStream);
                    }

                }
            }
        }
    }
}
