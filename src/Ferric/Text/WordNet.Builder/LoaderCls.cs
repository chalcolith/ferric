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
    class LoaderCls : Loader
    {
        static readonly Regex reg = new Regex(@"cls\(
                                                    (?<synset_id1>\d\d\d\d\d\d\d\d\d),
                                                    (?<w_num1>\d+),
                                                    (?<synset_id2>\d\d\d\d\d\d\d\d\d),
                                                    (?<w_num2>\d+),
                                                    (?<class_type>[tur])
                                                \)\.", 
            RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        public LoaderCls(TextReader tr, BuilderInfo info)
            : base("classes...", reg, tr, info)
        {
        }

        static int nextClassId = 1;

        protected override void ProcessLine(Match match)
        {
            var synset_id1 = GetValue<int>(match, "synset_id1");
            var w_num1 = GetValue<int>(match, "w_num1");
            var synset_id2 = GetValue<int>(match, "synset_id2");
            var w_num2 = GetValue<int>(match, "w_num2");
            var class_type = GetClassType(GetValue<string>(match, "class_type"));

            var members = w_num1 == 0
                ? Info.Synsets[synset_id1].Senses
                : Info.Synsets[synset_id1].Senses.Where(ws => ws.WordNum == w_num1);
            
            var heads = w_num2 == 0
                ? Info.Synsets[synset_id2].Senses
                : Info.Synsets[synset_id2].Senses.Where(ws => ws.WordNum == w_num2);

            SemanticClass sc;
            if (!Info.SemanticClasses.TryGetValue(heads.First().Synset.WordNetId, out sc))
            {
                sc = new SemanticClass
                {
                    SemanticClassId = nextClassId++,
                    Heads = new List<WordSense>(heads),
                    Members = new List<WordSense>(members)
                };

                foreach (var head in heads)
                {
                    Info.SemanticClasses[head.Synset.WordNetId] = sc;
                }
            }
            else
            {
                foreach (var member in members)
                {
                    if (!sc.Members.Any(m => m.WordSenseId == member.WordSenseId))
                        sc.Members.Add(member);
                }
            }
        }
    }
}
