using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.WordNet.Data
{
    public class WordNet : DbContext
    {
        public WordNet()
            : base()
        {
        }

        public WordNet(string connectionString)
            : base(connectionString)
        {
        }

        static WordNet()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<WordNet, Migrations.Configuration>());
        }

        public virtual DbSet<Synset> Synsets { get; set; }
        public virtual DbSet<WordSense> WordSenses { get; set; }
        public virtual DbSet<SemanticClass> SemanticClasses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // semantic class relations
            modelBuilder.Entity<SemanticClass>().HasMany(sc => sc.Heads).WithMany()
                .Map(m => { m.ToTable("SemanticClassHeads"); m.MapLeftKey("SemanticClassId"); m.MapRightKey("WordSenseId"); });

            modelBuilder.Entity<SemanticClass>().HasMany(sc => sc.Members).WithMany()
                .Map(m => { m.ToTable("SemanticClassMembers"); m.MapLeftKey("SemanticClassId"); m.MapRightKey("WordSenseId"); });

            // word sense relations
            modelBuilder.Entity<WordSense>().HasMany(ws => ws.Groups).WithMany()
                .Map(m => { m.ToTable("Groups"); m.MapLeftKey("FirstWordSenseId"); m.MapRightKey("SecondWordSenseId"); });

            modelBuilder.Entity<WordSense>().HasMany(ws => ws.Antonyms).WithMany()
                .Map(m => { m.ToTable("Antonyms"); m.MapLeftKey("FirstWordSenseId"); m.MapRightKey("SecondWordSenseId"); });

            modelBuilder.Entity<WordSense>().HasMany(ws => ws.SeeAlsos).WithMany()
                .Map(m => { m.ToTable("SeeAlsos"); m.MapLeftKey("FirstWordSenseId"); m.MapRightKey("SecondWordSenseId"); });

            modelBuilder.Entity<WordSense>().HasMany(ws => ws.Participles).WithMany()
                .Map(m => { m.ToTable("Participles"); m.MapLeftKey("FirstWordSenseId"); m.MapRightKey("SecondWordSenseId"); });

            modelBuilder.Entity<WordSense>().HasMany(ws => ws.PertainsTo).WithMany()
                .Map(m => { m.ToTable("PertainsTo"); m.MapLeftKey("FirstWordSenseId"); m.MapRightKey("SecondWordSenseId"); });

            // synset relations
            modelBuilder.Entity<Synset>().HasMany(s => s.Hypernyms).WithMany()
                .Map(m => { m.ToTable("Hypernyms"); m.MapLeftKey("FirstSynsetId"); m.MapRightKey("SecondSynsetId"); });

            modelBuilder.Entity<Synset>().HasMany(s => s.Hyponyms).WithMany()
                .Map(m => { m.ToTable("Hyponyms"); m.MapLeftKey("FirstSynsetId"); m.MapRightKey("SecondSynsetId"); });

            modelBuilder.Entity<Synset>().HasMany(s => s.Prototypes).WithMany()
                .Map(m => { m.ToTable("Prototypes"); m.MapLeftKey("FirstSynsetId"); m.MapRightKey("SecondSynsetId"); });

            modelBuilder.Entity<Synset>().HasMany(s => s.Instances).WithMany()
                .Map(m => { m.ToTable("Instances"); m.MapLeftKey("FirstSynsetId"); m.MapRightKey("SecondSynsetId"); });

            modelBuilder.Entity<Synset>().HasMany(s => s.Entailments).WithMany()
                .Map(m => { m.ToTable("Entailments"); m.MapLeftKey("FirstSynsetId"); m.MapRightKey("SecondSynsetId"); });

            modelBuilder.Entity<Synset>().HasMany(s => s.Satellites).WithMany()
                .Map(m => { m.ToTable("Satellites"); m.MapLeftKey("FirstSynsetId"); m.MapRightKey("SecondSynsetId"); });

            modelBuilder.Entity<Synset>().HasMany(s => s.MemberMeronyms).WithMany()
                .Map(m => { m.ToTable("MemberMeronyms"); m.MapLeftKey("FirstSynsetId"); m.MapRightKey("SecondSynsetId"); });

            modelBuilder.Entity<Synset>().HasMany(s => s.MemberHolonyms).WithMany()
                .Map(m => { m.ToTable("MemberHolonyms"); m.MapLeftKey("FirstSynsetId"); m.MapRightKey("SecondSynsetId"); });

            modelBuilder.Entity<Synset>().HasMany(s => s.SubstanceMeronyms).WithMany()
                .Map(m => { m.ToTable("SubstanceMeronyms"); m.MapLeftKey("FirstSynsetId"); m.MapRightKey("SecondSynsetId"); });

            modelBuilder.Entity<Synset>().HasMany(s => s.SubstanceHolonyms).WithMany()
                .Map(m => { m.ToTable("SubstanceHolonyms"); m.MapLeftKey("FirstSynsetId"); m.MapRightKey("SecondSynsetId"); });

            modelBuilder.Entity<Synset>().HasMany(s => s.PartMeronyms).WithMany()
                .Map(m => { m.ToTable("PartMeronyms"); m.MapLeftKey("FirstSynsetId"); m.MapRightKey("SecondSynsetId"); });

            modelBuilder.Entity<Synset>().HasMany(s => s.PartHolonyms).WithMany()
                .Map(m => { m.ToTable("PartHolonyms"); m.MapLeftKey("FirstSynsetId"); m.MapRightKey("SecondSynsetId"); });

            modelBuilder.Entity<Synset>().HasMany(s => s.Derivations).WithMany()
                .Map(m => { m.ToTable("Derivations"); m.MapLeftKey("FirstSynsetId"); m.MapRightKey("SecondSynsetId"); });

            modelBuilder.Entity<Synset>().HasMany(s => s.Causes).WithMany()
                .Map(m => { m.ToTable("Causes"); m.MapLeftKey("FirstSynsetId"); m.MapRightKey("SecondSynsetId"); });

            modelBuilder.Entity<Synset>().HasMany(s => s.Attributes).WithMany()
                .Map(m => { m.ToTable("Attributes"); m.MapLeftKey("FirstSynsetId"); m.MapRightKey("SecondSynsetId"); });
        }
    }
}
