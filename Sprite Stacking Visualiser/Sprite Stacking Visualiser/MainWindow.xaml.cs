using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using SkiaSharp;
using SkiaSharp.Views.WPF;
using Sprite_Stacking_Visualiser;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Security.Policy;

namespace Sprite_Stacking_Visualiser
{
    /// <summary>  
    /// Interaction logic for MainWindow.xaml  
    /// </summary>  
    public partial class MainWindow : Window
    {
        SpriteStackingRenderer renderer;
        SKBitmap bitmap;
        private DispatcherTimer _updateTimer; // Timer to control the rendering speed
        SKElement skiaView;
        SpriteStackData spriteStackData = new SpriteStackData(); // Database context to access the sprite stack data

        SpriteStack selectedStack = new SpriteStack(); // Variable to hold the selected sprite stack from the list box

        public MainWindow()
        {
            InitializeComponent();

            // Initialize the window and set its properties  
            Title = "Sprite Stacking Visualiser";
            Width = 1000;
            Height = 600;
            Background = new SolidColorBrush(Colors.Black);
            WindowStartupLocation = WindowStartupLocation.CenterScreen;


            _updateTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(16) // Approximately 60 Frames per second
            };
            CompositionTarget.Rendering += Update; // Attach the update event handler - this means the Update method will be called every 16ms 
            // Using CompositionTarget.Rendering instead of _updateTimer.Tick to ensure smoother rendering here

            _updateTimer.Start(); 


            // Initialize rendering engine 
            InitializeRenderer(spriteStackData, 1); // Pass the database context and stack ID to the renderer 
        }

        private void InitializeRenderer(SpriteStackData db, int stackID)
        {
            
            renderer = new SpriteStackingRenderer(db, stackID);
            renderer.LoadSpriteStack(renderer.StackToBeRendered);

            selectedStack = renderer.StackToBeRendered; // Get the selected sprite stack from the renderer at the start

            bitmap = new SKBitmap(1000, 600); // Create a bitmap to hold the rendered image
            

            // Create a SkiaSharp WPF view to display the canvas  
            skiaView = new SKElement
            {
                Width = 1000,
                Height = 600,
            };

            skiaView.PaintSurface += (s, args) => // PaintSurface event handler to draw the bitmap onto the SkiaSharp view
            {
                // Draw the bitmap onto the SkiaSharp view  
                args.Surface.Canvas.DrawBitmap(bitmap, new SKRect(0, 0, args.Info.Width, args.Info.Height));


            };

            using (var canvas = new SKCanvas(bitmap))
            {
                renderer.Render(canvas, 1000, 600, 0); // Render the sprite stack onto the bitmap


            }


        }
        private void Update(object sender, EventArgs e)
        {

            renderer._rotationAngle = (renderer._rotationAngle + (float)renderer.RotationSpeed * (float)_updateTimer.Interval.TotalSeconds) % 360f;
            // Update the rotation angle based on the rotation speed and time elapsed since the last update

            if (renderer._rotationAngle == 360f)
            {

                renderer._rotationAngle = 0f; // Reset the angle to avoid overflow
            }

            SkiaCanvas.InvalidateVisual(); // Invalidate the SkiaSharp view to trigger a repaint - updates wpf view

            renderer.StackToBeRendered = selectedStack; // Update the sprite stack to be rendered with the selected stack from the list box

            renderer.LoadSpriteStack(renderer.StackToBeRendered); // Load the selected sprite stack into the renderer

            Lsv_Sprites.Items.Clear(); 
            Lsv_Sprites_Initialized(sender, e);

            

        }

        private void SkiaCanvas_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {

            // Clear the canvas with a black color  
            e.Surface.Canvas.Clear(SKColors.Black);

            // Render the sprite stack on the canvas  
            renderer.Render(e.Surface.Canvas, e.Info.Width, e.Info.Height, selectedStack._spriteOffsetX);

        }

        private void Lbx_SpriteStacks_Initialized(object sender, EventArgs e)
        {
            Lbx_SpriteStacks.Items.Clear(); // Clear the list box before adding items

            var spritestacklist = spriteStackData.SpriteStacks.ToList(); // Get the list of sprite stacks from the database

            foreach (var stack in spritestacklist)
            {
                Lbx_SpriteStacks.Items.Add(stack._SpriteStackName); // Add each sprite stack to the list box
            }

            Lbx_SpriteStacks.SelectedIndex = 0; // Select the first sprite stack by default

            selectedStack = spriteStackData.SpriteStacks.FirstOrDefault(x => x._SpriteStackName == Lbx_SpriteStacks.SelectedItem.ToString()); // Get the selected sprite stack from the database

        }

        private void Lbx_SpriteStacks_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            selectedStack = spriteStackData.SpriteStacks.FirstOrDefault(x => x._SpriteStackName == Lbx_SpriteStacks.SelectedItem.ToString()); // Get the selected sprite stack from the database
            

        }

        private void Lsv_Sprites_Initialized(object sender, EventArgs e)
        {

            List<Image> SpriteList = new List<Image>();

            for (int i = 0; i < selectedStack._sprites.Count; i++)
            {
                // get image from file and set it to the image variable
                var image = new Image();
                var imagesource = new BitmapImage();

                imagesource.BeginInit();
                imagesource.UriSource = new Uri(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, selectedStack._sprites[i].Path));
                imagesource.EndInit();

                image.Stretch = Stretch.Fill;
                image.Width = imagesource.Width * 2;
                image.Height = imagesource.Height * 2;
                image.Source = imagesource;

                SpriteList.Add(image);

            }

            foreach (Image image in SpriteList)
            {
                Lsv_Sprites.Items.Add(image);
            }


        }

        private void Btn_Addstack_Click(object sender, RoutedEventArgs e)
        {
            // Open the AddSpriteStackWindow as a dialog
            AddSpriteStackWindow addSpriteStackWindow = new AddSpriteStackWindow();
            addSpriteStackWindow.ShowDialog();
        }
    }
}
