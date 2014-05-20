using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ferric.Text.WordNet.Builder
{
    class LoaderG : Loader
    {
        static readonly Regex reg = new Regex(
            @"g\( (?<synset_id>\d\d\d\d\d\d\d\d\d), (?<gloss>'?([^']|'')+'?) \)\.", 
            RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        public LoaderG(TextReader reader, BuilderInfo info)
            : base("glosses...", reg, reader, info)
        {
        }

        protected override void ProcessLine(Match match)
        {
            var synset_id = GetValue<int>(match, "synset_id");
            var gloss = GetValue<string>(match, "gloss");

            var synset = GetSynset(synset_id);
            synset.Gloss = gloss;
        }
    }
}
