using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ferric.Text.WordNet.Builder
{
    class LoaderFr : Loader
    {
        static readonly Regex reg = new Regex(@"fr\( 
                                                    (?<synset_id>\d\d\d\d\d\d\d\d\d),
                                                    (?<f_num>\d+),
                                                    (?<w_num>\d+)
                                                \)\.", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        public LoaderFr(TextReader tr, BuilderInfo info)
            : base("frames...", reg, tr, info)
        {
        }

        protected override void ProcessLine(Match match)
        {
            var synset_id = GetValue<int>(match, "synset_id");
            var f_num = GetValue<int>(match, "f_num");
            var w_num = GetValue<int>(match, "w_num");

            var synset = Info.Synsets[synset_id];
            var senses = w_num == 0
                ? synset.Senses
                : synset.Senses.Where(ws => ws.WordNum == w_num);

            foreach (var sense in senses)
            {
                sense.Frame = f_num;
            }
        }
    }
}
