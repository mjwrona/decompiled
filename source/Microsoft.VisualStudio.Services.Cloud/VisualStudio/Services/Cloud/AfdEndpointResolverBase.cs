// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AfdEndpointResolverBase
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Internal;
using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal abstract class AfdEndpointResolverBase : IAfdEndpointResolver
  {
    public bool TryGetEndpoint(
      IVssRequestContext requestContext,
      string routeKey,
      out string endpoint)
    {
      Uri hostUri = this.ToHostUri(requestContext, routeKey);
      HostRouteContext hostRouteContext = requestContext.GetService<IUrlHostResolutionService>().ResolveHost(requestContext, hostUri);
      if (hostRouteContext != null)
      {
        HostProxyData hostProxyData = requestContext.GetService<HostRoutingCacheService>().Get(requestContext, hostRouteContext.HostId, hostRouteContext.RouteFlags);
        if (hostProxyData != null)
        {
          endpoint = new Uri(hostProxyData.TargetInstanceUrl).Host;
          return true;
        }
      }
      endpoint = (string) null;
      return false;
    }

    protected abstract Uri ToHostUri(IVssRequestContext requestContext, string routeKey);
  }
}
