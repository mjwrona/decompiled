// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.RecycleBinCommonController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.Azure.Boards.WebApi.Common.Helpers;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
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
  public abstract class RecycleBinCommonController : WorkItemTrackingApiController
  {
    [HttpGet]
    [ClientLocationId("B70D8D39-926C-465E-B927-B1BF0E5CA0E0")]
    [ClientExample("GET__wit_recyclebin__id_.json", null, null, null)]
    public virtual WorkItemDelete GetDeletedWorkItem(int id) => this.GetDeletedWorkItemsInternal((IEnumerable<int>) new int[1]
    {
      id
    }).FirstOrDefault<WorkItemDelete>();

    [HttpGet]
    [ClientLocationId("B70D8D39-926C-465E-B927-B1BF0E5CA0E0")]
    [ClientExample("GET__wit_recyclebin__ids_.json", null, null, null)]
    public virtual IEnumerable<WorkItemDeleteReference> GetDeletedWorkItems([ClientParameterAsIEnumerable(typeof (int), ',')] string ids)
    {
      IEnumerable<WorkItemDeleteReference> referenceInternal = this.GetDeletedWorkItemReferenceInternal((IEnumerable<int>) ParsingHelper.ParseIds(ids));
      return referenceInternal == null ? (IEnumerable<WorkItemDeleteReference>) null : (IEnumerable<WorkItemDeleteReference>) referenceInternal.ToList<WorkItemDeleteReference>();
    }

    [HttpPatch]
    [ClientResponseType(typeof (WorkItemDelete), null, null)]
    [ClientExample("PATCH__wit_recyclebin_restore.json", "Restore a work item", null, null)]
    public virtual HttpResponseMessage RestoreWorkItem(int id, [FromBody] WorkItemDeleteUpdate payload)
    {
      if (payload == null)
        throw new VssPropertyValidationException(nameof (payload), ResourceStrings.NullQueryParameter());
      if (payload.IsDeleted)
        return this.Request.CreateResponse(HttpStatusCode.NoContent);
      WorkItemDelete internalResponse = WitDeleteHelper.GetWorkItemDeleteInternalResponse(this.TfsRequestContext, this.WorkItemService, this.WorkItemService.RestoreWorkItems(this.TfsRequestContext, (IEnumerable<int>) new int[1]
      {
        id
      }).FirstOrDefault<WorkItemUpdateResult>(), WorkItemRetrievalMode.NonDeleted, this.ShouldReturnProjectScopedUrls(this.TfsRequestContext));
      return internalResponse == null ? this.Request.CreateResponse(HttpStatusCode.NotFound) : this.Request.CreateResponse<WorkItemDelete>(HttpStatusCode.OK, internalResponse);
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientExample("DELETE__wit_recyclebin__id_.json", null, null, null)]
    public virtual HttpResponseMessage DestroyWorkItem(int id)
    {
      int[] numArray = new int[1]{ id };
      RecycleBinCommonController.VerifyItemsAreSoftDeleted(this.TfsRequestContext, this.WorkItemService, (IEnumerable<int>) numArray);
      this.WorkItemService.DestroyWorkItems(this.TfsRequestContext, (IEnumerable<int>) numArray);
      return new HttpResponseMessage(HttpStatusCode.NoContent);
    }

    private IEnumerable<WorkItemDeleteReference> GetDeletedWorkItemReferenceInternal(
      IEnumerable<int> ids)
    {
      List<WorkItemDeleteReference> referenceInternal = new List<WorkItemDeleteReference>();
      int count1 = 200;
      for (int count2 = 0; count2 < ids.Count<int>(); count2 += count1)
      {
        IEnumerable<int> workItemIds = ids.Skip<int>(count2).Take<int>(count1);
        Dictionary<int, WorkItemFieldData> dictionary = this.WorkItemService.GetWorkItemFieldValues(this.TfsRequestContext, workItemIds, (IEnumerable<string>) WitDeleteHelper.WorkItemDeleteDefaultFields, workItemRetrievalMode: WorkItemRetrievalMode.Deleted).ToDictionary<WorkItemFieldData, int>((Func<WorkItemFieldData, int>) (fv => fv.Id));
        foreach (int num in workItemIds)
        {
          WorkItemFieldData workItem;
          if (!dictionary.TryGetValue(num, out workItem))
            throw new WorkItemUnauthorizedAccessException(num, Microsoft.TeamFoundation.WorkItemTracking.Server.AccessType.Read);
          referenceInternal.Add(WorkItemDeleteReferenceFactory.Create(this.WitRequestContext, workItem, this.ShouldReturnProjectScopedUrls(this.TfsRequestContext)));
        }
      }
      return (IEnumerable<WorkItemDeleteReference>) referenceInternal;
    }

    private IEnumerable<WorkItemDelete> GetDeletedWorkItemsInternal(IEnumerable<int> ids)
    {
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem> deletedWorkItems = this.WorkItemService.GetDeletedWorkItems(this.TfsRequestContext, ids);
      List<WorkItemDelete> workItemsInternal = new List<WorkItemDelete>();
      foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItem in deletedWorkItems)
        workItemsInternal.Add(WorkItemDeleteFactory.Create(this.WitRequestContext, (WorkItemRevision) workItem, this.ShouldReturnProjectScopedUrls(this.TfsRequestContext)));
      return (IEnumerable<WorkItemDelete>) workItemsInternal;
    }

    protected virtual IEnumerable<int> GetWorkItemIdsFromRecycleBin() => this.QueryService.ExecuteRecycleBinQuery(this.TfsRequestContext, this.ProjectId).WorkItemIds;

    public static void VerifyItemsAreSoftDeleted(
      IVssRequestContext requestContext,
      ITeamFoundationWorkItemService workItemService,
      IEnumerable<int> ids)
    {
      Dictionary<int, WorkItemFieldData> dictionary = workItemService.GetWorkItemFieldValues(requestContext, ids, (IEnumerable<int>) new int[2]
      {
        -3,
        -404
      }, workItemRetrievalMode: WorkItemRetrievalMode.All).ToDictionary<WorkItemFieldData, int>((Func<WorkItemFieldData, int>) (fv => fv.Id));
      List<int> intList = new List<int>();
      foreach (int id in ids)
      {
        WorkItemFieldData workItemFieldData;
        if (!dictionary.TryGetValue(id, out workItemFieldData) || !workItemFieldData.IsDeleted)
          intList.Add(id);
      }
      if (intList.Any<int>())
        throw new WorkItemInvalidDestroyRequestException((IEnumerable<int>) intList);
    }
  }
}
