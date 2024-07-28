// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TableValuedParameterVersionHelper`1
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
  public abstract class TableValuedParameterVersionHelper<T>
  {
    private TeamFoundationSqlResourceComponent component;
    private string parameterName;
    private IEnumerable<T> rows;

    protected TableValuedParameterVersionHelper(
      TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<T> rows)
    {
      this.component = component;
      this.parameterName = parameterName;
      this.rows = rows ?? Enumerable.Empty<T>();
    }

    protected Func<T, SqlDataRecord> UsingRecord(
      SqlDataRecord record,
      Action<SqlDataRecord, T> bind)
    {
      return (Func<T, SqlDataRecord>) (value =>
      {
        bind(record, value);
        return record;
      });
    }

    protected Func<T, int, SqlDataRecord> UsingRecord(
      SqlDataRecord record,
      Action<SqlDataRecord, T, int> bind)
    {
      return (Func<T, int, SqlDataRecord>) ((value, index) =>
      {
        bind(record, value, index);
        return record;
      });
    }

    protected SqlParameter BindTable(
      SqlMetaData[] metaData,
      string typeName,
      Action<SqlDataRecord, T> bind)
    {
      SqlDataRecord record = new SqlDataRecord(metaData);
      return this.component.BindTable(this.parameterName, typeName, this.rows.Select<T, SqlDataRecord>(this.UsingRecord(record, bind)));
    }

    protected SqlParameter BindTable(
      SqlMetaData[] metaData,
      string typeName,
      Action<SqlDataRecord, T, int> bind)
    {
      SqlDataRecord record = new SqlDataRecord(metaData);
      return this.component.BindTable(this.parameterName, typeName, this.rows.Select<T, SqlDataRecord>(this.UsingRecord(record, bind)));
    }
  }
}
