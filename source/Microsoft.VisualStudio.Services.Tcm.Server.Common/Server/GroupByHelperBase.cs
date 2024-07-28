// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.GroupByHelperBase
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal abstract class GroupByHelperBase : IGroupByHelper
  {
    public Guid ProjectId;

    public virtual string GenerateSqlStatement(
      string groupByFieldStringWithOutcome,
      string propertiesToFetch,
      string filterClause,
      string orderBy)
    {
      return string.Empty;
    }

    public virtual void ReadAggregatedTestResults(
      SqlDataReader reader,
      Dictionary<string, TestResultsDetailsForGroup> aggregatedResultsMap,
      Dictionary<int, Tuple<int, int>> testPointDetails,
      Dictionary<int, Tuple<string, int, int>> testSuiteDetails,
      string groupByFieldString)
    {
      IQueryGroupedTestResultsColumns testResultsColumns = this.GetGroupedQueryResultsColumns();
      while (reader.Read())
        testResultsColumns.bindAggregateValues(reader, aggregatedResultsMap, groupByFieldString, testSuiteDetails);
    }

    public virtual void ReadResultDetails(
      ResultDetailsColumns resultDetails,
      Dictionary<object, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> resultsMap)
    {
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier key = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResultIdentifier(resultDetails.TestRunId, resultDetails.TestResultId);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        TestRun = new ShallowReference()
        {
          Id = resultDetails.TestRunId.ToString()
        },
        TestPoint = new ShallowReference()
        {
          Id = resultDetails.TestPointId.ToString()
        },
        Id = resultDetails.TestResultId,
        TestCaseReferenceId = resultDetails.TestCaseRefId,
        AutomatedTestStorage = resultDetails.AutomatedTestStorage,
        Priority = resultDetails.Priority,
        Project = new ShallowReference()
        {
          Id = this.ProjectId.ToString()
        },
        Owner = new IdentityRef()
        {
          DisplayName = resultDetails.Owner
        },
        Outcome = resultDetails.Outcome,
        DurationInMs = (double) resultDetails.DurationInMs
      };
      resultsMap[(object) key] = testCaseResult;
    }

    public virtual void PopulateAggregatedResults(
      Dictionary<object, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> resultsMap,
      Dictionary<string, TestResultsDetailsForGroup> aggregatedResultsMap,
      string groupByFieldString,
      Dictionary<int, Tuple<int, int>> testPointDetails)
    {
    }

    public IVssRequestContext RequestContext { get; set; }

    public Func<IQueryGroupedTestResultsColumns> GetGroupedQueryResultsColumns { get; set; }
  }
}
