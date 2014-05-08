using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ferric.Text.WordNet.Builder
{
    static class LoaderS
    {
        static readonly Regex Reg = new Regex(@"s\(
                                                    (?<id>\d\d\d\d\d\d\d\d\d),
                                                \)", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        public static void Load(TextReader r, BuilderInfo info)
        {
            
        }
    }
}
