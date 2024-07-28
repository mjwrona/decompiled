// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.Backlog.OrderedWorkItemResult
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Agile.Server.Backlog
{
  internal class OrderedWorkItemResult
  {
    private QueryResult m_queryResult;
    private Dictionary<int, int> m_order;

    internal OrderedWorkItemResult(QueryResult queryResult)
      : this(queryResult, true)
    {
    }

    internal OrderedWorkItemResult(QueryResult queryResult, bool generateOrder)
    {
      ArgumentUtility.CheckForNull<QueryResult>(queryResult, nameof (queryResult));
      this.m_queryResult = queryResult;
      if (!generateOrder)
        return;
      this.BuildOrder();
    }

    public QueryResult Result => this.m_queryResult;

    public Dictionary<int, int> Order
    {
      get => this.m_order;
      protected set => this.m_order = value;
    }

    protected void BuildOrder()
    {
      this.m_order = new Dictionary<int, int>();
      int num = 0;
      if (this.m_queryResult.QueryType == QueryType.WorkItems)
      {
        foreach (int workItemId in this.m_queryResult.WorkItemIds)
        {
          if (!this.m_order.ContainsKey(workItemId))
            this.m_order.Add(workItemId, num++);
        }
      }
      else
      {
        foreach (LinkQueryResultEntry workItemLink in this.m_queryResult.WorkItemLinks)
        {
          if (!this.m_order.ContainsKey(workItemLink.TargetId))
            this.m_order.Add(workItemLink.TargetId, num++);
        }
      }
    }

    public static OrderedWorkItemResult Create(QueryResult queryResult)
    {
      ArgumentUtility.CheckForNull<QueryResult>(queryResult, nameof (queryResult));
      return new OrderedWorkItemResult(queryResult);
    }
  }
}
