// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TfsDefaultCollectionRoutingModule
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Web;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class TfsDefaultCollectionRoutingModule : IHttpModule
  {
    private Guid m_defaultCollectionId;
    private bool m_initialized;
    private static readonly string[] s_v1ServiceVirtualDirectories = new string[11]
    {
      "~/Build/",
      "~/Lab/",
      "~/Services/",
      "~/VersionControl/",
      "~/TestManagement/",
      "~/WorkItemTracking/",
      "~/Warehouse/",
      "~/{Git}/",
      "~/%7BGit}/",
      "~/{Git%7D/",
      "~/%7BGit%7D/"
    };

    public void Init(HttpApplication context) => context.BeginRequest += new EventHandler(this.BeginRequest);

    public void Dispose()
    {
    }

    private void BeginRequest(object sender, EventArgs e)
    {
      try
      {
        this.EnsureInitialized();
        if (this.m_defaultCollectionId == Guid.Empty)
          return;
        HttpContext context = ((HttpApplication) sender).Context;
        HostRouteContext hostRouteContext = (HostRouteContext) context.Items[(object) HttpContextConstants.ServiceHostRouteContext];
        if (hostRouteContext == null || hostRouteContext.HostId != TeamFoundationApplicationCore.DeploymentServiceHost.InstanceId)
          return;
        bool flag = false;
        string appRelative = VirtualPathUtility.ToAppRelative(context.Request.Url.AbsolutePath);
        foreach (string virtualDirectory in TfsDefaultCollectionRoutingModule.s_v1ServiceVirtualDirectories)
        {
          if (appRelative.StartsWith(virtualDirectory, StringComparison.OrdinalIgnoreCase))
          {
            flag = true;
            break;
          }
        }
        if (!flag)
          return;
        context.Items[(object) HttpContextConstants.ServiceHostRouteContext] = (object) new HostRouteContext()
        {
          HostId = this.m_defaultCollectionId,
          VirtualPath = UrlHostResolutionService.ApplicationVirtualPath
        };
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(836567, nameof (TfsDefaultCollectionRoutingModule), "IHttpModule", ex);
      }
    }

    private void EnsureInitialized()
    {
      if (this.m_initialized)
        return;
      if (!TeamFoundationApplicationCore.DeploymentInitialized)
        TeamFoundationApplicationCore.ApplicationStart();
      using (IVssRequestContext systemContext = TeamFoundationApplicationCore.DeploymentServiceHost.CreateSystemContext())
        this.m_defaultCollectionId = systemContext.GetService<IVssRegistryService>().GetValue<Guid>(systemContext, (RegistryQuery) "/Configuration/DefaultCollection", Guid.Empty);
      this.m_initialized = true;
    }
  }
}
