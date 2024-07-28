// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.TestResultsService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  internal class TestResultsService : 
    TeamFoundationTestManagementService,
    ITestResultsService,
    IVssFrameworkService
  {
    public void CreateTestResults(
      TestManagementRequestContext context,
      string projectName,
      LegacyTestCaseResult[] results)
    {
      TestCaseResult[] convertedResults = this.GetConvertedResults(results);
      TestCaseResult.Create(context, convertedResults, false, projectName);
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun CreateTestRun(
      TestManagementRequestContext context,
      string projectName,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun testRun,
      LegacyTestCaseResult[] results,
      LegacyTestSettings testSettings)
    {
      TestCaseResult[] convertedResults = this.GetConvertedResults(results);
      TestSettings convertedSettings = this.GetConvertedSettings(testSettings);
      return this.Convert(this.GetConvertedTestRun(testRun).Create(context, convertedSettings, convertedResults, false, projectName));
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties AbortTestRun(
      TestManagementRequestContext context,
      string projectName,
      int testRunId,
      int revision,
      int options)
    {
      return this.Convert(TestRun.Abort(context, testRunId, revision, projectName, (TestRunAbortOptions) options));
    }

    public void DeleteTestRun(
      TestManagementRequestContext context,
      string projectName,
      int[] testRunIds)
    {
      TestRun.Delete(context, testRunIds, projectName);
    }

    public LegacyTestCaseResult GetTestResultInMultipleProjects(
      TestManagementRequestContext context,
      int testRunId,
      int testResultId,
      out string projectName)
    {
      return this.Convert(TestCaseResult.FindInMultipleProjects(context, testRunId, testResultId, out projectName));
    }

    public List<LegacyTestCaseResult> GetTestResultsByQuery(
      TestManagementRequestContext context,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query,
      int pageSize,
      out List<LegacyTestCaseResultIdentifier> excessIds)
    {
      List<TestCaseResultIdentifier> excessIds1;
      List<LegacyTestCaseResult> testResultsByQuery = this.Convert(TestCaseResult.Query(context, ResultsStoreQueryContractConverter.Convert(query), pageSize, out excessIds1));
      ref List<LegacyTestCaseResultIdentifier> local = ref excessIds;
      IEnumerable<LegacyTestCaseResultIdentifier> source = TestCaseResultIdentifierConverter.Convert((IEnumerable<TestCaseResultIdentifier>) excessIds1);
      List<LegacyTestCaseResultIdentifier> list = source != null ? source.ToList<LegacyTestCaseResultIdentifier>() : (List<LegacyTestCaseResultIdentifier>) null;
      local = list;
      return testResultsByQuery;
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun QueryTestRunByTmiRunId(
      TestManagementRequestContext context,
      Guid tmiRunId)
    {
      return TestRunContractConverter.Convert(TestRun.QueryTestRunByTmiRunId(context, tmiRunId));
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> Query(
      TestManagementRequestContext context,
      int testRunId,
      Guid owner,
      string buildUri,
      string teamProjectName,
      int planId = -1,
      int skip = 0,
      int top = 2147483647)
    {
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> source = TestRunContractConverter.Convert((IEnumerable<TestRun>) TestRun.Query(context, testRunId, owner, buildUri, teamProjectName, planId, skip, top));
      return source == null ? (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) null : source.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>();
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> QueryTestRunsInMultipleProjects(
      TestManagementRequestContext context,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query)
    {
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> source = TestRunContractConverter.Convert((IEnumerable<TestRun>) TestRun.QueryInMultipleProjects(context, ResultsStoreQueryContractConverter.Convert(query)));
      return source == null ? (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) null : source.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>();
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> Query(
      TestManagementRequestContext context,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery resultsStoreQuery,
      bool includeStatistics = false)
    {
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> source = TestRunContractConverter.Convert((IEnumerable<TestRun>) TestRun.Query(context, ResultsStoreQueryContractConverter.Convert(resultsStoreQuery), includeStatistics));
      return source == null ? (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) null : source.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>();
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[] Update(
      TestManagementRequestContext context,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest[] requests,
      string projectName)
    {
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateRequest> source1 = ResultUpdateRequestConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest>) requests);
      Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateRequest[] array = source1 != null ? source1.ToArray<Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateRequest>() : (Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateRequest[]) null;
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse> source2 = ResultUpdateResponseConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.ResultUpdateResponse>) TestCaseResult.Update(context, array, projectName));
      return source2 == null ? (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[]) null : source2.ToArray<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse>();
    }

    public List<LegacyTestRunStatistic> QueryTestRunStats(
      TestManagementRequestContext context,
      string projectName,
      int testRunId)
    {
      IEnumerable<LegacyTestRunStatistic> source = TestRunStatisticConverter.Convert((IEnumerable<TestRunStatistic>) TestRunStatistic.Query(context, projectName, testRunId));
      return source == null ? (List<LegacyTestRunStatistic>) null : source.ToList<LegacyTestRunStatistic>();
    }

    public LegacyTestCaseResult[] ResetTestResults(
      TestManagementRequestContext context,
      LegacyTestCaseResultIdentifier[] identifiers,
      string projectName)
    {
      TestManagementRequestContext context1 = context;
      IEnumerable<TestCaseResultIdentifier> source1 = TestCaseResultIdentifierConverter.Convert((IEnumerable<LegacyTestCaseResultIdentifier>) identifiers);
      TestCaseResultIdentifier[] array = source1 != null ? source1.ToArray<TestCaseResultIdentifier>() : (TestCaseResultIdentifier[]) null;
      string projectName1 = projectName;
      IEnumerable<LegacyTestCaseResult> source2 = TestCaseResultContractConverter.Convert((IEnumerable<TestCaseResult>) TestCaseResult.Reset(context1, array, projectName1));
      return source2 == null ? (LegacyTestCaseResult[]) null : source2.ToArray<LegacyTestCaseResult>();
    }

    public List<LegacyTestCaseResult> QueryByRunAndOutcome(
      TestManagementRequestContext context,
      int testRunId,
      byte outcome,
      int pageSize,
      out List<LegacyTestCaseResultIdentifier> excessIds,
      string projectName)
    {
      List<TestCaseResultIdentifier> excessIds1;
      IEnumerable<LegacyTestCaseResult> source1 = TestCaseResultContractConverter.Convert((IEnumerable<TestCaseResult>) TestCaseResult.QueryByRunAndOutcome(context, testRunId, outcome, pageSize, out excessIds1, projectName));
      List<LegacyTestCaseResult> list1 = source1 != null ? source1.ToList<LegacyTestCaseResult>() : (List<LegacyTestCaseResult>) null;
      ref List<LegacyTestCaseResultIdentifier> local = ref excessIds;
      IEnumerable<LegacyTestCaseResultIdentifier> source2 = TestCaseResultIdentifierConverter.Convert((IEnumerable<TestCaseResultIdentifier>) excessIds1);
      List<LegacyTestCaseResultIdentifier> list2 = source2 != null ? source2.ToList<LegacyTestCaseResultIdentifier>() : (List<LegacyTestCaseResultIdentifier>) null;
      local = list2;
      return list1;
    }

    public List<LegacyTestCaseResult> QueryByRunAndState(
      TestManagementRequestContext context,
      int testRunId,
      byte state,
      int pageSize,
      out List<LegacyTestCaseResultIdentifier> excessIds,
      string projectName)
    {
      List<TestCaseResultIdentifier> excessIds1;
      IEnumerable<LegacyTestCaseResult> source1 = TestCaseResultContractConverter.Convert((IEnumerable<TestCaseResult>) TestCaseResult.QueryByRunAndState(context, testRunId, state, pageSize, out excessIds1, projectName));
      List<LegacyTestCaseResult> list1 = source1 != null ? source1.ToList<LegacyTestCaseResult>() : (List<LegacyTestCaseResult>) null;
      ref List<LegacyTestCaseResultIdentifier> local = ref excessIds;
      IEnumerable<LegacyTestCaseResultIdentifier> source2 = TestCaseResultIdentifierConverter.Convert((IEnumerable<TestCaseResultIdentifier>) excessIds1);
      List<LegacyTestCaseResultIdentifier> list2 = source2 != null ? source2.ToList<LegacyTestCaseResultIdentifier>() : (List<LegacyTestCaseResultIdentifier>) null;
      local = list2;
      return list1;
    }

    public List<LegacyTestCaseResult> QueryByRunAndOwner(
      TestManagementRequestContext context,
      int testRunId,
      Guid owner,
      int pageSize,
      out List<LegacyTestCaseResultIdentifier> excessIds,
      string projectName)
    {
      List<TestCaseResultIdentifier> excessIds1;
      IEnumerable<LegacyTestCaseResult> source1 = TestCaseResultContractConverter.Convert((IEnumerable<TestCaseResult>) TestCaseResult.QueryByRunAndOwner(context, testRunId, owner, pageSize, out excessIds1, projectName));
      List<LegacyTestCaseResult> list1 = source1 != null ? source1.ToList<LegacyTestCaseResult>() : (List<LegacyTestCaseResult>) null;
      ref List<LegacyTestCaseResultIdentifier> local = ref excessIds;
      IEnumerable<LegacyTestCaseResultIdentifier> source2 = TestCaseResultIdentifierConverter.Convert((IEnumerable<TestCaseResultIdentifier>) excessIds1);
      List<LegacyTestCaseResultIdentifier> list2 = source2 != null ? source2.ToList<LegacyTestCaseResultIdentifier>() : (List<LegacyTestCaseResultIdentifier>) null;
      local = list2;
      return list1;
    }

    public List<LegacyTestCaseResult> QueryByPoint(
      TestManagementRequestContext context,
      string projectName,
      int planId,
      int pointId)
    {
      IEnumerable<LegacyTestCaseResult> source = TestCaseResultContractConverter.Convert((IEnumerable<TestCaseResult>) TestCaseResult.QueryByPoint(context, projectName, planId, pointId));
      return source == null ? (List<LegacyTestCaseResult>) null : source.ToList<LegacyTestCaseResult>();
    }

    public List<LegacyTestCaseResult> QueryByRun(
      TestManagementRequestContext context,
      int testRunId,
      int pageSize,
      out List<LegacyTestCaseResultIdentifier> webApiExcessIds,
      string projectName,
      bool includeActionResults,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> webApiActionResults,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> webApiParams,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> webApiAttachments)
    {
      List<TestCaseResultIdentifier> excessIds;
      List<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult> actionResults;
      List<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter> parameters;
      List<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment> attachments;
      List<TestCaseResult> testCaseResultList = TestCaseResult.QueryByRun(context, testRunId, pageSize, out excessIds, projectName, includeActionResults, out actionResults, out parameters, out attachments);
      ref List<LegacyTestCaseResultIdentifier> local1 = ref webApiExcessIds;
      IEnumerable<LegacyTestCaseResultIdentifier> source1 = TestCaseResultIdentifierConverter.Convert((IEnumerable<TestCaseResultIdentifier>) excessIds);
      List<LegacyTestCaseResultIdentifier> list1 = source1 != null ? source1.ToList<LegacyTestCaseResultIdentifier>() : (List<LegacyTestCaseResultIdentifier>) null;
      local1 = list1;
      ref List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> local2 = ref webApiActionResults;
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> source2 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult>) actionResults);
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> list2 = source2 != null ? source2.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult>() : (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult>) null;
      local2 = list2;
      ref List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> local3 = ref webApiParams;
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> source3 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter>) parameters);
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> list3 = source3 != null ? source3.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter>() : (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter>) null;
      local3 = list3;
      ref List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> local4 = ref webApiAttachments;
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> source4 = AttachmentContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>) attachments);
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> list4 = source4 != null ? source4.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>() : (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) null;
      local4 = list4;
      IEnumerable<LegacyTestCaseResult> source5 = TestCaseResultContractConverter.Convert((IEnumerable<TestCaseResult>) testCaseResultList);
      return source5 == null ? (List<LegacyTestCaseResult>) null : source5.ToList<LegacyTestCaseResult>();
    }

    public List<LegacyTestCaseResult> Fetch(
      TestManagementRequestContext context,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev> webApiIdAndRevs,
      string projectName,
      bool includeActionResults,
      out List<LegacyTestCaseResultIdentifier> webApiDeletedIds,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> webApiActionResults,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> webApiParams,
      out List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> webApiAttachments)
    {
      List<TestCaseResultIdentifier> resultIdentifierList = new List<TestCaseResultIdentifier>();
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev> source1 = TestCaseResultIdAndRevConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestCaseResultIdAndRev>) webApiIdAndRevs);
      Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev[] array = source1 != null ? source1.ToArray<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev>() : (Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdAndRev[]) null;
      List<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult> actionResults;
      List<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter> parameters;
      List<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment> attachments;
      List<TestCaseResult> testCaseResultList = TestCaseResult.Fetch(context, array, projectName, includeActionResults, resultIdentifierList, out actionResults, out parameters, out attachments);
      ref List<LegacyTestCaseResultIdentifier> local1 = ref webApiDeletedIds;
      IEnumerable<LegacyTestCaseResultIdentifier> source2 = TestCaseResultIdentifierConverter.Convert((IEnumerable<TestCaseResultIdentifier>) resultIdentifierList);
      List<LegacyTestCaseResultIdentifier> list1 = source2 != null ? source2.ToList<LegacyTestCaseResultIdentifier>() : (List<LegacyTestCaseResultIdentifier>) null;
      local1 = list1;
      ref List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> local2 = ref webApiActionResults;
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> source3 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestActionResult>) actionResults);
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> list2 = source3 != null ? source3.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult>() : (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult>) null;
      local2 = list2;
      ref List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> local3 = ref webApiParams;
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> source4 = TestActionResultUtils.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultParameter>) parameters);
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> list3 = source4 != null ? source4.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter>() : (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter>) null;
      local3 = list3;
      ref List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> local4 = ref webApiAttachments;
      IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> source5 = AttachmentContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>) attachments);
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> list4 = source5 != null ? source5.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>() : (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) null;
      local4 = list4;
      IEnumerable<LegacyTestCaseResult> source6 = TestCaseResultContractConverter.Convert((IEnumerable<TestCaseResult>) testCaseResultList);
      return source6 == null ? (List<LegacyTestCaseResult>) null : source6.ToList<LegacyTestCaseResult>();
    }

    public void DeleteAssociatedWorkItems(
      TestManagementRequestContext context,
      IEnumerable<LegacyTestCaseResultIdentifier> identifiers,
      string[] workItemUris)
    {
      TestManagementRequestContext context1 = context;
      IEnumerable<TestCaseResultIdentifier> source = TestCaseResultIdentifierConverter.Convert(identifiers);
      TestCaseResultIdentifier[] array = source != null ? source.ToArray<TestCaseResultIdentifier>() : (TestCaseResultIdentifier[]) null;
      string[] workItemUris1 = workItemUris;
      TestCaseResult.DeleteAssociatedWorkItems(context1, array, workItemUris1);
    }

    public void CreateAssociatedWorkItems(
      TestManagementRequestContext context,
      IEnumerable<LegacyTestCaseResultIdentifier> identifiers,
      string[] workItemUris)
    {
      TestManagementRequestContext context1 = context;
      IEnumerable<TestCaseResultIdentifier> source = TestCaseResultIdentifierConverter.Convert(identifiers);
      TestCaseResultIdentifier[] array = source != null ? source.ToArray<TestCaseResultIdentifier>() : (TestCaseResultIdentifier[]) null;
      string[] workItemUris1 = workItemUris;
      TestCaseResult.CreateAssociatedWorkItems(context1, array, workItemUris1);
    }

    public int QueryTestRunsCount(TestManagementRequestContext context, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query) => TestRun.QueryCount(context, ResultsStoreQueryContractConverter.Convert(query));

    public List<int> CreateLogEntriesForRun(
      TestManagementRequestContext context,
      string projectName,
      int testRunId,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry[] logEntries)
    {
      return TestRun.CreateLogEntriesForRun(context, testRunId, ((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry>) logEntries).Select<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry, Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry, Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry>) (logEntry => TestMessageLogEntryCoverter.Convert(logEntry))).ToArray<Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry>(), projectName);
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry> QueryLogEntriesForRun(
      TestManagementRequestContext context,
      string projectName,
      int testRunId,
      int testMessageLogId)
    {
      return TestRun.QueryLogEntriesForRun(context, testRunId, testMessageLogId, projectName).Select<Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestMessageLogEntry, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry>) (logEntry => TestMessageLogEntryCoverter.Convert(logEntry))).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestMessageLogEntry>();
    }

    public (Stream contentStream, string contentType, string fileName, long contentLength) DownloadAttachments(
      TestManagementRequestContext context,
      DownloadAttachmentsRequest request,
      out List<(int attachmentId, Guid projectId)> attachmentProjectMap)
    {
      return new AttachmentDownloadHelper().ProcessDownload(context, request.Ids?.ToArray(), request.Lengths?.ToArray(), out attachmentProjectMap);
    }

    private TestCaseResult[] GetConvertedResults(LegacyTestCaseResult[] results)
    {
      IEnumerable<TestCaseResult> source = TestCaseResultContractConverter.Convert((IEnumerable<LegacyTestCaseResult>) results);
      return source == null ? (TestCaseResult[]) null : source.ToArray<TestCaseResult>();
    }

    private TestRun GetConvertedTestRun(Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun testRun) => TestRunContractConverter.Convert(testRun);

    private TestSettings GetConvertedSettings(LegacyTestSettings testSettings) => TestSettingsContractConverter.Convert(testSettings);

    private Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun Convert(
      TestRun testRun)
    {
      return TestRunContractConverter.Convert(testRun);
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties Convert(
      Microsoft.TeamFoundation.TestManagement.Server.UpdatedProperties updatedProperties)
    {
      return UpdatedPropertiesConverter.Convert(updatedProperties);
    }

    private LegacyTestCaseResult Convert(TestCaseResult testCaseResult) => TestCaseResultContractConverter.Convert(testCaseResult);

    private List<LegacyTestCaseResult> Convert(List<TestCaseResult> testCaseResults)
    {
      IEnumerable<LegacyTestCaseResult> source = TestCaseResultContractConverter.Convert((IEnumerable<TestCaseResult>) testCaseResults);
      return source == null ? (List<LegacyTestCaseResult>) null : source.ToList<LegacyTestCaseResult>();
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties UpdateTestRun(
      TestManagementRequestContext context,
      string projectName,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun webApiTestRun,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment[] attachmentsToAdd,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity[] attachmentsToDelete,
      out int[] attachmentIds,
      bool shouldHyderate)
    {
      Microsoft.TeamFoundation.TestManagement.Server.UpdatedProperties updatedProperties = new Microsoft.TeamFoundation.TestManagement.Server.UpdatedProperties();
      TestRun testRun = TestRunContractConverter.Convert(webApiTestRun);
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment> source1 = AttachmentContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) attachmentsToAdd);
      Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment[] array1 = source1 != null ? source1.ToArray<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>() : (Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment[]) null;
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity> source2 = TestResultAttachmentIdentityConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity>) attachmentsToDelete);
      Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity[] array2 = source2 != null ? source2.ToArray<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity>() : (Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity[]) null;
      updatedProperties.Revision = -2;
      if (testRun != null)
      {
        testRun.ThrowInvalidOperationIfRunHasDtlEnvironment();
        updatedProperties = testRun.Update(context, projectName, shouldHyderate);
      }
      attachmentIds = Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment.Create(context, array1, projectName, false);
      Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment.Delete(context, array2, projectName);
      return UpdatedPropertiesConverter.Convert(updatedProperties);
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties UpdateTestRunForLogStoreAttachments(
      TestManagementRequestContext context,
      string projectName,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun webApiTestRun,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment[] attachmentsToAdd,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity[] attachmentsToDelete,
      out int[] attachmentIds,
      bool shouldHyderate)
    {
      Microsoft.TeamFoundation.TestManagement.Server.UpdatedProperties updatedProperties = new Microsoft.TeamFoundation.TestManagement.Server.UpdatedProperties();
      TestRun testRun = TestRunContractConverter.Convert(webApiTestRun);
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment> source1 = AttachmentContractConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) attachmentsToAdd);
      Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment[] array1 = source1 != null ? source1.ToArray<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>() : (Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment[]) null;
      IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity> source2 = TestResultAttachmentIdentityConverter.Convert((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachmentIdentity>) attachmentsToDelete);
      Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity[] array2 = source2 != null ? source2.ToArray<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity>() : (Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity[]) null;
      string str1 = string.Empty;
      string str2 = string.Empty;
      if (!((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>) array1).IsNullOrEmpty<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>())
      {
        str1 = string.Join<int>(", ", (IEnumerable<int>) ((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>) array1).Select<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment, int>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment, int>) (a => a.TestRunId)).ToList<int>());
        str2 = string.Join<int>(", ", (IEnumerable<int>) ((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>) array1).Select<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment, int>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment, int>) (a => a.TestResultId)).ToList<int>());
      }
      context.TraceVerbose("BusinessLayer", "UpdateTestRunForLogStoreAttachments invoked with runIds: {0}, resultIds: {1}, projectName: {2}", (object) str1, (object) str2, (object) projectName);
      updatedProperties.Revision = -2;
      if (testRun != null)
      {
        testRun.ThrowInvalidOperationIfRunHasDtlEnvironment();
        updatedProperties = testRun.Update(context, projectName, shouldHyderate);
      }
      attachmentIds = Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment.CreateAttachmentInLogStoreMapper(context, array1, projectName);
      List<string> enumerable = Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment.DeleteAttachmentsFromLogStoreMapper(context, array2, projectName);
      int length1 = ((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>) array1).IsNullOrEmpty<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachment>() ? 0 : array1.Length;
      int length2 = ((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity>) array2).IsNullOrEmpty<Microsoft.TeamFoundation.TestManagement.Server.TestResultAttachmentIdentity>() ? 0 : array2.Length;
      int length3 = ((IEnumerable<int>) attachmentIds).IsNullOrEmpty<int>() ? 0 : attachmentIds.Length;
      int count = enumerable.IsNullOrEmpty<string>() ? 0 : enumerable.Count;
      CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          "ProjectName",
          (object) projectName
        },
        {
          "TestRunIds",
          (object) str1
        },
        {
          "TestResultIds",
          (object) str2
        },
        {
          "AttachmentsToAdd",
          (object) length1.ToString()
        },
        {
          "AttachmentsToDelete",
          (object) length2.ToString()
        },
        {
          "AttachmentsAdded",
          (object) length3.ToString()
        },
        {
          "AttachmentsDeleted",
          (object) count.ToString()
        }
      });
      TelemetryLogger.Instance.PublishData(context.RequestContext, nameof (UpdateTestRunForLogStoreAttachments), cid);
      if (length1 == 0 && length2 == 0 || length1 != length3 || length2 != count)
        context.RequestContext.TraceAlways(1015115, TraceLevel.Error, "TestManagement", "BusinessLayer", "TestResultsService.UpdateTestRunForLogStoreAttachmentsTracePoint. Mismatch in actual and expected response. ProjectName = {0}, TestRunIds = {1}, TestResultIds = {2}, AttachmentsToAdd = {3}, AttachmentsToDelete = {4}, AttachmentsAdded = {5}, AttachmentsDeleted = {6}", (object) projectName, (object) str1, (object) str2, (object) length1, (object) length2, (object) length3, (object) count);
      return UpdatedPropertiesConverter.Convert(updatedProperties);
    }
  }
}
