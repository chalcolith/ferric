using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Math.Linear
{
    public abstract class Matrix<T>
    {
        #region Matrix<T> Members

        public abstract int Rows { get; }
        public abstract int Cols { get; }
        public abstract T this[int row, int col] { get; set; }

        public abstract Matrix<T> Transpose();
        public abstract Matrix<T> ScalarMultiply(T n, bool inPlace = false);
        public abstract Matrix<T> Add(Matrix<T> m, bool inPlace = false);
        public abstract Matrix<T> Negate(bool inPlace = false);
        public abstract Matrix<T> Subtract(Matrix<T> m, bool inPlace = false);
        public abstract Matrix<T> Multiply(Matrix<T> m);
        public abstract Matrix<T> Inverse();

        public void CopyRow(int row, Vector<T> v)
        {
            if (v.Cols != this.Cols)
                throw new ArgumentException("Unable to copy a vector row to a matrix with different dimensions");

            for (int i = 0; i < v.Cols; ++i)
            {
                this[row, i] = v[i];
            }
        }

        public void CopyCol(int col, Vector<T> v)
        {
            if (v.Cols != this.Rows)
                throw new ArgumentException("Unable to copy a vector column to a matrix with different dimensions");

            for (int i = 0; i < v.Cols; ++i)
            {
                this[i, col] = v[i];
            }
        }

        #endregion

        #region Object Members

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Matrix<T>);
        }

        public bool Equals(Matrix<T> m)
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

        public bool Equals(Matrix<double> m, double epsilon)
        {
            if (m == null)
                return false;
            if (this.Rows != m.Rows)
                return false;
            if (this.Cols != m.Cols)
                return false;

            var a = this as Matrix<double>;
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

        public static Matrix<T> operator *(Matrix<T> a, Matrix<T> b)
        {
            return a.Multiply(b);
        }

        public static Matrix<T> operator +(Matrix<T> a)
        {
            return a;
        }

        public static Matrix<T> operator +(Matrix<T> a, Matrix<T> b)
        {
            return a.Add(b, inPlace: false);
        }

        public static Matrix<T> operator -(Matrix<T> a)
        {
            return a.Negate(inPlace: false);
        }

        public static Matrix<T> operator -(Matrix<T> a, Matrix<T> b)
        {
            return a.Subtract(b, inPlace: false);
        }

        #endregion
    }
}
