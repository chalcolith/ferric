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

        Matrix<T> Transpose();
        Matrix<T> ScalarMult(T n, bool inPlace = false);
        Matrix<T> Add(Matrix<T> m, bool inPlace = false);
        Matrix<T> Negate(bool inPlace = false);
        Matrix<T> Subtract(Matrix<T> m, bool inPlace = false);
        Matrix<T> Multiply(Matrix<T> m, bool inPlace = false);
    }

    public abstract class BaseMatrix<T> : Matrix<T>
    {
        #region Matrix<T> Members

        public int Rows { get; protected set; }
        public int Cols { get; protected set; }
        public abstract T this[int row, int col] { get; set; }

        public Matrix<T> Transpose() { return BaseTranspose(); }

        public Matrix<T> ScalarMult(T n, bool inPlace = false) { return BaseScalarMult(n, inPlace); }

        public Matrix<T> Add(Matrix<T> m, bool inPlace = false) { return BaseAdd(m, inPlace); }

        public Matrix<T> Negate(bool inPlace = false) { return BaseNegate(inPlace); }

        public Matrix<T> Subtract(Matrix<T> m, bool inPlace = false) { return BaseSubtract(m, inPlace); }

        public Matrix<T> Multiply(Matrix<T> m, bool inPlace = false) { return BaseMultiply(m, inPlace); }

        public Matrix<T> Invert() { return BaseInvert(); }

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

        protected abstract BaseMatrix<T> BaseTranspose();
        protected abstract BaseMatrix<T> BaseScalarMult(T n, bool inPlace);
        protected abstract BaseMatrix<T> BaseAdd(Matrix<T> m, bool inPlace);
        protected abstract BaseMatrix<T> BaseNegate(bool inPlace);
        protected abstract BaseMatrix<T> BaseSubtract(Matrix<T> m, bool inPlace);
        protected abstract BaseMatrix<T> BaseMultiply(Matrix<T> m, bool inPlace);
        protected abstract BaseMatrix<T> BaseInvert();

        public static BaseMatrix<T> operator *(BaseMatrix<T> a, T n)
        {
            return a.BaseScalarMult(n, inPlace: false);
        }

        public static BaseMatrix<T> operator *(BaseMatrix<T> a, Matrix<T> b)
        {
            return a.BaseMultiply(b, inPlace: false);
        }

        public static BaseMatrix<T> operator +(BaseMatrix<T> a)
        {
            return a;
        }

        public static BaseMatrix<T> operator +(BaseMatrix<T> a, Matrix<T> b)
        {
            return a.BaseAdd(b, inPlace: false);
        }

        public static BaseMatrix<T> operator -(BaseMatrix<T> a)
        {
            return a.BaseNegate(inPlace: false);
        }

        public static BaseMatrix<T> operator -(BaseMatrix<T> a, Matrix<T> b)
        {
            return a.BaseSubtract(b, inPlace: false);
        }

        #endregion
    }

    public interface Vector<T> : Matrix<T>
    {
        int Dimensions { get; }
    }
}
