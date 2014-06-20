using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Math.Common
{
    public class SparseMatrix<T> : Matrix<T>, ISerializable
        where T : struct, IComparable
    {
        object changeLock = new object();
        IDictionary<Tuple<int, int>, T> dok = null;
        
        T[] values = null;
        int[] row_offsets = null;
        int[] col_offsets = null;

        public SparseMatrix(int rows, int cols)
        {
            this.Rows = rows;
            this.Cols = cols;
        }

        public SparseMatrix(T[,] data)
        {
            this.Rows = data.GetLength(0);
            this.Cols = data.GetLength(1);

            dok = new Dictionary<Tuple<int, int>, T>();
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Cols; col++)
                {
                    dok[Tuple.Create(row, col)] = data[row, col];
                }
            }
        }

        public SparseMatrix(IEnumerable<IEnumerable<T>> data)
        {
            dok = new Dictionary<Tuple<int, int>, T>();

            int i = 0;
            foreach (var row in data)
            {
                int j = 0;
                foreach (var val in row)
                {
                    if (!IsZero(val))
                        dok[Tuple.Create(i, j)] = val;

                    j++;
                }

                this.Cols = j;
                i++;
            }
            this.Rows = i;
        }

        public IEnumerable<Tuple<int, int, T>> GetDictionaryOfKeys()
        {
            lock (changeLock)
            {
                ChangeToDok();
                return dok.Select(kv => Tuple.Create(kv.Key.Item1, kv.Key.Item2, kv.Value));
            }
        }

        #region Object Members

        public override string ToString()
        {
            return "<SparseMatrix>";
        }

        #endregion

        #region Matrix<T> Members

        public override int Rows { get; protected set; }
        public override int Cols { get; protected set; }

        public override T this[int row, int col]
        {
            get
            {
                if (row < 0 || row >= Rows)
                    throw new ArgumentOutOfRangeException("row");
                if (col < 0 || col >= Cols)
                    throw new ArgumentOutOfRangeException("col");

                lock (changeLock)
                {
                    if (values != null)
                    {
                        int first = row_offsets[row];
                        int next = row_offsets[row + 1];
                        int index = Array.BinarySearch<int>(col_offsets, first, next - first, col);
                        if (index < 0)
                            return default(T);
                        return values[col_offsets[index]];
                    }
                    else if (dok != null)
                    {
                        T result;
                        dok.TryGetValue(Tuple.Create(row, col), out result);
                        return result;
                    }
                    else
                    {
                        return default(T);
                    }
                }
            }
            set
            {
                lock (changeLock)
                {
                    var key = Tuple.Create(row, col);

                    if (values != null)
                        ChangeToDok();
                    if (dok == null)
                        dok = new Dictionary<Tuple<int, int>, T>();
                    if (value.Equals(default(T)))
                        dok.Remove(key);
                    else
                        dok[key] = value;
                }
            }
        }

        public override Matrix<T> Transpose()
        {
            lock (changeLock)
            {
                var result = new SparseMatrix<T>(this.Cols, this.Rows);
                result.dok = new Dictionary<Tuple<int, int>, T>();

                if (values != null)
                {
                    int row_index = 1;
                    for (int i = 0; i < values.Length; ++i)
                    {
                        if (i >= row_offsets[row_index])
                            row_index++;

                        result.dok[Tuple.Create(row_index - 1, col_offsets[i])] = values[i];
                    }
                }
                else if (dok != null)
                {
                    foreach (var kv in dok)
                        result.dok[Tuple.Create(kv.Key.Item2, kv.Key.Item1)] = kv.Value;
                }

                return result;
            }
        }

        public override Matrix<T> ScalarMultiply(T n, bool inPlace = false)
        {
            var result = inPlace ? this : new SparseMatrix<T>(this.Rows, this.Cols);
            if (n.Equals(default(T)))
                return result;

            if (typeof(T) == typeof(double))
            {
                double nd = Convert.ToDouble(n);
                ScalarOp<double>(this as SparseMatrix<double>, result as SparseMatrix<double>, (double a) => a * nd);
            }
            else if (typeof(T) == typeof(int))
            {
                int ni = Convert.ToInt32(n);
                ScalarOp<int>(this as SparseMatrix<int>, result as SparseMatrix<int>, (int a) => a * ni);
            }
            else
            {
                var mul = typeof(T).GetMethod("op_Multiply", BindingFlags.Static | BindingFlags.Public);
                if (mul == null)
                    throw new ArgumentException("Unable to find a multiplication operator for " + typeof(T).FullName);

                object[] parms = new object[2];
                parms[1] = n;

                ScalarOp<T>(this, result, (T a) =>
                    {
                        parms[0] = a;
                        return (T)mul.Invoke(null, parms);
                    });
            }

            return result;
        }

        public override Matrix<T> Add(IMatrix<T> m, bool inPlace = false)
        {
            if (this.Rows != m.Rows || this.Cols != m.Cols)
                throw new ArgumentException("Unable to add matrices of different dimensions.");

            var result = inPlace ? this : new SparseMatrix<T>(this.Rows, this.Cols);

            if (typeof(T) == typeof(double))
            {
                AdditiveOp<double>(this as SparseMatrix<double>, m as IMatrix<double>, result, (double a, double b) => a + b);
            }
            else if (typeof(T) == typeof(int))
            {
                AdditiveOp<int>(this as SparseMatrix<int>, m as IMatrix<int>, result, (int a, int b) => a + b);
            }
            else
            {
                var add = typeof(T).GetMethod("op_Addition", BindingFlags.Static | BindingFlags.Public);
                if (add == null)
                    throw new ArgumentException("Unable to find an addition operator for " + typeof(T).FullName);

                object[] parms = new object[2];
                AdditiveOp<T>(this, m, result, (T a, T b) =>
                    {
                        parms[0] = a;
                        parms[1] = b;
                        return (T)add.Invoke(null, parms);
                    });
            }

            return result;
        }

        public override Matrix<T> Negate(bool inPlace = false)
        {
            var result = inPlace ? this : new SparseMatrix<T>(this.Rows, this.Cols);

            if (typeof(T) == typeof(double))
            {
                ScalarOp<double>(this as SparseMatrix<double>, result as SparseMatrix<double>, (double a) => -a);
            }
            else if (typeof(T) == typeof(int))
            {
                ScalarOp<int>(this as SparseMatrix<int>, result as SparseMatrix<int>, (int a) => -a);
            }
            else
            {
                var neg = typeof(T).GetMethod("op_UnaryNegation", BindingFlags.Static | BindingFlags.Public);
                if (neg == null)
                    throw new ArgumentException("Unable to find a unary negation operator for " + typeof(T).FullName);

                object[] parms = new object[1];
                ScalarOp<T>(this, result, (T a) =>
                    {
                        parms[0] = a;
                        return (T)neg.Invoke(null, parms);
                    });
            }

            return result;
        }

        public override Matrix<T> Subtract(IMatrix<T> m, bool inPlace = false)
        {
            if (this.Rows != m.Rows || this.Cols != m.Cols)
                throw new ArgumentException("Unable to subtract matrices of different dimensions.");

            var result = inPlace ? this : new SparseMatrix<T>(this.Rows, this.Cols);

            if (typeof(T) == typeof(double))
            {
                AdditiveOp<double>(this as SparseMatrix<double>, m as IMatrix<double>, result, (double a, double b) => a - b);
            }
            else if (typeof(T) == typeof(int))
            {
                AdditiveOp<int>(this as SparseMatrix<int>, m as IMatrix<int>, result, (int a, int b) => a - b);
            }
            else
            {
                var sub = typeof(T).GetMethod("op_Subtraction", BindingFlags.Static | BindingFlags.Public);
                if (sub == null)
                    throw new ArgumentException("Unable to find a subtraction operator for " + typeof(T).FullName);

                object[] parms = new object[2];
                AdditiveOp<T>(this, m, result, (T a, T b) =>
                    {
                        parms[0] = a;
                        parms[1] = b;
                        return (T)sub.Invoke(null, parms);
                    });
            }

            return result;
        }

        public override Matrix<T> Multiply(IMatrix<T> m)
        {
            if (this.Cols != m.Rows)
                throw new ArgumentException("Unable to multiply nonconformable matrices.");

            var result = new SparseMatrix<T>(this.Rows, m.Cols);

            if (typeof(T) == typeof(double))
            {
                Multiply<double>(this as SparseMatrix<double>, m as IMatrix<double>, result, 
                    (double a, double b) => a + b, (double a, double b) => a * b, 
                    (double n) => IsZero(n));
            }
            else if (typeof(T) == typeof(int))
            {
                Multiply<int>(this as SparseMatrix<int>, m as IMatrix<int>, result,
                    (int a, int b) => a + b, (int a, int b) => a * b,
                    (int n) => n == 0);
            }
            else
            {
                var add = typeof(T).GetMethod("op_Addition", BindingFlags.Static | BindingFlags.Public);
                if (add == null)
                    throw new ArgumentException("Unable to find an addition operator for " + typeof(T).FullName);
                var mul = typeof(T).GetMethod("op_Multiply", BindingFlags.Static | BindingFlags.Public);
                if (mul == null)
                    throw new ArgumentException("Unable to find a multiplication operator for " + typeof(T).FullName);

                object[] parms = new object[2];
                Multiply<T>(this, m, result,
                    (T a, T b) =>
                        {
                            parms[0] = a;
                            parms[1] = b;
                            return (T)add.Invoke(null, parms);
                        },
                    (T a, T b) =>
                        {
                            parms[0] = a;
                            parms[1] = b;
                            return (T)mul.Invoke(null, parms);
                        },
                    (T n) => n.Equals(default(T)));
            }

            return result;
        }

        public override Matrix<T> Inverse()
        {
            var m = this as SparseMatrix<double>;
            if (m == null)
                throw new ArgumentException("Unable to invert a non-double matrix");

            if (this.Rows != this.Cols)
                throw new ArgumentException("Unable to invert a non-square matrix");

            // inversion algo taken from http://msdn.microsoft.com/en-us/magazine/jj863137.aspx

            var n = m.Rows;
            var res = new SparseMatrix<double>(m.Rows, m.Cols);
            for (int i = 0; i < m.Rows; ++i)
            {
                for (int j = 0; j < m.Cols; ++j)
                {
                    if (!IsZero(m[i, j]))
                        res[i, j] = m[i, j];
                }
            }

            int[] perm;
            int toggle;
            var lum = Decompose(res, out perm, out toggle);
            if (lum == null)
                throw new ArgumentException("Matrix is not invertible");

            var b = new double[n];
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    if (i == perm[j])
                        b[j] = 1;
                    else
                        b[j] = 0;
                }

                var x = HelperSolve(lum, b);

                for (int j = 0; j < n; ++j)
                {
                    res[j, i] = x[j];
                }
            }

            return res as Matrix<T>;
        }

        static Matrix<double> Decompose(Matrix<double> m, out int[] perm, out int toggle)
        {
            if (m.Rows != m.Cols)
                throw new ArgumentException("Unable to decompose a non-square matrix");

            int n = m.Rows;
            var result = new SparseMatrix<double>(m);

            perm = new int[n];
            for (int i = 0; i < n; ++i) { perm[i] = i; }

            toggle = 1;

            for (int j = 0; j < n - 1; ++j)
            {
                double max = System.Math.Abs(result[j, j]);
                int maxRow = j;
                for (int i = j + 1; i < n; ++i)
                {
                    if (result[i, j] > max)
                    {
                        max = result[i, j];
                        maxRow = i;
                    }
                }

                if (maxRow != j)
                {
                    for (int k = 0; k < n; ++k)
                    {
                        var temp = result[maxRow, k];
                        result[maxRow, k] = result[j, k];
                        result[j, k] = temp;
                    }

                    var ptmp = perm[maxRow];
                    perm[maxRow] = perm[j];
                    perm[j] = ptmp;

                    toggle = -toggle;
                }

                if (IsZero(result[j, j]))
                    throw new Exception("Unable to decompose a non-decomposable matrix");

                for (int i = j + 1; i < n; ++i)
                {
                    result[i, j] /= result[j, j];
                    for (int k = j + 1; k < n; ++k)
                    {
                        result[i, k] -= result[i, j] * result[j, k];
                    }
                }
            }

            return result;
        }

        static double[] HelperSolve(Matrix<double> m, double[] b)
        {
            int n = m.Rows;
            var x = new double[b.Length];
            b.CopyTo(x, 0);

            for (int i = 1; i < n; ++i)
            {
                double sum = x[i];
                for (int j = 0; j < i; ++j)
                    sum -= m[i, j] * x[j];
                x[i] = sum;
            }

            x[n - 1] /= m[n - 1, n - 1];
            for (int i = n - 2; i >= 0; --i)
            {
                double sum = x[i];
                for (int j = i + 1; j < n; ++j)
                    sum -= m[i, j] * x[j];
                x[i] = sum / m[i, i];
            }

            return x;
        }

        #endregion

        #region IEnumerable<T> Members

        public override IEnumerator<IEnumerable<T>> GetEnumerator()
        {
            return base.GetEnumerator();
        }

        class RowEnumerator : IEnumerator<IEnumerable<T>>
        {
            SparseMatrix<T> matrix;
            int row = -1;
            IEnumerable<T> cur = null;

            public RowEnumerator(SparseMatrix<T> matrix)
            {
                this.matrix = matrix;
                lock (matrix.changeLock)
                {
                    matrix.ChangeToYale();
                }
            }

            #region IEnumerator<IEnumerable<T>> Members

            public IEnumerable<T> Current
            {
                get 
                {
                    if (row < 0 || row >= matrix.Rows)
                        throw new ArgumentOutOfRangeException("Matrix index out of bounds.");

                    return cur ?? (cur = new ColEnumerator(matrix, matrix.row_offsets[row], matrix.row_offsets[row + 1]).AsEnumerable());
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

                if (++row < matrix.Rows)
                    return true;
                else
                    return false;
            }

            public void Reset()
            {
                row = -1;
                cur = null;
            }

            #endregion
        }

        class ColEnumerator : IEnumerator<T>
        {
            SparseMatrix<T> matrix;
            int start, next, val_index, col;

            public ColEnumerator(SparseMatrix<T> matrix, int start, int next)
            {
                this.matrix = matrix;
                this.start = start;
                this.next = next;
                this.val_index = start-1;
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

                    if (col == matrix.col_offsets[val_index])
                        return matrix.values[val_index];
                    else
                        return default(T);
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
                if (++col < matrix.Cols)
                {
                    if (val_index < start || matrix.col_offsets[val_index] == col - 1)
                        val_index++;
                    return true;
                }
                return false;
            }

            public void Reset()
            {
                this.val_index = start - 1;
                this.col = -1;
            }

            #endregion
        }

        #endregion

        #region ISerializable Members

        public SparseMatrix(SerializationInfo info, StreamingContext context)
        {
            this.Rows = info.GetInt32("rows");
            this.Cols = info.GetInt32("cols");
            this.dok = (IDictionary<Tuple<int, int>, T>)info.GetValue("dok", typeof(IDictionary<Tuple<int, int>, T>));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            lock (changeLock)
            {
                ChangeToDok();

                info.AddValue("rows", this.Rows);
                info.AddValue("cols", this.Cols);
                info.AddValue("dok", this.dok);
            }
        }

        #endregion

        #region Utility Methods

        void ChangeToDok()
        {
            if (values != null)
            {
                if (dok == null)
                    dok = new Dictionary<Tuple<int, int>, T>();
                else
                    dok.Clear();

                for (int row = 0; row < row_offsets.Length - 1; row++)
                {
                    for (int index = row_offsets[row]; index < row_offsets[row + 1]; index++)
                    {
                        dok.Add(Tuple.Create(row, col_offsets[index]), values[row_offsets[row] + col_offsets[index]]);
                    }
                }

                values = null;
                row_offsets = null;
                col_offsets = null;
            }
        }

        void ChangeToYale()
        {
            if (dok != null)
            {
                values = new T[dok.Count];
                row_offsets = new int[Rows + 1];
                col_offsets = new int[dok.Count];

                int index = 0;
                int last_row = -1;
                foreach (var kv in dok)
                {
                    int row = kv.Key.Item1;
                    int col = kv.Key.Item2;

                    if (row > last_row)
                    {
                        row_offsets[row] = index;
                        last_row = row;
                    }
                    col_offsets[index] = col;
                    values[index] = kv.Value;

                    index++;
                }
                row_offsets[Rows] = dok.Count;

                dok = null;
            }
        }

        static void ScalarOp<Tt>(SparseMatrix<Tt> src, SparseMatrix<Tt> dest, Func<Tt, Tt> f)
            where Tt : struct, IComparable
        {
            if (src != dest)
            {
                if (src.values != null)
                {
                    dest.values = new Tt[src.values.Length];

                    dest.row_offsets = new int[src.row_offsets.Length];
                    Array.Copy(src.row_offsets, dest.row_offsets, src.row_offsets.Length);

                    dest.col_offsets = new int[src.col_offsets.Length];
                    Array.Copy(src.col_offsets, dest.col_offsets, src.col_offsets.Length);
                }
                else if (src.dok != null)
                {
                    dest.dok = new Dictionary<Tuple<int, int>, Tt>();
                }
            }

            if (src.values != null)
            {
                for (int i = 0; i < src.values.Length; ++i)
                    dest.values[i] = f(src.values[i]);
            }
            else if (src.dok != null)
            {
                foreach (var kv in src.dok)
                    dest.dok[kv.Key] = f(kv.Value);
            }
        }

        static void AdditiveOp<Tt>(SparseMatrix<Tt> a, IMatrix<Tt> b, SparseMatrix<T> result, Func<Tt, Tt, Tt> add)
            where Tt : struct, IComparable
        {
            var new_dok = new Dictionary<Tuple<int, int>, Tt>();

            int row = 0;
            var row_a = a.GetEnumerator();
            var row_b = b.GetEnumerator();

            while (row_a.MoveNext() && row_b.MoveNext())
            {
                int col = 0;
                var col_a = row_a.Current.GetEnumerator();
                var col_b = row_b.Current.GetEnumerator();

                while (col_a.MoveNext() && col_b.MoveNext())
                {
                    Tt sum = add(col_a.Current, col_b.Current);
                    if (!sum.Equals(default(Tt)))
                        new_dok[Tuple.Create(row, col)] = sum;

                    col++;
                }

                row++;
            }

            result.dok = (IDictionary<Tuple<int, int>, T>)new_dok;
            result.values = null;
            result.row_offsets = null;
            result.col_offsets = null;
        }

        static void Multiply<Tt>(SparseMatrix<Tt> a, IMatrix<Tt> b, SparseMatrix<T> result, Func<Tt, Tt, Tt> add, Func<Tt, Tt, Tt> mul, Func<Tt, bool> isZero)
            where Tt : struct, IComparable
        {
            // TODO: use a faster algo
            var new_dok = new Dictionary<Tuple<int, int>, Tt>();
            for (int i = 0; i < a.Rows; ++i)
            {
                for (int j = 0; j < b.Cols; ++j)
                {
                    Tt sum = default(Tt);
                    for (int k = 0; k < a.Cols; ++k)
                    {
                        sum = add(sum, mul(a[i, k], b[k, j]));
                    }
                    if (!isZero(sum))
                        new_dok[Tuple.Create(i, j)] = sum;
                }
            }

            result.dok = (IDictionary<Tuple<int, int>, T>)new_dok;
            result.values = null;
            result.row_offsets = null;
            result.col_offsets = null;
        }

        #endregion
    }
}
