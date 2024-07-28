// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.MergeDataExtensions
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class MergeDataExtensions
  {
    public static void Merge(this TestResultHistory history1, TestResultHistory history2)
    {
      if (history2 == null || history2.ResultsForGroup == null)
        return;
      if (history1.ResultsForGroup != null && history1.ResultsForGroup.Count > 0)
      {
        Dictionary<object, TestResultHistoryDetailsForGroup> groupByMap1 = history1.ResultsForGroup.ToDictionary<TestResultHistoryDetailsForGroup, object>((Func<TestResultHistoryDetailsForGroup, object>) (x => x.GroupByValue));
        history2.ResultsForGroup.ForEach<TestResultHistoryDetailsForGroup>((Action<TestResultHistoryDetailsForGroup>) (r =>
        {
          if (groupByMap1.ContainsKey(r.GroupByValue))
            return;
          history1.ResultsForGroup.Add(r);
        }));
      }
      else
        history1.ResultsForGroup = history2.ResultsForGroup;
    }

    public static void Merge(this TestHistoryQuery history1, TestHistoryQuery history2)
    {
      if (history1.ResultsForGroup != null)
      {
        Dictionary<string, TestResultHistoryForGroup> groupByMap1 = history1.ResultsForGroup.ToDictionary<TestResultHistoryForGroup, string>((Func<TestResultHistoryForGroup, string>) (x => x.GroupByValue));
        IList<TestResultHistoryForGroup> resultsForGroup = history2.ResultsForGroup;
        if (resultsForGroup != null)
          resultsForGroup.ForEach<TestResultHistoryForGroup>((Action<TestResultHistoryForGroup>) (r =>
          {
            TestResultHistoryForGroup resultHistoryForGroup;
            if (groupByMap1.TryGetValue(r.GroupByValue, out resultHistoryForGroup))
            {
              if (resultHistoryForGroup.Results != null)
                resultHistoryForGroup.Results.AddRange<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) r.Results);
              else
                resultHistoryForGroup.Results = r.Results;
            }
            else
              history1.ResultsForGroup.Add(r);
          }));
      }
      else
        history1.ResultsForGroup = history2.ResultsForGroup;
      history1.ContinuationToken = history2.ContinuationToken;
    }

    public static void Merge(
      this List<ShallowTestCaseResult> references1,
      List<ShallowTestCaseResult> references2)
    {
      if (references2 == null)
        return;
      references1.AddRange((IEnumerable<ShallowTestCaseResult>) references2);
      references1.Sort((Comparison<ShallowTestCaseResult>) ((ref1, ref2) => ref1.RunId != ref2.RunId ? ref1.RunId - ref2.RunId : ref1.Id - ref2.Id));
    }

    public static void Merge(
      this List<WorkItemReference> references1,
      List<WorkItemReference> references2)
    {
      if (references2 == null)
        return;
      Dictionary<string, WorkItemReference> references1Map = new Dictionary<string, WorkItemReference>();
      references1.ForEach((Action<WorkItemReference>) (reference => references1Map[reference.Id] = reference));
      references2.ForEach((Action<WorkItemReference>) (reference2 =>
      {
        if (references1Map.ContainsKey(reference2.Id))
          return;
        references1Map.Add(reference2.Id, reference2);
      }));
      references1.Clear();
      references1.AddRange((IEnumerable<WorkItemReference>) references1Map.Values);
    }

    public static void Merge(
      this TestResultsGroupsForRelease resultsGroupsForRelease1,
      TestResultsGroupsForRelease testResultsGroupsForRelase2)
    {
      if (testResultsGroupsForRelase2 == null)
        return;
      if (resultsGroupsForRelease1.Fields != null)
        resultsGroupsForRelease1.Fields.Merge(testResultsGroupsForRelase2.Fields);
      else
        resultsGroupsForRelease1.Fields = testResultsGroupsForRelase2.Fields;
    }

    public static void Merge(
      this TestResultsGroupsForBuild resultsGroupsForBuild1,
      TestResultsGroupsForBuild resultsGroupsForBuild2)
    {
      if (resultsGroupsForBuild2 == null)
        return;
      if (resultsGroupsForBuild1.Fields != null)
        resultsGroupsForBuild1.Fields.Merge(resultsGroupsForBuild2.Fields);
      else
        resultsGroupsForBuild1.Fields = resultsGroupsForBuild2.Fields;
    }

    public static void Merge(
      this IList<FieldDetailsForTestResults> fields1,
      IList<FieldDetailsForTestResults> fields2)
    {
      if (fields2 == null)
        return;
      Dictionary<string, FieldDetailsForTestResults> mergedFieldDetails = fields1.GetMergedFieldDetails(fields2);
      fields1.Clear();
      Action<KeyValuePair<string, FieldDetailsForTestResults>> action = (Action<KeyValuePair<string, FieldDetailsForTestResults>>) (pair => fields1.Add(new FieldDetailsForTestResults()
      {
        FieldName = pair.Key,
        GroupsForField = pair.Value.GroupsForField
      }));
      mergedFieldDetails.ForEach<KeyValuePair<string, FieldDetailsForTestResults>>(action);
    }

    public static void Merge(
      this IPagedList<FieldDetailsForTestResults> testResultsGroups1,
      IPagedList<FieldDetailsForTestResults> testResultsGroups2)
    {
      if (testResultsGroups2 == null)
        return;
      Dictionary<string, FieldDetailsForTestResults> mergedFieldDetails = testResultsGroups1.GetMergedFieldDetails((IList<FieldDetailsForTestResults>) testResultsGroups2);
      testResultsGroups1.Clear();
      Action<KeyValuePair<string, FieldDetailsForTestResults>> action = (Action<KeyValuePair<string, FieldDetailsForTestResults>>) (pair => testResultsGroups1.Add(new FieldDetailsForTestResults()
      {
        FieldName = pair.Key,
        GroupsForField = pair.Value.GroupsForField
      }));
      mergedFieldDetails.ForEach<KeyValuePair<string, FieldDetailsForTestResults>>(action);
    }

    public static void Merge(
      this TestResultSummary resultSummary1,
      TestResultSummary resultSummary2)
    {
      if (resultSummary2 == null)
        return;
      resultSummary1.TotalRunsCount += resultSummary2.TotalRunsCount;
      resultSummary1.NoConfigRunsCount += resultSummary2.NoConfigRunsCount;
      if (resultSummary1.AggregatedResultsAnalysis != null)
        resultSummary1.AggregatedResultsAnalysis.Merge(resultSummary2.AggregatedResultsAnalysis);
      else
        resultSummary1.AggregatedResultsAnalysis = resultSummary2.AggregatedResultsAnalysis;
      if (resultSummary1.TestFailures != null)
        resultSummary1.TestFailures.Merge(resultSummary2.TestFailures);
      else
        resultSummary1.TestFailures = resultSummary2.TestFailures;
    }

    public static void Merge(this TestSummaryForWorkItem summary1, TestSummaryForWorkItem summary2)
    {
      if (summary2 == null || summary2.Summary == null)
        return;
      if (summary1.Summary == null)
        summary1.Summary = summary2.Summary;
      else
        summary1.Summary.Merge(summary2.Summary);
    }

    public static void Merge(
      this AggregatedDataForResultTrend aggregatedDataForResultTrend1,
      AggregatedDataForResultTrend aggregatedDataForResultTrend2)
    {
      if (aggregatedDataForResultTrend2 == null)
        return;
      if (aggregatedDataForResultTrend1.ResultsByOutcome != null)
        aggregatedDataForResultTrend1.ResultsByOutcome.Merge(aggregatedDataForResultTrend2.ResultsByOutcome);
      else
        aggregatedDataForResultTrend1.ResultsByOutcome = aggregatedDataForResultTrend2.ResultsByOutcome;
      aggregatedDataForResultTrend1.TotalTests += aggregatedDataForResultTrend2.TotalTests;
      aggregatedDataForResultTrend1.Duration += aggregatedDataForResultTrend2.Duration;
      if (aggregatedDataForResultTrend1.RunSummaryByState != null)
        aggregatedDataForResultTrend1.RunSummaryByState.Merge(aggregatedDataForResultTrend2.RunSummaryByState);
      else
        aggregatedDataForResultTrend1.RunSummaryByState = aggregatedDataForResultTrend2.RunSummaryByState;
    }

    public static void Merge(
      this IDictionary<TestRunState, AggregatedRunsByState> runSummaryByState1,
      IDictionary<TestRunState, AggregatedRunsByState> runSummaryByState2)
    {
      if (runSummaryByState2 == null)
        return;
      foreach (TestRunState key in Enum.GetValues(typeof (TestRunState)))
      {
        AggregatedRunsByState aggregatedRunsByState1;
        if (runSummaryByState2.TryGetValue(key, out aggregatedRunsByState1))
        {
          AggregatedRunsByState aggregatedRunsByState2;
          if (runSummaryByState1.TryGetValue(key, out aggregatedRunsByState2))
            aggregatedRunsByState2.RunsCount += aggregatedRunsByState1.RunsCount;
          else
            runSummaryByState1.Add(key, aggregatedRunsByState1);
        }
      }
    }

    public static void Merge(
      this IDictionary<TestRunOutcome, AggregatedRunsByOutcome> runSummaryByOutcome1,
      IDictionary<TestRunOutcome, AggregatedRunsByOutcome> runSummaryByOutcome2)
    {
      if (runSummaryByOutcome2 == null)
        return;
      foreach (TestRunOutcome key in Enum.GetValues(typeof (TestRunOutcome)))
      {
        AggregatedRunsByOutcome aggregatedRunsByOutcome1;
        if (runSummaryByOutcome2.TryGetValue(key, out aggregatedRunsByOutcome1))
        {
          AggregatedRunsByOutcome aggregatedRunsByOutcome2;
          if (runSummaryByOutcome1.TryGetValue(key, out aggregatedRunsByOutcome2))
            aggregatedRunsByOutcome2.RunsCount += aggregatedRunsByOutcome1.RunsCount;
          else
            runSummaryByOutcome1.Add(key, aggregatedRunsByOutcome1);
        }
      }
    }

    public static void Merge(
      this AggregatedResultsAnalysis aggregatedResultsAnalysis1,
      AggregatedResultsAnalysis aggregatedResultsAnalysis2)
    {
      if (aggregatedResultsAnalysis2 == null)
        return;
      if (aggregatedResultsAnalysis1.ResultsByOutcome != null)
        aggregatedResultsAnalysis1.ResultsByOutcome.Merge(aggregatedResultsAnalysis2.ResultsByOutcome);
      else
        aggregatedResultsAnalysis1.ResultsByOutcome = aggregatedResultsAnalysis2.ResultsByOutcome;
      if (aggregatedResultsAnalysis1.NotReportedResultsByOutcome != null)
        aggregatedResultsAnalysis1.NotReportedResultsByOutcome.Merge(aggregatedResultsAnalysis2.NotReportedResultsByOutcome);
      else
        aggregatedResultsAnalysis1.NotReportedResultsByOutcome = aggregatedResultsAnalysis2.NotReportedResultsByOutcome;
      aggregatedResultsAnalysis1.TotalTests += aggregatedResultsAnalysis2.TotalTests;
      aggregatedResultsAnalysis1.Duration += aggregatedResultsAnalysis2.Duration;
      if (aggregatedResultsAnalysis1.ResultsDifference != null)
        aggregatedResultsAnalysis1.ResultsDifference.Merge(aggregatedResultsAnalysis2.ResultsDifference);
      else
        aggregatedResultsAnalysis1.ResultsDifference = aggregatedResultsAnalysis2.ResultsDifference;
      if (aggregatedResultsAnalysis1.RunSummaryByState != null)
        aggregatedResultsAnalysis1.RunSummaryByState.Merge(aggregatedResultsAnalysis2.RunSummaryByState);
      else
        aggregatedResultsAnalysis1.RunSummaryByState = aggregatedResultsAnalysis2.RunSummaryByState;
      if (aggregatedResultsAnalysis1.RunSummaryByOutcome != null)
        aggregatedResultsAnalysis1.RunSummaryByOutcome.Merge(aggregatedResultsAnalysis2.RunSummaryByOutcome);
      else
        aggregatedResultsAnalysis1.RunSummaryByOutcome = aggregatedResultsAnalysis2.RunSummaryByOutcome;
    }

    public static void Merge(
      this IDictionary<TestOutcome, AggregatedResultsByOutcome> resultsByOutcome1,
      IDictionary<TestOutcome, AggregatedResultsByOutcome> resultsByOutcome2)
    {
      if (resultsByOutcome2 == null)
        return;
      foreach (TestOutcome key in Enum.GetValues(typeof (TestOutcome)).Cast<TestOutcome>().Distinct<TestOutcome>().ToArray<TestOutcome>())
      {
        AggregatedResultsByOutcome aggregatedResultsByOutcome2;
        if (resultsByOutcome2.TryGetValue(key, out aggregatedResultsByOutcome2))
        {
          AggregatedResultsByOutcome aggregatedResultsByOutcome1;
          if (resultsByOutcome1.TryGetValue(key, out aggregatedResultsByOutcome1))
            aggregatedResultsByOutcome1.Merge(aggregatedResultsByOutcome2);
          else
            resultsByOutcome1.Add(key, aggregatedResultsByOutcome2);
        }
      }
    }

    public static void Merge(
      this AggregatedResultsDifference aggregatedResultsDifference1,
      AggregatedResultsDifference aggregatedResultsDifference2)
    {
      if (aggregatedResultsDifference2 == null)
        return;
      aggregatedResultsDifference1.IncreaseInDuration += aggregatedResultsDifference2.IncreaseInDuration;
      aggregatedResultsDifference1.IncreaseInFailures += aggregatedResultsDifference2.IncreaseInFailures;
      aggregatedResultsDifference1.IncreaseInOtherTests += aggregatedResultsDifference2.IncreaseInOtherTests;
      aggregatedResultsDifference1.IncreaseInPassedTests += aggregatedResultsDifference2.IncreaseInPassedTests;
      aggregatedResultsDifference1.IncreaseInTotalTests += aggregatedResultsDifference2.IncreaseInTotalTests;
    }

    public static void Merge(
      this AggregatedResultsByOutcome aggregatedResultsByOutcome1,
      AggregatedResultsByOutcome aggregatedResultsByOutcome2)
    {
      if (aggregatedResultsByOutcome2 == null)
        return;
      aggregatedResultsByOutcome1.Count += aggregatedResultsByOutcome2.Count;
      aggregatedResultsByOutcome1.Duration += aggregatedResultsByOutcome2.Duration;
      aggregatedResultsByOutcome1.RerunResultCount += aggregatedResultsByOutcome2.RerunResultCount;
    }

    public static void Merge(
      this TestFailuresAnalysis testFailuresAnalysis1,
      TestFailuresAnalysis testFailuresAnalysis2)
    {
      if (testFailuresAnalysis2 == null)
        return;
      if (testFailuresAnalysis1.NewFailures != null)
        testFailuresAnalysis1.NewFailures.Merge(testFailuresAnalysis2.NewFailures);
      else
        testFailuresAnalysis1.NewFailures = testFailuresAnalysis2.NewFailures;
      if (testFailuresAnalysis1.ExistingFailures != null)
        testFailuresAnalysis1.ExistingFailures.Merge(testFailuresAnalysis2.ExistingFailures);
      else
        testFailuresAnalysis1.ExistingFailures = testFailuresAnalysis2.ExistingFailures;
      if (testFailuresAnalysis1.FixedTests != null)
        testFailuresAnalysis1.FixedTests.Merge(testFailuresAnalysis2.FixedTests);
      else
        testFailuresAnalysis1.FixedTests = testFailuresAnalysis2.FixedTests;
    }

    public static void Merge(
      this TestFailureDetails testFailureDetails1,
      TestFailureDetails testFailureDetails2)
    {
      if (testFailureDetails2 == null)
        return;
      HashSet<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier> resultIds = new HashSet<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>) testFailureDetails1.TestResults);
      if (testFailureDetails2 != null)
      {
        IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier> testResults = testFailureDetails2.TestResults;
        if (testResults != null)
          testResults.ForEach<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>((Action<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>) (r => resultIds.Add(r)));
      }
      testFailureDetails1.TestResults = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>) resultIds.Select<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>) (r => r)).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier>();
      testFailureDetails1.Count += testFailureDetails2.Count;
    }

    public static void Merge(
      this TestResultsDetails resultDetails1,
      TestResultsDetails resultDetails2)
    {
      if (resultDetails2 == null || resultDetails2.ResultsForGroup == null)
        return;
      if (resultDetails1.ResultsForGroup == null)
      {
        resultDetails1.ResultsForGroup = resultDetails2.ResultsForGroup;
      }
      else
      {
        Dictionary<object, TestResultsDetailsForGroup> resultsByGroup = new Dictionary<object, TestResultsDetailsForGroup>();
        resultDetails2.ResultsForGroup.ForEach<TestResultsDetailsForGroup>((Action<TestResultsDetailsForGroup>) (rg => resultsByGroup[rg.GroupByValue] = rg));
        foreach (TestResultsDetailsForGroup resultsDetailsForGroup in (IEnumerable<TestResultsDetailsForGroup>) resultDetails1.ResultsForGroup)
        {
          if (resultsByGroup.ContainsKey(resultsDetailsForGroup.GroupByValue))
          {
            resultsDetailsForGroup.Results.AddRange<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) resultsByGroup[resultsDetailsForGroup.GroupByValue].Results);
            foreach (KeyValuePair<TestOutcome, AggregatedResultsByOutcome> keyValuePair in (IEnumerable<KeyValuePair<TestOutcome, AggregatedResultsByOutcome>>) resultsDetailsForGroup.ResultsCountByOutcome)
            {
              if (resultsByGroup[resultsDetailsForGroup.GroupByValue].ResultsCountByOutcome.ContainsKey(keyValuePair.Key))
                keyValuePair.Value.Merge(resultsByGroup[resultsDetailsForGroup.GroupByValue].ResultsCountByOutcome[keyValuePair.Key]);
            }
          }
        }
        Dictionary<object, TestResultsDetailsForGroup> resultsByGroup1 = new Dictionary<object, TestResultsDetailsForGroup>();
        resultDetails1.ResultsForGroup.ForEach<TestResultsDetailsForGroup>((Action<TestResultsDetailsForGroup>) (rg => resultsByGroup1[rg.GroupByValue] = rg));
        foreach (TestResultsDetailsForGroup resultsDetailsForGroup in (IEnumerable<TestResultsDetailsForGroup>) resultDetails2.ResultsForGroup)
        {
          if (!resultsByGroup1.ContainsKey(resultsDetailsForGroup.GroupByValue))
            resultDetails1.ResultsForGroup.Add(resultsDetailsForGroup);
        }
      }
    }

    private static Dictionary<string, FieldDetailsForTestResults> GetMergedFieldDetails(
      this IList<FieldDetailsForTestResults> fields1,
      IList<FieldDetailsForTestResults> fields2)
    {
      List<string> stringList = new List<string>();
      Dictionary<string, FieldDetailsForTestResults> mergedFieldDetails = new Dictionary<string, FieldDetailsForTestResults>();
      foreach (FieldDetailsForTestResults detailsForTestResults in (IEnumerable<FieldDetailsForTestResults>) fields1)
        mergedFieldDetails[detailsForTestResults.FieldName] = detailsForTestResults;
      foreach (FieldDetailsForTestResults detailsForTestResults in (IEnumerable<FieldDetailsForTestResults>) fields2)
      {
        if (mergedFieldDetails.ContainsKey(detailsForTestResults.FieldName))
        {
          List<object> list = mergedFieldDetails[detailsForTestResults.FieldName].GroupsForField.ToList<object>();
          list.AddRange((IEnumerable<object>) detailsForTestResults.GroupsForField.ToList<object>());
          mergedFieldDetails[detailsForTestResults.FieldName].GroupsForField = (IList<object>) list;
        }
        else
          mergedFieldDetails[detailsForTestResults.FieldName] = detailsForTestResults;
      }
      return mergedFieldDetails;
    }
  }
}
