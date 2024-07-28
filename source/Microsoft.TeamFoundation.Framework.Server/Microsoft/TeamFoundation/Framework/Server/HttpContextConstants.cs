// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HttpContextConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class HttpContextConstants
  {
    public static readonly string RequestServiceHostId = nameof (RequestServiceHostId);
    public static readonly string ServiceHostRouteContext = nameof (ServiceHostRouteContext);
    public static readonly string UserRouteContext = nameof (UserRouteContext);
    public static readonly string IVssRequestContext = nameof (IVssRequestContext);
    public static readonly string ArrRequestRouted = nameof (ArrRequestRouted);
    public static readonly string OriginalVirtualDirectory = nameof (OriginalVirtualDirectory);
    public static readonly string UserIdentityInternal = nameof (UserIdentityInternal);
    public static readonly string WebApiScopePath = nameof (WebApiScopePath);
    public static readonly string DisallowCookies = nameof (DisallowCookies);
    public static readonly string ResolvedClientIp = "VSS_RESOLVED_CLIENT_IP";
    public static readonly string SecurityTracking = nameof (SecurityTracking);
    public static readonly string SecurityTrackingException = nameof (SecurityTrackingException);
    public static readonly string IsStaticContentRequest = nameof (IsStaticContentRequest);
    public static readonly string StaticContentRewritePath = nameof (StaticContentRewritePath);
    public static readonly string VssRewriteUrl = "X-VSS-RewriteUrl";
  }
}
