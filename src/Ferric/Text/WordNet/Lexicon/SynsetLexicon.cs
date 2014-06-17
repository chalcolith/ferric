using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Text.Common.Lexicon;

namespace Ferric.Text.WordNet.Lexicon
{
    public class SynsetLexicon : FileLexicon
    {
        readonly Data.WordNet context;        

        object loadLock = new object();

        public ISet<int> WordNetIndices { get; private set; }

        public SynsetLexicon(Data.WordNet context, string fname, bool createIfNotFound = false)
            : base(fname, createIfNotFound)
        {
            this.context = context;
            WordNetIndices = new HashSet<int>();

            LoadWordNet();
        }

        public override int AddLemma(string lemma)
        {
            if (WordNetIndices.Contains(nextIndex))
                throw new Exception("WordNet index " + nextIndex + " is already in use.");

            return base.AddLemma(lemma);
        }

        public override void Save()
        {
            using (var sw = new StreamWriter(FullPath))
            {
                foreach (var kv in LemmasByIndex)
                {
                    if (!WordNetIndices.Contains(kv.Key))
                        sw.WriteLine("{0} {1}", kv.Key, kv.Value);
                }
            }
        }

        void LoadWordNet()
        {
            lock (loadLock)
            {
                var sb = new StringBuilder();
                foreach (var synset in context.Synsets)
                {
                    if (LemmasByIndex.ContainsKey(synset.WordNetId))
                        throw new Exception("WordNet Id " + synset.WordNetId + " is already in the file db.");

                    WordNetIndices.Add(synset.WordNetId);

                    sb.Clear();
                    foreach (var wordsense in synset.Senses)
                    {
                        var lemma = wordsense.Lemma.Trim().ToLowerInvariant();

                        if (sb.Length > 0)
                            sb.Append(", ");
                        sb.Append(lemma);

                        IndicesByLemma[lemma] = synset.WordNetId;
                    }

                    LemmasByIndex[synset.WordNetId] = sb.ToString();
                }
            }
        }
    }
}
