// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebServer.SDStalenessHandler.AzureWorkItemStore
// Assembly: Microsoft.VisualStudio.Services.Search.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1112A012-BB03-4D21-B53E-3AFB00CCC7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.WebServer.dll

using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.WebServer.SDStalenessHandler
{
  internal class AzureWorkItemStore
  {
    private readonly CloudTable m_workItemsTable;
    private readonly TimeSpan m_retryInterval;
    private readonly int m_retryCount;

    public AzureWorkItemStore(
      string connectionString,
      string workItemsTableName,
      TimeSpan retryInterval,
      int retryCount)
    {
      if (string.IsNullOrEmpty(connectionString))
        throw new ArgumentNullException(nameof (connectionString));
      if (string.IsNullOrEmpty(workItemsTableName))
        throw new ArgumentNullException(nameof (workItemsTableName));
      if (retryCount < 0)
        throw new ArgumentException("retryCount should be greater than zero");
      this.m_retryInterval = retryInterval;
      this.m_retryCount = retryCount;
      try
      {
        this.m_workItemsTable = CloudStorageAccount.Parse(connectionString).CreateCloudTableClient().GetTableReference(workItemsTableName);
      }
      catch
      {
        throw new AggregateException("Unable to connect to Azure Storage Table");
      }
    }

    public WorkItem[] GetWorkItem(string sourceControl, string branch)
    {
      try
      {
        return RetryHandler.Do<WorkItem[]>((Func<WorkItem[]>) (() =>
        {
          if (!this.m_workItemsTable.Exists())
            return (WorkItem[]) null;
          List<WorkItem> list1 = this.m_workItemsTable.ExecuteQuery<WorkItem>(new TableQuery<WorkItem>().Where(TableQuery.CombineFilters(TableQuery.GenerateFilterCondition("SourceControl", "eq", sourceControl), "and", TableQuery.GenerateFilterCondition("Branch", "eq", branch)))).ToList<WorkItem>();
          if (list1 == null)
            return (WorkItem[]) null;
          List<WorkItem> list2 = list1.ToList<WorkItem>();
          list2.Sort((Comparison<WorkItem>) ((x, y) => x.LastIndexedChangeId.CompareTo(y.LastIndexedChangeId)));
          return list2.ToArray();
        }), this.m_retryInterval, this.m_retryCount);
      }
      catch
      {
        throw new AggregateException("Unable to query WorkItem table in Azure Storage");
      }
    }
  }
}
