using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.WordNet
{
    public class WordNetContext : DbContext
    {
        public WordNetContext()
            : base()
        {
        }

        public virtual DbSet<WordSense> WordSenses { get; set; }
        public virtual DbSet<Synset> Synsets { get; set; }
        public virtual DbSet<SemanticClass> SemanticClasses { get; set; }
    }
}
