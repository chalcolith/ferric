using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Text.Common.Tokenizer;

namespace Ferric.Text.Common.Documents
{
    public class TokenTypeFilter : BaseTransducer<ISpan, ISpan>
    {
        ulong classFilters = 0;

        public TokenTypeFilter(ICreateContext context, string filter)
            : base(context)
        {
            LoadClassesToFilter(filter);
        }

        public override IEnumerable<ISpan> Process(IEnumerable<ISpan> inputs)
        {
            foreach (var input in inputs)
            {
                var token = input as TokenSpan;
                if (token != null)
                {
                    if ((classFilters & (1ul << (int)token.TokenClass)) == 0)
                        continue;
                    yield return input;
                }
            }
        }

        void LoadClassesToFilter(string classesToFilter)
        {
            classFilters = 0;
            if (string.IsNullOrWhiteSpace(classesToFilter))
                return;

            foreach (var classStr in classesToFilter.Split(','))
            {
                TokenClass tc;
                if (Enum.TryParse<TokenClass>(classStr.Trim(), out tc))
                {
                    classFilters |= (1ul << (int)tc);
                }
            }
        }
    }
}
