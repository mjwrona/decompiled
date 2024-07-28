// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common.SmartRouterFeatureFlagExtensions
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common
{
  internal static class SmartRouterFeatureFlagExtensions
  {
    public static bool IsSmartRouterFeatureEnabled(this IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.Cloud.SmartRouter.Enabled");

    public static bool IsSmartRouterReverseProxyFeatureEnabled(
      this IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("VisualStudio.Services.Cloud.SmartRouter.ReverseProxy.Enabled");
    }

    public static bool IsMinimumPercentNodesPerHostFeatureEnabled(
      this IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("VisualStudio.Services.Cloud.SmartRouter.MinimumPercentNodesPerHost.Enabled");
    }

    public static bool IsSmartRouterForwardingIpValidationEnabled(
      this IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("VisualStudio.Services.Cloud.SmartRouter.ForwardingIpValidation.Enabled");
    }
  }
}
