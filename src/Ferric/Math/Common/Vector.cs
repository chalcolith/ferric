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
    }

    public abstract class Vector<T> : Matrix<T>, IVector<T>
        where T : struct, IComparable
    {
        #region IVector<T> Members

        public abstract T this[int i] {get; set;}
        public abstract int Dimensions {get; }

        #endregion

        #region IEnumerable<T> Members

        public abstract IEnumerator<T> GetEnumerator();

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
