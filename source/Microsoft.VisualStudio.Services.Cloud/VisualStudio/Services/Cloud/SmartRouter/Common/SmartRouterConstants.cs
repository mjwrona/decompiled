// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common.SmartRouterConstants
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common
{
  internal static class SmartRouterConstants
  {
    internal const string ServiceName = "SmartRouter";
    internal const string SmartRouterReverseProxyEnabledFeatureFlag = "VisualStudio.Services.Cloud.SmartRouter.ReverseProxy.Enabled";
    internal const string MinimumPercentNodesPerHostFeatureFlag = "VisualStudio.Services.Cloud.SmartRouter.MinimumPercentNodesPerHost.Enabled";
    internal const string ConsistentHashRoutingPolicyFeatureFlag = "VisualStudio.Services.Cloud.SmartRouter.RoutingPolicy.ConsistentHash.Enabled";
    internal const string SmartRouted = "SmartRouted";
    public const string RequestSmartRoutedHeader = "X-VSS-RequestSmartRouted";
    public const string RequestSmartRouterForwardedFor = "X-SmartRouter-Forwarded-For";
    internal const string ATRoleName = "AT";
    internal const string JobAgentRoleName = "JobAgent";
    internal static readonly TimeSpan DefaultServerDiscoveryRefreshPeriod = TimeSpan.FromMinutes(1.0);
    internal static readonly TimeSpan DefaultServerPublishRefreshPeriod = TimeSpan.FromMinutes(3.0);
    internal static readonly TimeSpan DefaultServerHealthProbePeriod = TimeSpan.FromSeconds(5.0);
    internal static readonly TimeSpan DefaultServerHealthTimeToLive = TimeSpan.FromMinutes(1.0);
    internal static readonly TimeSpan DefaultServerTimeToLive = TimeSpan.FromMinutes(5.0);
    internal const string HealthEndpointPath = "_apis/health";
    internal const int DefaultMinimumNodesPerHost = 3;
    internal const int DefaultMinimumPercentNodesPerHost = 20;
    internal const int DefaultMinimumNodesPerScaleUnit = 5;
  }
}
