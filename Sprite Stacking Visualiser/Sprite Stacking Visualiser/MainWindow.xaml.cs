using System.Windows;
using System.Windows.Media;
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

        public MainWindow()
        {
            InitializeComponent();

            // Initialize the window and set its properties  
            Title = "Sprite Stacking Visualiser";
            Width = 800;
            Height = 600;
            Background = new SolidColorBrush(Colors.Black);
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            // Initialize rendering engine  
            var spriteStackData = new SpriteStackData();
            InitializeRenderer(spriteStackData, 1); // Pass the database context and stack ID to the renderer
        }

        private void InitializeRenderer(SpriteStackData db, int stackID)
        {
            renderer = new SpriteStackingRenderer(db, stackID);
            renderer.LoadSpriteStack(renderer.StackToBeRendered);

            bitmap = new SKBitmap(800, 600);
            using (var canvas = new SKCanvas(bitmap))
            {
                renderer.Render(canvas, 800, 600);
            }

            // Create a SkiaSharp WPF view to display the canvas  
            var skiaView = new SKElement
            {
                Width = 800,
                Height = 600,
            };

            skiaView.PaintSurface += (sender, e) =>
            {
                // Draw the bitmap onto the SkiaSharp view  
                e.Surface.Canvas.Clear(SKColors.Black);
                e.Surface.Canvas.DrawBitmap(bitmap, 0, 0);
            };


        }

        private void SkiaCanvas_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {

            // Clear the canvas with a black color  
            e.Surface.Canvas.Clear(SKColors.Black);
            // Render the sprite stack on the canvas  
            renderer.Render(e.Surface.Canvas, (int)e.Info.Width, (int)e.Info.Height);

        }
    }
}
