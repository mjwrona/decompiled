// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.QueryController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.Azure.Boards.WebApi.Common.Helpers;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [ValidateModel]
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "queries", ResourceVersion = 2)]
  public class QueryController : WorkItemTrackingApiController
  {
    private const int TraceRange = 5901000;

    public override string TraceArea => "queries";

    [TraceFilter(5901000, 5901010)]
    [HttpGet]
    [ClientLocationId("A67D190C-C41F-424B-814D-0E906F659301")]
    [ClientExample("GET__wit_queries__queryId_.json", "Query by ID", null, null)]
    [ClientExample("GET__wit_queries_Shared Queries__folderName___queryName_.json", "Query by Name", null, null)]
    [ClientExample("GET__wit_queries__folderId_.json", "Folder by ID", null, null)]
    [ClientExample("GET__wit_queries_Shared Queries__folderName_.json", "Folder by Name", null, null)]
    [ClientExample("GET__wit_queries__queryId___includeDeleted-true.json", "Deleted query by ID", null, null)]
    [ClientExample("GET__wit_queries__queryId___expand-clauses.json", "Flat query with expanded clauses", null, null)]
    [ClientExample("GET__wit_queries__hierarchicalQueryId___expand-clauses.json", "Hierarchical query with expanded clauses", null, null)]
    [ClientDebounce(true)]
    public QueryHierarchyItem GetQuery(
      string query,
      [FromUri(Name = "$expand")] QueryExpand expand = QueryExpand.None,
      [FromUri(Name = "$depth")] int depth = 0,
      [FromUri(Name = "$includeDeleted")] bool includeDeleted = false,
      [FromUri(Name = "$useIsoDateFormat")] bool useIsoDateFormat = false)
    {
      int maxGetQueriesDepth = this.WitRequestContext.ServerSettings.MaxGetQueriesDepth;
      if (depth < 0 || depth > maxGetQueriesDepth)
        throw new VssPropertyValidationException(nameof (depth), ResourceStrings.QueryParameterOutOfRangeWithRangeValues((object) nameof (depth), (object) 0, (object) maxGetQueriesDepth));
      if (string.IsNullOrEmpty(query))
        throw new VssPropertyValidationException(nameof (query), ResourceStrings.NullOrEmptyParameter((object) nameof (query)));
      WorkItemQueryControllerHelper.CheckForValidProject(this.TfsRequestContext, this.ProjectId, query);
      QueryResponseOptions options = QueryResponseOptions.Create(expand, this.ExcludeUrls, useIsoDateFormat);
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem query1 = this.QueryItemService.GetQuery(this.TfsRequestContext, this.ProjectId, query, new int?(depth), options.IncludeWiql, includeDeleted, true);
      if (WorkItemTrackingFeatureFlags.IsVisualStudio(this.TfsRequestContext))
        this.QueryItemService.StripOutCurrentIterationTeamParameter(this.TfsRequestContext, query1);
      this.LogCIForCrossQueryProjectId(query1, nameof (GetQuery));
      return QueryHierarchyItemFactory.Create(this.WitRequestContext, query1, false, options);
    }

    [TraceFilter(5901050, 5901060)]
    [HttpGet]
    [ClientLocationId("A67D190C-C41F-424B-814D-0E906F659301")]
    public QueryHierarchyItemsResult SearchQueries(
      [FromUri(Name = "$filter")] string filter,
      [FromUri(Name = "$top")] int top = 50,
      [FromUri(Name = "$expand")] QueryExpand expand = QueryExpand.None,
      [FromUri(Name = "$includeDeleted")] bool includeDeleted = false)
    {
      if (string.IsNullOrWhiteSpace(filter))
        throw new VssPropertyValidationException("$filter", ResourceStrings.NullOrEmptyParameter((object) "$filter"));
      if (top <= 0 || top > 200)
        throw new VssPropertyValidationException("$top", ResourceStrings.QueryParameterOutOfRange((object) "$top"));
      WorkItemQueryControllerHelper.CheckForValidProject(this.TfsRequestContext, this.ProjectId);
      QueryResponseOptions options = QueryResponseOptions.Create(expand, this.ExcludeUrls);
      IList<QueryHierarchyItem> source = QueryHierarchyItemFactory.Create(this.WitRequestContext, (IList<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>) this.QueryItemService.SearchQueries(this.TfsRequestContext, this.ProjectId, options.IncludeWiql, filter, top, includeDeleted, true).ToList<Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem>(), false, options);
      bool flag = source.Count > top;
      IList<QueryHierarchyItem> list = (IList<QueryHierarchyItem>) source.Take<QueryHierarchyItem>(top).ToList<QueryHierarchyItem>();
      return new QueryHierarchyItemsResult()
      {
        Count = list.Count,
        Value = (IEnumerable<QueryHierarchyItem>) list,
        HasMore = new bool?(flag)
      };
    }

    [TraceFilter(5901010, 5901020)]
    [HttpGet]
    [ClientLocationId("A67D190C-C41F-424B-814D-0E906F659301")]
    [ClientExample("GET__wit_queries__depth-1.json", null, null, null)]
    [ClientExample("GET__wit_queries.json", "Just the root query folders", null, null)]
    [ClientExample("GET__wit_queries__depth-1__expand-all.json", "With the query string and columns and sort options", null, null)]
    [ClientExample("GET__wit_queries__depth-2__includeDeleted-true.json", "Including deleted queries", null, null)]
    public IEnumerable<QueryHierarchyItem> GetQueries(
      [FromUri(Name = "$expand")] QueryExpand expand = QueryExpand.None,
      [FromUri(Name = "$depth")] int depth = 0,
      [FromUri(Name = "$includeDeleted")] bool includeDeleted = false)
    {
      IEnumerable<QueryHierarchyItem> queries = WorkItemQueryControllerHelper.GetQueries(this.TfsRequestContext, this.ProjectId, expand, depth, includeDeleted, this.ExcludeUrls);
      return queries == null ? (IEnumerable<QueryHierarchyItem>) null : (IEnumerable<QueryHierarchyItem>) queries.ToList<QueryHierarchyItem>();
    }

    [TraceFilter(5901020, 5901030)]
    [HttpPost]
    [ClientResponseType(typeof (QueryHierarchyItem), null, null)]
    [ClientLocationId("A67D190C-C41F-424B-814D-0E906F659301")]
    [ClientExample("POST__wit_queries_Shared Queries__folderName_.json", "Create a query", null, null)]
    [ClientExample("POST__wit_queries_Shared Queries.json", "Create a folder", null, null)]
    [ClientExample("POST__wit_queries_My Queries.json", "Move a query or folder", null, null)]
    public HttpResponseMessage CreateQuery(
      string query,
      [FromBody] QueryHierarchyItem postedQuery,
      [FromUri(Name = "validateWiqlOnly")] bool validateWiqlOnly = false)
    {
      if (string.IsNullOrEmpty(query))
        throw new VssPropertyValidationException(nameof (query), ResourceStrings.NullOrEmptyParameter((object) nameof (query)));
      if (postedQuery == null)
        throw new VssPropertyValidationException(nameof (postedQuery), ResourceStrings.NullQueryParameter());
      if (validateWiqlOnly && string.IsNullOrEmpty(postedQuery.Wiql))
        throw new VssPropertyValidationException("wiql", ResourceStrings.NullOrEmptyParameter((object) "wiql"));
      WorkItemQueryControllerHelper.CheckForValidProject(this.TfsRequestContext, this.ProjectId, query);
      string macrosUsed = string.Empty;
      Guid projectId = this.ProjectId;
      bool flag = postedQuery.Id == Guid.Empty;
      if (flag | validateWiqlOnly)
      {
        if (string.IsNullOrEmpty(postedQuery.Name) && !validateWiqlOnly)
          throw new VssPropertyValidationException("name", ResourceStrings.MissingQueryParameter((object) "Name"));
        bool? isFolder = postedQuery.IsFolder;
        if (isFolder.HasValue)
        {
          isFolder = postedQuery.IsFolder;
          if (isFolder.Value)
            goto label_13;
        }
        if (string.IsNullOrEmpty(postedQuery.Wiql))
          throw new VssPropertyValidationException("wiql", ResourceStrings.MissingQueryParameters((object) "Wiql", (object) "isFolder"));
label_13:
        if (!string.IsNullOrEmpty(postedQuery.Wiql))
          QueryUtils.ValidateQuery(postedQuery.Wiql, this.TfsRequestContext, this.ProjectId, this.Team?.Name, out macrosUsed);
      }
      if (validateWiqlOnly)
        return this.Request.CreateResponse(HttpStatusCode.OK);
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem query1 = this.GetQuery(projectId, query);
      this.LogCIForCrossQueryProjectId(query1, nameof (CreateQuery));
      if (!(query1 is QueryFolder))
        throw new VssPropertyValidationException(nameof (query), ResourceStrings.QueryInvalidParent((object) query));
      this.TfsRequestContext.Items["WIT.FromRest"] = (object) true;
      IDataAccessLayer dataAccessLayer = this.DataAccessLayer;
      Guid id;
      if (!flag)
      {
        id = postedQuery.Id;
        string name = postedQuery.Name;
        postedQuery = QueryHierarchyItemFactory.Create(this.WitRequestContext, this.QueryItemService.GetQueryById(this.TfsRequestContext, postedQuery.Id, new int?(), true, filterUnderProjectId: new Guid?(this.ProjectId)), false, QueryResponseOptions.Create(QueryExpand.None, this.ExcludeUrls));
        if (string.IsNullOrWhiteSpace(name))
          name = (string) null;
        dataAccessLayer.UpdateQueryItem(id, query1.Id, name, (string) null);
      }
      else
      {
        id = Guid.NewGuid();
        dataAccessLayer.CreateQueryItem(id, query1.Id, postedQuery.Name, postedQuery.Wiql);
      }
      QueryHierarchyItem query2 = this.GetQuery(id.ToString("D"), QueryExpand.All);
      HttpResponseMessage response = this.Request.CreateResponse<QueryHierarchyItem>(flag ? HttpStatusCode.Created : HttpStatusCode.OK, query2);
      response.Headers.Location = this.GetQueryUrl(query2, projectId);
      this.LogCIForMacrosUsed(query2.Id, macrosUsed, nameof (CreateQuery));
      return response;
    }

    [TraceFilter(5901030, 5901040)]
    [HttpPatch]
    [ClientResponseType(typeof (QueryHierarchyItem), null, null)]
    [ClientLocationId("A67D190C-C41F-424B-814D-0E906F659301")]
    [ClientExample("PATCH__wit_queries_Shared Queries__folderName___queryNameNew_.json", "Update a query", null, null)]
    [ClientExample("PATCH__wit_queries_Shared Queries__folderName_.json", "Rename a query", null, null)]
    [ClientExample("PATCH__wit_queries_Shared Queries__folderName_.json", "Rename a folder", null, null)]
    [ClientExample("PATCH__wit_queries__folderId___undeleteDescendants-true.json", "Undelete a query or folder", null, null)]
    public HttpResponseMessage UpdateQuery(
      string query,
      [FromBody] QueryHierarchyItem queryUpdate,
      [FromUri(Name = "$undeleteDescendants")] bool undeleteDescendants = false)
    {
      if (string.IsNullOrEmpty(query))
        throw new VssPropertyValidationException(nameof (query), ResourceStrings.NullOrEmptyParameter((object) nameof (query)));
      if (queryUpdate == null)
        throw new VssPropertyValidationException(nameof (queryUpdate), ResourceStrings.NullQueryParameter());
      string macrosUsed = string.Empty;
      WorkItemQueryControllerHelper.CheckForValidProject(this.TfsRequestContext, this.ProjectId, query);
      bool flag = false;
      this.TfsRequestContext.Items["WIT.FromRest"] = (object) true;
      Guid projectId = this.ProjectId;
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem query1 = this.GetQuery(projectId, query, true);
      this.LogCIForCrossQueryProjectId(query1, nameof (UpdateQuery));
      QueryResponseOptions options = QueryResponseOptions.Create(QueryExpand.None, this.ExcludeUrls);
      QueryHierarchyItem queryItem = QueryHierarchyItemFactory.Create(this.WitRequestContext, query1, false, options);
      if (queryUpdate.Id != Guid.Empty && queryUpdate.Id != query1.Id)
        throw new VssPropertyValidationException(nameof (query), ResourceStrings.IdMismatch((object) query1.Id, (object) queryUpdate.Id));
      if (!string.IsNullOrEmpty(queryUpdate.Name))
        queryItem.Name = queryUpdate.Name;
      if (!string.IsNullOrEmpty(queryUpdate.Wiql))
      {
        bool? isFolder = queryItem.IsFolder;
        if (isFolder.HasValue)
        {
          isFolder = queryItem.IsFolder;
          if (isFolder.Value)
            throw new VssPropertyValidationException("Wiql", ResourceStrings.QueryUpdateFolderConflict());
        }
        QueryUtils.ValidateQuery(queryUpdate.Wiql, this.TfsRequestContext, this.ProjectId, this.Team?.Name, out macrosUsed);
        queryItem.Wiql = queryUpdate.Wiql;
      }
      else
        queryItem.Wiql = (string) null;
      if (!queryUpdate.IsDeleted && query1.IsDeleted)
      {
        TelemetryHelper.PublishUndeleteQuery(this.TfsRequestContext, undeleteDescendants);
        flag |= undeleteDescendants;
        this.QueryItemService.UndeleteQueryItem(this.TfsRequestContext, query1.Id, undeleteDescendants);
      }
      Guid parentId = query1.ParentId;
      if (!string.IsNullOrEmpty(queryUpdate.Path) && !string.Equals(query1.Path, queryUpdate.Path, StringComparison.Ordinal))
      {
        int length = queryUpdate.Path.LastIndexOf('/');
        string path = queryUpdate.Path.Substring(0, length);
        if (!string.IsNullOrEmpty(queryUpdate.Name) && !queryUpdate.Path.EndsWith(queryUpdate.Name, StringComparison.Ordinal))
          throw new VssPropertyValidationException(query, ResourceStrings.NameAndPathInconsistent((object) query, (object) queryUpdate.Name, (object) queryUpdate.Path));
        queryItem.Name = queryUpdate.Path.Substring(length + 1, queryUpdate.Path.Length - length - 1);
        Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem queryByPath = this.QueryItemService.GetQueryByPath(this.TfsRequestContext, projectId, path, new int?(0), false);
        parentId = queryByPath != null && queryByPath is QueryFolder ? queryByPath.Id : throw new VssPropertyValidationException(nameof (query), ResourceStrings.QueryInvalidParent((object) query));
      }
      this.DataAccessLayer.UpdateQueryItem(queryItem.Id, parentId, queryItem.Name, queryItem.Wiql);
      QueryHierarchyItem query2 = this.GetQuery(queryItem.Id.ToString("D"), QueryExpand.All, flag ? this.WitRequestContext.ServerSettings.MaxGetQueriesDepth : 0);
      HttpResponseMessage response = this.Request.CreateResponse<QueryHierarchyItem>(HttpStatusCode.OK, query2);
      response.Headers.Location = this.GetQueryUrl(queryItem, projectId);
      this.LogCIForMacrosUsed(query2.Id, macrosUsed, nameof (UpdateQuery));
      return response;
    }

    [TraceFilter(5901040, 5901050)]
    [HttpDelete]
    [ClientLocationId("A67D190C-C41F-424B-814D-0E906F659301")]
    [ClientResponseType(typeof (void), null, null)]
    [ClientExample("DELETE__wit_queries__queryId_.json", "By ID", null, null)]
    [ClientExample("DELETE__wit_queries_My Queries__folderNameNew_.json", "By folder path", null, null)]
    public HttpResponseMessage DeleteQuery(string query)
    {
      if (string.IsNullOrEmpty(query))
        throw new VssPropertyValidationException(nameof (query), ResourceStrings.NullOrEmptyParameter((object) nameof (query)));
      WorkItemQueryControllerHelper.CheckForValidProject(this.TfsRequestContext, this.ProjectId, query);
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem query1 = this.GetQuery(this.ProjectId, query);
      Guid id = query1.Id;
      this.LogCIForCrossQueryProjectId(query1, nameof (DeleteQuery));
      this.TfsRequestContext.Items["WIT.FromRest"] = (object) false;
      this.DataAccessLayer.DeleteQueryItem(id);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    private Uri GetQueryUrl(QueryHierarchyItem queryItem, Guid projectId) => new Uri(queryItem.Url ?? WitUrlHelper.GetQueryUrl(this.WitRequestContext, projectId, queryItem.Id));

    private Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem GetQuery(
      Guid projectId,
      string queryReference,
      bool includeDeleted = false)
    {
      return this.QueryItemService.GetQuery(this.TfsRequestContext, projectId, queryReference, new int?(0), true, includeDeleted);
    }

    private Guid GetQueryId(Guid projectId, string queryReference) => this.GetQuery(projectId, queryReference).Id;

    private void LogCIForCrossQueryProjectId(Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItem query, string action)
    {
      if (this.ProjectId == Guid.Empty || !(query.ProjectId != this.ProjectId))
        return;
      CustomerIntelligenceService service = this.TfsRequestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("CrossProjectQueryUsage", true);
      intelligenceData.Add("RequestProjectId", (object) this.ProjectId);
      intelligenceData.Add("QueryProjectId", (object) query.ProjectId);
      intelligenceData.Add("Action", action);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feature = QueryTelemetry.Feature;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(tfsRequestContext, feature, "CrossProjectQueryUsage", properties);
    }

    private void LogCIForMacrosUsed(Guid queryId, string macrosUsed, string action)
    {
      CustomerIntelligenceService service = this.TfsRequestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("MacrosUsed", macrosUsed);
      intelligenceData.Add("Action", action);
      intelligenceData.Add("QueryId", (object) queryId);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string feature = QueryTelemetry.Feature;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(tfsRequestContext, feature, "MacrosUsed", properties);
    }
  }
}
