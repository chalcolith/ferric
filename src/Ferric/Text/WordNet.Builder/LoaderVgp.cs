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
    class LoaderVgp : Loader
    {
        static readonly Regex reg = new Regex(@"vgp\( 
                                                    (?<synset_id1>\d\d\d\d\d\d\d\d\d),
                                                    (?<w_num1>\d+),
                                                    (?<synset_id2>\d\d\d\d\d\d\d\d\d),
                                                    (?<w_num2>\d+)
                                                \)\.", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        public LoaderVgp(TextReader tr, BuilderInfo info)
            : base("verb groups...", reg, tr, info)
        {
        }

        protected override void ProcessLine(Match match)
        {
            var synset_id1 = GetValue<int>(match, "synset_id1");
            var w_num1 = GetValue<int>(match, "w_num1");
            var synset_id2 = GetValue<int>(match, "synset_id2");
            var w_num2 = GetValue<int>(match, "w_num2");

            var synset1 = GetSynset(synset_id1);
            var synset2 = GetSynset(synset_id2);

            if (synset1.Groups == null) synset1.Groups = new List<Synset>();
            if (synset2.Groups == null) synset2.Groups = new List<Synset>();

            if (synset1.Groups.All(s => s.WordNetId != synset2.WordNetId))
                synset1.Groups.Add(synset2);
            if (synset2.Groups.All(s => s.WordNetId != synset1.WordNetId))
                synset2.Groups.Add(synset1);
        }
    }
}
