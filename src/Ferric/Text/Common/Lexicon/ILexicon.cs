using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.Common.Lexicon
{
    public interface ILexicon
    {
        IDictionary<string, int> Lemmas { get; }
        IDictionary<int, string> Indices { get; }
    }

    public static class Constants
    {
        public const string UnknownToken = "?unknown?";
    }
}
