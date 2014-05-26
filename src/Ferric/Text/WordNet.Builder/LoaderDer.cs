using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Text.WordNet.Data;

namespace Ferric.Text.WordNet.Builder
{
    class LoaderDer : LoaderRel
    {
        public LoaderDer(TextReader tr, BuilderInfo info)
            : base("derivations...", "der", tr, info)
        {
        }

        protected override void ProcessLine(System.Text.RegularExpressions.Match match)
        {
            base.ProcessLine(match);

            if (Synset1.Derivations == null) Synset1.Derivations = new List<Synset>();
            if (Synset2.Derivations == null) Synset2.Derivations = new List<Synset>();

            Synset1.Derivations.Add(Synset2);
            Synset2.Derivations.Add(Synset1);
        }
    }
}
