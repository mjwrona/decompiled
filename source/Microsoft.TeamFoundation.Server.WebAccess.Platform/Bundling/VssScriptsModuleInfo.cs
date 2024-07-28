// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.VssScriptsModuleInfo
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  internal class VssScriptsModuleInfo
  {
    private static readonly string s_min = "/min/";
    private static readonly string s_debug = "/debug/";
    internal static readonly string s_dependenciesStart = "//dependencies=";
    internal static readonly string s_directDependenciesStart = "//direct-dependencies=";
    private static ConcurrentDictionary<string, VssScriptsModuleInfo> s_moduleInfoCache = new ConcurrentDictionary<string, VssScriptsModuleInfo>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase);
    private object m_dependenciesSyncObject = new object();
    private IEnumerable<VssScriptsModuleInfo> m_dependencies;
    private ResourceModuleInfo m_resourceModuleInfo;
    private ConcurrentDictionary<CultureInfo, byte[]> m_resourceContentHash;
    private ConcurrentDictionary<CultureInfo, int> m_resourceContentLengths;

    public string ScriptsRoot { get; private set; }

    public string Id { get; private set; }

    public string PluginName { get; private set; }

    public bool HasDependencies { get; private set; }

    public bool NeedsResolution { get; private set; }

    private VssScriptsModuleInfo(FileInfo fileInfo)
    {
      string moduleId = fileInfo.Name.Replace(".js", "");
      string str = fileInfo.DirectoryName.Replace("\\", "/");
      string[] strArray = Array.Empty<string>();
      if (str.IndexOf(VssScriptsModuleInfo.s_min, StringComparison.OrdinalIgnoreCase) > 0)
      {
        strArray = str.Split(new string[1]
        {
          VssScriptsModuleInfo.s_min
        }, StringSplitOptions.None);
        this.ScriptsRoot = Path.Combine(strArray[0].Replace("/", "\\"), VssScriptsModuleInfo.s_min.Trim('/'));
      }
      else if (str.IndexOf(VssScriptsModuleInfo.s_debug, StringComparison.OrdinalIgnoreCase) > 0)
      {
        strArray = str.Split(new string[1]
        {
          VssScriptsModuleInfo.s_debug
        }, StringSplitOptions.None);
        this.ScriptsRoot = Path.Combine(strArray[0].Replace("/", "\\"), VssScriptsModuleInfo.s_debug.Trim('/'));
        this.IsDebug = true;
      }
      else
        this.ScriptsRoot = str.Replace("/", "\\");
      if (strArray.Length > 1)
        this.SetId((strArray[1] + "/" + moduleId).Trim());
      else
        this.SetId(moduleId);
      this.NeedsResolution = true;
      this.HasDependencies = true;
      this.SetPath();
    }

    private VssScriptsModuleInfo(string moduleId, string scriptsRoot)
    {
      this.SetId(moduleId);
      this.ScriptsRoot = scriptsRoot;
      this.IsDebug = scriptsRoot.EndsWith("\\debug", StringComparison.OrdinalIgnoreCase);
      this.SetPath();
    }

    private void SetPath()
    {
      this.FilePath = Path.Combine(this.ScriptsRoot, this.Id.Replace("/", "\\")) + ".js";
      if (this.m_resourceModuleInfo != null)
        return;
      this.ScriptFile = VssScriptFileInfo.GetVssScriptFile(this.FilePath);
    }

    public string FilePath { get; private set; }

    public VssScriptFileInfo ScriptFile { get; private set; }

    public bool IsDebug { get; private set; }

    private void SetId(string moduleId)
    {
      string str = (string) null;
      int num = moduleId.LastIndexOf('#');
      if (num > 0)
        str = moduleId.Substring(num);
      if (!string.IsNullOrEmpty(str))
      {
        switch (str)
        {
          case "#?":
            this.NeedsResolution = true;
            this.HasDependencies = true;
            break;
          case "#0":
            this.HasDependencies = false;
            break;
        }
        this.Id = moduleId.Substring(0, num);
      }
      else
        this.Id = moduleId;
      int length = this.Id.IndexOf('!');
      if (length > 0)
      {
        this.PluginName = this.Id.Substring(length + 1);
        this.Id = this.Id.Substring(0, length);
      }
      if (this.Id.IndexOf(".Resources.", StringComparison.OrdinalIgnoreCase) < 0)
        return;
      this.m_resourceModuleInfo = ScriptRegistration.GetResourceManagerForScriptModule(this.Id);
      if (this.m_resourceModuleInfo == null)
        return;
      this.m_resourceContentHash = new ConcurrentDictionary<CultureInfo, byte[]>();
      this.m_resourceContentLengths = new ConcurrentDictionary<CultureInfo, int>();
      this.HasDependencies = false;
      this.NeedsResolution = false;
    }

    public byte[] GetHashCode(string staticContentVersion, CultureInfo cultureInfo = null)
    {
      if (this.m_resourceModuleInfo == null)
        return BundleMetadata.GetModuleHash(this.Id, staticContentVersion) ?? this.ScriptFile.HashCode;
      cultureInfo = cultureInfo ?? CultureInfo.CurrentUICulture;
      byte[] hashCode;
      this.m_resourceContentHash.TryGetValue(cultureInfo, out hashCode);
      return hashCode;
    }

    public int GetContentLength(CultureInfo cultureInfo = null)
    {
      if (this.m_resourceModuleInfo != null)
      {
        cultureInfo = cultureInfo ?? CultureInfo.CurrentUICulture;
        int contentLength;
        this.m_resourceContentLengths.TryGetValue(cultureInfo, out contentLength);
        return contentLength;
      }
      return !this.ScriptFile.FileInfo.Exists ? 0 : (int) this.ScriptFile.FileInfo.Length;
    }

    public string[] ReadLinesAndSetHashCode(
      Dictionary<string, string[]> contentCache,
      CultureInfo cultureInfo)
    {
      string[] lines;
      if (this.m_resourceModuleInfo != null)
      {
        cultureInfo = cultureInfo ?? CultureInfo.CurrentUICulture;
        lines = new string[1]
        {
          ScriptResourceController.GenerateResourceScript(this.m_resourceModuleInfo.GetResourceManager(), this.Id, cultureInfo, true, true)
        };
        if (!this.m_resourceContentHash.ContainsKey(cultureInfo))
        {
          byte[] bytes = Encoding.UTF8.GetBytes(BundlingHelper.JoinWithNewLine((IEnumerable<string>) lines));
          this.m_resourceContentHash[cultureInfo] = BundlingHelper.CalculateHashFromBytes(bytes);
          this.m_resourceContentLengths[cultureInfo] = bytes.Length;
        }
      }
      else
      {
        lines = this.ScriptFile.ReadLinesAndSetHash(contentCache);
        if (lines.Length == 0)
        {
          this.HasDependencies = false;
          this.NeedsResolution = false;
        }
      }
      return lines;
    }

    public IEnumerable<VssScriptsModuleInfo> GetDependencies(string staticContentVersion)
    {
      if (this.m_dependencies == null)
      {
        lock (this.m_dependenciesSyncObject)
        {
          if (this.m_dependencies == null)
          {
            if (this.HasDependencies)
            {
              IEnumerable<string> dependencies = BundleMetadata.GetDependencies(this.Id, staticContentVersion);
              if (dependencies != null)
                this.m_dependencies = dependencies.Select<string, VssScriptsModuleInfo>((Func<string, VssScriptsModuleInfo>) (d => VssScriptsModuleInfo.GetVssScriptsModuleInfo(d, this.ScriptsRoot)));
            }
            if (this.m_dependencies == null)
              this.m_dependencies = Enumerable.Empty<VssScriptsModuleInfo>();
          }
        }
      }
      return this.m_dependencies;
    }

    public string GetFilePart()
    {
      string[] strArray = this.Id.Split('/');
      return strArray.Length == 0 ? string.Empty : strArray[strArray.Length - 1];
    }

    public override string ToString() => this.Id;

    public bool StartsWith(HashSet<string> excludedPaths)
    {
      if (excludedPaths == null)
        return false;
      return excludedPaths.Contains(((IEnumerable<string>) this.Id.Split('/')).First<string>());
    }

    public static VssScriptsModuleInfo GetVssScriptsModuleInfo(FileInfo fileInfo)
    {
      VssScriptsModuleInfo scriptsModuleInfo1;
      if (VssScriptsModuleInfo.s_moduleInfoCache.TryGetValue(fileInfo.FullName, out scriptsModuleInfo1))
        return scriptsModuleInfo1;
      VssScriptsModuleInfo scriptsModuleInfo2 = new VssScriptsModuleInfo(fileInfo);
      VssScriptsModuleInfo.s_moduleInfoCache[fileInfo.FullName] = scriptsModuleInfo2;
      return scriptsModuleInfo2;
    }

    public static VssScriptsModuleInfo GetVssScriptsModuleInfo(string moduleId, string scriptsRoot)
    {
      string key = scriptsRoot + "\\" + moduleId;
      VssScriptsModuleInfo scriptsModuleInfo1;
      if (VssScriptsModuleInfo.s_moduleInfoCache.TryGetValue(key, out scriptsModuleInfo1))
        return scriptsModuleInfo1;
      VssScriptsModuleInfo scriptsModuleInfo2 = new VssScriptsModuleInfo(moduleId, scriptsRoot);
      VssScriptsModuleInfo.s_moduleInfoCache[key] = scriptsModuleInfo2;
      return scriptsModuleInfo2;
    }

    public IBundleStreamProvider GetStreamProvider(CultureInfo cultureInfo)
    {
      if (this.m_resourceModuleInfo != null)
        return (IBundleStreamProvider) new BundleTextStreamProvider(ScriptResourceController.GenerateResourceScript(this.m_resourceModuleInfo.GetResourceManager(), this.Id, cultureInfo ?? CultureInfo.CurrentUICulture, true, true));
      return this.ScriptFile != null && this.ScriptFile.FileInfo.Exists ? (IBundleStreamProvider) new BundleFileStreamProvider(this.ScriptFile.FileInfo) : (IBundleStreamProvider) new BundleTextStreamProvider("/* Module not found: " + this.Id + " */");
    }
  }
}
