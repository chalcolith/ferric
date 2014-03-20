using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.Common.Documents
{
    internal static class IoExtensions
    {
        public static IEnumerable<char> ReadChars(this StreamReader sr)
        {
            return ReadCharsAux(sr).AsEnumerable<char>();
        }

        static IEnumerable<char> ReadCharsAux(StreamReader sr)
        {
            int ch;
            while ((ch = sr.Read()) != -1)
            {
                yield return (char)ch;
            }
        }
    }
}
