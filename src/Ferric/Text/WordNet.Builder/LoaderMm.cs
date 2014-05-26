using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Text.WordNet.Data;

namespace Ferric.Text.WordNet.Builder
{
    class LoaderMm : LoaderRel
    {
        public LoaderMm(TextReader tr, BuilderInfo info)
            : base("member meronyms...", "mm", tr, info)
        {
        }

        protected override void ProcessLine(System.Text.RegularExpressions.Match match)
        {
            base.ProcessLine(match);

            if (Synset1.MemberMeronyms == null) Synset1.MemberMeronyms = new List<Synset>();
            if (Synset2.MemberHolonyms == null) Synset2.MemberHolonyms = new List<Synset>();

            Synset1.MemberMeronyms.Add(Synset2);
            Synset2.MemberHolonyms.Add(Synset1);
        }
    }
}
