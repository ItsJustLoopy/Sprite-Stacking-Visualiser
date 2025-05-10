using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using SkiaSharp;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace Sprite_Stacking_Visualiser
{
    public class SpriteStackingRenderer
    {
        List<SKBitmap> _sprites = new List<SKBitmap>(); // List to hold the sprites to be displayed  
        int _spriteStackID; // ID of the sprite stack to be rendered  

        public SpriteStack StackToBeRendered; // Sprite stack to be rendered  
        SpriteStackData SData; // Database context to access the sprite stack data  

        public float _rotationAngle = 3f; // Rotation angle for all sprites
        public float RotationSpeed = 50f; // Rotation speed in degrees per update

        public float scaleX, scaleY, scale, scaledWidth, scaledHeight, scaledX, scaledY; // Variables to hold the scale factors and dimensions of the sprites



        public enum effect
        {
            None,
            Spinning
        }

        public effect Currenteffect = effect.Spinning;

        public SpriteStackingRenderer(SpriteStackData spriteStackData, int spriteStackID) // Constructor to initialize the renderer with the sprite stack data and ID
        {
            SData = spriteStackData;
            _spriteStackID = spriteStackID;
            StackToBeRendered = StackToRender();

        }
        public SpriteStack StackToRender()
        {
            var spriteStack = SData.SpriteStacks.FirstOrDefault(x => x._SpriteStackID == _spriteStackID); // Get the sprite stack to be rendered from the database  
            if (spriteStack == null)
            {
                throw new ArgumentException($"Sprite stack with ID {_spriteStackID} not found.");
            }
            return spriteStack;
        }

        public void LoadSpriteStack(SpriteStack spriteStack)
        {
            _sprites.Clear(); // Clear the list before loading new sprites  
            for (int i = 0; i < spriteStack._numberOfFrames; i++)
            {
                var sprite = spriteStack._sprites[i];
                if (sprite == null)
                    throw new ArgumentNullException($"Sprite at index {i} is null.");


                if (string.IsNullOrEmpty(sprite.Path))
                    throw new InvalidOperationException("Sprite path is null or empty.");

                sprite.Bitmap = SKBitmap.Decode(sprite.Path); // Decode the image from the path

                _sprites.Add(sprite.Bitmap); // Add it to the list  

                var fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sprite.Path);
                // Combine resource directory with the sprite path - using this to avoid storing full path in the database for portability

                if (!System.IO.File.Exists(fullPath))
                    throw new InvalidOperationException($"File not found at path: {fullPath}");

                sprite.Bitmap = SKBitmap.Decode(fullPath); 
                if (sprite.Bitmap == null)
                    throw new InvalidOperationException($"Failed to decode image at path: {sprite.Path}");
            }

            foreach (var sprite in _sprites)
            {
                if (sprite == null)
                    throw new ArgumentNullException("Sprite is null.");

                if (sprite.Width <= 0 || sprite.Height <= 0)
                    throw new InvalidOperationException("Sprite dimensions are invalid.");

                // Calculate the scale factor to fit the sprite within the canvas dimensions


            }
        }

        public void Render(SKCanvas canvas, float width, float height, int spriteOffset)
        {
            if (canvas == null) throw new ArgumentNullException(nameof(canvas));
            if (width <= 0 || height <= 0) throw new ArgumentOutOfRangeException("Width and height must be positive.");
        
            // Center the sprite stack on the canvas and stack the sprites with a small offset between each sprite
            if (_sprites.Count > 0)
            {
                for (int i = 0; i < _sprites.Count; i++)
                {
                    var sprite = _sprites[i];

                    if (sprite != null)
                    {

                        // Calculate the scale factor to fit the sprite within the canvas dimensions
                        scaleX = width / (sprite.Width + 20);
                        scaleY = height / (sprite.Height + 20);
                        scale = Math.Min(scaleX, scaleY);

                        // Calculate the scaled width and height
                        scaledWidth = (sprite.Width * scale);
                        scaledHeight = (sprite.Height * scale);

                        scaledX = (width - scaledWidth) / 2;
                        scaledY = (height - scaledHeight) / 2 - i * 7; // Offset each sprite by 7 pixels vertically

                        canvas.Save();

                        canvas.Translate(scaledX + scaledWidth / 2, scaledY + scaledHeight / 2); // Translate to the center


                        if (Currenteffect == effect.Spinning)
                        {
                            canvas.RotateDegrees(_rotationAngle); // Apply the shared rotation angle
                        }

                        canvas.Translate(-(scaledX + scaledWidth / 2), -(scaledY + scaledHeight / 2)); // Translate back to avoid rotating the entire canvas

                        canvas.DrawBitmap(sprite, new SKRect(scaledX, scaledY, scaledX + scaledWidth, scaledY + scaledHeight)); // Draw the sprite with the calculated dimensions

                        canvas.Restore(); 
                    }
                }
            }
        }

        public void SetEffect(effect newEffect)
        {
            Currenteffect = newEffect; // Set the current effect to the new effect
        }

    }
}
