using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.Common.Tokenizer
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

    public class TokenSpan : BaseSpan
    {
        string lemma;

        public TokenClass TokenClass { get; internal set; }
        public string Text { get; protected set; }
        public string Lemma
        {
            get { return lemma ?? (lemma = Text.ToLowerInvariant()); }
            set { lemma = value; }
        }

        public TokenSpan(TokenClass tokenClass, string text, ulong charPos, ulong charNext, ulong ordinal)
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
            return string.Format("{{ {0}:{1} {2}-{3} {4} \"{5}\":{6} }}", 
                this.GetType().Name, Ordinal, CharPos, CharNext, TokenClass, 
                System.Text.RegularExpressions.Regex.Escape(Text),
                System.Text.RegularExpressions.Regex.Escape(Lemma));
        }
    }
}
