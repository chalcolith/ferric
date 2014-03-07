using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text
{
    public interface IDocument : ISpan
    {
        string Identifier { get; }
    }
}
