// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.QuerySqlComponent11
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class QuerySqlComponent11 : QuerySqlComponent10
  {
    public override QueryExecutionInfoReturnedPayload SaveQueryExecutionInformationIncludingAdhocAndOptimizationV2(
      IEnumerable<QueryExecutionInformation> queryExecutionInformation,
      DateTime mostRecentCacheUpdatedTime,
      int infoTableRowCountLimit,
      int detailTableRowCountLimit,
      bool getRowCount)
    {
      this.PrepareSaveQueryExecutionInformationStoredProcedure("prc_SaveQueryExecutionInformationIncludingAdhocAndOptimizationV2");
      this.BindQueryExecutionInformationTable("@queryExecutionInformation", queryExecutionInformation);
      this.BindInt("@infoTableRowCountLimit", infoTableRowCountLimit);
      this.BindInt("@detailTableRowCountLimit", detailTableRowCountLimit);
      this.BindDateTime("@mostRecentCacheUpdatedTime", mostRecentCacheUpdatedTime, true);
      this.BindBoolean("@getRowCount", getRowCount);
      if (getRowCount)
      {
        using (ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<QueryRecordingTableInfo>((ObjectBinder<QueryRecordingTableInfo>) new QuerySqlComponent.QueryRecordingTableInfoBinder());
          resultCollection.AddBinder<QueryOptimizationInstance>((ObjectBinder<QueryOptimizationInstance>) new QuerySqlComponent.QueryOptimizationInstanceBinder());
          resultCollection.AddBinder<QueryExecutionHistory>((ObjectBinder<QueryExecutionHistory>) new QuerySqlComponent.QueryExecutionHistorysBinder());
          QueryRecordingTableInfo recordingTableInfo = resultCollection.GetCurrent<QueryRecordingTableInfo>().Items.First<QueryRecordingTableInfo>();
          IEnumerable<QueryOptimizationInstance> optimizationInstances = (IEnumerable<QueryOptimizationInstance>) null;
          if (resultCollection.TryNextResult())
            optimizationInstances = (IEnumerable<QueryOptimizationInstance>) resultCollection.GetCurrent<QueryOptimizationInstance>().Items;
          IEnumerable<QueryExecutionHistory> executionHistories = (IEnumerable<QueryExecutionHistory>) null;
          if (resultCollection.TryNextResult())
            executionHistories = (IEnumerable<QueryExecutionHistory>) resultCollection.GetCurrent<QueryExecutionHistory>().Items;
          return new QueryExecutionInfoReturnedPayload()
          {
            QueryOptimizationInstances = optimizationInstances,
            QueryExecutionTableInfo = recordingTableInfo,
            QueryHistories = executionHistories
          };
        }
      }
      else
      {
        using (ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<QueryOptimizationInstance>((ObjectBinder<QueryOptimizationInstance>) new QuerySqlComponent.QueryOptimizationInstanceBinder());
          resultCollection.AddBinder<QueryExecutionHistory>((ObjectBinder<QueryExecutionHistory>) new QuerySqlComponent.QueryExecutionHistorysBinder());
          List<QueryOptimizationInstance> items = resultCollection.GetCurrent<QueryOptimizationInstance>().Items;
          IEnumerable<QueryExecutionHistory> executionHistories = (IEnumerable<QueryExecutionHistory>) null;
          if (resultCollection.TryNextResult())
            executionHistories = (IEnumerable<QueryExecutionHistory>) resultCollection.GetCurrent<QueryExecutionHistory>().Items;
          return new QueryExecutionInfoReturnedPayload()
          {
            QueryOptimizationInstances = (IEnumerable<QueryOptimizationInstance>) items,
            QueryHistories = executionHistories
          };
        }
      }
    }

    public override QueryExecutionInfoReturnedPayload SaveQueryExecutionInformationIncludingAdhocV2(
      IEnumerable<QueryExecutionInformation> queryExecutionInformation,
      int infoTableRowCountLimit,
      int detailTableRowCountLimit,
      IEnumerable<string> queryHashes,
      bool getRowCount)
    {
      this.PrepareSaveQueryExecutionInformationStoredProcedure("prc_SaveQueryExecutionInformationIncludingAdhocV2");
      this.BindQueryExecutionInformationTable("@queryExecutionInformation", queryExecutionInformation);
      this.BindInt("@infoTableRowCountLimit", infoTableRowCountLimit);
      this.BindInt("@detailTableRowCountLimit", detailTableRowCountLimit);
      this.BindBoolean("@getRowCount", getRowCount);
      this.BindStringTable("@queryHashes", queryHashes, nvarchar: false);
      if (getRowCount)
      {
        using (ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<QueryRecordingTableInfo>((ObjectBinder<QueryRecordingTableInfo>) new QuerySqlComponent.QueryRecordingTableInfoBinder());
          resultCollection.AddBinder<QueryExecutionHistory>((ObjectBinder<QueryExecutionHistory>) new QuerySqlComponent.QueryExecutionHistorysBinder());
          QueryRecordingTableInfo recordingTableInfo = resultCollection.GetCurrent<QueryRecordingTableInfo>().Items.First<QueryRecordingTableInfo>();
          IEnumerable<QueryExecutionHistory> executionHistories = (IEnumerable<QueryExecutionHistory>) null;
          if (resultCollection.TryNextResult())
            executionHistories = (IEnumerable<QueryExecutionHistory>) resultCollection.GetCurrent<QueryExecutionHistory>().Items;
          return new QueryExecutionInfoReturnedPayload()
          {
            QueryExecutionTableInfo = recordingTableInfo,
            QueryHistories = executionHistories
          };
        }
      }
      else
      {
        using (ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<QueryExecutionHistory>((ObjectBinder<QueryExecutionHistory>) new QuerySqlComponent.QueryExecutionHistorysBinder());
          IEnumerable<QueryExecutionHistory> items = (IEnumerable<QueryExecutionHistory>) resultCollection.GetCurrent<QueryExecutionHistory>().Items;
          return new QueryExecutionInfoReturnedPayload()
          {
            QueryHistories = items
          };
        }
      }
    }
  }
}
