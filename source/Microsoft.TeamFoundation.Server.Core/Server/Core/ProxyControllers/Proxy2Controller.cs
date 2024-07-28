// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProxyControllers.Proxy2Controller
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Server.Core.ProxyControllers
{
  [ControllerApiVersion(3.1)]
  [VersionedApiControllerCustomName(Area = "core", ResourceName = "proxies", ResourceVersion = 2)]
  public class Proxy2Controller : ProxyController
  {
    private static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (InvalidProxyUriException),
        HttpStatusCode.BadRequest
      }
    };

    [HttpPut]
    public Microsoft.TeamFoundation.Core.WebApi.Proxy CreateOrUpdateProxy(Microsoft.TeamFoundation.Core.WebApi.Proxy proxy) => this.TfsRequestContext.GetService<TeamFoundationProxyService>().AddProxy(this.TfsRequestContext, proxy);

    [HttpDelete]
    public void DeleteProxy(string proxyUrl, string site = null) => this.TfsRequestContext.GetService<TeamFoundationProxyService>().DeleteProxy(this.TfsRequestContext, proxyUrl, site);

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) Proxy2Controller.s_httpExceptions;
  }
}
