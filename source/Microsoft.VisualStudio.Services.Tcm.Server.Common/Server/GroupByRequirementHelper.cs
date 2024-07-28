// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.GroupByRequirementHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class GroupByRequirementHelper : GroupByHelperBase
  {
    public override string GenerateSqlStatement(
      string groupByFieldStringWithOutcome,
      string propertiesToFetch,
      string filterClause,
      string orderBy)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, TestManagementDynamicSqlBatchStatements.dynprc_QueryAggregatedResultsForBuildByRequirement_UnifyingViews_V2, (object) groupByFieldStringWithOutcome, (object) propertiesToFetch, (object) filterClause, (object) orderBy);
    }

    public override void ReadResultDetails(
      ResultDetailsColumns resultDetails,
      Dictionary<object, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> resultsMap)
    {
      TestResultRequirementAssociation key = new TestResultRequirementAssociation()
      {
        WorkItemId = resultDetails.WorkItemId,
        TestRunId = resultDetails.TestRunId,
        TestResultId = resultDetails.TestResultId
      };
      resultsMap[(object) key] = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        TestRun = new ShallowReference()
        {
          Id = resultDetails.TestRunId.ToString()
        },
        Id = resultDetails.TestResultId,
        Project = new ShallowReference()
        {
          Id = this.ProjectId.ToString()
        },
        AutomatedTestStorage = resultDetails.AutomatedTestStorage,
        Priority = resultDetails.Priority,
        TestCaseReferenceId = resultDetails.TestCaseRefId
      };
    }

    public override void PopulateAggregatedResults(
      Dictionary<object, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> resultsMap,
      Dictionary<string, TestResultsDetailsForGroup> aggregatedResultsMap,
      string groupByFieldString,
      Dictionary<int, Tuple<int, int>> testPointDetails)
    {
      foreach (TestResultRequirementAssociation key1 in resultsMap.Keys)
      {
        string key2 = key1.WorkItemId > 0 ? key1.WorkItemId.ToString() : string.Empty;
        resultsMap[(object) key1].AutomatedTestStorage = (string) null;
        resultsMap[(object) key1].Priority = (int) byte.MaxValue;
        resultsMap[(object) key1].TestPoint = (ShallowReference) null;
        aggregatedResultsMap[key2].Results.Add(resultsMap[(object) key1]);
      }
    }
  }
}
