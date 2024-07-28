// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UrlHelperExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class UrlHelperExtensions
  {
    public static string Route(this UrlHelper urlHelper, Guid locationId, object routeValues)
    {
      ApiResourceLocation locationById = VersionedApiResourceRegistration.ResourceLocations.GetLocationById(locationId);
      return UrlHelperExtensions.UrlHelperRoute(urlHelper, locationById, routeValues);
    }

    public static string RestLink(
      this UrlHelper urlHelper,
      IVssRequestContext requestContext,
      Guid locationId,
      object routeValues)
    {
      return urlHelper.RestLink(requestContext, locationId, (string) null, routeValues);
    }

    public static string RestLink(
      this UrlHelper urlHelper,
      IVssRequestContext requestContext,
      Guid locationId,
      string scopePath,
      object routeValues)
    {
      string rawUrl = urlHelper.Route(locationId, routeValues);
      if (string.IsNullOrWhiteSpace(rawUrl))
      {
        ApiResourceLocation locationById = VersionedApiResourceRegistration.ResourceLocations.GetLocationById(locationId);
        rawUrl = VssHttpUriUtility.ReplaceRouteValues(locationById.RouteTemplate, (Dictionary<string, object>) UrlHelperExtensions.GetResourceLocationRouteValues(locationById, routeValues), true, true);
      }
      if (string.IsNullOrWhiteSpace(rawUrl))
        return (string) null;
      string str = UrlHelperExtensions.RemoveVirtualDirectory(requestContext, rawUrl);
      string accessMappingMoniker = AccessMappingConstants.ClientAccessMappingMoniker;
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.WebApi.UsePublicAccessMappingMoniker"))
        accessMappingMoniker = AccessMappingConstants.PublicAccessMappingMoniker;
      StringBuilder stringBuilder = new StringBuilder(requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, Guid.Empty, accessMappingMoniker).TrimEnd('/'));
      stringBuilder.Append('/');
      if (!string.IsNullOrEmpty(scopePath))
        stringBuilder.Append(scopePath.Trim('/')).Append('/');
      return stringBuilder.Append(str.TrimStart('/')).ToString();
    }

    private static string RemoveVirtualDirectory(IVssRequestContext requestContext, string rawUrl)
    {
      string str1 = requestContext.VirtualPath();
      if (!string.IsNullOrEmpty(rawUrl) && !string.IsNullOrEmpty(str1))
      {
        rawUrl = rawUrl.TrimStart('/');
        string str2 = str1.TrimStart('/');
        if (rawUrl.StartsWith(str2, StringComparison.OrdinalIgnoreCase))
          rawUrl = rawUrl.Substring(str2.Length);
        else if (rawUrl.StartsWith("tfs/"))
          rawUrl = rawUrl.Substring(4);
      }
      return rawUrl;
    }

    public static string Link(this UrlHelper urlHelper, Guid locationId, object routeValues)
    {
      ApiResourceLocation locationById = VersionedApiResourceRegistration.ResourceLocations.GetLocationById(locationId);
      string path = VssHttpUriUtility.ReplaceRouteValues(locationById.RouteTemplate, (Dictionary<string, object>) UrlHelperExtensions.GetResourceLocationRouteValues(locationById, routeValues));
      return urlHelper.Content(path);
    }

    private static HttpRouteValueDictionary GetResourceLocationRouteValues(
      ApiResourceLocation location,
      object routeValues)
    {
      HttpRouteValueDictionary routeDictionary = HttpRouteValueExtensions.ToRouteDictionary(routeValues);
      routeDictionary.AddRouteValueIfNotPresent("area", (object) location.Area);
      routeDictionary.AddRouteValueIfNotPresent("resource", (object) location.ResourceName);
      return routeDictionary;
    }

    private static string UrlHelperRoute(
      UrlHelper urlHelper,
      ApiResourceLocation location,
      object routeValues)
    {
      HttpRouteValueDictionary locationRouteValues = UrlHelperExtensions.GetResourceLocationRouteValues(location, routeValues);
      IHttpRoute routeForLocation = VersionedApiResourceRegistration.GetRouteForLocation(location.Id);
      if (routeForLocation == null)
        return urlHelper.Route(location.RouteName, (IDictionary<string, object>) locationRouteValues);
      locationRouteValues[HttpRoute.HttpRouteKey] = (object) true;
      return routeForLocation.GetVirtualPath(urlHelper.Request, (IDictionary<string, object>) locationRouteValues)?.VirtualPath;
    }
  }
}
