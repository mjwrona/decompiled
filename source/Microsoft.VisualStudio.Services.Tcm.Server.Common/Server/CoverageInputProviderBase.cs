// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageInputProviderBase
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public abstract class CoverageInputProviderBase : ICoverageInputProvider
  {
    protected CoverageToolInput coverageInput;
    public const string CoverageScopesMetadataKey = "CoverageScopes";

    public CoverageInputProviderBase(CoverageToolInput coverageInput) => this.coverageInput = coverageInput;

    public abstract string GetUserDefinedCoverageScopesValue(CodeCoverageFile coverageFile);

    public IEnumerable<CodeCoverageFile> GetCoverageFiles() => (IEnumerable<CodeCoverageFile>) this.coverageInput.Files;

    public IEnumerable<CoverageScope> GetCoverageScopes()
    {
      List<CoverageScope> coverageScopes = new List<CoverageScope>();
      foreach (CodeCoverageFile file in this.coverageInput.Files)
      {
        foreach (CoverageScope coverageScope in this.GetCoverageScopes(file))
        {
          if (!coverageScopes.Contains(coverageScope))
            coverageScopes.Add(coverageScope);
        }
      }
      return (IEnumerable<CoverageScope>) coverageScopes;
    }

    public IEnumerable<CodeCoverageFile> GetCoverageFilesByScope(CoverageScope coverageScope)
    {
      List<CodeCoverageFile> coverageFilesByScope = new List<CodeCoverageFile>();
      foreach (CodeCoverageFile file in this.coverageInput.Files)
      {
        if (this.GetCoverageScopes(file).Contains(coverageScope))
          coverageFilesByScope.Add(file);
      }
      return (IEnumerable<CodeCoverageFile>) coverageFilesByScope;
    }

    public Dictionary<CoverageScope, Dictionary<CodeCoverageFile, string>> GetCoverageScopeIntermediateFilesMap(
      Dictionary<CodeCoverageFile, string> coverageDictionary,
      Dictionary<string, object> ciData)
    {
      Dictionary<CoverageScope, Dictionary<CodeCoverageFile, string>> coverageScopeIntermediateFilesMap = new Dictionary<CoverageScope, Dictionary<CodeCoverageFile, string>>();
      foreach (KeyValuePair<CoverageScope, List<CodeCoverageFile>> coverageScopeToFiles in this.GetCoverageScopeToFilesMap((IEnumerable<CodeCoverageFile>) coverageDictionary.Keys))
      {
        CoverageScope key1 = coverageScopeToFiles.Key;
        List<CodeCoverageFile> codeCoverageFileList = coverageScopeToFiles.Value;
        if (!coverageScopeIntermediateFilesMap.ContainsKey(key1))
        {
          Dictionary<CodeCoverageFile, string> dictionary = new Dictionary<CodeCoverageFile, string>();
          foreach (CodeCoverageFile key2 in codeCoverageFileList)
            dictionary.Add(key2, coverageDictionary[key2]);
          coverageScopeIntermediateFilesMap.Add(key1, dictionary);
        }
      }
      this.AddCoverageScopeTelemetry(coverageScopeIntermediateFilesMap, ciData);
      return coverageScopeIntermediateFilesMap;
    }

    private Dictionary<CoverageScope, List<CodeCoverageFile>> GetCoverageScopeToFilesMap(
      IEnumerable<CodeCoverageFile> codeCoverageFiles)
    {
      Dictionary<CoverageScope, List<CodeCoverageFile>> coverageScopeToFilesMap = new Dictionary<CoverageScope, List<CodeCoverageFile>>((IEqualityComparer<CoverageScope>) new CoverageScopeEqualityComparer());
      foreach (CodeCoverageFile codeCoverageFile in codeCoverageFiles)
      {
        List<CoverageScope> coverageScopes = this.GetCoverageScopes(codeCoverageFile);
        if (coverageScopes != null)
        {
          foreach (CoverageScope key in coverageScopes)
          {
            if (!coverageScopeToFilesMap.Keys.Contains<CoverageScope>(key, (IEqualityComparer<CoverageScope>) new CoverageScopeEqualityComparer()))
              coverageScopeToFilesMap.Add(key, new List<CodeCoverageFile>());
            coverageScopeToFilesMap[key].Add(codeCoverageFile);
          }
        }
      }
      return coverageScopeToFilesMap;
    }

    private void AddCoverageScopeTelemetry(
      Dictionary<CoverageScope, Dictionary<CodeCoverageFile, string>> coverageScopeIntermediateFilesMap,
      Dictionary<string, object> ciData)
    {
      int count = coverageScopeIntermediateFilesMap == null ? 0 : coverageScopeIntermediateFilesMap.Count;
      ciData.Add("CoverageScopesCount", (object) count);
      ciData.Add("BuildConfigCoverageScopesCount", (object) 0);
      ciData.Add("OtherCoverageScopesCount", (object) 0);
      if (count <= 0)
        return;
      IEnumerable<CoverageScope> source1 = coverageScopeIntermediateFilesMap.Where<KeyValuePair<CoverageScope, Dictionary<CodeCoverageFile, string>>>((Func<KeyValuePair<CoverageScope, Dictionary<CodeCoverageFile, string>>, bool>) (x => x.Key is BuildConfigCoverageScope)).Select<KeyValuePair<CoverageScope, Dictionary<CodeCoverageFile, string>>, CoverageScope>((Func<KeyValuePair<CoverageScope, Dictionary<CodeCoverageFile, string>>, CoverageScope>) (x => x.Key));
      ciData.Add("BuildConfigCoverageScopes", (object) JsonConvert.SerializeObject((object) source1));
      ciData["BuildConfigCoverageScopesCount"] = (object) source1.Count<CoverageScope>();
      IEnumerable<CoverageScope> source2 = coverageScopeIntermediateFilesMap.Where<KeyValuePair<CoverageScope, Dictionary<CodeCoverageFile, string>>>((Func<KeyValuePair<CoverageScope, Dictionary<CodeCoverageFile, string>>, bool>) (x => !(x.Key is BuildConfigCoverageScope))).Select<KeyValuePair<CoverageScope, Dictionary<CodeCoverageFile, string>>, CoverageScope>((Func<KeyValuePair<CoverageScope, Dictionary<CodeCoverageFile, string>>, CoverageScope>) (x => x.Key));
      ciData.Add("OtherCoverageScopes", (object) JsonConvert.SerializeObject((object) source2));
      ciData["OtherCoverageScopesCount"] = (object) source2.Count<CoverageScope>();
    }

    private List<CoverageScope> GetCoverageScopes(CodeCoverageFile coverageFile)
    {
      List<CoverageScope> coverageScopes = new List<CoverageScope>();
      string lower1 = coverageFile.BuildFlavor?.Trim()?.ToLower();
      string lower2 = coverageFile.BuildPlatform?.Trim()?.ToLower();
      BuildConfigCoverageScope configCoverageScope = new BuildConfigCoverageScope()
      {
        BuildFlavor = lower1,
        BuildPlatform = lower2
      };
      if (!coverageScopes.Contains((CoverageScope) configCoverageScope))
        coverageScopes.Add((CoverageScope) configCoverageScope);
      string coverageScopesValue = this.GetUserDefinedCoverageScopesValue(coverageFile);
      if (string.IsNullOrEmpty(coverageScopesValue))
        return coverageScopes;
      string[] strArray = coverageScopesValue.Split(',');
      if (strArray == null || strArray.Length == 0)
        return coverageScopes;
      foreach (string str1 in strArray)
      {
        if (!string.IsNullOrEmpty(str1))
        {
          string str2 = str1.Trim();
          CoverageScope coverageScope = new CoverageScope()
          {
            Name = str2
          };
          if (!coverageScopes.Contains(coverageScope))
            coverageScopes.Add(coverageScope);
        }
      }
      return coverageScopes;
    }
  }
}
