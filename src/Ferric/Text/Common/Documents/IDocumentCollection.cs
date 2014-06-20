using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ferric.Math.Common;
using Ferric.Text.Common.Lexicon;

namespace Ferric.Text.Common.Documents
{
    public interface IDocumentCollection<TLexiconEntry>
    {
        ILexicon<TLexiconEntry> Lexicon { get; set; }
        IList<IDocument> Documents { get; set; }
        IMatrix<double> DocumentTermMatrix { get; set; }
    }
}
