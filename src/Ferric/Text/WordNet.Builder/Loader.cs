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
    abstract class Loader
    {
        public string Description { get; protected set; }
        public Regex LineRegex { get; protected set; }

        public TextReader Reader { get; protected set; }
        public BuilderInfo Info { get; protected set; }

        public Loader(string description, Regex lineRegex, TextReader reader, BuilderInfo info)
        {
            this.Description = description;
            this.LineRegex = lineRegex;
            this.Reader = reader;
            this.Info = info;
        }

        public void Load()
        {
            Console.WriteLine(Description);

            int numLoaded = 0;

            string line;
            while ((line = Reader.ReadLine()) != null)
            {
                if (numLoaded++ > BuilderInfo.NumToLoad)
                    break;

                if ((numLoaded % BuilderInfo.DisplayStep) == 0)
                {
                    Console.WriteLine(numLoaded.ToString());
                }

                var match = LineRegex.Match(line);
                if (!match.Success)
                    throw new Exception("match failed: " + line);

                ProcessLine(match);
            }
        }

        protected abstract void ProcessLine(Match match);

        protected T GetValue<T>(Match match, string groupName)
        {
            return (T)Convert.ChangeType(match.Groups[groupName].Value, typeof(T));
        }

        protected Synset GetSynset(int wordNetId)
        {
            Synset result;
            Info.Synsets.TryGetValue(wordNetId, out result);
            return result;
        }

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

        public static AdjectiveSyntax GetSyntax(string syntax)
        {
            switch (syntax)
            {
                case "p": return AdjectiveSyntax.Predicate;
                case "a": return AdjectiveSyntax.Prenominal;
                case "ip": return AdjectiveSyntax.Postnominal;
            }
            throw new Exception("Unknown syntax " + syntax);
        }
    }

    abstract class LoaderRel : Loader
    {
        protected const string reg = @"\( (?<synset_id1>\d\d\d\d\d\d\d\d\d), (?<synset_id2>\d\d\d\d\d\d\d\d\d) \)\.";

        public LoaderRel(string description, string relation, TextReader tr, BuilderInfo info)
            : base(description, new Regex(relation + reg, RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace), tr, info)
        {
        }

        protected Synset Synset1 { get; set; }
        protected Synset Synset2 { get; set; }

        protected override void ProcessLine(Match match)
        {
            var synset_id1 = GetValue<int>(match, "synset_id1");
            Synset1 = GetSynset(synset_id1);

            var synset_id2 = GetValue<int>(match, "synset_id2");
            Synset2 = GetSynset(synset_id2);            
        }
    }
}
