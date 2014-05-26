using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Text.WordNet.Data;

namespace Ferric.Text.WordNet.Builder
{
    class LoaderEnt : LoaderRel
    {
        public LoaderEnt(TextReader tr, BuilderInfo info)
            : base("entailments...", "ent", tr, info)
        {
        }

        protected override void ProcessLine(System.Text.RegularExpressions.Match match)
        {
            base.ProcessLine(match);

            if (Synset1.Entailments == null) Synset1.Entailments = new List<Synset>();
            Synset1.Entailments.Add(Synset2);
        }
    }
}
