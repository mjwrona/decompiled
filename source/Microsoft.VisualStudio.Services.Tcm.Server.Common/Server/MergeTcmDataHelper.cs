// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.MergeTcmDataHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class MergeTcmDataHelper : IMergeDataHelper
  {
    public virtual List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> MergeTestRuns(
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> runs1,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> runs2)
    {
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun> testRunList = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>();
      if (runs1 != null)
        testRunList.AddRange((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) runs1);
      if (runs2 != null)
        testRunList.AddRange((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestRun>) runs2);
      return testRunList;
    }

    public List<TestResultSummary> MergeTestResultSummaryLists(
      List<TestResultSummary> summaryList1,
      List<TestResultSummary> summaryList2)
    {
      if (summaryList1 == null)
        return summaryList2;
      if (summaryList2 == null)
        return summaryList1;
      List<TestResultSummary> testResultSummaryList = new List<TestResultSummary>();
      Dictionary<string, TestResultSummary> dictionary1 = summaryList1.ToDictionary<TestResultSummary, string>((Func<TestResultSummary, string>) (t => t.TestResultsContext.Release.Id.ToString() + "," + (object) t.TestResultsContext.Release.EnvironmentId));
      Dictionary<string, TestResultSummary> dictionary2 = summaryList2.ToDictionary<TestResultSummary, string>((Func<TestResultSummary, string>) (t => t.TestResultsContext.Release.Id.ToString() + "," + (object) t.TestResultsContext.Release.EnvironmentId));
      foreach (KeyValuePair<string, TestResultSummary> keyValuePair in dictionary1)
      {
        if (dictionary2.ContainsKey(keyValuePair.Key))
          dictionary1[keyValuePair.Key].Merge(dictionary2[keyValuePair.Key]);
      }
      List<TestResultSummary> list = dictionary1.Values.ToList<TestResultSummary>();
      foreach (KeyValuePair<string, TestResultSummary> keyValuePair in dictionary2)
      {
        if (!dictionary1.ContainsKey(keyValuePair.Key))
          list.Add(dictionary2[keyValuePair.Key]);
      }
      return list;
    }

    public List<TestSummaryForWorkItem> MergeTestSummaryForWorkItemLists(
      List<TestSummaryForWorkItem> summaryList1,
      List<TestSummaryForWorkItem> summaryList2)
    {
      if (summaryList1 == null)
        return summaryList2;
      if (summaryList2 == null)
        return summaryList1;
      List<TestSummaryForWorkItem> summaryForWorkItemList = new List<TestSummaryForWorkItem>();
      Dictionary<string, TestSummaryForWorkItem> dictionary1 = summaryList1.ToDictionary<TestSummaryForWorkItem, string>((Func<TestSummaryForWorkItem, string>) (t => t.WorkItem.Id));
      Dictionary<string, TestSummaryForWorkItem> dictionary2 = summaryList2.ToDictionary<TestSummaryForWorkItem, string>((Func<TestSummaryForWorkItem, string>) (t => t.WorkItem.Id));
      foreach (KeyValuePair<string, TestSummaryForWorkItem> keyValuePair in dictionary1)
      {
        if (dictionary2.ContainsKey(keyValuePair.Key))
          dictionary1[keyValuePair.Key].Merge(dictionary2[keyValuePair.Key]);
      }
      List<TestSummaryForWorkItem> list = dictionary1.Values.ToList<TestSummaryForWorkItem>();
      foreach (KeyValuePair<string, TestSummaryForWorkItem> keyValuePair in dictionary2)
      {
        if (!dictionary1.ContainsKey(keyValuePair.Key))
          list.Add(dictionary2[keyValuePair.Key]);
      }
      return list;
    }

    public TestResultSummary MergeTestResultSummary(
      TestResultSummary summary1,
      TestResultSummary summary2)
    {
      TestResultSummary testResultSummary;
      if (summary1 != null)
      {
        summary1.Merge(summary2);
        testResultSummary = summary1;
      }
      else
        testResultSummary = summary2;
      return testResultSummary;
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> MergeTestResults(
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> results1,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> results2)
    {
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultList = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      if (results1 != null)
        testCaseResultList.AddRange((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) results1);
      if (results2 != null)
        testCaseResultList.AddRange((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) results2);
      return testCaseResultList;
    }

    public List<TestResultMetaData> MergeTestResultsMetaData(
      List<TestResultMetaData> mataData1,
      List<TestResultMetaData> mataData2)
    {
      List<TestResultMetaData> testResultMetaDataList = new List<TestResultMetaData>();
      if (mataData1 != null)
        testResultMetaDataList.AddRange((IEnumerable<TestResultMetaData>) mataData1);
      if (mataData2 != null)
        testResultMetaDataList.AddRange((IEnumerable<TestResultMetaData>) mataData2);
      return testResultMetaDataList;
    }

    public TestResultsDetails MergeTestResultDetails(
      TestResultsDetails resultDetails1,
      TestResultsDetails resultDetails2)
    {
      TestResultsDetails testResultsDetails;
      if (resultDetails1 != null)
      {
        resultDetails1.Merge(resultDetails2);
        testResultsDetails = resultDetails1;
      }
      else
        testResultsDetails = resultDetails2;
      return testResultsDetails;
    }

    public TestResultsGroupsForBuild MergeTestResultsGroupsForBuild(
      TestResultsGroupsForBuild testResultsGroupsForBuild1,
      TestResultsGroupsForBuild testResultsGroupsForBuild2)
    {
      TestResultsGroupsForBuild resultsGroupsForBuild;
      if (testResultsGroupsForBuild1 != null)
      {
        testResultsGroupsForBuild1.Merge(testResultsGroupsForBuild2);
        resultsGroupsForBuild = testResultsGroupsForBuild1;
      }
      else
        resultsGroupsForBuild = testResultsGroupsForBuild2;
      return resultsGroupsForBuild;
    }

    public TestResultsGroupsForRelease MergeTestResultsGroupsForRelease(
      TestResultsGroupsForRelease testResultsGroupsForRelease1,
      TestResultsGroupsForRelease testResultsGroupsForRelease2)
    {
      TestResultsGroupsForRelease groupsForRelease;
      if (testResultsGroupsForRelease1 != null)
      {
        testResultsGroupsForRelease1.Merge(testResultsGroupsForRelease2);
        groupsForRelease = testResultsGroupsForRelease1;
      }
      else
        groupsForRelease = testResultsGroupsForRelease2;
      return groupsForRelease;
    }

    public IPagedList<FieldDetailsForTestResults> MergeTestResultsGroups(
      IPagedList<FieldDetailsForTestResults> testResultsGroups1,
      IPagedList<FieldDetailsForTestResults> testResultsGroups2)
    {
      IPagedList<FieldDetailsForTestResults> pagedList;
      if (testResultsGroups1 != null)
      {
        string continuationToken = this.MergeContinuationTokens(testResultsGroups1.ContinuationToken, testResultsGroups2.ContinuationToken);
        MergeDataExtensions.Merge(testResultsGroups1, testResultsGroups2);
        pagedList = (IPagedList<FieldDetailsForTestResults>) new PagedList<FieldDetailsForTestResults>((IEnumerable<FieldDetailsForTestResults>) testResultsGroups1, continuationToken);
      }
      else
        pagedList = testResultsGroups2;
      return pagedList;
    }

    public string MergeContinuationTokens(string ct1, string ct2)
    {
      if (string.IsNullOrEmpty(ct1))
        return ct2;
      if (string.IsNullOrEmpty(ct2))
        return ct1;
      int continuationTokenRunId1;
      int continuationTokenResultId1;
      ParsingHelper.ParseContinuationTokenResultId(ct1, out continuationTokenRunId1, out continuationTokenResultId1);
      int continuationTokenRunId2;
      int continuationTokenResultId2;
      ParsingHelper.ParseContinuationTokenResultId(ct2, out continuationTokenRunId2, out continuationTokenResultId2);
      return Utils.GenerateTestResultsContinuationToken(Math.Min(continuationTokenRunId1, continuationTokenRunId2), Math.Min(continuationTokenResultId1, continuationTokenResultId2));
    }

    public List<AggregatedDataForResultTrend> MergeTestResultTrend(
      List<AggregatedDataForResultTrend> resultTrend1,
      List<AggregatedDataForResultTrend> resultTrend2,
      TestResultsContextType contextType)
    {
      if (resultTrend1 == null)
        return resultTrend2;
      if (resultTrend2 == null)
        return resultTrend1;
      if (contextType == TestResultsContextType.Build)
        return this.MergeTestResultTrendForBuild(resultTrend1, resultTrend2);
      return contextType == TestResultsContextType.Release ? this.MergeTestResultTrendForRelease(resultTrend1, resultTrend2) : new List<AggregatedDataForResultTrend>();
    }

    public List<WorkItemReference> MergeWorkItemReferences(
      List<WorkItemReference> references1,
      List<WorkItemReference> references2)
    {
      List<WorkItemReference> references1_1 = new List<WorkItemReference>();
      if (references1 == null)
        references1_1 = references2;
      else if (references2 == null)
      {
        references1_1 = references1;
      }
      else
      {
        references1_1.AddRange((IEnumerable<WorkItemReference>) references1);
        references1_1.Merge(references2);
      }
      return references1_1;
    }

    public List<ShallowTestCaseResult> MergeTestResultReferences(
      List<ShallowTestCaseResult> references1,
      List<ShallowTestCaseResult> references2,
      int top)
    {
      List<ShallowTestCaseResult> shallowTestCaseResultList = new List<ShallowTestCaseResult>();
      if (references1 == null)
        shallowTestCaseResultList = references2;
      else if (references2 == null)
      {
        shallowTestCaseResultList = references1;
      }
      else
      {
        shallowTestCaseResultList.AddRange((IEnumerable<ShallowTestCaseResult>) references1);
        shallowTestCaseResultList.Merge(references2);
      }
      return shallowTestCaseResultList.Take<ShallowTestCaseResult>(top).ToList<ShallowTestCaseResult>();
    }

    public TestResultHistory MergeTestResultHistory(
      TestResultHistory history1,
      TestResultHistory history2)
    {
      if (history1 == null)
        return history2;
      history1.Merge(history2);
      return history1;
    }

    public TestHistoryQuery MergeTestHistory(TestHistoryQuery history1, TestHistoryQuery history2)
    {
      if (history1 == null)
        return history2;
      if (history2 == null)
        return history1;
      history1.Merge(history2);
      return history1;
    }

    public TestToWorkItemLinks MergeTestToWorkItemLinks(
      TestToWorkItemLinks links1,
      TestToWorkItemLinks links2)
    {
      if (links1 == null || links1.Test == null || links1.WorkItems == null)
        return links2;
      if (links2 == null || links2.Test == null || links2.WorkItems == null)
        return links1;
      Dictionary<string, WorkItemReference> workItemIdDict = links1.WorkItems.ToDictionary<WorkItemReference, string>((Func<WorkItemReference, string>) (t => t.Id));
      links2.WorkItems.ForEach((Action<WorkItemReference>) (wit =>
      {
        if (workItemIdDict.ContainsKey(wit.Id))
          return;
        links1.WorkItems.Add(wit);
      }));
      return links1;
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary MergeCodeCoverageSummary(
      Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary summary1,
      Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary summary2)
    {
      if (summary1 == null)
        return summary2;
      if (summary2 == null)
        return summary1;
      List<CodeCoverageData> codeCoverageDataList = new List<CodeCoverageData>();
      if (summary1.CoverageData != null && summary1.CoverageData.Count > 0)
        codeCoverageDataList.AddRange((IEnumerable<CodeCoverageData>) summary1.CoverageData);
      if (summary2.CoverageData != null && summary2.CoverageData.Count > 0)
        codeCoverageDataList.AddRange((IEnumerable<CodeCoverageData>) summary2.CoverageData);
      summary2.CoverageData = (IList<CodeCoverageData>) codeCoverageDataList;
      summary2.CoverageDetailedSummaryStatus = summary2.CoverageData != null ? CoverageDetailedSummaryStatus.CodeCoverageSuccess : CoverageDetailedSummaryStatus.NoModulesFound;
      return summary2;
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage> MergeBuildCoverages(
      List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage> coverages1,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage> coverages2)
    {
      List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage> buildCoverageList = new List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>();
      if (coverages1 != null)
        buildCoverageList.AddRange((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>) coverages1);
      if (coverages2 != null)
        buildCoverageList.AddRange((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>) coverages2);
      return buildCoverageList;
    }

    public List<AfnStrip> MergeDefaultAfnStrips(
      List<AfnStrip> afnStrips1,
      List<AfnStrip> afnStrips2)
    {
      if (afnStrips1 == null && afnStrips2 == null)
        return new List<AfnStrip>();
      if (afnStrips1 == null)
        return afnStrips2;
      if (afnStrips2 == null)
        return afnStrips1;
      List<AfnStrip> afnStripList = new List<AfnStrip>();
      IEnumerable<int> ints = afnStrips1.Concat<AfnStrip>((IEnumerable<AfnStrip>) afnStrips2).Select<AfnStrip, int>((Func<AfnStrip, int>) (afnStrip => afnStrip.TestCaseId)).Distinct<int>();
      Dictionary<int, AfnStrip> dictionary1 = afnStrips1.ToDictionary<AfnStrip, int, AfnStrip>((Func<AfnStrip, int>) (afnStrip => afnStrip.TestCaseId), (Func<AfnStrip, AfnStrip>) (afnStrip => afnStrip));
      Dictionary<int, AfnStrip> dictionary2 = afnStrips2.ToDictionary<AfnStrip, int, AfnStrip>((Func<AfnStrip, int>) (afnStrip => afnStrip.TestCaseId), (Func<AfnStrip, AfnStrip>) (afnStrip => afnStrip));
      foreach (int key in ints)
      {
        bool flag1 = dictionary1.ContainsKey(key);
        bool flag2 = dictionary2.ContainsKey(key);
        if (flag1 & flag2)
          afnStripList.Add(MergeTcmDataHelper.GetMostRecentAfnStrip(dictionary1[key], dictionary2[key]));
        else if (flag1)
          afnStripList.Add(dictionary1[key]);
        else
          afnStripList.Add(dictionary2[key]);
      }
      return afnStripList;
    }

    public Dictionary<int, bool> MergeExistenceMapping(
      Dictionary<int, bool> mapping1,
      Dictionary<int, bool> mapping2)
    {
      if (mapping1 == null && mapping2 == null)
        return new Dictionary<int, bool>();
      if (mapping1 == null)
        return mapping2;
      if (mapping2 == null)
        return mapping1;
      IEnumerable<int> ints = mapping1.Keys.Union<int>((IEnumerable<int>) mapping2.Keys);
      Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
      foreach (int key in ints)
      {
        bool flag1;
        mapping1.TryGetValue(key, out flag1);
        bool flag2;
        mapping2.TryGetValue(key, out flag2);
        dictionary.Add(key, flag1 | flag2);
      }
      return dictionary;
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> MergeAttachments(
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> attachments1,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> attachments2,
      bool union = false)
    {
      if (attachments1 == null && attachments2 == null)
        return new List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>();
      if (attachments1 == null)
        return attachments2;
      if (attachments2 == null)
        return attachments1;
      if (!union)
        return attachments1.Concat<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) attachments2).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>();
      AttachmentComparer comparer = new AttachmentComparer();
      return attachments1.Union<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) attachments2, (IEqualityComparer<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) comparer).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>();
    }

    private static AfnStrip GetMostRecentAfnStrip(AfnStrip afnStrip1, AfnStrip afnStrip2) => !(afnStrip1.CreationDate > afnStrip2.CreationDate) ? afnStrip2 : afnStrip1;

    private List<AggregatedDataForResultTrend> MergeTestResultTrendForBuild(
      List<AggregatedDataForResultTrend> resultTrend1,
      List<AggregatedDataForResultTrend> resultTrend2)
    {
      Dictionary<int, AggregatedDataForResultTrend> dictionary1 = resultTrend1.ToDictionary<AggregatedDataForResultTrend, int>((Func<AggregatedDataForResultTrend, int>) (t => t.TestResultsContext.Build.Id));
      Dictionary<int, AggregatedDataForResultTrend> dictionary2 = resultTrend2.ToDictionary<AggregatedDataForResultTrend, int>((Func<AggregatedDataForResultTrend, int>) (t => t.TestResultsContext.Build.Id));
      foreach (KeyValuePair<int, AggregatedDataForResultTrend> keyValuePair in dictionary1)
      {
        if (dictionary2.ContainsKey(keyValuePair.Key))
          dictionary1[keyValuePair.Key].Merge(dictionary2[keyValuePair.Key]);
      }
      List<AggregatedDataForResultTrend> list = dictionary1.Values.ToList<AggregatedDataForResultTrend>();
      foreach (KeyValuePair<int, AggregatedDataForResultTrend> keyValuePair in dictionary2)
      {
        if (!dictionary1.ContainsKey(keyValuePair.Key))
          list.Add(dictionary2[keyValuePair.Key]);
      }
      list.Sort((Comparison<AggregatedDataForResultTrend>) ((i, j) => j.TestResultsContext.Build.Id - i.TestResultsContext.Build.Id));
      return list;
    }

    private List<AggregatedDataForResultTrend> MergeTestResultTrendForRelease(
      List<AggregatedDataForResultTrend> resultTrend1,
      List<AggregatedDataForResultTrend> resultTrend2)
    {
      Dictionary<string, AggregatedDataForResultTrend> dictionary1 = resultTrend1.ToDictionary<AggregatedDataForResultTrend, string>((Func<AggregatedDataForResultTrend, string>) (t => t.TestResultsContext.Release.Id.ToString() + "," + (object) t.TestResultsContext.Release.EnvironmentId));
      Dictionary<string, AggregatedDataForResultTrend> dictionary2 = resultTrend2.ToDictionary<AggregatedDataForResultTrend, string>((Func<AggregatedDataForResultTrend, string>) (t => t.TestResultsContext.Release.Id.ToString() + "," + (object) t.TestResultsContext.Release.EnvironmentId));
      foreach (KeyValuePair<string, AggregatedDataForResultTrend> keyValuePair in dictionary1)
      {
        if (dictionary2.ContainsKey(keyValuePair.Key))
          dictionary1[keyValuePair.Key].Merge(dictionary2[keyValuePair.Key]);
      }
      List<AggregatedDataForResultTrend> list = dictionary1.Values.ToList<AggregatedDataForResultTrend>();
      foreach (KeyValuePair<string, AggregatedDataForResultTrend> keyValuePair in dictionary2)
      {
        if (!dictionary1.ContainsKey(keyValuePair.Key))
          list.Add(dictionary2[keyValuePair.Key]);
      }
      list.Sort((Comparison<AggregatedDataForResultTrend>) ((i, j) => j.TestResultsContext.Release.Id - i.TestResultsContext.Release.Id));
      return list;
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[] MergeUpdateResponseLegacy(
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest[] requests,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest[] requestsForRemote,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[] responseFromRemote,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest[] requestsForLocal,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[] responseFromLocal)
    {
      if (requests == null)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[]) null;
      if (requestsForLocal == null)
        return responseFromRemote;
      if (requestsForRemote == null)
        return responseFromLocal;
      Dictionary<TestCaseResultIdentifier, int> dictionary = ((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest>) requests).Select((req, index) => new
      {
        req = req,
        index = index
      }).ToDictionary(reqAndIndex => new TestCaseResultIdentifier(reqAndIndex.req.TestRunId, reqAndIndex.req.TestResultId), reqAndIndex => reqAndIndex.index);
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[] resultUpdateResponseArray = new Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateResponse[requests.Length];
      for (int index1 = 0; index1 < ((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest>) requestsForRemote).Count<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest>(); ++index1)
      {
        TestCaseResultIdentifier key = new TestCaseResultIdentifier(requestsForRemote[index1].TestRunId, requestsForRemote[index1].TestResultId);
        int index2 = dictionary[key];
        resultUpdateResponseArray[index2] = responseFromRemote[index1];
      }
      for (int index3 = 0; index3 < ((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest>) requestsForLocal).Count<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultUpdateRequest>(); ++index3)
      {
        TestCaseResultIdentifier key = new TestCaseResultIdentifier(requestsForLocal[index3].TestRunId, requestsForLocal[index3].TestResultId);
        int index4 = dictionary[key];
        resultUpdateResponseArray[index4] = responseFromLocal[index3];
      }
      return resultUpdateResponseArray;
    }

    public ResultsByQueryResponse MergeResultsByQueryResponse(
      ResultsByQueryResponse responseFromRemote,
      ResultsByQueryResponse responseFromLocal,
      int pageSize)
    {
      int? nullable1 = responseFromRemote?.TestResults?.Count;
      int valueOrDefault1 = nullable1.GetValueOrDefault();
      int? nullable2;
      if (responseFromLocal == null)
      {
        nullable1 = new int?();
        nullable2 = nullable1;
      }
      else
      {
        List<LegacyTestCaseResult> testResults = responseFromLocal.TestResults;
        if (testResults == null)
        {
          nullable1 = new int?();
          nullable2 = nullable1;
        }
        else
        {
          // ISSUE: explicit non-virtual call
          nullable2 = new int?(__nonvirtual (testResults.Count));
        }
      }
      nullable1 = nullable2;
      int valueOrDefault2 = nullable1.GetValueOrDefault();
      if (pageSize == 0)
      {
        int? nullable3;
        if (responseFromLocal == null)
        {
          nullable1 = new int?();
          nullable3 = nullable1;
        }
        else
        {
          List<LegacyTestCaseResultIdentifier> excessIds = responseFromLocal.ExcessIds;
          if (excessIds == null)
          {
            nullable1 = new int?();
            nullable3 = nullable1;
          }
          else
          {
            // ISSUE: explicit non-virtual call
            nullable3 = new int?(__nonvirtual (excessIds.Count));
          }
        }
        nullable1 = nullable3;
        int valueOrDefault3 = nullable1.GetValueOrDefault();
        int? nullable4;
        if (responseFromRemote == null)
        {
          nullable1 = new int?();
          nullable4 = nullable1;
        }
        else
        {
          List<LegacyTestCaseResultIdentifier> excessIds = responseFromRemote.ExcessIds;
          if (excessIds == null)
          {
            nullable1 = new int?();
            nullable4 = nullable1;
          }
          else
          {
            // ISSUE: explicit non-virtual call
            nullable4 = new int?(__nonvirtual (excessIds.Count));
          }
        }
        nullable1 = nullable4;
        int valueOrDefault4 = nullable1.GetValueOrDefault();
        if (valueOrDefault3 == 0)
          return responseFromRemote;
        if (valueOrDefault4 == 0)
          return responseFromLocal;
        return new ResultsByQueryResponse()
        {
          TestResults = responseFromRemote.TestResults.Concat<LegacyTestCaseResult>(responseFromLocal.TestResults.Take<LegacyTestCaseResult>(pageSize - valueOrDefault1)).ToList<LegacyTestCaseResult>(),
          ExcessIds = responseFromRemote.ExcessIds.Concat<LegacyTestCaseResultIdentifier>((IEnumerable<LegacyTestCaseResultIdentifier>) responseFromLocal.ExcessIds).ToList<LegacyTestCaseResultIdentifier>()
        };
      }
      if (valueOrDefault1 == 0)
        return responseFromLocal;
      if (valueOrDefault2 == 0 || valueOrDefault1 == pageSize)
        return responseFromRemote;
      IEnumerable<LegacyTestCaseResult> source = responseFromLocal.TestResults.Skip<LegacyTestCaseResult>(pageSize - valueOrDefault1);
      return new ResultsByQueryResponse()
      {
        TestResults = responseFromRemote.TestResults.Concat<LegacyTestCaseResult>(responseFromLocal.TestResults.Take<LegacyTestCaseResult>(pageSize - valueOrDefault1)).ToList<LegacyTestCaseResult>(),
        ExcessIds = source.Select<LegacyTestCaseResult, LegacyTestCaseResultIdentifier>((Func<LegacyTestCaseResult, LegacyTestCaseResultIdentifier>) (result => new LegacyTestCaseResultIdentifier()
        {
          TestRunId = result.TestRunId,
          TestResultId = result.TestResultId
        })).Concat<LegacyTestCaseResultIdentifier>((IEnumerable<LegacyTestCaseResultIdentifier>) responseFromRemote.ExcessIds).Concat<LegacyTestCaseResultIdentifier>((IEnumerable<LegacyTestCaseResultIdentifier>) responseFromLocal.ExcessIds).ToList<LegacyTestCaseResultIdentifier>()
      };
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> MergeTestRunsLegacy(
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> runsFromRemote,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun> localRuns)
    {
      // ISSUE: explicit non-virtual call
      int count1 = runsFromRemote != null ? __nonvirtual (runsFromRemote.Count) : 0;
      // ISSUE: explicit non-virtual call
      int count2 = localRuns != null ? __nonvirtual (localRuns.Count) : 0;
      if (count1 == 0)
        return localRuns;
      return count2 == 0 ? runsFromRemote : runsFromRemote.Concat<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>) localRuns).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.LegacyTestRun>();
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.FetchTestResultsResponse FetchTestResultsResponse(
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.FetchTestResultsResponse responseFromRemote,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.FetchTestResultsResponse responseFromLocal)
    {
      if (responseFromRemote == null)
        return responseFromLocal;
      if (responseFromLocal == null)
        return responseFromRemote;
      return new Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.FetchTestResultsResponse()
      {
        Results = this.MergeLegacyResults(responseFromRemote.Results, responseFromLocal.Results),
        ActionResults = this.MergeActionResults(responseFromRemote.ActionResults, responseFromLocal.ActionResults),
        TestParameters = this.MergeParameters(responseFromRemote.TestParameters, responseFromLocal.TestParameters),
        Attachments = this.MergeAttachments(responseFromRemote.Attachments, responseFromLocal.Attachments, false),
        DeletedIds = this.MergeTestCaseResultIdentifiers(responseFromRemote.DeletedIds, responseFromLocal.DeletedIds)
      };
    }

    public LegacyTestCaseResult[] MergeLegacyTestResults(
      LegacyTestCaseResultIdentifier[] identifiers,
      LegacyTestCaseResultIdentifier[] requestsForRemote,
      LegacyTestCaseResult[] responseFromRemote,
      LegacyTestCaseResultIdentifier[] requestsForLocal,
      LegacyTestCaseResult[] responseFromLocal)
    {
      if (identifiers == null)
        return (LegacyTestCaseResult[]) null;
      if (requestsForRemote == null)
        return responseFromLocal;
      if (requestsForLocal == null)
        return responseFromRemote;
      Dictionary<TestCaseResultIdentifier, int> dictionary = ((IEnumerable<LegacyTestCaseResultIdentifier>) identifiers).Select((identifier, index) => new
      {
        identifier = identifier,
        index = index
      }).ToDictionary(identifierAndIndex => new TestCaseResultIdentifier(identifierAndIndex.identifier.TestRunId, identifierAndIndex.identifier.TestResultId), identifierAndIndex => identifierAndIndex.index);
      LegacyTestCaseResult[] legacyTestCaseResultArray = new LegacyTestCaseResult[identifiers.Length];
      for (int index1 = 0; index1 < ((IEnumerable<LegacyTestCaseResultIdentifier>) requestsForRemote).Count<LegacyTestCaseResultIdentifier>(); ++index1)
      {
        TestCaseResultIdentifier key = new TestCaseResultIdentifier(requestsForRemote[index1].TestRunId, requestsForRemote[index1].TestResultId);
        int index2 = dictionary[key];
        legacyTestCaseResultArray[index2] = responseFromRemote[index1];
      }
      for (int index3 = 0; index3 < ((IEnumerable<LegacyTestCaseResultIdentifier>) requestsForLocal).Count<LegacyTestCaseResultIdentifier>(); ++index3)
      {
        TestCaseResultIdentifier key = new TestCaseResultIdentifier(requestsForLocal[index3].TestRunId, requestsForLocal[index3].TestResultId);
        int index4 = dictionary[key];
        legacyTestCaseResultArray[index4] = responseFromLocal[index3];
      }
      return legacyTestCaseResultArray;
    }

    public List<LegacyTestCaseResult> MergeLegacyTestResults(
      List<LegacyTestCaseResult> resultsFromRemote,
      List<LegacyTestCaseResult> resultsFromLocal)
    {
      if (resultsFromLocal == null)
        return resultsFromRemote;
      return resultsFromRemote == null ? resultsFromLocal : resultsFromRemote.Concat<LegacyTestCaseResult>((IEnumerable<LegacyTestCaseResult>) resultsFromLocal).ToList<LegacyTestCaseResult>();
    }

    private List<LegacyTestCaseResult> MergeLegacyResults(
      List<LegacyTestCaseResult> results1,
      List<LegacyTestCaseResult> results2)
    {
      if (results1 == null)
        return results2;
      return results2 == null ? results1 : results1.Concat<LegacyTestCaseResult>((IEnumerable<LegacyTestCaseResult>) results2).ToList<LegacyTestCaseResult>();
    }

    private List<LegacyTestCaseResultIdentifier> MergeTestCaseResultIdentifiers(
      List<LegacyTestCaseResultIdentifier> deletedIds1,
      List<LegacyTestCaseResultIdentifier> deletedIds2)
    {
      if (deletedIds1 == null)
        return deletedIds2;
      return deletedIds2 == null ? deletedIds1 : deletedIds1.Concat<LegacyTestCaseResultIdentifier>((IEnumerable<LegacyTestCaseResultIdentifier>) deletedIds2).ToList<LegacyTestCaseResultIdentifier>();
    }

    private List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> MergeParameters(
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> testParameters1,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter> testParameters2)
    {
      if (testParameters1 == null)
        return testParameters2;
      return testParameters2 == null ? testParameters1 : testParameters1.Concat<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter>) testParameters2).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultParameter>();
    }

    private List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> MergeActionResults(
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> actionResults1,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult> actionResults2)
    {
      if (actionResults1 == null)
        return actionResults2;
      return actionResults2 == null ? actionResults1 : actionResults1.Concat<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult>) actionResults2).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestActionResult>();
    }
  }
}
