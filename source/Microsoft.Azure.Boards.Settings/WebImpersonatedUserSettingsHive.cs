// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Settings.WebImpersonatedUserSettingsHive
// Assembly: Microsoft.Azure.Boards.Settings, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2AC3574E-9414-4605-BAB7-1F6B28A75804
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.Settings.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.Azure.Boards.Settings
{
  public class WebImpersonatedUserSettingsHive : SettingsHive
  {
    private CachedRegistryService m_registry;
    private Guid m_userId;

    public WebImpersonatedUserSettingsHive(
      IVssRequestContext requestContext,
      Guid userTeamFoundationId)
      : base(requestContext)
    {
      this.m_registry = requestContext.GetService<CachedRegistryService>();
      this.m_userId = userTeamFoundationId;
    }

    protected override string Prefix => WebAccessRegistryConstants.Prefix;

    protected override string ToWebRegistryPath(string path) => WebRegistryUtils.ToWebRegistryPath(path);

    protected override string FromWebRegistryPath(string path) => WebRegistryUtils.FromWebRegistryPath(path);

    protected override void UpdateEntries(RegistryEntry[] entries)
    {
      using (WebPerformanceTimer.StartMeasure(HttpContext.Current, "TFS.ImpersonatedUserSettings.UpdateEntries", string.Join(",", ((IEnumerable<RegistryEntry>) entries).Select<RegistryEntry, string>((Func<RegistryEntry, string>) (e => e.Path)))))
        this.m_registry.WriteEntries(this.RequestContext, this.m_userId, (IEnumerable<RegistryEntry>) entries);
    }

    protected override void RemoveEntries(string[] entries)
    {
      using (WebPerformanceTimer.StartMeasure(HttpContext.Current, "TFS.ImpersonatedUserSettings.RemoveEntries", string.Join(",", entries)))
        this.m_registry.DeleteEntries(this.RequestContext, this.m_userId, entries);
    }

    protected override IList<RegistryEntry> QueryEntries(string pathPattern, bool includeFolders)
    {
      using (WebPerformanceTimer.StartMeasure(HttpContext.Current, "TFS.ImpersonatedUserSettings.QueryEntries", pathPattern))
        return (IList<RegistryEntry>) this.m_registry.ReadEntries(this.RequestContext, this.m_userId, pathPattern, includeFolders).ToList<RegistryEntry>();
    }
  }
}
