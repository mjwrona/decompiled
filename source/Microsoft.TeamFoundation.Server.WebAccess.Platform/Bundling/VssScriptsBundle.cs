// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.VssScriptsBundle
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
  internal class VssScriptsBundle
  {
    public VssScriptsBundleBuilder BundleBuilder { get; private set; }

    public string Id { get; private set; }

    private HashSet<string> Items { get; set; }

    public IEnumerable<string> ExcludedModules { get; private set; }

    public IEnumerable<string> ExcludedPaths { get; private set; }

    public bool Diagnose { get; set; }

    public CultureInfo CultureInfo { get; private set; }

    public string StaticContentVersion { get; private set; }

    public VssScriptsBundle(
      string id,
      IEnumerable<string> includedModules,
      IEnumerable<string> excludedModules,
      IEnumerable<string> excludedPaths,
      CultureInfo cultureInfo,
      Func<string, string> moduleToPathFunc,
      bool diagnose = false,
      string staticContentVersion = null)
    {
      this.Id = id;
      this.Items = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.ExcludedModules = excludedModules;
      this.ExcludedPaths = excludedPaths;
      this.BundleBuilder = new VssScriptsBundleBuilder();
      this.CultureInfo = cultureInfo;
      this.Diagnose = diagnose;
      this.StaticContentVersion = staticContentVersion;
      HashSet<string> source = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (includedModules != null)
        source.UnionWith(includedModules);
      if (excludedModules != null)
        source.UnionWith(excludedModules);
      this.Include(source.Select<string, string>(moduleToPathFunc).ToArray<string>());
    }

    public string GetBundleContent(
      Dictionary<string, string[]> contentCache,
      HashSet<string> missingModuleIds,
      string staticContentVersion)
    {
      return this.BundleBuilder != null ? this.BundleBuilder.BuildBundleContent(this, this.GetFiles(), this.CultureInfo, contentCache, missingModuleIds, staticContentVersion) : string.Empty;
    }

    public IEnumerable<IBundleStreamProvider> GetBundleStreamProviders(string staticContentVersion) => this.BundleBuilder != null ? this.BundleBuilder.BuildBundleStreamProviders(this, this.GetFiles(), this.CultureInfo, staticContentVersion) : (IEnumerable<IBundleStreamProvider>) Array.Empty<IBundleStreamProvider>();

    public List<byte[]> GetBundleHashCode(
      Dictionary<string, string[]> contentCache,
      out int contentLength)
    {
      if (this.BundleBuilder != null)
        return this.BundleBuilder.BuildBundleHashCodeList(this, this.GetFiles(), this.CultureInfo, contentCache, this.StaticContentVersion, out contentLength);
      contentLength = 0;
      return new List<byte[]>();
    }

    public IEnumerable<FileInfo> GetFiles() => this.Items.Select<string, FileInfo>((Func<string, FileInfo>) (m => new FileInfo(m)));

    public void Include(params string[] virtualPaths)
    {
      foreach (string virtualPath in virtualPaths)
      {
        VssScriptFileInfo rootRelativePath = VssScriptFileInfo.GetVssScriptFileFromSitesRootRelativePath(virtualPath);
        if (rootRelativePath != null && rootRelativePath.FileInfo.Exists)
          this.Items.Add(rootRelativePath.FileInfo.FullName);
      }
    }
  }
}
