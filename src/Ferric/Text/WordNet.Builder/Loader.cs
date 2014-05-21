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
                    Console.WriteLine(numLoaded.ToString());

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

        protected Synset GetSynset(int synsetId)
        {
            Synset synset;
            if (!Info.SynsetsByWordNetId.TryGetValue(synsetId, out synset))
                throw new Exception("synset not found: " + synsetId);
            return synset;
        }        
    }
}
