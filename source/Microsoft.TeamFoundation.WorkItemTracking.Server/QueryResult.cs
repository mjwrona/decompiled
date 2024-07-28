// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryResult
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class QueryResult
  {
    private IEnumerable<int> m_workItemIds;
    private IEnumerable<LinkQueryResultEntry> m_workItemLinks;
    private IDictionary<int, string> m_workItemToTokenLookup = (IDictionary<int, string>) new Dictionary<int, string>();

    internal QueryResult(QueryType queryType, ExtendedQueryExecutionResult extendedResult)
    {
      if (extendedResult.AsOfDateTime.Kind != DateTimeKind.Utc)
        extendedResult.AsOfDateTime = DateTime.SpecifyKind(extendedResult.AsOfDateTime, DateTimeKind.Utc);
      this.QueryType = queryType;
      this.AsOfDateTime = extendedResult.AsOfDateTime;
      this.HasMoreResult = extendedResult.HasMoreResult;
      this.HasPendingReclassification = extendedResult.HasPendingReclassification;
    }

    public QueryResult()
    {
    }

    public QueryType QueryType { get; private set; }

    public DateTime AsOfDateTime { get; private set; }

    public QueryResultType ResultType { get; private set; }

    public int Count { get; private set; }

    public bool HasMoreResult { get; private set; }

    public bool HasPendingReclassification { get; private set; }

    public IEnumerable<int> WorkItemIds => this.m_workItemIds;

    public IEnumerable<LinkQueryResultEntry> WorkItemLinks => this.m_workItemLinks;

    internal void SetWorkItemResult(ICollection<int> workItemIds)
    {
      this.ResultType = QueryResultType.WorkItem;
      this.Count = workItemIds.Count;
      this.m_workItemIds = (IEnumerable<int>) workItemIds;
    }

    internal void SetWorkItemResult(
      ICollection<int> workItemIds,
      IDictionary<int, string> workItemToTokenLookup)
    {
      this.ResultType = QueryResultType.WorkItem;
      this.Count = workItemIds.Count;
      this.m_workItemIds = (IEnumerable<int>) workItemIds;
      this.m_workItemToTokenLookup = workItemToTokenLookup;
    }

    internal void SetWorkItemLinkResult(ICollection<LinkQueryResultEntry> workItemLinks)
    {
      this.ResultType = QueryResultType.WorkItemLink;
      this.Count = workItemLinks.Count;
      this.m_workItemLinks = (IEnumerable<LinkQueryResultEntry>) workItemLinks;
    }

    public string GetTokenWorkItem(int workItemId) => this.m_workItemToTokenLookup.ContainsKey(workItemId) ? this.m_workItemToTokenLookup[workItemId] : throw new KeyNotFoundException(ServerResources.CannotFindTokenForWorkItem((object) workItemId));

    public Guid GetNamespaceIdWorkItem() => AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid;

    public int GetRequiredPermissionsWorkItem() => 16;
  }
}
