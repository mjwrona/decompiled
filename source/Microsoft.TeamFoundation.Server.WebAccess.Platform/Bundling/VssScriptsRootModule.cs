// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.VssScriptsRootModule
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  internal class VssScriptsRootModule : VssScriptsModule
  {
    private string m_id;
    private IEnumerable<FileInfo> m_dependencies;
    private HashSet<string> m_excluded;

    public VssScriptsRootModule(
      string id,
      IEnumerable<FileInfo> dependencies,
      IEnumerable<string> excluded,
      Dictionary<string, string[]> contentCache)
      : base((VssScriptsModuleInfo) null, contentCache)
    {
      this.m_id = id;
      this.m_dependencies = dependencies;
      this.m_excluded = new HashSet<string>(excluded ?? Enumerable.Empty<string>());
    }

    protected override bool ShouldReadDependencies() => true;

    protected override IEnumerable<VssScriptsModuleInfo> ReadDependencies(
      string staticContentVersion,
      bool excluded)
    {
      return this.m_dependencies.Select<FileInfo, VssScriptsModuleInfo>((Func<FileInfo, VssScriptsModuleInfo>) (fi => this.CreateScriptsModuleInfo(fi))).Where<VssScriptsModuleInfo>((Func<VssScriptsModuleInfo, bool>) (m => !excluded ? !this.m_excluded.Contains(m.Id) : this.m_excluded.Contains(m.Id)));
    }

    protected override void AddContentToBundle(
      List<string> lines,
      HashSet<string> missingModuleIds,
      bool diagnose)
    {
    }

    protected override void AddToBundleHashList(
      List<byte[]> bundleHashList,
      CultureInfo cultureInfo,
      string staticContentVersion,
      out int contentLength)
    {
      contentLength = 0;
    }

    protected override string GetFilePart() => this.ToString();

    public override string ToString() => "#root#";

    public override List<string> GetBundleContent(
      PluginModuleLookup pluginModules,
      IEnumerable<string> excludedPaths,
      HashSet<string> missingModuleIds,
      string staticContentVersion,
      HashSet<string> modulesExcludedByPath = null,
      CultureInfo cultureInfo = null,
      bool diagnose = false)
    {
      List<string> bundleContent = base.GetBundleContent(pluginModules, excludedPaths, missingModuleIds, staticContentVersion, modulesExcludedByPath, cultureInfo, diagnose);
      if (diagnose)
        bundleContent.Insert(0, DiagnoseHelper.GetBundleInfo(this.m_id, this.m_dependencies));
      return bundleContent;
    }

    public override List<byte[]> GetBundleHashList(
      PluginModuleLookup pluginModules,
      IEnumerable<string> excludedPaths,
      string staticContentVersion,
      out int contentLength,
      HashSet<string> modulesExcludedByPath = null,
      CultureInfo cultureInfo = null,
      bool diagnose = false)
    {
      List<byte[]> bundleHashList = base.GetBundleHashList(pluginModules, excludedPaths, staticContentVersion, out contentLength, modulesExcludedByPath, cultureInfo, diagnose);
      if (diagnose)
        bundleHashList.Add(BundlingHelper.CalculateHashFromLines(new string[1]
        {
          DateTime.UtcNow.Ticks.ToString()
        }));
      return bundleHashList;
    }
  }
}
