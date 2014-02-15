using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ferric.Utils.Regexp
{
    public class FiniteStateAutomaton
    {
        public enum SpecialIndices
        {
            FAIL = -1,
            END = -2,
            DOT = -3,
        }
    }
}
