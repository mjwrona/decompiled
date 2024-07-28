// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemPickListController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ProcessDefinitions.Factories;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "processDefinitions", ResourceName = "lists", ResourceVersion = 1)]
  [FeatureEnabled("WebAccess.Process.Hierarchy")]
  public class WorkItemPickListController : WorkItemTrackingApiController
  {
    [HttpGet]
    [ClientLocationId("0B6179E2-23CE-46B2-B094-2FFA5EE70286")]
    public PickListModel GetList(Guid listId) => !(listId == Guid.Empty) ? PickListModelFactory.Create(this.TfsRequestContext, this.TfsRequestContext.GetService<IWorkItemPickListService>().GetList(this.TfsRequestContext, listId)) : throw new VssPropertyValidationException(nameof (listId), ResourceStrings.NullOrEmptyParameter((object) nameof (listId)));

    [HttpGet]
    public IReadOnlyCollection<PickListMetadataModel> GetListsMetadata() => (IReadOnlyCollection<PickListMetadataModel>) this.TfsRequestContext.GetService<IWorkItemPickListService>().GetListsMetadata(this.TfsRequestContext).Select<WorkItemPickListMetadata, PickListMetadataModel>((Func<WorkItemPickListMetadata, PickListMetadataModel>) (item => PickListMetadataModelFactory.Create(this.TfsRequestContext, item))).ToList<PickListMetadataModel>();

    [HttpPost]
    [ClientResponseType(typeof (PickListModel), null, null)]
    [ClientLocationId("0B6179E2-23CE-46B2-B094-2FFA5EE70286")]
    public HttpResponseMessage CreateList([FromBody] PickListModel picklist)
    {
      if (picklist == null)
        throw new VssPropertyValidationException(nameof (picklist), ResourceStrings.NullOrEmptyParameter((object) nameof (picklist)));
      if (string.IsNullOrWhiteSpace(picklist.Name))
        throw new VssPropertyValidationException("Name", ResourceStrings.NullOrEmptyParameter((object) "Name"));
      if (string.IsNullOrWhiteSpace(picklist.Type))
        throw new VssPropertyValidationException("Type", ResourceStrings.NullOrEmptyParameter((object) "Type"));
      if (picklist.Items == null || !picklist.Items.Any<PickListItemModel>())
        throw new VssPropertyValidationException("Items", ResourceStrings.NullOrEmptyParameter((object) "Items"));
      if (picklist.Items.Any<PickListItemModel>((Func<PickListItemModel, bool>) (i => string.IsNullOrWhiteSpace(i.Value))))
        throw new VssPropertyValidationException("Items", ResourceStrings.NullOrEmptyParameter((object) "value"));
      return this.Request.CreateResponse<PickListModel>(HttpStatusCode.Created, PickListModelFactory.Create(this.TfsRequestContext, this.TfsRequestContext.GetService<IWorkItemPickListService>().CreateList(this.TfsRequestContext, picklist.Name, this.GetPickListType(picklist.Type), (IReadOnlyList<string>) picklist.Items.Select<PickListItemModel, string>((Func<PickListItemModel, string>) (i => i.Value)).ToList<string>(), picklist.IsSuggested)));
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("0B6179E2-23CE-46B2-B094-2FFA5EE70286")]
    public HttpResponseMessage DeleteList(Guid listId)
    {
      if (listId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (listId), ResourceStrings.NullOrEmptyParameter((object) nameof (listId)));
      this.TfsRequestContext.GetService<IWorkItemPickListService>().DeleteList(this.TfsRequestContext, listId);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    [HttpPut]
    [ClientResponseType(typeof (PickListModel), null, null)]
    [ClientLocationId("0B6179E2-23CE-46B2-B094-2FFA5EE70286")]
    public HttpResponseMessage UpdateList(Guid listId, [FromBody] PickListModel picklist)
    {
      if (listId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (listId), ResourceStrings.NullOrEmptyParameter((object) nameof (listId)));
      if (picklist == null)
        throw new VssPropertyValidationException(nameof (picklist), ResourceStrings.NullOrEmptyParameter((object) nameof (picklist)));
      if (picklist.Items == null)
        picklist.Items = (IList<PickListItemModel>) new List<PickListItemModel>(0);
      if (picklist.Items.Any<PickListItemModel>((Func<PickListItemModel, bool>) (i => string.IsNullOrWhiteSpace(i.Value))))
        throw new VssPropertyValidationException("Items", ResourceStrings.NullOrEmptyParameter((object) "value"));
      if (picklist.Id != new Guid() && picklist.Id != listId)
        throw new VssPropertyValidationException(nameof (listId), ResourceStrings.IdMismatch((object) listId, (object) picklist.Id));
      IWorkItemPickListService service = this.TfsRequestContext.GetService<IWorkItemPickListService>();
      if (!string.IsNullOrWhiteSpace(picklist.Type))
      {
        WorkItemPickListType pickListType = this.GetPickListType(picklist.Type);
        WorkItemPickList picklist1;
        if (service.TryGetList(this.TfsRequestContext, listId, out picklist1) && picklist1.Type != pickListType)
          throw new VssPropertyValidationException("Type", ResourceStrings.InvalidPickListType());
      }
      return this.Request.CreateResponse<PickListModel>(HttpStatusCode.OK, PickListModelFactory.Create(this.TfsRequestContext, service.UpdateList(this.TfsRequestContext, listId, picklist.Name, (IReadOnlyList<string>) picklist.Items.Select<PickListItemModel, string>((Func<PickListItemModel, string>) (i => i.Value)).ToList<string>(), picklist.IsSuggested)));
    }

    private WorkItemPickListType GetPickListType(string type)
    {
      if (type.Equals("String", StringComparison.OrdinalIgnoreCase))
        return WorkItemPickListType.String;
      if (type.Equals("Integer", StringComparison.OrdinalIgnoreCase))
        return WorkItemPickListType.Integer;
      if (type.Equals("Double", StringComparison.OrdinalIgnoreCase))
        return WorkItemPickListType.Double;
      throw new VssPropertyValidationException(nameof (type), ResourceStrings.InvalidPickListType());
    }
  }
}
