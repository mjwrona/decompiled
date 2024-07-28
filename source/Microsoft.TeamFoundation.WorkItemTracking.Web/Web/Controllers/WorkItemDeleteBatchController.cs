// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemDeleteBatchController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Helpers;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [ClientGroupByResource("workItems")]
  [ValidateModel]
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "workItemsDelete", ResourceVersion = 1)]
  public class WorkItemDeleteBatchController : WorkItemTrackingApiController
  {
    private const int TraceRange = 5924000;

    public override string TraceArea => "workItemsDelete";

    [TraceFilter(5924000, 5924010)]
    [HttpPost]
    [ClientLocationId("8bc57545-27e5-420d-b709-f6e3ebcc1fc1")]
    [ClientResponseType(typeof (WorkItemDeleteBatch), null, null)]
    [ClientExample("POST__wit_workitems_delete.json", null, null, null)]
    public HttpResponseMessage DeleteWorkItems(WorkItemDeleteBatchRequest deleteRequest)
    {
      if (WorkItemTrackingFeatureFlags.IsWorkItemsBulkDeleteDisabled(this.TfsRequestContext))
        throw new FeatureDisabledException("DeleteWorkItems feature is not enabled");
      if (deleteRequest == null)
        throw new VssPropertyValidationException(nameof (deleteRequest), ResourceStrings.NullOrEmptyParameter((object) nameof (deleteRequest)));
      if (deleteRequest.Ids == null || deleteRequest.Ids.Count<int>() == 0)
        throw new VssPropertyValidationException("Ids", ResourceStrings.NullOrEmptyParameter((object) "Ids"));
      int num = this.TfsRequestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>().GetValue<int>(this.TfsRequestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/WorkItemsDeleteBatchSize", 200);
      if (deleteRequest.Ids.Count<int>() > num)
        throw new VssPropertyValidationException("Ids", ResourceStrings.ParameterExceededThreshold((object) "Ids", (object) num));
      if (deleteRequest.Destroy)
      {
        this.WorkItemService.DestroyWorkItems(this.TfsRequestContext, deleteRequest.Ids, deleteRequest.SkipNotifications, true);
        return new HttpResponseMessage(HttpStatusCode.NoContent);
      }
      WorkItemDeleteBatch internalResponse = WitDeleteHelper.GetWorkItemDeleteInternalResponse(this.WorkItemService.DeleteWorkItems(this.TfsRequestContext, deleteRequest.Ids, deleteRequest.SkipNotifications, true));
      return internalResponse == null ? this.Request.CreateResponse(HttpStatusCode.NotFound) : this.Request.CreateResponse<WorkItemDeleteBatch>(HttpStatusCode.OK, internalResponse);
    }
  }
}
