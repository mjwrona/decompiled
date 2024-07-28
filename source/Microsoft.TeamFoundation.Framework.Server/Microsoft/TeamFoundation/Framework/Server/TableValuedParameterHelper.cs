// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TableValuedParameterHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class TableValuedParameterHelper
  {
    public static SqlParameter Bind<T>(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<T> rows,
      SqlMetaData[] metaData,
      string typeName,
      Action<SqlDataRecord, T> bind)
    {
      rows = rows ?? Enumerable.Empty<T>();
      Func<T, SqlDataRecord> selector = (Func<T, SqlDataRecord>) (value =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(metaData);
        bind(sqlDataRecord, value);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, typeName, rows.Select<T, SqlDataRecord>(selector));
    }

    public static SqlParameter Bind<T>(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<T> rows,
      SqlMetaData[] metaData,
      string typeName,
      Action<SqlDataRecord, T, int> bind)
    {
      rows = rows ?? Enumerable.Empty<T>();
      Func<T, int, SqlDataRecord> selector = (Func<T, int, SqlDataRecord>) ((value, index) =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(metaData);
        bind(sqlDataRecord, value, index);
        return sqlDataRecord;
      });
      return component.BindTable(parameterName, typeName, rows.Select<T, SqlDataRecord>(selector));
    }
  }
}
