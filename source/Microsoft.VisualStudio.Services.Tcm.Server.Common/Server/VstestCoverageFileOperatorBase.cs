// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.VstestCoverageFileOperatorBase
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.CodeCoverage.Analysis;
using Microsoft.TeamFoundation.Coverage.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public abstract class VstestCoverageFileOperatorBase : CoverageFileOperatorBase
  {
    public override void UpdateModuleCoverageInfo(
      TestManagementRequestContext requestContext,
      PipelineContext pipelineContext,
      CoverageMergeResults coverageMergeResults,
      string fileUrl,
      string buildPlatform,
      string buildFlavor)
    {
      VstestCoverageMetadataUpdater coverageMetadataUpdater = new VstestCoverageMetadataUpdater();
      requestContext.Logger.Info(1015203, "VstestCoverageFileOperatorBase: Uploading coverage stats");
      TestManagementRequestContext requestContext1 = requestContext;
      PipelineContext pipelineContext1 = pipelineContext;
      string buildPlatform1 = buildPlatform;
      string buildFlavor1 = buildFlavor;
      string mergedCoverageFile = coverageMergeResults.MergedCoverageFile;
      string fileUrl1 = fileUrl;
      coverageMetadataUpdater.UpdateModuleCoverage(requestContext1, pipelineContext1, buildPlatform1, buildFlavor1, mergedCoverageFile, fileUrl1);
      requestContext.Logger.Info(1015204, "VstestCoverageFileOperatorBase: Uploaded the coverage stats");
    }

    public override IDictionary<string, string> GetFileLevelCoverageData(
      TestManagementRequestContext requestContext,
      CoverageMergeResults coverageMergeResults,
      IEnumerable<string> filesChangedInIteration)
    {
      using (new SimpleTimer(requestContext.RequestContext, "VstestCoverageFileOperatorBase.AnalyzeCoverageFiles for " + Path.GetFileNameWithoutExtension(coverageMergeResults.MergedCoverageFile) + "."))
      {
        IDictionary<string, string> coverageJsonFiles = (IDictionary<string, string>) new ConcurrentDictionary<string, string>();
        if (filesChangedInIteration != null && filesChangedInIteration.Any<string>())
        {
          using (CoverageInfo fromFile = CoverageInfo.CreateFromFile(coverageMergeResults.MergedCoverageFile))
          {
            CoverageDS coverageData = fromFile.BuildDataSet();
            IDictionary<string, string> source = this.sourcePathFilter.FilterSourceFiles(coverageData.GetSourceFiles(), filesChangedInIteration);
            int num = Environment.ProcessorCount;
            if (num <= 1)
            {
              requestContext.Logger.Error(1015123, string.Format("VstestCoverageFileOperatorBase.GetFileLevelCoverageData Processor Count invalid {0}. Trying with count 2", (object) Environment.ProcessorCount));
              num = 2;
            }
            bool enableLineCoverageStatusAggregation = requestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableLineCoverageStatusAggregation");
            ParallelOptions parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = num / 2;
            Action<KeyValuePair<string, string>> body = (Action<KeyValuePair<string, string>>) (sourceFile =>
            {
              IList<LineCoverageInfo> lineCoverageInfo = CoverageDS.GetSourceFileLineCoverageInfo(coverageData, sourceFile.Key);
              string str1 = Path.Combine(Path.GetDirectoryName(coverageMergeResults.MergedCoverageFile), Guid.NewGuid().ToString(), Path.GetFileNameWithoutExtension(sourceFile.Key) + ".json");
              string directoryName = Path.GetDirectoryName(str1);
              if (!Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);
              using (FileStream fileStream = new FileStream(str1, FileMode.OpenOrCreate))
              {
                string str2 = this.SerializeCoverageLineData(sourceFile.Value, lineCoverageInfo, enableLineCoverageStatusAggregation);
                using (StreamWriter streamWriter = new StreamWriter((Stream) fileStream, Encoding.UTF8))
                {
                  streamWriter.Write(str2);
                  streamWriter.Flush();
                }
                coverageJsonFiles.Add(str1, sourceFile.Value);
              }
            });
            Parallel.ForEach<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>) source, parallelOptions, body);
          }
        }
        return coverageJsonFiles;
      }
    }

    public override IEnumerable<FileCoverageDetails> GetFileCoverageDetails(
      TestManagementRequestContext requestContext,
      CoverageMergeResults coverageMergeResult)
    {
      using (new SimpleTimer(requestContext.RequestContext, "VstestCoverageFileOperatorBase.GetFileCoverageDetails for: " + Path.GetFileNameWithoutExtension(coverageMergeResult.MergedCoverageFile)))
      {
        List<FileCoverageDetails> fileCoverageDetails1 = new List<FileCoverageDetails>();
        using (CoverageInfo fromFile = CoverageInfo.CreateFromFile(coverageMergeResult.MergedCoverageFile))
        {
          CoverageDS coverageDs = fromFile.BuildDataSet();
          foreach (string sourceFile in coverageDs.GetSourceFiles())
          {
            IList<LineCoverageInfo> lineCoverageInfo = CoverageDS.GetSourceFileLineCoverageInfo(coverageDs, sourceFile);
            FileCoverageDetails fileCoverageDetails2 = CommonHelper.GetFileCoverageDetails(sourceFile, lineCoverageInfo);
            fileCoverageDetails1.Add(fileCoverageDetails2);
          }
        }
        return (IEnumerable<FileCoverageDetails>) fileCoverageDetails1;
      }
    }

    private string SerializeCoverageLineData(
      string sourceFile,
      IList<LineCoverageInfo> lineCoverageInfo,
      bool enableLineCoverageStatusAggregation)
    {
      Dictionary<uint, CoverageStatus> dictionary = !enableLineCoverageStatusAggregation ? this.GetLineCoverageStatusMap(lineCoverageInfo) : this.GetLineCoverageStatusMapWithAggregation(lineCoverageInfo);
      return new FileCoverageInfo()
      {
        FilePath = sourceFile,
        LineCoverageStatus = dictionary
      }.Serialize<FileCoverageInfo>();
    }

    private Dictionary<uint, CoverageStatus> GetLineCoverageStatusMap(
      IList<LineCoverageInfo> lineInfo)
    {
      Dictionary<uint, CoverageStatus> coverageStatusMap = new Dictionary<uint, CoverageStatus>();
      foreach (LineCoverageInfo lineCoverageInfo in (IEnumerable<LineCoverageInfo>) lineInfo)
      {
        for (uint lineBegin = ((LineCoverageInfo) ref lineCoverageInfo).LineBegin; lineBegin <= ((LineCoverageInfo) ref lineCoverageInfo).LineEnd; ++lineBegin)
        {
          switch ((int) ((LineCoverageInfo) ref lineCoverageInfo).CoverageStatus)
          {
            case 0:
              coverageStatusMap[lineBegin] = CoverageStatus.Covered;
              break;
            case 1:
              coverageStatusMap[lineBegin] = CoverageStatus.PartiallyCovered;
              break;
            case 2:
              coverageStatusMap[lineBegin] = CoverageStatus.NotCovered;
              break;
          }
        }
      }
      return coverageStatusMap;
    }

    private Dictionary<uint, CoverageStatus> GetLineCoverageStatusMapWithAggregation(
      IList<LineCoverageInfo> lineInfo)
    {
      Dictionary<uint, CoverageStatus> mapWithAggregation = new Dictionary<uint, CoverageStatus>();
      foreach (LineCoverageInfo lineCoverageInfo in (IEnumerable<LineCoverageInfo>) lineInfo)
      {
        for (uint lineBegin = ((LineCoverageInfo) ref lineCoverageInfo).LineBegin; lineBegin <= ((LineCoverageInfo) ref lineCoverageInfo).LineEnd; ++lineBegin)
        {
          switch ((int) ((LineCoverageInfo) ref lineCoverageInfo).CoverageStatus)
          {
            case 0:
              mapWithAggregation[lineBegin] = !mapWithAggregation.ContainsKey(lineBegin) ? CoverageStatus.Covered : this.AggregateCoverageStatus(mapWithAggregation[lineBegin], CoverageStatus.Covered);
              break;
            case 1:
              mapWithAggregation[lineBegin] = !mapWithAggregation.ContainsKey(lineBegin) ? CoverageStatus.PartiallyCovered : this.AggregateCoverageStatus(mapWithAggregation[lineBegin], CoverageStatus.PartiallyCovered);
              break;
            case 2:
              mapWithAggregation[lineBegin] = !mapWithAggregation.ContainsKey(lineBegin) ? CoverageStatus.NotCovered : this.AggregateCoverageStatus(mapWithAggregation[lineBegin], CoverageStatus.NotCovered);
              break;
          }
        }
      }
      return mapWithAggregation;
    }

    private CoverageStatus AggregateCoverageStatus(
      CoverageStatus coverageStatus1,
      CoverageStatus coverageStatus2)
    {
      return coverageStatus1.Equals((object) coverageStatus2) ? coverageStatus1 : CoverageStatus.PartiallyCovered;
    }
  }
}
