using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ferric.Text.Common.Lexicon
{
    public abstract class FileLexicon<TEntry> : ILexicon<TEntry>
    {
        protected int nextIndex = 1;

        public string FullPath { get; private set; }

        public FileLexicon(string fname, bool createIfNotFound = false)
        {
            this.FullPath = Path.GetFullPath(fname);

            if (!createIfNotFound && !File.Exists(this.FullPath))
                throw new ArgumentException("file does not exist: " + fname, "fname");
        }

        #region ILexicon<T> Members

        public IDictionary<TEntry, ISet<int>> IndicesByEntry { get; protected set; }
        public IDictionary<int, ISet<TEntry>> EntriesByIndex { get; protected set; }

        public virtual ISet<int> AddEntry(TEntry entry)
        {
            ISet<int> indices;
            if (!IndicesByEntry.TryGetValue(entry, out indices))
            {
                var index = nextIndex++;

                indices = new HashSet<int> { index };
                IndicesByEntry.Add(entry, indices);
                EntriesByIndex.Add(index, new HashSet<TEntry> { entry });
            }
            return indices;
        }

        #endregion

        public abstract IEnumerable<int> GetIndices(string token);
        public abstract IEnumerable<string> GetLemmas(int index);

        protected abstract int LoadLine(int num, string line);
        protected abstract IEnumerable<string> SaveLine(int index, ISet<TEntry> entries);

        protected void Load()
        {
            using (var s = new FileStream(FullPath, FileMode.Open, FileAccess.Read))
            using (var sr = new StreamReader(s))
            {
                int num = 0;
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    LoadLine(num++, line);
                }
            }
        }

        public virtual void Save()
        {
            using (var s = new FileStream(FullPath, FileMode.Create, FileAccess.Write))
            using (var sw = new StreamWriter(s))
            {
                foreach (var kv in EntriesByIndex)
                {
                    foreach (var line in SaveLine(kv.Key, kv.Value))
                        sw.WriteLine("{0}", line);
                }
            }
        }
    }

    public class FlatFileLexicon: FileLexicon<string>
    {
        public FlatFileLexicon(string fname, bool create = false)
            : base(fname, create)
        {
            EntriesByIndex = new Dictionary<int, ISet<string>>();
            EntriesByIndex[Constants.UnknownIndex] = new HashSet<string> { Constants.UnknownLemma };

            IndicesByEntry = new Dictionary<string, ISet<int>>();
            IndicesByEntry[Constants.UnknownLemma] = new HashSet<int> { Constants.UnknownIndex };

            if (File.Exists(FullPath))
                Load();
        }

        public override IEnumerable<int> GetIndices(string token)
        {
            ISet<int> result;
            if (IndicesByEntry.TryGetValue(token, out result))
                return result;
            else
                return Enumerable.Empty<int>();
        }

        public override IEnumerable<string> GetLemmas(int index)
        {
            ISet<string> result;
            if (EntriesByIndex.TryGetValue(index, out result))
                return result;
            else
                return Enumerable.Empty<string>();
        }

        static readonly Regex LineRegex = new Regex(@"(\d+) (.*)", RegexOptions.Compiled);

        protected override int LoadLine(int num, string line)
        {
            var match = LineRegex.Match(line);
            if (!match.Success)
                throw new Exception("Error in line: " + num + ": " + line);

            int index = int.Parse(match.Groups[1].Value);
            var lemma = match.Groups[2].Value;

            if (index >= nextIndex)
                nextIndex = index + 1;

            ISet<int> indices;
            if (!IndicesByEntry.TryGetValue(lemma, out indices))
            {
                indices = new HashSet<int>();
                IndicesByEntry.Add(lemma, indices);
            }
            indices.Add(index);

            ISet<string> entries;
            if (!EntriesByIndex.TryGetValue(index, out entries))
            {
                entries = new HashSet<string>();
                EntriesByIndex.Add(index, entries);
            }
            entries.Add(lemma);

            return index;
        }

        protected override IEnumerable<string> SaveLine(int index, ISet<string> entries)
        {
            foreach (var entry in entries)
                yield return string.Format("{0} {1}", index, entry);
        }
    }
}
