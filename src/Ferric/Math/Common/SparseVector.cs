using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Math.Common
{
    public class SparseVector<T> : SparseMatrix<T>, IVector<T>
        where T : struct, IComparable
    {
        public SparseVector(int cols)
            : base(1, cols)
        {
        }

        public SparseVector(IEnumerable<T> data)
            : base(new[] { data })
        {
        }

        #region IVector<T> Members

        public T this[int i]
        {
            get { return this[0, i]; }
            set { this[0, i] = value; }
        }

        public int Dimensions
        {
            get { return this.Cols; }
        }

        public T AbsSquared()
        {
            throw new NotImplementedException();
        }

        public T Abs()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<T> Members

        public new IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<IEnumerable<T>>)this).First().GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
