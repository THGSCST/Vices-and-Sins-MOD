using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SPRTYL2PNG
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        string lastOpenedFile;
        Sprite loadedSprite;
        Color[] transparencyColor = new Color[]{ Colors.Magenta, Colors.Black, Colors.White };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void UpdatePreviewPanel()
        {
            previewPanel.Children.Clear();

            if (zoomCombo.SelectedIndex > 0)
                RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            else RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.Fant);

            var spriteImage = new Image[loadedSprite.Count];
            for (int i = 0; i < spriteImage.Length; i++)
            {
                spriteImage[i] = new Image();

                spriteImage[i].Margin = new Thickness(marginsStroke.SelectedIndex * 4);

                spriteImage[i].Stretch = Stretch.None;
                spriteImage[i].Tag = i;
                spriteImage[i].MouseDown += (sender, args) =>
                {
                    selectedFrame.Source = ((Image)sender).Source;
                    selectedFrameIndex.Content = ((Image)sender).Tag;
                    selectedFrameSize.Content = selectedFrame.Source.Width.ToString() + ";" + selectedFrame.Source.Height.ToString();
                    bottomBar.IsEnabled = true;
                };
                spriteImage[i].SnapsToDevicePixels = true;
                spriteImage[i].LayoutTransform = new ScaleTransform(zoomCombo.SelectedIndex + 1, zoomCombo.SelectedIndex + 1);
                spriteImage[i].Source = loadedSprite.ToBitmap(i, GamePalettes.ByIndex(palettePicker.SelectedIndex), transparencyColor[transparencyColorPicker.SelectedIndex]);
                previewPanel.Children.Add(spriteImage[i]);
            }
            
        }

        private void Open(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Sprite or Tileset Files|*.spr;*.tyl|Sprite Files|*.spr|Tileset Files|*.tyl";
            if (openFileDialog.ShowDialog() == true)
            {
                bottomBar.IsEnabled = false;
                previewOptionsMenu.IsEnabled = true;
                lastOpenedFile = openFileDialog.FileName;
                this.Title = openFileDialog.FileName + " - SPRTYL2PNG";
                bool header = System.IO.Path.GetExtension(openFileDialog.FileName).ToUpper() == ".SPR";
                loadedSprite = new Sprite(File.OpenRead(openFileDialog.FileName), header);
                UpdatePreviewPanel();
            }
        }

        private void SaveAs(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.FileName = lastOpenedFile;
            saveFileDialog.Filter = "Sprite Files|*.spr|Tileset Files|*.tyl";
            saveFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(lastOpenedFile);
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllBytes(saveFileDialog.FileName, loadedSprite.RawData);
                this.Title = this.Title.Remove(this.Title.Length - 1);
            }
        }
        private void previewOptionsChanged(object sender, SelectionChangedEventArgs e)
        {
            if (previewPanel != null)
                UpdatePreviewPanel();
        }

        private void ExportPNGFrame(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog exportFileDialog = new Microsoft.Win32.SaveFileDialog();
            exportFileDialog.Filter = "PNG-8|*.png";
            if (exportFileDialog.ShowDialog() == true)
            {
                //Create a 8bit PNG with transparency
                var bitmap = loadedSprite.ToBitmap(int.Parse(selectedFrameIndex.Content.ToString()), GamePalettes.ByIndex(palettePicker.SelectedIndex), Color.FromArgb(0,0,0,0));
                using (var fileStream = new FileStream(exportFileDialog.FileName, FileMode.Create))
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    encoder.Save(fileStream);
                }
            }
        }

        private void ImportPNGFrame(object sender, RoutedEventArgs e)
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
                    loadedSprite.Replace(int.Parse(selectedFrameIndex.Content.ToString()), bs, GamePalettes.ByIndex(palettePicker.SelectedIndex));
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
    }
}
