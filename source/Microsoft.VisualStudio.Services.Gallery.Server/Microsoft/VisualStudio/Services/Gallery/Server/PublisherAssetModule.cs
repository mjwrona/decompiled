// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.PublisherAssetModule
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Web;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class PublisherAssetModule : IHttpModule
  {
    public void Dispose()
    {
    }

    public void Init(HttpApplication context) => context.BeginRequest += new EventHandler(this.BeginRequest);

    private void BeginRequest(object sender, EventArgs e)
    {
      HttpContext context = ((HttpApplication) sender).Context;
      PublisherAssetConfiguration configuration = (PublisherAssetConfiguration) null;
      if (context.Items.Contains((object) HttpContextConstants.ServiceHostRouteContext))
        return;
      if (!TeamFoundationApplicationCore.DeploymentInitialized)
        TeamFoundationApplicationCore.ApplicationStart();
      using (IVssRequestContext systemContext = TeamFoundationApplicationCore.DeploymentServiceHost.CreateSystemContext())
        this.InitialConfiguration(context, configuration, systemContext);
    }

    internal void InitialConfiguration(
      HttpContext context,
      PublisherAssetConfiguration configuration,
      IVssRequestContext deploymentContext)
    {
      configuration = deploymentContext.GetService<IPublisherAssetService>().GetConfiguration(deploymentContext);
      if (string.IsNullOrEmpty(configuration.Host))
        return;
      Uri url = context.Request.Url;
      if (!url.Host.EndsWith(configuration.Host, StringComparison.OrdinalIgnoreCase) || !url.AbsolutePath.StartsWith(configuration.VirtualPath))
        return;
      string str = url.Authority.Substring(0, url.Host.Length - configuration.Host.Length);
      context.Items[(object) "PublisherAsset"] = (object) str;
      context.Items[(object) HttpContextConstants.ServiceHostRouteContext] = (object) new HostRouteContext()
      {
        HostId = deploymentContext.ServiceHost.InstanceId,
        VirtualPath = configuration.VirtualPath
      };
      context.Items[(object) HttpContextConstants.DisallowCookies] = (object) true;
    }
  }
}
