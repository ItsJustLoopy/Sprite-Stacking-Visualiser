namespace Sprite_Stacking_Visualiser.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSpriteOffset : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SpriteStacks", "_spriteOffsetX", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SpriteStacks", "_spriteOffsetX");
        }
    }
}
