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
    class LoaderPpl : Loader
    {
        static readonly Regex reg = new Regex(@"ppl\( 
                                                    (?<synset_id1>\d\d\d\d\d\d\d\d\d),
                                                    (?<w_num1>\d+),
                                                    (?<synset_id2>\d\d\d\d\d\d\d\d\d),
                                                    (?<w_num2>\d+)
                                                \)\.", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        public LoaderPpl(TextReader tr, BuilderInfo info)
            : base("participles...", reg, tr, info)
        {
        }

        protected override void ProcessLine(Match match)
        {
            var synset_id1 = GetValue<int>(match, "synset_id1");
            var w_num1 = GetValue<int>(match, "w_num1");
            var synset_id2 = GetValue<int>(match, "synset_id2");
            var w_num2 = GetValue<int>(match, "w_num2");

            var synset1 = Info.Synsets[synset_id1];
            var senses1 = w_num1 == 0
                ? synset1.Senses
                : synset1.Senses.Where(ws => ws.WordNum == w_num1);

            var synset2 = Info.Synsets[synset_id2];
            var senses2 = w_num2 == 0
                ? synset2.Senses
                : synset2.Senses.Where(ws => ws.WordNum == w_num2);

            foreach (var sense1 in senses1)
            {
                if (sense1.Participles == null) sense1.Participles = new List<WordSense>();
                foreach (var sense2 in senses2)
                {
                    if (sense1.Participles.All(ws => ws.WordSenseId != sense2.WordSenseId))
                        sense1.Participles.Add(sense2);
                }
            }

            foreach (var sense2 in senses2)
            {
                if (sense2.Participles == null) sense2.Participles = new List<WordSense>();
                foreach (var sense1 in senses1)
                {
                    if (sense2.Participles.All(ws => ws.WordSenseId != sense2.WordSenseId))
                        sense2.Participles.Add(sense1);
                }
            }
        }
    }
}
