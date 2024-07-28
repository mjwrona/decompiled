// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HttpRouteCollectionExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Routing;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class HttpRouteCollectionExtensions
  {
    private static string m_defaultRoutePrefix = "_apis";
    private const string c_apisAggregateRouteName = "DefaultApisAggregateRoute";

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that allows you to specify a location id.  A unique location id is required for all routes.")]
    public static IHttpRoute MapResourceRoute(
      this HttpRouteCollection routes,
      TeamFoundationHostType hostType,
      string name,
      string routeTemplate)
    {
      return routes.MapResourceRoute(hostType, name, routeTemplate, (object) null);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that allows you to specify a location id.  A unique location id is required for all routes.")]
    public static IHttpRoute MapResourceRoute(
      this HttpRouteCollection routes,
      TeamFoundationHostType hostType,
      string name,
      string routeTemplate,
      object defaults)
    {
      return routes.MapResourceRoute(hostType, name, routeTemplate, defaults, (object) null);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that allows you to specify a location id.  A unique location id is required for all routes.")]
    public static IHttpRoute MapResourceRoute(
      this HttpRouteCollection routes,
      TeamFoundationHostType hostType,
      string name,
      string routeTemplate,
      object defaults,
      object constraints)
    {
      return HttpRouteCollectionExtensions.MapHttpRoute(routes, hostType, name, routeTemplate, defaults, constraints, (HttpMessageHandler) null);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use the overload that allows you to specify a location id.  A unique location id is required for all routes.")]
    public static IHttpRoute MapResourceRoute(
      this HttpRouteCollection routes,
      TeamFoundationHostType hostType,
      string name,
      string routeTemplate,
      object defaults,
      object constraints,
      HttpMessageHandler handler)
    {
      return HttpRouteCollectionExtensions.MapHttpRoute(routes, hostType, name, routeTemplate, defaults, constraints, handler);
    }

    private static IHttpRoute MapHttpRoute(
      HttpRouteCollection routes,
      TeamFoundationHostType hostType,
      string name,
      string routeTemplate,
      object defaults,
      object constraints,
      HttpMessageHandler handler)
    {
      TfsApiHostTypeConstraint hostTypeConstraint = new TfsApiHostTypeConstraint(hostType);
      return HttpRouteCollectionExtensions.MapHttpRoute(routes, hostType, name, HttpRouteCollectionExtensions.CreateResourcePath(routeTemplate), defaults, hostTypeConstraint, constraints, handler);
    }

    private static IHttpRoute MapHttpRoute(
      HttpRouteCollection routes,
      TeamFoundationHostType hostType,
      string name,
      string routeTemplate,
      object defaults,
      TfsApiHostTypeConstraint hostTypeConstraint,
      object constraints,
      HttpMessageHandler handler)
    {
      ArgumentUtility.CheckForNull<HttpRouteCollection>(routes, nameof (routes));
      ArgumentUtility.CheckStringForNullOrEmpty(routeTemplate, nameof (routeTemplate));
      return routes.MapHttpRoute(name, HttpRouteCollectionExtensions.CreateResourcePath(routeTemplate), defaults, (object) new RouteValueDictionary(constraints)
      {
        ["TFS_HostType"] = (object) hostTypeConstraint
      }, handler);
    }

    public static IHttpRoute MapResourceRoute(
      this HttpRouteCollection routes,
      TeamFoundationHostType hostType,
      Guid locationId,
      string area,
      string resource,
      string routeTemplate,
      VssRestApiVersion initialVersion,
      VssRestApiReleaseState releaseState = VssRestApiReleaseState.Preview,
      int maxResourceVersion = 1,
      object defaults = null,
      object constraints = null,
      string routeName = null,
      HttpMessageHandler handler = null,
      IEnumerable<string> hostScopeTemplates = null,
      bool hostScopeTemplateOptional = false,
      VssRestApiVersion? deprecatedAtVersion = null,
      string routePrefix = null)
    {
      Version version1 = initialVersion.ToVersion();
      Version version2 = !deprecatedAtVersion.HasValue ? VssRestApiVersionsRegistry.GetLatestVersion() : deprecatedAtVersion.Value.ToVersion();
      Version releasedApiVersion = releaseState != VssRestApiReleaseState.Preview ? VssRestApiVersionsRegistry.GetLatestReleasedVersion(version1, version2) : new Version(0, 0);
      Version defaultApiVersion = releasedApiVersion.Major > 0 || releasedApiVersion.Minor > 0 ? releasedApiVersion : version2;
      routePrefix = routePrefix ?? HttpRouteCollectionExtensions.DefaultRoutePrefix;
      return routes.MapResourceRouteInternal(hostType, locationId, area, resource, routeTemplate, version1, version2, releasedApiVersion, defaultApiVersion, maxResourceVersion, RequestApiVersionRequirement.RequiredOnEditOperations, defaults, constraints, routeName, handler, hostScopeTemplates, hostScopeTemplateOptional, routePrefix);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static IHttpRoute MapLegacyResourceRoute(
      this HttpRouteCollection routes,
      TeamFoundationHostType hostType,
      Guid locationId,
      string area,
      string resource,
      string routeTemplate,
      Version minApiVersion,
      Version maxApiVersion = null,
      Version releasedApiVersion = null,
      Version defaultApiVersion = null,
      int maxResourceVersion = 1,
      object defaults = null,
      object constraints = null,
      string routeName = null,
      HttpMessageHandler handler = null,
      IEnumerable<string> hostScopeTemplates = null,
      bool hostScopeTemplateOptional = false)
    {
      if (maxApiVersion == (Version) null)
        maxApiVersion = VssRestApiVersionsRegistry.GetLatestVersion();
      if (releasedApiVersion == (Version) null || (releasedApiVersion.Major > 0 || releasedApiVersion.Minor > 0) && releasedApiVersion < VssRestApiVersionsRegistry.GetLatestReleasedVersion())
        releasedApiVersion = VssRestApiVersionsRegistry.GetLatestReleasedVersion();
      if (defaultApiVersion == (Version) null)
        defaultApiVersion = releasedApiVersion.Major > 0 || releasedApiVersion.Minor > 0 ? releasedApiVersion : maxApiVersion;
      RequestApiVersionRequirement apiVersionRequirement = RequestApiVersionRequirement.NeverRequired;
      return routes.MapResourceRouteInternal(hostType, locationId, area, resource, routeTemplate, minApiVersion, maxApiVersion, releasedApiVersion, defaultApiVersion, maxResourceVersion, apiVersionRequirement, defaults, constraints, routeName, handler, hostScopeTemplates, hostScopeTemplateOptional, HttpRouteCollectionExtensions.DefaultRoutePrefix);
    }

    private static IHttpRoute MapResourceRouteInternal(
      this HttpRouteCollection routes,
      TeamFoundationHostType hostType,
      Guid locationId,
      string area,
      string resource,
      string routeTemplate,
      Version minApiVersion,
      Version maxApiVersion,
      Version releasedApiVersion,
      Version defaultApiVersion,
      int maxResourceVersion,
      RequestApiVersionRequirement apiVersionRequirement,
      object defaults,
      object constraints,
      string routeName,
      HttpMessageHandler handler,
      IEnumerable<string> hostScopeTemplates,
      bool hostScopeTemplateOptional,
      string routePrefix)
    {
      ArgumentUtility.CheckForNull<HttpRouteCollection>(routes, nameof (routes));
      ArgumentUtility.CheckForEmptyGuid(locationId, nameof (locationId));
      ArgumentUtility.CheckForNull<string>(area, nameof (area));
      ArgumentUtility.CheckStringForNullOrEmpty(resource, nameof (resource));
      ArgumentUtility.CheckStringForNullOrEmpty(routeTemplate, nameof (routeTemplate));
      ArgumentUtility.CheckForNull<Version>(minApiVersion, nameof (minApiVersion));
      ArgumentUtility.CheckForNull<Version>(maxApiVersion, nameof (maxApiVersion));
      ArgumentUtility.CheckForNull<Version>(defaultApiVersion, nameof (defaultApiVersion));
      ArgumentUtility.CheckForNull<Version>(releasedApiVersion, nameof (releasedApiVersion));
      RouteValueDictionary routeValueDictionary = new RouteValueDictionary(constraints);
      routeValueDictionary["TFS_HostType"] = (object) new TfsApiHostTypeConstraint(hostType);
      string name = "DefaultApisAggregateRoute";
      if (routePrefix != HttpRouteCollectionExtensions.DefaultRoutePrefix)
        name = "DefaultApisAggregateRoute" + "-" + routePrefix;
      IHttpRoute route;
      if (!routes.TryGetValue(name, out route))
      {
        route = (IHttpRoute) new GroupedWebApiRoute(routePrefix);
        routes.Add(name, route);
      }
      int hostType1 = (int) hostType;
      HttpRouteCollection routes1 = ((GroupedWebApiRoute) route).Routes;
      Guid locationId1 = locationId;
      string area1 = area;
      string resource1 = resource;
      string resourcePath = HttpRouteCollectionExtensions.CreateResourcePath(routeTemplate, routePrefix);
      Version minApiVersion1 = minApiVersion;
      Version maxApiVersion1 = maxApiVersion;
      Version releasedApiVersion1 = releasedApiVersion;
      Version defaultApiVersion1 = defaultApiVersion;
      int maxResourceVersion1 = maxResourceVersion;
      int apiVersionRequirement1 = (int) apiVersionRequirement;
      IEnumerable<string> strings = hostScopeTemplates;
      bool flag = hostScopeTemplateOptional;
      object defaults1 = defaults;
      RouteValueDictionary constraints1 = routeValueDictionary;
      HttpMessageHandler handler1 = handler;
      string routeName1 = routeName;
      IEnumerable<string> hostScopeTemplates1 = strings;
      int num = flag ? 1 : 0;
      return VersionedApiResourceRegistration.RegisterResource((TeamFoundationHostType) hostType1, routes1, locationId1, area1, resource1, resourcePath, minApiVersion1, maxApiVersion1, releasedApiVersion1, defaultApiVersion1, maxResourceVersion1, (RequestApiVersionRequirement) apiVersionRequirement1, defaults1, (object) constraints1, handler1, routeName1, hostScopeTemplates1, num != 0);
    }

    internal static void RegisterResourceOptionsHandler(this HttpRouteCollection routes) => routes.MapHttpRoute("ApiResourceOptions", HttpRouteCollectionExtensions.CreateResourcePath("{area}/{resource}"), (object) new
    {
      area = RouteParameter.Optional,
      resource = RouteParameter.Optional,
      controller = "ApiResourceOptions"
    }, (object) new
    {
      optionsConstraint = new ApiResourceOptionsRouteContraint()
    });

    private static string CreateResourcePath(string resourcePath, string routePrefix = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(resourcePath, nameof (resourcePath));
      routePrefix = routePrefix ?? HttpRouteCollectionExtensions.DefaultRoutePrefix;
      StringBuilder stringBuilder = new StringBuilder();
      string str = resourcePath.TrimStart('/');
      if (!str.StartsWith(routePrefix, StringComparison.OrdinalIgnoreCase))
      {
        stringBuilder.Append(routePrefix);
        stringBuilder.Append('/');
      }
      stringBuilder.Append(str);
      return stringBuilder.ToString();
    }

    public static string DefaultRoutePrefix => HttpRouteCollectionExtensions.m_defaultRoutePrefix;
  }
}
