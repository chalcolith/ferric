using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.Documents
{
    internal static class IoExtensions
    {
        public static IEnumerable<char> ReadChars(this StreamReader sr)
        {
            int ch;
            while ((ch = sr.Read()) != -1)
            {
                yield return (char)ch;
            }
        }
    }
}
