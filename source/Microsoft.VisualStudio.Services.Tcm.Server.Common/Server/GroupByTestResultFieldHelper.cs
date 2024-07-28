// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.GroupByTestResultFieldHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class GroupByTestResultFieldHelper : GroupByHelperBase
  {
    public override string GenerateSqlStatement(
      string groupByFieldStringWithOutcome,
      string propertiesToFetch,
      string filterClause,
      string orderBy)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, TestManagementDynamicSqlBatchStatements.dynprc_QueryAggregatedResultsForBuildV2_UnifyingViews_V2, (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, TestManagementDynamicSqlBatchStatements.stmt_groupTestResultsBy, (object) groupByFieldStringWithOutcome), (object) propertiesToFetch, (object) filterClause, (object) orderBy);
    }

    public override void PopulateAggregatedResults(
      Dictionary<object, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> resultsMap,
      Dictionary<string, TestResultsDetailsForGroup> aggregatedResultsMap,
      string groupByFieldString,
      Dictionary<int, Tuple<int, int>> testPointDetails)
    {
      foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier key1 in resultsMap.Keys)
      {
        string key2 = string.Empty;
        if (!string.IsNullOrEmpty(groupByFieldString))
        {
          if (string.Equals(ValidTestResultGroupByFields.Container, groupByFieldString, StringComparison.OrdinalIgnoreCase))
            key2 = resultsMap[(object) key1].AutomatedTestStorage;
          else if (string.Equals(ValidTestResultGroupByFields.TestRun, groupByFieldString, StringComparison.OrdinalIgnoreCase))
            key2 = resultsMap[(object) key1].TestRun.Id;
          else if (string.Equals(ValidTestResultGroupByFields.Priority, groupByFieldString, StringComparison.OrdinalIgnoreCase))
            key2 = resultsMap[(object) key1].Priority.ToString();
          else if (string.Equals(ValidTestResultGroupByFields.Owner, groupByFieldString, StringComparison.OrdinalIgnoreCase))
            key2 = resultsMap[(object) key1].Owner.DisplayName;
        }
        resultsMap[(object) key1].AutomatedTestStorage = (string) null;
        resultsMap[(object) key1].Priority = 0;
        resultsMap[(object) key1].TestPoint = (ShallowReference) null;
        resultsMap[(object) key1].Owner = (IdentityRef) null;
        aggregatedResultsMap[key2].Results.Add(resultsMap[(object) key1]);
      }
    }
  }
}
