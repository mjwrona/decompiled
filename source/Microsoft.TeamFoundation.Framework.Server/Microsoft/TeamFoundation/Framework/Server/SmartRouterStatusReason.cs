// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SmartRouterStatusReason
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll


#nullable enable
namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class SmartRouterStatusReason
  {
    public const string IsArrRouted = "IsArrRouted";
    public const string IsArrForwardedFor = "IsArrForwardedFor";
    public const string IsDeploymentHost = "IsDeploymentHost";
    public const string IsNotApplicationTier = "IsNotApplicationTier";
    public const string IsSystemContext = "IsSystemContext";
    public const string IsStaticContent = "IsStaticContent";
    public const string IpValidationFailed = "IpValidationFailed";
    public const string RoutingPolicyDisabled = "RoutingPolicyDisabled";
    public const string NoServersAvailable = "NoServersAvailable";
    public const string InsufficientServersForSU = "InsufficientServersForSU";
    public const string InsufficientServersForHost = "InsufficientServersForHost";
    public const string NoSelectedTargetServer = "NoSelectedTargetServer";
    public const string ProxySent = "ProxySent";
    public const string NotWebContext = "!NotWebContext";
    public const string ReverseProxyToSelf = "!ReverseProxyToSelf";
    public const string ReverseProxyDisabled = "!ReverseProxyDisabled";
  }
}
