using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Text.Common;
using Ferric.Text.Common.Documents;
using Ferric.Text.Common.Lexicon;
using Ferric.Text.Common.Tokenizer;

namespace Ferric.Text.WordNet.Lexicon
{
    public class WordNetLexiconBuilder : BaseTransducer<IDocument, IDocumentCollection<WordNetEntry>>
    {
        readonly string wordNetDir;
        readonly string extrasFname;
        readonly bool addUnknown;

        public WordNetLexiconBuilder(ICreateContext context, string wordNetDir, string extras, bool addUnknown)
            : base(context)
        {
            this.wordNetDir = wordNetDir;
            this.extrasFname = extras;
            this.addUnknown = addUnknown;
        }

        public override IEnumerable<IDocumentCollection<WordNetEntry>> Process(IEnumerable<IDocument> inputs)
        {
            var wnPath = CreateContext.GetFullPath(wordNetDir);
            var lexicon = new SynsetLexicon(wnPath, CreateContext.GetFullPath(extrasFname), true);
            var morph = new Morph.Morphy(lexicon, wnPath);

            // now process documents
            var collection = new DocumentCollection<WordNetEntry>
            {
                Lexicon = lexicon,
                Documents = new List<IDocument>()
            };

            foreach (var document in inputs)
            {
                collection.Documents.Add(document);

                foreach (var token in document.ChildrenOfType<TokenSpan>().Where(t => t.TokenClass == TokenClass.Word))
                {
                    var stemmedEntries = token.PossibleTokens.SelectMany(orig =>
                    {
                        var stems = morph.GetStems(orig.Lemma).ToList();
                        var possibilities = stems.Select(stem => 
                            new TokenPossibility
                            {
                                Indices = stem.Indices,
                                Lemma = stem.Lemma,
                                Weight = orig.Weight / stems.Count,
                                Data = stem
                            });
                        return possibilities;
                    })
                    .ToList();

                    if (stemmedEntries.Count == 0)
                    {
                        foreach (var possibility in token.PossibleTokens)
                        {
                            var temp = possibility;
                            if (addUnknown)
                            {
                                var newEntry = new WordNetEntry
                                {
                                    Lemma = possibility.Lemma,
                                    PartOfSpeech = Data.SynsetType.Unknown
                                };

                                temp.Indices = lexicon.AddEntry(newEntry);
                            }
                            else
                            {
                                var indices = lexicon.GetIndices(temp.Lemma);
                                temp.Indices = indices.Any() ? new HashSet<int>(indices) : SynsetLexicon.UnknownIndices;
                            }
                        };
                    }
                    else
                    {
                        token.PossibleTokens = stemmedEntries;
                    }
                }
            }

            //
            return new[] { collection };
        }
    }
}
