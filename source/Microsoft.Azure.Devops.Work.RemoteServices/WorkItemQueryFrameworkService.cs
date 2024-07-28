// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.RemoteServices.WorkItemQueryFrameworkService
// Assembly: Microsoft.Azure.Devops.Work.RemoteServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C97796CA-4166-42B2-B96F-9A166B07FF72
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Devops.Work.RemoteServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Devops.Work.RemoteServices
{
  internal class WorkItemQueryFrameworkService : IWorkItemQueryRemotableService, IVssFrameworkService
  {
    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<QueryHierarchyItem> GetQueries(
      IVssRequestContext requestContext,
      Guid projectId,
      QueryExpand expand = QueryExpand.None,
      int depth = 0,
      bool includeDeleted = false)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      return (IEnumerable<QueryHierarchyItem>) requestContext.TraceBlock<List<QueryHierarchyItem>>(919700, 919701, 919702, "FrameworkServices", nameof (WorkItemQueryFrameworkService), nameof (GetQueries), (Func<List<QueryHierarchyItem>>) (() => requestContext.GetClient<WorkItemTrackingHttpClient>().GetQueriesAsync(projectId, new QueryExpand?(expand), new int?(depth), new bool?(includeDeleted)).Result));
    }

    public IEnumerable<QueryHierarchyItem> GetQueries(
      IVssRequestContext requestContext,
      string projectName,
      QueryExpand expand = QueryExpand.None,
      int depth = 0,
      bool includeDeleted = false)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      return (IEnumerable<QueryHierarchyItem>) requestContext.TraceBlock<List<QueryHierarchyItem>>(919700, 919701, 919702, "FrameworkServices", nameof (WorkItemQueryFrameworkService), nameof (GetQueries), (Func<List<QueryHierarchyItem>>) (() => requestContext.GetClient<WorkItemTrackingHttpClient>().GetQueriesAsync(projectName, new QueryExpand?(expand), new int?(depth), new bool?(includeDeleted)).Result));
    }

    public WorkItemQueryResult GetQueryResult(
      IVssRequestContext requestContext,
      Wiql wiql,
      Guid projectId,
      bool? timePrecision = null,
      int? top = null,
      bool skipWiqlTextValidation = false)
    {
      throw new NotImplementedException();
    }

    public WorkItemQueryResult GetQueryResult(
      IVssRequestContext requestContext,
      Wiql wiql,
      string projectName,
      bool? timePrecision = null,
      int? top = null,
      bool skipWiqlTextValidation = false)
    {
      throw new NotImplementedException();
    }

    public void ValidateQuery(IVssRequestContext requestContext, Guid projectId, string wiql)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckStringForNullOrEmpty(wiql, nameof (wiql));
      QueryHierarchyItem postQuery = new QueryHierarchyItem()
      {
        Wiql = wiql
      };
      requestContext.TraceBlock<QueryHierarchyItem>(919700, 919701, 919702, "FrameworkServices", nameof (WorkItemQueryFrameworkService), nameof (ValidateQuery), (Func<QueryHierarchyItem>) (() => requestContext.GetClient<WorkItemTrackingHttpClient>().CreateQueryAsync(postQuery, projectId, "My queries", new bool?(true)).Result));
    }
  }
}
