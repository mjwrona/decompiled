// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITcmServiceHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Tcm.WebApi;
using Microsoft.VisualStudio.Services.TestResults.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public interface ITcmServiceHelper
  {
    bool TryQueryLinkedWorkItems(
      IVssRequestContext context,
      GuidAndString projectId,
      string testName,
      out TestToWorkItemLinks links);

    bool TryGetTestResultGroupsByReleaseWithWatermark(
      IVssRequestContext context,
      ProjectInfo projectInfo,
      int releaseId,
      int releaseEnvironmentId,
      string fields,
      string publishContext,
      string continuationToken,
      out IPagedList<FieldDetailsForTestResults> resultsGroupsForRelease);

    bool TryDeleteWorkItemLink(
      IVssRequestContext context,
      GuidAndString projectId,
      string testName,
      int workItemId);

    bool TryAddWorkItemLink(
      IVssRequestContext context,
      GuidAndString projectId,
      WorkItemToTestLinks workItemToTestLinks,
      out WorkItemToTestLinks links);

    bool TryQueryTestCaseResultHistory(
      IVssRequestContext context,
      Guid projectId,
      ResultsFilter filter,
      out TestResultHistory resultHistory);

    bool TryQueryTestHistory(
      IVssRequestContext context,
      Guid projectId,
      TestHistoryQuery filter,
      out TestHistoryQuery result);

    bool TryCreateTestRunLegacy(
      IVssRequestContext context,
      CreateTestRunRequest createTestRunRequest,
      out Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun response);

    bool TryQueryTestRuns(
      IVssRequestContext context,
      Guid projectId,
      DateTime minLastUpdatedDate,
      DateTime maxLastUpdatedDate,
      TestRunState? state,
      IEnumerable<int> planIds,
      bool? isAutomated,
      TestRunPublishContext? publishContext,
      IEnumerable<int> buildIds,
      IEnumerable<int> buildDefIds,
      string branchName,
      IEnumerable<int> releaseIds,
      IEnumerable<int> releaseDefIds,
      IEnumerable<int> releaseEnvIds,
      IEnumerable<int> releaseEnvDefIds,
      string runTitle,
      int top,
      string continuationToken,
      out IPagedList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> runs);

    bool TryGetTestRunsByQuery(
      IVssRequestContext context,
      Guid projectId,
      QueryModel query,
      bool includeIdsOnly,
      bool includeRunDetails,
      int skip,
      int top,
      out IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> testRuns);

    bool TryGetTestResultsByQuery(
      IVssRequestContext context,
      Guid projectId,
      QueryModel query,
      bool includeResultDetails,
      bool includeIterationDetails,
      int skip,
      int top,
      out IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testResults);

    bool TryGetTestResultGroupsByRelease(
      IVssRequestContext context,
      ProjectInfo projectInfo,
      int releaseId,
      int releaseEnvironmentId,
      string fields,
      string publishContext,
      out TestResultsGroupsForRelease resultsGroupsForRelease);

    bool TryGetTestResultsByRelease(
      IVssRequestContext context,
      Guid projectId,
      int releaseId,
      int releaseEnvId,
      string publishContext,
      List<TestOutcome> outcomes,
      int top,
      string continuationToken,
      out IList<ShallowTestCaseResult> results);

    bool TryGetTestResultsByBuild(
      IVssRequestContext context,
      Guid projectId,
      int buildId,
      string publishContext,
      List<TestOutcome> outcomes,
      int top,
      string continuationToken,
      out IList<ShallowTestCaseResult> results);

    bool TryAbortTestRun(
      IVssRequestContext requestContext,
      string projectName,
      int testRunId,
      int revision,
      int options,
      out Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties updatedProperties);

    bool TryGetAssociatedBugsForResult(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      int testCaseResultId,
      out List<WorkItemReference> references);

    bool TryQueryTestResultWorkItems(
      IVssRequestContext context,
      GuidAndString projectId,
      string workItemCategory,
      string automatedTestName,
      int testCaseId,
      DateTime? maxCompleteDate,
      int days,
      int workItemCount,
      out List<WorkItemReference> references);

    bool TryQueryTestActionResults(
      IVssRequestContext requestContext,
      string projectName,
      TestCaseResultIdentifier identifier,
      out QueryTestActionResultResponse response);

    bool TryGetTestResultGroupsByBuild(
      IVssRequestContext context,
      ProjectInfo projectInfo,
      int buildId,
      string fields,
      string publishContext,
      out TestResultsGroupsForBuild resultsGroupsForBuild);

    bool TryGetTestResultGroupsByBuildWithWatermark(
      IVssRequestContext context,
      ProjectInfo projectInfo,
      int buildId,
      string fields,
      string publishContext,
      string continuationToken,
      out IPagedList<FieldDetailsForTestResults> resultsGroupsForBuild);

    bool TryGetTestResultInMultipleProjects(
      IVssRequestContext requestContext,
      int testRunId,
      int testResultId,
      out TestResultAcrossProjectResponse response);

    bool TryGetTestSettingById(
      IVssRequestContext context,
      Guid projectId,
      int testSettingsId,
      out Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings testSettings);

    bool TryCreateTestSettings(
      IVssRequestContext context,
      Guid projectId,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestSettings testSettings,
      out int? testSettingsId);

    bool TryDeleteTestSettings(IVssRequestContext context, Guid projectId, int testSettingsId);

    bool TryCreateTestRun(
      IVssRequestContext context,
      Guid projectId,
      RunCreateModel testRun,
      out Microsoft.TeamFoundation.TestManagement.WebApi.TestRun newTestRun);

    bool TryGetTestResultsByQuery(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query,
      int pageSize,
      out ResultsByQueryResponse reponse);

    bool TryGetTestRunById(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      bool includeDetails,
      out Microsoft.TeamFoundation.TestManagement.WebApi.TestRun testRun);

    bool TryGetTestRunStatistics(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      out Microsoft.TeamFoundation.TestManagement.WebApi.TestRunStatistic testRunStatistic);

    bool TryGetTestRuns(
      IVssRequestContext context,
      Guid projectId,
      string buildUri,
      string owner,
      string tmiRunId,
      int planId,
      bool includeRunDetails,
      bool? automated,
      int skip,
      int top,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> runs);

    bool TryUpdateTestRun(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      RunUpdateModel runUpdateModel,
      out Microsoft.TeamFoundation.TestManagement.WebApi.TestRun updatedTestRun);

    bool TryQueryTestRunByTmiRunId(
      IVssRequestContext requestContext,
      Guid tmiRunId,
      out Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun testRun);

    bool TryDeleteTestRun(IVssRequestContext context, Guid projectId, int runId);

    bool TryAddTestResultsToTestRun(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] results,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> newTestResults);

    bool TryGetTestResults(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> results,
      ResultDetails? detailsToInclude = null,
      int? skip = null,
      int? top = null,
      IEnumerable<TestOutcome> outcomes = null);

    bool TryQueryTestRunsLegacy(
      IVssRequestContext requestContext,
      int testRunId,
      Guid owner,
      string buildUri,
      string teamProjectName,
      int planId,
      int skip,
      int top,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> runsFromRemote);

    bool TryUpdateTestResultsLegacy(
      IVssRequestContext requestContext,
      BulkResultUpdateRequest request,
      out Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[] response);

    bool TryGetTestResultById(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      int resultId,
      bool includeIterationDetails,
      bool includeAssociatedBugs,
      bool includeSubResultDetails,
      out Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult result);

    bool TryQueryTestRunsInMultipleProjects(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> runsFromRemote);

    bool TryUpdateTestResults(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] results,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> updatedResults);

    bool TryQueryTestResultsReportForBuild(
      IVssRequestContext context,
      Guid projectId,
      int buildId,
      string publishContext,
      bool includeFailureDetails,
      BuildReference buildToCompare,
      out TestResultSummary testReport);

    bool TryQueryTestResultsReportForRelease(
      IVssRequestContext context,
      Guid projectId,
      int releaseId,
      int releaseEnvId,
      string publishContext,
      bool includeFailureDetails,
      Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference releaseToCompare,
      out TestResultSummary testReport);

    bool TryQueryTestRuns2Legacy(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery resultsStoreQuery,
      bool includeStatistics,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> runsFromRemote);

    bool TryQueryTestResultsSummaryForReleases(
      IVssRequestContext context,
      Guid projectId,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference> releases,
      out List<TestResultSummary> testSummaryForReleases);

    bool TryGetTestResultsByQuery(
      IVssRequestContext context,
      Guid projectId,
      TestResultsQuery query,
      out TestResultsQuery results);

    bool TryGetTestResultsMetaData(
      IVssRequestContext context,
      Guid projectId,
      IList<int> testReferenceIds,
      ResultMetaDataDetails detailsToInclude,
      out IList<TestResultMetaData> metaDataList);

    bool TryGetTestResultDetailsForBuild(
      IVssRequestContext context,
      Guid projectId,
      int buildId,
      string publishContext,
      string groupBy,
      string filter,
      string orderBy,
      bool shouldIncludeResults,
      bool queryRunSummaryForInProgress,
      out TestResultsDetails resultDetails);

    bool TryGetTestResultDetailsForRelease(
      IVssRequestContext context,
      Guid projectId,
      int releaseId,
      int releaseEnvId,
      string publishContext,
      string groupBy,
      string filter,
      string orderBy,
      bool shouldIncludeResults,
      bool queryRunSummaryForInProgress,
      out TestResultsDetails resultDetails);

    bool TryQueryResultTrendForBuild(
      IVssRequestContext context,
      Guid projectId,
      TestResultTrendFilter filter,
      out List<AggregatedDataForResultTrend> resultTrend);

    bool TryQueryResultTrendForRelease(
      IVssRequestContext context,
      Guid projectId,
      TestResultTrendFilter filter,
      out List<AggregatedDataForResultTrend> resultTrend);

    bool TryCreateTestRunAttachment(
      IVssRequestContext context,
      Guid projectId,
      TestAttachmentRequestModel attachmentRequestModel,
      int runId,
      out TestAttachmentReference attachment);

    bool TryCreateTestResultAttachment(
      IVssRequestContext context,
      Guid projectId,
      TestAttachmentRequestModel attachmentRequestModel,
      int runId,
      int testResultId,
      out TestAttachmentReference attachment);

    bool TryCreateTestSubResultAttachment(
      IVssRequestContext context,
      Guid projectId,
      TestAttachmentRequestModel attachmentRequestModel,
      int runId,
      int testResultId,
      int subResultId,
      out TestAttachmentReference attachment);

    bool TryCreateTestIterationResultAttachment(
      IVssRequestContext context,
      Guid projectId,
      TestAttachmentRequestModel attachmentRequestModel,
      int runId,
      int testResultId,
      int iterationId,
      string actionPath,
      out TestAttachmentReference attachment);

    bool TryGetTestRunAttachment(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      int attachmentId,
      out Attachment attachment);

    bool TryGetTestResultAttachment(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      int resultId,
      int attachmentId,
      out Attachment attachment);

    bool TryGetTestSubResultAttachment(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      int resultId,
      int subResultId,
      int attachmentId,
      out Attachment attachment);

    bool TryGetTestIterationAttachment(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      int resultId,
      int attachmentId,
      int iterationId,
      out Attachment attachment);

    bool TryQueryTestRunStats(
      IVssRequestContext requestContext,
      string projectName,
      int testRunId,
      out List<LegacyTestRunStatistic> testRunStats);

    bool TryGetTestRunAttachments(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      out List<TestAttachment> attachments);

    bool TryQueryByRunAndOutcome(
      IVssRequestContext requestContext,
      int testRunId,
      byte outcome,
      int pageSize,
      string projectName,
      out FetchTestResultsResponse responseFromRemote);

    bool TryResetTestResults(
      IVssRequestContext requestContext,
      string projectName,
      LegacyTestCaseResultIdentifier[] requestsForRemote,
      out LegacyTestCaseResult[] responseFromRemote);

    bool TryGetTestResultAttachments(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      int resultId,
      out List<TestAttachment> attachments);

    bool TryGetTestSubResultAttachments(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      int resultId,
      int subResultId,
      out List<TestAttachment> attachments);

    bool TryDeleteTestRunAttachment(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      int attachmentId);

    bool TryDeleteTestResultAttachment(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      int resultId,
      int attachmentId);

    bool TryQueryTestSummaryByRequirement(
      IVssRequestContext context,
      Guid projectId,
      TestResultsContext resultsContext,
      List<int> workItemIds,
      out List<TestSummaryForWorkItem> summaryForWorkItemList);

    bool TryQueryByRunAndState(
      IVssRequestContext requestContext,
      int testRunId,
      byte state,
      int pageSize,
      string projectName,
      out FetchTestResultsResponse responseFromRemote);

    bool TryGetCodeCoverageSummary(
      IVssRequestContext context,
      Guid projectId,
      int buildId,
      int deltaBuildId,
      out Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary codeCoverageSummary);

    bool TryGetBuildCodeCoverage(
      IVssRequestContext context,
      Guid projectId,
      int buildId,
      int flag,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage> buildCoverage);

    bool TryQueryByRunAndOwner(
      IVssRequestContext requestContext,
      int testRunId,
      Guid owner,
      int pageSize,
      string projectName,
      out FetchTestResultsResponse responseFromRemote);

    bool TryGetTestRunCodeCoverage(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      int flags,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage> runCoverage);

    bool TryGetAfnStripsExistenceMapping(
      IVssRequestContext context,
      string projectName,
      IList<int> testCaseIds,
      out Dictionary<int, bool> existenceMapping);

    bool TryGetAttachmentsByQuery(
      IVssRequestContext context,
      string projectName,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> attachments);

    bool TryUpdateDefaultStrips(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding> list);

    bool TryGetTestAttachments(
      IVssRequestContext requestContext,
      string projectName,
      int runId,
      int resultId,
      int sessionId,
      int subResultId,
      int attachmentId,
      out IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> attachments);

    bool TryQueryByRun(
      IVssRequestContext requestContext,
      QueryByRunRequest queryByRunRequest,
      out FetchTestResultsResponse responseFromRemote);

    bool TryGetTestAttachments(
      IVssRequestContext requestContext,
      string projectName,
      int attachmentId,
      bool getSiblingAttachments,
      out IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> attachments);

    bool IsTestRunInTCM(IVssRequestContext requestContext, int runId, bool queryFlow = true);

    bool IsTestRunInTfs(int runId, bool queryFlow = true);

    bool TryGetTestRunSummaryReport(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      List<string> dimensions,
      out TestExecutionReportData runSummaryReport);

    bool TryGetTestExecutionSummaryReport(
      IVssRequestContext context,
      Guid projectId,
      int planId,
      List<TestAuthoringDetails> testAuthoringDetails,
      List<string> dimensions,
      out TestExecutionReportData executionSummaryReport);

    bool TryQueryByPoint(
      IVssRequestContext requestContext,
      string projectName,
      int planId,
      int pointId,
      out List<LegacyTestCaseResult> resultsFromRemote);

    bool TryCreateTestResultsLegacy(
      IVssRequestContext context,
      string projectName,
      CreateTestResultsRequest request);

    bool TryFetchTestResults(
      IVssRequestContext requestContext,
      FetchTestResultsRequest fetchTestResultsRequest,
      out FetchTestResultsResponse responseFromRemote);

    bool TryQueryCount(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query,
      out int? countFromRemote);

    bool TryUpdateTestRunLegacy(
      IVssRequestContext requestContext,
      string projectName,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun testRun,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment[] attachmentsToAdd,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity[] attachmentsToDelete,
      bool shouldHyderate,
      out UpdateTestRunResponse response);

    bool TryGetTestResultAttachment(
      TestManagementRequestContext requestContext,
      string projectName,
      int attachmentId,
      out TcmAttachment tcmAttachment);

    bool TryGetTestRunLogs(
      IVssRequestContext context,
      Guid projectId,
      int runId,
      out List<TestMessageLogDetails> testMessageLogs);

    bool TryQueuePublishTestResultJob(
      IVssRequestContext context,
      ProjectInfo projectInfo,
      int runId,
      int attachmentId,
      TestResultDocument document,
      out TestResultDocument testResultDocument);
  }
}
