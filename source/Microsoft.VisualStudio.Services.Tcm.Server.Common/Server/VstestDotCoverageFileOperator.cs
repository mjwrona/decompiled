// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.VstestDotCoverageFileOperator
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.CodeCoverage.Core;
using Microsoft.CodeCoverage.IO;
using Microsoft.CodeCoverage.IO.Exceptions;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class VstestDotCoverageFileOperator : VstestCoverageFileOperatorBase
  {
    private ICoverageFileUtility fileUtility;
    private ICoverageFileUtilityV2 fileUtilityV2;

    public VstestDotCoverageFileOperator()
    {
      this.fileUtility = GetFileUtilityInstance.GetCoverageFileUtilityInstance();
      this.fileUtilityV2 = GetFileUtilityInstance.GetCoverageFileUtilityV2Instance();
    }

    public VstestDotCoverageFileOperator(
      ICoverageFileUtility fileUtility,
      ICoverageFileUtilityV2 fileUtilityV2)
    {
      this.fileUtility = fileUtility;
      this.fileUtilityV2 = fileUtilityV2;
    }

    public override CoverageMergeResults MergeCoverageFiles(
      TestManagementRequestContext requestContext,
      IEnumerable<string> coverageFilePaths,
      string folderPath)
    {
      if (coverageFilePaths != null && coverageFilePaths.Count<string>() > 0)
      {
        bool flag1 = false;
        bool flag2 = false;
        CoverageConfiguration coverageConfiguration = new CoverageConfiguration();
        string str = Path.Combine(folderPath, "MergedCov_.coverage");
        int mergeParallelism = coverageConfiguration.GetMaxDegreeOfMergeParallelism(requestContext.RequestContext);
        IList<string> list = (IList<string>) coverageFilePaths.ToList<string>();
        IList<string> stringList = (IList<string>) null;
        bool flag3 = requestContext.IsFeatureEnabled("TestManagement.Server.EnableDuplicateDotCoverageMergeJobUsingCoverageUtilityV2");
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        long operationThresholdInMs = coverageConfiguration.GetMergeOperationThresholdInMs(requestContext.RequestContext);
        using (new SimpleTimer(requestContext.RequestContext, "VstestDotCoverageFileOperator.MergeCoverageFiles", operationThresholdInMs))
        {
          try
          {
            if (flag3)
            {
              string result = this.DotCoverageDuplicateMergeCoverageUtilityV2(requestContext, folderPath, list, dictionary).Result;
              return new CoverageMergeResults(this.CoverageTool, folderPath)
              {
                FileCoverageList = (List<FileCoverageInfo>) null,
                MergedCoverageFile = result
              };
            }
            IList<string> result1 = this.fileUtility.MergeCoverageReportsAsync(str, list, (CoverageMergeOperation) 1, false, mergeParallelism, requestContext.RequestContext.CancellationToken).Result;
            if (result1 != null)
            {
              if (result1.Count > 0)
                return new CoverageMergeResults(this.CoverageTool, folderPath)
                {
                  FileCoverageList = (List<FileCoverageInfo>) null,
                  MergedCoverageFile = result1[0]
                };
            }
          }
          catch (VanguardException ex)
          {
            requestContext.Logger.Error(1015399, string.Format("CoverageMergeJob: few coverage files are corrupted/invalid out of total files: {0}, Exception: {1}", (object) list.Count, (object) ((Exception) ex).Message));
            flag1 = true;
            stringList = this.fileUtility.MergeCoverageReportsAsync(str, list, (CoverageMergeOperation) 1, true, mergeParallelism, requestContext.RequestContext.CancellationToken).Result;
            flag2 = true;
          }
          finally
          {
            dictionary.Add("TotalFilesToBeMerged", (object) list.Count);
            dictionary.Add("CodeCoverageLibraryDetails", (object) "Microsoft.CodeCoverage.IO.17.5.0");
            dictionary.Add("CoverageMergeJobEncounteredInvalidFiles", (object) flag1);
            dictionary.Add("SkippedInvalidCoverageFiles", (object) flag2);
            CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) dictionary);
            TelemetryLogger.Instance.PublishData(requestContext.RequestContext, nameof (VstestDotCoverageFileOperator), cid);
          }
        }
      }
      return (CoverageMergeResults) null;
    }

    private async Task<string> DotCoverageDuplicateMergeCoverageUtilityV2(
      TestManagementRequestContext requestContext,
      string folderPath,
      IList<string> coverageFiles,
      Dictionary<string, object> ciData)
    {
      string mergedCovdataFile = Path.Combine(folderPath, "MergedCov_.coverage");
      try
      {
        await this.fileUtilityV2.MergeCoverageFilesAsync(mergedCovdataFile, coverageFiles, CancellationToken.None);
        return mergedCovdataFile;
      }
      catch (VanguardException ex)
      {
        requestContext.Logger.Error(1015399, string.Format("CoverageMergeJob: few coverage files are corrupted/invalid out of total files: {0}, Exception: {1}", (object) coverageFiles.Count, (object) ((Exception) ex).Message));
        return (string) null;
      }
      finally
      {
        ciData.Add("MergedFileOutputPath", (object) mergedCovdataFile);
        ciData.Add("NumberOfFileTobeMerged", (object) coverageFiles.Count);
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) ciData);
        TelemetryLogger.Instance.PublishData(requestContext.RequestContext, nameof (VstestDotCoverageFileOperator), cid);
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
      VsTestDotCoverageMetadataUpdater coverageMetadataUpdater = new VsTestDotCoverageMetadataUpdater();
      requestContext.Logger.Info(1015503, "VsTestDotCoverageFileOperator: Uploading coverage stats");
      string mergedCoverageFile = coverageMergeResults.MergedCoverageFile;
      TestManagementRequestContext requestContext1 = requestContext;
      PipelineContext pipelineContext1 = pipelineContext;
      string buildPlatform1 = buildPlatform;
      string buildFlavor1 = buildFlavor;
      string mergedCovFilePath = mergedCoverageFile;
      string empty = string.Empty;
      coverageMetadataUpdater.UpdateModuleCoverage(requestContext1, pipelineContext1, buildPlatform1, buildFlavor1, mergedCovFilePath, empty);
      requestContext.Logger.Info(1015504, "VsTestDotCoverageFileOperator: Uploaded the coverage stats");
    }

    public override string CoverageTool => "VstestDotCoverage";
  }
}
