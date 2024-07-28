// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Compat2011QU1Helper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class Compat2011QU1Helper
  {
    internal static TestPlan Convert(TestManagementRequestContext context, TestPlan plan)
    {
      if (plan == null)
        return (TestPlan) null;
      DateTime minValue1 = DateTime.MinValue;
      DateTime minValue2 = DateTime.MinValue;
      if (plan.PlanId > 0)
      {
        IEnumerable<WorkItem> workItems = context.RequestContext.GetService<IWitHelper>().GetWorkItems(context.RequestContext, new List<int>()
        {
          plan.PlanId
        }, new List<string>()
        {
          TCMWitFields.StartDate,
          TCMWitFields.EndDate
        });
        WorkItem workItem = workItems != null && workItems.ToList<WorkItem>().Count == 1 ? workItems.First<WorkItem>() : throw new TestObjectNotFoundException(context.RequestContext, plan.PlanId, ObjectTypes.TestPlan);
        object obj1;
        workItem.Fields.TryGetValue(TCMWitFields.StartDate, out obj1);
        if (obj1 != null)
        {
          DateTime dateTime = (DateTime) obj1;
          if (DateTime.Equals(plan.StartDate.Date, dateTime.Date))
            plan.StartDate = dateTime;
        }
        object obj2;
        workItem.Fields.TryGetValue(TCMWitFields.EndDate, out obj2);
        if (obj2 != null)
        {
          DateTime dateTime = (DateTime) obj2;
          if (DateTime.Equals(plan.EndDate.Date, dateTime.Date))
            plan.EndDate = dateTime;
        }
      }
      return plan;
    }

    internal static List<TestPointStatistic> Convert(List<TestPointStatistic> list)
    {
      if (list == null)
        return (List<TestPointStatistic>) null;
      Compat2011QU1Helper.MergeNotApplicableWithPassedPointStatistic(list);
      foreach (TestPointStatistic testPointStatistic in list)
      {
        testPointStatistic.FailureType = TestFailureType.GetFailureTypeFromId((int) testPointStatistic.FailureType);
        testPointStatistic.ResultOutcome = TestResult.ToPreDev12QU2Outcome(testPointStatistic.ResultOutcome);
      }
      return list;
    }

    internal static void MergeNotApplicableWithPassedPointStatistic(List<TestPointStatistic> list)
    {
      if (list == null)
        return;
      TestPointStatistic testPointStatistic1 = list.FirstOrDefault<TestPointStatistic>((Func<TestPointStatistic, bool>) (item => item.ResultOutcome == (byte) 11));
      TestPointStatistic testPointStatistic2 = list.FirstOrDefault<TestPointStatistic>((Func<TestPointStatistic, bool>) (item => item.ResultOutcome == (byte) 2));
      if (testPointStatistic1 == null || testPointStatistic2 == null)
        return;
      testPointStatistic2.Count += testPointStatistic1.Count;
      list.Remove(testPointStatistic1);
    }

    internal static List<TestPointStatisticPivotItem> Convert(List<TestPointStatisticPivotItem> list)
    {
      if (list == null)
        return (List<TestPointStatisticPivotItem>) null;
      foreach (TestPointStatisticPivotItem statisticPivotItem in list)
        statisticPivotItem.Statistics = Compat2011QU1Helper.Convert(statisticPivotItem.Statistics);
      return list;
    }

    internal static List<TestPointStatisticsByPivotType> Convert(
      List<TestPointStatisticsByPivotType> list)
    {
      if (list == null)
        return (List<TestPointStatisticsByPivotType>) null;
      foreach (TestPointStatisticsByPivotType statisticsByPivotType in list)
        statisticsByPivotType.Statistics = Compat2011QU1Helper.Convert(statisticsByPivotType.Statistics);
      return list;
    }

    internal static List<TestPoint> Convert(List<TestPoint> list)
    {
      if (list == null)
        return (List<TestPoint>) null;
      foreach (TestPoint testPoint in list)
      {
        testPoint.FailureType = TestFailureType.GetFailureTypeFromId((int) testPoint.FailureType);
        testPoint.LastResultOutcome = TestResult.ToPreDev12QU2Outcome(testPoint.LastResultOutcome);
      }
      return list;
    }

    internal static List<TestCaseResult> Convert(List<TestCaseResult> list)
    {
      if (list == null)
        return (List<TestCaseResult>) null;
      foreach (TestCaseResult testCaseResult in list)
      {
        testCaseResult.FailureType = TestFailureType.GetFailureTypeFromId((int) testCaseResult.FailureType);
        testCaseResult.Outcome = TestResult.ToPreDev12QU2Outcome(testCaseResult.Outcome);
      }
      return list;
    }

    internal static TestCaseResult Convert(TestCaseResult result)
    {
      if (result == null)
        return (TestCaseResult) null;
      result.FailureType = TestFailureType.GetFailureTypeFromId((int) result.FailureType);
      result.Outcome = TestResult.ToPreDev12QU2Outcome(result.Outcome);
      return result;
    }

    internal static TestCaseResult[] Convert(TestCaseResult[] resultsArray) => resultsArray == null ? (TestCaseResult[]) null : Compat2011QU1Helper.Convert(((IEnumerable<TestCaseResult>) resultsArray).ToList<TestCaseResult>()).ToArray();

    internal static List<TestRunStatistic> Convert(List<TestRunStatistic> list)
    {
      if (list == null)
        return (List<TestRunStatistic>) null;
      Compat2011QU1Helper.MergeNotApplicableWithPassedRunStatistic(list);
      return list.Select<TestRunStatistic, TestRunStatistic>((Func<TestRunStatistic, TestRunStatistic>) (trs =>
      {
        trs.Outcome = TestResult.ToPreDev12QU2Outcome(trs.Outcome);
        return trs;
      })).ToList<TestRunStatistic>();
    }

    internal static void MergeNotApplicableWithPassedRunStatistic(List<TestRunStatistic> list)
    {
      if (list == null)
        return;
      TestRunStatistic testRunStatistic1 = list.FirstOrDefault<TestRunStatistic>((Func<TestRunStatistic, bool>) (item => item.Outcome == (byte) 11));
      TestRunStatistic testRunStatistic2 = list.FirstOrDefault<TestRunStatistic>((Func<TestRunStatistic, bool>) (item => item.Outcome == (byte) 2));
      if (testRunStatistic1 == null || testRunStatistic2 == null)
        return;
      testRunStatistic2.Count += testRunStatistic1.Count;
      list.Remove(testRunStatistic1);
    }

    internal static List<TestActionResult> Convert(List<TestActionResult> list) => list == null ? (List<TestActionResult>) null : list.Select<TestActionResult, TestActionResult>((Func<TestActionResult, TestActionResult>) (ar =>
    {
      ar.Outcome = TestResult.ToPreDev12QU2Outcome(ar.Outcome);
      return ar;
    })).ToList<TestActionResult>();

    internal static ResultUpdateRequest[] Convert(ResultUpdateRequest[] list) => list == null ? (ResultUpdateRequest[]) null : ((IEnumerable<ResultUpdateRequest>) list).Select<ResultUpdateRequest, ResultUpdateRequest>((Func<ResultUpdateRequest, ResultUpdateRequest>) (item => Compat2011QU1Helper.Convert(item))).ToArray<ResultUpdateRequest>();

    internal static ResultUpdateRequest Convert(ResultUpdateRequest updateRequest)
    {
      if (updateRequest == null || updateRequest.TestCaseResult == null || updateRequest.TestCaseResult.State != (byte) 4)
        return updateRequest;
      updateRequest.TestCaseResult.Outcome = TestResult.FromPreDev12QU2Outcome(updateRequest.TestCaseResult.Outcome);
      if (updateRequest.ActionResults != null)
        updateRequest.ActionResults = ((IEnumerable<TestActionResult>) updateRequest.ActionResults).Select<TestActionResult, TestActionResult>((Func<TestActionResult, TestActionResult>) (item =>
        {
          item.Outcome = TestResult.FromPreDev12QU2Outcome(item.Outcome);
          return item;
        })).ToArray<TestActionResult>();
      return updateRequest;
    }

    internal static List<TestRun> Convert(
      List<TestRun> list,
      TfsTestManagementRequestContext context)
    {
      return list == null ? (List<TestRun>) null : list.Select<TestRun, TestRun>((Func<TestRun, TestRun>) (item => Compat2011QU1Helper.Convert(item, context))).ToList<TestRun>();
    }

    internal static TestRun Convert(TestRun run, TfsTestManagementRequestContext context)
    {
      if (run == null)
        return (TestRun) null;
      run.PassedTests += run.NotApplicableTests;
      return run;
    }

    internal static void ValidateCompatibleResultOutcome(
      ResultUpdateRequest[] requests,
      string projectName,
      TfsTestManagementRequestContext context)
    {
      if (requests == null || requests.Length == 0)
        return;
      foreach (IGrouping<int, TestCaseResultIdentifier> grouping in ((IEnumerable<TestCaseResultIdentifier>) ((IEnumerable<ResultUpdateRequest>) requests).Where<ResultUpdateRequest>((Func<ResultUpdateRequest, bool>) (rur => rur.TestCaseResult != null && rur.TestCaseResult.Outcome == (byte) 2)).Select<ResultUpdateRequest, TestCaseResultIdentifier>((Func<ResultUpdateRequest, TestCaseResultIdentifier>) (rur => rur.TestCaseResult.Id)).ToArray<TestCaseResultIdentifier>()).GroupBy<TestCaseResultIdentifier, int>((Func<TestCaseResultIdentifier, int>) (tci => tci.TestRunId)))
      {
        IGrouping<int, TestCaseResultIdentifier> group = grouping;
        List<TestCaseResultIdentifier> excessIds = new List<TestCaseResultIdentifier>();
        List<TestCaseResultIdentifier> tcIdentifiers = new List<TestCaseResultIdentifier>();
        TestCaseResult.QueryByRunAndOutcomeInternal((TestManagementRequestContext) context, group.Key, (byte) 11, 0, false, out excessIds, projectName, out tcIdentifiers);
        if (tcIdentifiers.Where<TestCaseResultIdentifier>((Func<TestCaseResultIdentifier, bool>) (nar => group.Count<TestCaseResultIdentifier>((Func<TestCaseResultIdentifier, bool>) (tci => tci.TestResultId == nar.TestResultId)) > 0)).Count<TestCaseResultIdentifier>() > 0)
          throw new TestManagementInvalidOperationException(ServerResources.TestResultUpdateFailureCompatiblityIssue);
      }
      List<TestActionResult> testActionResultList = new List<TestActionResult>();
      foreach (ResultUpdateRequest request in requests)
      {
        if (request.ActionResults != null)
          testActionResultList.AddRange(((IEnumerable<TestActionResult>) request.ActionResults).Where<TestActionResult>((Func<TestActionResult, bool>) (ar => ar.Outcome == (byte) 2)));
      }
      foreach (TestActionResult testActionResult in testActionResultList)
      {
        TestActionResult ar = testActionResult;
        List<TestActionResult> actions = new List<TestActionResult>();
        List<TestResultParameter> parameters = new List<TestResultParameter>();
        List<TestResultAttachment> attachments = new List<TestResultAttachment>();
        TestActionResult.QueryById((TestManagementRequestContext) context, ar.Id, projectName, out actions, out parameters, out attachments);
        if (actions.Where<TestActionResult>((Func<TestActionResult, bool>) (tar => tar.Outcome == (byte) 11 && tar.IterationId == ar.IterationId)).Count<TestActionResult>() > 0)
          throw new TestManagementInvalidOperationException(ServerResources.TestResultUpdateFailureCompatiblityIssue);
      }
    }

    internal static string GetSourceWorkflowForTestRun(TestRun testRun)
    {
      if (!testRun.IsAutomated)
        return SourceWorkflow.Manual;
      return !string.IsNullOrEmpty(testRun.ReleaseUri) && !string.IsNullOrEmpty(testRun.ReleaseEnvironmentUri) ? SourceWorkflow.ContinuousDelivery : SourceWorkflow.ContinuousIntegration;
    }
  }
}
