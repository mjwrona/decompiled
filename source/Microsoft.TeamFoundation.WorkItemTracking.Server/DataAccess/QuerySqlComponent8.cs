// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.QuerySqlComponent8
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class QuerySqlComponent8 : QuerySqlComponent7
  {
    private int? saveQueryExecutionInformationTimeout;

    protected override void BindQueryExecutionInformationTable(
      string parameterName,
      IEnumerable<QueryExecutionInformation> queryExecutionInformation)
    {
      new QuerySqlComponent.QueryExecutionInformationTable5(parameterName, queryExecutionInformation).BindTable((QuerySqlComponent) this);
    }

    protected virtual SqlCommand PrepareSaveQueryExecutionInformationStoredProcedure(
      string storedProcedure)
    {
      return !this.saveQueryExecutionInformationTimeout.HasValue ? this.PrepareStoredProcedure(storedProcedure) : this.PrepareStoredProcedure(storedProcedure, this.saveQueryExecutionInformationTimeout.Value);
    }

    public override void SetSaveQueryExecutionInformationTimeout(int? timeoutInSeconds) => this.saveQueryExecutionInformationTimeout = timeoutInSeconds;

    public override int CleanupQueryExecutionInformation(
      DateTime cutOffTime,
      int maxAdhocQueriesRowCount,
      int batchSize)
    {
      this.PrepareStoredProcedure("prc_CleanupQueryExecutionInformation");
      this.BindDateTime("@cutOffTime", cutOffTime);
      this.BindInt("@maxAdhocQueriesRowCount", maxAdhocQueriesRowCount);
      this.BindInt("@batchSize", batchSize);
      return (int) this.ExecuteScalar();
    }

    public override int CleanupQueryExecutionDetails(
      DateTime cutOffTime,
      int maxRowCount,
      int batchSize)
    {
      this.PrepareStoredProcedure("prc_CleanupQueryExecutionDetails");
      this.BindDateTime("@cutOffTime", cutOffTime);
      this.BindInt("@maxRowCount", maxRowCount);
      this.BindInt("@batchSize", batchSize);
      return (int) this.ExecuteScalar();
    }

    public override QueryExecutionInfoReturnedPayload LoadQueryOptimizationEntriesForCacheInitialization(
      int loadCount,
      DateTime cacheLoadCutOffTime)
    {
      this.PrepareStoredProcedure("prc_LoadQueryOptimizationEntriesForCacheInitialization");
      this.BindInt("@loadCount", loadCount);
      this.BindDateTime("@cacheLoadCutOffTime", cacheLoadCutOffTime);
      using (ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<QueryOptimizationInstance>((ObjectBinder<QueryOptimizationInstance>) new QuerySqlComponent.QueryOptimizationInstanceBinder());
        resultCollection.AddBinder<QueryExecutionHistory>((ObjectBinder<QueryExecutionHistory>) new QuerySqlComponent.QueryExecutionHistorysBinder());
        IEnumerable<QueryOptimizationInstance> items = (IEnumerable<QueryOptimizationInstance>) resultCollection.GetCurrent<QueryOptimizationInstance>().Items;
        IEnumerable<QueryExecutionHistory> executionHistories = (IEnumerable<QueryExecutionHistory>) null;
        if (resultCollection.TryNextResult())
          executionHistories = (IEnumerable<QueryExecutionHistory>) resultCollection.GetCurrent<QueryExecutionHistory>().Items;
        return new QueryExecutionInfoReturnedPayload()
        {
          QueryOptimizationInstances = items,
          QueryHistories = executionHistories
        };
      }
    }

    public override QueryExecutionInfoReturnedPayload SaveQueryExecutionInformationIncludingAdhoc(
      IEnumerable<QueryExecutionInformation> queryExecutionInformation,
      int infoTableRowCountLimit,
      int detailTableRowCountLimit,
      IEnumerable<string> queryHashes,
      bool getRowCount)
    {
      this.PrepareSaveQueryExecutionInformationStoredProcedure("prc_SaveQueryExecutionInformationIncludingAdhoc");
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

    public override QueryExecutionInfoReturnedPayload SaveQueryExecutionInformationIncludingAdhocAndOptimization(
      IEnumerable<QueryExecutionInformation> queryExecutionInformation,
      DateTime mostRecentCacheUpdatedTime,
      int infoTableRowCountLimit,
      int detailTableRowCountLimit,
      bool getRowCount)
    {
      this.PrepareSaveQueryExecutionInformationStoredProcedure("prc_SaveQueryExecutionInformationIncludingAdhocAndOptimization");
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

    public override QueryRecordingTableInfo SaveQueryExecutionInformation(
      IEnumerable<QueryExecutionInformation> queryExecutionInformation,
      int infoTableRowCountLimit,
      int detailTableRowCountLimit)
    {
      this.PrepareSaveQueryExecutionInformationStoredProcedure("prc_SaveQueryExecutionInformation");
      this.BindQueryExecutionInformationTable("@queryExecutionInformation", queryExecutionInformation);
      this.BindInt("@infoTableRowCountLimit", infoTableRowCountLimit);
      this.BindInt("@detailTableRowCountLimit", detailTableRowCountLimit);
      using (ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<QueryRecordingTableInfo>((ObjectBinder<QueryRecordingTableInfo>) new QuerySqlComponent.QueryRecordingTableInfoBinder());
        return resultCollection.GetCurrent<QueryRecordingTableInfo>().Items.First<QueryRecordingTableInfo>();
      }
    }
  }
}
