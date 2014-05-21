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

        protected override void ProcessLine(Match match)
        {
            var synset_id = GetValue<int>(match, "synset_id");
            var w_num = GetValue<int>(match, "w_num");
            var word = GetValue<string>(match, "word");
            var ss_type = BuilderInfo.GetSynsetType(GetValue<string>(match, "ss_type"));

            int sense_number, tag_count;
            int.TryParse(GetValue<string>(match, "sense_number"), out sense_number);
            int.TryParse(GetValue<string>(match, "tag_count"), out tag_count);

            Synset synset;
            if (Info.SynsetsByWordNetId.TryGetValue(synset_id, out synset))
            {
                if (synset.SynsetType != ss_type)
                    throw new Exception("Duplicate synset with id " + synset_id);
            }
            else
            {
                synset = new Synset
                {
                    WordNetId = synset_id,
                    SynsetType = ss_type,
                    Senses = new List<WordSense>()
                };
                Info.SynsetsByWordNetId.Add(synset_id, synset);
            }

            var senses = synset.Senses as IList<WordSense>;
            if (senses == null)
                throw new Exception("when creating synsets, senses must be an IList");

            WordSense sense = senses.FirstOrDefault(s => s.Synset == synset && s.WordNum == w_num);
            if (sense != null)
                return; // throw new Exception("duplicate word sense for " + synset_id + ": " + w_num);

            sense = new WordSense
            {
                Synset = synset,
                WordNum = w_num,
                Lemma = word,
                SenseNumber = sense_number,
                TagCount = tag_count
            };

            senses.Add(sense);
            if (senses.Count > 1)
                synset.Senses = senses.OrderBy(s => s.WordNum).ToList();
        }
    }
}
