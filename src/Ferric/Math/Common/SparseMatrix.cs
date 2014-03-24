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
            set
            {
                if (!value.Equals(default(T)))
                {
                    if (values != null)
                        ChangeToDok();
                    if (dok == null)
                        dok = new Dictionary<Tuple<int, int>, T>();
                    dok[Tuple.Create(row, col)] = value;
                }
            }
        }

        public override Matrix<T> Transpose()
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

        public override Matrix<T> ScalarMultiply(T n, bool inPlace = false)
        {
            var result = inPlace ? this : new SparseMatrix<T>(this.Rows, this.Cols);
            if (n.Equals(default(T)))
                return result;

            if (typeof(T) == typeof(double))
            {
                double nd = Convert.ToDouble(n);
                ScalarMultiply<double>(this as SparseMatrix<double>, result as SparseMatrix<double>, (double a) => a * nd);
            }
            else if (typeof(T) == typeof(int))
            {
                int ni = Convert.ToInt32(n);
                ScalarMultiply<int>(this as SparseMatrix<int>, result as SparseMatrix<int>, (int a) => a * ni);
            }
            else
            {
                var mul = typeof(T).GetMethod("op_Multiply", BindingFlags.Static | BindingFlags.Public);
                if (mul == null)
                    throw new ArgumentException("Unable to find a multiplication operator for " + typeof(T).FullName);

                object[] parms = new object[2];
                parms[1] = n;

                ScalarMultiply<T>(this, result, (T a) =>
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
                Add<double>(this as SparseMatrix<double>, m as IMatrix<double>, result, (double a, double b) => a + b);
            }
            else if (typeof(T) == typeof(int))
            {
                Add<int>(this as SparseMatrix<int>, m as IMatrix<int>, result, (int a, int b) => a + b);
            }
            else
            {
                var add = typeof(T).GetMethod("op_Addition", BindingFlags.Static | BindingFlags.Public);
                if (add == null)
                    throw new ArgumentException("Unable to find an addition operator for " + typeof(T).FullName);

                object[] parms = new object[2];
                Add<T>(this, m, result, (T a, T b) =>
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
            throw new NotImplementedException();
        }

        public override Matrix<T> Subtract(IMatrix<T> m, bool inPlace = false)
        {
            throw new NotImplementedException();
        }

        public override Matrix<T> Multiply(IMatrix<T> m)
        {
            throw new NotImplementedException();
        }

        public override Matrix<T> Inverse()
        {
            throw new NotImplementedException();
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
                matrix.ChangeToYale();
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
            ChangeToDok();

            info.AddValue("rows", this.Rows);
            info.AddValue("cols", this.Cols);
            info.AddValue("dok", this.dok);
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

        static void ScalarMultiply<Tt>(SparseMatrix<Tt> src, SparseMatrix<Tt> dest, Func<Tt, Tt> mult)
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
                    dest.values[i] = mult(src.values[i]);
            }
            else if (src.dok != null)
            {
                foreach (var kv in src.dok)
                    dest.dok[kv.Key] = mult(kv.Value);
            }
        }

        static void Add<Tt>(SparseMatrix<Tt> a, IMatrix<Tt> b, SparseMatrix<T> result, Func<Tt, Tt, Tt> add)
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

        #endregion
    }
}
