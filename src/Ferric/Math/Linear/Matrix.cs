using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Math.Linear
{
    public abstract class Matrix<T>
    {
        public int Rows { get; protected set; }
        public int Cols { get; protected set; }
        public abstract T this[int row, int col] { get; set; }

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

        public abstract Matrix<T> Transpose();

        public abstract Matrix<T> ScalarMult(T n, bool inPlace = false);

        public abstract Matrix<T> Add(Matrix<T> m, bool inPlace = false);

        public abstract Matrix<T> Negate(bool inPlace = false);

        public abstract Matrix<T> Subtract(Matrix<T> m, bool inPlace = false);

        #region Object Implementation

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Matrix<T>);
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

        #region Operators

        public static Matrix<T> operator* (Matrix<T> a, T n)
        {
            return a.ScalarMult(n, inPlace: false);
        }

        public static Matrix<T> operator+ (Matrix<T> a)
        {
            return a;
        }

        public static Matrix<T> operator+ (Matrix<T> a, Matrix<T> b)
        {
            return a.Add(b, inPlace: false);
        }

        public static Matrix<T> operator- (Matrix<T> a)
        {
            return a.Negate(inPlace: false);
        }

        public static Matrix<T> operator- (Matrix<T> a, Matrix<T> b)
        {
            return a.Subtract(b, inPlace: false);
        }

        #endregion
    }
}
