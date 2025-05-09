using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sprite_Stacking_Visualiser
{
    public class SpriteStackData : DbContext
    {
        public SpriteStackData() : base("SpriteStackData")
        {
        }
        public DbSet<SpriteStack> SpriteStacks { get; set; }
        public DbSet<Sprite> Sprites { get; set; }
    }
}
