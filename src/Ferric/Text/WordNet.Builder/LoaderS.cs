using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ferric.Text.WordNet.Builder
{
    static class LoaderS
    {
        static readonly Regex Reg = new Regex(@"s\(
                                                    (?<synset_id>\d\d\d\d\d\d\d\d\d),
                                                    (?<w_num>\d+),
                                                    (?<word>'?([^']|'')+'?),
                                                    (?<ss_type>[nvasr])(,
                                                    (?<sense_number>\d+))?(,
                                                    (?<tag_count>\d+))?
                                                \)\.", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        public static void Load(TextReader tr, BuilderInfo info)
        {
            Console.WriteLine("synsets...");

            const int NumToLoad = 10000;
            int numLoaded = 0;

            string line;
            while ((line = tr.ReadLine()) != null)
            {
                if (numLoaded++ > NumToLoad)
                    break;

                if ((numLoaded % 1000) == 0)
                    Console.WriteLine(numLoaded.ToString());

                var match = Reg.Match(line);
                if (!match.Success)
                    throw new Exception("Line not matched: " + line);

                var synset_id = int.Parse(match.Groups["synset_id"].Value);
                var w_num = int.Parse(match.Groups["w_num"].Value);
                var word = match.Groups["word"].Value.Trim('\'');
                var ss_type = BuilderInfo.GetSynsetType(match.Groups["ss_type"].Value);

                int sense_number, tag_count;

                int.TryParse(match.Groups["sense_number"].Value, out sense_number);
                int.TryParse(match.Groups["tag_count"].Value, out tag_count);

                Synset synset;
                if (info.SynsetsByWordNetId.TryGetValue(synset_id, out synset))
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
                    info.SynsetsByWordNetId.Add(synset_id, synset);
                }

                var senses = synset.Senses as IList<WordSense>;
                if (senses == null)
                    throw new Exception("when creating synsets, senses must be an IList");

                WordSense sense = senses.FirstOrDefault(s => s.Synset == synset && s.WordNum == w_num);
                if (sense != null)
                    continue; // throw new Exception("duplicate word sense for " + synset_id + ": " + w_num);

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
}
