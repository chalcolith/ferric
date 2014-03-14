using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.Tokenizer
{
    public class UnicodeTokenRegexp : TokenRegexp
    {
        public TokenClass TokenClass { get; internal set; }

        public UnicodeTokenRegexp(string regexp, TokenClass tokenClass)
            : base(regexp)
        {
            this.TokenClass = tokenClass;
        }

        public override TokenSpan OnReadToken(string text, ulong charPos, ulong charNext, ulong ordinal)
        {
            return new TokenSpan(TokenClass, text, charPos, charNext, ordinal);
        }
    }

    public class UnicodeRegexpTokenizer : RegexpTokenizer
    {
        static readonly IList<TokenRegexp> defaultTokens = new List<TokenRegexp>
        {
            new UnicodeTokenRegexp(@"[\p{Lu}\p{Ll}\p{Lt}\p{Lm}\p{Lo}\p{Mn}\p{Mc}\p{Me}]+", TokenClass.Word),
            new UnicodeTokenRegexp(@"[\p{Nd}\p{Nl}\p{No}]+", TokenClass.Number),
            new UnicodeTokenRegexp(@"[\p{Pc}\p{Pd}\p{Ps}\p{Pe}\p{Pi}\p{Pf}\p{Po}]", TokenClass.Punct),
            new UnicodeTokenRegexp(@"[\p{Sm}\p{Sc}\p{Sk}\p{So}]", TokenClass.Symbol),
            new UnicodeTokenRegexp(@"[\p{Zs}\p{Zl}\p{Zp}\p{Cc}]+", TokenClass.Space),
            new UnicodeTokenRegexp(@"[\p{Cf}\p{Cs}\p{Co}\p{Cn}]", TokenClass.Other)
        };

        public UnicodeRegexpTokenizer()
            : this(defaultTokens)
        {
        }

        public UnicodeRegexpTokenizer(IList<TokenRegexp> tokens)
            : base(tokens)
        {
        }
    }
}
