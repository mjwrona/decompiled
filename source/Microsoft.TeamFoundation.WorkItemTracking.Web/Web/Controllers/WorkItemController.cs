// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.Azure.Boards.WebApi.Common.Helpers;
using Microsoft.Azure.Devops.Work.PlatformServices.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [ValidateModel]
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "workItems", ResourceVersion = 2)]
  [ControllerApiVersion(1.0)]
  [ClientIgnoreRouteScopes(ClientRouteScopes.Project)]
  public class WorkItemController : WorkItemTrackingApiController
  {
    private const int TraceRange = 5904000;

    public override string TraceArea => "workItems";

    internal virtual bool UseLegacyLinkHandling { get; } = true;

    protected override bool ControllerSupportsIdentityRefForWorkItemFieldValues() => false;

    protected override bool ControllerSupportsProjectScopedUrls() => false;

    [TraceFilter(5904010, 5904020)]
    [HttpGet]
    [ClientExample("GET_work_item_template.json", "Returns a single work item from a template", null, null)]
    [ClientLocationId("62D3D110-0047-428C-AD3C-4FE872C91C74")]
    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem GetWorkItemTemplate(
      string type,
      string fields = null,
      DateTime? asOf = null,
      [FromUri(Name = "$expand")] WorkItemExpand expand = WorkItemExpand.None)
    {
      if (this.ProjectInfo == null)
        throw new VssPropertyValidationException("Project", ResourceStrings.ProjectNotFound());
      bool flag = this.ShouldUseIdentityRefForWorkItemFieldValues(this.TfsRequestContext);
      bool expandFields = expand == WorkItemExpand.All || expand == WorkItemExpand.Fields;
      return WorkItemFactory.Create(this.WitRequestContext, (WorkItemRevision) this.WorkItemService.GetWorkItemTemplate(this.TfsRequestContext, this.ProjectInfo.Name, type, flag), true, true, expandFields, false, false, (IEnumerable<string>) null, false, false, (IDictionary<Guid, IdentityReference>) null, flag, this.ShouldReturnProjectScopedUrls(this.TfsRequestContext), true);
    }

    [HttpGet]
    [TraceFilter(5904000, 5904010)]
    [ClientExample("GET__wit_WorkItems_id.json", "Get work item", null, null)]
    [ClientExample("GET__wit_WorkItems_id _asOf.json", "Get work item with asOf parameter", null, null)]
    [PublicProjectRequestRestrictions(false, false, "5.0")]
    [ClientTSProjectParameterPosition(4)]
    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem GetWorkItem(
      int id,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string fields = null,
      DateTime? asOf = null,
      [FromUri(Name = "$expand")] WorkItemExpand expand = WorkItemExpand.None)
    {
      return this.GetWorkItems(id.ToString(), fields, asOf, expand, true, projectId: this.GetNullableProjectId(), returnIdentityRef: this.ShouldUseIdentityRefForWorkItemFieldValues(this.TfsRequestContext)).FirstOrDefault<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>() ?? throw new HttpResponseException(this.Request.CreateResponse<string>(HttpStatusCode.NotFound, ResourceStrings.WorkItemNotFound((object) id.ToString())));
    }

    [TraceFilter(5904020, 5904030)]
    [HttpGet]
    [ClientExample("GET__wit_WorkItems_ids-_ids_.json", "Get list of work items", null, null)]
    [ClientExample("GET__wit_WorkItems_ids-_ids___expand-all.json", "Get list of work items, all expanded", null, null)]
    [ClientExample("GET__wit_WorkItems_ids-_ids__fields-_columns_.json", "Get list of work items for specific fields", null, null)]
    [ClientExample("GET__wit_WorkItems_ids-_ids__fields-_columns__asOf-_asof_.json", "Get list of work items as of specific datetime #1", null, null)]
    [ClientExample("GET__wit_WorkItems_ids-_ids__fields-_columns__asOf-_asof_2.json", "Get list of work items as of specific datetime #2", null, null)]
    [ClientExample("GET__wit_WorkItems_ids-_ids__fields-_columns__asOf-_asof_3.json", "Get list of work items as of specific datetime #3", null, null)]
    [ClientExample("GET__wit_WorkItems_ids-_ids__fields-_columns__asOf-_asof_4.json", "Get list of work items as of specific datetime #4", null, null)]
    [PublicProjectRequestRestrictions(false, false, "5.0")]
    [ClientTSProjectParameterPosition(5)]
    public IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> GetWorkItems(
      [ClientParameterAsIEnumerable(typeof (int), ',')] string ids,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string fields = null,
      DateTime? asOf = null,
      [FromUri(Name = "$expand")] WorkItemExpand expand = WorkItemExpand.None,
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemErrorPolicy errorPolicy = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemErrorPolicy.Fail)
    {
      bool includeLinks;
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemErrorPolicy serverErrorPolicy;
      WorkItemGetRequestProcessor.ProcessWorkItemOptions(expand, errorPolicy, out includeLinks, out serverErrorPolicy);
      return (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>) this.GetWorkItems(ids, fields, asOf, expand, includeLinks, serverErrorPolicy, this.GetNullableProjectId(), this.ShouldUseIdentityRefForWorkItemFieldValues(this.TfsRequestContext)).ToList<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>();
    }

    [HttpPatch]
    [ClientLocationId("72C7DDF8-2CDC-4F60-90CD-AB71C14A399B")]
    [ClientRequestContentMediaType("application/json-patch+json")]
    [RequestContentTypeRestriction(AllowJson = false)]
    [ClientExample("PATCH__wit_workitems__taskId_.json", "Update a field", null, null)]
    [ClientExample("PATCH__wit_workitems__taskId_10.json", "Update an identity field by IdentityRef", null, null)]
    [ClientExample("PATCH__wit_workitems__taskId_11.json", "Update an identity field by display name", null, null)]
    [ClientExample("PATCH__wit_workitems__taskId_12.json", "Update an identity field by distinct display name", null, null)]
    [ClientExample("PATCH__wit_workitems__taskId_13.json", "Reset an identity field", null, null)]
    [ClientExample("PATCH__wit_workitems__bug1Id_.json", "Move work items (API Availability: Team Services only (not TFS))", null, null)]
    [ClientExample("PATCH__wit_workitems__bug1Id_2.json", "Change work item type (API Availability: Team Services only (not TFS))", null, null)]
    [ClientExample("PATCH__wit_workitems__taskId_9.json", "Add a tag", null, null)]
    [ClientExample("PATCH__wit_workitems__taskId_14.json", "Update a tag", null, null)]
    [ClientExample("PATCH__wit_workitems__taskId_3.json", "Add a link", null, null)]
    [ClientExample("PATCH__wit_workitems__taskId_4.json", "Update a link", "The number in the path (e.g. /relations/[number]) is a zero based index of the relation which represents its order displayed on the work item of that specific revision.", null)]
    [ClientExample("PATCH__wit_workitems__taskId_5.json", "Remove a link", "The number in the path (e.g. /relations/[number]) is a zero based index of the relation which represents its order displayed on the work item of that specific revision.", null)]
    [ClientExample("PATCH__wit_workitems__taskId_6.json", "Add an attachment", null, null)]
    [ClientExample("PATCH__wit_workitems__taskId_7.json", "Remove an attachment", "The number in the path (e.g. /relations/[number]) is a zero based index of the relation which represents its order displayed on the work item of that specific revision.", null)]
    [ClientExample("PATCH__wit_workitems__taskId_8.json", "Add a hyperlink", null, null)]
    [ClientExample("PATCH__wit_workitems__taskId__bypassRules-true.json", "Make an update bypassing rules", null, null)]
    [ClientExample("PATCH__wit_workitems__taskId_validate_only.json", "Validate only update", null, null)]
    [ClientExample("PATCH__wit_workitems__taskId_validate_only.json", "Add a Markdown comment", null, null)]
    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem UpdateWorkItem(
      int id,
      [ClientParameterType(typeof (JsonPatchDocument), false)] PatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> document,
      [FromUri] bool validateOnly = false,
      [FromUri] bool bypassRules = false,
      [FromUri] bool suppressNotifications = false,
      [FromUri(Name = "$expand")] WorkItemExpand expand = WorkItemExpand.Relations)
    {
      bool useWorkItemIdentity = this.ShouldUseIdentityRefForWorkItemFieldValues(this.TfsRequestContext);
      bool returnProjectScopedUrl = this.ShouldReturnProjectScopedUrls(this.TfsRequestContext);
      return WorkItemUpdateHelper.UpdateWorkItem(this.TfsRequestContext, WitUpdateHelper.PrepareUpdateWorkItem(this.TfsRequestContext, id, document, useWorkItemIdentity), document, this.UseLegacyLinkHandling, validateOnly, bypassRules, suppressNotifications, !suppressNotifications, useWorkItemIdentity, returnProjectScopedUrl, expand);
    }

    [HttpPost]
    [ClientLocationId("62D3D110-0047-428C-AD3C-4FE872C91C74")]
    [ClientRequestContentMediaType("application/json-patch+json")]
    [RequestContentTypeRestriction(AllowJson = false)]
    [ClientExample("POST__wit_workitems_create.json", "Create work item", null, null)]
    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem CreateWorkItem(
      string type,
      [ClientParameterType(typeof (JsonPatchDocument), false)] PatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> document,
      [FromUri] bool validateOnly = false,
      [FromUri] bool bypassRules = false,
      [FromUri] bool suppressNotifications = false,
      [FromUri(Name = "$expand")] WorkItemExpand expand = WorkItemExpand.Relations)
    {
      if (this.ProjectInfo == null)
        throw new VssPropertyValidationException("Project", ResourceStrings.ProjectNotFound());
      bool useWorkItemIdentity = this.ShouldUseIdentityRefForWorkItemFieldValues(this.TfsRequestContext);
      bool returnProjectScopedUrl = this.ShouldReturnProjectScopedUrls(this.TfsRequestContext);
      return WorkItemUpdateHelper.UpdateWorkItem(this.TfsRequestContext, WitUpdateHelper.PrepareUpdateWorkItem(this.TfsRequestContext, this.ProjectInfo.Name, type, document, useWorkItemIdentity), document, this.UseLegacyLinkHandling, validateOnly, bypassRules, suppressNotifications, !suppressNotifications, useWorkItemIdentity, returnProjectScopedUrl, expand);
    }

    [HttpPatch]
    [ClientLocationId("62D3D110-0047-428C-AD3C-4FE872C91C74")]
    [ClientRequestContentMediaType("application/json-patch+json")]
    [RequestContentTypeRestriction(AllowJson = false)]
    [Obsolete("Use CreateWorkItem instead")]
    [ClientInternalUseOnly(true)]
    [ClientInclude(RestClientLanguages.TypeScript)]
    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem UpdateWorkItemTemplate(
      string type,
      [ClientParameterType(typeof (JsonPatchDocument), false)] PatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> document,
      [FromUri] bool validateOnly = false,
      [FromUri] bool bypassRules = false)
    {
      return this.CreateWorkItem(type, document, validateOnly, bypassRules);
    }

    [HttpDelete]
    [ClientLocationId("72C7DDF8-2CDC-4F60-90CD-AB71C14A399B")]
    [ClientResponseType(typeof (WorkItemDelete), null, null)]
    [ClientExample("DELETE__wit_workitems_id_.json", null, null, null)]
    public HttpResponseMessage DeleteWorkItem(int id, bool destroy = false)
    {
      this.CheckIfNonTestWorkItem(id);
      if (destroy)
      {
        this.WorkItemService.DestroyWorkItems(this.TfsRequestContext, (IEnumerable<int>) new int[1]
        {
          id
        });
        return new HttpResponseMessage(HttpStatusCode.NoContent);
      }
      WorkItemDelete internalResponse = WitDeleteHelper.GetWorkItemDeleteInternalResponse(this.TfsRequestContext, this.WorkItemService, this.WorkItemService.DeleteWorkItems(this.TfsRequestContext, (IEnumerable<int>) new int[1]
      {
        id
      }).FirstOrDefault<WorkItemUpdateResult>(), WorkItemRetrievalMode.Deleted, this.ShouldReturnProjectScopedUrls(this.TfsRequestContext));
      return internalResponse == null ? this.Request.CreateResponse(HttpStatusCode.NotFound) : this.Request.CreateResponse<WorkItemDelete>(HttpStatusCode.OK, internalResponse);
    }

    private void CheckIfNonTestWorkItem(int workItemId) => WitDeleteHelper.CheckIfNonTestWorkItem(this.TfsRequestContext, this.GetWorkItems(workItemId.ToString(), includeLinks: true).FirstOrDefault<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>());

    private IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> GetWorkItems(
      string ids,
      string fields = null,
      DateTime? asOf = null,
      WorkItemExpand expand = WorkItemExpand.None,
      bool includeLinks = false,
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemErrorPolicy errorPolicy = Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemErrorPolicy.Fail,
      Guid? projectId = null,
      bool returnIdentityRef = false)
    {
      ICollection<int> workItemIds = !string.IsNullOrEmpty(ids) ? ParsingHelper.ParseIds(ids) : throw new VssPropertyValidationException(nameof (ids), ResourceStrings.NullOrEmptyParameter((object) nameof (ids)));
      string[] commaSeparatedString = ParsingHelper.ParseCommaSeparatedString(fields);
      return WorkItemGetRequestProcessor.GetWorkItems(this.WitRequestContext, this.WorkItemService, this.ShouldReturnProjectScopedUrls(this.TfsRequestContext), workItemIds, (ICollection<string>) commaSeparatedString, asOf, expand, includeLinks, errorPolicy, projectId, returnIdentityRef);
    }
  }
}
