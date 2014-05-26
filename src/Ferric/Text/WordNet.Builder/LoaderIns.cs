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
    class LoaderIns : LoaderRel
    {
        public LoaderIns(TextReader tr, BuilderInfo info)
            : base("prototype/instances...", "ins", tr, info)
        {
        }

        protected override void ProcessLine(Match match)
        {
            base.ProcessLine(match);

            if (Synset1.Prototypes == null) Synset1.Prototypes = new List<Synset>();
            if (Synset2.Instances == null) Synset2.Instances = new List<Synset>();

            Synset1.Prototypes.Add(Synset2);
            Synset2.Instances.Add(Synset1);
        }
    }
}
