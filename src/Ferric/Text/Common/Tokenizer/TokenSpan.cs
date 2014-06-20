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

    public struct TokenPossibility
    {
        public ISet<int> Indices;
        public double Weight;
        public string Lemma;
        public object Data;
    }

    public class TokenSpan : BaseSpan
    {
        IList<TokenPossibility> possibilities;

        public TokenClass TokenClass { get; internal set; }
        public string Text { get; protected set; }
        
        public IEnumerable<TokenPossibility> Possibilities
        {
            get
            {
                if (possibilities == null)
                {
                    possibilities = new List<TokenPossibility>() { new TokenPossibility { Weight = 1.0, Lemma = Text.Trim().ToLowerInvariant() } };
                }
                return possibilities;
            }
            set
            {
                possibilities = value.ToList();
            }
        }

        public string Lemma
        {
            set
            {
                possibilities = new List<TokenPossibility>() { new TokenPossibility { Weight = 1.0, Lemma = value.Trim().ToLowerInvariant() } };
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
            var ll = string.Join(", ", possibilities.Select(l => string.Format("{0}/{1}", l.Lemma, l.Weight)));

            return string.Format("{{ {0}:{1} {2}-{3} {4} \"{5}\":{{{6}}} }}", 
                this.GetType().Name, Ordinal, CharPos, CharNext, TokenClass, 
                System.Text.RegularExpressions.Regex.Escape(Text),
                System.Text.RegularExpressions.Regex.Escape(ll));
        }
    }
}
