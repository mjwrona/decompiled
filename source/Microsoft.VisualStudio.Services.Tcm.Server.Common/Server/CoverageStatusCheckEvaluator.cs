// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageStatusCheckEvaluator
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class CoverageStatusCheckEvaluator
  {
    public CoverageStatusCheckResult Evaluate(
      TestManagementRequestContext requestContext,
      string source,
      CoverageMetrics coverageMetrics,
      CoverageStatusCheckConfiguration statusCheckConfiguration,
      List<ICoverageStatusCheck> coverageStatusChecks = null)
    {
      CoverageConfiguration coverageConfiguration = new CoverageConfiguration();
      string coverageStatusCheckName = coverageConfiguration.GetCoverageStatusCheckName(requestContext.RequestContext);
      if (string.IsNullOrWhiteSpace(source))
        source = coverageConfiguration.GetCoverageStatusCheckSource(requestContext.RequestContext);
      if (coverageMetrics == null)
        return CoverageStatusCheckResultHelper.GetCoverageStatusCheckResult(source, coverageStatusCheckName, CoverageStatusCheckState.InProgress);
      if (coverageMetrics.CoverageEvaluationStatus == FileCoverageEvaluationStatus.FailedToGetPullRequestChanges)
        return CoverageStatusCheckResultHelper.GetCoverageStatusCheckResult(source, coverageStatusCheckName, CoverageStatusCheckState.Error);
      if (coverageMetrics.CoverageEvaluationStatus == FileCoverageEvaluationStatus.MaxFilesInPullRequestExceeded || coverageMetrics.CoverageEvaluationStatus == FileCoverageEvaluationStatus.NoCodeFilesInPullRequest)
        return CoverageStatusCheckResultHelper.GetCoverageStatusCheckResult(source, coverageStatusCheckName, CoverageStatusCheckState.NotApplicable);
      if (coverageMetrics.CoverageEvaluationStatus == FileCoverageEvaluationStatus.FilePathNotFound)
        return CoverageStatusCheckResultHelper.GetCoverageStatusCheckResult(source, coverageStatusCheckName, CoverageStatusCheckState.Failed);
      this.CoverageStatusCheckResults = new List<CoverageStatusCheckResult>();
      if (coverageStatusChecks == null)
      {
        coverageStatusChecks = new List<ICoverageStatusCheck>();
        coverageStatusChecks.Add((ICoverageStatusCheck) new DiffCoverageStatusCheck());
      }
      if (statusCheckConfiguration == null)
        statusCheckConfiguration = coverageConfiguration.GetCoverageStatusCheckConfiguration(requestContext.RequestContext);
      foreach (ICoverageStatusCheck coverageStatusCheck in coverageStatusChecks)
      {
        try
        {
          this.CoverageStatusCheckResults.Add(coverageStatusCheck.Evaluate(requestContext, source, coverageMetrics, statusCheckConfiguration));
        }
        catch (InvalidStatusCheckConfigurationException ex)
        {
          requestContext.Logger.Error(1015798, string.Format("Error while evaluating {0}: {1}", (object) coverageStatusCheck.GetType(), (object) ex));
          this.CoverageStatusCheckResults.Add(new CoverageStatusCheckResult()
          {
            Source = coverageStatusCheck.Source,
            Name = coverageStatusCheck.Name,
            State = CoverageStatusCheckState.Error,
            Description = ex.Message
          });
        }
        catch (Exception ex)
        {
          requestContext.Logger.Error(1015788, string.Format("Error while evaluating {0}: {1}", (object) coverageStatusCheck.GetType(), (object) ex));
          this.CoverageStatusCheckResults.Add(new CoverageStatusCheckResult()
          {
            Source = coverageStatusCheck.Source,
            Name = coverageStatusCheck.Name,
            State = CoverageStatusCheckState.Error
          });
        }
      }
      return this.GetAggregatedCoverageStatusCheckResult(this.CoverageStatusCheckResults, source, coverageStatusCheckName);
    }

    public CoverageStatusCheckResult Evaluate(
      TestManagementRequestContext requestContext,
      CoverageStatusCheckResult coverageStatusCheckResult,
      Dictionary<string, FolderCoverageResult> folderLevelResults)
    {
      foreach (KeyValuePair<string, FolderCoverageResult> folderLevelResult in folderLevelResults)
      {
        folderLevelResult.Value.CoverageStatusCheck = folderLevelResult.Value.CumulativeDiffCoverage.Coverage.Value * 100.0 >= folderLevelResult.Value.target ? CoverageStatusCheckState.Succeeded : CoverageStatusCheckState.Failed;
        if (folderLevelResult.Value.CoverageStatusCheck < coverageStatusCheckResult.State)
          coverageStatusCheckResult.State = folderLevelResult.Value.CoverageStatusCheck;
      }
      coverageStatusCheckResult.Description = CoverageStatusCheckResultHelper.GetCoverageStatusCheckResultDescription(coverageStatusCheckResult.State, coverageStatusCheckResult.Source);
      return coverageStatusCheckResult;
    }

    public List<CoverageStatusCheckResult> CoverageStatusCheckResults { get; private set; }

    private CoverageStatusCheckResult GetAggregatedCoverageStatusCheckResult(
      List<CoverageStatusCheckResult> coverageStatusCheckResults,
      string source,
      string name)
    {
      int num = 5;
      foreach (CoverageStatusCheckResult statusCheckResult in coverageStatusCheckResults)
      {
        if (statusCheckResult.State < (CoverageStatusCheckState) num)
          num = (int) statusCheckResult.State;
      }
      CoverageStatusCheckResult statusCheckResult1 = CoverageStatusCheckResultHelper.GetCoverageStatusCheckResult(source, name, (CoverageStatusCheckState) num);
      statusCheckResult1.SubResults = coverageStatusCheckResults;
      return statusCheckResult1;
    }
  }
}
