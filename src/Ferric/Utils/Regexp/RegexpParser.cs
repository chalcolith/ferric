using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Utils.Regexp
{
    public partial class RegexpParser
    {
        int NEXT_POS = 0;

        static char Unescape(IEnumerable<char> chars)
        {
            var str = new string(chars.ToArray());
            return System.Text.RegularExpressions.Regex.Unescape(str)[0];
        }

        static char UnHex(IEnumerable<char> chars)
        {
            var str = new string(chars.ToArray());
            return (char)Convert.ToInt32(str, 16);
        }
    }
}
