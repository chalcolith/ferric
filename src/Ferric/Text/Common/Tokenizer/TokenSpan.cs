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

    public struct TokenLemma
    {
        public double Weight;
        public string Lemma;
    }

    public class TokenSpan : BaseSpan
    {
        IList<TokenLemma> lemmas;

        public TokenClass TokenClass { get; internal set; }
        public string Text { get; protected set; }
        
        public IEnumerable<TokenLemma> Lemmas
        {
            get
            {
                if (lemmas == null)
                {
                    lemmas = new List<TokenLemma>() { new TokenLemma { Weight = 1.0, Lemma = Text.Trim().ToLowerInvariant() } };
                }
                return lemmas;
            }
            set
            {
                lemmas = value.ToList();
            }
        }

        public string Lemma
        {
            set
            {
                lemmas = new List<TokenLemma>() { new TokenLemma { Weight = 1.0, Lemma = value.Trim().ToLowerInvariant() } };
            }
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
            var ll = string.Join(", ", lemmas.Select(l => string.Format("{0}/{1}", l.Lemma, l.Weight)));

            return string.Format("{{ {0}:{1} {2}-{3} {4} \"{5}\":{{{6}}} }}", 
                this.GetType().Name, Ordinal, CharPos, CharNext, TokenClass, 
                System.Text.RegularExpressions.Regex.Escape(Text),
                System.Text.RegularExpressions.Regex.Escape(ll));
        }
    }
}
