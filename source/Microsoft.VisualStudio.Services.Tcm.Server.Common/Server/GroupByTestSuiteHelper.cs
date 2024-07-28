// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.GroupByTestSuiteHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class GroupByTestSuiteHelper : GroupByHelperBase
  {
    public override string GenerateSqlStatement(
      string groupByFieldStringWithOutcome,
      string propertiesToFetch,
      string filterClause,
      string orderBy)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, TestManagementDynamicSqlBatchStatements.dynprc_QueryAggregatedResultsForBuildBySuite_UnifyingViews_V2, (object) groupByFieldStringWithOutcome, (object) propertiesToFetch, (object) filterClause, (object) orderBy);
    }

    public override void ReadAggregatedTestResults(
      SqlDataReader reader,
      Dictionary<string, TestResultsDetailsForGroup> aggregatedResultsMap,
      Dictionary<int, Tuple<int, int>> testPointDetails,
      Dictionary<int, Tuple<string, int, int>> testSuiteDetails,
      string groupByFieldString)
    {
      IQueryGroupedTestResultsColumns testResultsColumns1 = this.GetGroupedQueryResultsColumns();
      while (reader.Read())
        testResultsColumns1.bindPointDetails(reader, testPointDetails);
      if (reader.NextResult())
      {
        IQueryGroupedTestResultsColumns testResultsColumns2 = this.GetGroupedQueryResultsColumns();
        while (reader.Read())
          testResultsColumns2.bindSuiteDetails(reader, testSuiteDetails);
      }
      if (!reader.NextResult())
        return;
      IQueryGroupedTestResultsColumns testResultsColumns3 = this.GetGroupedQueryResultsColumns();
      while (reader.Read())
        testResultsColumns3.bindAggregateValues(reader, aggregatedResultsMap, groupByFieldString, testSuiteDetails);
    }

    public override void PopulateAggregatedResults(
      Dictionary<object, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> resultsMap,
      Dictionary<string, TestResultsDetailsForGroup> aggregatedResultsMap,
      string groupByFieldString,
      Dictionary<int, Tuple<int, int>> testPointDetails)
    {
      foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier key1 in resultsMap.Keys)
      {
        string empty = string.Empty;
        if (!string.IsNullOrEmpty(groupByFieldString))
        {
          int key2 = int.Parse(resultsMap[(object) key1].TestPoint.Id);
          if (key2 != 0 && testPointDetails.ContainsKey(key2))
            empty = testPointDetails[key2].Item2.ToString();
        }
        resultsMap[(object) key1].AutomatedTestStorage = (string) null;
        resultsMap[(object) key1].Priority = (int) byte.MaxValue;
        resultsMap[(object) key1].TestPoint = (ShallowReference) null;
        aggregatedResultsMap[empty].Results.Add(resultsMap[(object) key1]);
      }
    }
  }
}
