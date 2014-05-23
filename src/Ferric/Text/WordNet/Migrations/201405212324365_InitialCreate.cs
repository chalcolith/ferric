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
                        SemanticClassId = c.Int(nullable: false, identity: true),
                        ClassType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SemanticClassId);

            CreateTable(
                "dbo.WordSenses",
                c => new
                    {
                        WordSenseId = c.Int(nullable: false, identity: true),
                        WordNum = c.Int(nullable: false),
                        Lemma = c.String(nullable: false, maxLength: 200),
                        SenseNumber = c.Int(nullable: false),
                        TagCount = c.Int(),
                        SenseKey = c.String(maxLength: 200),
                        Syntax = c.Int(),
                        Frame = c.Int(),
                        SynsetId = c.Int(),
                    })
                .PrimaryKey(t => t.WordSenseId)
                .ForeignKey("dbo.Synsets", t => t.SynsetId)
                .Index(t => new { t.SynsetId, t.WordNum })
                .Index(t => t.Lemma);
            
            CreateTable(
                "dbo.Synsets",
                c => new
                    {
                        SynsetId = c.Int(nullable: false, identity: true),
                        WordNetId = c.Int(nullable: false),
                        SynsetType = c.Int(nullable: false),
                        Gloss = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.SynsetId)
                .Index(t => t.WordNetId);
            
            CreateTable(
                "dbo.Antonyms",
                c => new
                    {
                        FirstWordSenseId = c.Int(nullable: false),
                        SecondWordSenseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FirstWordSenseId, t.SecondWordSenseId })
                .ForeignKey("dbo.WordSenses", t => t.FirstWordSenseId)
                .ForeignKey("dbo.WordSenses", t => t.SecondWordSenseId)
                .Index(t => t.FirstWordSenseId)
                .Index(t => t.SecondWordSenseId);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        FirstWordSenseId = c.Int(nullable: false),
                        SecondWordSenseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FirstWordSenseId, t.SecondWordSenseId })
                .ForeignKey("dbo.WordSenses", t => t.FirstWordSenseId)
                .ForeignKey("dbo.WordSenses", t => t.SecondWordSenseId)
                .Index(t => t.FirstWordSenseId)
                .Index(t => t.SecondWordSenseId);
            
            CreateTable(
                "dbo.Participles",
                c => new
                    {
                        FirstWordSenseId = c.Int(nullable: false),
                        SecondWordSenseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FirstWordSenseId, t.SecondWordSenseId })
                .ForeignKey("dbo.WordSenses", t => t.FirstWordSenseId)
                .ForeignKey("dbo.WordSenses", t => t.SecondWordSenseId)
                .Index(t => t.FirstWordSenseId)
                .Index(t => t.SecondWordSenseId);
            
            CreateTable(
                "dbo.PertainsTo",
                c => new
                    {
                        FirstWordSenseId = c.Int(nullable: false),
                        SecondWordSenseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FirstWordSenseId, t.SecondWordSenseId })
                .ForeignKey("dbo.WordSenses", t => t.FirstWordSenseId)
                .ForeignKey("dbo.WordSenses", t => t.SecondWordSenseId)
                .Index(t => t.FirstWordSenseId)
                .Index(t => t.SecondWordSenseId);
            
            CreateTable(
                "dbo.SeeAlsos",
                c => new
                    {
                        FirstWordSenseId = c.Int(nullable: false),
                        SecondWordSenseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FirstWordSenseId, t.SecondWordSenseId })
                .ForeignKey("dbo.WordSenses", t => t.FirstWordSenseId)
                .ForeignKey("dbo.WordSenses", t => t.SecondWordSenseId)
                .Index(t => t.FirstWordSenseId)
                .Index(t => t.SecondWordSenseId);
            
            CreateTable(
                "dbo.Attributes",
                c => new
                    {
                        FirstSynsetId = c.Int(nullable: false),
                        SecondSynsetId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FirstSynsetId, t.SecondSynsetId })
                .ForeignKey("dbo.Synsets", t => t.FirstSynsetId)
                .ForeignKey("dbo.Synsets", t => t.SecondSynsetId)
                .Index(t => t.FirstSynsetId)
                .Index(t => t.SecondSynsetId);
            
            CreateTable(
                "dbo.Causes",
                c => new
                    {
                        FirstSynsetId = c.Int(nullable: false),
                        SecondSynsetId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FirstSynsetId, t.SecondSynsetId })
                .ForeignKey("dbo.Synsets", t => t.FirstSynsetId)
                .ForeignKey("dbo.Synsets", t => t.SecondSynsetId)
                .Index(t => t.FirstSynsetId)
                .Index(t => t.SecondSynsetId);
            
            CreateTable(
                "dbo.Derivations",
                c => new
                    {
                        FirstSynsetId = c.Int(nullable: false),
                        SecondSynsetId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FirstSynsetId, t.SecondSynsetId })
                .ForeignKey("dbo.Synsets", t => t.FirstSynsetId)
                .ForeignKey("dbo.Synsets", t => t.SecondSynsetId)
                .Index(t => t.FirstSynsetId)
                .Index(t => t.SecondSynsetId);
            
            CreateTable(
                "dbo.Entailments",
                c => new
                    {
                        FirstSynsetId = c.Int(nullable: false),
                        SecondSynsetId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FirstSynsetId, t.SecondSynsetId })
                .ForeignKey("dbo.Synsets", t => t.FirstSynsetId)
                .ForeignKey("dbo.Synsets", t => t.SecondSynsetId)
                .Index(t => t.FirstSynsetId)
                .Index(t => t.SecondSynsetId);
            
            CreateTable(
                "dbo.Hypernyms",
                c => new
                    {
                        FirstSynsetId = c.Int(nullable: false),
                        SecondSynsetId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FirstSynsetId, t.SecondSynsetId })
                .ForeignKey("dbo.Synsets", t => t.FirstSynsetId)
                .ForeignKey("dbo.Synsets", t => t.SecondSynsetId)
                .Index(t => t.FirstSynsetId)
                .Index(t => t.SecondSynsetId);
            
            CreateTable(
                "dbo.Hyponyms",
                c => new
                    {
                        FirstSynsetId = c.Int(nullable: false),
                        SecondSynsetId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FirstSynsetId, t.SecondSynsetId })
                .ForeignKey("dbo.Synsets", t => t.FirstSynsetId)
                .ForeignKey("dbo.Synsets", t => t.SecondSynsetId)
                .Index(t => t.FirstSynsetId)
                .Index(t => t.SecondSynsetId);
            
            CreateTable(
                "dbo.Instances",
                c => new
                    {
                        FirstSynsetId = c.Int(nullable: false),
                        SecondSynsetId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FirstSynsetId, t.SecondSynsetId })
                .ForeignKey("dbo.Synsets", t => t.FirstSynsetId)
                .ForeignKey("dbo.Synsets", t => t.SecondSynsetId)
                .Index(t => t.FirstSynsetId)
                .Index(t => t.SecondSynsetId);
            
            CreateTable(
                "dbo.MemberHolonyms",
                c => new
                    {
                        FirstSynsetId = c.Int(nullable: false),
                        SecondSynsetId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FirstSynsetId, t.SecondSynsetId })
                .ForeignKey("dbo.Synsets", t => t.FirstSynsetId)
                .ForeignKey("dbo.Synsets", t => t.SecondSynsetId)
                .Index(t => t.FirstSynsetId)
                .Index(t => t.SecondSynsetId);
            
            CreateTable(
                "dbo.MemberMeronyms",
                c => new
                    {
                        FirstSynsetId = c.Int(nullable: false),
                        SecondSynsetId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FirstSynsetId, t.SecondSynsetId })
                .ForeignKey("dbo.Synsets", t => t.FirstSynsetId)
                .ForeignKey("dbo.Synsets", t => t.SecondSynsetId)
                .Index(t => t.FirstSynsetId)
                .Index(t => t.SecondSynsetId);
            
            CreateTable(
                "dbo.PartHolonyms",
                c => new
                    {
                        FirstSynsetId = c.Int(nullable: false),
                        SecondSynsetId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FirstSynsetId, t.SecondSynsetId })
                .ForeignKey("dbo.Synsets", t => t.FirstSynsetId)
                .ForeignKey("dbo.Synsets", t => t.SecondSynsetId)
                .Index(t => t.FirstSynsetId)
                .Index(t => t.SecondSynsetId);
            
            CreateTable(
                "dbo.PartMeronyms",
                c => new
                    {
                        FirstSynsetId = c.Int(nullable: false),
                        SecondSynsetId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FirstSynsetId, t.SecondSynsetId })
                .ForeignKey("dbo.Synsets", t => t.FirstSynsetId)
                .ForeignKey("dbo.Synsets", t => t.SecondSynsetId)
                .Index(t => t.FirstSynsetId)
                .Index(t => t.SecondSynsetId);
            
            CreateTable(
                "dbo.Prototypes",
                c => new
                    {
                        FirstSynsetId = c.Int(nullable: false),
                        SecondSynsetId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FirstSynsetId, t.SecondSynsetId })
                .ForeignKey("dbo.Synsets", t => t.FirstSynsetId)
                .ForeignKey("dbo.Synsets", t => t.SecondSynsetId)
                .Index(t => t.FirstSynsetId)
                .Index(t => t.SecondSynsetId);
            
            CreateTable(
                "dbo.Satellites",
                c => new
                    {
                        FirstSynsetId = c.Int(nullable: false),
                        SecondSynsetId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FirstSynsetId, t.SecondSynsetId })
                .ForeignKey("dbo.Synsets", t => t.FirstSynsetId)
                .ForeignKey("dbo.Synsets", t => t.SecondSynsetId)
                .Index(t => t.FirstSynsetId)
                .Index(t => t.SecondSynsetId);
            
            CreateTable(
                "dbo.SubstanceHolonyms",
                c => new
                    {
                        FirstSynsetId = c.Int(nullable: false),
                        SecondSynsetId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FirstSynsetId, t.SecondSynsetId })
                .ForeignKey("dbo.Synsets", t => t.FirstSynsetId)
                .ForeignKey("dbo.Synsets", t => t.SecondSynsetId)
                .Index(t => t.FirstSynsetId)
                .Index(t => t.SecondSynsetId);
            
            CreateTable(
                "dbo.SubstanceMeronyms",
                c => new
                    {
                        FirstSynsetId = c.Int(nullable: false),
                        SecondSynsetId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.FirstSynsetId, t.SecondSynsetId })
                .ForeignKey("dbo.Synsets", t => t.FirstSynsetId)
                .ForeignKey("dbo.Synsets", t => t.SecondSynsetId)
                .Index(t => t.FirstSynsetId)
                .Index(t => t.SecondSynsetId);
            
            CreateTable(
                "dbo.SemanticClassHeads",
                c => new
                    {
                        SemanticClassId = c.Int(nullable: false),
                        WordSenseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SemanticClassId, t.WordSenseId })
                .ForeignKey("dbo.SemanticClasses", t => t.SemanticClassId, cascadeDelete: true)
                .ForeignKey("dbo.WordSenses", t => t.WordSenseId, cascadeDelete: true)
                .Index(t => t.SemanticClassId)
                .Index(t => t.WordSenseId);
            
            CreateTable(
                "dbo.SemanticClassMembers",
                c => new
                    {
                        SemanticClassId = c.Int(nullable: false),
                        WordSenseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SemanticClassId, t.WordSenseId })
                .ForeignKey("dbo.SemanticClasses", t => t.SemanticClassId, cascadeDelete: true)
                .ForeignKey("dbo.WordSenses", t => t.WordSenseId, cascadeDelete: true)
                .Index(t => t.SemanticClassId)
                .Index(t => t.WordSenseId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SemanticClassMembers", "WordSenseId", "dbo.WordSenses");
            DropForeignKey("dbo.SemanticClassMembers", "SemanticClassId", "dbo.SemanticClasses");
            DropForeignKey("dbo.SemanticClassHeads", "WordSenseId", "dbo.WordSenses");
            DropForeignKey("dbo.SemanticClassHeads", "SemanticClassId", "dbo.SemanticClasses");
            DropForeignKey("dbo.SubstanceMeronyms", "SecondSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.SubstanceMeronyms", "FirstSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.SubstanceHolonyms", "SecondSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.SubstanceHolonyms", "FirstSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.WordSenses", "SynsetId", "dbo.Synsets");
            DropForeignKey("dbo.Satellites", "SecondSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.Satellites", "FirstSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.Prototypes", "SecondSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.Prototypes", "FirstSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.PartMeronyms", "SecondSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.PartMeronyms", "FirstSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.PartHolonyms", "SecondSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.PartHolonyms", "FirstSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.MemberMeronyms", "SecondSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.MemberMeronyms", "FirstSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.MemberHolonyms", "SecondSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.MemberHolonyms", "FirstSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.Instances", "SecondSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.Instances", "FirstSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.Hyponyms", "SecondSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.Hyponyms", "FirstSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.Hypernyms", "SecondSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.Hypernyms", "FirstSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.Entailments", "SecondSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.Entailments", "FirstSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.Derivations", "SecondSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.Derivations", "FirstSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.Causes", "SecondSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.Causes", "FirstSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.Attributes", "SecondSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.Attributes", "FirstSynsetId", "dbo.Synsets");
            DropForeignKey("dbo.SeeAlsos", "SecondWordSenseId", "dbo.WordSenses");
            DropForeignKey("dbo.SeeAlsos", "FirstWordSenseId", "dbo.WordSenses");
            DropForeignKey("dbo.PertainsTo", "SecondWordSenseId", "dbo.WordSenses");
            DropForeignKey("dbo.PertainsTo", "FirstWordSenseId", "dbo.WordSenses");
            DropForeignKey("dbo.Participles", "SecondWordSenseId", "dbo.WordSenses");
            DropForeignKey("dbo.Participles", "FirstWordSenseId", "dbo.WordSenses");
            DropForeignKey("dbo.Groups", "SecondWordSenseId", "dbo.WordSenses");
            DropForeignKey("dbo.Groups", "FirstWordSenseId", "dbo.WordSenses");
            DropForeignKey("dbo.Antonyms", "SecondWordSenseId", "dbo.WordSenses");
            DropForeignKey("dbo.Antonyms", "FirstWordSenseId", "dbo.WordSenses");
            DropIndex("dbo.SemanticClassMembers", new[] { "WordSenseId" });
            DropIndex("dbo.SemanticClassMembers", new[] { "SemanticClassId" });
            DropIndex("dbo.SemanticClassHeads", new[] { "WordSenseId" });
            DropIndex("dbo.SemanticClassHeads", new[] { "SemanticClassId" });
            DropIndex("dbo.SubstanceMeronyms", new[] { "SecondSynsetId" });
            DropIndex("dbo.SubstanceMeronyms", new[] { "FirstSynsetId" });
            DropIndex("dbo.SubstanceHolonyms", new[] { "SecondSynsetId" });
            DropIndex("dbo.SubstanceHolonyms", new[] { "FirstSynsetId" });
            DropIndex("dbo.Satellites", new[] { "SecondSynsetId" });
            DropIndex("dbo.Satellites", new[] { "FirstSynsetId" });
            DropIndex("dbo.Prototypes", new[] { "SecondSynsetId" });
            DropIndex("dbo.Prototypes", new[] { "FirstSynsetId" });
            DropIndex("dbo.PartMeronyms", new[] { "SecondSynsetId" });
            DropIndex("dbo.PartMeronyms", new[] { "FirstSynsetId" });
            DropIndex("dbo.PartHolonyms", new[] { "SecondSynsetId" });
            DropIndex("dbo.PartHolonyms", new[] { "FirstSynsetId" });
            DropIndex("dbo.MemberMeronyms", new[] { "SecondSynsetId" });
            DropIndex("dbo.MemberMeronyms", new[] { "FirstSynsetId" });
            DropIndex("dbo.MemberHolonyms", new[] { "SecondSynsetId" });
            DropIndex("dbo.MemberHolonyms", new[] { "FirstSynsetId" });
            DropIndex("dbo.Instances", new[] { "SecondSynsetId" });
            DropIndex("dbo.Instances", new[] { "FirstSynsetId" });
            DropIndex("dbo.Hyponyms", new[] { "SecondSynsetId" });
            DropIndex("dbo.Hyponyms", new[] { "FirstSynsetId" });
            DropIndex("dbo.Hypernyms", new[] { "SecondSynsetId" });
            DropIndex("dbo.Hypernyms", new[] { "FirstSynsetId" });
            DropIndex("dbo.Entailments", new[] { "SecondSynsetId" });
            DropIndex("dbo.Entailments", new[] { "FirstSynsetId" });
            DropIndex("dbo.Derivations", new[] { "SecondSynsetId" });
            DropIndex("dbo.Derivations", new[] { "FirstSynsetId" });
            DropIndex("dbo.Causes", new[] { "SecondSynsetId" });
            DropIndex("dbo.Causes", new[] { "FirstSynsetId" });
            DropIndex("dbo.Attributes", new[] { "SecondSynsetId" });
            DropIndex("dbo.Attributes", new[] { "FirstSynsetId" });
            DropIndex("dbo.SeeAlsos", new[] { "SecondWordSenseId" });
            DropIndex("dbo.SeeAlsos", new[] { "FirstWordSenseId" });
            DropIndex("dbo.PertainsTo", new[] { "SecondWordSenseId" });
            DropIndex("dbo.PertainsTo", new[] { "FirstWordSenseId" });
            DropIndex("dbo.Participles", new[] { "SecondWordSenseId" });
            DropIndex("dbo.Participles", new[] { "FirstWordSenseId" });
            DropIndex("dbo.Groups", new[] { "SecondWordSenseId" });
            DropIndex("dbo.Groups", new[] { "FirstWordSenseId" });
            DropIndex("dbo.Antonyms", new[] { "SecondWordSenseId" });
            DropIndex("dbo.Antonyms", new[] { "FirstWordSenseId" });
            DropIndex("dbo.WordSenses", new[] { "SynsetId" });
            DropTable("dbo.SemanticClassMembers");
            DropTable("dbo.SemanticClassHeads");
            DropTable("dbo.SubstanceMeronyms");
            DropTable("dbo.SubstanceHolonyms");
            DropTable("dbo.Satellites");
            DropTable("dbo.Prototypes");
            DropTable("dbo.PartMeronyms");
            DropTable("dbo.PartHolonyms");
            DropTable("dbo.MemberMeronyms");
            DropTable("dbo.MemberHolonyms");
            DropTable("dbo.Instances");
            DropTable("dbo.Hyponyms");
            DropTable("dbo.Hypernyms");
            DropTable("dbo.Entailments");
            DropTable("dbo.Derivations");
            DropTable("dbo.Causes");
            DropTable("dbo.Attributes");
            DropTable("dbo.SeeAlsos");
            DropTable("dbo.PertainsTo");
            DropTable("dbo.Participles");
            DropTable("dbo.Groups");
            DropTable("dbo.Antonyms");
            DropTable("dbo.Synsets");
            DropTable("dbo.WordSenses");
            DropTable("dbo.SemanticClasses");
        }
    }
}
