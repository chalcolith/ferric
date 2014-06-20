using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.Common.Lexicon
{
    public interface ILexicon<TEntry>
    {
        IDictionary<TEntry, ISet<int>> IndicesByEntry { get; }
        IDictionary<int, ISet<TEntry>> EntriesByIndex { get; }

        IEnumerable<int> GetIndices(string token);
        IEnumerable<string> GetLemmas(int index);

        ISet<int> AddEntry(TEntry entry);
    }

    public static class Constants
    {
        public const int UnknownIndex = 0;
        public const string UnknownLemma = "?unknown?";
    }
}
