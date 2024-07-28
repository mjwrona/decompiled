// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessPickListController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [FeatureEnabled("WebAccess.Process.Hierarchy")]
  [VersionedApiControllerCustomName(Area = "processes", ResourceName = "lists", ResourceVersion = 1)]
  public class ProcessPickListController : WorkItemTrackingApiController
  {
    [HttpGet]
    [ClientExample("GET__list.json", "Get list metadata", null, null)]
    public PickList GetList(Guid listId) => !(listId == Guid.Empty) ? PickListModelFactory.Create(this.TfsRequestContext, this.TfsRequestContext.GetService<IWorkItemPickListService>().GetList(this.TfsRequestContext, listId)) : throw new VssPropertyValidationException(nameof (listId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (listId)));

    [HttpGet]
    [ClientExample("GET__lists.json", "Get lists metadata", null, null)]
    public IReadOnlyCollection<PickListMetadata> GetListsMetadata() => (IReadOnlyCollection<PickListMetadata>) this.TfsRequestContext.GetService<IWorkItemPickListService>().GetListsMetadata(this.TfsRequestContext).Select<WorkItemPickListMetadata, PickListMetadata>((Func<WorkItemPickListMetadata, PickListMetadata>) (item => PickListMetadataModelFactory.Create(this.TfsRequestContext, item))).ToList<PickListMetadata>();

    [HttpPost]
    [ClientResponseType(typeof (PickList), null, null)]
    [ClientExample("POST__list.json", "Create the picklist", null, null)]
    public HttpResponseMessage CreateList([FromBody] PickList picklist)
    {
      if (picklist == null)
        throw new VssPropertyValidationException(nameof (picklist), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (picklist)));
      if (string.IsNullOrWhiteSpace(picklist.Name))
        throw new VssPropertyValidationException("Name", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) "Name"));
      if (string.IsNullOrWhiteSpace(picklist.Type))
        throw new VssPropertyValidationException("Type", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) "Type"));
      if (picklist.Items == null || !picklist.Items.Any<string>())
        throw new VssPropertyValidationException("Items", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) "Items"));
      if (picklist.Items.Any<string>((Func<string, bool>) (i => string.IsNullOrWhiteSpace(i))))
        throw new VssPropertyValidationException("Items", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) "value"));
      WorkItemPickList list = this.TfsRequestContext.GetService<IWorkItemPickListService>().CreateList(this.TfsRequestContext, picklist.Name, PickListHelper.GetPickListType(picklist.Type), (IReadOnlyList<string>) picklist.Items.ToList<string>(), picklist.IsSuggested);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string actionId = ProcessAuditConstants.GetActionId("List", "Create");
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("PicklistName", (object) picklist.Name);
      Guid targetHostId = new Guid();
      Guid projectId = new Guid();
      tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      return this.Request.CreateResponse<PickList>(HttpStatusCode.Created, PickListModelFactory.Create(this.TfsRequestContext, list));
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientExample("DELETE__list.json", "Delete the picklist", null, null)]
    public HttpResponseMessage DeleteList(Guid listId)
    {
      if (listId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (listId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (listId)));
      IWorkItemPickListService service = this.TfsRequestContext.GetService<IWorkItemPickListService>();
      WorkItemPickList picklist;
      bool list = service.TryGetList(this.TfsRequestContext, listId, out picklist);
      service.DeleteList(this.TfsRequestContext, listId);
      if (list)
      {
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        string actionId = ProcessAuditConstants.GetActionId("List", "Delete");
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("PicklistName", (object) picklist.Name);
        Guid targetHostId = new Guid();
        Guid projectId = new Guid();
        tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      }
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    [HttpPut]
    [ClientResponseType(typeof (PickList), null, null)]
    [ClientExample("PUT__list.json", "Update the picklist", null, null)]
    public HttpResponseMessage UpdateList(Guid listId, [FromBody] PickList picklist)
    {
      if (listId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (listId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (listId)));
      if (picklist == null)
        throw new VssPropertyValidationException(nameof (picklist), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) nameof (picklist)));
      if (picklist.Items == null)
        picklist.Items = (IList<string>) new List<string>(0);
      if (picklist.Items.Any<string>((Func<string, bool>) (i => string.IsNullOrWhiteSpace(i))))
        throw new VssPropertyValidationException("Items", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.NullOrEmptyParameter((object) "value"));
      if (picklist.Id != new Guid() && picklist.Id != listId)
        throw new VssPropertyValidationException(nameof (listId), Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.IdMismatch((object) listId, (object) picklist.Id));
      IWorkItemPickListService service = this.TfsRequestContext.GetService<IWorkItemPickListService>();
      if (!string.IsNullOrWhiteSpace(picklist.Type))
      {
        WorkItemPickListType pickListType = PickListHelper.GetPickListType(picklist.Type);
        WorkItemPickList picklist1;
        if (service.TryGetList(this.TfsRequestContext, listId, out picklist1) && picklist1.Type != pickListType)
          throw new VssPropertyValidationException("Type", Microsoft.TeamFoundation.WorkItemTracking.Web.ResourceStrings.InvalidPickListType());
      }
      WorkItemPickList list = service.UpdateList(this.TfsRequestContext, listId, picklist.Name, (IReadOnlyList<string>) picklist.Items.ToList<string>(), picklist.IsSuggested);
      if (!picklist.Name.IsNullOrEmpty<char>())
      {
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        string actionId = ProcessAuditConstants.GetActionId("List", "Update");
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("PicklistName", (object) picklist.Name);
        Guid targetHostId = new Guid();
        Guid projectId = new Guid();
        tfsRequestContext.LogAuditEvent(actionId, data, targetHostId, projectId);
      }
      return this.Request.CreateResponse<PickList>(HttpStatusCode.OK, PickListModelFactory.Create(this.TfsRequestContext, list));
    }
  }
}
