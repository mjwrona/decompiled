// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Internal.StaticResourcesUrlBuilder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Runtime.CompilerServices;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server.Internal
{
  internal class StaticResourcesUrlBuilder : IStaticResourcesUrlBuilder
  {
    private static IStaticResourcesUrlBuilder s_instance = (IStaticResourcesUrlBuilder) new StaticResourcesUrlBuilder();

    internal static IStaticResourcesUrlBuilder Current => StaticResourcesUrlBuilder.s_instance;

    internal static void SetCurrent(
      IVssRequestContext requestContext,
      IStaticResourcesUrlBuilder current)
    {
      if (requestContext.ServiceHost.IsProduction)
        throw new InvalidOperationException("This operation is not allowed in production.");
      StaticResourcesUrlBuilder.s_instance = current;
    }

    public virtual string GetCdnRootUrl(IVssRequestContext requestContext)
    {
      bool flag = false;
      if (HttpContext.Current != null)
      {
        if (requestContext == null)
          requestContext = (IVssRequestContext) HttpContext.Current.Items[(object) HttpContextConstants.IVssRequestContext];
        if (HttpContext.Current.Request != null)
          flag = "disabled".Equals(HttpContext.Current.Request.Cookies["TFS-CDN"]?.Value, StringComparison.OrdinalIgnoreCase);
      }
      if (flag || requestContext == null || !requestContext.ExecutionEnvironment.IsHostedDeployment || !requestContext.IsFeatureEnabled("VisualStudio.Services.WebAccess.UseCDN"))
        return (string) null;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<ICdnLocationService>().GetCdnUrl(vssRequestContext, (string) null);
    }

    public virtual string GetStaticUrl(
      IVssRequestContext requestContext,
      string path,
      StaticResourcePathKind pathKind)
    {
      string staticUrl = "~/_static/" + path;
      switch (pathKind)
      {
        case StaticResourcePathKind.Physical:
          return this.MapPath(staticUrl);
        case StaticResourcePathKind.Remote:
          if (requestContext == null && HttpContext.Current != null)
            requestContext = (IVssRequestContext) HttpContext.Current.Items[(object) HttpContextConstants.IVssRequestContext];
          return VirtualPathUtility.ToAbsolute(staticUrl, requestContext.WebApplicationPath());
        default:
          return staticUrl;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual string MapPath(string path) => HttpContext.Current.Server.MapPath(path);
  }
}
