// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.ODataActionSelector
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Routing.Conventions;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;

namespace Microsoft.AspNet.OData.Routing
{
  public class ODataActionSelector : IHttpActionSelector
  {
    private const string MessageDetailKey = "MessageDetail";
    private readonly IHttpActionSelector _innerSelector;

    public ODataActionSelector(IHttpActionSelector innerSelector) => this._innerSelector = innerSelector != null ? innerSelector : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (innerSelector));

    public ILookup<string, HttpActionDescriptor> GetActionMapping(
      HttpControllerDescriptor controllerDescriptor)
    {
      return this._innerSelector.GetActionMapping(controllerDescriptor);
    }

    public HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
    {
      HttpRequestMessage request = controllerContext != null ? controllerContext.Request : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (controllerContext));
      ODataPath path = request.ODataProperties().Path;
      IEnumerable<IODataRoutingConvention> routingConventions = request.GetRoutingConventions();
      IHttpRouteData routeData = controllerContext.RouteData;
      if (path == null || routingConventions == null || routeData.Values.ContainsKey(ODataRouteConstants.Action))
        return this._innerSelector.SelectAction(controllerContext);
      ILookup<string, HttpActionDescriptor> actionMapping = this._innerSelector.GetActionMapping(controllerContext.ControllerDescriptor);
      foreach (IODataRoutingConvention routingConvention in routingConventions)
      {
        string str = routingConvention.SelectAction(path, controllerContext, actionMapping);
        if (str != null)
        {
          routeData.Values[ODataRouteConstants.Action] = (object) str;
          return this._innerSelector.SelectAction(controllerContext);
        }
      }
      throw new HttpResponseException(ODataActionSelector.CreateErrorResponse(request, HttpStatusCode.NotFound, Microsoft.AspNet.OData.Common.Error.Format(SRResources.NoMatchingResource, (object) controllerContext.Request.RequestUri), Microsoft.AspNet.OData.Common.Error.Format(SRResources.NoRoutingHandlerToSelectAction, (object) path.PathTemplate)));
    }

    private static HttpResponseMessage CreateErrorResponse(
      HttpRequestMessage request,
      HttpStatusCode statusCode,
      string message,
      string messageDetail)
    {
      HttpError error = new HttpError(message);
      if (request.ShouldIncludeErrorDetail())
        error.Add("MessageDetail", (object) messageDetail);
      return request.CreateErrorResponse(statusCode, error);
    }
  }
}
