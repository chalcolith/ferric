using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ferric.Text.WordNet.Builder
{
    class RelationReader<TSrc, TDest> : IDataReader
    {
        DataTable schemaTable;
        bool closed;

        IEnumerable<TSrc> source;
        string tableName;

        PropertyInfo srcProperty;
        PropertyInfo srcIdProperty;
        PropertyInfo destIdProperty;

        IEnumerator<Tuple<int, int>> results;

        int numRead = 0;

        public RelationReader(IEnumerable<TSrc> source, string srcPropertyName, string tableName)
        {
            this.source = source;
            this.tableName = tableName;

            this.srcProperty = typeof(TSrc).GetProperty(srcPropertyName);
            this.srcIdProperty = typeof(TSrc).GetProperty(typeof(TSrc).Name + "Id");
            this.destIdProperty = typeof(TDest).GetProperty(typeof(TDest).Name + "Id");
        }

        private IEnumerable<Tuple<int, int>> GetResults()
        {
            var sourceEnum = source.GetEnumerator();
            while (sourceEnum.MoveNext())
            {
                int sourceId = (int)srcIdProperty.GetValue(sourceEnum.Current);

                var dest = srcProperty.GetValue(sourceEnum.Current) as IEnumerable<TDest>;
                if (dest == null)
                    continue;
                
                var destEnum = dest.GetEnumerator();
                while (destEnum.MoveNext())
                {
                    int destId = (int)destIdProperty.GetValue(destEnum.Current);
                    yield return Tuple.Create(sourceId, destId);
                }
            }
        }

        #region IDataReader Members

        public void Close()
        {
            closed = true;
        }

        public int Depth
        {
            get { return 0; }
        }

        public DataTable GetSchemaTable()
        {
            if (schemaTable == null)
            {
                schemaTable = new DataTable(tableName);

                var firstName = typeof(TSrc).Name + "Id";
                var secondName = typeof(TDest).Name + "Id";

                if (typeof(TSrc) == typeof(TDest))
                {
                    firstName = "First" + firstName;
                    secondName = "Second" + secondName;
                }

                schemaTable.Columns.Add(firstName, typeof(int));
                schemaTable.Columns.Add(secondName, typeof(int));
            }
            return schemaTable;
        }

        public bool IsClosed
        {
            get { return closed; }
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        public bool Read()
        {
            if (closed)
                return false;

            if (results == null)
                results = GetResults().GetEnumerator();

            if (results.MoveNext())
            {
                numRead++;
                return true;
            }
            else
            {
                closed = true;
                return false;
            }
        }

        public int RecordsAffected
        {
            get { return numRead; }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDataRecord Members

        public int FieldCount
        {
            get { return 2; }
        }

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            return typeof(int).FullName;
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int i)
        {
            return (int)GetValue(i);
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public string GetName(int i)
        {
            return GetSchemaTable().Columns[i].ColumnName;
        }

        public int GetOrdinal(string name)
        {
            for (int i = 0; i < GetSchemaTable().Columns.Count; i++)
            {
                if (GetSchemaTable().Columns[i].ColumnName == name)
                    return i;
            }
            return -1;
        }

        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        public object GetValue(int i)
        {
            if (closed)
                throw new Exception("reader is closed");
            if (i >= 2)
                throw new IndexOutOfRangeException("only 2 columns");

            return i == 0 ? results.Current.Item1 : results.Current.Item2;
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            return false;
        }

        public object this[string name]
        {
            get { return GetValue(GetOrdinal(name)); }
        }

        public object this[int i]
        {
            get { return GetValue(i); }
        }

        #endregion
    }
}
