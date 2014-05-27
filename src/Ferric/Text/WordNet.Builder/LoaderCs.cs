using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Text.WordNet.Data;

namespace Ferric.Text.WordNet.Builder
{
    class LoaderCs : LoaderRel
    {
        public LoaderCs(TextReader tr, BuilderInfo info)
            : base("causes...", "cs", tr, info)
        {
        }

        protected override void ProcessLine(System.Text.RegularExpressions.Match match)
        {
            base.ProcessLine(match);

            if (Synset1.Causes == null) Synset1.Causes = new List<Synset>();
            Synset1.Causes.Add(Synset2);
        }
    }
}
