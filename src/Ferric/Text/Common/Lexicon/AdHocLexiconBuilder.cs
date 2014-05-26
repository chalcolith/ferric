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
        string fname;

        public AdHocLexiconBuilder(ICreateContext context, string file)
            : base(context)
        {
            this.fname = file;
        }

        public override IEnumerable<IDocumentCollection> Process(IEnumerable<IDocument> inputs)
        {
            var lexicon = new FileLexicon(CreateContext.GetFullPath(fname), true);

            var collection = new DocumentCollection()
            {
                Lexicon = lexicon,
                Documents = new List<IDocument>()
            };

            // collect tokens and counts
            foreach (var document in inputs)
            {
                collection.Documents.Add(document);

                foreach (var token in document.ChildrenOfType<TokenSpan>().Where(t => t.TokenClass == TokenClass.Word))
                {
                    lexicon.AddLemma(token.Lemma);
                }
            }

            //
            lexicon.Save();
            return new[] { collection };
        }
    }
}
