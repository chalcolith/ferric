using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Math.Common
{
    public interface IMatrix<T> : IEnumerable<IEnumerable<T>>
        where T : struct, IComparable
    {
        int Rows { get; }
        int Cols { get; }
        T this[int row, int col] { get; set; }
    }

    public abstract class Matrix<T> : IMatrix<T>
        where T : struct, IComparable
    {
        public abstract Matrix<T> Transpose();
        public abstract Matrix<T> ScalarMultiply(T n, bool inPlace = false);
        public abstract Matrix<T> Add(IMatrix<T> m, bool inPlace = false);
        public abstract Matrix<T> Negate(bool inPlace = false);
        public abstract Matrix<T> Subtract(IMatrix<T> m, bool inPlace = false);
        public abstract Matrix<T> Multiply(IMatrix<T> m);
        public abstract Matrix<T> Inverse();

        #region IMatrix Members

        public abstract int Rows { get; protected set; }
        public abstract int Cols { get; protected set; }
        public abstract T this[int row, int col] { get; set; }

        #endregion

        #region Object Members

        public override bool Equals(object obj)
        {
            return this.Equals(obj as IMatrix<T>);
        }

        public bool Equals(IMatrix<T> m)
        {
            if (m == null)
                return false;
            if (this.Rows != m.Rows)
                return false;
            if (this.Cols != m.Cols)
                return false;

            for (var i = 0; i < this.Rows; ++i)
            {
                for (var j = 0; j < this.Cols; ++j)
                {
                    if (!this[i, j].Equals(m[i, j]))
                        return false;
                }
            }
            return true;
        }

        public bool Equals(IMatrix<double> m, double epsilon)
        {
            if (m == null)
                return false;
            if (this.Rows != m.Rows)
                return false;
            if (this.Cols != m.Cols)
                return false;

            var a = this as IMatrix<double>;
            if (a == null)
                throw new ArgumentException("Unable to compare double and non-double matrices");

            for (var i = 0; i < this.Rows; ++i)
            {
                for (var j = 0; j < this.Cols; ++j)
                {
                    if (System.Math.Abs(a[i, j] - m[i, j]) > epsilon)
                        return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            int res = 0;

            for (var i = 0; i < this.Rows; ++i)
            {
                for (var j = 0; j < this.Cols; ++j)
                {
                    res ^= this[i, j].GetHashCode();
                }
            }

            return res;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("{ ");
            for (var i = 0; i < this.Rows; ++i)
            {
                if (i > 0)
                    sb.Append(", ");

                sb.Append("{ ");
                for (var j = 0; j < this.Cols; ++j)
                {
                    if (j > 0)
                        sb.Append(", ");
                    sb.Append(this[i, j]);
                }
                sb.Append(" }");
            }
            sb.Append(" }");
            return sb.ToString();
        }

        #endregion

        #region ISerializable Members

        public abstract void GetObjectData(SerializationInfo info, StreamingContext context);

        #endregion

        #region Operators

        public static Matrix<T> operator *(Matrix<T> a, T n)
        {
            return a.ScalarMultiply(n, inPlace: false);
        }

        public static Matrix<T> operator *(Matrix<T> a, IMatrix<T> b)
        {
            return a.Multiply(b);
        }

        public static Matrix<T> operator +(Matrix<T> a)
        {
            return a;
        }

        public static Matrix<T> operator +(Matrix<T> a, IMatrix<T> b)
        {
            return a.Add(b, inPlace: false);
        }

        public static Matrix<T> operator -(Matrix<T> a)
        {
            return a.Negate(inPlace: false);
        }

        public static Matrix<T> operator -(Matrix<T> a, IMatrix<T> b)
        {
            return a.Subtract(b, inPlace: false);
        }

        #endregion

        #region IEnumerable<IEnumerable<T>> Members

        public virtual IEnumerator<IEnumerable<T>> GetEnumerator()
        {
            return new RowEnumerator(this);
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Enumerator Classes

        class ColEnumerator : IEnumerator<T>
        {
            Matrix<T> matrix;
            int row, col;

            public ColEnumerator(Matrix<T> m, int row)
            {
                this.matrix = m;
                this.row = row;
                this.col = -1;
            }

            public IEnumerable<T> AsEnumerable()
            {
                this.Reset();
                while (this.MoveNext())
                    yield return this.Current;
            }

            #region IEnumerator<T> Members

            public T Current
            {
                get 
                {
                    if (col < 0 || col >= matrix.Cols)
                        throw new ArgumentOutOfRangeException("Matrix index out of range.");
                    return matrix[row, col];
                }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                matrix = null;
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get 
                {
                    return Current;
                }
            }

            public bool MoveNext()
            {
                if (++col >= matrix.Cols)
                    return false;
                return true;
            }

            public void Reset()
            {
                col = -1;
            }

            #endregion
        }

        class RowEnumerator : IEnumerator<IEnumerable<T>>
        {
            Matrix<T> matrix;
            int row;
            IEnumerable<T> cur;

            public RowEnumerator(Matrix<T> m)
            {
                this.matrix = m;
                this.row = -1;
            }

            #region IEnumerator<IEnumerable<T>> Members

            public IEnumerable<T> Current
            {
                get 
                {
                    if (row < 0 || row >= matrix.Rows)
                        throw new ArgumentOutOfRangeException("Matrix index out of range.");
                    return cur ?? (cur = new ColEnumerator(matrix, row).AsEnumerable());
                }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                matrix = null;
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                cur = null;
                if (++row >= matrix.Rows)
                    return false;
                return true;
            }

            public void Reset()
            {
                row = -1;
            }

            #endregion
        }

        #endregion

        #region Utilities

        public void CopyRow(int row, IVector<T> v)
        {
            if (v.Dimensions != this.Cols)
                throw new ArgumentException("Unable to copy a vector row to a matrix with different dimensions");

            for (int i = 0; i < v.Dimensions; ++i)
            {
                this[row, i] = v[i];
            }
        }

        public void CopyCol(int col, IVector<T> v)
        {
            if (v.Dimensions != this.Rows)
                throw new ArgumentException("Unable to copy a vector column to a matrix with different dimensions");

            for (int i = 0; i < v.Dimensions; ++i)
            {
                this[i, col] = v[i];
            }
        }

        #endregion
    }
}
