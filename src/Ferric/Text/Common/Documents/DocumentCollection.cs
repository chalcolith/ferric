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

            sb.AppendFormat("lemmas:    {0}\t{1}", Lexicon.Lemmas.Count, Lexicon.GetType().FullName);
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendFormat("\t{0}", string.Join("\t", Enumerable.Range(0, Lexicon.Lemmas.Count).Select(i =>
                {
                    var lemma = Lexicon.Indices[i];
                    return lemma.Length < 8
                        ? lemma
                        : lemma.Substring(0, 5) + "..";
                })));
            sb.AppendLine();

            for (int row = 0; row < DocumentTermMatrix.Rows; row++)
            {
                sb.AppendFormat("{0}\t{1}", row, string.Join("\t", Enumerable.Range(0, Lexicon.Lemmas.Count).Select(col =>
                    {
                        var count = DocumentTermMatrix[row, col];
                        return count > 0 ? count.ToString() : "";
                    })));
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
