// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Batch.WorkItemPartialUpdateBatchHandler
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Helpers;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Batch
{
  public class WorkItemPartialUpdateBatchHandler : WorkItemUpdateBatchHandler
  {
    public WorkItemPartialUpdateBatchHandler(HttpServer server)
      : base(server)
    {
    }

    public override IEnumerable<HttpResponseMessage> ProcessBatch(
      IVssRequestContext requestContext,
      IEnumerable<BatchHttpRequestMessage> requests)
    {
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate> workItemUpdateList = new List<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate>();
      List<BatchHttpRequestMessage> handledRequests = new List<BatchHttpRequestMessage>();
      List<HttpResponseMessage> responses = new List<HttpResponseMessage>();
      WorkItemController controller = new WorkItemController();
      bool flag = false;
      BatchRequestParameters batchRequestParameters;
      if (!this.IsBatchRequestValid(requestContext, requests, controller, responses, out batchRequestParameters))
        return (IEnumerable<HttpResponseMessage>) responses;
      List<BatchRequest> batchRequestList1 = new List<BatchRequest>();
      HashSet<int> ids = new HashSet<int>();
      foreach (BatchHttpRequestMessage request in requests)
      {
        try
        {
          IHttpRouteData routeData = request.GetRouteData();
          if (this.ValidateRequest(request, routeData))
          {
            int id;
            if (this.TryGetWorkItemId(routeData, out id))
            {
              List<BatchRequest> batchRequestList2 = batchRequestList1;
              BatchUpdateRequest batchUpdateRequest = new BatchUpdateRequest();
              batchUpdateRequest.RequestMessage = request;
              batchUpdateRequest.WorkItemId = id;
              batchRequestList2.Add((BatchRequest) batchUpdateRequest);
              ids.Add(id);
            }
            else
            {
              string projectNameOrGuid;
              string workItemType;
              if (this.TryGetCreateRequestParameters(routeData, out projectNameOrGuid, out workItemType))
              {
                List<BatchRequest> batchRequestList3 = batchRequestList1;
                BatchCreateRequest batchCreateRequest = new BatchCreateRequest();
                batchCreateRequest.RequestMessage = request;
                batchCreateRequest.ProjectNameOrGuid = projectNameOrGuid;
                batchCreateRequest.WorkItemType = workItemType;
                batchRequestList3.Add((BatchRequest) batchCreateRequest);
              }
            }
          }
        }
        catch (Exception ex1)
        {
          flag = true;
          request.Handled = true;
          Exception ex2 = WitUpdateHelper.TranslateUpdateResultException(ex1);
          responses.Add(request.CreateErrorResponse(controller.MapException(ex2), controller.TranslateException(ex2).Message));
        }
      }
      Dictionary<int, Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem> dictionary = WitUpdateHelper.PrepareUpdateWorkItems(requestContext, (IEnumerable<int>) ids, batchRequestParameters.UseIdentityRef).ToDictionary<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem, int>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem, int>) (workItem => workItem.Id));
      foreach (BatchRequest batchRequest in batchRequestList1)
      {
        try
        {
          PatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> patchDocument = this.ReadPatchDocument((HttpRequestMessage) batchRequest.RequestMessage);
          if (patchDocument == null)
            throw new VssPropertyValidationException("document", ResourceStrings.MissingPatchDocument());
          Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem serverWorkItem;
          if (batchRequest is BatchCreateRequest batchCreateRequest)
            serverWorkItem = WitUpdateHelper.PrepareUpdateWorkItem(requestContext, HttpUtility.UrlDecode(batchCreateRequest.ProjectNameOrGuid), HttpUtility.UrlDecode(batchCreateRequest.WorkItemType), patchDocument, batchRequestParameters.UseIdentityRef);
          else if (batchRequest is BatchUpdateRequest batchUpdateRequest)
            serverWorkItem = dictionary[batchUpdateRequest.WorkItemId];
          else
            continue;
          Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate updateRequest = this.CreateUpdateRequest(requestContext, workItemUpdateList, batchRequestParameters.UseLegacyIndexHandling, batchRequestParameters.UseIdentityRef, batchRequest.RequestMessage, patchDocument, serverWorkItem);
          workItemUpdateList.Add(updateRequest);
          handledRequests.Add(batchRequest.RequestMessage);
        }
        catch (Exception ex)
        {
          responses.Add(batchRequest.RequestMessage.CreateErrorResponse(controller.MapException(ex), controller.TranslateException(ex).Message));
          batchRequest.RequestMessage.Handled = true;
        }
      }
      if (!flag && workItemUpdateList.Any<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate>())
        this.CommitChanges(requestContext, workItemUpdateList, handledRequests, responses, controller, batchRequestParameters);
      return (IEnumerable<HttpResponseMessage>) responses;
    }

    private bool TryGetCreateRequestParameters(
      IHttpRouteData route,
      out string projectNameOrGuid,
      out string workItemType)
    {
      workItemType = (string) null;
      projectNameOrGuid = (string) null;
      return string.Equals(route.Route.RouteTemplate, WorkItemUpdateBatchHandler.createRouteTemplate, StringComparison.OrdinalIgnoreCase) && route.Values.TryGetValue<string>("project", out projectNameOrGuid) && projectNameOrGuid != null && route.Values.TryGetValue<string>("type", out workItemType) && workItemType != null;
    }

    private bool TryGetWorkItemId(IHttpRouteData route, out int id)
    {
      id = 0;
      return int.TryParse((string) route.Values[nameof (id)], out id) && string.Equals(route.Route.RouteTemplate, WorkItemUpdateBatchHandler.updateRouteTemplate, StringComparison.OrdinalIgnoreCase);
    }
  }
}
