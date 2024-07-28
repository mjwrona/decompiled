// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AfdClientIpHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class AfdClientIpHandler : DelegatingHandler
  {
    private readonly string _clientIp;

    internal AfdClientIpHandler(IVssRequestContext requestContext)
    {
      HttpRequest request = HttpContext.Current?.Request;
      if (request == null)
        return;
      this._clientIp = AfdClientIpHandler.ExtractIp((HttpRequestBase) new HttpRequestWrapper(request), requestContext);
    }

    internal static string ExtractIp(HttpRequestBase httpRequest, IVssRequestContext requestContext)
    {
      string ip = httpRequest.Headers["X-FD-ClientIP"];
      if (string.IsNullOrEmpty(ip))
        ip = httpRequest.Headers["X-MSEdge-ClientIP"];
      if (string.IsNullOrEmpty(ip))
        ip = IpHelper.ResolveClientIp(requestContext, httpRequest);
      return ip;
    }

    protected override Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      if (!string.IsNullOrEmpty(this._clientIp))
        request.Headers.Add("X-MSEdge-ClientIP", this._clientIp);
      return base.SendAsync(request, cancellationToken);
    }
  }
}
