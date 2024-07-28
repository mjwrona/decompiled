// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Settings.RootSettingsHive
// Assembly: Microsoft.Azure.Boards.Settings, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2AC3574E-9414-4605-BAB7-1F6B28A75804
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.Settings.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.Azure.Boards.Settings
{
  public class RootSettingsHive : SettingsHive
  {
    private CachedRegistryService m_tfsRegistry;

    public RootSettingsHive(IVssRequestContext requestContext)
      : this(requestContext, (string) null)
    {
    }

    public RootSettingsHive(IVssRequestContext requestContext, string cachePattern)
      : base(requestContext, cachePattern)
    {
      this.m_tfsRegistry = requestContext.GetService<CachedRegistryService>();
    }

    protected override string Prefix => "";

    protected override void UpdateEntries(RegistryEntry[] entries)
    {
      using (WebPerformanceTimer.StartMeasure(HttpContext.Current, "TFS.RootUserSettings.UpdateEntries", string.Join(",", ((IEnumerable<RegistryEntry>) entries).Select<RegistryEntry, string>((Func<RegistryEntry, string>) (e => e.Path)))))
        this.m_tfsRegistry.WriteEntries(this.RequestContext, (IEnumerable<RegistryEntry>) entries);
    }

    protected override void RemoveEntries(string[] entries)
    {
      using (WebPerformanceTimer.StartMeasure(HttpContext.Current, "TFS.RootUserSettings.RemoveEntries", string.Join(",", entries)))
        this.m_tfsRegistry.DeleteEntries(this.RequestContext, entries);
    }

    protected override IList<RegistryEntry> QueryEntries(string pathPattern, bool includeFolders)
    {
      using (WebPerformanceTimer.StartMeasure(HttpContext.Current, "TFS.RootUserSettings.QueryEntries", pathPattern))
        return (IList<RegistryEntry>) this.m_tfsRegistry.ReadEntries(this.RequestContext, (RegistryQuery) pathPattern, includeFolders).ToList<RegistryEntry>();
    }
  }
}
