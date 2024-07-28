// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPointUpdate
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class TestPointUpdate
  {
    internal static List<TestPoint> UpdatePointsWithLatestResults(
      TfsTestManagementRequestContext requestContext,
      string projectName,
      int planId,
      List<TestPoint> testPoints)
    {
      if (testPoints == null || !testPoints.Any<TestPoint>())
        return testPoints;
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName((TestManagementRequestContext) requestContext, projectName);
      testPoints = requestContext.PlannedTestingTCMServiceHelper.UpdatePointsWithLatestResults((TestManagementRequestContext) requestContext, projectFromName.GuidId, planId, testPoints);
      return testPoints;
    }

    internal static List<TestPointStatistic> GetTestPointStatistics(
      TfsTestManagementRequestContext requestContext,
      string projectName,
      int planId,
      List<TestPoint> testPoints)
    {
      if (testPoints == null || !testPoints.Any<TestPoint>())
        return new List<TestPointStatistic>();
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName((TestManagementRequestContext) requestContext, projectName);
      return TestPointUpdate.GroupPointsByStatistics(requestContext.PlannedTestingTCMServiceHelper.UpdatePointsWithLatestResults((TestManagementRequestContext) requestContext, projectFromName.GuidId, planId, testPoints));
    }

    internal static List<TestPointStatistic> GroupPointsByStatistics(List<TestPoint> testPoints)
    {
      if (testPoints == null || !testPoints.Any<TestPoint>())
        return new List<TestPointStatistic>();
      TestPointStatisticObjectComparer comparer = new TestPointStatisticObjectComparer();
      return testPoints.GroupBy<TestPoint, TestPointStatistic>((Func<TestPoint, TestPointStatistic>) (point => new TestPointStatistic()
      {
        TestPointState = point.State,
        FailureType = point.FailureType,
        ResolutionStateId = point.LastResolutionStateId,
        ResultOutcome = point.LastResultOutcome,
        ResultState = point.LastResultState
      }), (IEqualityComparer<TestPointStatistic>) comparer).Select<IGrouping<TestPointStatistic, TestPoint>, TestPointStatistic>((Func<IGrouping<TestPointStatistic, TestPoint>, TestPointStatistic>) (group => new TestPointStatistic()
      {
        TestPointState = group.Key.TestPointState,
        FailureType = group.Key.FailureType,
        ResolutionStateId = group.Key.ResolutionStateId,
        ResultOutcome = group.Key.ResultOutcome,
        ResultState = group.Key.ResultState,
        Count = group.Count<TestPoint>()
      })).ToList<TestPointStatistic>();
    }

    internal static List<TestPointStatisticsByPivotType> GetTestPointStatisticsByPivotType(
      TfsTestManagementRequestContext requestContext,
      string projectName,
      int planId,
      List<TestPoint> testPoints,
      List<TestPointStatisticsQueryPivotType> testPointStatsQueryPivotType,
      bool resolveUserName = false)
    {
      if (testPoints == null)
        return new List<TestPointStatisticsByPivotType>();
      List<TestPointStatisticsByPivotType> statisticsByPivotType = new List<TestPointStatisticsByPivotType>();
      foreach (TestPointStatisticsQueryPivotType statisticsQueryPivotType in testPointStatsQueryPivotType)
      {
        List<TestPointStatisticPivotItem> statisticPivotItemList = new List<TestPointStatisticPivotItem>();
        switch (statisticsQueryPivotType)
        {
          case TestPointStatisticsQueryPivotType.Suite:
            IEnumerable<IGrouping<int, TestPoint>> groupedPointsByPivot1 = testPoints.GroupBy<TestPoint, int>((Func<TestPoint, int>) (point => point.SuiteId));
            TestPointUpdate.GroupByPivotItem<int>(statisticPivotItemList, groupedPointsByPivot1);
            break;
          case TestPointStatisticsQueryPivotType.Tester:
            IEnumerable<IGrouping<Guid, TestPoint>> groupedPointsByPivot2 = testPoints.GroupBy<TestPoint, Guid>((Func<TestPoint, Guid>) (point => point.AssignedTo));
            TestPointUpdate.GroupByPivotItem<Guid>(statisticPivotItemList, groupedPointsByPivot2);
            if (resolveUserName)
            {
              TestPointStatisticPivotItem.ResolveUserName((TestManagementRequestContext) requestContext, statisticPivotItemList);
              break;
            }
            break;
        }
        statisticsByPivotType.Add(new TestPointStatisticsByPivotType()
        {
          PivotType = statisticsQueryPivotType,
          Statistics = statisticPivotItemList
        });
      }
      return statisticsByPivotType;
    }

    private static void GroupByPivotItem<T>(
      List<TestPointStatisticPivotItem> stats,
      IEnumerable<IGrouping<T, TestPoint>> groupedPointsByPivot)
    {
      stats.AddRange(groupedPointsByPivot.Select<IGrouping<T, TestPoint>, TestPointStatisticPivotItem>((Func<IGrouping<T, TestPoint>, TestPointStatisticPivotItem>) (group => new TestPointStatisticPivotItem()
      {
        Pivot = group.Key.ToString(),
        Statistics = TestPointUpdate.GroupPointsByStatistics(group.Select<TestPoint, TestPoint>((Func<TestPoint, TestPoint>) (point => point)).ToList<TestPoint>())
      })));
    }
  }
}
