// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common.SmartRouterSettingsExtensions
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common
{
  internal static class SmartRouterSettingsExtensions
  {
    private const string c_configurationRoot = "/Configuration/SmartRouter";
    private const string c_serverDiscoveryRefreshPeriodPath = "/Configuration/SmartRouter/ServerDiscoveryRefreshPeriod";
    private const string c_serverHealthProbePeriodPath = "/Configuration/SmartRouter/ServerHealthProbePeriod";
    private const string c_serverHealthTimeToLivePath = "/Configuration/SmartRouter/ServerHealthTimeToLive";
    private const string c_serverPublishRefreshPeriodPath = "/Configuration/SmartRouter/ServerPublishRefreshPeriod";
    private const string c_serverTimeToLivePath = "/Configuration/SmartRouter/ServerTimeToLive";
    private const string c_minimumNodesPerHostPath = "/Configuration/SmartRouter/MinimumNodesPerHost";
    private const string c_minimumPercentNodesPerHostPath = "/Configuration/SmartRouter/MinimumPercentNodesPerHost";
    private const string c_minimumNodesPerScaleUnitPath = "/Configuration/SmartRouter/MinimumNodesPerScaleUnit";

    public static TimeSpan GetSmartRouterDiscoverRefreshPeriodSetting(
      this IVssRequestContext requestContext)
    {
      return requestContext.GetDeploymentRegistryValue<TimeSpan>((RegistryQuery) "/Configuration/SmartRouter/ServerDiscoveryRefreshPeriod", SmartRouterConstants.DefaultServerDiscoveryRefreshPeriod);
    }

    public static TimeSpan GetSmartRouterPublishRefreshPeriodSetting(
      this IVssRequestContext requestContext)
    {
      return requestContext.GetDeploymentRegistryValue<TimeSpan>((RegistryQuery) "/Configuration/SmartRouter/ServerPublishRefreshPeriod", SmartRouterConstants.DefaultServerPublishRefreshPeriod);
    }

    public static TimeSpan GetSmartRouterHealthProbePeriodSetting(
      this IVssRequestContext requestContext)
    {
      return requestContext.GetDeploymentRegistryValue<TimeSpan>((RegistryQuery) "/Configuration/SmartRouter/ServerHealthProbePeriod", SmartRouterConstants.DefaultServerHealthProbePeriod);
    }

    public static TimeSpan GetSmartRouterServerTimeToLiveSetting(
      this IVssRequestContext requestContext)
    {
      return requestContext.GetDeploymentRegistryValue<TimeSpan>((RegistryQuery) "/Configuration/SmartRouter/ServerTimeToLive", SmartRouterConstants.DefaultServerTimeToLive);
    }

    public static TimeSpan GetSmartRouterServerHealthTimeToLiveSetting(
      this IVssRequestContext requestContext)
    {
      return requestContext.GetDeploymentRegistryValue<TimeSpan>((RegistryQuery) "/Configuration/SmartRouter/ServerHealthTimeToLive", SmartRouterConstants.DefaultServerHealthTimeToLive);
    }

    public static int GetSmartRouterMinimumNodesPerHostSetting(
      this IVssRequestContext requestContext)
    {
      return requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/SmartRouter/MinimumNodesPerHost", true, 3);
    }

    public static int GetSmartRouterMinimumPercentNodesPerHostSetting(
      this IVssRequestContext requestContext)
    {
      return requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/SmartRouter/MinimumPercentNodesPerHost", true, 20);
    }

    public static int GetSmartRouterMinimumNodesPerScaleUnitSetting(
      this IVssRequestContext requestContext)
    {
      return requestContext.GetDeploymentRegistryValue<int>((RegistryQuery) "/Configuration/SmartRouter/MinimumNodesPerScaleUnit", 5);
    }

    private static T GetDeploymentRegistryValue<T>(
      this IVssRequestContext requestContext,
      RegistryQuery query,
      T? defaultValue = null)
    {
      IVssRequestContext deploymentHostContext = requestContext.ToDeploymentHostContext();
      return deploymentHostContext.GetService<IVssRegistryService>().GetValue<T>(deploymentHostContext, in query, defaultValue);
    }
  }
}
