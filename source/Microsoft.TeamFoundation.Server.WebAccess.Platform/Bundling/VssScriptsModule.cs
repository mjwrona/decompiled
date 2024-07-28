// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.VssScriptsModule
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  internal class VssScriptsModule
  {
    private const string c_defineStart = "define(";
    private const string c_copyrightText = "// Copyright (C) Microsoft Corporation. All rights reserved.";
    private string[] m_lines;
    private bool m_alreadyContributed;
    private bool m_alreadyContributedForHashCode;
    private bool m_alreadyPopulated;
    private CultureInfo m_culture;
    private VssScriptsModule.VssBundleState m_bundleState = new VssScriptsModule.VssBundleState();
    private static readonly string[] s_lineSplitSeperator = new string[1]
    {
      Environment.NewLine
    };
    private static readonly Regex s_defineRegEx = new Regex("define\\(([\\\"'][\\w\\/\\.-]+[\\\"'],)?\\s*[^\\)\"']", RegexOptions.Compiled, TimeSpan.FromSeconds(30.0));

    public VssScriptsModuleInfo ModuleInfo { get; private set; }

    protected Dictionary<string, string[]> FileContentCache { get; private set; }

    protected VssScriptsModule(
      VssScriptsModuleInfo moduleInfo,
      Dictionary<string, string[]> contentCache,
      CultureInfo cultureInfo = null)
    {
      this.ModuleInfo = moduleInfo;
      this.m_culture = cultureInfo;
      this.FileContentCache = contentCache;
    }

    protected virtual bool ShouldReadDependencies() => this.ModuleInfo.NeedsResolution;

    protected virtual string GetFilePart() => this.ModuleInfo.GetFilePart();

    private string[] GetLines()
    {
      if (this.m_lines == null)
        this.m_lines = this.ModuleInfo.ReadLinesAndSetHashCode(this.FileContentCache, this.m_culture);
      return this.m_lines;
    }

    protected virtual IEnumerable<VssScriptsModuleInfo> ReadDependencies(
      string staticContentVersion,
      bool excluded = false)
    {
      return this.ModuleInfo.GetDependencies(staticContentVersion);
    }

    protected virtual IEnumerable<VssScriptsModuleInfo> ReadDependentPlugins(
      PluginModuleLookup pluginModules)
    {
      string filePart = this.GetFilePart();
      return !string.IsNullOrEmpty(filePart) ? (IEnumerable<VssScriptsModuleInfo>) pluginModules.GetPluginsForModule(filePart).Select<string, VssScriptsModuleInfo>((Func<string, VssScriptsModuleInfo>) (m => this.CreateScriptsModuleInfo(m + "#?", this.ModuleInfo.ScriptsRoot))).OrderBy<VssScriptsModuleInfo, string>((Func<VssScriptsModuleInfo, string>) (mi => mi.Id)) : Enumerable.Empty<VssScriptsModuleInfo>();
    }

    protected virtual void AddContentToBundle(
      List<string> lines,
      HashSet<string> missingModuleIds,
      bool diagnose)
    {
      if (this.ModuleInfo.IsDebug)
        lines.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "// {0} //", (object) this.ModuleInfo.Id));
      bool flag1 = true;
      bool flag2 = true;
      bool flag3 = false;
      StringBuilder stringBuilder1 = new StringBuilder();
      int count = lines.Count;
      string expandedDependenciesString = (string) null;
      string directDependenciesString = (string) null;
      foreach (string line in this.GetLines())
      {
        if (flag2)
        {
          flag2 = false;
          if (line.StartsWith(VssScriptsModuleInfo.s_dependenciesStart))
          {
            expandedDependenciesString = line;
            continue;
          }
        }
        if (flag1 && line == "// Copyright (C) Microsoft Corporation. All rights reserved.")
        {
          if (!this.m_bundleState.LastModuleHeaderIsVssHeader)
          {
            this.m_bundleState.LastModuleHeaderIsVssHeader = true;
            lines.Add(line);
          }
          flag1 = false;
        }
        else
        {
          if (flag1)
          {
            this.m_bundleState.LastModuleHeaderIsVssHeader = false;
            flag1 = false;
          }
          if (!flag3)
          {
            Match match = VssScriptsModule.s_defineRegEx.Match(line);
            if (match.Success)
            {
              flag3 = true;
              bool flag4 = false;
              int num = match.Index + "define(".Length;
              StringBuilder stringBuilder2 = new StringBuilder();
              if (this.m_bundleState.LastModuleHeaderIsVssHeader)
              {
                if (match.Index > 0)
                  stringBuilder1.Append(line.Substring(0, match.Index));
                string str = stringBuilder1.ToString();
                if (str.Length > 0)
                {
                  if (this.m_bundleState.CodeFoundBeforeDefine.Contains(str))
                    flag4 = true;
                  else if (!this.ModuleInfo.IsDebug || str.TrimStart().Length > 0)
                    this.m_bundleState.CodeFoundBeforeDefine.Add(str);
                }
                if (!flag4 && str.Length > 0)
                {
                  string[] strArray = str.Split(VssScriptsModule.s_lineSplitSeperator, StringSplitOptions.None);
                  for (int index = 0; index < strArray.Length - 1; ++index)
                    lines.Add(strArray[index]);
                  stringBuilder2.Append(strArray[strArray.Length - 1]);
                }
                stringBuilder2.Append("define(");
              }
              else
                stringBuilder2.Append(line.Substring(0, num));
              if (string.IsNullOrEmpty(match.Groups[1].Value))
              {
                stringBuilder2.Append("\"");
                stringBuilder2.Append(this.ModuleInfo.Id);
                stringBuilder2.Append("\",");
                missingModuleIds.Add(this.ModuleInfo.Id);
              }
              stringBuilder2.Append(line.Substring(num));
              lines.Add(stringBuilder2.ToString());
            }
            else if (!this.m_bundleState.LastModuleHeaderIsVssHeader)
              lines.Add(line);
            else if (!this.ModuleInfo.IsDebug || !line.TrimStart().StartsWith("///"))
              stringBuilder1.AppendLine(line);
          }
          else if (!line.StartsWith(VssScriptsModuleInfo.s_directDependenciesStart))
            lines.Add(line);
          else
            directDependenciesString = line;
        }
      }
      if (!diagnose)
        return;
      long size = this.ModuleInfo.ScriptFile == null || this.ModuleInfo.ScriptFile.FileInfo == null || !this.ModuleInfo.ScriptFile.FileInfo.Exists ? 0L : this.ModuleInfo.ScriptFile.FileInfo.Length;
      if (size == 0L)
        size = (long) lines.Skip<string>(count).Select<string, int>((Func<string, int>) (l => l.Length + Environment.NewLine.Length)).Sum();
      lines.Add(DiagnoseHelper.GetBundleModuleInfo(this.ModuleInfo.Id, size, directDependenciesString, expandedDependenciesString));
    }

    public virtual List<string> GetBundleContent(
      PluginModuleLookup pluginModules,
      IEnumerable<string> excludedPaths,
      HashSet<string> missingModuleIds,
      string staticContentVersion,
      HashSet<string> modulesExcludedByPath = null,
      CultureInfo cultureInfo = null,
      bool diagnose = false)
    {
      HashSet<string> excludedModuleIds = new HashSet<string>();
      this.PopulateExcludedModuleIds(excludedModuleIds, pluginModules, cultureInfo, staticContentVersion);
      return this.GetBundleContent(new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase), pluginModules, excludedModuleIds, missingModuleIds, excludedPaths != null ? new HashSet<string>(excludedPaths, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (HashSet<string>) null, modulesExcludedByPath, cultureInfo, diagnose, staticContentVersion);
    }

    public virtual void PopulateContentStreamProviders(
      List<IBundleStreamProvider> streamProviders,
      PluginModuleLookup pluginModules,
      IEnumerable<string> excludedPaths,
      string staticContentVersion,
      HashSet<string> modulesExcludedByPath = null,
      CultureInfo cultureInfo = null,
      bool diagnose = false)
    {
      HashSet<string> excludedModuleIds = new HashSet<string>();
      this.PopulateExcludedModuleIds(excludedModuleIds, pluginModules, cultureInfo, staticContentVersion);
      this.PopulateContentStreamProviders(streamProviders, new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase), pluginModules, excludedModuleIds, excludedPaths != null ? new HashSet<string>(excludedPaths, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (HashSet<string>) null, modulesExcludedByPath, cultureInfo, diagnose, staticContentVersion);
    }

    private void PopulateContentStreamProviders(
      List<IBundleStreamProvider> streamProviders,
      HashSet<string> moduleMap,
      PluginModuleLookup pluginModules,
      HashSet<string> excludedModuleIds,
      HashSet<string> excludedPaths,
      HashSet<string> modulesExcludedByPath,
      CultureInfo cultureInfo,
      bool diagnose,
      string staticContentVersion)
    {
      if (this.m_alreadyContributed)
        return;
      foreach (VssScriptsModuleInfo dependantModule in this.GetDependantModules(pluginModules, excludedModuleIds, staticContentVersion))
      {
        if (dependantModule.StartsWith(excludedPaths))
          modulesExcludedByPath?.Add(dependantModule.Id);
        else if (!moduleMap.Contains(dependantModule.Id))
        {
          VssScriptsModule scriptsModule = this.CreateScriptsModule(dependantModule, this.FileContentCache, cultureInfo);
          moduleMap.Add(dependantModule.Id);
          List<IBundleStreamProvider> streamProviders1 = streamProviders;
          HashSet<string> moduleMap1 = moduleMap;
          PluginModuleLookup pluginModules1 = pluginModules;
          HashSet<string> excludedModuleIds1 = excludedModuleIds;
          HashSet<string> excludedPaths1 = excludedPaths;
          HashSet<string> modulesExcludedByPath1 = modulesExcludedByPath;
          CultureInfo cultureInfo1 = cultureInfo;
          int num = diagnose ? 1 : 0;
          string staticContentVersion1 = staticContentVersion;
          scriptsModule.PopulateContentStreamProviders(streamProviders1, moduleMap1, pluginModules1, excludedModuleIds1, excludedPaths1, modulesExcludedByPath1, cultureInfo1, num != 0, staticContentVersion1);
        }
      }
      if (this.ModuleInfo != null)
        streamProviders.Add(this.ModuleInfo.GetStreamProvider(cultureInfo));
      this.m_alreadyContributed = true;
    }

    private List<string> GetBundleContent(
      HashSet<string> moduleMap,
      PluginModuleLookup pluginModules,
      HashSet<string> excludedModuleIds,
      HashSet<string> missingModuleIds,
      HashSet<string> excludedPaths,
      HashSet<string> modulesExcludedByPath,
      CultureInfo cultureInfo,
      bool diagnose,
      string staticContentVersion)
    {
      List<string> lines = new List<string>();
      if (!this.m_alreadyContributed)
      {
        foreach (VssScriptsModuleInfo dependantModule in this.GetDependantModules(pluginModules, excludedModuleIds, staticContentVersion))
        {
          if (dependantModule.StartsWith(excludedPaths))
            modulesExcludedByPath?.Add(dependantModule.Id);
          else if (!moduleMap.Contains(dependantModule.Id))
          {
            VssScriptsModule scriptsModule = this.CreateScriptsModule(dependantModule, this.FileContentCache, cultureInfo);
            moduleMap.Add(dependantModule.Id);
            lines.AddRange((IEnumerable<string>) scriptsModule.GetBundleContent(moduleMap, pluginModules, excludedModuleIds, missingModuleIds, excludedPaths, modulesExcludedByPath, cultureInfo, diagnose, staticContentVersion));
          }
        }
        this.AddContentToBundle(lines, missingModuleIds, diagnose);
        this.m_alreadyContributed = true;
      }
      return lines;
    }

    public virtual List<byte[]> GetBundleHashList(
      PluginModuleLookup pluginModules,
      IEnumerable<string> excludedPaths,
      string staticContentVersion,
      out int contentLength,
      HashSet<string> modulesExcludedByPath = null,
      CultureInfo cultureInfo = null,
      bool diagnose = false)
    {
      HashSet<string> excludedModuleIds = new HashSet<string>();
      this.PopulateExcludedModuleIds(excludedModuleIds, pluginModules, cultureInfo, staticContentVersion);
      return this.GetBundleHashList(new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase), pluginModules, excludedModuleIds, excludedPaths != null ? new HashSet<string>(excludedPaths, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (HashSet<string>) null, modulesExcludedByPath, cultureInfo, diagnose, staticContentVersion, out contentLength);
    }

    private List<byte[]> GetBundleHashList(
      HashSet<string> moduleMap,
      PluginModuleLookup pluginModules,
      HashSet<string> excludedModuleIds,
      HashSet<string> excludedPaths,
      HashSet<string> modulesExcludedByPath,
      CultureInfo cultureInfo,
      bool diagnose,
      string staticContentVersion,
      out int contentLength)
    {
      contentLength = 0;
      List<byte[]> bundleHashList = new List<byte[]>();
      if (!this.m_alreadyContributedForHashCode)
      {
        foreach (VssScriptsModuleInfo dependantModule in this.GetDependantModules(pluginModules, excludedModuleIds, staticContentVersion))
        {
          if (dependantModule.StartsWith(excludedPaths))
            modulesExcludedByPath?.Add(dependantModule.Id);
          else if (!moduleMap.Contains(dependantModule.Id))
          {
            VssScriptsModule scriptsModule = this.CreateScriptsModule(dependantModule, this.FileContentCache, cultureInfo);
            moduleMap.Add(dependantModule.Id);
            int contentLength1;
            bundleHashList.AddRange((IEnumerable<byte[]>) scriptsModule.GetBundleHashList(moduleMap, pluginModules, excludedModuleIds, excludedPaths, modulesExcludedByPath, cultureInfo, diagnose, staticContentVersion, out contentLength1));
            contentLength += contentLength1;
          }
        }
        int contentLength2;
        this.AddToBundleHashList(bundleHashList, cultureInfo, staticContentVersion, out contentLength2);
        contentLength += contentLength2;
        this.m_alreadyContributedForHashCode = true;
      }
      return bundleHashList;
    }

    protected virtual void AddToBundleHashList(
      List<byte[]> bundleHashList,
      CultureInfo cultureInfo,
      string staticContentVersion,
      out int contentLength)
    {
      byte[] hashCode = this.ModuleInfo.GetHashCode(staticContentVersion, cultureInfo);
      if (hashCode == null)
      {
        string[] strArray = this.ModuleInfo.ReadLinesAndSetHashCode(this.FileContentCache, cultureInfo);
        if (this.FileContentCache != null)
          this.FileContentCache[this.ModuleInfo.FilePath] = strArray;
        hashCode = this.ModuleInfo.GetHashCode(staticContentVersion, cultureInfo);
      }
      contentLength = this.ModuleInfo.GetContentLength(cultureInfo);
      bundleHashList.Add(hashCode);
    }

    private IEnumerable<VssScriptsModuleInfo> GetDependantModules(
      PluginModuleLookup pluginModules,
      HashSet<string> excludedModuleIds,
      string staticContentVersion)
    {
      IEnumerable<VssScriptsModuleInfo> scriptsModuleInfos = Enumerable.Empty<VssScriptsModuleInfo>();
      if (this.ShouldReadDependencies())
        scriptsModuleInfos = scriptsModuleInfos.Concat<VssScriptsModuleInfo>(this.ReadDependencies(staticContentVersion));
      if (pluginModules != null)
        scriptsModuleInfos = scriptsModuleInfos.Concat<VssScriptsModuleInfo>(this.ReadDependentPlugins(pluginModules));
      return scriptsModuleInfos.Where<VssScriptsModuleInfo>((Func<VssScriptsModuleInfo, bool>) (m => !excludedModuleIds.Contains(m.Id)));
    }

    private void PopulateExcludedModuleIds(
      HashSet<string> excludedModuleIds,
      PluginModuleLookup pluginModules,
      CultureInfo cultureInfo,
      string staticContentVersion)
    {
      if (this.m_alreadyPopulated)
        return;
      IEnumerable<VssScriptsModuleInfo> first = Enumerable.Empty<VssScriptsModuleInfo>();
      if (this.ShouldReadDependencies())
        first = first.Concat<VssScriptsModuleInfo>(this.ReadDependencies(staticContentVersion, true));
      if (pluginModules != null)
        first = first.Concat<VssScriptsModuleInfo>(this.ReadDependentPlugins(pluginModules));
      foreach (VssScriptsModuleInfo moduleInfo in first)
      {
        if (!excludedModuleIds.Contains(moduleInfo.Id))
        {
          VssScriptsModule scriptsModule = this.CreateScriptsModule(moduleInfo, this.FileContentCache, cultureInfo);
          excludedModuleIds.Add(moduleInfo.Id);
          HashSet<string> excludedModuleIds1 = excludedModuleIds;
          PluginModuleLookup pluginModules1 = pluginModules;
          CultureInfo cultureInfo1 = cultureInfo;
          string staticContentVersion1 = staticContentVersion;
          scriptsModule.PopulateExcludedModuleIds(excludedModuleIds1, pluginModules1, cultureInfo1, staticContentVersion1);
        }
      }
      this.m_alreadyPopulated = true;
    }

    protected virtual VssScriptsModule CreateScriptsModule(
      VssScriptsModuleInfo moduleInfo,
      Dictionary<string, string[]> contentCache,
      CultureInfo cultureInfo)
    {
      return new VssScriptsModule(moduleInfo, contentCache, cultureInfo)
      {
        m_bundleState = this.m_bundleState
      };
    }

    protected VssScriptsModuleInfo CreateScriptsModuleInfo(FileInfo fileInfo) => VssScriptsModuleInfo.GetVssScriptsModuleInfo(fileInfo);

    protected VssScriptsModuleInfo CreateScriptsModuleInfo(string moduleId, string scriptsRoot) => VssScriptsModuleInfo.GetVssScriptsModuleInfo(moduleId, scriptsRoot);

    public override string ToString() => this.ModuleInfo.Id;

    private class VssBundleState
    {
      public VssBundleState() => this.CodeFoundBeforeDefine = new HashSet<string>();

      public HashSet<string> CodeFoundBeforeDefine { get; set; }

      public bool LastModuleHeaderIsVssHeader { get; set; }
    }
  }
}
