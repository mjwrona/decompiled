// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RouteKeyVersion0Handler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class RouteKeyVersion0Handler : IRouteKeyVersionHandler
  {
    public int Version => 0;

    public bool ShouldUpdateEndpointForRouteKey(
      IVssRequestContext requestContext,
      Uri requestUri,
      RouteFlags routeFlags,
      IProxyData proxyData)
    {
      return !routeFlags.HasFlag((Enum) RouteFlags.AnyHost) || proxyData == null || string.Equals(requestUri.Host, new Uri(proxyData.TargetPublicUrl).Host, StringComparison.OrdinalIgnoreCase);
    }
  }
}
