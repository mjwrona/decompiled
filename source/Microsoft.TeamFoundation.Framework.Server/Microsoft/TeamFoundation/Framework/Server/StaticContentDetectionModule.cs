// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StaticContentDetectionModule
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class StaticContentDetectionModule : IHttpModule
  {
    private const string c_staticSegment = "/_static/";
    private const string c_cssBundlesSegment = "/_cssbundles/";

    public void Init(HttpApplication context) => context.BeginRequest += new EventHandler(this.OnBeginRequest);

    public void Dispose()
    {
    }

    private void OnBeginRequest(object sender, EventArgs e)
    {
      HttpContext context = ((HttpApplication) sender).Context;
      if (!(context.Items[(object) HttpContextConstants.ServiceHostRouteContext] is HostRouteContext hostRouteContext))
        return;
      string rewrittenPath;
      if (StaticContentDetectionModule.IsStaticContentRequest(context.Request.Url.AbsolutePath, hostRouteContext.VirtualPath, UrlHostResolutionService.ApplicationVirtualPath, out rewrittenPath))
        context.Items[(object) HttpContextConstants.IsStaticContentRequest] = (object) true;
      if (rewrittenPath == null)
        return;
      context.Items[(object) HttpContextConstants.StaticContentRewritePath] = (object) rewrittenPath;
    }

    internal static bool IsStaticContentRequest(
      string requestPath,
      string hostVirtualPath,
      string applicationVirtualPath,
      out string rewrittenPath)
    {
      if (requestPath.StartsWith(hostVirtualPath, StringComparison.OrdinalIgnoreCase) && string.Compare(requestPath, hostVirtualPath.Length - 1, "/_static/", 0, "/_static/".Length, true) == 0)
      {
        bool flag = true;
        if (requestPath.IndexOf("/_cssbundles/", hostVirtualPath.Length + "/_static/".Length - 1, StringComparison.OrdinalIgnoreCase) >= 0)
          flag = false;
        string str = VirtualPathUtility.AppendTrailingSlash(applicationVirtualPath);
        rewrittenPath = hostVirtualPath.Length <= str.Length ? (string) null : str + requestPath.Substring(hostVirtualPath.Length);
        return flag;
      }
      rewrittenPath = (string) null;
      return false;
    }
  }
}
