// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.ITeamFoundationQueryItemService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems
{
  [DefaultServiceImplementation(typeof (TeamFoundationQueryItemService))]
  public interface ITeamFoundationQueryItemService : IVssFrameworkService
  {
    QueryItem GetQuery(
      IVssRequestContext requestContext,
      Guid projectId,
      string queryReference,
      int? expandDepth,
      bool includeWiql,
      bool includeDeleted = false,
      bool includeExecutionInfo = false);

    QueryItem GetQueryById(
      IVssRequestContext requestContext,
      Guid id,
      int? expandDepth,
      bool includeWiql,
      bool includeDeleted = false,
      bool includeExecutionInfo = false,
      Guid? filterUnderProjectId = null);

    QueryItem GetQueryByPath(
      IVssRequestContext requestContext,
      Guid projectId,
      string path,
      int? expandDepth,
      bool includeWiql,
      bool includeDeleted = false,
      bool includeExecutionInfo = false);

    IEnumerable<QueryItem> GetRootQueries(
      IVssRequestContext requestContext,
      Guid projectId,
      int? expandDepth,
      bool includeWiql,
      bool includeDeleted = false,
      bool includeExecutionInfo = false);

    QueryItem UndeleteQueryItem(
      IVssRequestContext requestContext,
      Guid queryId,
      bool undeleteDescendants);

    IEnumerable<QueryItem> GetQueriesById(
      IVssRequestContext requestContext,
      IEnumerable<Guid> ids,
      int? expandDepth,
      bool includeWiql,
      bool includeDeleted = false,
      bool includeExecutionInfo = false,
      Guid? filterUnderProjectId = null);

    IEnumerable<QueryItem> GetDeletedQueryItems(
      IVssRequestContext requestContext,
      Guid projectId,
      int maxCount);

    QueryFolder[] GetQueryHierarchy(IVssRequestContext requestContext, Guid projectId);

    IEnumerable<QueryItem> SearchQueries(
      IVssRequestContext requestContext,
      Guid projectId,
      bool includeWiql,
      string searchText,
      int maxCount,
      bool includeDeleted = false,
      bool includeExecutionInfo = false);

    IEnumerable<QueryItem> GetQueriesExceedingMaxWiqlLength(IVssRequestContext requestContext);

    void DeleteQueriesExceedingMaxWiqlLength(IVssRequestContext requestContext);

    void StripOutCurrentIterationTeamParameter(
      IVssRequestContext requestContext,
      QueryItem queryItem);

    void StripOutCurrentIterationTeamParameter(
      IVssRequestContext requestContext,
      IEnumerable<QueryItem> queryItems);
  }
}
