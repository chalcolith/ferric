using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.WordNet.Builder
{
    class BuilderInfo
    {
        public IDictionary<int, Synset> SynsetsByWordNetId = new Dictionary<int, Synset>();
    }
}
