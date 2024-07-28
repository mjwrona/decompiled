// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Routing.HttpRouteCollectionExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Routing;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Routing
{
  public static class HttpRouteCollectionExtensions
  {
    public const string PackagingRoutePrefix = "_packaging";
    private const string PackagingFallBackRouteName = "packagingNotFound";

    public static IHttpRoute MapPackagingRoute(
      this HttpRouteCollection routes,
      TeamFoundationHostType hostType,
      Guid locationId,
      string area,
      string resource,
      string routeTemplate,
      Version minApiVersion,
      Version maxApiVersion = null,
      Version defaultApiVersion = null,
      int maxResourceVersion = 1,
      object defaults = null,
      object constraints = null,
      string routeName = null,
      HttpMessageHandler handler = null,
      IEnumerable<string> hostScopeTemplates = null,
      bool hostScopeTemplateOptional = false)
    {
      Version version = new Version(0, 0);
      if (maxApiVersion == (Version) null)
        maxApiVersion = VssRestApiVersionsRegistry.GetLatestVersion();
      if (defaultApiVersion == (Version) null)
        defaultApiVersion = maxApiVersion;
      RequestApiVersionRequirement versionRequirement = RequestApiVersionRequirement.NeverRequired;
      RouteValueDictionary routeValueDictionary = new RouteValueDictionary(constraints);
      routeValueDictionary["TFS_HostType"] = (object) new TfsApiHostTypeConstraint(hostType);
      int hostType1 = (int) hostType;
      HttpRouteCollection routes1 = routes;
      Guid locationId1 = locationId;
      string area1 = area;
      string resource1 = resource;
      string routeTemplate1 = "_packaging/" + routeTemplate;
      Version minApiVersion1 = minApiVersion;
      Version maxApiVersion1 = maxApiVersion;
      Version releasedApiVersion = version;
      Version defaultApiVersion1 = defaultApiVersion;
      int maxResourceVersion1 = maxResourceVersion;
      int apiVersionRequirement = (int) versionRequirement;
      IEnumerable<string> strings = hostScopeTemplates;
      bool flag = hostScopeTemplateOptional;
      object defaults1 = defaults;
      RouteValueDictionary constraints1 = routeValueDictionary;
      HttpMessageHandler handler1 = handler;
      string routeName1 = routeName;
      IEnumerable<string> hostScopeTemplates1 = strings;
      int num = flag ? 1 : 0;
      return VersionedApiResourceRegistration.RegisterResource((TeamFoundationHostType) hostType1, routes1, locationId1, area1, resource1, routeTemplate1, minApiVersion1, maxApiVersion1, releasedApiVersion, defaultApiVersion1, maxResourceVersion1, (RequestApiVersionRequirement) apiVersionRequirement, defaults1, (object) constraints1, handler1, routeName1, hostScopeTemplates1, num != 0);
    }

    public static IHttpRoute MapPackagingRoute(
      this HttpRouteCollection routes,
      TfsApiResourceScope tfsApiScope,
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
      VssRestApiVersion? deprecatedAtVersion = null)
    {
      return HttpRouteCollectionExtensions.CopiedCode.MapResourceRoute(routes, tfsApiScope, locationId, area, resource, routeTemplate, initialVersion, releaseState, maxResourceVersion, defaults, constraints, routeName, handler, deprecatedAtVersion, "_packaging", RequestApiVersionRequirement.NeverRequired);
    }

    private static class CopiedCode
    {
      private const string c_apisAggregateRouteName = "DefaultApisAggregateRoute";

      public static IHttpRoute MapResourceRoute(
        HttpRouteCollection routes,
        TfsApiResourceScope tfsApiScope,
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
        VssRestApiVersion? deprecatedAtVersion = null,
        string routePrefix = null,
        RequestApiVersionRequirement apiVersionRequirement = RequestApiVersionRequirement.RequiredOnEditOperations)
      {
        TeamFoundationHostType hostType = TfsApiScopeRouteCollectionExtensions.ConvertToHostType(tfsApiScope);
        List<string> hostScopeTemplates = new List<string>();
        bool hostScopeTemplateOptional = false;
        if (tfsApiScope.HasFlag((Enum) TfsApiResourceScope.Project))
        {
          string str = "{project}";
          hostScopeTemplates.Add(str);
        }
        if (tfsApiScope.HasFlag((Enum) TfsApiResourceScope.Team))
        {
          string str = "{project}/{team}";
          hostScopeTemplates.Add(str);
        }
        if (hostScopeTemplates.Count > 0)
          hostScopeTemplateOptional = tfsApiScope.HasFlag((Enum) TfsApiResourceScope.Application) || tfsApiScope.HasFlag((Enum) TfsApiResourceScope.Collection) || tfsApiScope.HasFlag((Enum) TfsApiResourceScope.Deployment);
        return HttpRouteCollectionExtensions.CopiedCode.MapResourceRoute(routes, hostType, locationId, area, resource, routeTemplate, initialVersion, releaseState, maxResourceVersion, defaults, constraints, routeName, handler, (IEnumerable<string>) hostScopeTemplates, hostScopeTemplateOptional, deprecatedAtVersion, routePrefix, apiVersionRequirement);
      }

      private static string DefaultRoutePrefix => Microsoft.TeamFoundation.Framework.Server.HttpRouteCollectionExtensions.DefaultRoutePrefix;

      private static IHttpRoute MapResourceRoute(
        HttpRouteCollection routes,
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
        string routePrefix = null,
        RequestApiVersionRequirement apiVersionRequirement = RequestApiVersionRequirement.RequiredOnEditOperations)
      {
        Version version1 = initialVersion.ToVersion();
        Version version2 = !deprecatedAtVersion.HasValue ? VssRestApiVersionsRegistry.GetLatestVersion() : deprecatedAtVersion.Value.ToVersion();
        Version releasedApiVersion = releaseState != VssRestApiReleaseState.Preview ? VssRestApiVersionsRegistry.GetLatestReleasedVersion(version1, version2) : new Version(0, 0);
        Version defaultApiVersion = releasedApiVersion.Major > 0 || releasedApiVersion.Minor > 0 ? releasedApiVersion : version2;
        routePrefix = routePrefix ?? HttpRouteCollectionExtensions.CopiedCode.DefaultRoutePrefix;
        return HttpRouteCollectionExtensions.CopiedCode.MapResourceRouteInternal(routes, hostType, locationId, area, resource, routeTemplate, version1, version2, releasedApiVersion, defaultApiVersion, maxResourceVersion, apiVersionRequirement, defaults, constraints, routeName, handler, hostScopeTemplates, hostScopeTemplateOptional, routePrefix);
      }

      private static IHttpRoute MapResourceRouteInternal(
        HttpRouteCollection routes,
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
        if (routePrefix != HttpRouteCollectionExtensions.CopiedCode.DefaultRoutePrefix)
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
        string resourcePath = HttpRouteCollectionExtensions.CopiedCode.CreateResourcePath(routeTemplate, routePrefix);
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

      private static string CreateResourcePath(string resourcePath, string routePrefix = null)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(resourcePath, nameof (resourcePath));
        routePrefix = routePrefix ?? HttpRouteCollectionExtensions.CopiedCode.DefaultRoutePrefix;
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
    }
  }
}
