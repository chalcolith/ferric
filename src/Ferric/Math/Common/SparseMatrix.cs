using System;
using System.Collections.Generic;
using System.Linq;
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
                if (values != null)
                    ChangeToDok();
                if (dok == null)
                    dok = new Dictionary<Tuple<int, int>, T>();
                dok[Tuple.Create(row, col)] = value;
            }
        }

        public override Matrix<T> Transpose()
        {
            throw new NotImplementedException();
        }

        public override Matrix<T> ScalarMultiply(T n, bool inPlace = false)
        {
            throw new NotImplementedException();
        }

        public override Matrix<T> Add(IMatrix<T> m, bool inPlace = false)
        {
            throw new NotImplementedException();
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

        #region ISerializable Members

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        #endregion

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
    }
}
