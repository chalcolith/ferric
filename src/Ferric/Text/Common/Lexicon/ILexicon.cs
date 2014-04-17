using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.Common.Lexicon
{
    public interface ILexicon
    {
        IDictionary<string, int> IndicesByLemma { get; }
        IDictionary<int, string> LemmasByIndex { get; }
    }

    public static class Constants
    {
        public const string UnknownToken = "?unknown?";
    }
}
