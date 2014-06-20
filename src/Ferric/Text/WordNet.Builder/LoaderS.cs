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
    class LoaderS : Loader
    {
        static readonly Regex reg = new Regex(@"s\(
                                                    (?<synset_id>\d\d\d\d\d\d\d\d\d),
                                                    (?<w_num>\d+),
                                                    (?<word>'?([^']|'')+'?),
                                                    (?<ss_type>[nvasr])(,
                                                    (?<sense_number>\d+))?(,
                                                    (?<tag_count>\d+))?
                                                \)\.", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        public LoaderS(TextReader tr, BuilderInfo info)
            : base("synsets...", reg, tr, info)
        {
        }

        int nextSynsetId = 1;
        int nextWordSenseId = 1;

        protected override void ProcessLine(Match match)
        {
            var synset_id = GetValue<int>(match, "synset_id");
            var w_num = GetValue<int>(match, "w_num");
            var word = GetValue<string>(match, "word").Trim().Trim('\'').ToLowerInvariant();
            var ss_type = GetSynsetType(GetValue<string>(match, "ss_type"));

            int sense_number, tag_count;
            int.TryParse(GetValue<string>(match, "sense_number"), out sense_number);
            int.TryParse(GetValue<string>(match, "tag_count"), out tag_count);

            var synset = GetSynset(synset_id);
            if (synset == null)
            {
                synset = new Synset
                {
                    SynsetId = nextSynsetId++,
                    WordNetId = synset_id,
                    SynsetType = ss_type,
                    Senses = new List<WordSense>()
                };

                Info.Synsets.Add(synset_id, synset);
            }

            WordSense sense = synset.Senses.FirstOrDefault(s => s.Synset == synset && s.WordNum == w_num);
            if (sense != null) // there are some duplicates in the data files
                return; // throw new Exception("duplicate word sense for " + synset_id + ": " + w_num);

            sense = new WordSense
            {
                WordSenseId = nextWordSenseId++,
                SynsetId = synset.SynsetId,
                Synset = synset,
                WordNum = w_num,
                Lemma = word,
                SenseNumber = sense_number,
                TagCount = tag_count
            };

            synset.Senses.Add(sense);
            Info.Lemmas.Add(sense.Lemma);
        }
    }
}
