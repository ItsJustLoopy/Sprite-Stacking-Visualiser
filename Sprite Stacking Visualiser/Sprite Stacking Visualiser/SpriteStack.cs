using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprite_Stacking_Visualiser
{
    public class SpriteStack
    {

        public int _numberOfFrames { get; set; }
        [Key] public int _SpriteStackID { get; set; } // Primary key for the sprite stack
        public string _SpriteStackName { get; set; }

        public virtual List<Sprite> _sprites { get; set; } // List of sprites in the stack - virtual to allow for polymorphism & lazy loading
    }
}
