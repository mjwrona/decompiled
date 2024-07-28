// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ApiResourceOptionsController
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ApiResourceOptionsController : TfsApiController
  {
    private const string c_binaryOptionsFeatureFlag = "VisualStudio.Services.Location.ResourcesFromBinariesInOptionsController";
    private const string c_binaryOptionsHeaderName = "x-ms-binarysupportedoptions";

    [PublicCollectionRequestRestrictions(false, true, null)]
    public HttpResponseMessage Options(string area = null, string resource = null, bool allHostTypes = false)
    {
      this.TfsRequestContext.GetService<SecuredLocationManager>().Connect(this.TfsRequestContext);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/json");
      bool values1 = this.Request.Headers.TryGetValues("x-ms-binarysupportedoptions", out IEnumerable<string> _);
      ApiResourceLocationCollection locationCollection1;
      if (this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Location.ResourcesFromBinariesInOptionsController") | values1)
      {
        locationCollection1 = !(allHostTypes | values1) ? VersionedApiResourceRegistration.GetLocationsForHostType(this.TfsRequestContext) : VersionedApiResourceRegistration.GetLocationsForAllHostTypes(this.TfsRequestContext);
      }
      else
      {
        LocationService service = this.TfsRequestContext.GetService<LocationService>();
        locationCollection1 = ((ILocationDataProvider) service)?.GetResourceLocations(this.TfsRequestContext) ?? new ApiResourceLocationCollection();
        if (allHostTypes)
        {
          if (this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          {
            IVssRequestContext requestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
            ApiResourceLocationCollection locationCollection2 = ((ILocationDataProvider) service)?.GetResourceLocations(requestContext) ?? new ApiResourceLocationCollection();
            locationCollection1.AddResourceLocations(locationCollection2.GetAllLocations());
          }
          if (this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) || this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.Application))
          {
            IVssRequestContext requestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
            ApiResourceLocationCollection locationCollection3 = ((ILocationDataProvider) service)?.GetResourceLocations(requestContext) ?? new ApiResourceLocationCollection();
            locationCollection1.AddResourceLocations(locationCollection3.GetAllLocations());
          }
        }
      }
      HttpConfiguration httpConfiguration = WebApiConfiguration.GetHttpConfiguration(this.TfsRequestContext);
      IEnumerable<ApiResourceLocation> source;
      if (string.IsNullOrEmpty(area))
        source = locationCollection1.GetAllLocations();
      else if (string.IsNullOrEmpty(resource))
      {
        source = locationCollection1.GetAreaLocations(area);
      }
      else
      {
        source = locationCollection1.GetResourceLocations(area, resource);
        if (source.Any<ApiResourceLocation>())
        {
          ApiResourceVersion requestVersion = this.Request.GetApiResourceVersion();
          if (requestVersion == null)
          {
            Version apiVersion1 = source.Max<ApiResourceLocation, Version>((Func<ApiResourceLocation, Version>) (l => l.ReleasedVersion));
            Version apiVersion2 = source.Min<ApiResourceLocation, Version>((Func<ApiResourceLocation, Version>) (l => l.MinVersion));
            requestVersion = !(apiVersion2 > apiVersion1) ? new ApiResourceVersion(apiVersion1) : new ApiResourceVersion(apiVersion2);
          }
          VersionedApiControllerInfo controllerInfo = VersionedApiControllerInfoCache.GetControllerInfo(area, resource, requestVersion, this.TfsRequestContext);
          if (controllerInfo != null)
          {
            mediaType.Parameters.AddApiResourceVersionValues(controllerInfo.Version);
            HttpControllerDescriptor controllerDescriptor;
            if (httpConfiguration.Services.GetHttpControllerSelector().GetControllerMapping().TryGetValue(controllerInfo.ControllerName, out controllerDescriptor))
            {
              HashSet<string> values2 = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
              foreach (HttpMethod httpMethod in httpConfiguration.Services.GetActionSelector().GetActionMapping(controllerDescriptor).SelectMany<IGrouping<string, HttpActionDescriptor>, HttpMethod>((Func<IGrouping<string, HttpActionDescriptor>, IEnumerable<HttpMethod>>) (group => group.SelectMany<HttpActionDescriptor, HttpMethod>((Func<HttpActionDescriptor, IEnumerable<HttpMethod>>) (action => (IEnumerable<HttpMethod>) action.SupportedHttpMethods)))))
                values2.Add(httpMethod.Method);
              response.Headers.Add("Access-Control-Allow-Methods", string.Join(",", (IEnumerable<string>) values2));
            }
          }
        }
      }
      VssJsonCollectionWrapper<IEnumerable<ApiResourceLocation>> collectionWrapper = new VssJsonCollectionWrapper<IEnumerable<ApiResourceLocation>>((IEnumerable) source);
      Type type = typeof (VssJsonCollectionWrapper<IEnumerable<ApiResourceLocation>>);
      ContentNegotiationResult negotiationResult = httpConfiguration.Services.GetContentNegotiator().Negotiate(type, this.Request, (IEnumerable<MediaTypeFormatter>) httpConfiguration.Formatters);
      response.Content = (HttpContent) new ObjectContent(type, (object) collectionWrapper, negotiationResult.Formatter, mediaType);
      return response;
    }

    public override string TraceArea => "SecurityService";

    public override string ActivityLogArea => "Framework";
  }
}
