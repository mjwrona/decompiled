// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssPathMatchRequestRewriter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class VssPathMatchRequestRewriter : ITeamFoundationRequestFilter
  {
    protected VssPathMatchRequestRewriter(string routePrefix)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(routePrefix, nameof (routePrefix));
      this.RoutePrefix = routePrefix;
    }

    public string RoutePrefix { get; private set; }

    public void BeginRequest(IVssRequestContext requestContext)
    {
      if (HttpContext.Current == null || HttpContext.Current.Items.Contains((object) HttpContextConstants.ArrRequestRouted) || HttpContext.Current.Items.Contains((object) HttpContextConstants.OriginalVirtualDirectory))
        return;
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      string str1 = requestContext.VirtualPath();
      string str2 = VirtualPathUtility.ToAbsolute(str1).TrimEnd('/');
      if (str2.Length == 0)
        return;
      string path = HttpContext.Current.Request.Path;
      string str3 = path.Substring(str2.Length);
      string str4 = "/" + this.RoutePrefix;
      if (str3.IndexOf(str4 + "/", StringComparison.OrdinalIgnoreCase) < 0 && !str3.EndsWith(str4, StringComparison.OrdinalIgnoreCase))
        return;
      string appRelative = VirtualPathUtility.ToAppRelative(path, str1);
      if (appRelative == HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath)
        return;
      HttpContext.Current.Items[(object) HttpContextConstants.OriginalVirtualDirectory] = (object) str1;
      HttpContext.Current.RewritePath(appRelative, true);
    }

    public Task BeginRequestAsync(IVssRequestContext requestContext) => Task.CompletedTask;

    public void EndRequest(IVssRequestContext requestContext)
    {
    }

    public void EnterMethod(IVssRequestContext requestContext)
    {
    }

    public void LeaveMethod(IVssRequestContext requestContext)
    {
    }

    public Task PostAuthenticateRequest(IVssRequestContext requestContext) => (Task) Task.FromResult<int>(0);

    public Task PostLogRequestAsync(IVssRequestContext requestContext) => (Task) Task.FromResult<int>(0);

    void ITeamFoundationRequestFilter.PostAuthorizeRequest(IVssRequestContext requestContext)
    {
    }
  }
}
