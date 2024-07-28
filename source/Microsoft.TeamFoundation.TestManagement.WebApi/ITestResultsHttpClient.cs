// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.ITestResultsHttpClient
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  public interface ITestResultsHttpClient
  {
    Task<TestRun> CreateTestRunAsync(
      RunCreateModel testRun,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<TestRun> CreateTestRunAsync(
      RunCreateModel testRun,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<TestRun> UpdateTestRunAsync(
      RunUpdateModel runUpdateModel,
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<TestRun> UpdateTestRunAsync(
      RunUpdateModel runUpdateModel,
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<List<TestCaseResult>> AddTestResultsToTestRunAsync(
      TestCaseResult[] results,
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<List<TestCaseResult>> AddTestResultsToTestRunAsync(
      TestCaseResult[] results,
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<List<TestCaseResult>> UpdateTestResultsAsync(
      TestCaseResult[] results,
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<List<TestCaseResult>> UpdateTestResultsAsync(
      TestCaseResult[] results,
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<TestAttachmentReference> CreateTestSubResultAttachmentAsync(
      TestAttachmentRequestModel attachmentRequestModel,
      Guid project,
      int runId,
      int testCaseResultId,
      int testSubResultId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<TestAttachmentReference> CreateTestSubResultAttachmentAsync(
      TestAttachmentRequestModel attachmentRequestModel,
      string project,
      int runId,
      int testCaseResultId,
      int testSubResultId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<TestAttachmentReference> CreateTestRunAttachmentAsync(
      TestAttachmentRequestModel attachmentRequestModel,
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<TestAttachmentReference> CreateTestRunAttachmentAsync(
      TestAttachmentRequestModel attachmentRequestModel,
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<TestAttachmentReference> CreateTestResultAttachmentAsync(
      TestAttachmentRequestModel attachmentRequestModel,
      string project,
      int runId,
      int testCaseResultId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<TestAttachmentReference> CreateTestResultAttachmentAsync(
      TestAttachmentRequestModel attachmentRequestModel,
      Guid project,
      int runId,
      int testCaseResultId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<TestRun> GetTestRunByIdAsync(
      Guid project,
      int runId,
      bool? includeDetails = null,
      bool? includeTags = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<TestRun> GetTestRunByIdAsync(
      string project,
      int runId,
      bool? includeDetails = null,
      bool? includeTags = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<List<TestCaseResult>> GetTestResultsAsync(
      Guid project,
      int runId,
      ResultDetails? detailsToInclude = null,
      int? skip = null,
      int? top = null,
      IEnumerable<TestOutcome> outcomes = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<List<TestCaseResult>> GetTestResultsAsync(
      string project,
      int runId,
      ResultDetails? detailsToInclude = null,
      int? skip = null,
      int? top = null,
      IEnumerable<TestOutcome> outcomes = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<PagedList<ShallowTestCaseResult>> GetTestResultsByBuildAsync(
      Guid project,
      int buildId,
      string publishContext = null,
      IEnumerable<TestOutcome> outcomes = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<PagedList<ShallowTestCaseResult>> GetTestResultsByBuildAsync(
      string project,
      int buildId,
      string publishContext = null,
      IEnumerable<TestOutcome> outcomes = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<PagedList<ShallowTestCaseResult>> GetTestResultsByReleaseAsync(
      Guid project,
      int releaseId,
      int? releaseEnvid = null,
      string publishContext = null,
      IEnumerable<TestOutcome> outcomes = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<PagedList<ShallowTestCaseResult>> GetTestResultsByReleaseAsync(
      string project,
      int releaseId,
      int? releaseEnvid = null,
      string publishContext = null,
      IEnumerable<TestOutcome> outcomes = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<TestResultsQuery> GetTestResultsByQueryAsync(
      TestResultsQuery query,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<TestResultsQuery> GetTestResultsByQueryAsync(
      TestResultsQuery query,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<List<AggregatedDataForResultTrend>> QueryResultTrendForBuildAsync(
      TestResultTrendFilter filter,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<List<AggregatedDataForResultTrend>> QueryResultTrendForBuildAsync(
      TestResultTrendFilter filter,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<List<AggregatedDataForResultTrend>> QueryResultTrendForReleaseAsync(
      TestResultTrendFilter filter,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<List<AggregatedDataForResultTrend>> QueryResultTrendForReleaseAsync(
      TestResultTrendFilter filter,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<List<TestAttachment>> GetTestRunAttachmentsAsync(
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<List<TestAttachment>> GetTestRunAttachmentsAsync(
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<Stream> GetTestResultAttachmentContentAsync(
      string project,
      int runId,
      int testCaseResultId,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<Stream> GetTestResultAttachmentContentAsync(
      Guid project,
      int runId,
      int testCaseResultId,
      int attachmentId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<List<TestAttachment>> GetTestResultAttachmentsAsync(
      Guid project,
      int runId,
      int testCaseResultId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<List<TestAttachment>> GetTestResultAttachmentsAsync(
      string project,
      int runId,
      int testCaseResultId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<TestRunStatistic> GetTestRunStatisticsAsync(
      Guid project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<TestRunStatistic> GetTestRunStatisticsAsync(
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<TestSettings> GetTestSettingsByIdAsync(
      string project,
      int testSettingsId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<TestSettings> GetTestSettingsByIdAsync(
      Guid project,
      int testSettingsId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<int> CreateTestSettingsAsync(
      TestSettings testSettings,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<int> CreateTestSettingsAsync(
      TestSettings testSettings,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task DeleteTestSettingsAsync(
      Guid project,
      int testSettingsId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task DeleteTestSettingsAsync(
      string project,
      int testSettingsId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IPagedList<TestRun>> QueryTestRunsAsync2(
      string project,
      DateTime minLastUpdatedDate,
      DateTime maxLastUpdatedDate,
      TestRunState? state = null,
      IEnumerable<int> planIds = null,
      bool? isAutomated = null,
      TestRunPublishContext? publishContext = null,
      IEnumerable<int> buildIds = null,
      IEnumerable<int> buildDefIds = null,
      string branchName = null,
      IEnumerable<int> releaseIds = null,
      IEnumerable<int> releaseDefIds = null,
      IEnumerable<int> releaseEnvIds = null,
      IEnumerable<int> releaseEnvDefIds = null,
      string runTitle = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<IPagedList<TestRun>> QueryTestRunsAsync2(
      Guid project,
      DateTime minLastUpdatedDate,
      DateTime maxLastUpdatedDate,
      TestRunState? state = null,
      IEnumerable<int> planIds = null,
      bool? isAutomated = null,
      TestRunPublishContext? publishContext = null,
      IEnumerable<int> buildIds = null,
      IEnumerable<int> buildDefIds = null,
      string branchName = null,
      IEnumerable<int> releaseIds = null,
      IEnumerable<int> releaseDefIds = null,
      IEnumerable<int> releaseEnvIds = null,
      IEnumerable<int> releaseEnvDefIds = null,
      string runTitle = null,
      int? top = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<TestTagSummary> GetTestTagSummaryForBuildAsync(
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<TestTagSummary> GetTestTagSummaryForBuildAsync(
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<TestTagSummary> GetTestTagSummaryForReleaseAsync(
      string project,
      int releaseId,
      int releaseEnvId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<TestTagSummary> GetTestTagSummaryForReleaseAsync(
      Guid project,
      int releaseId,
      int releaseEnvId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<List<TestTag>> GetTestTagsForBuildAsync(
      string project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<List<TestTag>> GetTestTagsForBuildAsync(
      Guid project,
      int buildId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<List<TestTag>> GetTestTagsForReleaseAsync(
      string project,
      int releaseId,
      int releaseEnvId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<List<TestTag>> GetTestTagsForReleaseAsync(
      Guid project,
      int releaseId,
      int releaseEnvId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<TestRunStatistic> GetTestRunSummaryByOutcomeAsync(
      string project,
      int runId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));

    [EditorBrowsable(EditorBrowsableState.Never)]
    Task<TestResultsSettings> GetTestResultsSettingsAsync(
      string project,
      TestResultsSettingsType? settingsType = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken));
  }
}
