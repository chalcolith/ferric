using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Text.WordNet.Data;

namespace Ferric.Text.WordNet.Builder
{
    class LoaderSim : LoaderRel
    {
        public LoaderSim(TextReader tr, BuilderInfo info)
            : base("satellites...", "sim", tr, info)
        {
        }

        protected override void ProcessLine(System.Text.RegularExpressions.Match match)
        {
            base.ProcessLine(match);

            if (Synset1.Satellites == null) Synset1.Satellites = new List<Synset>();
            Synset1.Satellites.Add(Synset2);
        }
    }
}
