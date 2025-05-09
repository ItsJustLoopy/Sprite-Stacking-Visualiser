namespace Sprite_Stacking_Visualiser.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFilePathsToSprite : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Sprites",
                c => new
                    {
                        SpriteStackID = c.Int(nullable: false, identity: true),
                        Path = c.String(nullable: false),
                        Frame = c.Int(nullable: false),
                        SpriteStack__SpriteStackID = c.Int(),
                    })
                .PrimaryKey(t => t.SpriteStackID)
                .ForeignKey("dbo.SpriteStacks", t => t.SpriteStack__SpriteStackID)
                .Index(t => t.SpriteStack__SpriteStackID);
            
            CreateTable(
                "dbo.SpriteStacks",
                c => new
                    {
                        _SpriteStackID = c.Int(nullable: false, identity: true),
                        _numberOfFrames = c.Int(nullable: false),
                        _SpriteStackName = c.String(),
                    })
                .PrimaryKey(t => t._SpriteStackID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sprites", "SpriteStack__SpriteStackID", "dbo.SpriteStacks");
            DropIndex("dbo.Sprites", new[] { "SpriteStack__SpriteStackID" });
            DropTable("dbo.SpriteStacks");
            DropTable("dbo.Sprites");
        }
    }
}
