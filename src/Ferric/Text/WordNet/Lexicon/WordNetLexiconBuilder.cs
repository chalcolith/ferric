using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Text.Common;
using Ferric.Text.Common.Documents;
using Ferric.Text.Common.Tokenizer;

namespace Ferric.Text.WordNet.Lexicon
{
    public class WordNetLexiconBuilder : BaseTransducer<IDocument, IDocumentCollection>
    {
        readonly string wordNetDir;
        readonly string extrasFname;

        public WordNetLexiconBuilder(ICreateContext context, string wordNetDir, string extras)
            : base(context)
        {
            this.wordNetDir = wordNetDir;
            this.extrasFname = extras;
        }

        public override IEnumerable<IDocumentCollection> Process(IEnumerable<IDocument> inputs)
        {
            var dbPath = CreateContext.GetFullPath(Path.Combine(wordNetDir, "WordNet.mdf"));

            using (var context = Data.WordNet.CreateContext(dbPath))
            {
                var lexicon = new SynsetLexicon(context, CreateContext.GetFullPath(extrasFname), true);
                var morph = new Morph.Morphy(context, wordNetDir);

                // now process documents
                var collection = new DocumentCollection
                {
                    Lexicon = lexicon,
                    Documents = new List<IDocument>()
                };

                foreach (var document in inputs)
                {
                    collection.Documents.Add(document);

                    foreach (var token in document.ChildrenOfType<TokenSpan>().Where(t => t.TokenClass == TokenClass.Word))
                    {
                        token.Lemmas = token.Lemmas.SelectMany(l =>
                        {
                            var stems = morph.GetStems(l.Lemma).ToList();
                            return stems.Select(s => new TokenLemma { Weight = l.Weight / stems.Count, Lemma = s.Item2 });
                        });

                        foreach (var lemma in token.Lemmas)
                            lexicon.AddLemma(lemma.Lemma);
                    }
                }

                //
                return new[] { collection };
            }
        }
    }
}
