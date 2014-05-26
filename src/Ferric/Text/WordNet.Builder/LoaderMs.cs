using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Text.WordNet.Data;

namespace Ferric.Text.WordNet.Builder
{
    class LoaderMs : LoaderRel
    {
        public LoaderMs(TextReader tr, BuilderInfo info)
            : base("substance meronyms...", "ms", tr, info)
        {
        }

        protected override void ProcessLine(System.Text.RegularExpressions.Match match)
        {
            base.ProcessLine(match);

            if (Synset1.SubstanceMeronyms == null) Synset1.SubstanceMeronyms = new List<Synset>();
            if (Synset2.SubstanceHolonyms == null) Synset2.SubstanceHolonyms = new List<Synset>();

            Synset1.SubstanceMeronyms.Add(Synset2);
            Synset2.SubstanceHolonyms.Add(Synset1);
        }
    }
}
