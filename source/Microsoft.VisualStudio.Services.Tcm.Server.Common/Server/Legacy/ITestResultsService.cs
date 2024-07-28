// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.ITestResultsService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  [DefaultServiceImplementation(typeof (TestResultsService))]
  internal interface ITestResultsService : IVssFrameworkService
  {
    void CreateTestResults(
      TestManagementRequestContext context,
      string projectName,
      LegacyTestCaseResult[] results);

    Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun CreateTestRun(
      TestManagementRequestContext context,
      string projectName,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun testRun,
      LegacyTestCaseResult[] results,
      LegacyTestSettings testSettings);

    Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties AbortTestRun(
      TestManagementRequestContext context,
      string projectName,
      int testRunId,
      int revision,
      int options);

    void DeleteTestRun(TestManagementRequestContext context, string projectName, int[] testRunIds);

    LegacyTestCaseResult GetTestResultInMultipleProjects(
      TestManagementRequestContext context,
      int testRunId,
      int testResultId,
      out string projectName);

    List<LegacyTestCaseResult> GetTestResultsByQuery(
      TestManagementRequestContext context,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query,
      int pageSize,
      out List<LegacyTestCaseResultIdentifier> excessIds);

    Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun QueryTestRunByTmiRunId(
      TestManagementRequestContext context,
      Guid tmiRunId);

    List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> Query(
      TestManagementRequestContext context,
      int testRunId,
      Guid owner,
      string buildUri,
      string teamProjectName,
      int planId = -1,
      int skip = 0,
      int top = 2147483647);

    List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> QueryTestRunsInMultipleProjects(
      TestManagementRequestContext context,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query);

    List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> Query(
      TestManagementRequestContext context,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery resultsStoreQuery,
      bool includeStatistics);

    Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[] Update(
      TestManagementRequestContext context,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest[] requests,
      string projectName);

    List<LegacyTestRunStatistic> QueryTestRunStats(
      TestManagementRequestContext context,
      string projectName,
      int testRunId);

    LegacyTestCaseResult[] ResetTestResults(
      TestManagementRequestContext context,
      LegacyTestCaseResultIdentifier[] identifiers,
      string projectName);

    List<LegacyTestCaseResult> QueryByRunAndOutcome(
      TestManagementRequestContext context,
      int testRunId,
      byte outcome,
      int pageSize,
      out List<LegacyTestCaseResultIdentifier> excessIds,
      string projectName);

    List<LegacyTestCaseResult> QueryByRunAndState(
      TestManagementRequestContext context,
      int testRunId,
      byte state,
      int pageSize,
      out List<LegacyTestCaseResultIdentifier> excessIds,
      string projectName);

    List<LegacyTestCaseResult> QueryByRunAndOwner(
      TestManagementRequestContext context,
      int testRunId,
      Guid owner,
      int pageSize,
      out List<LegacyTestCaseResultIdentifier> excessIds,
      string projectName);

    List<LegacyTestCaseResult> QueryByPoint(
      TestManagementRequestContext context,
      string projectName,
      int planId,
      int pointId);

    List<LegacyTestCaseResult> QueryByRun(
      TestManagementRequestContext context,
      int testRunId,
      int pageSize,
      out List<LegacyTestCaseResultIdentifier> webApiExcessIds,
      string projectName,
      bool includeActionResults,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> webApiActionResults,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> webApiParams,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> webApiAttachments);

    List<LegacyTestCaseResult> Fetch(
      TestManagementRequestContext context,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev> webApiIdAndRevs,
      string projectName,
      bool includeActionResults,
      out List<LegacyTestCaseResultIdentifier> webApiDeletedIds,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> webApiActionResults,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> webApiParams,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> webApiAttachments);

    void DeleteAssociatedWorkItems(
      TestManagementRequestContext context,
      IEnumerable<LegacyTestCaseResultIdentifier> identifiers,
      string[] workItemUris);

    void CreateAssociatedWorkItems(
      TestManagementRequestContext context,
      IEnumerable<LegacyTestCaseResultIdentifier> identifiers,
      string[] workItemUris);

    Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties UpdateTestRun(
      TestManagementRequestContext context,
      string projectName,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun webApiTestRun,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment[] attachmentsToAdd,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity[] attachmentsToDelete,
      out int[] attachmentIds,
      bool shouldHyderate);

    Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties UpdateTestRunForLogStoreAttachments(
      TestManagementRequestContext context,
      string projectName,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun webApiTestRun,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment[] attachmentsToAdd,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity[] attachmentsToDelete,
      out int[] attachmentIds,
      bool shouldHyderate);

    List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry> QueryLogEntriesForRun(
      TestManagementRequestContext context,
      string projectName,
      int testRunId,
      int testMessageLogId);

    List<int> CreateLogEntriesForRun(
      TestManagementRequestContext context,
      string projectName,
      int testRunId,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry[] logEntries);

    int QueryTestRunsCount(
      TestManagementRequestContext testManagementRequestContext,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query);

    (Stream contentStream, string contentType, string fileName, long contentLength) DownloadAttachments(
      TestManagementRequestContext context,
      DownloadAttachmentsRequest request,
      out List<(int attachmentId, Guid projectId)> attachmentProjectMap);
  }
}
