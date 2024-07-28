// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.QuerySqlComponent10
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class QuerySqlComponent10 : QuerySqlComponent9
  {
    public override QueryExecutionDetailRowModel GetQueryExecutionDetailsByQueryHash(
      string queryHash)
    {
      this.PrepareStoredProcedure("prc_GetQueryExecutionDetailsByQueryHash");
      this.BindString("@queryHash", queryHash, 64, false, SqlDbType.VarChar);
      return this.ExecuteUnknown<QueryExecutionDetailRowModel>((System.Func<IDataReader, QueryExecutionDetailRowModel>) (reader =>
      {
        reader.Read();
        return this.GetQueryExecutionDetailBinder().Bind(reader);
      }));
    }

    public override QueryExecutionDetailRowModel GetQueryExecutionDetailsByQueryId(Guid queryId)
    {
      this.PrepareStoredProcedure("prc_GetQueryExecutionDetailsByQueryId");
      this.BindGuid("@queryId", queryId);
      return this.ExecuteUnknown<QueryExecutionDetailRowModel>((System.Func<IDataReader, QueryExecutionDetailRowModel>) (reader =>
      {
        reader.Read();
        return this.GetQueryExecutionDetailBinder().Bind(reader);
      }));
    }
  }
}
