using System;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;

namespace Sprite_Stacking_Visualiser
{
    public class SpriteStackingRenderer
    {
        List<SKBitmap> _sprites = new List<SKBitmap>(); // List to hold the sprites to be displayed  
        int _spriteStackID; // ID of the sprite stack to be rendered  

        public SpriteStack StackToBeRendered; // Sprite stack to be rendered  
        SpriteStackData SData; // Database context to access the sprite stack data  

        public SpriteStackingRenderer(SpriteStackData spriteStackData, int spriteStackID)
        {
            SData = spriteStackData;
            _spriteStackID = spriteStackID;
            StackToBeRendered = StackToRender(spriteStackID, spriteStackData);
        }


        public SpriteStack StackToRender(int ID, SpriteStackData db)
        {
            var spriteStack = db.SpriteStacks.FirstOrDefault(x => x._SpriteStackID == ID); // Get the sprite stack to be rendered from the database  
            if (spriteStack == null)
            {
                throw new ArgumentException($"Sprite stack with ID {ID} not found.");
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

                var fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sprite.Path);//combine resource directory with the sprite path

                if (!System.IO.File.Exists(fullPath))
                    throw new InvalidOperationException($"File not found at path: {fullPath}");

                sprite.Bitmap = SKBitmap.Decode(fullPath);
                if (sprite.Bitmap == null)
                    throw new InvalidOperationException($"Failed to decode image at path: {sprite.Path}");
            }
        }

        public void Render(SKCanvas canvas, int width, int height) // Render the sprite stack  
        {
            if (canvas == null) throw new ArgumentNullException(nameof(canvas));
            if (width <= 0 || height <= 0) throw new ArgumentOutOfRangeException("Width and height must be positive.");

            // Clear the canvas with a black color  
            canvas.Clear(SKColors.Black);

            // Center the sprite stack on the canvas and stack the sprites with a small offset  
            if (_sprites.Count > 0)
            {
                for (int i = 0; i < _sprites.Count; i++)
                {
                    var sprite = _sprites[i];

                    if (sprite != null)
                    {

                        var x = (width - sprite.Width) / 2;
                        var y = (height - sprite.Height) / 2 - i * 1; // Offset each sprite by 10 pixels
                        canvas.DrawBitmap(sprite, x, y);

                    }

                }
            }
        }
    }
}
