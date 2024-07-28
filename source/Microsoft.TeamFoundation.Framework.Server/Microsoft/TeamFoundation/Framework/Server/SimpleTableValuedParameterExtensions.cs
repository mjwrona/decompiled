// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SimpleTableValuedParameterExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class SimpleTableValuedParameterExtensions
  {
    private static readonly SqlMetaData[] typ_BooleanTable = new SqlMetaData[1]
    {
      new SqlMetaData("Flag", SqlDbType.Bit)
    };
    private static readonly SqlMetaData[] typ_DateTimeTable = new SqlMetaData[1]
    {
      new SqlMetaData("Val", SqlDbType.DateTime)
    };
    private static readonly SqlMetaData[] typ_DoubleTable = new SqlMetaData[1]
    {
      new SqlMetaData("Val", SqlDbType.Float)
    };
    private static readonly SqlMetaData[] typ_GuidTinyIntTable = new SqlMetaData[2]
    {
      new SqlMetaData("a", SqlDbType.UniqueIdentifier),
      new SqlMetaData("b", SqlDbType.TinyInt)
    };
    private static readonly SqlMetaData[] typ_GuidInt32Table = new SqlMetaData[2]
    {
      new SqlMetaData("a", SqlDbType.UniqueIdentifier),
      new SqlMetaData("b", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_GuidStringTable = new SqlMetaData[2]
    {
      new SqlMetaData("a", SqlDbType.UniqueIdentifier),
      new SqlMetaData("b", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] typ_GuidStringVarcharTable = new SqlMetaData[2]
    {
      new SqlMetaData("a", SqlDbType.UniqueIdentifier),
      new SqlMetaData("b", SqlDbType.VarChar, -1L)
    };
    private static readonly SqlMetaData[] typ_GuidTable = new SqlMetaData[1]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier)
    };
    private static readonly SqlMetaData[] typ_GuidGuidTable = new SqlMetaData[2]
    {
      new SqlMetaData("a", SqlDbType.UniqueIdentifier),
      new SqlMetaData("b", SqlDbType.UniqueIdentifier)
    };
    private static readonly SqlMetaData[] typ_KeyValuePairInt32StringTable = new SqlMetaData[2]
    {
      new SqlMetaData("Key", SqlDbType.Int),
      new SqlMetaData("Value", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] typ_KeyValuePairInt32StringVarcharTable = new SqlMetaData[2]
    {
      new SqlMetaData("Key", SqlDbType.Int),
      new SqlMetaData("Value", SqlDbType.VarChar, -1L)
    };
    private static readonly SqlMetaData[] typ_TinyIntTable = new SqlMetaData[1]
    {
      new SqlMetaData("Val", SqlDbType.TinyInt)
    };
    private static readonly SqlMetaData[] typ_Int32Table = new SqlMetaData[1]
    {
      new SqlMetaData("Val", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_UniqueInt32Table = new SqlMetaData[1]
    {
      new SqlMetaData("Val", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_Int64Table = new SqlMetaData[1]
    {
      new SqlMetaData("Val", SqlDbType.BigInt)
    };
    private static readonly SqlMetaData[] typ_Int64WithIndexTable = new SqlMetaData[2]
    {
      new SqlMetaData("RowId", SqlDbType.Int, true, false, System.Data.SqlClient.SortOrder.Unspecified, -1),
      new SqlMetaData("Val", SqlDbType.BigInt)
    };
    private static readonly SqlMetaData[] typ_KeyValuePairGuidGuidTable = new SqlMetaData[2]
    {
      new SqlMetaData("Key", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Value", SqlDbType.UniqueIdentifier)
    };
    private static readonly SqlMetaData[] typ_KeyValuePairGuidStringTable = new SqlMetaData[2]
    {
      new SqlMetaData("Key", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Value", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] typ_KeyValuePairInt32DateTimeTable = new SqlMetaData[2]
    {
      new SqlMetaData("Key", SqlDbType.Int),
      new SqlMetaData("Value", SqlDbType.DateTime)
    };
    private static readonly SqlMetaData[] typ_KeyValuePairInt32Int32Table = new SqlMetaData[2]
    {
      new SqlMetaData("Key", SqlDbType.Int),
      new SqlMetaData("Value", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_KeyValuePairInt64Int32Table = new SqlMetaData[2]
    {
      new SqlMetaData("Key", SqlDbType.BigInt),
      new SqlMetaData("Value", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_KeyValuePairStringInt32Table = new SqlMetaData[2]
    {
      new SqlMetaData("Key", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Value", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_KeyValuePairStringTable = new SqlMetaData[2]
    {
      new SqlMetaData("Key", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Value", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] typ_KeyValuePairStringTableNullable = new SqlMetaData[2]
    {
      new SqlMetaData("Key", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Value", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] typ_SortedGuidTable = new SqlMetaData[1]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier)
    };
    private static readonly SqlMetaData[] typ_StringTable = new SqlMetaData[1]
    {
      new SqlMetaData("Data", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] typ_StringVarcharTable = new SqlMetaData[1]
    {
      new SqlMetaData("Data", SqlDbType.VarChar, -1L)
    };

    public static SqlParameter BindBooleanTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<bool> rows)
    {
      rows = rows ?? Enumerable.Empty<bool>();
      System.Func<bool, SqlDataRecord> selector = (System.Func<bool, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SimpleTableValuedParameterExtensions.typ_BooleanTable);
        sqlDataRecord.SetBoolean(0, row);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "typ_BooleanTable", rows.Select<bool, SqlDataRecord>(selector));
    }

    public static SqlParameter BindDateTimeTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<DateTime> rows)
    {
      rows = rows ?? Enumerable.Empty<DateTime>();
      System.Func<DateTime, SqlDataRecord> selector = (System.Func<DateTime, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SimpleTableValuedParameterExtensions.typ_DateTimeTable);
        sqlDataRecord.SetDateTime(0, row);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "typ_DateTimeTable", rows.Select<DateTime, SqlDataRecord>(selector));
    }

    public static SqlParameter BindDoubleTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<double> rows)
    {
      rows = rows ?? Enumerable.Empty<double>();
      System.Func<double, SqlDataRecord> selector = (System.Func<double, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SimpleTableValuedParameterExtensions.typ_DoubleTable);
        sqlDataRecord.SetDouble(0, row);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "typ_DoubleTable", rows.Select<double, SqlDataRecord>(selector));
    }

    public static SqlParameter BindGuidTinyIntTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Tuple<Guid, byte>> rows)
    {
      rows = rows ?? Enumerable.Empty<Tuple<Guid, byte>>();
      System.Func<Tuple<Guid, byte>, SqlDataRecord> selector = (System.Func<Tuple<Guid, byte>, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SimpleTableValuedParameterExtensions.typ_GuidTinyIntTable);
        sqlDataRecord.SetGuid(0, row.Item1);
        sqlDataRecord.SetByte(1, row.Item2);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "typ_GuidTinyIntTable", rows.Select<Tuple<Guid, byte>, SqlDataRecord>(selector));
    }

    public static SqlParameter BindGuidInt32Table(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Tuple<Guid, int>> rows)
    {
      rows = rows ?? Enumerable.Empty<Tuple<Guid, int>>();
      System.Func<Tuple<Guid, int>, SqlDataRecord> selector = (System.Func<Tuple<Guid, int>, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SimpleTableValuedParameterExtensions.typ_GuidInt32Table);
        sqlDataRecord.SetGuid(0, row.Item1);
        sqlDataRecord.SetInt32(1, row.Item2);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "typ_GuidInt32Table", rows.Select<Tuple<Guid, int>, SqlDataRecord>(selector));
    }

    public static SqlParameter BindGuidStringTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Tuple<Guid, string>> rows,
      bool nvarchar = true)
    {
      rows = rows ?? Enumerable.Empty<Tuple<Guid, string>>();
      System.Func<Tuple<Guid, string>, SqlDataRecord> selector = (System.Func<Tuple<Guid, string>, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(nvarchar ? SimpleTableValuedParameterExtensions.typ_GuidStringTable : SimpleTableValuedParameterExtensions.typ_GuidStringVarcharTable);
        sqlDataRecord.SetGuid(0, row.Item1);
        sqlDataRecord.SetString(1, row.Item2);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, nvarchar ? "typ_GuidStringTable" : "typ_GuidStringVarcharTable", rows.Select<Tuple<Guid, string>, SqlDataRecord>(selector));
    }

    public static SqlParameter BindGuidTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Guid> rows)
    {
      rows = rows ?? Enumerable.Empty<Guid>();
      System.Func<Guid, SqlDataRecord> selector = (System.Func<Guid, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SimpleTableValuedParameterExtensions.typ_GuidTable);
        sqlDataRecord.SetGuid(0, row);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "typ_GuidTable", rows.Select<Guid, SqlDataRecord>(selector));
    }

    public static SqlParameter BindGuidGuidTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Tuple<Guid, Guid>> rows)
    {
      rows = rows ?? Enumerable.Empty<Tuple<Guid, Guid>>();
      System.Func<Tuple<Guid, Guid>, SqlDataRecord> selector = (System.Func<Tuple<Guid, Guid>, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SimpleTableValuedParameterExtensions.typ_GuidGuidTable);
        sqlDataRecord.SetGuid(0, row.Item1);
        sqlDataRecord.SetGuid(1, row.Item2);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "typ_GuidGuidTable", rows.Select<Tuple<Guid, Guid>, SqlDataRecord>(selector));
    }

    public static SqlParameter BindGuidGuidTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<(Guid, Guid)> rows)
    {
      return component.BindGuidGuidTable(parameterName, rows.Select<(Guid, Guid), Tuple<Guid, Guid>>((System.Func<(Guid, Guid), Tuple<Guid, Guid>>) (r => r.ToTuple<Guid, Guid>())));
    }

    public static SqlParameter BindInt32StringTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Tuple<int, string>> rows,
      bool nvarchar = true)
    {
      rows = rows ?? Enumerable.Empty<Tuple<int, string>>();
      System.Func<Tuple<int, string>, SqlDataRecord> selector = (System.Func<Tuple<int, string>, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(nvarchar ? SimpleTableValuedParameterExtensions.typ_KeyValuePairInt32StringTable : SimpleTableValuedParameterExtensions.typ_KeyValuePairInt32StringVarcharTable);
        sqlDataRecord.SetInt32(0, row.Item1);
        sqlDataRecord.SetString(1, row.Item2);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, nvarchar ? "typ_KeyValuePairInt32StringTable" : "typ_KeyValuePairInt32StringVarcharTable", rows.Select<Tuple<int, string>, SqlDataRecord>(selector));
    }

    public static SqlParameter BindTinyIntTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<byte> rows)
    {
      rows = rows ?? Enumerable.Empty<byte>();
      System.Func<byte, SqlDataRecord> selector = (System.Func<byte, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SimpleTableValuedParameterExtensions.typ_TinyIntTable);
        sqlDataRecord.SetByte(0, row);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "typ_TinyIntTable", rows.Select<byte, SqlDataRecord>(selector));
    }

    public static SqlParameter BindInt32Table(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<int> rows)
    {
      rows = rows ?? Enumerable.Empty<int>();
      System.Func<int, SqlDataRecord> selector = (System.Func<int, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SimpleTableValuedParameterExtensions.typ_Int32Table);
        sqlDataRecord.SetInt32(0, row);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "typ_Int32Table", rows.Select<int, SqlDataRecord>(selector));
    }

    public static SqlParameter BindUniqueInt32Table(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<int> rows)
    {
      rows = rows ?? Enumerable.Empty<int>();
      System.Func<int, SqlDataRecord> selector = (System.Func<int, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SimpleTableValuedParameterExtensions.typ_UniqueInt32Table);
        sqlDataRecord.SetInt32(0, row);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "typ_UniqueInt32Table", rows.Select<int, SqlDataRecord>(selector));
    }

    public static SqlParameter BindInt64Table(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<long> rows)
    {
      rows = rows ?? Enumerable.Empty<long>();
      System.Func<long, SqlDataRecord> selector = (System.Func<long, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SimpleTableValuedParameterExtensions.typ_Int64Table);
        sqlDataRecord.SetInt64(0, row);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "typ_Int64Table", rows.Select<long, SqlDataRecord>(selector));
    }

    public static SqlParameter BindInt64WithIndexTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<long> rows)
    {
      rows = rows ?? Enumerable.Empty<long>();
      System.Func<long, SqlDataRecord> selector = (System.Func<long, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SimpleTableValuedParameterExtensions.typ_Int64WithIndexTable);
        sqlDataRecord.SetInt64(1, row);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "typ_Int64WithIndexTable", rows.Select<long, SqlDataRecord>(selector));
    }

    public static SqlParameter BindKeyValuePairGuidGuidTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<KeyValuePair<Guid, Guid>> rows)
    {
      rows = rows ?? Enumerable.Empty<KeyValuePair<Guid, Guid>>();
      System.Func<KeyValuePair<Guid, Guid>, SqlDataRecord> selector = (System.Func<KeyValuePair<Guid, Guid>, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SimpleTableValuedParameterExtensions.typ_KeyValuePairGuidGuidTable);
        sqlDataRecord.SetGuid(0, row.Key);
        sqlDataRecord.SetGuid(1, row.Value);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "typ_KeyValuePairGuidGuidTable", rows.Select<KeyValuePair<Guid, Guid>, SqlDataRecord>(selector));
    }

    public static SqlParameter BindKeyValuePairGuidStringTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<KeyValuePair<Guid, string>> rows)
    {
      rows = rows ?? Enumerable.Empty<KeyValuePair<Guid, string>>();
      System.Func<KeyValuePair<Guid, string>, SqlDataRecord> selector = (System.Func<KeyValuePair<Guid, string>, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SimpleTableValuedParameterExtensions.typ_KeyValuePairGuidStringTable);
        sqlDataRecord.SetGuid(0, row.Key);
        sqlDataRecord.SetString(1, row.Value);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "typ_KeyValuePairGuidStringTable", rows.Select<KeyValuePair<Guid, string>, SqlDataRecord>(selector));
    }

    public static SqlParameter BindKeyValuePairInt32DateTimeTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<KeyValuePair<int, DateTime>> rows)
    {
      rows = rows ?? Enumerable.Empty<KeyValuePair<int, DateTime>>();
      System.Func<KeyValuePair<int, DateTime>, SqlDataRecord> selector = (System.Func<KeyValuePair<int, DateTime>, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SimpleTableValuedParameterExtensions.typ_KeyValuePairInt32DateTimeTable);
        sqlDataRecord.SetInt32(0, row.Key);
        sqlDataRecord.SetDateTime(1, row.Value);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "typ_KeyValuePairInt32DateTimeTable", rows.Select<KeyValuePair<int, DateTime>, SqlDataRecord>(selector));
    }

    public static SqlParameter BindKeyValuePairInt32Int32Table(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<KeyValuePair<int, int>> rows)
    {
      rows = rows ?? Enumerable.Empty<KeyValuePair<int, int>>();
      System.Func<KeyValuePair<int, int>, SqlDataRecord> selector = (System.Func<KeyValuePair<int, int>, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SimpleTableValuedParameterExtensions.typ_KeyValuePairInt32Int32Table);
        sqlDataRecord.SetInt32(0, row.Key);
        sqlDataRecord.SetInt32(1, row.Value);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "typ_KeyValuePairInt32Int32Table", rows.Select<KeyValuePair<int, int>, SqlDataRecord>(selector));
    }

    public static SqlParameter BindKeyValuePairInt64Int32Table(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<KeyValuePair<long, int>> rows)
    {
      rows = rows ?? Enumerable.Empty<KeyValuePair<long, int>>();
      System.Func<KeyValuePair<long, int>, SqlDataRecord> selector = (System.Func<KeyValuePair<long, int>, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SimpleTableValuedParameterExtensions.typ_KeyValuePairInt64Int32Table);
        sqlDataRecord.SetInt64(0, row.Key);
        sqlDataRecord.SetInt32(1, row.Value);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "typ_KeyValuePairInt64Int32Table", rows.Select<KeyValuePair<long, int>, SqlDataRecord>(selector));
    }

    public static SqlParameter BindKeyValuePairInt32StringTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<KeyValuePair<int, string>> rows)
    {
      rows = rows ?? Enumerable.Empty<KeyValuePair<int, string>>();
      System.Func<KeyValuePair<int, string>, SqlDataRecord> selector = (System.Func<KeyValuePair<int, string>, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SimpleTableValuedParameterExtensions.typ_KeyValuePairInt32StringTable);
        sqlDataRecord.SetInt32(0, row.Key);
        sqlDataRecord.SetString(1, row.Value);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "typ_KeyValuePairInt32StringTable", rows.Select<KeyValuePair<int, string>, SqlDataRecord>(selector));
    }

    public static SqlParameter BindKeyValuePairStringInt32Table(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<KeyValuePair<string, int>> rows)
    {
      rows = rows ?? Enumerable.Empty<KeyValuePair<string, int>>();
      System.Func<KeyValuePair<string, int>, SqlDataRecord> selector = (System.Func<KeyValuePair<string, int>, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SimpleTableValuedParameterExtensions.typ_KeyValuePairStringInt32Table);
        sqlDataRecord.SetString(0, row.Key);
        sqlDataRecord.SetInt32(1, row.Value);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "typ_KeyValuePairStringInt32Table", rows.Select<KeyValuePair<string, int>, SqlDataRecord>(selector));
    }

    public static SqlParameter BindKeyValuePairStringTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<KeyValuePair<string, string>> rows)
    {
      rows = rows ?? Enumerable.Empty<KeyValuePair<string, string>>();
      System.Func<KeyValuePair<string, string>, SqlDataRecord> selector = (System.Func<KeyValuePair<string, string>, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SimpleTableValuedParameterExtensions.typ_KeyValuePairStringTable);
        sqlDataRecord.SetString(0, row.Key);
        sqlDataRecord.SetString(1, row.Value);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "typ_KeyValuePairStringTable", rows.Select<KeyValuePair<string, string>, SqlDataRecord>(selector));
    }

    public static SqlParameter BindKeyValuePairStringTableNullable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<KeyValuePair<string, string>> rows)
    {
      rows = rows ?? Enumerable.Empty<KeyValuePair<string, string>>();
      System.Func<KeyValuePair<string, string>, SqlDataRecord> selector = (System.Func<KeyValuePair<string, string>, SqlDataRecord>) (row =>
      {
        SqlDataRecord record = new SqlDataRecord(SimpleTableValuedParameterExtensions.typ_KeyValuePairStringTableNullable);
        record.SetString(0, row.Key);
        record.SetNullableString(1, row.Value);
        return record;
      });
      return component.BindTable(parameterName, "typ_KeyValuePairStringTableNullable", rows.Select<KeyValuePair<string, string>, SqlDataRecord>(selector));
    }

    public static SqlParameter BindOrderedGuidTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Guid> rows,
      bool omitEmptyEntries = false)
    {
      rows = rows ?? Enumerable.Empty<Guid>();
      Func<Guid, int, IEnumerable<SqlDataRecord>> selector = (Func<Guid, int, IEnumerable<SqlDataRecord>>) ((row, index) =>
      {
        if (omitEmptyEntries && row == Guid.Empty)
          return Enumerable.Empty<SqlDataRecord>();
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SimpleTableValuedParameterExtensions.typ_GuidInt32Table);
        sqlDataRecord.SetGuid(0, row);
        sqlDataRecord.SetInt32(1, index);
        return (IEnumerable<SqlDataRecord>) new SqlDataRecord[1]
        {
          sqlDataRecord
        };
      });
      return component.BindTable(parameterName, "typ_GuidInt32Table", rows.SelectMany<Guid, SqlDataRecord>(selector));
    }

    public static SqlParameter BindOrderedStringTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<string> rows,
      bool nvarchar = true,
      bool omitNullEntries = false)
    {
      rows = rows ?? Enumerable.Empty<string>();
      Func<string, int, IEnumerable<SqlDataRecord>> selector = (Func<string, int, IEnumerable<SqlDataRecord>>) ((row, index) =>
      {
        if (omitNullEntries && row == null)
          return Enumerable.Empty<SqlDataRecord>();
        SqlDataRecord sqlDataRecord = new SqlDataRecord(nvarchar ? SimpleTableValuedParameterExtensions.typ_KeyValuePairInt32StringTable : SimpleTableValuedParameterExtensions.typ_KeyValuePairInt32StringVarcharTable);
        sqlDataRecord.SetInt32(0, index);
        sqlDataRecord.SetString(1, row);
        return (IEnumerable<SqlDataRecord>) new SqlDataRecord[1]
        {
          sqlDataRecord
        };
      });
      return component.BindTable(parameterName, nvarchar ? "typ_KeyValuePairInt32StringTable" : "typ_KeyValuePairInt32StringVarcharTable", rows.SelectMany<string, SqlDataRecord>(selector));
    }

    public static SqlParameter BindSortedGuidTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<Guid> rows)
    {
      rows = rows ?? Enumerable.Empty<Guid>();
      System.Func<Guid, SqlDataRecord> selector = (System.Func<Guid, SqlDataRecord>) (row =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(SimpleTableValuedParameterExtensions.typ_SortedGuidTable);
        sqlDataRecord.SetGuid(0, row);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, "typ_SortedGuidTable", rows.Select<Guid, SqlDataRecord>(selector));
    }

    public static SqlParameter BindStringTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<string> rows,
      bool treatNullAsEmpty = false,
      int maxLength = 2147483647,
      bool nvarchar = true)
    {
      rows = rows ?? Enumerable.Empty<string>();
      System.Func<string, SqlDataRecord> selector = (System.Func<string, SqlDataRecord>) (row =>
      {
        SqlDataRecord record = new SqlDataRecord(nvarchar ? SimpleTableValuedParameterExtensions.typ_StringTable : SimpleTableValuedParameterExtensions.typ_StringVarcharTable);
        if (treatNullAsEmpty)
          row = row ?? string.Empty;
        if (row != null && row.Length > maxLength)
          row = row.Substring(0, maxLength);
        record.SetNullableString(0, row);
        return record;
      });
      return component.BindTable(parameterName, nvarchar ? "typ_StringTable" : "typ_StringVarcharTable", rows.Select<string, SqlDataRecord>(selector));
    }
  }
}
