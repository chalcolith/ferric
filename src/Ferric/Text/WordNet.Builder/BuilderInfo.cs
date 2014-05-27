using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Text.WordNet.Data;

namespace Ferric.Text.WordNet.Builder
{
    class BuilderInfo
    {
        public const int NumToLoad = int.MaxValue;
        public const int DisplayStep = 10000;

        public IDictionary<int, Synset> Synsets = new Dictionary<int, Synset>();
        public IDictionary<int, SemanticClass> SemanticClasses = new Dictionary<int, SemanticClass>();
        public ISet<string> Lemmas = new HashSet<string>();
    }
}
