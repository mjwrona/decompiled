// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AspNetRequestContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;
using System.Net;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class AspNetRequestContext : WebRequestContext, ITrackClientConnection
  {
    private bool m_endRequestCalled;
    private HttpContextBase m_httpContext;
    private static readonly string s_Area = "HostManagement";
    private static readonly string s_Layer = nameof (AspNetRequestContext);

    public AspNetRequestContext(
      IVssServiceHost serviceHost,
      RequestContextType requestContextType,
      HttpContextBase httpContext,
      LockHelper helper,
      TimeSpan timeout)
      : base(serviceHost, requestContextType, httpContext, helper, timeout)
    {
      this.m_endRequestCalled = false;
      this.m_httpContext = httpContext;
      httpContext.Response.ClientDisconnectedToken.Register((Action) (() => this.Cancel(FrameworkResources.ClientDisconnectedCancelReason(), HttpStatusCode.RequestTimeout)), false);
      this.IsTracked = true;
    }

    public override bool GetSessionValue(string sessionKey, out string sessionValue)
    {
      HttpCookie cookie = this.HttpContext.Request.Cookies[this.GetCookieFromSessionKey(sessionKey)];
      if (cookie != null)
      {
        sessionValue = cookie.Value;
        return true;
      }
      sessionValue = string.Empty;
      return false;
    }

    public override bool SetSessionValue(string sessionKey, string sessionValue)
    {
      if (this.HttpContext.Items.Contains((object) HttpContextConstants.DisallowCookies))
        return false;
      this.HttpContext.Response.Cookies.Add(new HttpCookie(this.GetCookieFromSessionKey(sessionKey), sessionValue)
      {
        Secure = this.HttpContext.Request.IsSecureConnection
      });
      return true;
    }

    public override void PartialResultsReady()
    {
      this.HttpContext.Response.BufferOutput = false;
      this.RequestTimer.SetTimeToFirstPageEnd();
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "AspRequestContext[{0} {1}, RemoteComputer: {2}, RemoteIPAddress: {3}, RemotePort: {4}, UserAgent: {5}]", (object) this.HttpMethod, (object) this.RequestUri, (object) this.RemoteComputer, (object) this.RemoteIPAddress, (object) this.RemotePort, (object) this.UserAgent);

    protected override void EndRequest()
    {
      if (this.m_endRequestCalled)
        return;
      this.m_endRequestCalled = true;
      this.RequestTracer.TraceEnter(36108, AspNetRequestContext.s_Area, AspNetRequestContext.s_Layer, nameof (EndRequest));
      try
      {
        base.EndRequest();
      }
      finally
      {
        this.RequestTracer.TraceLeave(36102, AspNetRequestContext.s_Area, AspNetRequestContext.s_Layer, nameof (EndRequest));
      }
    }

    bool ITrackClientConnection.IsClientConnected => this.m_httpContext != null && this.m_httpContext.Response != null && this.m_httpContext.Response.IsClientConnected;

    private string GetCookieFromSessionKey(string sessionKey) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Tfs-{0}", (object) sessionKey);
  }
}
