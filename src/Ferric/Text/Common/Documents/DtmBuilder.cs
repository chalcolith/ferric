using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Math.Common;
using Ferric.Text.Common.Tokenizer;

namespace Ferric.Text.Common.Documents
{
    public class DtmBuilder : BaseTransducer<IDocumentCollection, IDocumentCollection>
    {
        public DtmBuilder(ICreateContext context)
            : base(context)
        {
        }

        public override IEnumerable<IDocumentCollection> Process(IEnumerable<IDocumentCollection> inputs)
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
                        foreach (var lemma in token.Lemmas)
                        {
                            int col;
                            collection.Lexicon.IndicesByLemma.TryGetValue(lemma.Lemma, out col);

                            double count;
                            row.TryGetValue(col, out count);
                            row[col] = count + lemma.Weight;
                            num++;
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
                int numColumns = collection.Lexicon.IndicesByLemma.Count;
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
