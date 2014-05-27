using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Text.WordNet.Data;

namespace Ferric.Text.WordNet.Builder
{
    class LoaderAt : LoaderRel
    {
        public LoaderAt(TextReader tr, BuilderInfo info)
            : base("attributes...", "at", tr, info)
        {
        }

        protected override void ProcessLine(System.Text.RegularExpressions.Match match)
        {
            base.ProcessLine(match);

            if (Synset1.Attributes == null) Synset1.Attributes = new List<Synset>();
            if (Synset1.Attributes.All(s => s.WordNetId != Synset2.WordNetId))
                Synset1.Attributes.Add(Synset2);
        }
    }
}
