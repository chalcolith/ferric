using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace English
{
    public enum TokenClass
    {
        Word,
        Number,
        Punct,
        Symbol,
        Space,
        Other
    }

    public class TokenSpan : Ferric.Text.Tokenizer.TokenSpan
    {
        public TokenClass TokenClass { get; internal set; }

        public TokenSpan(TokenClass tokenClass, string text, ulong charPos, ulong charNext, ulong ordinal)
            : base()
        {
            this.TokenClass = tokenClass;
            this.Text = text;
            this.CharPos = charPos;
            this.CharNext = charNext;
            this.Ordinal = ordinal;
        }
    }

    public class TokenRegexp : Ferric.Text.Tokenizer.TokenRegexp
    {
        public TokenClass TokenClass { get; internal set; }

        public TokenRegexp(string regexp, TokenClass tokenClass)
            : base(regexp)
        {
            this.TokenClass = tokenClass;
        }

        public override Ferric.Text.Tokenizer.TokenSpan OnReadToken(string text, ulong charPos, ulong charNext, ulong ordinal)
        {
            return new TokenSpan(TokenClass, text, charPos, charNext, ordinal);
        }
    }

    public class RegexpTokenizer : Ferric.Text.Tokenizer.RegexpTokenizer
    {
        static readonly IList<Ferric.Text.Tokenizer.TokenRegexp> defaultTokens = new List<Ferric.Text.Tokenizer.TokenRegexp>
        {
            new TokenRegexp(@"[\p{Lu}\p{Ll}\p{Lt}\p{Lm}\p{Lo}\p{Mn}\p{Mc}\p{Me}]+('(m|re|s|t|d|ve|ll))?", TokenClass.Word),
            new TokenRegexp(@"[\p{Nd}\p{Nl}\p{No}]+", TokenClass.Number),
            new TokenRegexp(@"[\p{Pc}\p{Pd}\p{Ps}\p{Pe}\p{Pi}\p{Pf}\p{Po}]", TokenClass.Punct),
            new TokenRegexp(@"[\p{Sm}\p{Sc}\p{Sk}\p{So}]", TokenClass.Symbol),
            new TokenRegexp(@"[\p{Zs}\p{Zl}\p{Zp}]+", TokenClass.Space),
            new TokenRegexp(@"[\p{Cc}\p{Cf}\p{Cs}\p{Co}\p{Cn}]", TokenClass.Other)
        };

        public RegexpTokenizer()
            : this(defaultTokens)
        {
        }

        public RegexpTokenizer(IList<Ferric.Text.Tokenizer.TokenRegexp> tokens)
            : base(tokens)
        {
        }
    }
}
