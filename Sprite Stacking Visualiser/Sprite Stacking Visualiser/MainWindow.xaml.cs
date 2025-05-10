using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using SkiaSharp;
using SkiaSharp.Views.WPF;
using Sprite_Stacking_Visualiser;

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
            var spriteStackData = new SpriteStackData();
            InitializeRenderer(spriteStackData, 1); // Pass the database context and stack ID to the renderer
        }

        private void InitializeRenderer(SpriteStackData db, int stackID)
        {
            
            renderer = new SpriteStackingRenderer(db, stackID);
            renderer.LoadSpriteStack(renderer.StackToBeRendered);

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
                renderer.Render(canvas, 1000, 600); // Render the sprite stack onto the bitmap


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

        }

        private void SkiaCanvas_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {

            // Clear the canvas with a black color  
            e.Surface.Canvas.Clear(SKColors.Black);

            // Render the sprite stack on the canvas  
            renderer.Render(e.Surface.Canvas, e.Info.Width, e.Info.Height);

        }
    }
}
