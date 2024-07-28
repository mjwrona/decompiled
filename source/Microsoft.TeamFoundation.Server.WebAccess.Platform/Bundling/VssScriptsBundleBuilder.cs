// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.VssScriptsBundleBuilder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  internal class VssScriptsBundleBuilder
  {
    internal static readonly PluginModuleLookup PluginModules = new PluginModuleLookup();
    public HashSet<string> m_modulesExcludedByPath = new HashSet<string>();

    public ISet<string> GetModulesExcludedByPath() => (ISet<string>) this.m_modulesExcludedByPath;

    public string BuildBundleContent(
      VssScriptsBundle bundle,
      IEnumerable<FileInfo> files,
      CultureInfo cultureInfo,
      Dictionary<string, string[]> contentCache,
      HashSet<string> missingModuleIds,
      string staticContentVersion)
    {
      return BundlingHelper.JoinWithNewLine((IEnumerable<string>) new VssScriptsRootModule(bundle.Id, files, bundle.ExcludedModules, contentCache).GetBundleContent(VssScriptsBundleBuilder.PluginModules, bundle.ExcludedPaths, missingModuleIds, staticContentVersion, this.m_modulesExcludedByPath, cultureInfo, bundle.Diagnose));
    }

    public IEnumerable<IBundleStreamProvider> BuildBundleStreamProviders(
      VssScriptsBundle bundle,
      IEnumerable<FileInfo> files,
      CultureInfo cultureInfo,
      string staticContentVersion)
    {
      List<IBundleStreamProvider> streamProviders = new List<IBundleStreamProvider>();
      new VssScriptsRootModule(bundle.Id, files, bundle.ExcludedModules, (Dictionary<string, string[]>) null).PopulateContentStreamProviders(streamProviders, VssScriptsBundleBuilder.PluginModules, bundle.ExcludedPaths, staticContentVersion, this.m_modulesExcludedByPath, cultureInfo, bundle.Diagnose);
      return (IEnumerable<IBundleStreamProvider>) streamProviders;
    }

    public List<byte[]> BuildBundleHashCodeList(
      VssScriptsBundle bundle,
      IEnumerable<FileInfo> files,
      CultureInfo cultureInfo,
      Dictionary<string, string[]> contentCache,
      string staticContentVersion,
      out int contentLength)
    {
      return new VssScriptsRootModule(bundle.Id, files, bundle.ExcludedModules, contentCache).GetBundleHashList(VssScriptsBundleBuilder.PluginModules, bundle.ExcludedPaths, staticContentVersion, out contentLength, this.m_modulesExcludedByPath, cultureInfo, bundle.Diagnose);
    }
  }
}
