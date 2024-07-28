// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProxyControllers.ProxyController
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Server.Core.ProxyControllers
{
  [ControllerApiVersion(1.0)]
  [ClientTemporarySwaggerExclusion]
  [VersionedApiControllerCustomName(Area = "core", ResourceName = "proxies")]
  public class ProxyController : ServerCoreApiController
  {
    [HttpGet]
    public List<Microsoft.TeamFoundation.Core.WebApi.Proxy> GetProxies(string proxyUrl = null)
    {
      TeamFoundationProxyService service = this.TfsRequestContext.GetService<TeamFoundationProxyService>();
      List<string> stringList = new List<string>();
      if (!string.IsNullOrWhiteSpace(proxyUrl))
        stringList.Add(proxyUrl);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      List<string> proxyUrls = stringList;
      return service.QueryProxies(tfsRequestContext, (IList<string>) proxyUrls);
    }
  }
}
