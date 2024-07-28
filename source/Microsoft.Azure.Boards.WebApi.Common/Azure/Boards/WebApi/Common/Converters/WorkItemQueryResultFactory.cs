// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.WorkItemQueryResultFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.Azure.Boards.WebApi.Common.Extensions;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters
{
  [DataContract]
  public class WorkItemQueryResultFactory
  {
    public static WorkItemQueryResult Create(
      WorkItemTrackingRequestContext witRequestContext,
      QueryResult queryResult,
      QueryExpression queryExpression,
      bool hasCrossProjectQueryPermission,
      Guid? projectId = null,
      bool excludeUrls = false)
    {
      QueryDualIntentSecuredObject intentSecuredObject = new QueryDualIntentSecuredObject(hasCrossProjectQueryPermission ? new Guid?() : projectId);
      WorkItemQueryResult workItemQueryResult = new WorkItemQueryResult((ISecuredObject) intentSecuredObject)
      {
        AsOf = queryResult.AsOfDateTime,
        QueryType = queryResult.QueryType.ToQueryType(),
        QueryResultType = queryResult.ResultType.ToQueryResultType(),
        Columns = WorkItemFieldReferenceFactory.Create(witRequestContext, queryExpression.DisplayFields, (ISecuredObject) intentSecuredObject, hasCrossProjectQueryPermission ? new Guid?() : projectId, includeUrls: !excludeUrls),
        SortColumns = WorkItemQuerySortColumnFactory.Create(witRequestContext, queryExpression.SortFields, (ISecuredObject) intentSecuredObject, hasCrossProjectQueryPermission ? new Guid?() : projectId, excludeUrls)
      };
      if (workItemQueryResult.QueryResultType == Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.QueryResultType.WorkItem)
        workItemQueryResult.WorkItems = queryResult.WorkItemIds.Select<int, WorkItemReference>((Func<int, WorkItemReference>) (wid => WorkItemReferenceFactory.Create(witRequestContext, wid, projectId, queryResult.GetTokenWorkItem(wid), excludeUrls: excludeUrls)));
      else
        workItemQueryResult.WorkItemRelations = queryResult.WorkItemLinks.Select<LinkQueryResultEntry, WorkItemLink>((Func<LinkQueryResultEntry, WorkItemLink>) (wil => WorkItemLinkFactory.Create(witRequestContext, wil.SourceId, wil.TargetId, hasCrossProjectQueryPermission ? new Guid?() : projectId, hasCrossProjectQueryPermission ? new Guid?() : projectId, wil.SourceToken, wil.TargetToken, (int) wil.LinkTypeId, excludeUrls: excludeUrls)));
      return workItemQueryResult;
    }
  }
}
