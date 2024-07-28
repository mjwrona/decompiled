// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.QuerySqlComponent5
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class QuerySqlComponent5 : QuerySqlComponent4
  {
    protected override void BindQueryExecutionInformationTable(
      string parameterName,
      IEnumerable<QueryExecutionInformation> queryExecutionInformation)
    {
      new QuerySqlComponent.QueryExecutionInformationTable4(parameterName, queryExecutionInformation).BindTable((QuerySqlComponent) this);
    }

    public override QueryExecutionInfoReturnedPayload SaveQueryExecutionInformationIncludingAdhocAndOptimization(
      IEnumerable<QueryExecutionInformation> queryExecutionInformation,
      DateTime mostRecentCacheUpdatedTime,
      int infoTableRowCountLimit,
      int detailTableRowCountLimit,
      bool getRowCount)
    {
      this.PrepareStoredProcedure("prc_SaveQueryExecutionInformationIncludingAdhocAndOptimization");
      this.BindQueryExecutionInformationTable("@queryExecutionInformation", queryExecutionInformation);
      this.BindInt("@infoTableRowCountLimit", infoTableRowCountLimit);
      this.BindBoolean("@isRecordQueryExecutionDetailsEnabled", true);
      this.BindInt("@detailTableRowCountLimit", detailTableRowCountLimit);
      this.BindDateTime("@mostRecentCacheUpdatedTime", mostRecentCacheUpdatedTime, true);
      using (ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<QueryRecordingTableInfo>((ObjectBinder<QueryRecordingTableInfo>) new QuerySqlComponent.QueryRecordingTableInfoBinder());
        resultCollection.AddBinder<QueryOptimizationInstance>((ObjectBinder<QueryOptimizationInstance>) new QuerySqlComponent.QueryOptimizationInstanceBinder());
        QueryRecordingTableInfo recordingTableInfo = resultCollection.GetCurrent<QueryRecordingTableInfo>().Items.First<QueryRecordingTableInfo>();
        IEnumerable<QueryOptimizationInstance> optimizationInstances = (IEnumerable<QueryOptimizationInstance>) null;
        if (resultCollection.TryNextResult())
          optimizationInstances = (IEnumerable<QueryOptimizationInstance>) resultCollection.GetCurrent<QueryOptimizationInstance>().Items;
        return new QueryExecutionInfoReturnedPayload()
        {
          QueryOptimizationInstances = optimizationInstances,
          QueryExecutionTableInfo = recordingTableInfo
        };
      }
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
        IEnumerable<QueryOptimizationInstance> items = (IEnumerable<QueryOptimizationInstance>) resultCollection.GetCurrent<QueryOptimizationInstance>().Items;
        return new QueryExecutionInfoReturnedPayload()
        {
          QueryOptimizationInstances = items
        };
      }
    }
  }
}
