// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VersionedApiResourceConstraint
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class VersionedApiResourceConstraint : IHttpRouteConstraint
  {
    internal const string ApiResourceVersionRouteKey = "apiResourceVersion";
    private const string c_controllerRouteKey = "controller";
    private const string c_resourceRouteKey = "resource";
    private const string c_areaRouteKey = "area";
    private const string c_apiResourceVersionExceptionRouteKey = "apiResourceVersionException";
    private static readonly HashSet<string> s_editVerbsRequiringVersion = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "post",
      "put",
      "patch",
      "delete"
    };
    private Version m_defaultApiVersion;
    private Version m_minApiVersion;
    private Version m_maxApiVersion;
    private Version m_releasedApiVersion;

    public VersionedApiResourceConstraint(
      Version defaultApiVersion,
      Version minApiVersion,
      Version maxApiVersion,
      Version releasedApiVersion)
    {
      this.m_defaultApiVersion = defaultApiVersion;
      this.m_minApiVersion = minApiVersion;
      this.m_maxApiVersion = maxApiVersion;
      this.m_releasedApiVersion = releasedApiVersion;
      this.ApiVersionRequirement = RequestApiVersionRequirement.RequiredOnEditOperations;
    }

    public RequestApiVersionRequirement ApiVersionRequirement { get; set; }

    public bool Match(
      HttpRequestMessage request,
      IHttpRoute route,
      string parameterName,
      IDictionary<string, object> values,
      HttpRouteDirection routeDirection)
    {
      bool flag1;
      if (request.Method == HttpMethod.Options && routeDirection == HttpRouteDirection.UriResolution && (!route.Defaults.TryGetValue<bool>("allowHttpOptions", out flag1) || !flag1))
        return false;
      string routeValue1 = this.GetRouteValue<string>(route, values, "area", (string) null);
      string routeValue2 = this.GetRouteValue<string>(route, values, "resource", (string) null);
      if (string.IsNullOrEmpty(routeValue2) || string.IsNullOrEmpty(routeValue1))
        return false;
      ApiResourceVersion apiResourceVersion1 = (ApiResourceVersion) null;
      try
      {
        apiResourceVersion1 = request.GetApiResourceVersion();
        if (apiResourceVersion1 == null)
        {
          if (this.ApiVersionRequirement != RequestApiVersionRequirement.RequiredOnAllRequests)
          {
            if (this.ApiVersionRequirement == RequestApiVersionRequirement.RequiredOnEditOperations)
            {
              if (!VersionedApiResourceConstraint.s_editVerbsRequiringVersion.Contains(request.Method.Method))
                goto label_11;
            }
            else
              goto label_11;
          }
          values["apiResourceVersionException"] = (object) new VssVersionNotSpecifiedException(request.Method.Method);
        }
      }
      catch (VssResourceVersionException ex)
      {
        values["apiResourceVersionException"] = (object) ex;
      }
label_11:
      if (apiResourceVersion1 != null)
      {
        Version latestVersion = VssRestApiVersionsRegistry.GetLatestVersion();
        if (apiResourceVersion1.ApiVersion > latestVersion)
        {
          values["apiResourceVersionException"] = (object) new VssVersionOutOfRangeException(apiResourceVersion1.ApiVersion, latestVersion);
          apiResourceVersion1 = (ApiResourceVersion) null;
        }
      }
      if (apiResourceVersion1 == null)
      {
        apiResourceVersion1 = new ApiResourceVersion(this.m_defaultApiVersion);
        if (this.m_defaultApiVersion > this.m_releasedApiVersion)
          apiResourceVersion1.IsPreview = true;
      }
      if (apiResourceVersion1.ApiVersion > this.m_maxApiVersion || apiResourceVersion1.ApiVersion < this.m_minApiVersion)
        return false;
      bool flag2 = apiResourceVersion1.ApiVersion > this.m_releasedApiVersion;
      if (flag2 && !apiResourceVersion1.IsPreview && apiResourceVersion1.ApiVersion > new Version(1, 0))
        values["apiResourceVersionException"] = (object) new VssInvalidPreviewVersionException(apiResourceVersion1);
      IVssRequestContext requestContext = request.GetIVssRequestContext();
      List<VersionedApiControllerInfo> controllerInfos = VersionedApiControllerInfoCache.GetControllerInfos(requestContext, routeValue1, routeValue2);
      if (!controllerInfos.Any<VersionedApiControllerInfo>())
        return false;
      VersionedApiControllerInfo controllerInfo = VersionedApiControllerInfoCache.GetControllerInfo(apiResourceVersion1, requestContext, (IEnumerable<VersionedApiControllerInfo>) controllerInfos);
      if (controllerInfo != null)
      {
        values["controller"] = (object) controllerInfo.ControllerName;
        ApiResourceVersion apiResourceVersion2 = new ApiResourceVersion(apiResourceVersion1.ApiVersion);
        if (flag2 || apiResourceVersion1.IsPreview)
        {
          apiResourceVersion2.IsPreview = true;
          apiResourceVersion2.ResourceVersion = controllerInfo.Version.ResourceVersion;
        }
        values["apiResourceVersion"] = (object) apiResourceVersion2;
        return true;
      }
      if (controllerInfos.Any<VersionedApiControllerInfo>((Func<VersionedApiControllerInfo, bool>) (c => string.IsNullOrEmpty(c.FeatureFlag) || requestContext.IsFeatureEnabled(c.FeatureFlag))))
      {
        List<string> stringList;
        if (!requestContext.Items.TryGetValue<List<string>>(RequestContextItemsKeys.RoutesMatchedExceptVersion, out stringList))
        {
          stringList = new List<string>();
          requestContext.Items[RequestContextItemsKeys.RoutesMatchedExceptVersion] = (object) stringList;
        }
        stringList.Add(routeValue1 + "/" + routeValue2);
      }
      return false;
    }

    internal static ApiResourceVersion GetApiVersionFromRouteData(IHttpRouteData routeData)
    {
      ApiResourceVersion versionFromRouteData = (ApiResourceVersion) null;
      if (routeData != null)
        routeData.Values.TryGetValue<ApiResourceVersion>("apiResourceVersion", out versionFromRouteData);
      return versionFromRouteData;
    }

    internal static string GetAreaFromRouteData(IHttpRouteData routeData)
    {
      string areaFromRouteData = (string) null;
      if (routeData != null)
        routeData.Values.TryGetValue<string>("area", out areaFromRouteData);
      return areaFromRouteData;
    }

    internal static string GetResourceFromRouteData(IHttpRouteData routeData)
    {
      string resourceFromRouteData = (string) null;
      if (routeData != null)
        routeData.Values.TryGetValue<string>("resource", out resourceFromRouteData);
      return resourceFromRouteData;
    }

    internal static VssResourceVersionException GetApiVersionExceptionFromRouteData(
      IHttpRouteData routeData)
    {
      VssResourceVersionException exceptionFromRouteData = (VssResourceVersionException) null;
      if (routeData != null)
        routeData.Values.TryGetValue<VssResourceVersionException>("apiResourceVersionException", out exceptionFromRouteData);
      return exceptionFromRouteData;
    }

    private T GetRouteValue<T>(
      IHttpRoute route,
      IDictionary<string, object> routeValues,
      string key,
      T defaultValue)
    {
      object obj1;
      return (route.RouteTemplate.Contains(key) || route.Constraints.ContainsKey(key) || route.Defaults.ContainsKey(key)) && routeValues.TryGetValue(key, out obj1) && obj1 is T obj2 ? obj2 : defaultValue;
    }
  }
}
