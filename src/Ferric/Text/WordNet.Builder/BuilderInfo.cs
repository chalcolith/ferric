using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.WordNet.Builder
{
    class BuilderInfo
    {
        public const int NumToLoad = int.MaxValue;
        public const int DisplayStep = 10000;

        public IDictionary<int, Synset> SynsetsByWordNetId = new Dictionary<int, Synset>();

        public static SynsetType GetSynsetType(string ss_type)
        {
            switch (ss_type)
            {
                case "n": return SynsetType.Noun;
                case "v": return SynsetType.Verb;
                case "a": return SynsetType.Adjective;
                case "s": return SynsetType.AdjectiveSatellite;
                case "r": return SynsetType.Adverb;
            }
            throw new Exception("Unknown synset type " + ss_type);
        }
    }
}
