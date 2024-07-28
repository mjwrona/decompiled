// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.VssCSSRootModule
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  internal class VssCSSRootModule : VssCSSModule
  {
    private IEnumerable<FileInfo> m_dependencies;
    private HashSet<string> m_excluded;

    public VssCSSRootModule(
      IEnumerable<FileInfo> dependencies,
      IEnumerable<string> excluded,
      HashSet<string> cssFiles,
      CSSPluginModuleNameParser nameParser,
      Dictionary<string, string[]> contentCache)
      : base((VssScriptsModuleInfo) null, cssFiles, nameParser, contentCache)
    {
      this.m_dependencies = dependencies;
      this.m_excluded = new HashSet<string>(excluded ?? Enumerable.Empty<string>());
    }

    protected override bool ShouldReadDependencies() => true;

    protected override IEnumerable<VssScriptsModuleInfo> ReadDependencies(
      string staticContentVersion,
      bool excluded)
    {
      IEnumerable<VssScriptsModuleInfo> dependencies = this.m_dependencies.Select<FileInfo, VssScriptsModuleInfo>((Func<FileInfo, VssScriptsModuleInfo>) (fi => this.CreateScriptsModuleInfo(fi))).Where<VssScriptsModuleInfo>((Func<VssScriptsModuleInfo, bool>) (m => !excluded ? !this.m_excluded.Contains(m.Id) : this.m_excluded.Contains(m.Id)));
      if (!excluded)
        this.AddCSSDependencies(dependencies);
      return dependencies;
    }

    protected override string GetFilePart() => "#cssroot#";
  }
}
