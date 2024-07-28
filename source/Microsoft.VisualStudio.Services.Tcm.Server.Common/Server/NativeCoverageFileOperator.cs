// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.NativeCoverageFileOperator
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.CodeCoverage.IO.Coverage.Report;
using Microsoft.TeamFoundation.Coverage.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class NativeCoverageFileOperator : CoverageFileOperatorBase
  {
    public IFileSystem FileSystem { get; set; } = (IFileSystem) new System.IO.Abstractions.FileSystem();

    public override string CoverageTool => "NativeCoverage";

    public override IDictionary<string, string> GetFileLevelCoverageData(
      TestManagementRequestContext requestContext,
      CoverageMergeResults coverageMergeResults,
      IEnumerable<string> filesChangedInIteration)
    {
      using (new SimpleTimer(requestContext.RequestContext, "NativeCoverageFileOperator.AnalyzeCoverageFiles for " + this.FileSystem.Path.GetFileNameWithoutExtension(coverageMergeResults.ModuleName) + "."))
      {
        IDictionary<string, string> coverageJsonFiles = (IDictionary<string, string>) new Dictionary<string, string>();
        if (filesChangedInIteration != null && filesChangedInIteration.Any<string>())
        {
          Dictionary<string, FileCoverageInfo> sourceFiles = new Dictionary<string, FileCoverageInfo>();
          foreach (FileCoverageInfo fileCoverage in coverageMergeResults.FileCoverageList)
            sourceFiles.Add(fileCoverage.FilePath, fileCoverage);
          IDictionary<string, string> source = this.sourcePathFilter.FilterSourceFiles(sourceFiles.Keys.ToList<string>(), filesChangedInIteration);
          int num = Environment.ProcessorCount / 2;
          if (num == 0)
            num = 1;
          ParallelOptions parallelOptions = new ParallelOptions();
          parallelOptions.MaxDegreeOfParallelism = num;
          Action<KeyValuePair<string, string>> body = (Action<KeyValuePair<string, string>>) (sourceFileMap =>
          {
            FileCoverageInfo fileCoverageInfo = sourceFiles[sourceFileMap.Key];
            fileCoverageInfo.FilePath = sourceFileMap.Value;
            string key = this.FileSystem.Path.Combine(coverageMergeResults.JobSpecificTempFolderPath, Guid.NewGuid().ToString(), this.FileSystem.Path.GetFileNameWithoutExtension(sourceFileMap.Key) + ".json");
            string directoryName = this.FileSystem.Path.GetDirectoryName(key);
            if (!this.FileSystem.Directory.Exists(directoryName))
              this.FileSystem.Directory.CreateDirectory(directoryName);
            this.FileSystem.File.WriteAllText(key, fileCoverageInfo.Serialize<FileCoverageInfo>());
            coverageJsonFiles.Add(key, sourceFileMap.Value);
          });
          Parallel.ForEach<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>) source, parallelOptions, body);
        }
        requestContext.Logger.Info(1015909, "NativeCoverageFileOperator: coverageJsonFiles: " + JsonConvert.SerializeObject((object) coverageJsonFiles));
        return coverageJsonFiles;
      }
    }

    public override CoverageMergeResults MergeCoverageFiles(
      TestManagementRequestContext requestContext,
      IEnumerable<string> coverageFilePaths,
      string folderPath)
    {
      long operationThresholdInMs = new CoverageConfiguration().GetMergeOperationThresholdInMs(requestContext.RequestContext);
      using (new SimpleTimer(requestContext.RequestContext, "NativeCoverageFileOperator.MergeCoverageFiles", operationThresholdInMs))
      {
        if (coverageFilePaths == null || !coverageFilePaths.Any<string>())
        {
          requestContext.Logger.Warning(1015241, "NativeCoverageFileOperator.MergeCoverageFiles:Coverage files null.");
          return (CoverageMergeResults) null;
        }
        Dictionary<string, FileCoverageInfo> dictionary = new Dictionary<string, FileCoverageInfo>();
        foreach (string coverageFilePath in coverageFilePaths)
        {
          foreach (FileCoverageInfo fileCoverageInfo2 in JsonUtilities.Deserialize<List<FileCoverageInfo>>(this.FileSystem.File.ReadAllText(coverageFilePath)))
            dictionary[fileCoverageInfo2.FilePath.ToLower()] = dictionary.ContainsKey(fileCoverageInfo2.FilePath.ToLower()) ? FileCoverageInfo.MergeFileCoverage(requestContext, dictionary[fileCoverageInfo2.FilePath.ToLower()], fileCoverageInfo2) : fileCoverageInfo2;
        }
        if (requestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableCodeCoverageFolderViewGeneratorJob"))
        {
          RandomFileNameGenerator fileNameGenerator = new RandomFileNameGenerator();
          string str1 = Path.Combine(folderPath, fileNameGenerator.GetRandomFileNameWithoutExtension());
          requestContext.Logger.Info(1015909, "NativeCoverageFileOperator: mergeFile: " + str1);
          string str2 = dictionary.Values.ToList<FileCoverageInfo>().Serialize<List<FileCoverageInfo>>();
          this.FileSystem.File.WriteAllText(str1, str2);
          return new CoverageMergeResults(this.CoverageTool, folderPath)
          {
            FileCoverageList = dictionary.Values.ToList<FileCoverageInfo>(),
            MergedCoverageFile = str1
          };
        }
        return new CoverageMergeResults(this.CoverageTool, folderPath)
        {
          FileCoverageList = dictionary.Values.ToList<FileCoverageInfo>(),
          MergedCoverageFile = (string) null
        };
      }
    }

    public override void UpdateModuleCoverageInfo(
      TestManagementRequestContext requestContext,
      PipelineContext pipelineContext,
      CoverageMergeResults coverageMergeResults,
      string fileUrl,
      string buildPlatform,
      string buildFlavor)
    {
      BuildConfiguration buildConfiguration = new BuildConfiguration()
      {
        BuildPlatform = buildPlatform,
        BuildFlavor = buildFlavor,
        BuildUri = pipelineContext.Uri,
        BuildId = pipelineContext.Id,
        TeamProjectName = requestContext.ProjectServiceHelper.GetProjectName(pipelineContext.ProjectId)
      }.QueryWithPlatformAndFlavor(requestContext.RequestContext, pipelineContext.ProjectId, pipelineContext.Id, buildPlatform, buildFlavor);
      if (buildConfiguration.BuildConfigurationId == 0 && requestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableJobFailureIfBuildConfigIdNotPresent"))
      {
        requestContext.RequestContext.Trace(1015930, TraceLevel.Error, "TestManagementJob", "CodeCoverageJob", "Build configuration not found for the specified buildFlavor: " + buildFlavor + " buildPlatform: " + buildPlatform + " buildId:" + pipelineContext.Id.ToString());
        throw new Exception("Build configuration not found for the specified buildFlavor: " + buildFlavor + " buildPlatform: " + buildPlatform + " buildId:" + pipelineContext.Id.ToString());
      }
      buildConfiguration.BuildId = pipelineContext.Id;
      List<FileCoverageInfo> fileCoverageList = coverageMergeResults.FileCoverageList;
      BuildCoverage buildCoverage = new BuildCoverage();
      buildCoverage.Configuration = buildConfiguration;
      foreach (FileCoverageInfo fileCoverageInfo in fileCoverageList)
      {
        ModuleCoverage moduleCoverage = new ModuleCoverage()
        {
          Name = fileCoverageInfo.FilePath,
          Signature = Guid.NewGuid(),
          SignatureAge = 1,
          Statistics = new CoverageStatistics()
        };
        moduleCoverage.Statistics.LinesCovered = fileCoverageInfo.LineCoverageStatus.Where<KeyValuePair<uint, CoverageStatus>>((Func<KeyValuePair<uint, CoverageStatus>, bool>) (x => x.Value == CoverageStatus.Covered)).Count<KeyValuePair<uint, CoverageStatus>>();
        moduleCoverage.Statistics.LinesNotCovered = fileCoverageInfo.LineCoverageStatus.Where<KeyValuePair<uint, CoverageStatus>>((Func<KeyValuePair<uint, CoverageStatus>, bool>) (x => x.Value == CoverageStatus.NotCovered)).Count<KeyValuePair<uint, CoverageStatus>>();
        moduleCoverage.Statistics.LinesPartiallyCovered = fileCoverageInfo.LineCoverageStatus.Where<KeyValuePair<uint, CoverageStatus>>((Func<KeyValuePair<uint, CoverageStatus>, bool>) (x => x.Value == CoverageStatus.PartiallyCovered)).Count<KeyValuePair<uint, CoverageStatus>>();
        moduleCoverage.ModuleId = fileCoverageInfo.FilePath.GetHashCode();
        moduleCoverage.CoverageId = fileCoverageInfo.FilePath.GetHashCode();
        moduleCoverage.BlockData = new byte[0];
        buildCoverage.Modules.Add(moduleCoverage);
      }
      buildCoverage.Update(requestContext, 1, pipelineContext.ProjectId);
    }

    public override IEnumerable<FileCoverageDetails> GetFileCoverageDetails(
      TestManagementRequestContext requestContext,
      CoverageMergeResults coverageMergeResult)
    {
      using (new SimpleTimer(requestContext.RequestContext, "NativeCoverageFileOperator.GetFileCoverageDetails"))
      {
        List<FileCoverageDetails> fileCoverageDetails1 = new List<FileCoverageDetails>();
        foreach (FileCoverageInfo fileCoverage in coverageMergeResult.FileCoverageList)
        {
          FileCoverageDetails fileCoverageDetails2 = CommonHelper.GetFileCoverageDetails(fileCoverage.FilePath, fileCoverage.LineCoverageStatus);
          fileCoverageDetails1.Add(fileCoverageDetails2);
        }
        return (IEnumerable<FileCoverageDetails>) fileCoverageDetails1;
      }
    }

    public List<FileData> ExtractFileDataFromMergedFiles(List<string> downloadedCoverageFiles)
    {
      List<FileData> dataFromMergedFiles = new List<FileData>();
      foreach (string downloadedCoverageFile in downloadedCoverageFiles)
      {
        foreach (FileCoverageInfo fileCoverageInfo in JsonUtilities.Deserialize<List<FileCoverageInfo>>(File.ReadAllText(downloadedCoverageFile)))
        {
          FileData fileData = new FileData();
          fileData.Name = fileCoverageInfo.FilePath;
          int num1 = 0;
          int num2 = 0;
          int num3 = 0;
          foreach (KeyValuePair<uint, CoverageStatus> lineCoverageStatu in fileCoverageInfo.LineCoverageStatus)
          {
            if (lineCoverageStatu.Value == CoverageStatus.Covered)
              ++num1;
            else if (lineCoverageStatu.Value == CoverageStatus.NotCovered)
              ++num2;
            else if (lineCoverageStatu.Value == CoverageStatus.PartiallyCovered)
              ++num3;
          }
          ((Microsoft.CodeCoverage.IO.Coverage.CoverageStatistics) fileData).LinesCovered = (uint) num1;
          ((Microsoft.CodeCoverage.IO.Coverage.CoverageStatistics) fileData).LinesNotCovered = (uint) num2;
          ((Microsoft.CodeCoverage.IO.Coverage.CoverageStatistics) fileData).LinesPartiallyCovered = (uint) num3;
          dataFromMergedFiles.Add(fileData);
        }
      }
      return dataFromMergedFiles;
    }
  }
}
