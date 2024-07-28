// Decompiled with JetBrains decompiler
// Type: MemOTableValuedParameter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

internal static class MemOTableValuedParameter
{
  private static readonly SqlMetaData[] memo_typ_GuidTable = new SqlMetaData[1]
  {
    new SqlMetaData("Id", SqlDbType.UniqueIdentifier)
  };
  private static readonly SqlMetaData[] memo_typ_KeyValuePairInt32StringTable = new SqlMetaData[2]
  {
    new SqlMetaData("Key", SqlDbType.Int),
    new SqlMetaData("Value", SqlDbType.NVarChar, -1L)
  };
  private static readonly SqlMetaData[] memo_typ_KeyValuePairInt32StringVarcharTable = new SqlMetaData[2]
  {
    new SqlMetaData("Key", SqlDbType.Int),
    new SqlMetaData("Value", SqlDbType.VarChar, -1L)
  };
  private static readonly SqlMetaData[] memo_typ_GuidInt32Table = new SqlMetaData[2]
  {
    new SqlMetaData("a", SqlDbType.UniqueIdentifier),
    new SqlMetaData("b", SqlDbType.Int)
  };

  public static SqlParameter BindMemoGuidTable(
    this TeamFoundationSqlResourceComponent component,
    string parameterName,
    IEnumerable<Guid> rows)
  {
    rows = rows ?? Enumerable.Empty<Guid>();
    System.Func<Guid, SqlDataRecord> selector = (System.Func<Guid, SqlDataRecord>) (row =>
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(MemOTableValuedParameter.memo_typ_GuidTable);
      sqlDataRecord.SetGuid(0, row);
      return sqlDataRecord;
    });
    return component.BindTable(parameterName, "MemO.typ_GuidTable", rows.Select<Guid, SqlDataRecord>(selector));
  }

  public static SqlParameter BindMemoOrderedStringTable(
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
      SqlDataRecord sqlDataRecord = new SqlDataRecord(nvarchar ? MemOTableValuedParameter.memo_typ_KeyValuePairInt32StringTable : MemOTableValuedParameter.memo_typ_KeyValuePairInt32StringVarcharTable);
      sqlDataRecord.SetInt32(0, index);
      sqlDataRecord.SetString(1, row);
      return (IEnumerable<SqlDataRecord>) new SqlDataRecord[1]
      {
        sqlDataRecord
      };
    });
    return component.BindTable(parameterName, nvarchar ? "MemO.typ_KeyValuePairInt32StringTable" : "MemO.typ_KeyValuePairInt32StringVarcharTable", rows.SelectMany<string, SqlDataRecord>(selector));
  }

  public static SqlParameter BindMemoOrderedGuidTable(
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
      SqlDataRecord sqlDataRecord = new SqlDataRecord(MemOTableValuedParameter.memo_typ_GuidInt32Table);
      sqlDataRecord.SetGuid(0, row);
      sqlDataRecord.SetInt32(1, index);
      return (IEnumerable<SqlDataRecord>) new SqlDataRecord[1]
      {
        sqlDataRecord
      };
    });
    return component.BindTable(parameterName, "MemO.typ_GuidInt32Table", rows.SelectMany<Guid, SqlDataRecord>(selector));
  }
}
