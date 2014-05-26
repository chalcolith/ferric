using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Text.WordNet.Data;

namespace Ferric.Text.WordNet.Builder
{
    class LoaderMp : LoaderRel
    {
        public LoaderMp(TextReader tr, BuilderInfo info)
            : base("part meronyms...", "mp", tr, info)
        {
        }

        protected override void ProcessLine(System.Text.RegularExpressions.Match match)
        {
            base.ProcessLine(match);

            if (Synset1.PartMeronyms == null) Synset1.PartMeronyms = new List<Synset>();
            if (Synset2.PartHolonyms == null) Synset2.PartHolonyms = new List<Synset>();

            Synset1.PartMeronyms.Add(Synset2);
            Synset2.PartHolonyms.Add(Synset1);
        }
    }
}
