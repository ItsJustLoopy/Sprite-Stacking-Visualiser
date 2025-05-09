using System;
using System.Collections.Generic;
using System.Linq;
using Sprite_Stacking_Visualiser;

namespace DataManager
{
    public class Program
    {


        static void Main(string[] args)
        {
            SpriteStackData db = new SpriteStackData();

            using (db)
            {
                // Create new sprite stacks
                CreateSpriteStack(db, "Crate", 8, 1);
            }

        }

        static void CreateSpriteStack(SpriteStackData db, string stackname, int frames, int spriteStackID)
        {


            var spriteStack = new SpriteStack();
            spriteStack._sprites = new List<Sprite>();
            spriteStack._SpriteStackName = stackname;
            spriteStack._numberOfFrames = frames;


            spriteStack._SpriteStackID = spriteStackID;

            // Check if the sprite stack already exists
            var existingStack = db.SpriteStacks.FirstOrDefault(x => x._SpriteStackID == spriteStackID);
            if (existingStack != null)
            {
                Console.WriteLine($"Sprite stack with ID {spriteStackID} already exists. Please use a different ID.");
                return;
            }
            // Create the sprites and add them to the sprite stack
            for (int i = 0; i < frames; i++)
            {
                Sprite s = new Sprite();
                s.Path = $"Assets/{stackname}/{stackname}{i}.png"; // Path to the sprite image
                s.SpriteStack = spriteStack;
                s.SpriteStackID = spriteStackID;
                s.Frame = i;

                db.Sprites.Add(s); // Add the sprite to the database
                spriteStack._sprites.Add(s); // Add the sprite to the stack
                
                Console.WriteLine($"{stackname} sprite {i} added to stack [{spriteStackID}].");
            }



            db.SpriteStacks.Add(spriteStack);
            Console.WriteLine($"Sprite stack {stackname} with ID {spriteStackID} created with {frames} frames.");

            // Save changes to the database
            db.SaveChanges();
            Console.WriteLine($"Sprite stack {stackname} with ID {spriteStackID} saved to database.");
        }
    }
}

