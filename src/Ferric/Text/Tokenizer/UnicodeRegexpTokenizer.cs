using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.Tokenizer
{
    public enum UnicodeTokenClass
    {
        Word,
        Number,
        Punct,
        Symbol,
        Space,
        Other
    }

    public class UnicodeTokenSpan : TokenSpan
    {
        public UnicodeTokenClass TokenClass { get; internal set; }

        public UnicodeTokenSpan(UnicodeTokenClass tokenClass, string text, ulong charPos, ulong charNext, ulong ordinal)
            : base()
        {
            this.TokenClass = tokenClass;
            this.Text = text;
            this.CharPos = charPos;
            this.CharNext = charNext;
            this.Ordinal = ordinal;
        }

        public override string ToString()
        {
            return string.Format("{{ {0}:{1} {2}-{3} {4} \"{5}\" }}", this.GetType().Name, Ordinal, CharPos, CharNext, TokenClass, System.Text.RegularExpressions.Regex.Escape(Text));
        }
    }

    public class UnicodeTokenRegexp : TokenRegexp
    {
        public UnicodeTokenClass TokenClass { get; internal set; }

        public UnicodeTokenRegexp(string regexp, UnicodeTokenClass tokenClass)
            : base(regexp)
        {
            this.TokenClass = tokenClass;
        }

        public override TokenSpan OnReadToken(string text, ulong charPos, ulong charNext, ulong ordinal)
        {
            return new UnicodeTokenSpan(TokenClass, text, charPos, charNext, ordinal);
        }
    }

    public class UnicodeRegexpTokenizer : RegexpTokenizer
    {
        static readonly IList<TokenRegexp> defaultTokens = new List<TokenRegexp>
        {
            new UnicodeTokenRegexp(@"[\p{Lu}\p{Ll}\p{Lt}\p{Lm}\p{Lo}\p{Mn}\p{Mc}\p{Me}]+", UnicodeTokenClass.Word),
            new UnicodeTokenRegexp(@"[\p{Nd}\p{Nl}\p{No}]+", UnicodeTokenClass.Number),
            new UnicodeTokenRegexp(@"[\p{Pc}\p{Pd}\p{Ps}\p{Pe}\p{Pi}\p{Pf}\p{Po}]", UnicodeTokenClass.Punct),
            new UnicodeTokenRegexp(@"[\p{Sm}\p{Sc}\p{Sk}\p{So}]", UnicodeTokenClass.Symbol),
            new UnicodeTokenRegexp(@"[\p{Zs}\p{Zl}\p{Zp}\p{Cc}]+", UnicodeTokenClass.Space),
            new UnicodeTokenRegexp(@"[\p{Cf}\p{Cs}\p{Co}\p{Cn}]", UnicodeTokenClass.Other)
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
