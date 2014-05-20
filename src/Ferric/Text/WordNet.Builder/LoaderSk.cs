using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ferric.Text.WordNet.Builder
{
    class LoaderSk : Loader
    {
        static readonly Regex reg = new Regex(@"sk\(
                                                    (?<synset_id>\d\d\d\d\d\d\d\d\d),
                                                    (?<w_num>\d+),
                                                    (?<sense_key>'?([^']|'')+'?)
                                                \)\.", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        public LoaderSk(TextReader tr, BuilderInfo info)
            : base("sense keys...", reg, tr, info)
        {
        }

        protected override void ProcessLine(Match match)
        {
            var synset_id = GetValue<int>(match, "synset_id");
            var w_num = GetValue<int>(match, "w_num");
            var sense_key = GetValue<string>(match, "sense_key");

            var synset = GetSynset(synset_id);

            var sense = synset.Senses.SingleOrDefault(s => s.WordNum == w_num);
            if (sense == null)
                throw new Exception("Word sense " + w_num + " for synset " + synset_id + " not found");

            sense.SenseKey = sense_key;
        }
    }
}
