using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.Tokenizer
{
    public class TokenizerException : Exception
    {
        public TokenizerException(string message)
            : base(message)
        {
        }
    }

    public class InvalidTokenException : TokenizerException
    {
        public ulong CharPos { get; set; }

        public InvalidTokenException(ulong pos)
            : base(string.Format("invalid token at position {0}", pos))
        {
            this.CharPos = pos;
        }
    }
}
