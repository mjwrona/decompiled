// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.IMergeDataHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public interface IMergeDataHelper
  {
    List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> MergeTestRuns(
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> runs1,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> runs2);

    List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> MergeTestResults(
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> results1,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> results2);

    List<TestResultMetaData> MergeTestResultsMetaData(
      List<TestResultMetaData> mataData1,
      List<TestResultMetaData> mataData2);

    TestResultSummary MergeTestResultSummary(TestResultSummary summary1, TestResultSummary summary2);

    List<TestResultSummary> MergeTestResultSummaryLists(
      List<TestResultSummary> summaryList1,
      List<TestResultSummary> summaryList2);

    TestResultsDetails MergeTestResultDetails(
      TestResultsDetails resultDetails1,
      TestResultsDetails resultDetails2);

    List<AggregatedDataForResultTrend> MergeTestResultTrend(
      List<AggregatedDataForResultTrend> resultTrend1,
      List<AggregatedDataForResultTrend> resultTrend2,
      TestResultsContextType contextType);

    TestResultsGroupsForBuild MergeTestResultsGroupsForBuild(
      TestResultsGroupsForBuild testResultsGroupsForBuild1,
      TestResultsGroupsForBuild testResultsGroupsForBuild2);

    TestResultsGroupsForRelease MergeTestResultsGroupsForRelease(
      TestResultsGroupsForRelease testResultsGroupsForRelease1,
      TestResultsGroupsForRelease testResultsGroupsForRelease2);

    IPagedList<FieldDetailsForTestResults> MergeTestResultsGroups(
      IPagedList<FieldDetailsForTestResults> testResultsGroups1,
      IPagedList<FieldDetailsForTestResults> testResultsGroups2);

    List<WorkItemReference> MergeWorkItemReferences(
      List<WorkItemReference> references1,
      List<WorkItemReference> references2);

    List<ShallowTestCaseResult> MergeTestResultReferences(
      List<ShallowTestCaseResult> references1,
      List<ShallowTestCaseResult> references2,
      int top);

    List<TestSummaryForWorkItem> MergeTestSummaryForWorkItemLists(
      List<TestSummaryForWorkItem> summaryList1,
      List<TestSummaryForWorkItem> summaryList2);

    TestResultHistory MergeTestResultHistory(TestResultHistory history1, TestResultHistory history2);

    TestHistoryQuery MergeTestHistory(TestHistoryQuery history1, TestHistoryQuery history2);

    TestToWorkItemLinks MergeTestToWorkItemLinks(
      TestToWorkItemLinks links1,
      TestToWorkItemLinks links2);

    Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary MergeCodeCoverageSummary(
      Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary summary1,
      Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary summary2);

    List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage> MergeBuildCoverages(
      List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage> coverages1,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage> coverages2);

    List<AfnStrip> MergeDefaultAfnStrips(List<AfnStrip> afnStrips1, List<AfnStrip> afnStrips2);

    Dictionary<int, bool> MergeExistenceMapping(
      Dictionary<int, bool> mapping1,
      Dictionary<int, bool> mapping2);

    List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> MergeAttachments(
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> attachments1,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> attachments2,
      bool union = false);

    Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[] MergeUpdateResponseLegacy(
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest[] requests,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest[] requestsForRemote,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[] responseFromRemote,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest[] requestsForLocal,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[] responseFromLocal);

    ResultsByQueryResponse MergeResultsByQueryResponse(
      ResultsByQueryResponse response,
      ResultsByQueryResponse resultsByQueryResponse,
      int pageSize);

    List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> MergeTestRunsLegacy(
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> runsFromRemote,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> localRuns);

    Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.FetchTestResultsResponse FetchTestResultsResponse(
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.FetchTestResultsResponse responseFromRemote,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.FetchTestResultsResponse fetchTestResultsResponse);

    LegacyTestCaseResult[] MergeLegacyTestResults(
      LegacyTestCaseResultIdentifier[] identifiers,
      LegacyTestCaseResultIdentifier[] requestsForRemote,
      LegacyTestCaseResult[] responseFromRemote,
      LegacyTestCaseResultIdentifier[] requestsForLocal,
      LegacyTestCaseResult[] responseFromLocal);

    List<LegacyTestCaseResult> MergeLegacyTestResults(
      List<LegacyTestCaseResult> resultsFromRemote,
      List<LegacyTestCaseResult> resultsFromLocal);
  }
}
