using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.Tokenizer
{
    public class TokenAnnotation : IAnnotation
    {
        #region IAnnotation Members

        public string Name { get { return "TokenAnnotation"; } }

        #endregion

        public ulong Ordinal { get; set; }
    }
}
