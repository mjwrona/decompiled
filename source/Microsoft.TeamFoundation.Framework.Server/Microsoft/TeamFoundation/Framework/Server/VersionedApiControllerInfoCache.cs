// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VersionedApiControllerInfoCache
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class VersionedApiControllerInfoCache
  {
    private const string c_controllerSuffix = "Controller";
    private const string c_layer = "Routing";
    private static ConcurrentDictionary<string, List<VersionedApiControllerInfo>> s_cache = new ConcurrentDictionary<string, List<VersionedApiControllerInfo>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static bool s_warnOnFeatureFlagUsage = true;

    public static List<VersionedApiControllerInfo> GetControllerInfos(
      IVssRequestContext requestContext,
      string area,
      string resource)
    {
      string key = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) area, (object) resource);
      List<VersionedApiControllerInfo> controllerInfos1;
      if (VersionedApiControllerInfoCache.s_cache.TryGetValue(key, out controllerInfos1))
        return controllerInfos1;
      HttpConfiguration httpConfiguration = WebApiConfiguration.GetHttpConfiguration(requestContext);
      IHttpControllerTypeResolver controllerTypeResolver = httpConfiguration.Services.GetHttpControllerTypeResolver();
      IAssembliesResolver assembliesResolver = httpConfiguration.Services.GetAssembliesResolver();
      if (VersionedApiControllerInfoCache.s_warnOnFeatureFlagUsage && requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        VersionedApiControllerInfoCache.s_warnOnFeatureFlagUsage = false;
      List<VersionedApiControllerInfo> controllerInfos2 = new List<VersionedApiControllerInfo>();
      foreach (Type controllerType in (IEnumerable<Type>) controllerTypeResolver.GetControllerTypes(assembliesResolver))
      {
        string featureFlag = ((IEnumerable<object>) controllerType.GetCustomAttributes(typeof (FeatureEnabledAttribute), false)).FirstOrDefault<object>() is FeatureEnabledAttribute enabledAttribute ? enabledAttribute.FeatureFlag : (string) null;
        if (VersionedApiControllerInfoCache.s_warnOnFeatureFlagUsage && !string.IsNullOrEmpty(featureFlag) && !requestContext.IsFeatureEnabled(featureFlag))
          requestContext.TraceAlways(393096681, TraceLevel.Warning, area, "Routing", "AzDevNext: Controller " + controllerType.FullName + " disabled by feature flag " + featureFlag);
        if (((IEnumerable<object>) controllerType.GetCustomAttributes(typeof (VersionedApiControllerCustomNameAttribute), false)).FirstOrDefault<object>() is VersionedApiControllerCustomNameAttribute customNameAttribute)
        {
          if (string.Equals(customNameAttribute.Area, area, StringComparison.OrdinalIgnoreCase) && string.Equals(customNameAttribute.ResourceName, resource, StringComparison.OrdinalIgnoreCase))
            controllerInfos2.Add(new VersionedApiControllerInfo()
            {
              AreaName = area,
              ResourceName = resource,
              ControllerName = VersionedApiControllerInfoCache.StripControllerSuffix(controllerType.Name),
              FeatureFlag = featureFlag,
              Version = VersionedApiControllerInfoCache.GetControllerVersion(controllerType, customNameAttribute.ResourceVersion)
            });
        }
        else
        {
          string str = area + resource;
          if (controllerType.Name.StartsWith(str, StringComparison.OrdinalIgnoreCase) && controllerType.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase))
          {
            int result = 0;
            if (controllerType.Name.Length == str.Length + "Controller".Length)
              result = 1;
            else
              int.TryParse(controllerType.Name.Substring(str.Length, controllerType.Name.Length - "Controller".Length - str.Length), out result);
            if (result > 0)
              controllerInfos2.Add(new VersionedApiControllerInfo()
              {
                AreaName = area,
                ResourceName = resource,
                ControllerName = VersionedApiControllerInfoCache.StripControllerSuffix(controllerType.Name),
                FeatureFlag = featureFlag,
                Version = VersionedApiControllerInfoCache.GetControllerVersion(controllerType, result)
              });
          }
        }
      }
      VersionedApiControllerInfoCache.s_warnOnFeatureFlagUsage = false;
      VersionedApiControllerInfoCache.s_cache.TryAdd(key, controllerInfos2);
      return controllerInfos2;
    }

    public static VersionedApiControllerInfo GetControllerInfo(
      string area,
      string resource,
      ApiResourceVersion requestVersion,
      IVssRequestContext requestContext)
    {
      List<VersionedApiControllerInfo> controllerInfos = VersionedApiControllerInfoCache.GetControllerInfos(requestContext, area, resource);
      return VersionedApiControllerInfoCache.GetControllerInfo(requestVersion, requestContext, (IEnumerable<VersionedApiControllerInfo>) controllerInfos);
    }

    public static VersionedApiControllerInfo GetControllerInfo(
      ApiResourceVersion requestVersion,
      IVssRequestContext requestContext,
      IEnumerable<VersionedApiControllerInfo> candidateControllers)
    {
      VersionedApiControllerInfo controllerInfo = (VersionedApiControllerInfo) null;
      foreach (VersionedApiControllerInfo candidateController in candidateControllers)
      {
        if (!(candidateController.Version.ApiVersion > requestVersion.ApiVersion) && (controllerInfo == null || !(candidateController.Version.ApiVersion < controllerInfo.Version.ApiVersion)) && (string.IsNullOrEmpty(candidateController.FeatureFlag) || requestContext.IsFeatureEnabled(candidateController.FeatureFlag)) && (controllerInfo == null || controllerInfo.Version.ApiVersion < candidateController.Version.ApiVersion || controllerInfo.Version.ResourceVersion < candidateController.Version.ResourceVersion) && (requestVersion.ResourceVersion == 0 || candidateController.Version.ResourceVersion == requestVersion.ResourceVersion))
          controllerInfo = candidateController;
      }
      return controllerInfo;
    }

    private static string StripControllerSuffix(string controllerTypeName) => controllerTypeName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase) ? controllerTypeName.Substring(0, controllerTypeName.Length - "Controller".Length) : controllerTypeName;

    private static ApiResourceVersion GetControllerVersion(Type controllerType, int resourceVersion) => new ApiResourceVersion(!(((IEnumerable<object>) controllerType.GetCustomAttributes(typeof (ControllerApiVersionAttribute), true)).FirstOrDefault<object>() is ControllerApiVersionAttribute versionAttribute) ? new Version(1, 0) : new Version(versionAttribute.ApiVersion.ToString("0.0", (IFormatProvider) CultureInfo.InvariantCulture)), resourceVersion);
  }
}
