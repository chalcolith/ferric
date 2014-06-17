using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ferric.Text.WordNet.Morph
{
    public class Morphy
    {
        static readonly SuffixRule[] SuffixRules = new[]
        {
            new SuffixRule { PartOfSpeech = Data.SynsetType.Noun, Suffix = "s", Ending = "" },
            new SuffixRule { PartOfSpeech = Data.SynsetType.Noun, Suffix = "ses", Ending = "s" },
            new SuffixRule { PartOfSpeech = Data.SynsetType.Noun, Suffix = "xes", Ending = "x" },
            new SuffixRule { PartOfSpeech = Data.SynsetType.Noun, Suffix = "zes", Ending = "z" },
            new SuffixRule { PartOfSpeech = Data.SynsetType.Noun, Suffix = "ches", Ending = "ch" },
            new SuffixRule { PartOfSpeech = Data.SynsetType.Noun, Suffix = "shes", Ending = "sh" },
            new SuffixRule { PartOfSpeech = Data.SynsetType.Noun, Suffix = "men", Ending = "man" },
            new SuffixRule { PartOfSpeech = Data.SynsetType.Noun, Suffix = "ies", Ending = "y" },
            new SuffixRule { PartOfSpeech = Data.SynsetType.Verb, Suffix = "s", Ending = "" },
            new SuffixRule { PartOfSpeech = Data.SynsetType.Verb, Suffix = "ies", Ending = "y" },
            new SuffixRule { PartOfSpeech = Data.SynsetType.Verb, Suffix = "es", Ending = "e" },
            new SuffixRule { PartOfSpeech = Data.SynsetType.Verb, Suffix = "es", Ending = "" },
            new SuffixRule { PartOfSpeech = Data.SynsetType.Verb, Suffix = "ed", Ending = "e" },
            new SuffixRule { PartOfSpeech = Data.SynsetType.Verb, Suffix = "ed", Ending = "" },
            new SuffixRule { PartOfSpeech = Data.SynsetType.Verb, Suffix = "ing", Ending = "e" },
            new SuffixRule { PartOfSpeech = Data.SynsetType.Verb, Suffix = "ing", Ending = "" },
            new SuffixRule { PartOfSpeech = Data.SynsetType.Adjective, Suffix = "er", Ending = "" },
            new SuffixRule { PartOfSpeech = Data.SynsetType.Adjective, Suffix = "est", Ending = "" },
            new SuffixRule { PartOfSpeech = Data.SynsetType.Adjective, Suffix = "er", Ending = "e" },
            new SuffixRule { PartOfSpeech = Data.SynsetType.Adjective, Suffix = "est", Ending = "e" },
        };

        static readonly Regex ExceptionRegex = new Regex(@"(\S+)\s+(\S+)", RegexOptions.Compiled);
        readonly Data.WordNet context;
        readonly IDictionary<Data.SynsetType, IDictionary<string, string>> exceptions
            = new Dictionary<Data.SynsetType, IDictionary<string, string>>();

        public Morphy(Data.WordNet context, string exceptionsDir)
        {
            this.context = context;

            exceptions[Data.SynsetType.Noun] = LoadExceptions(Path.Combine(exceptionsDir, "noun.exc"));
            exceptions[Data.SynsetType.Verb] = LoadExceptions(Path.Combine(exceptionsDir, "verb.exc"));
            exceptions[Data.SynsetType.Adjective] = LoadExceptions(Path.Combine(exceptionsDir, "adj.exc"));
            exceptions[Data.SynsetType.Adverb] = LoadExceptions(Path.Combine(exceptionsDir, "adv.exc"));
        }

        public IEnumerable<Tuple<Data.SynsetType, string>> GetStems(string token)
        {
            token = token.Trim().ToLowerInvariant();

            // check directly
            var senses = context.WordSenses.Where(ws => ws.Lemma == token);
            foreach (var ws in senses)
                yield return Tuple.Create(ws.Synset.SynsetType, ws.Lemma);

            // now check exceptions
            foreach (var pos in exceptions)
            {
                foreach (var kv in pos.Value)
                {
                    senses = context.WordSenses.Where(ws => ws.Lemma == token && ws.Synset.SynsetType == pos.Key);
                    foreach (var ws in senses)
                        yield return Tuple.Create(pos.Key, kv.Value);
                }
            }

            // now try rules
            foreach (var rule in SuffixRules)
            {
                var form = rule.Apply(token);
                if (string.IsNullOrWhiteSpace(form))
                    continue;

                senses = context.WordSenses.Where(ws => ws.Lemma == form && ws.Synset.SynsetType == rule.PartOfSpeech);
                foreach (var ws in senses)
                    yield return Tuple.Create(rule.PartOfSpeech, form);
            }
        }

        IDictionary<string, string> LoadExceptions(string fname)
        {
            var result = new Dictionary<string, string>();

            using (var sr = new StreamReader(fname))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var match = ExceptionRegex.Match(line);
                    if (match.Success)
                        result[match.Groups[1].Value] = match.Groups[2].Value;
                }
            }

            return result;
        }
    }

    class SuffixRule
    {
        public Data.SynsetType PartOfSpeech { get; set; }
        public string Suffix { get; set; }
        public string Ending { get; set; }

        public string Apply(string token)
        {
            if (token.EndsWith(Suffix))
                return token.Substring(0, token.Length - Suffix.Length) + Ending;
            return null;
        }
    }
}
