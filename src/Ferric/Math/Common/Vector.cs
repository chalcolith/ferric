using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Math.Common
{
    public interface IVector<T> : IEnumerable<T>
        where T : struct, IComparable
    {
        T this[int i] { get; set; }
        int Dimensions { get; }

        T AbsSquared();
        T Abs();
    }
}
