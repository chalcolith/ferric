using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ferric.Text.Common.Lexicon;

namespace Ferric.Text.WordNet.Lexicon
{
    public struct WordNetEntry
    {
        public ISet<int> Indices;
        public Data.SynsetType PartOfSpeech;
        public string Lemma;
    }

    public class SynsetLexicon : FileLexicon<WordNetEntry>
    {
        public static ISet<int> UnknownIndices = new HashSet<int> { Constants.UnknownIndex };

        static Data.SynsetType[] partsOfSpeech;
        static readonly Regex EntryRegex = new Regex(@"^(\d+)\s+(\w+)\s+(.*)$", RegexOptions.Compiled);

        readonly string wordNetDir;
        readonly ISet<int> wordNetIndices = new HashSet<int>();
        readonly IDictionary<string, ISet<WordNetEntry>> entriesByLemma = new Dictionary<string, ISet<WordNetEntry>>();

        public SynsetLexicon(string wordNetDir, string fname, bool createIfNotFound = false)
            : base(fname, createIfNotFound)
        {
            this.wordNetDir = wordNetDir;

            var unknownEntry = new WordNetEntry
            {
                Indices = new HashSet<int> { Constants.UnknownIndex },
                PartOfSpeech = Data.SynsetType.Unknown,
                Lemma = Constants.UnknownLemma
            };

            EntriesByIndex = new Dictionary<int, ISet<WordNetEntry>>();
            EntriesByIndex[Constants.UnknownIndex] = new HashSet<WordNetEntry> { unknownEntry };

            IndicesByEntry = new Dictionary<WordNetEntry, ISet<int>>();
            IndicesByEntry[unknownEntry] = new HashSet<int> { Constants.UnknownIndex };

            entriesByLemma = new Dictionary<string, ISet<WordNetEntry>>();

            LoadWordNet();
        }

        public IEnumerable<Data.SynsetType> PartsOfSpeech
        {
            get
            {
                if (partsOfSpeech == null)
                    partsOfSpeech = Enum.GetValues(typeof(Data.SynsetType)).OfType<Data.SynsetType>().ToArray();
                return partsOfSpeech;
            }
        }

        public IEnumerable<WordNetEntry> GetEntries(int index)
        {
            ISet<WordNetEntry> entries;
            if (EntriesByIndex.TryGetValue(index, out entries))
                return entries;
            else
                return Enumerable.Empty<WordNetEntry>();
        }

        public IEnumerable<WordNetEntry> GetEntries(string token)
        {
            ISet<WordNetEntry> entries;
            if (entriesByLemma.TryGetValue(token, out entries))
                return entries;
            return Enumerable.Empty<WordNetEntry>();
        }

        #region FileLexicon<T> Members

        public override IEnumerable<int> GetIndices(string token)
        {
            return GetEntries(token).SelectMany(e => e.Indices);
        }

        public override IEnumerable<string> GetLemmas(int index)
        {
            return GetEntries(index).Select(l => l.Lemma);
        }

        public override ISet<int> AddEntry(WordNetEntry entry)
        {
            if (wordNetIndices.Contains(nextIndex))
                throw new Exception("WordNet index " + nextIndex + " is already in use.");
            return base.AddEntry(entry);
        }

        protected override int LoadLine(int num, string line)
        {
            var match = EntryRegex.Match(line);
            if (!match.Success)
                throw new Exception("Error in line " + num);

            var index = int.Parse(match.Groups[1].Value);
            var pos = (Data.SynsetType)Enum.Parse(typeof(Data.SynsetType), match.Groups[2].Value);
            var entry = new WordNetEntry 
            { 
                Indices = new HashSet<int> { index },
                PartOfSpeech = pos,
                Lemma = match.Groups[3].Value.Trim().ToLowerInvariant(), 
            };

            ISet<int> indices;
            if (!IndicesByEntry.TryGetValue(entry, out indices))
            {
                indices = new HashSet<int>();
                IndicesByEntry.Add(entry, indices);
            }
            indices.Add(index);

            ISet<WordNetEntry> entries;
            if (!EntriesByIndex.TryGetValue(index, out entries))
            {
                entries = new HashSet<WordNetEntry>();
                EntriesByIndex.Add(index, entries);
            }
            entries.Add(entry);

            if (!entriesByLemma.TryGetValue(entry.Lemma, out entries))
            {
                entries = new HashSet<WordNetEntry>();
                entriesByLemma.Add(entry.Lemma, entries);
            }
            entries.Add(entry);

            return index;
        }

        protected override IEnumerable<string> SaveLine(int index, ISet<WordNetEntry> entries)
        {
            foreach (var entry in entries)
                yield return string.Format("{0} {1} {2}", index, entry.PartOfSpeech, entry.Lemma);
        }

        public override void Save()
        {
            using (var s = new FileStream(FullPath, FileMode.Create, FileAccess.Write))
            using (var sw = new StreamWriter(s))
            {
                foreach (var kv in EntriesByIndex)
                {
                    if (wordNetIndices.Contains(kv.Key))
                        continue;

                    foreach (var line in SaveLine(kv.Key, kv.Value))
                        sw.WriteLine("{0}", line);
                }
            }
        }

        #endregion

        #region Utility Members

        void LoadWordNet()
        {
            using (var tr = new StreamReader(Path.Combine(wordNetDir, "lemmas.txt")))
            {
                int num = 0;
                string line;
                while ((line = tr.ReadLine()) != null)
                {
                    var index = LoadLine(++num, line);
                    wordNetIndices.Add(index);
                }
            }
        }

        #endregion
    }
}
