using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace Sprite_Stacking_Visualiser
{
    // Sprite class to hold the sprite data
    public class Sprite
    {
        [Required] public string Path { get; set; } // Path to the sprite image - required to ensure it is not null or empty when saving to the database
        public int Frame { get; set; } // Frame number of the sprite

        public SKBitmap Bitmap = new SKBitmap(); // Bitmap object to hold the sprite image

        
        [Key] public int SpriteStackID { get; set; } // Primary key for the sprite stack
        public virtual SpriteStack SpriteStack { get; set; } // Reference to the sprite stack this sprite belongs to - virtusl to allow for polymorphism & lazy loading


    }


}
