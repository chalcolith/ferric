using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ferric.Text.WordNet.Data;

namespace Ferric.Text.WordNet.Builder
{
    class LoaderHyp : LoaderRel
    {
        public LoaderHyp(TextReader tr, BuilderInfo info)
            : base("hypernyms...", "hyp", tr, info)
        {
        }

        protected override void ProcessLine(Match match)
        {
            base.ProcessLine(match);

            if (Synset1.Hypernyms == null) Synset1.Hypernyms = new List<Synset>();
            if (Synset2.Hyponyms == null) Synset2.Hyponyms = new List<Synset>();

            Synset1.Hypernyms.Add(Synset2);
            Synset2.Hyponyms.Add(Synset1);
        }
    }
}
