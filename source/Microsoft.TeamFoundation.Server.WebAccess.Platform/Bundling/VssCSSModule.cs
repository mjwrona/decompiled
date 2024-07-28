// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.VssCSSModule
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  internal class VssCSSModule : VssScriptsModule
  {
    private HashSet<string> m_cssFiles;
    private CSSPluginModuleNameParser m_nameParser;

    internal VssCSSModule(
      VssScriptsModuleInfo moduleInfo,
      HashSet<string> cssFiles,
      CSSPluginModuleNameParser nameParser,
      Dictionary<string, string[]> contentCache)
      : base(moduleInfo, contentCache)
    {
      this.m_cssFiles = cssFiles;
      this.m_nameParser = nameParser;
    }

    protected override IEnumerable<VssScriptsModuleInfo> ReadDependencies(
      string staticContentVersion,
      bool excluded = false)
    {
      IEnumerable<VssScriptsModuleInfo> dependencies = base.ReadDependencies(staticContentVersion, excluded);
      if (!excluded)
        this.AddCSSDependencies(dependencies);
      return dependencies;
    }

    protected void AddCSSDependencies(IEnumerable<VssScriptsModuleInfo> dependencies)
    {
      foreach (VssScriptsModuleInfo dependency in dependencies)
      {
        string cssPluginFile = this.m_nameParser.GetCSSPluginFile(dependency);
        if (!string.IsNullOrEmpty(cssPluginFile))
          this.m_cssFiles.Add(cssPluginFile);
      }
    }

    protected override VssScriptsModule CreateScriptsModule(
      VssScriptsModuleInfo moduleInfo,
      Dictionary<string, string[]> contentCache,
      CultureInfo cultureInfo)
    {
      return (VssScriptsModule) new VssCSSModule(moduleInfo, this.m_cssFiles, this.m_nameParser, contentCache);
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
  }
}
