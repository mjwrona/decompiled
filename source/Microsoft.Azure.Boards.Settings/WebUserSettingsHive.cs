// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Settings.WebUserSettingsHive
// Assembly: Microsoft.Azure.Boards.Settings, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2AC3574E-9414-4605-BAB7-1F6B28A75804
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.Settings.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.Azure.Boards.Settings
{
  public class WebUserSettingsHive : SettingsHive
  {
    private IVssRegistryService m_tfsRegistry;
    private ITeamFoundationSecurityService m_securityService;
    private IdentityService m_identityService;

    public WebUserSettingsHive(IVssRequestContext requestContext)
      : this(requestContext, (string) null)
    {
    }

    public WebUserSettingsHive(IVssRequestContext requestContext, string cachePattern)
      : base(requestContext, cachePattern)
    {
      this.m_tfsRegistry = requestContext.GetService<IVssRegistryService>();
      this.m_identityService = requestContext.GetService<IdentityService>();
      this.m_securityService = (ITeamFoundationSecurityService) requestContext.GetService<TeamFoundationSecurityService>();
    }

    protected override string Prefix => WebAccessRegistryConstants.Prefix;

    protected override string ToWebRegistryPath(string path) => WebRegistryUtils.ToWebRegistryPath(path);

    protected override string FromWebRegistryPath(string path) => WebRegistryUtils.FromWebRegistryPath(path);

    protected override void UpdateEntries(RegistryEntry[] entries)
    {
      using (WebPerformanceTimer.StartMeasure(HttpContext.Current, "TFS.UserSettings.UpdateEntries", string.Join(",", ((IEnumerable<RegistryEntry>) entries).Select<RegistryEntry, string>((Func<RegistryEntry, string>) (e => e.Path)))))
      {
        this.CheckGlobalReadPermission(this.RequestContext);
        this.m_tfsRegistry.WriteEntries(this.RequestContext, this.RequestContext.GetUserIdentity(), (IEnumerable<RegistryEntry>) entries);
      }
    }

    protected override void RemoveEntries(string[] entries)
    {
      using (WebPerformanceTimer.StartMeasure(HttpContext.Current, "TFS.UserSettings.RemoveEntries", string.Join(",", entries)))
      {
        this.CheckGlobalReadPermission(this.RequestContext);
        this.m_tfsRegistry.DeleteEntries(this.RequestContext, this.RequestContext.GetUserIdentity(), entries);
      }
    }

    protected override IList<RegistryEntry> QueryEntries(string pathPattern, bool includeFolders)
    {
      using (WebPerformanceTimer.StartMeasure(HttpContext.Current, "TFS.UserSettings.QueryEntries", pathPattern))
      {
        this.CheckGlobalReadPermission(this.RequestContext);
        return (IList<RegistryEntry>) this.m_tfsRegistry.ReadEntries(this.RequestContext, this.RequestContext.GetUserIdentity(), pathPattern, includeFolders).ToList<RegistryEntry>();
      }
    }

    private void CheckGlobalReadPermission(IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      this.m_securityService.GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 1);
    }
  }
}
