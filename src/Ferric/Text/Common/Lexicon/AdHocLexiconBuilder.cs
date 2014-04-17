using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Text.Common.Documents;
using Ferric.Text.Common.Tokenizer;

namespace Ferric.Text.Common.Lexicon
{
    public class AdHocLexiconBuilder : BaseTransducer<IDocument, IDocumentCollection>
    {
        public AdHocLexiconBuilder(ICreateContext context)
            : base(context)
        {
        }

        public override IEnumerable<IDocumentCollection> Process(IEnumerable<IDocument> inputs)
        {
            var lexicon = new BaseLexicon();

            lexicon.IndicesByLemma.Clear();
            lexicon.IndicesByLemma[Constants.UnknownToken] = 0;

            lexicon.LemmasByIndex.Clear();
            lexicon.LemmasByIndex[0] = Constants.UnknownToken;

            var collection = new DocumentCollection()
            {
                Lexicon = lexicon,
                Documents = new List<IDocument>()
            };

            // collect tokens and counts
            int nextColumn = 1;
            foreach (var document in inputs)
            {
                collection.Documents.Add(document);

                foreach (var token in document.ChildrenOfType<TokenSpan>()
                                        .Where(t => t.TokenClass == TokenClass.Word))
                {
                    // get column (term index)
                    int column;
                    if (!lexicon.IndicesByLemma.TryGetValue(token.Lemma, out column))
                    {
                        column = nextColumn++;
                        lexicon.IndicesByLemma[token.Lemma] = column;
                        lexicon.LemmasByIndex[column] = token.Lemma;
                    }
                }
            }

            //
            return new[] { collection };
        }
    }
}
