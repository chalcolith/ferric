using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Math.Common;
using Ferric.Text.Common.Tokenizer;

namespace Ferric.Text.Common.Documents
{
    public class DtmBuilder<TLexiconEntry> : BaseTransducer<IDocumentCollection<TLexiconEntry>, IDocumentCollection<TLexiconEntry>>
    {
        public DtmBuilder(ICreateContext context)
            : base(context)
        {
        }

        void AddCount(IDictionary<int, double> row, int col, double weight)
        {
            double count;
            row.TryGetValue(col, out count);
            row[col] = count + weight;
        }

        public override IEnumerable<IDocumentCollection<TLexiconEntry>> Process(IEnumerable<IDocumentCollection<TLexiconEntry>> inputs)
        {
            foreach (var collection in inputs)
            {
                var rows = new List<IDictionary<int, double>>();
                foreach (var document in collection.Documents)
                {
                    var row = new Dictionary<int, double>();
                    rows.Add(row);

                    foreach (var token in document.ChildrenOfType<TokenSpan>().Where(t => t.TokenClass == TokenClass.Word))
                    {
                        int num = 0;
                        foreach (var entry in token.PossibleTokens)
                        {
                            if (entry.Indices != null)
                            {
                                foreach (var index in entry.Indices)
                                {
                                    AddCount(row, index, entry.Weight / entry.Indices.Count);
                                    num++;
                                }
                            }
                            else
                            {
                                var indices = collection.Lexicon.GetIndices(entry.Lemma).ToList();
                                foreach (var index in indices)
                                {
                                    AddCount(row, index, entry.Weight / indices.Count);
                                    num++;
                                }
                            }
                        }

                        if (num == 0)
                        {
                            double count;
                            row.TryGetValue(0, out count);
                            row[0] = count + 1;
                        }
                    }
                }

                // build document-term matrix
                int numColumns = collection.Lexicon.IndicesByEntry.Count;
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
            }

            return inputs;
        }
    }
}
