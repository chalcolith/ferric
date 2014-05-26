using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Math.Common;

namespace Ferric.Text.Common.Documents
{
    public class DocumentCollection : IDocumentCollection
    {
        #region IDocumentCollection Members

        public Lexicon.ILexicon Lexicon { get; set; }

        public IList<IDocument> Documents { get; set; }

        public IMatrix<double> DocumentTermMatrix { get; set; }

        #endregion

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("documents: {0}", Documents.Count);
            sb.AppendLine();

            sb.AppendFormat("lemmas:    {0}\t{1}", Lexicon.IndicesByLemma.Count, Lexicon.GetType().FullName);
            sb.AppendLine();

            var words = new Dictionary<int, IDictionary<int, int>>();

            var sparse = DocumentTermMatrix as SparseMatrix<double>;
            if (sparse != null)
            {
                foreach (var tuple in sparse.GetDictionaryOfKeys())
                {
                    var doc = tuple.Item1;
                    var index = tuple.Item2;
                    var count = (int)tuple.Item3;

                    IDictionary<int, int> counts;
                    if (!words.TryGetValue(index, out counts))
                    {
                        counts = new Dictionary<int, int>();
                        words[index] = counts;
                    }

                    if (count != 0)
                        counts[doc] = count;
                }
            }
            else
            {
                for (int row = 0; row < DocumentTermMatrix.Rows; row++)
                {
                    for (int col = 0; col < DocumentTermMatrix.Cols; col++)
                    {
                        IDictionary<int, int> counts;
                        if (!words.TryGetValue(col, out counts))
                        {
                            counts = new Dictionary<int, int>();
                            words[row] = counts;
                        }

                        var count = (int)DocumentTermMatrix[row, col];
                        if (count != 0)
                            counts[row] = count;
                    }
                }
            }

            sb.AppendFormat("\t{0}", string.Join("\t", words.Select(kv =>
                {
                    var lemma = Lexicon.LemmasByIndex[kv.Key];
                    return lemma.Length < 8
                        ? lemma
                        : lemma.Substring(0, 5) + "..";
                })));
            sb.AppendLine();

            for (int row = 0; row < DocumentTermMatrix.Rows; row++)
            {
                sb.AppendFormat("{0}\t{1}", row, string.Join("\t", words.Select(kv =>
                    {
                        int count;
                        return kv.Value.TryGetValue(row, out count) ? count.ToString() : "";
                    })));
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
