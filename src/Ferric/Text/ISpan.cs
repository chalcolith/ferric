using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text
{
    public interface IAnnotation
    {
        string Name { get; }
    }

    public interface ISpan
    {
        ulong CharPos { get; }
        ulong CharNext { get; }
        ulong Ordinal { get; }

        IList<IAnnotation> Annotations { get; }
        IList<ISpan> Children { get; }
    }
}
