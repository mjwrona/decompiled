// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Batch.InvalidRouteBatchHandler
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
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
  public class InvalidRouteBatchHandler : BatchHandler
  {
    public InvalidRouteBatchHandler(HttpServer server)
      : base(server)
    {
    }

    public override IEnumerable<HttpResponseMessage> ProcessBatch(
      IVssRequestContext requestContext,
      IEnumerable<BatchHttpRequestMessage> requests)
    {
      WorkItemController workItemController = new WorkItemController();
      List<HttpResponseMessage> httpResponseMessageList = new List<HttpResponseMessage>();
      foreach (BatchHttpRequestMessage request in requests)
      {
        IHttpRouteData routeData = request.GetRouteData();
        if (routeData == null)
        {
          request.Handled = true;
          httpResponseMessageList.Add(request.CreateResponse(HttpStatusCode.NotFound));
        }
        else
        {
          Exception ex = routeData.Values.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (v => v.Value is Exception)).Select<KeyValuePair<string, object>, Exception>((Func<KeyValuePair<string, object>, Exception>) (v => (Exception) v.Value)).FirstOrDefault<Exception>();
          if (ex != null)
          {
            request.Handled = true;
            httpResponseMessageList.Add(request.CreateErrorResponse(workItemController.MapException(ex), workItemController.TranslateException(ex).Message));
          }
        }
      }
      return (IEnumerable<HttpResponseMessage>) httpResponseMessageList;
    }
  }
}
