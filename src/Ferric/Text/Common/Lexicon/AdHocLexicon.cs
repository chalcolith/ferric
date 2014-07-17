using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Text.Common.Documents;
using Ferric.Text.Common.Tokenizer;

namespace Ferric.Text.Common.Lexicon
{
    public class AdHocLexicon : BaseTransducer<IDocument, IDocumentCollection<string>>
    {
        FlatFileLexicon lexicon;

        public AdHocLexicon(ICreateContext context, string file)
            : base(context)
        {
            this.lexicon = new FlatFileLexicon(CreateContext.GetFullPath(file), false);
        }

        public override IEnumerable<IDocumentCollection<string>> Process(IEnumerable<IDocument> inputs)
        {
            var collection = new DocumentCollection<string>()
            {
                Lexicon = lexicon,
                Documents = inputs.ToList()
            };

            return new[] { collection };
        }
    }

    public class AdHocLexiconBuilder : BaseTransducer<IDocument, IDocumentCollection<string>>
    {
        string fname;
        FlatFileLexicon lexicon;

        public AdHocLexiconBuilder(ICreateContext context, string file)
            : base(context)
        {
            this.fname = file;
            this.lexicon = new FlatFileLexicon(CreateContext.GetFullPath(fname), true);
        }

        public override IEnumerable<IDocumentCollection<string>> Process(IEnumerable<IDocument> inputs)
        {
            var collection = new DocumentCollection<string>()
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
                    foreach (var entry in token.PossibleTokens)
                        lexicon.AddEntry(entry.Lemma);
                }
            }

            //
            lexicon.Save();
            return new[] { collection };
        }
    }
}
