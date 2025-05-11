using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using Microsoft.Win32;
using SkiaSharp;

namespace Sprite_Stacking_Visualiser
{
    /// <summary>
    /// Interaction logic for AddSpriteStackWindow.xaml
    /// </summary>
    public partial class AddSpriteStackWindow : Window
    {
        public List<string> FramePaths { get; private set; } = new List<string>();
        public int SpriteOffsetX { get; set; } = 7; // Default value for sprite offse

        public string SpriteStackName { get; set; } = string.Empty; // Default value for sprite stack name

        public AddSpriteStackWindow()
        {
            InitializeComponent();
        }

        private void Btn_UploadFrames_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Txt_SpriteStackName.Text))
            {
                MessageBox.Show("Please enter a stack name.");
                return;
            }
            if (string.IsNullOrWhiteSpace(Txt_SpriteOffset.Text))
            {
                MessageBox.Show("Please enter a sprite offset.");
                return;
            }

            // Open file dialog to select image files
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Image Files|*.png;*.jpg"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                SpriteStackName = Txt_SpriteStackName.Text.Trim(); // Get the stack name from the text box - trtimmed to remove any leading or trailing whitespace
                SpriteOffsetX = int.TryParse(Txt_SpriteOffset.Text, out int offset) ? offset : 0; // Get the sprite offset from the text box - default to 0 if not a valid integer

                string assetsFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", SpriteStackName);

                // Ensuring the Assets folder exists
                if (!System.IO.Directory.Exists(assetsFolder))
                {
                    System.IO.Directory.CreateDirectory(assetsFolder);
                }

                int frameNumber = 0;

                foreach (var file in openFileDialog.FileNames)
                {
                    // Generate new file name with the format {stackname}{framenumber}.{extension}
                    string extension = System.IO.Path.GetExtension(file);
                    string newFileName = $"{SpriteStackName}{frameNumber}{extension}";
                    string newFilePath = System.IO.Path.Combine(assetsFolder, newFileName);

                    // Copy the file to the Assets folder
                    if (System.IO.File.Exists(newFilePath))
                    {
                        System.IO.File.SetAttributes(newFilePath, System.IO.FileAttributes.Normal); // Remove read-only attribute if it exists (just in case :) )
                    }
                    try
                    {
                        System.IO.File.Copy(file, newFilePath, overwrite: true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error copying file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    if (openFileDialog.FileNames == null || openFileDialog.FileNames.Length == 0)
                    {
                        MessageBox.Show("No files selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    System.IO.File.Copy(file, newFilePath, overwrite: true);

                    // Add the new file path to the list and ListBox
                    FramePaths.Add(newFilePath);
                    Lbx_Frames.Items.Add(newFileName);

                    // Increment frame number
                    frameNumber++;
                }
            }
        }


        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            

            if (string.IsNullOrWhiteSpace(SpriteStackName) || FramePaths.Count == 0)
            {
                MessageBox.Show("Please provide a name and upload at least one frame.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Save the sprite stack to the database
            SaveSpriteStackToDatabase(SpriteStackName, FramePaths);

            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

            mainWindow.Lbx_SpriteStacks.Items.Add(SpriteStackName); // Add the new sprite stack to the main window's list box

            MessageBox.Show("Sprite stack saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }

        private void SaveSpriteStackToDatabase(string spriteStackName, List<string> framePaths)
        {
            List<Sprite> frames = new List<Sprite>();

            for (int i = 0; i < framePaths.Count; i++)
            {
                frames.Add(new Sprite
                {
                    Path = framePaths[i],
                    Frame = i,


                });
            }

            using (var context = new SpriteStackData())
            {
                var spriteStack = new SpriteStack
                {
                    _SpriteStackName = spriteStackName,
                    _sprites = frames,
                    _numberOfFrames = frames.Count,
                    _spriteOffsetX = SpriteOffsetX,

                };

                context.SpriteStacks.Add(spriteStack);
                context.SaveChanges();
            }
        }
    }
}
