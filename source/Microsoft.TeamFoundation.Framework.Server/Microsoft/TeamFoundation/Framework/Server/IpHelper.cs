// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IpHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using Microsoft.TeamFoundation.Framework.Server.HttpModule.IpResolution;
using Microsoft.TeamFoundation.Framework.Server.SmartRouter.IpResolution;
using System.Collections.Generic;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class IpHelper
  {
    private static readonly IClientIpResolver _clientIpResolver = (IClientIpResolver) new ClientIpResolver((IAfdIpValidator) new AfdIpValidator(ConfigProxy.Create<List<IPv4Subnet>>(AfdSubnetConfig.AfdSubnetsPrototype)), (IArrForwardedIpValidator) new ArrForwardedIpValidator(), (ISmartRouterForwardedIpValidator) new SmartRouterForwardedIpValidator(), (IIpResolutionMetricCollector) new IpResolutionMetricCollector());

    public static string ResolveClientIp(
      IVssRequestContext requestContext,
      HttpRequestBase httpRequest)
    {
      return IpHelper._clientIpResolver.Resolve(requestContext, httpRequest);
    }
  }
}
