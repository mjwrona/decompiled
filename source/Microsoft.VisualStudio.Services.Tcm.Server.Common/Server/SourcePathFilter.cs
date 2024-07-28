// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.SourcePathFilter
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class SourcePathFilter : ISourcePathFilter
  {
    private const int MaxMatchMissAllowed = 5;
    private const int MaxDepth = 100;
    private TestManagementRequestContext _tcmRequestContext;
    private PipelineContext _pipelineContext;

    public SourcePathFilter()
    {
    }

    public SourcePathFilter(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext)
    {
      this._tcmRequestContext = tcmRequestContext;
      this._pipelineContext = pipelineContext;
    }

    public IDictionary<string, string> FilterSourceFiles(
      List<string> inputFilePaths,
      IEnumerable<string> partiallyMatchingPaths)
    {
      IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>();
      foreach (string partiallyMatchingPath in partiallyMatchingPaths)
      {
        string normalizedPath = partiallyMatchingPath.TrimStart('/', '\\');
        string backslashPath = normalizedPath.Replace("/", "\\");
        IEnumerable<string> source1 = inputFilePaths.Where<string>((Func<string, bool>) (inputFilePath => inputFilePath.EndsWith(backslashPath, StringComparison.OrdinalIgnoreCase)));
        if (source1.Any<string>())
        {
          dictionary.Add(source1.OrderBy<string, int>((Func<string, int>) (matchFile => matchFile.Length)).FirstOrDefault<string>(), partiallyMatchingPath);
        }
        else
        {
          IEnumerable<string> source2 = inputFilePaths.Where<string>((Func<string, bool>) (inputFilePath => inputFilePath.EndsWith(normalizedPath, StringComparison.OrdinalIgnoreCase)));
          if (source2.Any<string>())
            dictionary.Add(source2.OrderBy<string, int>((Func<string, int>) (matchingFile => matchingFile.Length)).FirstOrDefault<string>(), partiallyMatchingPath);
        }
      }
      return dictionary;
    }

    public IDictionary<string, string> GetSourceControlPaths(
      List<string> coveredFiles,
      List<string> sourceFolders)
    {
      IDictionary<string, string> sourceControlPaths = (IDictionary<string, string>) new Dictionary<string, string>();
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      try
      {
        using (new SimpleTimer(this._tcmRequestContext.RequestContext, "SourcePathFilter: GetSourceControlPaths", dictionary))
        {
          List<string> stringList1 = new List<string>();
          List<string> stringList2 = new List<string>();
          AzureReposProvider azureReposProvider = new AzureReposProvider();
          int num1 = 0;
          foreach (string sourceFolder in sourceFolders)
          {
            string str = sourceFolder.Replace("\\", "/");
            stringList1.Add(str.EndsWith("/") ? str : str + "/");
          }
          dictionary.Add("SourceFoldersFromCoverageData", (object) sourceFolders.Count);
          dictionary.Add("CoveredFilesCount", (object) coveredFiles.Count);
          dictionary.Add("PipielineId", (object) this._pipelineContext.Id);
          dictionary.Add("ProjectId", (object) this._pipelineContext.ProjectId);
          dictionary.Add("RepositoryId", (object) this._pipelineContext.RepositoryId);
          int num2 = 0;
          int num3 = 0;
          int num4 = 0;
          foreach (string coveredFile in coveredFiles)
          {
            if (!sourceControlPaths.ContainsKey(coveredFile))
            {
              bool flag = false;
              string inputFilePath = coveredFile.Replace("\\", "/");
              foreach (string str in stringList1)
              {
                if (inputFilePath.StartsWith(str))
                {
                  flag = true;
                  sourceControlPaths.Add(coveredFile, inputFilePath.Substring(str.Length));
                  ++num2;
                  break;
                }
              }
              if (!flag && num1 < 5)
              {
                foreach (string str in stringList2)
                {
                  if (inputFilePath.StartsWith(str) && azureReposProvider.IsFilePresentInRepo(this._tcmRequestContext, this._pipelineContext, inputFilePath.Substring(str.Length)))
                  {
                    flag = true;
                    sourceControlPaths.Add(coveredFile, inputFilePath.Substring(str.Length));
                    stringList2.Remove(str);
                    stringList1.Add(str);
                    break;
                  }
                }
                if (!flag)
                {
                  string predefinedSourceDirectory = this.FindPredefinedSourceDirectory(inputFilePath, (IVersionControlProvider) azureReposProvider);
                  if (!string.IsNullOrEmpty(predefinedSourceDirectory))
                  {
                    ++num3;
                    sourceControlPaths.Add(coveredFile, predefinedSourceDirectory);
                    stringList2.Add(inputFilePath.Substring(0, inputFilePath.Length - predefinedSourceDirectory.Length));
                  }
                  else
                  {
                    string sourceDirectory = this.FindSourceDirectory(inputFilePath, (IVersionControlProvider) azureReposProvider);
                    if (!string.IsNullOrEmpty(sourceDirectory))
                    {
                      sourceControlPaths.Add(coveredFile, sourceDirectory);
                      ++num4;
                      stringList2.Add(inputFilePath.Substring(0, inputFilePath.Length - sourceDirectory.Length));
                    }
                    else
                    {
                      this._tcmRequestContext.Logger.Info(1015646, "GetSourceControlPaths: Failed to map the file in repo: " + coveredFile);
                      ++num1;
                    }
                  }
                }
              }
            }
          }
          dictionary.Add("MissCount", (object) num1);
          dictionary.Add("MatchesFromSourceDirectory", (object) num2);
          dictionary.Add("MatchesFromPredefinedDirectory", (object) num3);
          dictionary.Add("MatchesFromVCPath", (object) num4);
          dictionary.Add("TotalMatches", (object) sourceControlPaths.Count);
        }
      }
      catch (Exception ex)
      {
        this._tcmRequestContext.Logger.Error(1015645, string.Format("GetSourceControlPaths: Error mapping files: {0}", (object) ex));
      }
      finally
      {
        TelemetryLogger.Instance.PublishData(this._tcmRequestContext.RequestContext, nameof (GetSourceControlPaths), new CustomerIntelligenceData((IDictionary<string, object>) dictionary));
      }
      return sourceControlPaths;
    }

    private string FindSourceDirectory(
      string inputFilePath,
      IVersionControlProvider versionControlProvider)
    {
      string sourceDirectory = inputFilePath.Substring(inputFilePath.IndexOf('/') + 1);
      for (int index = 1; !string.IsNullOrWhiteSpace(sourceDirectory) && index < 100 && !versionControlProvider.IsFilePresentInRepo(this._tcmRequestContext, this._pipelineContext, sourceDirectory); ++index)
        sourceDirectory = sourceDirectory.Contains<char>('/') ? sourceDirectory.Substring(sourceDirectory.IndexOf('/') + 1) : string.Empty;
      return sourceDirectory;
    }

    private string FindPredefinedSourceDirectory(
      string inputFilePath,
      IVersionControlProvider versionControlProvider)
    {
      string agentSourceFolder = new CoverageConfiguration().GetAgentSourceFolder(this._tcmRequestContext.RequestContext);
      int num = inputFilePath.IndexOf(agentSourceFolder);
      if (num != -1)
      {
        string filePath = inputFilePath.Substring(num + agentSourceFolder.Length);
        if (versionControlProvider.IsFilePresentInRepo(this._tcmRequestContext, this._pipelineContext, filePath))
          return filePath;
      }
      return string.Empty;
    }
  }
}
