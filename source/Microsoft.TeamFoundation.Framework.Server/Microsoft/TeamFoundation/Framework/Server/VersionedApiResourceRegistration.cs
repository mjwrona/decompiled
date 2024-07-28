// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VersionedApiResourceRegistration
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Routing;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class VersionedApiResourceRegistration
  {
    private const string c_versionedControllerConstraintKey = "VersionedControllerConstraint";
    private const string c_noHostScopePathRouteNameSuffix = "-NoHostScope";
    private static int s_changeVersion;
    private static ApiResourceLocationCollection s_resourceLocations = new ApiResourceLocationCollection();
    private static Dictionary<TeamFoundationHostType, ApiResourceLocationCollection> s_resourceLocationsByHostType = new Dictionary<TeamFoundationHostType, ApiResourceLocationCollection>();
    private static Dictionary<Guid, IHttpRoute> s_routesByLocation = new Dictionary<Guid, IHttpRoute>();

    static VersionedApiResourceRegistration()
    {
      VersionedApiResourceRegistration.s_resourceLocationsByHostType.Add(TeamFoundationHostType.Deployment, new ApiResourceLocationCollection());
      VersionedApiResourceRegistration.s_resourceLocationsByHostType.Add(TeamFoundationHostType.Application, new ApiResourceLocationCollection());
      VersionedApiResourceRegistration.s_resourceLocationsByHostType.Add(TeamFoundationHostType.ProjectCollection, new ApiResourceLocationCollection());
    }

    public static int ChangeVersion => VersionedApiResourceRegistration.s_changeVersion;

    public static ApiResourceLocationCollection ResourceLocations => VersionedApiResourceRegistration.s_resourceLocations;

    internal static ApiResourceLocationCollection GetLocationsForHostType(
      TeamFoundationHostType hostType)
    {
      ApiResourceLocationCollection locationsForHostType;
      if (VersionedApiResourceRegistration.s_resourceLocationsByHostType.TryGetValue(hostType, out locationsForHostType))
        return locationsForHostType;
      throw new ArgumentException(string.Format("Invalid hostType {0} provided to GetLocationsForHostType. Must be a single scope.", (object) hostType), nameof (hostType));
    }

    public static ApiResourceLocationCollection GetLocationsForHostType(
      IVssRequestContext requestContext)
    {
      TeamFoundationHostType hostType = requestContext.ServiceHost.HostType;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        hostType = TeamFoundationHostType.Deployment;
      TeamFoundationHostType foundationHostType = TeamFoundationHostType.Application | TeamFoundationHostType.Deployment;
      if ((hostType & foundationHostType) == foundationHostType)
      {
        ApiResourceLocationCollection locationsForHostType1 = VersionedApiResourceRegistration.GetLocationsForHostType(TeamFoundationHostType.Deployment);
        ApiResourceLocationCollection locationsForHostType2 = VersionedApiResourceRegistration.GetLocationsForHostType(TeamFoundationHostType.Application);
        ApiResourceLocationCollection locationsForHostType3 = new ApiResourceLocationCollection();
        locationsForHostType3.AddResourceLocations(locationsForHostType2.GetAllLocations());
        locationsForHostType3.AddResourceLocations(locationsForHostType1.GetAllLocations());
        return locationsForHostType3;
      }
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment || !requestContext.RootContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return VersionedApiResourceRegistration.GetLocationsForHostType(hostType);
      ApiResourceLocationCollection locationsForHostType4 = VersionedApiResourceRegistration.GetLocationsForHostType(TeamFoundationHostType.Application);
      ApiResourceLocationCollection locationsForHostType5 = VersionedApiResourceRegistration.GetLocationsForHostType(TeamFoundationHostType.ProjectCollection);
      ApiResourceLocationCollection locationsForHostType6 = new ApiResourceLocationCollection();
      locationsForHostType6.AddResourceLocations(locationsForHostType4.GetAllLocations());
      locationsForHostType6.AddResourceLocations(locationsForHostType5.GetAllLocations());
      return locationsForHostType6;
    }

    public static ApiResourceLocationCollection GetLocationsForAllHostTypes(
      IVssRequestContext requestContext)
    {
      ApiResourceLocationCollection locationsForAllHostTypes = new ApiResourceLocationCollection();
      locationsForAllHostTypes.AddResourceLocations(VersionedApiResourceRegistration.GetLocationsForHostType(TeamFoundationHostType.Deployment).GetAllLocations());
      locationsForAllHostTypes.AddResourceLocations(VersionedApiResourceRegistration.GetLocationsForHostType(TeamFoundationHostType.Application).GetAllLocations());
      locationsForAllHostTypes.AddResourceLocations(VersionedApiResourceRegistration.GetLocationsForHostType(TeamFoundationHostType.ProjectCollection).GetAllLocations());
      return locationsForAllHostTypes;
    }

    public static IHttpRoute RegisterResource(
      TeamFoundationHostType hostType,
      HttpRouteCollection routes,
      Guid locationId,
      string area,
      string resource,
      string routeTemplate,
      Version minApiVersion,
      Version maxApiVersion,
      Version releasedApiVersion,
      Version defaultApiVersion,
      int maxResourceVersion = 1,
      RequestApiVersionRequirement apiVersionRequirement = RequestApiVersionRequirement.RequiredOnEditOperations,
      object defaults = null,
      object constraints = null,
      HttpMessageHandler handler = null,
      string routeName = null,
      IEnumerable<string> hostScopeTemplates = null,
      bool hostScopeTemplateOptional = false)
    {
      ArgumentUtility.CheckForNull<Version>(minApiVersion, nameof (minApiVersion));
      ArgumentUtility.CheckForNull<Version>(maxApiVersion, nameof (maxApiVersion));
      ArgumentUtility.CheckForNull<Version>(releasedApiVersion, nameof (releasedApiVersion));
      ArgumentUtility.CheckForNull<Version>(defaultApiVersion, nameof (defaultApiVersion));
      string name = string.Format("{0}:{1}", (object) area, (object) resource);
      if (!string.IsNullOrEmpty(routeName))
        name = name + " " + routeName;
      HttpRouteValueDictionary routeDictionary = HttpRouteValueExtensions.ToRouteDictionary(defaults);
      routeDictionary.AddRouteValueIfNotPresent(nameof (area), (object) area);
      routeDictionary.AddRouteValueIfNotPresent("controller", (object) resource);
      routeDictionary.AddRouteValueIfNotPresent("/locationId", (object) locationId);
      HttpRouteValueDictionary constraints1 = new HttpRouteValueDictionary();
      if (routeTemplate != null)
      {
        if (routeTemplate.IndexOf("{area}", StringComparison.OrdinalIgnoreCase) >= 0)
          constraints1.Add(nameof (area), (object) area);
        if (routeTemplate.IndexOf("{resource}", StringComparison.OrdinalIgnoreCase) >= 0)
          constraints1.Add(nameof (resource), (object) resource);
        else
          routeDictionary.AddRouteValueIfNotPresent(nameof (resource), (object) resource);
      }
      foreach (KeyValuePair<string, object> route in (Dictionary<string, object>) HttpRouteValueExtensions.ToRouteDictionary(constraints))
        constraints1[route.Key] = route.Value;
      if (!constraints1.ContainsKey("VersionedControllerConstraint"))
        constraints1.Add("VersionedControllerConstraint", (object) new VersionedApiResourceConstraint(defaultApiVersion, minApiVersion, maxApiVersion, releasedApiVersion)
        {
          ApiVersionRequirement = apiVersionRequirement
        });
      IHttpRoute httpRoute = (IHttpRoute) null;
      if (hostScopeTemplates != null && hostScopeTemplates.Count<string>() > 0)
      {
        if (hostScopeTemplateOptional)
          VersionedApiResourceRegistration.MapHttpRoute(routes, name + "-NoHostScope", routeTemplate, routeDictionary, constraints1, handler);
        string str1 = routeTemplate;
        string str2 = name;
        foreach (string hostScopeTemplate in hostScopeTemplates)
        {
          routeTemplate = hostScopeTemplate.TrimEnd('/') + "/" + str1.TrimStart('/');
          name = str2 + "/" + hostScopeTemplate.Trim('/');
          httpRoute = VersionedApiResourceRegistration.MapHttpRoute(routes, name, routeTemplate, routeDictionary, constraints1, handler);
        }
      }
      else
        httpRoute = VersionedApiResourceRegistration.MapHttpRoute(routes, name, routeTemplate, routeDictionary, constraints1, handler);
      VersionedApiResourceRegistration.s_routesByLocation[locationId] = httpRoute;
      ApiResourceLocation location = new ApiResourceLocation()
      {
        Id = locationId,
        Area = area,
        ResourceName = resource,
        RouteTemplate = routeTemplate,
        RouteName = name,
        ResourceVersion = maxResourceVersion,
        MinVersion = minApiVersion,
        MaxVersion = maxApiVersion,
        ReleasedVersion = releasedApiVersion
      };
      VersionedApiResourceRegistration.ResourceLocations.AddResourceLocation(location);
      if (hostType.HasFlag((Enum) TeamFoundationHostType.Deployment))
        VersionedApiResourceRegistration.s_resourceLocationsByHostType[TeamFoundationHostType.Deployment].AddResourceLocation(location);
      if (hostType.HasFlag((Enum) TeamFoundationHostType.Application))
        VersionedApiResourceRegistration.s_resourceLocationsByHostType[TeamFoundationHostType.Application].AddResourceLocation(location);
      if (hostType.HasFlag((Enum) TeamFoundationHostType.ProjectCollection))
        VersionedApiResourceRegistration.s_resourceLocationsByHostType[TeamFoundationHostType.ProjectCollection].AddResourceLocation(location);
      Interlocked.Increment(ref VersionedApiResourceRegistration.s_changeVersion);
      return httpRoute;
    }

    public static IHttpRoute GetRouteForLocation(Guid locationId)
    {
      IHttpRoute routeForLocation;
      VersionedApiResourceRegistration.s_routesByLocation.TryGetValue(locationId, out routeForLocation);
      return routeForLocation;
    }

    private static IHttpRoute MapHttpRoute(
      HttpRouteCollection routes,
      string name,
      string routeTemplate,
      HttpRouteValueDictionary defaults,
      HttpRouteValueDictionary constraints,
      HttpMessageHandler handler)
    {
      if (routes.ContainsKey(name))
        throw new VssApiResourceDuplicateRouteNameException(name);
      IHttpRoute route = (IHttpRoute) new VssfWebApiRoute(routeTemplate, defaults, constraints, (HttpRouteValueDictionary) null, handler);
      routes.Add(name, route);
      return route;
    }
  }
}
