using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ferric.Text.WordNet.Builder
{
    class LoaderSyntax : Loader
    {
        static readonly Regex reg = new Regex(@"syntax\( (?<synset_id>\d\d\d\d\d\d\d\d\d), (?<w_num>\d+), (?<syntax>(p|a|ip)) \)\.",
            RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        public LoaderSyntax(TextReader tr, BuilderInfo info)
            : base("syntax...", reg, tr, info)
        {
        }

        protected override void ProcessLine(Match match)
        {
            var synset_id = GetValue<int>(match, "synset_id");
            var w_num = GetValue<int>(match, "w_num");
            var syntax = GetValue<string>(match, "syntax");

            var synset = GetSynset(synset_id);
            var sense = synset.Senses.Single(ws => ws.WordNum == w_num);
            sense.Syntax = GetSyntax(syntax);
        }
    }
}
