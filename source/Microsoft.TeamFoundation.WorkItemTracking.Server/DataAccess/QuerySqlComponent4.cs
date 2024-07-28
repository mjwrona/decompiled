// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.QuerySqlComponent4
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class QuerySqlComponent4 : QuerySqlComponent3
  {
    protected override void BindQueryExecutionInformationTable(
      string parameterName,
      IEnumerable<QueryExecutionInformation> queryExecutionInformation)
    {
      new QuerySqlComponent.QueryExecutionInformationTable3(parameterName, queryExecutionInformation).BindTable((QuerySqlComponent) this);
    }

    public override QueryRecordingTableInfo SaveQueryExecutionInformation(
      IEnumerable<QueryExecutionInformation> queryExecutionInformation,
      int infoTableRowCountLimit,
      int detailTableRowCountLimit)
    {
      this.PrepareStoredProcedure("prc_SaveQueryExecutionInformation");
      this.BindQueryExecutionInformationTable("@queryExecutionInformation", queryExecutionInformation);
      this.BindInt("@infoTableRowCountLimit", infoTableRowCountLimit);
      this.BindBoolean("@isRecordQueryExecutionDetailsEnabled", true);
      this.BindInt("@detailTableRowCountLimit", detailTableRowCountLimit);
      using (ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<QueryRecordingTableInfo>((ObjectBinder<QueryRecordingTableInfo>) new QuerySqlComponent.QueryRecordingTableInfoBinder());
        return resultCollection.GetCurrent<QueryRecordingTableInfo>().Items.First<QueryRecordingTableInfo>();
      }
    }

    public override QueryExecutionInfoReturnedPayload SaveQueryExecutionInformationIncludingAdhoc(
      IEnumerable<QueryExecutionInformation> queryExecutionInformation,
      int infoTableRowCountLimit,
      int detailTableRowCountLimit,
      IEnumerable<string> queryHashes,
      bool getRowCount)
    {
      this.PrepareStoredProcedure("prc_SaveQueryExecutionInformationIncludingAdhoc");
      this.BindQueryExecutionInformationTable("@queryExecutionInformation", queryExecutionInformation);
      this.BindInt("@infoTableRowCountLimit", infoTableRowCountLimit);
      this.BindBoolean("@isRecordQueryExecutionDetailsEnabled", true);
      this.BindInt("@detailTableRowCountLimit", detailTableRowCountLimit);
      using (ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<QueryRecordingTableInfo>((ObjectBinder<QueryRecordingTableInfo>) new QuerySqlComponent.QueryRecordingTableInfoBinder());
        QueryRecordingTableInfo recordingTableInfo = resultCollection.GetCurrent<QueryRecordingTableInfo>().Items.First<QueryRecordingTableInfo>();
        return new QueryExecutionInfoReturnedPayload()
        {
          QueryExecutionTableInfo = recordingTableInfo
        };
      }
    }

    public override int CleanupQueryExecutionInformation(
      DateTime cutOffTime,
      int maxAdhocQueriesRowCount,
      int batchSize)
    {
      this.PrepareStoredProcedure("prc_CleanupQueryExecutionInformation");
      this.BindDateTime("@cutOffTime", cutOffTime);
      this.BindInt("@maxAdhocQueriesRowCount", maxAdhocQueriesRowCount);
      return (int) this.ExecuteScalar();
    }
  }
}
