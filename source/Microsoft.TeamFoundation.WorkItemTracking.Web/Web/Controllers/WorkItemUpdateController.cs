// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemUpdateController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "updates", ResourceVersion = 2)]
  [ControllerApiVersion(1.0)]
  [ClientIgnoreRouteScopes(ClientRouteScopes.Project)]
  public class WorkItemUpdateController : WorkItemTrackingApiController
  {
    protected const int TraceRange = 5905000;

    public override string TraceArea => "updates";

    protected override bool ControllerSupportsIdentityRefForWorkItemFieldValues() => false;

    protected override bool ControllerSupportsProjectScopedUrls() => false;

    [TraceFilter(5905000, 5905010)]
    [HttpGet]
    [ClientTSProjectParameterPosition(3)]
    [ClientExample("GET__list_of_workitem_updates.json", "List of work item updates", null, null)]
    [ClientExample("GET__list_of_workitem_updates_paged.json", "Paged list of work item updates", null, null)]
    [ClientExample("GET__list_of_workitem_updates_projectscope.json", "List of work item updates (project scoped)", null, null)]
    public IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate> GetUpdates(
      int id,
      [FromUri(Name = "$top")] int top = 200,
      [FromUri(Name = "$skip")] int skip = 0)
    {
      if (top <= 0 || top > 200)
        throw new VssPropertyValidationException("$top", ResourceStrings.QueryParameterOutOfRange((object) "$top"));
      if (skip < 0)
        throw new VssPropertyValidationException("$skip", ResourceStrings.QueryParameterOutOfRange((object) "$skip"));
      bool useWorkItemIdentity = this.ShouldUseIdentityRefForWorkItemFieldValues(this.TfsRequestContext);
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItemById = this.WorkItemService.GetWorkItemById(this.TfsRequestContext, id, includeInRecentActivity: true, useWorkItemIdentity: useWorkItemIdentity, projectId: this.GetNullableProjectId());
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate> source = WorkItemUpdateFactory.Create(new WorkItemUpdateCreateParams()
      {
        WitRequestContext = this.WitRequestContext,
        WorkItem = workItemById,
        IncludeLinks = false,
        ReturnIdentityRef = useWorkItemIdentity,
        ReturnProjectScopedUrl = this.ShouldReturnProjectScopedUrls(this.TfsRequestContext)
      }).Skip<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate>(skip).Take<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate>(top);
      List<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate> list1 = source != null ? source.ToList<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate>() : (List<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate>) null;
      if (WorkItemTrackingFeatureFlags.IsDisabledLinksOfDeletedAttachments(this.TfsRequestContext))
      {
        HashSet<WorkItemRelation> deletedAttachments = list1.SelectMany<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate, WorkItemRelation>((Func<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate, IEnumerable<WorkItemRelation>>) (f => f.Relations?.Removed ?? Enumerable.Empty<WorkItemRelation>())).Where<WorkItemRelation>((Func<WorkItemRelation, bool>) (f => f.Rel == "AttachedFile")).ToHashSet<WorkItemRelation>();
        List<WorkItemRelation> list2 = list1.SelectMany<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate, WorkItemRelation>((Func<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate, IEnumerable<WorkItemRelation>>) (f => f.Relations?.Added ?? Enumerable.Empty<WorkItemRelation>())).Where<WorkItemRelation>((Func<WorkItemRelation, bool>) (f => f.Rel == "AttachedFile" && deletedAttachments.Any<WorkItemRelation>((Func<WorkItemRelation, bool>) (a => a.Url == f.Url)))).ToList<WorkItemRelation>();
        List<WorkItemRelation> list3 = list1.SelectMany<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate, WorkItemRelation>((Func<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate, IEnumerable<WorkItemRelation>>) (f => f.Relations?.Updated ?? Enumerable.Empty<WorkItemRelation>())).Where<WorkItemRelation>((Func<WorkItemRelation, bool>) (f => f.Rel == "AttachedFile" && deletedAttachments.Any<WorkItemRelation>((Func<WorkItemRelation, bool>) (a => a.Url == f.Url)))).ToList<WorkItemRelation>();
        foreach (Link link in deletedAttachments)
          link.Url = (string) null;
        foreach (Link link in list2)
          link.Url = (string) null;
        foreach (Link link in list3)
          link.Url = (string) null;
      }
      return (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate>) list1;
    }

    [TraceFilter(5905010, 5905020)]
    [HttpGet]
    [ClientTSProjectParameterPosition(2)]
    [ClientExample("GET__work_item_update.json", "work item update", null, null)]
    [ClientExample("GET__work_item_update_project_scope.json", "work item update (project scoped)", null, null)]
    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate GetUpdate(
      int id,
      int updateNumber)
    {
      if (updateNumber < 1)
        throw new VssPropertyValidationException(nameof (updateNumber), ResourceStrings.QueryParameterOutOfRange((object) nameof (updateNumber)));
      bool useWorkItemIdentity = this.ShouldUseIdentityRefForWorkItemFieldValues(this.TfsRequestContext);
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItemById = this.WorkItemService.GetWorkItemById(this.TfsRequestContext, id, includeInRecentActivity: true, useWorkItemIdentity: useWorkItemIdentity, projectId: this.GetNullableProjectId());
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate> source = WorkItemUpdateFactory.Create(new WorkItemUpdateCreateParams()
      {
        WitRequestContext = this.WitRequestContext,
        WorkItem = workItemById,
        IncludeLinks = true,
        ReturnIdentityRef = useWorkItemIdentity,
        ReturnProjectScopedUrl = this.ShouldReturnProjectScopedUrls(this.TfsRequestContext)
      });
      return updateNumber <= source.Count<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate>() ? source.ElementAt<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemUpdate>(updateNumber - 1) : throw new VssPropertyValidationException(nameof (updateNumber), ResourceStrings.QueryParameterOutOfRange((object) nameof (updateNumber)));
    }
  }
}
