// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITestReportsHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public interface ITestReportsHelper
  {
    TestResultSummary QueryTestReportForBuild(
      GuidAndString projectId,
      BuildReference build,
      string sourceWorkflow,
      BuildReference buildToCompare,
      bool returnSummary,
      bool returnFailureDetails);

    TestResultSummary QueryTestReportForRelease(
      GuidAndString projectId,
      Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference release,
      string sourceWorkflow,
      Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference releaseToCompare,
      bool returnSummary,
      bool returnFailureDetails);

    TestResultSummary QueryTestReportForPipeline(GuidAndString projectId, bool returnFailureDetails);

    List<TestResultSummary> QueryTestSummaryForReleases(
      GuidAndString projectId,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference> releases);

    List<WorkItemReference> QueryTestResultWorkItems(
      GuidAndString projectId,
      string workItemCategory,
      string automatedTestName,
      int testCaseId,
      DateTime? maxCompleteDate,
      int days,
      int workItemCount);

    TestResultHistory QueryTestCaseResultHistory(GuidAndString projectId, ResultsFilter filter);

    List<AggregatedDataForResultTrend> QueryTestResultTrendForBuild(
      GuidAndString projectId,
      TestResultTrendFilter filter);

    List<AggregatedDataForResultTrend> QueryTestResultTrendForRelease(
      GuidAndString projectId,
      TestResultTrendFilter filter);

    void SecureTestResultSummary(TestResultSummary summary);

    void SecureTestResultTrend(List<AggregatedDataForResultTrend> result, Guid projectId);
  }
}
