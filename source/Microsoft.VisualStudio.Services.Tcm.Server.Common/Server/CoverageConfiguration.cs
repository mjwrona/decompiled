// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageConfiguration
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class CoverageConfiguration
  {
    public int GetSummaryStatusPendingTimeoutMinutes(IVssRequestContext requestContext) => this.GetValue<int>(requestContext, "/Service/TestManagement/CodeCoverage/SummaryStatusPendingTimeoutMinutes", 10);

    public int GetModuleCoverageMonitorJobTimeoutInSeconds(IVssRequestContext requestContext) => this.GetValue<int>(requestContext, "/Service/TestManagement/CodeCoverage/ModuleCoverageMonitorJobTimeoutInSeconds", 1200);

    public int GetCacheExpiryTimeInHours(IVssRequestContext requestContext) => this.GetValue<int>(requestContext, "/Service/TestManagement/CodeCoverage/CacheExpiryTimeInHours", 24);

    public int GetPipelineCoverageMaxFilesLimit(IVssRequestContext requestContext) => this.GetValue<int>(requestContext, "/Service/TestManagement/CodeCoverage/PipelineCoverageMaxFilesLimit", 100000);

    public TimeSpan GetModuleMergeTimeoutInSeconds(IVssRequestContext requestContext) => TimeSpan.FromSeconds((double) this.GetValue<int>(requestContext, "/Service/TestManagement/CodeCoverage/ModuleCoverageMergeJobTimeoutInSeconds", 1800));

    public TimeSpan GetCoverageMergeInvokerJobTimeoutInSeconds(IVssRequestContext requestContext) => TimeSpan.FromSeconds((double) this.GetValue<int>(requestContext, "/Service/TestManagement/CodeCoverage/CoverageMergeInvokerJobTimeOutInSeconds", 300));

    public int GetMonitorSleepTimeInSeconds(IVssRequestContext requestContext) => this.GetValue<int>(requestContext, "/Service/TestManagement/CodeCoverage/MonitorSleepTimeInSeconds", 10);

    public TimeSpan GetFileCoverageEvaluationJobTimeoutInSeconds(IVssRequestContext requestContext) => TimeSpan.FromSeconds((double) this.GetValue<int>(requestContext, "/Service/TestManagement/CodeCoverage/FileCoverageEvaluationJobTimeoutInSeconds", 300));

    public TimeSpan GetPublishCoverageStatusJobTimeoutInSeconds(IVssRequestContext requestContext) => TimeSpan.FromSeconds((double) this.GetValue<int>(requestContext, "/Service/TestManagement/CodeCoverage/PublishCoverageStatusJobTimeoutInSeconds", 60));

    public int GetMaxFilesInPullRequest(IVssRequestContext requestContext) => this.GetValue<int>(requestContext, "/Service/TestManagement/CodeCoverage/MaxFilesInPullRequest", 100);

    public string[] GetSupportedFileExtensionsForCodeCoverageEvaluation(
      IVssRequestContext requestContext)
    {
      return this.GetValue<string>(requestContext, "/Service/TestManagement/CodeCoverage/SupportedFileExtensionsForCodeCoverageEvaluation", CoverageConfiguration.Defaults.SupportedFileExtensionsForCodeCoverageEvaluation).Split(',');
    }

    public int GetNumberOfModulesPerMergeJobInstance(IVssRequestContext requestContext) => this.GetValue<int>(requestContext, "/Service/TestManagement/CodeCoverage/ModulesPerMergeJob", 10);

    public int GetCoverageMetricsTableMaxRows(IVssRequestContext requestContext) => this.GetValue<int>(requestContext, "/Service/TestManagement/CodeCoverage/CoverageMetricsTableMaxRows", 20);

    public CommentThreadStatus GetCommentDefaultStatus(IVssRequestContext requestContext) => this.GetValue<CommentThreadStatus>(requestContext, "/Service/TestManagement/CodeCoverage/CoverageMetricsCommentDefaultState", CommentThreadStatus.Closed);

    public int GetFileDiffsCallBatchSize(IVssRequestContext requestContext) => this.GetValue<int>(requestContext, "/Service/TestManagement/CodeCoverage/FileDiffsCallBatchSize", 10);

    public CoverageStatusCheckConfiguration GetCoverageStatusCheckConfiguration(
      IVssRequestContext requestContext)
    {
      double num = this.GetValue<double>(requestContext, "/Service/TestManagement/CodeCoverage/DiffCoverageThreshold", 70.0);
      return new CoverageStatusCheckConfiguration()
      {
        DiffCoverageThreshold = new double?(num)
      };
    }

    public long GetMergeOperationThresholdInMs(IVssRequestContext requestContext) => this.GetValue<long>(requestContext, "/Service/TestManagement/CodeCoverage/MergeOperationThresholdInMilliSeconds", 60000L);

    public bool GetCodeCoverageStatusFeature(IVssRequestContext requestContext) => this.GetValue<bool>(requestContext, "/Service/TestManagement/CodeCoverage/CoverageStatusFeature", true);

    public bool GetCoveragePRCommentsFeature(IVssRequestContext requestContext) => this.GetValue<bool>(requestContext, "/Service/TestManagement/CodeCoverage/CoveragePRCommentsFeature", false);

    public string GetCoverageStatusCheckSource(IVssRequestContext requestContext) => this.GetValue<string>(requestContext, "/Service/TestManagement/CodeCoverage/StatusCheckSource", "AzureDevOpsTestService");

    public string GetCoverageStatusCheckName(IVssRequestContext requestContext) => this.GetValue<string>(requestContext, "/Service/TestManagement/CodeCoverage/StatusCheckName", "CodeCoverage");

    public int GetCodeCoverageYamlMaxBytes(IVssRequestContext requestContext) => this.GetValue<int>(requestContext, "/Service/TestManagement/CodeCoverage/CodeCoverageYamlMaxBytes", 102400);

    public string[] GetSupportedCoverageTools(IVssRequestContext requestContext) => this.GetValue<string>(requestContext, "/Service/TestManagement/CodeCoverage/SupportedCoverageTool", CoverageConfiguration.Defaults.SupportedCoverageTools).Split(',');

    public double GetCoverageStatusLowerThreshold(IVssRequestContext requestContext) => (double) this.GetValue<int>(requestContext, "/Service/TestManagement/CodeCoverage/Settings/Badges/Definitions/CoverageStatusLowerThreshold", 30);

    public double GetCoverageStatusUpperThreshold(IVssRequestContext requestContext) => (double) this.GetValue<int>(requestContext, "/Service/TestManagement/CodeCoverage/Settings/Badges/Definitions/CoverageStatusUpperThreshold", 70);

    public int GetPipelineCoverageChangeSummaryBatchSize(IVssRequestContext requestContext) => this.GetValue<int>(requestContext, "/Service/TestManagement/CodeCoverage/PipelineCoverageChangeSummaryBatchSize", 10000);

    public int GetMaxDegreeOfMergeParallelism(IVssRequestContext requestContext) => this.GetValue<int>(requestContext, "/Service/TestManagement/CodeCoverage/DegreeofParallelism", CoverageConfiguration.Defaults.MergeParallelism);

    public string GetAgentSourceFolder(IVssRequestContext requestContext) => this.GetValue<string>(requestContext, "/Service/TestManagement/CodeCoverage/AgentSourceFolder", CoverageConfiguration.Defaults.AgentSourceFolder);

    public TimeSpan GetPipelineScopeLevelFileCoverageAggregationJobTimeoutInSeconds(
      IVssRequestContext requestContext)
    {
      return TimeSpan.FromSeconds((double) this.GetValue<int>(requestContext, "/Service/TestManagement/CodeCoverage/PipelineScopeLevelFileCoverageAggregationJobTimeoutInSeconds", 300));
    }

    public int GetFileCoverageDetailsBatchSize(IVssRequestContext requestContext) => this.GetValue<int>(requestContext, "/Service/TestManagement/CodeCoverage/FileCoverageDetailsBatchSize", 10000);

    public int GetDirectoryCoverageSummaryBatchSize(IVssRequestContext requestContext) => this.GetValue<int>(requestContext, "/Service/TestManagement/CodeCoverage/DirectoryCoverageSummaryBatchSize", 10000);

    public TimeSpan GetPipelineCoverageEvaluationJobTimeoutInSeconds(
      IVssRequestContext requestContext)
    {
      return TimeSpan.FromSeconds((double) this.GetValue<int>(requestContext, "/Service/TestManagement/CodeCoverage/PipelineCoverageEvaluationJobTimeoutInSeconds", 300));
    }

    private T GetValue<T>(IVssRequestContext requestContext, string registryPath, T defaultValue) => requestContext.GetService<IVssRegistryService>().GetValue<T>(requestContext, (RegistryQuery) registryPath, true, defaultValue);

    public class Defaults
    {
      public const int ModuleCoverageMonitorJobTimeOutInSeconds = 1200;
      public const int MergeJobTimeOutInSeconds = 1800;
      public const int MonitorSleepTimeInSeconds = 10;
      public const int FileCoverageEvaluationJobTimeOutInSeconds = 300;
      public const int PublishCoverageStatusJobTimeOutInSeconds = 60;
      public const int PipelineCoverageEvaluationJobTimeOutInSeconds = 300;
      public const int CoverageMergeInvokerJobTimeOutInSeconds = 300;
      public const int ModulesPerMergeJob = 10;
      public const int MaxFilesInPullRequest = 100;
      public static readonly string SupportedFileExtensionsForCodeCoverageEvaluation = ".cs,.vb,.fs,.cpp,.hpp,.c,.h,.ts,.tsx,.js,.jsx,.go,.java,.swift,.py";
      public static readonly string SupportedCoverageTools = "VstestCoverageInput,VstestDotCoverageInput,NativeCoverageInput";
      public const int CoverageMetricsTableMaxRows = 20;
      public const int FileDiffsCallBatchSize = 10;
      public const CommentThreadStatus CoverageMetricsCommentDefaultState = CommentThreadStatus.Closed;
      public const string StatusCheckSource = "AzureDevOpsTestService";
      public const string StatusCheckName = "CodeCoverage";
      public const double DiffCoverageThreshold = 70.0;
      public const bool CoverageStatusFeature = true;
      public const bool FolderLevelCoveragePolicy = false;
      public const bool CoveragePRCommentsFeature = false;
      public const int CodeCoverageYamlMaxBytes = 102400;
      public const int SummaryStatusPendingTimeoutMinutes = 10;
      public const int CoverageStatusLowerThreshold = 30;
      public const int CoverageStatusUpperThreshold = 70;
      public const int PipelineCoverageChangeSummaryBatchSize = 10000;
      public static readonly int MergeParallelism = Environment.ProcessorCount / 2;
      public const int PipelineScopeLevelFileCoverageAggregationJobTimeoutInSeconds = 300;
      public static readonly string AgentSourceFolder = "/s/";
      public const int FileCoverageDetailsBatchSize = 10000;
      public const int PipelineCoverageMaxFilesLimit = 100000;
      public const int CacheExpiryTimeInHours = 24;
      public const int DirectoryCoverageSummaryBatchSize = 10000;
      public const int MergeOperationThresholdInMilliSeconds = 60000;
    }
  }
}
