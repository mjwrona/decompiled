// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITeamFoundationTestManagementResultService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  [DefaultServiceImplementation(typeof (TeamFoundationTestManagementResultService))]
  public interface ITeamFoundationTestManagementResultService : IVssFrameworkService
  {
    Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] AddTestResultsToTestRun(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      TestRun testRun,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] results,
      bool testSessionProperties = false);

    Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] UpdateTestResults(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      TestRun testRun,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] results,
      out TeamProjectTestArtifacts teamProjectTestArtifacts,
      bool autoComputeTestRunState = true);

    Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] UpdateTestResultsWithIterationDetails(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      TestRun testRun,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] results,
      bool autoComputeTestRunState = true);

    TestCaseResult FetchTestCaseResult(
      TestManagementRequestContext context,
      int runId,
      int resultId,
      string projectName,
      out List<TestActionResult> actionResults,
      out List<TestResultParameter> testResultParmeters,
      out List<TestResultAttachment> testResultAttachments);

    IList<TestCaseResult> FetchTestCaseResults(
      TestManagementRequestContext context,
      List<TestCaseResult> results,
      string projectName,
      out List<TestActionResult> actionResults,
      out List<TestResultParameter> testResultParmeters,
      out List<TestResultAttachment> testResultAttachments);

    IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> GetSimilarTestCaseResults(
      TestManagementRequestContext context,
      ProjectInfo projectReference,
      int runId,
      int resultId,
      int subResultId,
      int top,
      string continuationToken);

    Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] BulkUpdateTestResults(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      TestRun testRun,
      int[] resultIds,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult result,
      out TeamProjectTestArtifacts teamProjectTestArtifacts,
      bool autoComputeTestRunState = true);

    List<TestCaseResult> QueryTestResults(
      TestManagementRequestContext context,
      ResultsStoreQuery query,
      out List<TestCaseResultIdentifier> excessIds);

    List<TestCaseResult> QueryTestResultsByRun(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      int runId);

    List<TestCaseResult> GetTestCaseResultsByIds(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      List<TestCaseResultIdentifier> resultIds,
      List<string> fields);

    List<TestCaseResult> GetTestCaseResultsByPointIds(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      int planId,
      List<int> pointIds);

    TestResultArtifacts FetchTestResultArtifacts(
      TestManagementRequestContext context,
      int runId,
      int testCaseResultId,
      string projectName);

    IList<TestResultArtifacts> FetchTestResultsArtifacts(
      TestManagementRequestContext context,
      List<TestCaseResult> results,
      string projectName);

    int GetIterationCount(List<TestActionResult> testActionResults);

    Dictionary<Guid, Tuple<Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>> GetTfsIdentityGuidToIdentityMappingForTestCaseResults(
      TestManagementRequestContext context,
      List<TestCaseResult> testResults);

    TeamProjectTestArtifacts GetTeamProjectTestArtifacts(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      bool includeConfigurations = true,
      bool includeFailureTypes = true);

    Dictionary<string, ShallowReference> GetAreaPathUriMappingForTestCaseResults(
      TestManagementRequestContext context,
      List<TestCaseResult> testResults);

    TestResultsDetails GetAggregatedTestResultDetailsForBuild(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      int buildId,
      string sourceWorkflow,
      List<string> groupByFields,
      Dictionary<string, Tuple<string, List<string>>> filterValues,
      Dictionary<string, string> orderBy,
      bool isRerunOnPassedFilter = false,
      bool shouldIncludeResults = true,
      bool queryRunSummaryForInProgress = false);

    TestResultsDetails GetAggregatedTestResultDetailsForRelease(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      int releaseId,
      int releaseEnvId,
      string sourceWorkflow,
      List<string> groupByFields,
      Dictionary<string, Tuple<string, List<string>>> filterValues,
      Dictionary<string, string> orderBy,
      bool isRerunOnPassedFilter = false,
      bool shouldIncludeResults = true,
      bool queryRunSummaryForInProgress = false);

    List<TestCaseResult> QueryTestResultHistory(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      string automatedTestName,
      int testCaseId,
      DateTime maxCompleteDate,
      int historyDays);

    TestResultHistory QueryTestCaseResultHistory(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      ResultsFilter filter);

    TestHistoryQuery QueryTestHistory(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      TestHistoryQuery filter);

    void UpdateTestCaseReferences(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      IList<TestCaseResult> results);

    void UpdateTestRunSummaryForResults(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      int testRunId,
      IList<TestCaseResult> results);

    void UpdateFlakinessFieldForResults(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      int testRunId,
      IList<TestCaseResult> results);

    List<TestCaseResult> GetTestResultsByFQDN(
      TestManagementRequestContext context,
      TeamProjectReference projectReference,
      int buildId,
      int releaseId,
      int releaseEnvironmentId,
      string sourceWorkflow,
      List<TestCaseReference> testIdentities);

    List<TestResultRecord> QueryTestResultsByTestRunChangedDate(
      TestManagementRequestContext context,
      int projectId,
      int runBatchSize,
      int resultBatchSize,
      TestResultWatermark fromWatermark,
      out TestResultWatermark toWatermark,
      TestArtifactSource dataSource);

    List<TestCaseReferenceRecord> QueryTestCaseReferencesByChangedDate(
      TestManagementRequestContext context,
      int projectId,
      int testCaseRefBatchSize,
      TestCaseReferenceWatermark fromWatermark,
      out TestCaseReferenceWatermark toWatermark,
      TestArtifactSource dataSource);

    List<TestResultExArchivalRecord> QueryTestResultExtensionsByTestRunChangedDate(
      TestManagementRequestContext context,
      int projectId,
      int runBatchSize,
      int resultExBatchSize,
      TestResultExArchivalWatermark fromWatermark,
      DateTime maxTestRunUpdatedDate,
      out TestResultExArchivalWatermark toWatermark,
      TestArtifactSource source);

    TestResultsGroupsForBuild GetTestResultGroupsByBuild(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      int buildId,
      string fields,
      string publishContext);

    TestResultsGroupsForRelease GetTestResultGroupsByRelease(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      int releaseId,
      int releaseEnvId,
      string fields,
      string publishContext);

    IPagedList<FieldDetailsForTestResults> GetTestResultGroupsByBuild(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      int buildId,
      string fields,
      string publishContext,
      int continuationTokenRunId,
      int continuationTokenResultId);

    IPagedList<FieldDetailsForTestResults> GetTestResultGroupsByRelease(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      int releaseId,
      int releaseEnvId,
      string fields,
      string publishContext,
      int continuationTokenRunId,
      int continuationTokenResultId);

    IList<ShallowTestCaseResult> GetTestResultsByBuild(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      int buildId,
      string publishContext,
      List<TestOutcome> outcomes,
      int continuationTokenRunId,
      int continuationTokenResultId,
      int top);

    IList<ShallowTestCaseResult> GetTestResultsByPipeline(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      PipelineReference pipelineReference,
      List<TestOutcome> outcomes,
      int continuationTokenRunId,
      int continuationTokenResultId,
      int top);

    TestResultsDetails GetTestResultsGroupDetails(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      PipelineReference pipelineReference,
      bool shouldIncludeFailedAndAbortedResults = false,
      bool queryGroupSummaryForInProgress = false);

    IList<TestCaseReferenceRecord> GetTestResultsMetaData(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      IList<int> testReferenceIds,
      bool shouldFetchFlakyDetails = false);

    TestResultMetaData UpdateTestResultsMetaData(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      int testCaseReferenceId,
      TestResultMetaDataUpdateInput testResultMetaDataUpdateInput);

    IList<ShallowTestCaseResult> GetTestResultsByRelease(
      TestManagementRequestContext context,
      ProjectInfo projectInfo,
      int releaseId,
      int releaseEnvId,
      string publishContext,
      List<TestOutcome> outcomes,
      int continuationTokenRunId,
      int continuationTokenResultId,
      int top);
  }
}
