using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Math.Common;
using Ferric.Text.Common.Documents;
using Ferric.Text.Common.Tokenizer;

namespace Ferric.Text.Common.Lexicon
{
    public abstract class BaseLexicon : ILexicon
    {
        public BaseLexicon()
        {
            IndicesByLemma = new Dictionary<string, int>();
            LemmasByIndex = new Dictionary<int, string>();

            IndicesByLemma[Constants.UnknownToken] = 0;
            LemmasByIndex[0] = Constants.UnknownToken;
        }

        #region ILexicon Members

        public IDictionary<string, int> IndicesByLemma { get; protected set; }
        public IDictionary<int, string> LemmasByIndex { get; protected set; }
        public abstract int AddLemma(string lemma);

        #endregion
    }
}
