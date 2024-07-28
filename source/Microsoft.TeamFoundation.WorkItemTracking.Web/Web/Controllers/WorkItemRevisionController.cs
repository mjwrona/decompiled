// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemRevisionController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [ValidateModel]
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "revisions", ResourceVersion = 2)]
  [ControllerApiVersion(1.0)]
  [ClientIgnoreRouteScopes(ClientRouteScopes.Project)]
  public class WorkItemRevisionController : WorkItemTrackingApiController
  {
    private const int TraceRange = 5903000;

    public override string TraceArea => "revisions";

    protected override bool ControllerSupportsIdentityRefForWorkItemFieldValues() => false;

    protected override bool ControllerSupportsProjectScopedUrls() => false;

    [TraceFilter(5903000, 5903010)]
    [HttpGet]
    [ClientExample("GET__list_workitem_revisions.json", "List work item revisions", null, null)]
    [ClientExample("GET__list_workitem_revisions_paged.json", "List work item revisions with paging", null, null)]
    [ClientExample("GET__list_workitem_revisions_project_scope.json", "List work item revisions (project scoped)", null, null)]
    [ClientTSProjectParameterPosition(4)]
    public IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> GetRevisions(
      int id,
      [FromUri(Name = "$top")] int top = 200,
      [FromUri(Name = "$skip")] int skip = 0,
      [FromUri(Name = "$expand")] WorkItemExpand expand = WorkItemExpand.None)
    {
      if (top <= 0 || top > 200)
        throw new VssPropertyValidationException("$top", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.QueryParameterOutOfRange((object) "$top"));
      if (skip < 0)
        throw new VssPropertyValidationException("$skip", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.QueryParameterOutOfRange((object) "$skip"));
      bool useIdentityRef = this.ShouldUseIdentityRefForWorkItemFieldValues(this.TfsRequestContext);
      bool includeRelations = expand == WorkItemExpand.All || expand == WorkItemExpand.Relations;
      bool expandFields = expand == WorkItemExpand.All || expand == WorkItemExpand.Fields;
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItemById = this.WorkItemService.GetWorkItemById(this.TfsRequestContext, id, includeRelations, includeRelations, useWorkItemIdentity: useIdentityRef, projectId: this.GetNullableProjectId());
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> source = workItemById.Revisions.Concat<WorkItemRevision>((IEnumerable<WorkItemRevision>) new Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem[1]
      {
        workItemById
      }).Skip<WorkItemRevision>(skip).Take<WorkItemRevision>(top).Select<WorkItemRevision, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>((Func<WorkItemRevision, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>) (revision => WorkItemFactory.Create(this.WitRequestContext, revision, includeRelations, false, expandFields, true, false, (IEnumerable<string>) null, false, false, (IDictionary<Guid, IdentityReference>) null, useIdentityRef, this.ShouldReturnProjectScopedUrls(this.TfsRequestContext), true)));
      return source == null ? (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>) null : (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>) source.ToList<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>();
    }

    [TraceFilter(5903010, 5903020)]
    [HttpGet]
    [ClientExample("GET__workitem_revision.json", "Get work item revision", null, null)]
    [ClientExample("GET__workitem_revision_project_scope.json", "Get work item revision (project scoped)", null, null)]
    [ClientTSProjectParameterPosition(3)]
    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem GetRevision(
      int id,
      int revisionNumber,
      [FromUri(Name = "$expand")] WorkItemExpand expand = WorkItemExpand.None)
    {
      bool flag1 = this.ShouldUseIdentityRefForWorkItemFieldValues(this.TfsRequestContext);
      bool returnProjectScopedUrl = this.ShouldReturnProjectScopedUrls(this.TfsRequestContext);
      bool flag2 = expand == WorkItemExpand.All || expand == WorkItemExpand.Relations;
      bool expandFields = expand == WorkItemExpand.All || expand == WorkItemExpand.Fields;
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItemById = this.WorkItemService.GetWorkItemById(this.TfsRequestContext, id, flag2, flag2, useWorkItemIdentity: flag1, projectId: this.GetNullableProjectId());
      if (workItemById.Revision == revisionNumber)
        return WorkItemFactory.Create(this.WitRequestContext, (WorkItemRevision) workItemById, flag2, true, expandFields, true, false, (IEnumerable<string>) null, false, false, (IDictionary<Guid, IdentityReference>) null, flag1, returnProjectScopedUrl, true);
      return WorkItemFactory.Create(this.WitRequestContext, workItemById.Revisions.FirstOrDefault<WorkItemRevision>((Func<WorkItemRevision, bool>) (x => x.Revision == revisionNumber)) ?? throw new WitResourceNotFoundException(Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.RevisionNotFound((object) id, (object) revisionNumber)), flag2, true, expandFields, true, false, (IEnumerable<string>) null, false, false, (IDictionary<Guid, IdentityReference>) null, flag1, returnProjectScopedUrl, true);
    }
  }
}
