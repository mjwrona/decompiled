// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.VssCSSBundleBuilder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  internal class VssCSSBundleBuilder
  {
    private HashSet<string> m_cssFiles;
    private HashSet<string> m_cssFilesToExclude;
    private HashSet<string> m_includedCssFiles;
    private Func<string, string> m_cssFileToLocalPath;

    public VssCSSBundleBuilder(
      IEnumerable<string> cssFiles,
      IEnumerable<string> cssFilesToExclude,
      Func<string, string> cssFileToLocalPath)
    {
      this.m_cssFiles = new HashSet<string>(cssFiles ?? Enumerable.Empty<string>(), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_cssFilesToExclude = new HashSet<string>(cssFilesToExclude ?? Enumerable.Empty<string>(), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_includedCssFiles = new HashSet<string>();
      this.m_cssFileToLocalPath = cssFileToLocalPath;
    }

    public IEnumerable<string> GetIncludedCssFiles() => (IEnumerable<string>) this.m_includedCssFiles;

    public void AddCSSFilesFromScripts(
      VssScriptsBundle bundle,
      bool ignorecssModulePrefixes,
      IEnumerable<string> cssModulePrefixes,
      IEnumerable<string> excludedPaths,
      Dictionary<string, string[]> contentCache)
    {
      CSSPluginModuleNameParser nameParser = new CSSPluginModuleNameParser(ignorecssModulePrefixes, new HashSet<string>(cssModulePrefixes ?? Enumerable.Empty<string>(), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase));
      new VssCSSRootModule(bundle.GetFiles(), bundle.ExcludedModules, this.m_cssFiles, nameParser, contentCache).GetBundleHashList(VssScriptsBundleBuilder.PluginModules, excludedPaths, bundle.StaticContentVersion, out int _);
    }

    public string GetBundleContent(
      Dictionary<string, string[]> contentCache,
      Func<string, string> customContentHandler = null)
    {
      bool flag = false;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string cssFile in this.m_cssFiles.Where<string>((Func<string, bool>) (cf => !this.m_cssFilesToExclude.Contains(cf))))
      {
        if (customContentHandler != null)
        {
          string str = customContentHandler(cssFile);
          if (!string.IsNullOrEmpty(str))
          {
            stringBuilder.AppendLine(string.Format("/* Adding custom content for css {0} */", (object) cssFile));
            stringBuilder.Append(str);
            continue;
          }
        }
        VssScriptFileInfo fileInfoForCssFile = this.GetVssScriptFileInfoForCssFile(cssFile);
        if (fileInfoForCssFile == null || !fileInfoForCssFile.FileInfo.Exists)
        {
          stringBuilder.AppendLine(string.Format("/* Missing CSS reference:{0} */", (object) cssFile));
        }
        else
        {
          flag = true;
          stringBuilder.AppendLine(string.Format("/* CSS file: {0} */", (object) cssFile));
          string[] lines = fileInfoForCssFile.ReadLinesAndSetHash(contentCache);
          stringBuilder.AppendLine(BundlingHelper.JoinWithNewLine((IEnumerable<string>) lines));
          contentCache[fileInfoForCssFile.FileInfo.FullName] = lines;
          stringBuilder.AppendLine();
          this.m_includedCssFiles.Add(cssFile);
        }
      }
      return flag ? stringBuilder.ToString() : string.Empty;
    }

    public IEnumerable<IBundleStreamProvider> GetBundleStreamProviders(
      Func<string, string> customContentHandler = null)
    {
      List<IBundleStreamProvider> bundleStreamProviders = new List<IBundleStreamProvider>();
      foreach (string cssFile in this.m_cssFiles.Where<string>((Func<string, bool>) (cf => !this.m_cssFilesToExclude.Contains(cf))))
      {
        if (customContentHandler != null)
        {
          string str = customContentHandler(cssFile);
          if (!string.IsNullOrEmpty(str))
          {
            bundleStreamProviders.Add((IBundleStreamProvider) new BundleTextStreamProvider(string.Format("/* Custom content for {0} */{1}{2}", (object) cssFile, (object) Environment.NewLine, (object) str)));
            continue;
          }
        }
        VssScriptFileInfo fileInfoForCssFile = this.GetVssScriptFileInfoForCssFile(cssFile);
        if (fileInfoForCssFile?.FileInfo != null && fileInfoForCssFile.FileInfo.Exists)
        {
          bundleStreamProviders.Add((IBundleStreamProvider) new BundleTextStreamProvider(string.Format("/* CSS file:{0} */", (object) cssFile)));
          bundleStreamProviders.Add((IBundleStreamProvider) new BundleFileStreamProvider(fileInfoForCssFile.FileInfo));
        }
        else
          bundleStreamProviders.Add((IBundleStreamProvider) new BundleTextStreamProvider(string.Format("/* Missing CSS reference:{0} */", (object) cssFile)));
      }
      return (IEnumerable<IBundleStreamProvider>) bundleStreamProviders;
    }

    private VssScriptFileInfo GetVssScriptFileInfoForCssFile(string cssFile) => VssScriptFileInfo.GetVssScriptFileFromSitesRootRelativePath(this.m_cssFileToLocalPath(cssFile));

    public List<byte[]> BuildBundleHashCodeList(
      Dictionary<string, string[]> contentCache,
      out int contentLength,
      Func<string, string> customContentHandler = null)
    {
      List<byte[]> numArrayList = new List<byte[]>();
      contentLength = 0;
      foreach (string cssFile in this.m_cssFiles.Where<string>((Func<string, bool>) (cf => !this.m_cssFilesToExclude.Contains(cf))))
      {
        if (customContentHandler != null)
        {
          string str1 = customContentHandler(cssFile);
          if (!string.IsNullOrEmpty(str1))
          {
            string str2 = "Custom " + cssFile + " " + str1;
            contentLength += str2.Length;
            numArrayList.Add(BundlingHelper.CalculateHashFromLines(new string[1]
            {
              str2
            }));
            continue;
          }
        }
        VssScriptFileInfo fileInfoForCssFile = this.GetVssScriptFileInfoForCssFile(cssFile);
        if (fileInfoForCssFile != null && fileInfoForCssFile.FileInfo.Exists)
          contentLength += (int) fileInfoForCssFile.FileInfo.Length;
        byte[] hashCode = fileInfoForCssFile.HashCode;
        if (fileInfoForCssFile.HashCode == null)
        {
          string[] strArray = fileInfoForCssFile.ReadLinesAndSetHash(contentCache);
          contentCache[fileInfoForCssFile.FileInfo.FullName] = strArray;
          hashCode = fileInfoForCssFile.HashCode;
        }
        this.m_includedCssFiles.Add(cssFile);
        numArrayList.Add(hashCode);
      }
      return numArrayList;
    }
  }
}
