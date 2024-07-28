// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Batch.WorkItemDestroyBatchHandler
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Batch
{
  public class WorkItemDestroyBatchHandler : BatchHandler
  {
    public WorkItemDestroyBatchHandler(HttpServer server)
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
        if (!(request.Method != HttpMethod.Delete))
        {
          IHttpRouteData routeData = request.GetRouteData();
          object b1;
          object b2;
          int result;
          if (routeData.Values.TryGetValue("area", out b1) && string.Equals("wit", b1 as string, StringComparison.InvariantCultureIgnoreCase) && routeData.Values.TryGetValue("resource", out b2) && string.Equals("recyclebin", b2 as string, StringComparison.InvariantCultureIgnoreCase) && request.RequestUri.ParseQueryString().Count <= 1 && (routeData.Route.RouteTemplate == "_apis/{area}/{resource}/{id}" || routeData.Route.RouteTemplate == "{project}/_apis/{area}/{resource}/{id}") && int.TryParse((string) routeData.Values["id"], out result))
          {
            intList.Add(result);
            request.Handled = true;
            request.Handler = nameof (WorkItemDestroyBatchHandler);
            httpRequestMessageList.Add(request);
          }
        }
      }
      if (!flag)
      {
        if (intList.Any<int>())
        {
          try
          {
            RecycleBinCommonController.VerifyItemsAreSoftDeleted(requestContext, service, (IEnumerable<int>) intList);
            service.DestroyWorkItems(requestContext, (IEnumerable<int>) intList);
            BatchHttpRequestMessage request = httpRequestMessageList[0];
            for (int index = 0; index < intList.Count; ++index)
              httpResponseMessageList.Add(request.CreateResponse(HttpStatusCode.NoContent));
          }
          catch (Exception ex)
          {
            httpResponseMessageList.Add(httpRequestMessageList[0].CreateErrorResponse(recycleBinController.MapException(ex), ex.Message));
          }
        }
      }
      return (IEnumerable<HttpResponseMessage>) httpResponseMessageList;
    }
  }
}
