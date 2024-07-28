// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlColumnBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DebuggerDisplay("Name: {m_columnName}, Ordinal: {m_column}")]
  public struct SqlColumnBinder
  {
    private int m_column;
    private readonly string m_columnName;
    private const int c_columnUnknown = -2;
    private const int c_columnNotFound = -1;

    public SqlColumnBinder(string columnName)
    {
      this.m_columnName = columnName;
      this.m_column = -2;
    }

    public string ColumnName => this.m_columnName;

    public string GetString(IDataReader reader, bool allowNulls)
    {
      this.InitColumn(reader);
      return allowNulls && reader.IsDBNull(this.m_column) ? (string) null : reader.GetString(this.m_column);
    }

    public string GetString(IDataReader reader, string missingColumnValue)
    {
      this.InitColumn(reader, true);
      if (this.m_column == -1)
        return missingColumnValue;
      return reader.IsDBNull(this.m_column) ? (string) null : reader.GetString(this.m_column);
    }

    public string GetString(IDataReader reader, bool allowNulls, string missingColumnValue)
    {
      this.InitColumn(reader, true);
      if (this.m_column == -1)
        return missingColumnValue;
      return allowNulls && reader.IsDBNull(this.m_column) ? (string) null : reader.GetString(this.m_column);
    }

    public char GetChar(IDataReader reader)
    {
      this.InitColumn(reader);
      char[] buffer = new char[1];
      reader.GetChars(this.m_column, 0L, buffer, 0, 1);
      return buffer[0];
    }

    public short GetInt16(IDataReader reader)
    {
      this.InitColumn(reader);
      return reader.GetInt16(this.m_column);
    }

    public short GetInt16(IDataReader reader, short nullValue)
    {
      this.InitColumn(reader);
      return reader.IsDBNull(this.m_column) ? nullValue : reader.GetInt16(this.m_column);
    }

    public short GetInt16(IDataReader reader, short nullValue, short missingColumnValue)
    {
      this.InitColumn(reader, true);
      if (this.m_column == -1)
        return missingColumnValue;
      return reader.IsDBNull(this.m_column) ? nullValue : reader.GetInt16(this.m_column);
    }

    public short? GetNullableInt16(IDataReader reader) => !this.IsNull(reader) ? new short?(reader.GetInt16(this.m_column)) : new short?();

    public int GetInt32(IDataReader reader)
    {
      this.InitColumn(reader);
      return reader.GetInt32(this.m_column);
    }

    public int GetInt32(IDataReader reader, int nullValue)
    {
      this.InitColumn(reader);
      return reader.IsDBNull(this.m_column) ? nullValue : reader.GetInt32(this.m_column);
    }

    public int GetInt32(IDataReader reader, int nullValue, int missingColumnValue)
    {
      this.InitColumn(reader, true);
      if (this.m_column == -1)
        return missingColumnValue;
      return reader.IsDBNull(this.m_column) ? nullValue : reader.GetInt32(this.m_column);
    }

    public int? GetNullableInt32(IDataReader reader) => !this.IsNull(reader) ? new int?(this.GetInt32(reader)) : new int?();

    public long GetInt64(IDataReader reader)
    {
      this.InitColumn(reader);
      return reader.GetInt64(this.m_column);
    }

    public long GetInt64(IDataReader reader, long nullValue)
    {
      this.InitColumn(reader);
      return reader.IsDBNull(this.m_column) ? nullValue : reader.GetInt64(this.m_column);
    }

    public long GetInt64(IDataReader reader, long nullValue, long missingColumnValue)
    {
      this.InitColumn(reader, true);
      if (this.m_column == -1)
        return missingColumnValue;
      return reader.IsDBNull(this.m_column) ? nullValue : reader.GetInt64(this.m_column);
    }

    public long? GetNullableInt64(IDataReader reader) => !this.IsNull(reader) ? new long?(this.GetInt64(reader)) : new long?();

    public byte GetByte(IDataReader reader)
    {
      this.InitColumn(reader);
      return reader.GetByte(this.m_column);
    }

    public byte GetByte(IDataReader reader, byte nullValue)
    {
      this.InitColumn(reader);
      return reader.IsDBNull(this.m_column) ? nullValue : reader.GetByte(this.m_column);
    }

    public byte GetByte(IDataReader reader, byte nullValue, byte missingColumnValue)
    {
      this.InitColumn(reader, true);
      if (this.m_column == -1)
        return missingColumnValue;
      return reader.IsDBNull(this.m_column) ? nullValue : reader.GetByte(this.m_column);
    }

    public byte? GetNullableByte(IDataReader reader) => !this.IsNull(reader) ? new byte?(reader.GetByte(this.m_column)) : new byte?();

    public double GetDouble(IDataReader reader)
    {
      this.InitColumn(reader);
      return reader.GetDouble(this.m_column);
    }

    public double GetDouble(IDataReader reader, double nullValue)
    {
      this.InitColumn(reader);
      return reader.IsDBNull(this.m_column) ? nullValue : reader.GetDouble(this.m_column);
    }

    public double? GetNullableDouble(IDataReader reader) => !this.IsNull(reader) ? new double?(reader.GetDouble(this.m_column)) : new double?();

    public float GetFloat(IDataReader reader)
    {
      this.InitColumn(reader);
      return reader.GetFloat(this.m_column);
    }

    public float GetFloat(IDataReader reader, float nullValue)
    {
      this.InitColumn(reader);
      return reader.IsDBNull(this.m_column) ? nullValue : reader.GetFloat(this.m_column);
    }

    public float GetFloat(IDataReader reader, float nullValue, float missingColumnValue)
    {
      this.InitColumn(reader, true);
      if (this.m_column == -1)
        return missingColumnValue;
      return reader.IsDBNull(this.m_column) ? nullValue : reader.GetFloat(this.m_column);
    }

    public bool GetBoolean(IDataReader reader)
    {
      this.InitColumn(reader);
      return reader.GetBoolean(this.m_column);
    }

    public bool GetBoolean(IDataReader reader, bool nullValue)
    {
      this.InitColumn(reader);
      return !reader.IsDBNull(this.m_column) ? reader.GetBoolean(this.m_column) : nullValue;
    }

    public bool GetBoolean(IDataReader reader, bool nullValue, bool missingColumnValue)
    {
      this.InitColumn(reader, true);
      if (this.m_column == -1)
        return missingColumnValue;
      return !reader.IsDBNull(this.m_column) ? reader.GetBoolean(this.m_column) : nullValue;
    }

    public bool GetBoolean(IDataReader reader, bool nullValue, out bool isNull)
    {
      this.InitColumn(reader);
      isNull = reader.IsDBNull(this.m_column);
      return !isNull ? reader.GetBoolean(this.m_column) : nullValue;
    }

    public bool? GetNullableBoolean(IDataReader reader)
    {
      this.InitColumn(reader);
      return !reader.IsDBNull(this.m_column) ? new bool?(reader.GetBoolean(this.m_column)) : new bool?();
    }

    public byte[] GetBytes(IDataReader reader, bool allowNulls)
    {
      this.InitColumn(reader);
      if (reader.IsDBNull(this.m_column))
        return Array.Empty<byte>();
      if (reader is SqlDataReader)
        return ((SqlDataReader) reader).GetSqlBinary(this.m_column).Value;
      int length = 1048576;
      byte[] array = new byte[length];
      int bytes = (int) reader.GetBytes(this.m_column, 0L, array, 0, length);
      Array.Resize<byte>(ref array, bytes);
      return array;
    }

    public byte[] GetBytes(IDataReader reader, byte[] missingColumnValue)
    {
      this.InitColumn(reader, true);
      if (this.m_column == -1)
        return missingColumnValue;
      if (reader.IsDBNull(this.m_column))
        return Array.Empty<byte>();
      if (reader is SqlDataReader)
        return ((SqlDataReader) reader).GetSqlBinary(this.m_column).Value;
      int length = 1048576;
      byte[] array = new byte[length];
      int bytes = (int) reader.GetBytes(this.m_column, 0L, array, 0, length);
      Array.Resize<byte>(ref array, bytes);
      return array;
    }

    public int GetBytes(
      IDataReader reader,
      long dataOffset,
      byte[] buffer,
      int bufferIndex,
      int length)
    {
      this.InitColumn(reader);
      return (int) reader.GetBytes(this.m_column, dataOffset, buffer, bufferIndex, length);
    }

    public Guid GetGuid(IDataReader reader) => this.GetGuid(reader, false);

    public Guid GetGuid(IDataReader reader, bool allowNulls)
    {
      this.InitColumn(reader);
      return allowNulls && reader.IsDBNull(this.m_column) ? Guid.Empty : reader.GetGuid(this.m_column);
    }

    public Guid GetGuid(IDataReader reader, bool allowNulls, Guid missingColumnValue)
    {
      this.InitColumn(reader, true);
      if (this.m_column == -1)
        return missingColumnValue;
      return allowNulls && reader.IsDBNull(this.m_column) ? Guid.Empty : reader.GetGuid(this.m_column);
    }

    public Guid? GetNullableGuid(IDataReader reader)
    {
      this.InitColumn(reader);
      return reader.IsDBNull(this.m_column) ? new Guid?() : new Guid?(reader.GetGuid(this.m_column));
    }

    public Guid? GetNullableGuid(IDataReader reader, Guid? missingColumnValue)
    {
      this.InitColumn(reader, true);
      if (this.m_column == -1)
        return missingColumnValue;
      return reader.IsDBNull(this.m_column) ? new Guid?() : new Guid?(reader.GetGuid(this.m_column));
    }

    public DateTime GetDateTime(IDataReader reader)
    {
      this.InitColumn(reader);
      if (reader.IsDBNull(this.m_column))
        return DateTime.MinValue;
      DateTime dateTime = reader.GetDateTime(this.m_column);
      dateTime = new DateTime(dateTime.Ticks, DateTimeKind.Utc);
      return dateTime;
    }

    public DateTime? GetNullableDateTime(IDataReader reader)
    {
      this.InitColumn(reader);
      if (reader.IsDBNull(this.m_column))
        return new DateTime?();
      DateTime dateTime = reader.GetDateTime(this.m_column);
      dateTime = new DateTime(dateTime.Ticks, DateTimeKind.Utc);
      return new DateTime?(dateTime);
    }

    public DateTimeOffset GetDateTimeOffset(SqlDataReader reader)
    {
      this.InitColumn((IDataReader) reader);
      return reader.IsDBNull(this.m_column) ? DateTimeOffset.MinValue : reader.GetDateTimeOffset(this.m_column);
    }

    public DateTimeOffset? GetNullableDateTimeOffset(SqlDataReader reader)
    {
      this.InitColumn((IDataReader) reader);
      return reader.IsDBNull(this.m_column) ? new DateTimeOffset?() : new DateTimeOffset?(reader.GetDateTimeOffset(this.m_column));
    }

    public DateTimeOffset? GetNullableDateTimeOffset(
      SqlDataReader reader,
      DateTimeOffset? missingColumnValue)
    {
      this.InitColumn((IDataReader) reader, true);
      if (this.m_column == -1)
        return missingColumnValue;
      return reader.IsDBNull(this.m_column) ? new DateTimeOffset?() : new DateTimeOffset?(reader.GetDateTimeOffset(this.m_column));
    }

    public DateTime GetDateTime(IDataReader reader, DateTime missingColumnValue)
    {
      this.InitColumn(reader, true);
      if (this.m_column == -1)
        return missingColumnValue;
      if (reader.IsDBNull(this.m_column))
        return DateTime.MinValue;
      DateTime dateTime = reader.GetDateTime(this.m_column);
      dateTime = new DateTime(dateTime.Ticks, DateTimeKind.Utc);
      return dateTime;
    }

    public DateTime? GetNullableDateTime(IDataReader reader, DateTime? missingColumnValue)
    {
      this.InitColumn(reader, true);
      if (this.m_column == -1)
        return missingColumnValue;
      if (reader.IsDBNull(this.m_column))
        return new DateTime?();
      DateTime dateTime = reader.GetDateTime(this.m_column);
      dateTime = new DateTime(dateTime.Ticks, DateTimeKind.Utc);
      return new DateTime?(dateTime);
    }

    public TimeSpan GetTimeSpan(SqlDataReader reader)
    {
      this.InitColumn((IDataReader) reader);
      return reader.GetTimeSpan(this.m_column);
    }

    public TimeSpan GetTimeSpan(SqlDataReader reader, TimeSpan nullValue)
    {
      this.InitColumn((IDataReader) reader);
      return reader.IsDBNull(this.m_column) ? nullValue : reader.GetTimeSpan(this.m_column);
    }

    public TimeSpan GetTimeSpan(
      SqlDataReader reader,
      TimeSpan nullValue,
      TimeSpan missingColumnValue)
    {
      this.InitColumn((IDataReader) reader);
      if (this.m_column == -1)
        return missingColumnValue;
      return reader.IsDBNull(this.m_column) ? nullValue : reader.GetTimeSpan(this.m_column);
    }

    public object GetObject(IDataReader reader)
    {
      this.InitColumn(reader);
      return reader.IsDBNull(this.m_column) ? (object) null : reader.GetValue(this.m_column);
    }

    public ulong GetRowVersion(IDataReader reader)
    {
      byte[] bytes = this.GetBytes(reader, false);
      return (ulong) ((long) bytes[0] << 56 | (long) bytes[1] << 48 | (long) bytes[2] << 40 | (long) bytes[3] << 32 | (long) bytes[4] << 24 | (long) bytes[5] << 16 | (long) bytes[6] << 8) | (ulong) bytes[7];
    }

    public bool IsInitialized() => this.m_column != -2;

    public bool IsNull(IDataReader reader)
    {
      this.InitColumn(reader);
      return reader.IsDBNull(this.m_column);
    }

    public int GetOrdinal(IDataReader reader)
    {
      if (this.m_column == -2)
        this.InitColumn(reader);
      return this.m_column;
    }

    public bool ColumnExists(IDataReader reader)
    {
      if (this.m_column == -2)
        this.InitColumn(reader, true);
      return this.m_column >= 0;
    }

    private void InitColumn(IDataReader reader, bool useFindOrdinal = false)
    {
      if (this.IsInitialized())
        return;
      if (useFindOrdinal)
        this.m_column = SqlColumnBinder.FindOrdinal(reader, this.m_columnName);
      else
        this.m_column = reader.GetOrdinal(this.m_columnName);
    }

    private static int FindOrdinal(IDataReader reader, string columnName)
    {
      int i;
      for (i = reader.FieldCount - 1; i >= 0; --i)
      {
        string name = reader.GetName(i);
        if (columnName.Equals(name, StringComparison.CurrentCultureIgnoreCase))
          break;
      }
      return i;
    }
  }
}
