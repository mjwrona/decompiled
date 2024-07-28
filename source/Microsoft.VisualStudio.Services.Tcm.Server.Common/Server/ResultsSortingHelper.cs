// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ResultsSortingHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class ResultsSortingHelper : IResultsSortingHelper
  {
    public TestResultsDetails OrderGroupsForGroupedResults(
      TestResultsDetails groupedResults,
      Dictionary<string, string> orderBy,
      List<string> groupByFields)
    {
      List<TestResultsDetailsForGroup> list = groupedResults.ResultsForGroup.ToList<TestResultsDetailsForGroup>();
      if (orderBy != null && orderBy.ContainsKey(TestResultsConstants.TestResultPropertyDuration))
        list.Sort((Comparison<TestResultsDetailsForGroup>) ((groupItem1, groupItem2) =>
        {
          double num1 = groupItem1.ResultsCountByOutcome.Sum<KeyValuePair<TestOutcome, AggregatedResultsByOutcome>>((Func<KeyValuePair<TestOutcome, AggregatedResultsByOutcome>, double>) (item => item.Value.Duration.TotalMilliseconds));
          double num2 = groupItem2.ResultsCountByOutcome.Sum<KeyValuePair<TestOutcome, AggregatedResultsByOutcome>>((Func<KeyValuePair<TestOutcome, AggregatedResultsByOutcome>, double>) (item => item.Value.Duration.TotalMilliseconds));
          return !orderBy[TestResultsConstants.TestResultPropertyDuration].Equals(ODataQueryConstants.OrderByAsc, StringComparison.OrdinalIgnoreCase) ? num2.CompareTo(num1) : num1.CompareTo(num2);
        }));
      else
        list.Sort((Comparison<TestResultsDetailsForGroup>) ((groupItem1, groupItem2) =>
        {
          if (groupByFields != null)
          {
            if (groupByFields.Any<string>((Func<string, bool>) (groupByField => groupByField.Equals(ValidTestResultGroupByFields.Container, StringComparison.OrdinalIgnoreCase))))
            {
              string groupByValue1 = groupItem1.GroupByValue as string;
              string groupByValue2 = groupItem2.GroupByValue as string;
              if (groupByValue1 != null && groupByValue2 != null)
                return this.CompareStrings(groupByValue1, groupByValue2);
            }
            else if (groupByFields.Any<string>((Func<string, bool>) (groupByField => groupByField.Equals(ValidTestResultGroupByFields.TestRun, StringComparison.OrdinalIgnoreCase))))
            {
              Microsoft.TeamFoundation.TestManagement.WebApi.TestRun groupByValue3 = groupItem1.GroupByValue as Microsoft.TeamFoundation.TestManagement.WebApi.TestRun;
              Microsoft.TeamFoundation.TestManagement.WebApi.TestRun groupByValue4 = groupItem2.GroupByValue as Microsoft.TeamFoundation.TestManagement.WebApi.TestRun;
              if (groupByValue3 != null && groupByValue4 != null)
                return this.CompareStrings(groupByValue3.Name, groupByValue4.Name);
            }
            else if (groupByFields.Any<string>((Func<string, bool>) (groupByField => groupByField.Equals(ValidTestResultGroupByFields.Priority, StringComparison.OrdinalIgnoreCase))))
            {
              string groupByValue5 = groupItem1.GroupByValue as string;
              string groupByValue6 = groupItem2.GroupByValue as string;
              if (groupByValue5 != null && groupByValue6 != null)
              {
                int result1 = 0;
                int result2 = 0;
                if (int.TryParse(groupByValue5, out result1) && int.TryParse(groupByValue6, out result2))
                  return result1.CompareTo(result2);
              }
            }
            else if (groupByFields.Any<string>((Func<string, bool>) (groupByField => groupByField.Equals(ValidTestResultGroupByFields.TestSuite, StringComparison.OrdinalIgnoreCase))))
            {
              TestSuite groupByValue7 = groupItem1.GroupByValue as TestSuite;
              TestSuite groupByValue8 = groupItem2.GroupByValue as TestSuite;
              if (groupByValue7 != null && groupByValue8 != null)
                return this.CompareStrings(groupByValue7.Name, groupByValue8.Name);
            }
            else if (groupByFields.Any<string>((Func<string, bool>) (groupByField => groupByField.Equals(ValidTestResultGroupByFields.Requirement, StringComparison.OrdinalIgnoreCase))))
            {
              WorkItemReference groupByValue9 = groupItem1.GroupByValue as WorkItemReference;
              WorkItemReference groupByValue10 = groupItem2.GroupByValue as WorkItemReference;
              if (groupByValue9 != null && groupByValue10 != null)
                return this.CompareStrings(groupByValue9.Name, groupByValue10.Name);
            }
            else if (groupByFields.Any<string>((Func<string, bool>) (groupByField => groupByField.Equals(ValidTestResultGroupByFields.Owner, StringComparison.OrdinalIgnoreCase))))
            {
              string groupByValue11 = groupItem1.GroupByValue as string;
              string groupByValue12 = groupItem2.GroupByValue as string;
              if (groupByValue11 != null && groupByValue12 != null)
                return this.CompareStrings(groupByValue11, groupByValue12);
            }
          }
          return 0;
        }));
      groupedResults.ResultsForGroup = (IList<TestResultsDetailsForGroup>) list;
      return groupedResults;
    }

    private int CompareStrings(string a, string b)
    {
      if (string.IsNullOrEmpty(a))
        return 1;
      return string.IsNullOrEmpty(b) ? -1 : a.CompareTo(b);
    }
  }
}
