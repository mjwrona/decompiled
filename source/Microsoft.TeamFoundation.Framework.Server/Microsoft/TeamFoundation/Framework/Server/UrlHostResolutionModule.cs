// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UrlHostResolutionModule
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class UrlHostResolutionModule : IHttpModule
  {
    public void Init(HttpApplication context) => context.BeginRequest += new EventHandler(this.BeginRequest);

    public void Dispose()
    {
    }

    private void BeginRequest(object sender, EventArgs e)
    {
      HttpContext context = ((HttpApplication) sender).Context;
      if (context.Items.Contains((object) HttpContextConstants.ServiceHostRouteContext))
        return;
      if (!TeamFoundationApplicationCore.DeploymentInitialized)
        TeamFoundationApplicationCore.ApplicationStart();
      using (IVssRequestContext systemContext = TeamFoundationApplicationCore.DeploymentServiceHost.CreateSystemContext())
      {
        Uri url;
        try
        {
          url = context.Request.Url;
        }
        catch (UriFormatException ex)
        {
          systemContext.Trace(1050002, TraceLevel.Info, HostRoutingService.Area, HostRoutingService.Layer, "Could not parse URI {0}: {1}", (object) context.Request.RawUrl, (object) ex);
          return;
        }
        HostRouteContext hostRouteContext = systemContext.GetService<IUrlHostResolutionService>().ResolveHost(systemContext, url);
        if (hostRouteContext == null)
          return;
        context.Items[(object) HttpContextConstants.ServiceHostRouteContext] = (object) hostRouteContext;
      }
    }
  }
}
