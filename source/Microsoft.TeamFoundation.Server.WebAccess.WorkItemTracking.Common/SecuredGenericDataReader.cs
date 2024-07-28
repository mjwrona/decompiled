// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.SecuredGenericDataReader
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class SecuredGenericDataReader : 
    IDataReader,
    IDisposable,
    IDataRecord,
    IEnumerable<SecuredGenericData>,
    IEnumerable
  {
    private bool m_open = true;
    private KeyValuePair<string, Type>[] m_columns;
    private Dictionary<string, int> m_nameToOrdinalMap;
    private IEnumerator<SecuredGenericData> m_enumerator;
    private object[] m_currentRow;

    public SecuredGenericDataReader(
      IEnumerable<string> columns,
      IEnumerable<SecuredGenericData> data)
      : this(SecuredGenericDataReader.CreateColumnInfo(columns), data)
    {
    }

    public SecuredGenericDataReader(
      IEnumerable<KeyValuePair<string, Type>> columns,
      IEnumerable<SecuredGenericData> data)
    {
      this.m_columns = columns != null ? columns.ToArray<KeyValuePair<string, Type>>() : throw new ArgumentNullException(nameof (columns));
      this.m_nameToOrdinalMap = new Dictionary<string, int>(this.m_columns.Length, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      for (int index = 0; index < this.m_columns.Length; ++index)
        this.m_nameToOrdinalMap[this.m_columns[index].Key] = index;
      if (data != null)
        this.m_enumerator = data.GetEnumerator();
      else
        this.m_enumerator = Enumerable.Empty<SecuredGenericData>().GetEnumerator();
    }

    private static IEnumerable<KeyValuePair<string, Type>> CreateColumnInfo(
      IEnumerable<string> columns)
    {
      return columns == null ? (IEnumerable<KeyValuePair<string, Type>>) null : columns.Select<string, KeyValuePair<string, Type>>((System.Func<string, KeyValuePair<string, Type>>) (c => new KeyValuePair<string, Type>(c, typeof (object))));
    }

    public void Close() => this.m_open = false;

    public int Depth => 0;

    public DataTable GetSchemaTable() => throw new NotSupportedException();

    public bool IsClosed => !this.m_open;

    public bool NextResult() => false;

    public bool Read()
    {
      if (this.m_enumerator.MoveNext())
      {
        this.m_currentRow = this.m_enumerator.Current.Data;
        return true;
      }
      this.m_currentRow = (object[]) null;
      return false;
    }

    public int RecordsAffected => -1;

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.Close();
    }

    public int FieldCount => this.m_columns.Length;

    public bool GetBoolean(int i) => (bool) this.m_currentRow[i];

    public byte GetByte(int i) => (byte) this.m_currentRow[i];

    public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
    {
      byte[] sourceArray = (byte[]) this.m_currentRow[i];
      long length1 = Math.Min((long) sourceArray.Length - fieldOffset, (long) length);
      Array.Copy((Array) sourceArray, fieldOffset, (Array) buffer, (long) bufferoffset, length1);
      return length1;
    }

    public char GetChar(int i) => (char) this.m_currentRow[i];

    public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
    {
      char[] sourceArray = (char[]) this.m_currentRow[i];
      long length1 = Math.Min((long) sourceArray.Length - fieldoffset, (long) length);
      Array.Copy((Array) sourceArray, fieldoffset, (Array) buffer, (long) bufferoffset, length1);
      return length1;
    }

    public DateTime GetDateTime(int i) => (DateTime) this.m_currentRow[i];

    public Decimal GetDecimal(int i) => (Decimal) this.m_currentRow[i];

    public double GetDouble(int i) => (double) this.m_currentRow[i];

    public float GetFloat(int i) => (float) this.m_currentRow[i];

    public Guid GetGuid(int i) => (Guid) this.m_currentRow[i];

    public short GetInt16(int i) => (short) this.m_currentRow[i];

    public int GetInt32(int i) => (int) this.m_currentRow[i];

    public long GetInt64(int i) => (long) this.m_currentRow[i];

    public string GetString(int i) => (string) this.m_currentRow[i];

    public object GetValue(int i) => this.m_currentRow[i];

    public IDataReader GetData(int i) => throw new NotSupportedException("GetData not supported.");

    public string GetDataTypeName(int i) => this.m_columns[i].Value.Name;

    public Type GetFieldType(int i) => this.m_columns[i].Value;

    public string GetName(int i) => this.m_columns[i].Key;

    public int GetOrdinal(string name)
    {
      int ordinal;
      if (this.m_nameToOrdinalMap.TryGetValue(name, out ordinal))
        return ordinal;
      throw new IndexOutOfRangeException();
    }

    public int GetValues(object[] values)
    {
      int length = Math.Min(values.Length, this.m_columns.Length);
      Array.Copy((Array) this.m_currentRow, (Array) values, length);
      return length;
    }

    public bool IsDBNull(int i) => this.m_currentRow[i] == DBNull.Value;

    public object this[string name] => this.m_currentRow[this.GetOrdinal(name)];

    public object this[int i] => this.m_currentRow[i];

    public IEnumerator<SecuredGenericData> GetEnumerator()
    {
      IEnumerator enumerator = (IEnumerator) this.m_enumerator;
      while (enumerator.MoveNext())
        yield return (SecuredGenericData) enumerator.Current;
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
  }
}
