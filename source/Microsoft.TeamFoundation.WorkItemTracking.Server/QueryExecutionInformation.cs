// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExecutionInformation
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class QueryExecutionInformation : BufferableItem
  {
    public Guid? QueryId { get; set; }

    public string QueryHash { get; set; }

    public DateTime? LastRunTime { get; set; }

    public Guid? LastRunByVsid { get; set; }

    public int? LastExecutionTimeMilliseconds { get; set; }

    public int? LastExecutionResultCount { get; set; }

    public Microsoft.TeamFoundation.WorkItemTracking.Server.QueryType? QueryType { get; set; }

    public bool? IsExecutedOnReadReplica { get; set; }

    public string WiqlText { get; set; }

    public string SqlText { get; set; }

    public Microsoft.TeamFoundation.WorkItemTracking.Server.QueryCategory? QueryCategory { get; set; }

    public QueryOptimizationInstance OptimizationInstance { get; set; }

    public IList<QueryExecutionRecord> ExecutionHistory { get; set; }

    public bool NeedResetHistory { get; set; }

    protected override BufferableItem Combine(BufferableItem existingItem, BufferableItem newItem)
    {
      QueryExecutionInformation executionInformation1 = (QueryExecutionInformation) existingItem;
      QueryExecutionInformation executionInformation2 = (QueryExecutionInformation) newItem;
      executionInformation1.QueryHash = executionInformation2.QueryHash;
      executionInformation1.LastRunTime = executionInformation2.LastRunTime;
      executionInformation1.LastRunByVsid = executionInformation2.LastRunByVsid;
      executionInformation1.LastExecutionTimeMilliseconds = executionInformation2.LastExecutionTimeMilliseconds;
      executionInformation1.LastExecutionResultCount = executionInformation2.LastExecutionResultCount;
      executionInformation1.QueryType = executionInformation2.QueryType;
      executionInformation1.IsExecutedOnReadReplica = executionInformation2.IsExecutedOnReadReplica;
      executionInformation1.WiqlText = executionInformation2.WiqlText;
      executionInformation1.SqlText = executionInformation2.SqlText;
      executionInformation1.QueryCategory = executionInformation2.QueryCategory;
      executionInformation1.NeedResetHistory = executionInformation2.NeedResetHistory;
      if (executionInformation1.OptimizationInstance != null)
        executionInformation1.OptimizationInstance.Merge((BufferableItem) executionInformation2.OptimizationInstance);
      else
        executionInformation1.OptimizationInstance = executionInformation2.OptimizationInstance;
      if (executionInformation1.ExecutionHistory == null)
        executionInformation1.ExecutionHistory = (IList<QueryExecutionRecord>) new List<QueryExecutionRecord>();
      if (executionInformation2.ExecutionHistory != null)
      {
        foreach (QueryExecutionRecord queryExecutionRecord in (IEnumerable<QueryExecutionRecord>) executionInformation2.ExecutionHistory)
        {
          if (queryExecutionRecord != null)
            executionInformation1.ExecutionHistory.Add(queryExecutionRecord);
        }
      }
      return (BufferableItem) executionInformation1;
    }
  }
}
