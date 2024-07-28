// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITestReportsDatabase
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public interface ITestReportsDatabase
  {
    void UpdateTestRunSummaryAndInsights(
      GuidAndString projectId,
      int testRunId,
      BuildConfiguration buildToCompare,
      ReleaseReference releaseToCompare,
      TestResultsContextType resultsContextType);

    void UpdateTestRunSummaryAndInsights2(
      GuidAndString projectId,
      BuildConfiguration buildToUpdate,
      BuildConfiguration previousBuild,
      ReleaseReference releaseToUpdate,
      ReleaseReference previousRelease);

    void UpdateTestRunSummaryForResults(
      Guid projectId,
      int testRunId,
      IEnumerable<TestCaseResult> results);

    RunSummaryAndInsights QueryTestRunSummaryAndInsightsForBuild(
      GuidAndString projectId,
      string sourceWorkflow,
      BuildConfiguration buildRef,
      bool returnSummary,
      bool returnFailureDetails,
      string categoryName,
      out int runsCount,
      out bool isBuildOld,
      int rundIdThreshold = 0);

    RunSummaryAndInsights QueryTestRunSummaryAndInsightsForRelease(
      GuidAndString projectId,
      string sourceWorkflow,
      ReleaseReference releaseRef,
      bool returnSummary,
      bool returnFailureDetails,
      string categoryName,
      out int runsCount,
      int rundIdThreshold = 0);

    Dictionary<ReleaseReference, RunSummaryAndInsights> QueryTestRunSummaryForReleases(
      GuidAndString projectId,
      List<ReleaseReference> releases,
      string categoryName,
      int runIdThreshold = 0);

    void FetchTestFailureDetails(
      GuidAndString projectId,
      int testRunId,
      BuildConfiguration buildToCompare,
      ReleaseReference releaseToCompare,
      out List<TestCaseResult> currentFailedResults,
      out Dictionary<int, TestCaseResult> previousFailedResultsMap,
      out int prevTestRunContextId);

    void PublishTestSummaryAndFailureDetails(
      GuidAndString projectId,
      int testRunId,
      ResultInsights resultInsights,
      Dictionary<int, string> failingSinceDetails,
      bool includeFailureDetails);
  }
}
