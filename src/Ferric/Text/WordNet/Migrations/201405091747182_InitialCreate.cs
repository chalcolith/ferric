namespace Ferric.Text.WordNet.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SemanticClasses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ClassType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Synsets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WordNetId = c.Int(nullable: false),
                        SynsetType = c.Int(nullable: false),
                        Gloss = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WordSenses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WordNum = c.Int(nullable: false),
                        Lemma = c.String(),
                        SenseNumber = c.Int(nullable: false),
                        TagCount = c.Int(nullable: false),
                        SenseKey = c.Int(nullable: false),
                        Syntax = c.Int(nullable: false),
                        Frame = c.Int(nullable: false),
                        Synset_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Synsets", t => t.Synset_Id)
                .Index(t => t.Synset_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WordSenses", "Synset_Id", "dbo.Synsets");
            DropIndex("dbo.WordSenses", new[] { "Synset_Id" });
            DropTable("dbo.WordSenses");
            DropTable("dbo.Synsets");
            DropTable("dbo.SemanticClasses");
        }
    }
}
