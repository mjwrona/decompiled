// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITeamFoundationTestReportService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  [DefaultServiceImplementation(typeof (TeamFoundationTestReportService))]
  public interface ITeamFoundationTestReportService : IVssFrameworkService
  {
    void UpdateTestRunSummaryAndInsights(
      IVssRequestContext context,
      GuidAndString projectId,
      List<int> testRunIds,
      BuildConfiguration buildRef,
      ReleaseReference releaseRef);

    TestResultSummary QueryTestSummaryAndInsightsForBuild(
      IVssRequestContext context,
      GuidAndString projectId,
      string sourceWorkflow,
      BuildConfiguration currentBuild,
      BuildConfiguration previousBuild,
      bool returnSummary,
      bool returnFailureDetails);

    TestResultSummary QueryTestSummaryAndInsightsForPipeline(
      IVssRequestContext context,
      GuidAndString projectId,
      PipelineReference pipelineReference,
      bool includeFailureDetails = false);

    PipelineTestMetrics GetPipelineTestMetrics(
      IVssRequestContext context,
      GuidAndString projectId,
      PipelineReference pipelineReference,
      string mmetricNames,
      bool groupByNode = false);

    TestResultSummary QueryTestSummaryAndInsightsForRelease(
      IVssRequestContext context,
      GuidAndString projectId,
      string sourceWorkflow,
      ReleaseReference currentRelease,
      ReleaseReference previousRelease,
      bool returnSummary,
      bool returnFailureDetails);

    List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> QueryTestResultTrendReport(
      IVssRequestContext tfsRequestContext,
      GuidAndString projectId,
      ResultsFilter filter);

    List<AggregatedDataForResultTrend> QueryTestResultTrendForBuild(
      IVssRequestContext context,
      GuidAndString projectId,
      TestResultTrendFilter filter);

    List<AggregatedDataForResultTrend> QueryTestResultTrendForRelease(
      IVssRequestContext context,
      GuidAndString projectId,
      TestResultTrendFilter filter);

    List<TestResultSummary> QueryTestSummaryForReleases(
      IVssRequestContext context,
      GuidAndString projectId,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference> releases);
  }
}
