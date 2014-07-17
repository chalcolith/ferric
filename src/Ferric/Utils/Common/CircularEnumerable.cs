using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Utils.Common
{
    public class CircularEnumerable<T> : IEnumerable<T>
    {
        IEnumerable<T> inner;

        public CircularEnumerable(IEnumerable<T> inner)
        {
            this.inner = inner.ToList();
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return new CircularEnumerator<T>(inner.GetEnumerator());
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }

    class CircularEnumerator<T> : IEnumerator<T>
    {
        IEnumerator<T> inner;

        public CircularEnumerator(IEnumerator<T> inner)
        {
            this.inner = inner;
        }

        #region IEnumerator<T> Members

        public T Current
        {
            get { return inner.Current; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.inner = null;
        }

        #endregion

        #region IEnumerator Members

        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }

        public bool MoveNext()
        {
            if (inner.MoveNext())
                return true;

            inner.Reset();
            return inner.MoveNext();
        }

        public void Reset()
        {
            inner.Reset();
        }

        #endregion
    }
}
