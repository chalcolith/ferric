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
    public class AdHocLexicon : BaseTransducer<IDocument, IDocumentCollection>, ILexicon
    {
        public AdHocLexicon(ICreateContext context)
            : base(context)
        {
            Lemmas = new Dictionary<string, int>();
            Indices = new Dictionary<int, string>();
        }

        #region ILexicon Members

        public IDictionary<string, int> Lemmas { get; protected set; }
        public IDictionary<int, string> Indices { get; protected set; }

        #endregion

        public override IEnumerable<IDocumentCollection> Process(IEnumerable<IDocument> inputs)
        {
            Lemmas.Clear();
            Lemmas[Constants.UnknownToken] = 0;

            Indices.Clear();
            Indices[0] = Constants.UnknownToken;

            var collection = new DocumentCollection()
            {
                Lexicon = this,
                Documents = new List<IDocument>()
            };

            // collect tokens and counts
            int nextColumn = 1;
            var rows = new List<IDictionary<int, int>>();
            foreach (var document in inputs)
            {
                collection.Documents.Add(document);

                var row = new Dictionary<int, int>();
                rows.Add(row);

                foreach (var token in document.ChildrenOfType<TokenSpan>())
                {
                    if (token.TokenClass != TokenClass.Word)
                        continue;

                    // get column (term index)
                    int column;
                    if (!Lemmas.TryGetValue(token.Lemma, out column))
                    {
                        column = nextColumn++;
                        Lemmas[token.Lemma] = column;
                        Indices[column] = token.Lemma;
                    }

                    // increment term value
                    int count;
                    row.TryGetValue(column, out count);
                    row[column] = count+1;
                }
            }

            // build document-term matrix
            int numColumns = Lemmas.Count;
            int numRows = rows.Count;

            var dtm = new SparseMatrix<double>(numRows, numColumns);
            int curRow = 0;
            foreach (var row in rows)
            {
                foreach (var kv in row)
                    dtm[curRow, kv.Key] = kv.Value;
                curRow++;
            }

            collection.DocumentTermMatrix = dtm;

            return new[] { collection };
        }
    }
}
