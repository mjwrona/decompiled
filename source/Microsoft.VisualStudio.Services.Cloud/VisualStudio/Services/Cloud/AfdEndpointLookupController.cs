// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AfdEndpointLookupController
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "AzureFrontDoor", ResourceName = "AfdEndpointLookup")]
  public class AfdEndpointLookupController : TfsApiController
  {
    [HttpGet]
    public HttpResponseMessage GetAfdEndpoint(string key)
    {
      IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
      IAfdEndpointResolverProvider service = vssRequestContext.GetService<IAfdEndpointResolverProvider>();
      foreach (VersionedRouteKey allRouteKey in (IEnumerable<VersionedRouteKey>) AfdRouteKeyUtils.ExtractAllRouteKeys(key))
      {
        IAfdEndpointResolver resolver;
        if (!service.TryGetResolver(allRouteKey.Version, out resolver))
        {
          vssRequestContext.Trace(111111, TraceLevel.Error, this.TraceArea, nameof (GetAfdEndpoint), string.Format("Route key version '{0}' is unexpected.", (object) allRouteKey.Version));
        }
        else
        {
          string endpoint;
          if (!resolver.TryGetEndpoint(vssRequestContext, allRouteKey.RouteKey, out endpoint))
          {
            vssRequestContext.Trace(111111, TraceLevel.Error, this.TraceArea, nameof (GetAfdEndpoint), string.Format("Cannot route the request based on '{0}'.", (object) allRouteKey));
          }
          else
          {
            vssRequestContext.Trace(121212, TraceLevel.Info, this.TraceArea, nameof (GetAfdEndpoint), string.Format("Route key: '{0}'. Endpoint: '{1}'.", (object) allRouteKey, (object) endpoint));
            return this.CreateResponse(endpoint);
          }
        }
      }
      vssRequestContext.Trace(121212, TraceLevel.Info, this.TraceArea, nameof (GetAfdEndpoint), "Did not find the endpoint for route key '" + key + "'.");
      return this.CreateResponse(string.Empty);
    }

    private HttpResponseMessage CreateResponse(string content) => new HttpResponseMessage(HttpStatusCode.OK)
    {
      Content = (HttpContent) new StringContent(content, Encoding.UTF8, "text/plain")
    };

    public override string TraceArea => nameof (AfdEndpointLookupController);

    public override string ActivityLogArea => "Framework";
  }
}
