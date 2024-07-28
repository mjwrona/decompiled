// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.PlatformServices.PlatformWorkItemService
// Assembly: Microsoft.Azure.Devops.Work.PlatformServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7C8E511A-CB9A-4327-9803-A1164853E0F0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Work.PlatformServices.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.Azure.Boards.WebApi.Common.Helpers;
using Microsoft.Azure.Devops.Work.PlatformServices.Common;
using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Web;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Microsoft.Azure.Devops.Work.PlatformServices
{
  public class PlatformWorkItemService : IWorkItemRemotableService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem GetWorkItemTemplate(
      IVssRequestContext requestContext,
      Guid projectId,
      string type,
      WorkItemExpand expand = WorkItemExpand.None)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckStringForNullOrEmpty(type, nameof (type));
      ITeamFoundationWorkItemService service = requestContext.GetService<ITeamFoundationWorkItemService>();
      string projectName = requestContext.GetService<IProjectService>().GetProjectName(requestContext, projectId);
      return WorkItemFactory.Create(requestContext.WitContext(), (WorkItemRevision) service.GetWorkItemTemplate(requestContext, projectName, type, true), true, true, true, false, false, (IEnumerable<string>) null, false, false, (IDictionary<Guid, IdentityReference>) null, true, this.ShouldReturnProjectScopedUrls(requestContext), true);
    }

    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem GetWorkItemTemplate(
      IVssRequestContext requestContext,
      string projectName,
      string type,
      WorkItemExpand expand = WorkItemExpand.None)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      ArgumentUtility.CheckStringForNullOrEmpty(type, nameof (type));
      ITeamFoundationWorkItemService service = requestContext.GetService<ITeamFoundationWorkItemService>();
      bool expandFields = expand == WorkItemExpand.Fields || expand == WorkItemExpand.All;
      return WorkItemFactory.Create(requestContext.WitContext(), (WorkItemRevision) service.GetWorkItemTemplate(requestContext, projectName, type, true), true, true, expandFields, false, false, (IEnumerable<string>) null, false, false, (IDictionary<Guid, IdentityReference>) null, true, this.ShouldReturnProjectScopedUrls(requestContext), true);
    }

    public IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> GetWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      IEnumerable<string> fields = null,
      DateTime? asOf = null,
      WorkItemExpand expand = WorkItemExpand.None,
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemErrorPolicy errorPolicy = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemErrorPolicy.Fail)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<int>>(workItemIds, nameof (workItemIds));
      if (fields == null)
        fields = (IEnumerable<string>) Array.Empty<string>();
      ITeamFoundationWorkItemService service = requestContext.GetService<ITeamFoundationWorkItemService>();
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemErrorPolicy serverErrorPolicy;
      WorkItemGetRequestProcessor.ProcessWorkItemOptions(expand, errorPolicy, out bool _, out serverErrorPolicy);
      return WorkItemGetRequestProcessor.GetWorkItems(requestContext.WitContext(), service, this.ShouldReturnProjectScopedUrls(requestContext), (ICollection<int>) workItemIds.ToList<int>(), (ICollection<string>) fields.ToList<string>(), asOf, expand, true, serverErrorPolicy, new Guid?(), true, true);
    }

    public IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> GetWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      string projectName,
      IEnumerable<string> fields = null,
      DateTime? asOf = null,
      WorkItemExpand expand = WorkItemExpand.None,
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemErrorPolicy errorPolicy = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemErrorPolicy.Fail)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<int>>(workItemIds, nameof (workItemIds));
      Guid? projectId = new Guid?();
      if (!string.IsNullOrEmpty(projectName) && !string.IsNullOrWhiteSpace(projectName))
        projectId = new Guid?(requestContext.GetService<IProjectService>().GetProjectId(requestContext, projectName));
      if (fields == null)
        fields = (IEnumerable<string>) Array.Empty<string>();
      ITeamFoundationWorkItemService service = requestContext.GetService<ITeamFoundationWorkItemService>();
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemErrorPolicy serverErrorPolicy;
      WorkItemGetRequestProcessor.ProcessWorkItemOptions(expand, errorPolicy, out bool _, out serverErrorPolicy);
      return WorkItemGetRequestProcessor.GetWorkItems(requestContext.WitContext(), service, this.ShouldReturnProjectScopedUrls(requestContext), (ICollection<int>) workItemIds.ToList<int>(), (ICollection<string>) fields.ToList<string>(), asOf, expand, true, serverErrorPolicy, projectId, true, true);
    }

    public IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> GetWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      Guid projectId,
      IEnumerable<string> fields = null,
      DateTime? asOf = null,
      WorkItemExpand expand = WorkItemExpand.None,
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemErrorPolicy errorPolicy = Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemErrorPolicy.Fail)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<int>>(workItemIds, nameof (workItemIds));
      if (fields == null)
        fields = (IEnumerable<string>) Array.Empty<string>();
      ITeamFoundationWorkItemService service = requestContext.GetService<ITeamFoundationWorkItemService>();
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemErrorPolicy serverErrorPolicy;
      WorkItemGetRequestProcessor.ProcessWorkItemOptions(expand, errorPolicy, out bool _, out serverErrorPolicy);
      return WorkItemGetRequestProcessor.GetWorkItems(requestContext.WitContext(), service, this.ShouldReturnProjectScopedUrls(requestContext), (ICollection<int>) workItemIds.ToList<int>(), (ICollection<string>) fields.ToList<string>(), asOf, expand, true, serverErrorPolicy, new Guid?(projectId), true, true);
    }

    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem GetWorkItem(
      IVssRequestContext requestContext,
      int id,
      IEnumerable<string> fields = null,
      DateTime? asOf = null,
      WorkItemExpand expand = WorkItemExpand.None)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (fields == null)
        fields = (IEnumerable<string>) Array.Empty<string>();
      IVssRequestContext requestContext1 = requestContext;
      List<int> workItemIds = new List<int>();
      workItemIds.Add(id);
      IEnumerable<string> fields1 = fields;
      DateTime? asOf1 = asOf;
      int expand1 = (int) expand;
      return this.GetWorkItems(requestContext1, (IEnumerable<int>) workItemIds, fields1, asOf1, (WorkItemExpand) expand1, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemErrorPolicy.Fail).First<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>();
    }

    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem UpdateWorkItem(
      IVssRequestContext requestContext,
      int workItemId,
      JsonPatchDocument workItemPatchDocument,
      bool validateOnly = false,
      bool bypassRules = false,
      bool suppressNotifications = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<JsonPatchDocument>(workItemPatchDocument, nameof (workItemPatchDocument));
      bool useWorkItemIdentity = true;
      bool returnProjectScopedUrl = this.ShouldReturnProjectScopedUrls(requestContext);
      PatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> fromJson = PatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>.CreateFromJson(this.RemoveFieldUpdatesForGuid(requestContext, workItemPatchDocument));
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem serverWorkItem = WitUpdateHelper.PrepareUpdateWorkItem(requestContext, workItemId, fromJson, useWorkItemIdentity);
      return WorkItemUpdateHelper.UpdateWorkItem(requestContext, serverWorkItem, fromJson, false, validateOnly, bypassRules, suppressNotifications, !suppressNotifications, true, returnProjectScopedUrl);
    }

    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem CreateWorkItem(
      IVssRequestContext requestContext,
      string projectName,
      string workItemtype,
      JsonPatchDocument workItemDocument,
      bool validateOnly = false,
      bool bypassRules = false,
      bool suppressNotifications = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(projectName, nameof (projectName));
      ArgumentUtility.CheckForNull<JsonPatchDocument>(workItemDocument, nameof (workItemDocument));
      bool useWorkItemIdentity = true;
      bool returnProjectScopedUrl = this.ShouldReturnProjectScopedUrls(requestContext);
      PatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> fromJson = PatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>.CreateFromJson(this.RemoveFieldUpdatesForGuid(requestContext, workItemDocument));
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem serverWorkItem = WitUpdateHelper.PrepareUpdateWorkItem(requestContext, projectName, workItemtype, fromJson, useWorkItemIdentity);
      return WorkItemUpdateHelper.UpdateWorkItem(requestContext, serverWorkItem, fromJson, false, validateOnly, bypassRules, suppressNotifications, !suppressNotifications, useWorkItemIdentity, returnProjectScopedUrl);
    }

    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem CreateWorkItem(
      IVssRequestContext requestContext,
      Guid projectId,
      string workItemtype,
      JsonPatchDocument workItemDocument,
      bool validateOnly = false,
      bool bypassRules = false,
      bool suppressNotifications = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<JsonPatchDocument>(workItemDocument, nameof (workItemDocument));
      bool useWorkItemIdentity = true;
      bool returnProjectScopedUrl = this.ShouldReturnProjectScopedUrls(requestContext);
      PatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> fromJson = PatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>.CreateFromJson(this.RemoveFieldUpdatesForGuid(requestContext, workItemDocument));
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem serverWorkItem = WitUpdateHelper.PrepareUpdateWorkItem(requestContext, projectId.ToString(), workItemtype, fromJson, useWorkItemIdentity);
      return WorkItemUpdateHelper.UpdateWorkItem(requestContext, serverWorkItem, fromJson, false, validateOnly, bypassRules, suppressNotifications, !suppressNotifications, useWorkItemIdentity, returnProjectScopedUrl);
    }

    public IEnumerable<WorkItemDelete> DeleteWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<int>>(workItemIds, nameof (workItemIds));
      ITeamFoundationWorkItemService service = requestContext.GetService<ITeamFoundationWorkItemService>();
      IDictionary<int, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem> dictionary = (IDictionary<int, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem>) service.GetWorkItems(requestContext, workItemIds, includeHistory: false, workItemRetrievalMode: WorkItemRetrievalMode.All).ToDictionary<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem, int, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem, int>) (item => item.Id), (Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem>) (item => item));
      IEnumerable<WorkItemUpdateResult> itemUpdateResults = service.DeleteWorkItems(requestContext, workItemIds);
      List<WorkItemDelete> workItemDeleteList = new List<WorkItemDelete>();
      foreach (WorkItemUpdateResult result in itemUpdateResults)
      {
        Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItem;
        dictionary.TryGetValue(result.Id, out workItem);
        workItemDeleteList.Add(WitDeleteHelper.GetWorkItemDeleteResponse(requestContext, service, workItem, result, WorkItemRetrievalMode.Deleted, this.ShouldReturnProjectScopedUrls(requestContext)));
      }
      return (IEnumerable<WorkItemDelete>) workItemDeleteList;
    }

    public WorkItemDelete DeleteWorkItem(IVssRequestContext requestContext, int id, bool destroy = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ITeamFoundationWorkItemService service = requestContext.GetService<ITeamFoundationWorkItemService>();
      WorkItemDelete workItemDelete = (WorkItemDelete) null;
      if (destroy)
      {
        service.DestroyWorkItems(requestContext, (IEnumerable<int>) new int[1]
        {
          id
        });
      }
      else
      {
        Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItem = service.GetWorkItems(requestContext, (IEnumerable<int>) new int[1]
        {
          id
        }, workItemRetrievalMode: WorkItemRetrievalMode.All).FirstOrDefault<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem>();
        WorkItemUpdateResult result = service.DeleteWorkItems(requestContext, (IEnumerable<int>) new int[1]
        {
          id
        }).FirstOrDefault<WorkItemUpdateResult>();
        workItemDelete = WitDeleteHelper.GetWorkItemDeleteResponse(requestContext, service, workItem, result, WorkItemRetrievalMode.Deleted, this.ShouldReturnProjectScopedUrls(requestContext));
      }
      return workItemDelete;
    }

    internal bool ShouldReturnProjectScopedUrls(IVssRequestContext requestContext) => true;

    public IEnumerable<WitBatchResponse> CreateWorkItems(
      IVssRequestContext requestContext,
      string projectName,
      IList<CreateWitRequest> createWitRequests,
      bool bypassRules,
      bool suppressNotifications)
    {
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate> workItemUpdates = new List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate>();
      foreach (CreateWitRequest createWitRequest in (IEnumerable<CreateWitRequest>) createWitRequests)
      {
        PatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> patchDocument = this.ReadPatchDocument(requestContext, createWitRequest);
        Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem serverWorkItem = WitUpdateHelper.PrepareUpdateWorkItem(requestContext, projectName, createWitRequest.TypeName, patchDocument, true);
        Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate updateRequest = PlatformWorkItemService.CreateUpdateRequest(requestContext, createWitRequest.Id, workItemUpdates, patchDocument, serverWorkItem, true);
        workItemUpdates.Add(updateRequest);
      }
      List<WorkItemUpdateResult> updateResults = this.GetUpdateResults(requestContext, workItemUpdates, bypassRules, suppressNotifications, true);
      List<Tuple<int, WitBatchResponse>> workitemResponses = new List<Tuple<int, WitBatchResponse>>(updateResults.Count);
      for (int index = 0; index < updateResults.Count; ++index)
      {
        WorkItemUpdateResult itemUpdateResult = updateResults[index];
        CreateWitRequest createWitRequest = createWitRequests[index];
        if (itemUpdateResult.Exception != null)
        {
          HttpStatusCode httpStatusCode = new TfsApiController().MapException(WitUpdateHelper.TranslateUpdateResultException((Exception) itemUpdateResult.Exception, true));
          workitemResponses.Add(new Tuple<int, WitBatchResponse>(itemUpdateResult.Id, new WitBatchResponse()
          {
            Code = (int) httpStatusCode,
            Body = JsonConvert.SerializeObject((object) itemUpdateResult.Exception)
          }));
        }
        else
          workitemResponses.Add(new Tuple<int, WitBatchResponse>(itemUpdateResult.Id, (WitBatchResponse) null));
      }
      return this.GetWitBatchResponse(requestContext, (IList<Tuple<int, WitBatchResponse>>) workitemResponses);
    }

    public IEnumerable<WitBatchResponse> UpdateWorkItems(
      IVssRequestContext requestContext,
      IList<UpdateWitRequest> updateWitRequests,
      bool bypassRules,
      bool suppressNotifications)
    {
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate> workItemUpdates = new List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate>();
      foreach (UpdateWitRequest updateWitRequest in (IEnumerable<UpdateWitRequest>) updateWitRequests)
      {
        PatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> patchDocument = this.ReadPatchDocument(requestContext, updateWitRequest);
        Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem serverWorkItem = WitUpdateHelper.PrepareUpdateWorkItem(requestContext, updateWitRequest.Id, patchDocument, true);
        Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate updateRequest = PlatformWorkItemService.CreateUpdateRequest(requestContext, updateWitRequest.Id, workItemUpdates, patchDocument, serverWorkItem, true);
        workItemUpdates.Add(updateRequest);
      }
      List<WorkItemUpdateResult> updateResults = this.GetUpdateResults(requestContext, workItemUpdates, bypassRules, suppressNotifications, true);
      List<Tuple<int, WitBatchResponse>> workitemResponses = new List<Tuple<int, WitBatchResponse>>(updateResults.Count);
      for (int index = 0; index < updateResults.Count; ++index)
      {
        WorkItemUpdateResult itemUpdateResult = updateResults[index];
        UpdateWitRequest updateWitRequest = updateWitRequests[index];
        if (itemUpdateResult.Exception != null)
        {
          HttpStatusCode httpStatusCode = new TfsApiController().MapException(WitUpdateHelper.TranslateUpdateResultException((Exception) itemUpdateResult.Exception, true));
          workitemResponses.Add(new Tuple<int, WitBatchResponse>(itemUpdateResult.Id, new WitBatchResponse()
          {
            Code = (int) httpStatusCode,
            Body = JsonConvert.SerializeObject((object) itemUpdateResult.Exception)
          }));
        }
        else
          workitemResponses.Add(new Tuple<int, WitBatchResponse>(itemUpdateResult.Id, (WitBatchResponse) null));
      }
      return this.GetWitBatchResponse(requestContext, (IList<Tuple<int, WitBatchResponse>>) workitemResponses);
    }

    private List<WorkItemUpdateResult> GetUpdateResults(
      IVssRequestContext requestContext,
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate> workItemUpdates,
      bool bypassRules,
      bool suppressNotifications,
      bool useIdentityRef)
    {
      ITeamFoundationWorkItemService service = requestContext.GetService<ITeamFoundationWorkItemService>();
      WorkItemUpdateRuleExecutionMode ruleExecutionMode = bypassRules ? WorkItemUpdateRuleExecutionMode.Bypass : WorkItemUpdateRuleExecutionMode.Full;
      IVssRequestContext requestContext1 = requestContext;
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate> workItemUpdateList = workItemUpdates;
      int num1 = (int) ruleExecutionMode;
      bool flag = suppressNotifications;
      int num2 = !suppressNotifications ? 1 : 0;
      int num3 = flag ? 1 : 0;
      int num4 = bypassRules ? 1 : 0;
      int num5 = useIdentityRef ? 1 : 0;
      return service.UpdateWorkItems(requestContext1, (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate>) workItemUpdateList, (WorkItemUpdateRuleExecutionMode) num1, includeInRecentActivity: num2 != 0, suppressNotifications: num3 != 0, isPermissionCheckRequiredForBypassRules: num4 != 0, useWorkItemIdentity: num5 != 0).ToList<WorkItemUpdateResult>();
    }

    private IEnumerable<WitBatchResponse> GetWitBatchResponse(
      IVssRequestContext requestContext,
      IList<Tuple<int, WitBatchResponse>> workitemResponses)
    {
      IEnumerable<int> workItemIds = workitemResponses.Where<Tuple<int, WitBatchResponse>>((Func<Tuple<int, WitBatchResponse>, bool>) (wir => wir.Item2 == null)).Select<Tuple<int, WitBatchResponse>, int>((Func<Tuple<int, WitBatchResponse>, int>) (wir => wir.Item1));
      Dictionary<int?, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> dictionary = requestContext.GetService<IWorkItemRemotableService>().GetWorkItems(requestContext, workItemIds).ToDictionary<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem, int?>((Func<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem, int?>) (wi => wi.Id));
      for (int index = 0; index < workitemResponses.Count; ++index)
      {
        int num = workitemResponses[index].Item1;
        if (workitemResponses[index].Item2 == null)
          workitemResponses[index] = new Tuple<int, WitBatchResponse>(num, new WitBatchResponse()
          {
            Code = 200,
            Headers = new Dictionary<string, string>()
            {
              {
                "Content-Type",
                "application/json;charset=utf-8"
              }
            },
            Body = JsonConvert.SerializeObject((object) dictionary[new int?(num)])
          });
      }
      return workitemResponses.Select<Tuple<int, WitBatchResponse>, WitBatchResponse>((Func<Tuple<int, WitBatchResponse>, WitBatchResponse>) (witBatchResponse => witBatchResponse.Item2));
    }

    private static Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate CreateUpdateRequest(
      IVssRequestContext requestContext,
      int Id,
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate> workItemUpdates,
      PatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> patchDocument,
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem serverWorkItem,
      bool useIdentityRef)
    {
      WorkItemPatchDocument itemPatchDocument = new WorkItemPatchDocument(requestContext.WitContext(), false, (IPatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>) patchDocument, serverWorkItem, useIdentityRef);
      itemPatchDocument.Evaluate();
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate workItemUpdate = itemPatchDocument.GetWorkItemUpdate(requestContext.WitContext(), useIdentityRef);
      workItemUpdate.Id = Id;
      if (workItemUpdates.Any<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate, bool>) (u => u.Id == workItemUpdate.Id)))
        throw new PatchOperationFailedException(ResourceStrings.WorkItemPatchDocument_DuplicateWorkItemId((object) workItemUpdate.Id));
      return workItemUpdate;
    }

    private PatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> ReadPatchDocument(
      IVssRequestContext requestContext,
      CreateWitRequest request)
    {
      return PatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>.CreateFromJson(this.RemoveFieldUpdatesForGuid(requestContext, request.document));
    }

    private PatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> ReadPatchDocument(
      IVssRequestContext requestContext,
      UpdateWitRequest request)
    {
      return PatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>.CreateFromJson(this.RemoveFieldUpdatesForGuid(requestContext, request.document));
    }

    private JsonPatchDocument RemoveFieldUpdatesForGuid(
      IVssRequestContext requestContext,
      JsonPatchDocument patchDocument)
    {
      Dictionary<string, object> data = new Dictionary<string, object>();
      JsonPatchDocument jsonPatchDocument = new JsonPatchDocument();
      if (patchDocument == null)
        return (JsonPatchDocument) null;
      foreach (JsonPatchOperation jsonPatchOperation in (List<JsonPatchOperation>) patchDocument)
      {
        if ((jsonPatchOperation != null ? (jsonPatchOperation.Path.StartsWith("/fields", StringComparison.InvariantCultureIgnoreCase) ? 1 : 0) : 0) != 0 && jsonPatchOperation.Value is Guid)
          data[jsonPatchOperation.Path] = (object) jsonPatchOperation.Value.ToString();
        else
          jsonPatchDocument.Add(jsonPatchOperation);
      }
      if (data.Count > 0 && requestContext != null)
        requestContext.GetService<CustomerIntelligenceService>()?.Publish(requestContext, "platformWorkItemService", "UpdateFields", new CustomerIntelligenceData((IDictionary<string, object>) data));
      return jsonPatchDocument;
    }
  }
}
