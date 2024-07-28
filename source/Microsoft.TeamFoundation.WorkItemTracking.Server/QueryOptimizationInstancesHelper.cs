// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryOptimizationInstancesHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public static class QueryOptimizationInstancesHelper
  {
    public static void GetSlownessThresholdFromHistories(
      IEnumerable<QueryOptimizationInstance> instances,
      IEnumerable<QueryExecutionHistory> histories,
      int minRecordCountForOpt,
      int percentileForThreshold,
      bool filterDuplicateQueryExecutionHistories)
    {
      ArgumentUtility.CheckForNull<IEnumerable<QueryOptimizationInstance>>(instances, nameof (instances));
      if (histories == null)
        return;
      IEnumerable<QueryExecutionHistory> inner = histories;
      if (filterDuplicateQueryExecutionHistories)
        inner = (IEnumerable<QueryExecutionHistory>) histories.GroupBy<QueryExecutionHistory, string, QueryExecutionHistory>((Func<QueryExecutionHistory, string>) (history => history.QueryHash), (Func<string, IEnumerable<QueryExecutionHistory>, QueryExecutionHistory>) ((queryHash, historyGroup) => historyGroup.First<QueryExecutionHistory>())).ToList<QueryExecutionHistory>();
      instances.Join<QueryOptimizationInstance, QueryExecutionHistory, string, QueryOptimizationInstance>(inner, (Func<QueryOptimizationInstance, string>) (instance => instance.QueryHash), (Func<QueryExecutionHistory, string>) (history => history.QueryHash), (Func<QueryOptimizationInstance, QueryExecutionHistory, QueryOptimizationInstance>) ((instance, history) =>
      {
        int num = history.ExecutionRecords.Count<QueryExecutionRecord>();
        instance.SetSlownessThresholdInMsFromHistory(num < minRecordCountForOpt ? 0 : history.ExecutionRecords.OrderBy<QueryExecutionRecord, int>((Func<QueryExecutionRecord, int>) (r => r.DurationInMs)).ElementAt<QueryExecutionRecord>(percentileForThreshold * (num - 1) / 100).DurationInMs);
        return instance;
      })).ToList<QueryOptimizationInstance>();
    }
  }
}
