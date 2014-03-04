using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Math.Linear
{
    public interface Matrix<T> : IEquatable<Matrix<T>>, ISerializable
    {
        int Rows { get; }
        int Cols { get; }
        T this[int row, int col] { get; }

        BaseMatrix<T> Transpose();
        BaseMatrix<T> ScalarMultiply(T n, bool inPlace = false);
        BaseMatrix<T> Add(Matrix<T> m, bool inPlace = false);
        BaseMatrix<T> Negate(bool inPlace = false);
        BaseMatrix<T> Subtract(Matrix<T> m, bool inPlace = false);
        BaseMatrix<T> Multiply(Matrix<T> m, bool inPlace = false);
        BaseMatrix<T> Inverse();
    }

    public abstract class BaseMatrix<T> : Matrix<T>
    {
        #region Matrix<T> Members

        public int Rows { get; protected set; }
        public int Cols { get; protected set; }
        public abstract T this[int row, int col] { get; set; }

        public abstract BaseMatrix<T> Transpose();
        public abstract BaseMatrix<T> ScalarMultiply(T n, bool inPlace = false);
        public abstract BaseMatrix<T> Add(Matrix<T> m, bool inPlace = false);
        public abstract BaseMatrix<T> Negate(bool inPlace = false);
        public abstract BaseMatrix<T> Subtract(Matrix<T> m, bool inPlace = false);
        public abstract BaseMatrix<T> Multiply(Matrix<T> m, bool inPlace = false);
        public abstract BaseMatrix<T> Inverse();

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

        public static BaseMatrix<T> operator *(BaseMatrix<T> a, T n)
        {
            return a.ScalarMultiply(n, inPlace: false);
        }

        public static BaseMatrix<T> operator *(BaseMatrix<T> a, Matrix<T> b)
        {
            return a.Multiply(b, inPlace: false);
        }

        public static BaseMatrix<T> operator +(BaseMatrix<T> a)
        {
            return a;
        }

        public static BaseMatrix<T> operator +(BaseMatrix<T> a, Matrix<T> b)
        {
            return a.Add(b, inPlace: false);
        }

        public static BaseMatrix<T> operator -(BaseMatrix<T> a)
        {
            return a.Negate(inPlace: false);
        }

        public static BaseMatrix<T> operator -(BaseMatrix<T> a, Matrix<T> b)
        {
            return a.Subtract(b, inPlace: false);
        }

        #endregion
    }

    public interface Vector<T> : Matrix<T>
    {
        int Dimensions { get; }
    }
}
