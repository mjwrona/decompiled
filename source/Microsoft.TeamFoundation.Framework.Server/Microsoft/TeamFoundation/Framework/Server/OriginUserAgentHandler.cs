// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OriginUserAgentHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class OriginUserAgentHandler : DelegatingHandler
  {
    private readonly string _clientUserAgent;

    internal OriginUserAgentHandler(IVssRequestContext requestContext)
    {
      HttpRequest request = HttpContext.Current?.Request;
      if (request == null)
        return;
      this._clientUserAgent = OriginUserAgentHandler.ExtractUserAgent((HttpRequestBase) new HttpRequestWrapper(request), requestContext);
    }

    internal static string ExtractUserAgent(
      HttpRequestBase httpRequest,
      IVssRequestContext requestContext)
    {
      string userAgent = httpRequest.Headers["X-VSS-OriginUserAgent"];
      if (string.IsNullOrEmpty(userAgent))
        userAgent = requestContext.UserAgent;
      return userAgent;
    }

    protected override Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      if (!string.IsNullOrEmpty(this._clientUserAgent))
        request.Headers.Add("X-VSS-OriginUserAgent", this._clientUserAgent);
      return base.SendAsync(request, cancellationToken);
    }
  }
}
