using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Utils.Regexp;

namespace Ferric.Text.Tokenizer
{
    public abstract class RegexpTokenizer : BaseTransducer<char, ISpan>
    {
        readonly IList<TokenRegexp> tokenRegexps;

        public RegexpTokenizer(IList<TokenRegexp> tokenRegexps)
        {
            this.tokenRegexps = tokenRegexps;
        }

        #region ITransducer<char,ISpan> Members

        public override IEnumerable<ISpan> Process(IEnumerable<char> input)
        {
            Reset();

            ulong ordinal = 0;
            ulong charStart = 0;
            ulong charPos = 0;

            var token = new StringBuilder();
            var e = input.GetEnumerator();

            if (e.MoveNext())
            {
                char ch = e.Current;

                TokenRegexp longest = null;
                while (true)
                {
                    // update each machine
                    bool allFailed = true;
                    foreach (var machine in tokenRegexps)
                    {
                        if (machine.Failed)
                            continue;

                        machine.ProcessInput((char)ch);
                        allFailed = allFailed && machine.Failed;

                        if (machine.Succeeded)
                        {
                            if (longest == null || machine.Count > longest.Count)
                                longest = machine;
                        }
                    }

                    // if all failed, then we may have got a token
                    if (allFailed)
                    {
                        if (token.Length > 0 && longest != null)
                        {
                            var span = longest.OnReadToken(token.ToString(), charStart, charPos, ordinal);
                            yield return span;

                            longest = null;
                            token.Clear();
                            charStart = charPos;
                            ordinal++;
                            Reset();
                        }
                        else
                        {
                            throw new InvalidTokenException(ordinal);
                        }
                    }
                    // if some succeeded, keep going
                    else
                    {
                        token.Append((char)ch);
                        charPos++;

                        // if no more chars, return last token (if any)
                        if (e.MoveNext())
                        {
                            ch = e.Current;
                        }
                        else
                        {
                            if (token.Length > 0 && longest != null)
                            {
                                var span = longest.OnReadToken(token.ToString(), charStart, charPos, ordinal);
                                yield return span;
                            }
                            yield break;
                        }
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        void Reset()
        {
            foreach (var machine in tokenRegexps)
                machine.Reset();
        }

        #endregion
    }

    public abstract class TokenRegexp : StringRegexp
    {
        public TokenRegexp(string regexp, bool literal = false)
            : base(regexp, literal)
        {
        }

        public abstract TokenSpan OnReadToken(string text, ulong charPos, ulong charNext, ulong ordinal);
    }

    public abstract class TokenSpan : BaseSpan
    {
        public string Text { get; protected set; }

        public override string ToString()
        {
            return string.Format("{{ {0}:{1} {2}-{3} \"{4}\"}}", this.GetType().Name, Ordinal, CharPos, CharNext, System.Text.RegularExpressions.Regex.Escape(Text));
        }
    }
}
