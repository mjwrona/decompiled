// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Batch.WorkItemRestoreBatchHandler
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Helpers;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Batch
{
  public class WorkItemRestoreBatchHandler : BatchHandler
  {
    private static readonly HttpMethod Patch = new HttpMethod("PATCH");
    private static readonly IEnumerable<MediaTypeFormatter> MediaTypeFormatters = (IEnumerable<MediaTypeFormatter>) Enumerable.Repeat<VssJsonPatchMediaTypeFormatter>(new VssJsonPatchMediaTypeFormatter(), 1);

    public WorkItemRestoreBatchHandler(HttpServer server)
      : base(server)
    {
    }

    public override IEnumerable<HttpResponseMessage> ProcessBatch(
      IVssRequestContext requestContext,
      IEnumerable<BatchHttpRequestMessage> requests)
    {
      ITeamFoundationWorkItemService service = requestContext.GetService<ITeamFoundationWorkItemService>();
      List<int> intList = new List<int>();
      List<BatchHttpRequestMessage> httpRequestMessageList = new List<BatchHttpRequestMessage>();
      List<HttpResponseMessage> httpResponseMessageList = new List<HttpResponseMessage>();
      RecycleBinController recycleBinController = new RecycleBinController();
      bool flag = false;
      foreach (BatchHttpRequestMessage request in requests)
      {
        if (!(request.Method != WorkItemRestoreBatchHandler.Patch))
        {
          IHttpRouteData routeData = request.GetRouteData();
          object b1;
          object b2;
          if (routeData.Values.TryGetValue("area", out b1) && string.Equals("wit", b1 as string, StringComparison.InvariantCultureIgnoreCase) && routeData.Values.TryGetValue("resource", out b2) && string.Equals("recyclebin", b2 as string, StringComparison.InvariantCultureIgnoreCase))
          {
            if (request.RequestUri.ParseQueryString().Count <= 1)
            {
              try
              {
                if (!(routeData.Route.RouteTemplate == "_apis/{area}/{resource}/{id}"))
                {
                  if (!(routeData.Route.RouteTemplate == "{project}/_apis/{area}/{resource}/{id}"))
                    continue;
                }
                IPatchOperation<WorkItemDeleteUpdate> patchOperation = (this.ReadPatchDocument((HttpRequestMessage) request) ?? throw new VssPropertyValidationException("patchDocument", ResourceStrings.MissingPatchDocument())).Operations.FirstOrDefault<IPatchOperation<WorkItemDeleteUpdate>>();
                if (patchOperation.Path.Equals("/IsDeleted", StringComparison.OrdinalIgnoreCase))
                {
                  if (patchOperation.Value.ToString().Equals("false", StringComparison.OrdinalIgnoreCase))
                  {
                    int result;
                    if (int.TryParse((string) routeData.Values["id"], out result))
                    {
                      intList.Add(result);
                      request.Handled = true;
                      request.Handler = nameof (WorkItemRestoreBatchHandler);
                      httpRequestMessageList.Add(request);
                    }
                  }
                }
              }
              catch (Exception ex)
              {
                httpResponseMessageList.Add(httpRequestMessageList[0].CreateErrorResponse(recycleBinController.MapException(ex), ex.Message));
                flag = true;
                request.Handled = true;
              }
            }
          }
        }
      }
      if (!flag)
      {
        if (intList.Any<int>())
        {
          try
          {
            BatchHttpRequestMessage request = httpRequestMessageList[0];
            IEnumerable<WorkItemUpdateResult> results = service.RestoreWorkItems(requestContext, (IEnumerable<int>) intList);
            Dictionary<int?, WorkItemDeleteReference> resultMap = WitDeleteHelper.GetWorkItemDeleteReferencesInternalResponse(requestContext, service, results, WorkItemRetrievalMode.NonDeleted, this.ShouldReturnProjectScopedUrl(requestContext, (HttpRequestMessage) request)).ToList<WorkItemDeleteReference>().ToDictionary<WorkItemDeleteReference, int?, WorkItemDeleteReference>((Func<WorkItemDeleteReference, int?>) (key => key.Id), (Func<WorkItemDeleteReference, WorkItemDeleteReference>) (value => value));
            List<WorkItemDeleteReference> list = intList.Select<int, WorkItemDeleteReference>((Func<int, WorkItemDeleteReference>) (id => resultMap[new int?(id)])).ToList<WorkItemDeleteReference>();
            for (int index = 0; index < list.Count; ++index)
            {
              WorkItemDeleteReference itemDeleteReference = list[index];
              string message = list[index].Message;
              if (message != null)
              {
                HttpStatusCode statusCode = (HttpStatusCode) itemDeleteReference.Code.Value;
                httpResponseMessageList.Add(request.CreateErrorResponse(statusCode, message));
              }
              else
                httpResponseMessageList.Add(request.CreateResponse<WorkItemDeleteReference>(itemDeleteReference));
            }
          }
          catch (Exception ex)
          {
            httpResponseMessageList.Add(httpRequestMessageList[0].CreateErrorResponse(recycleBinController.MapException(ex), ex.Message));
          }
        }
      }
      return (IEnumerable<HttpResponseMessage>) httpResponseMessageList;
    }

    private PatchDocument<WorkItemDeleteUpdate> ReadPatchDocument(HttpRequestMessage request) => request.Content.ReadAsAsync<PatchDocument<WorkItemDeleteUpdate>>(WorkItemRestoreBatchHandler.MediaTypeFormatters).Result;
  }
}
