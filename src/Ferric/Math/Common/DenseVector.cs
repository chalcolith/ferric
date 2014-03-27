using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Math.Common
{
    public class DenseVector<T> : DenseMatrix<T>, IVector<T>
        where T: struct, IComparable
    {
        public DenseVector(int cols)
            : base(1, cols)
        {
        }

        public DenseVector(IEnumerable<T> data, bool copy = false)
            : base(new[] { data }, copy:false)
        {
            if (copy)
            {
                var v = data.ToArray();
                this.squareData = new T[1, v.Length];
                for (int i = 0; i < v.Length; ++i)
                    this[i] = v[i];
                this.jaggedData = null;
            }
        }

        #region IVector<T> Members

        public T this[int i]
        {
            get { return this[0, i]; }
            set { this[0, i] = value; }
        }

        public int Dimensions { get { return this.Cols; } }

        public T AbsSquared()
        {
            throw new NotImplementedException();

            //var m = this as DenseVector<double>;
            //if (m == null)
            //    throw new ArgumentException("Cannot take the absolute value of a non-double vector");

            //double sum = 0;
            //for (int i = 0; i < m.Dimensions; ++i)
            //    sum += m[i] * m[i];
            //return sum;
        }

        public T Abs()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<T> Members

        public new IEnumerator<T> GetEnumerator()
        {
            return new VectorEnumerator(this);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Enumerator Class

        class VectorEnumerator : IEnumerator<T>
        {
            DenseVector<T> vector;
            int pos = -1;

            public VectorEnumerator(DenseVector<T> vector)
            {
                this.vector = vector;
            }

            #region IEnumerator<T> Members

            public T Current
            {
                get { return vector[pos]; }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                vector = null;
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return this.Current; }
            }

            public bool MoveNext()
            {
                return ++pos < vector.Cols;
            }

            public void Reset()
            {
                pos = -1;
            }

            #endregion
        }

        #endregion

        #region Operators

        public static DenseVector<T> operator +(DenseVector<T> a, DenseVector<T> b)
        {
            if (a.Dimensions != b.Dimensions)
                throw new ArgumentException("Cannot add vectors of different dimension");
            var res = new DenseVector<T>(a, copy: true);
            var temp = new DenseMatrix<T>(res.squareData, copy: false);
            temp.Add(b, inPlace: true);
            return res;
        }

        public static DenseVector<T> operator -(DenseVector<T> a, DenseVector<T> b)
        {
            if (a.Dimensions != b.Dimensions)
                throw new ArgumentException("Cannot subtract vectors of different dimension");
            var res = new DenseVector<T>(a, copy: true);
            var temp = new DenseMatrix<T>(res.squareData, copy: false);
            temp.Subtract(b, inPlace: true);
            return res;
        }

        #endregion
    }
}
