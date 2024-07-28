// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase48
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase48 : TestManagementDatabase47
  {
    internal TestManagementDatabase48(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase48()
    {
    }

    internal override List<TestVariable> QueryTestVariables(
      Guid projectId,
      int skip,
      int top,
      int watermark)
    {
      Dictionary<int, TestVariable> dictionary = new Dictionary<int, TestVariable>();
      this.PrepareStoredProcedure("prc_QueryVariablesByProject");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@skip", skip);
      this.BindInt("@top", top);
      this.BindInt("@watermark", watermark);
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.QueryTestVariablesColumns variablesColumns = new TestManagementDatabase.QueryTestVariablesColumns();
      while (reader.Read())
      {
        TestVariable testVariable = variablesColumns.bind(reader);
        dictionary[testVariable.Id] = testVariable;
      }
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryVariablesByProject");
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("VariableId");
      SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("Value");
      while (reader.Read())
      {
        int int32 = sqlColumnBinder1.GetInt32((IDataReader) reader);
        string str = sqlColumnBinder2.GetString((IDataReader) reader, false);
        TestVariable testVariable;
        if (dictionary.TryGetValue(int32, out testVariable))
          testVariable.Values.Add(str);
      }
      List<TestVariable> testVariableList = new List<TestVariable>(dictionary.Count);
      foreach (TestVariable testVariable in dictionary.Values)
        testVariableList.Add(testVariable);
      return testVariableList;
    }

    public override TestResultsDetailsGroupData GetAggregatedTestResultsForBuild4(
      Guid projectGuid,
      int buildId,
      string buildUri,
      string sourceWorkflow,
      List<string> groupByFields,
      Dictionary<string, Tuple<string, List<string>>> filterValues,
      Dictionary<string, string> orderBy,
      bool isRerunOnPassedFilter,
      bool isAbortedRunEnabled,
      bool isInProgressRunsEnabled,
      bool shouldReturnResults,
      bool queryRunSummaryForInProgress,
      bool isDefaultFilterWithFilteredIndex,
      bool shouldFetchOldTestCaseRefId,
      int runIdThreshold = 0)
    {
      IGroupByHelper helper = GroupByHelperFactory2.GetHelper(groupByFields, this.RequestContext, projectGuid, (Func<IQueryGroupedTestResultsColumns>) (() => (IQueryGroupedTestResultsColumns) new TestManagementDatabase36.QueryGroupedTestResultsColumns3()));
      return this.GetAggregatedResults4(projectGuid, buildId, buildUri, 0, 0, sourceWorkflow, groupByFields, filterValues, orderBy, helper, isRerunOnPassedFilter, isAbortedRunEnabled, isInProgressRunsEnabled, shouldReturnResults, queryRunSummaryForInProgress, isDefaultFilterWithFilteredIndex, shouldFetchOldTestCaseRefId, runIdThreshold);
    }

    public override TestResultsDetailsGroupData GetAggregatedTestResultsForRelease4(
      Guid projectGuid,
      int releaseId,
      int releaseEnvId,
      string sourceWorkflow,
      List<string> groupByFields,
      Dictionary<string, Tuple<string, List<string>>> filterValues,
      Dictionary<string, string> orderBy,
      bool isRerunOnPassedFilter,
      bool isAbortedRunEnabled,
      bool isInProgressRunsEnabled,
      bool shouldReturnResults,
      bool queryRunSummaryForInProgress,
      bool isDefaultFilterWithFilteredIndex,
      bool shouldFetchOldTestCaseRefId,
      int runIdThreshold = 0)
    {
      IGroupByHelper helper = GroupByHelperFactory2.GetHelper(groupByFields, this.RequestContext, projectGuid, (Func<IQueryGroupedTestResultsColumns>) (() => (IQueryGroupedTestResultsColumns) new TestManagementDatabase36.QueryGroupedTestResultsColumns3()));
      return this.GetAggregatedResults4(projectGuid, 0, string.Empty, releaseId, releaseEnvId, sourceWorkflow, groupByFields, filterValues, orderBy, helper, isRerunOnPassedFilter, isAbortedRunEnabled, isInProgressRunsEnabled, shouldReturnResults, queryRunSummaryForInProgress, isDefaultFilterWithFilteredIndex, shouldFetchOldTestCaseRefId, runIdThreshold);
    }

    protected TestResultsDetailsGroupData GetAggregatedResults4(
      Guid projectGuid,
      int buildId,
      string buildUri,
      int releaseId,
      int releaseEnvId,
      string sourceWorkflow,
      List<string> groupByFields,
      Dictionary<string, Tuple<string, List<string>>> filterValues,
      Dictionary<string, string> orderBy,
      IGroupByHelper groupByHelper,
      bool isRerunOnPassedFilter,
      bool isAbortedRunEnabled,
      bool isInProgressRunsEnabled,
      bool shouldReturnResults,
      bool queryRunSummaryForInProgress,
      bool isDefaultFilterWithFilteredIndex,
      bool shouldFetchOldTestCaseRefId,
      int runIdThreshold = 0)
    {
      string str1 = string.Empty;
      string groupByDbFieldString = string.Empty;
      if (groupByFields != null && groupByFields.Any<string>())
      {
        str1 = string.Join(",", (IEnumerable<string>) groupByFields);
        groupByDbFieldString = string.Join(",", (IEnumerable<string>) this.ConvertGroupByFieldNameToDbColumnName(groupByFields, releaseId > 0 && releaseEnvId <= 0));
      }
      List<string> dbColumnName3 = this.ConvertGroupByFieldNameToDbColumnName3(groupByFields);
      dbColumnName3.Add(TestResultsConstants.OutcomeColumnName);
      if (!string.IsNullOrEmpty(groupByDbFieldString))
      {
        string str2 = groupByDbFieldString + ", " + TestResultsConstants.OutcomeColumnName + " ";
      }
      else
      {
        string outcomeColumnName = TestResultsConstants.OutcomeColumnName;
      }
      List<int> outcomeFilters = this.GetOutcomeFilters(filterValues);
      List<string> filters1 = this.GetFilters(filterValues, TestResultsConstants.OwnerFilterName);
      List<string> filters2 = this.GetFilters(filterValues, TestResultsConstants.ContainerFilterName);
      List<string> orderByFields = this.GetOrderByFields(orderBy);
      string groupByCriteria3 = this.GetGroupByCriteria3(groupByFields);
      List<string> propertiesToFetchList = this.GetPropertiesToFetchList(str1, groupByDbFieldString);
      if (shouldFetchOldTestCaseRefId)
        propertiesToFetchList.Add(TestResultsConstants.OldTestCaseRefId);
      this.PrepareStoredProcedure("prc_QueryAggregatedTestResults");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
      this.BindInt("@buildId", buildId);
      this.BindString("@buildUri", buildUri, 256, false, SqlDbType.NVarChar);
      this.BindInt("@releaseId", releaseId);
      this.BindInt("@releaseEnvId", releaseEnvId);
      this.BindString("@sourceWorkflow", sourceWorkflow, 128, false, SqlDbType.NVarChar);
      this.BindIdTypeTable("@outcomeFilters", (IEnumerable<int>) outcomeFilters);
      this.BindNameTypeTable("@ownerFilters", (IEnumerable<string>) filters1);
      this.BindNameTypeTable("@containerFilters", (IEnumerable<string>) filters2);
      this.BindBoolean("@rerunFilter", isRerunOnPassedFilter);
      this.BindBoolean("@isAbortedRunEnabled", isAbortedRunEnabled);
      this.BindBoolean("@shouldReturnResults", shouldReturnResults);
      this.BindBoolean("@isInProgressEnabled", isInProgressRunsEnabled);
      this.BindNameTypeTable("@propertiesToFetch", (IEnumerable<string>) propertiesToFetchList);
      this.BindNameTypeTable("@orderBy", (IEnumerable<string>) orderByFields);
      this.BindString("@groupByPivot", groupByCriteria3, 512, false, SqlDbType.NVarChar);
      this.BindNameTypeTable("@groupBy", (IEnumerable<string>) dbColumnName3);
      this.BindBoolean("@queryRunSummaryForInProgress", queryRunSummaryForInProgress);
      this.BindBoolean("@isDefaultFilterWithFilteredIndex", isDefaultFilterWithFilteredIndex);
      this.BindInt("@runIdThreshold", runIdThreshold);
      TestResultsDetailsGroupData aggregatedResults4 = new TestResultsDetailsGroupData();
      TestResultsDetails testResultsDetails = new TestResultsDetails();
      testResultsDetails.GroupByField = str1;
      Dictionary<object, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> resultsMap = new Dictionary<object, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      Dictionary<string, TestResultsDetailsForGroup> aggregatedResultsMap = new Dictionary<string, TestResultsDetailsForGroup>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      TestManagementDatabase.FetchTestResultsColumns testResultsColumns1 = new TestManagementDatabase.FetchTestResultsColumns();
      TestManagementDatabase36.QueryGroupedTestResultsColumns testResultsColumns2 = new TestManagementDatabase36.QueryGroupedTestResultsColumns();
      Dictionary<int, Tuple<string, int, int>> testSuiteDetails = new Dictionary<int, Tuple<string, int, int>>();
      Dictionary<int, Tuple<int, int>> testPointDetails = new Dictionary<int, Tuple<int, int>>();
      Dictionary<int, int> oldTestCaseRefMap = new Dictionary<int, int>();
      using (SqlDataReader reader = this.ExecuteReader())
      {
        groupByHelper.ReadAggregatedTestResults(reader, aggregatedResultsMap, testPointDetails, testSuiteDetails, str1);
        if (reader.NextResult())
        {
          while (reader.Read())
          {
            testResultsColumns2.bindResultDetails(reader, resultsMap, groupByHelper);
            if (shouldFetchOldTestCaseRefId)
              testResultsColumns2.bindOldTestCaseRefDetails(reader, oldTestCaseRefMap);
          }
        }
      }
      groupByHelper.PopulateAggregatedResults(resultsMap, aggregatedResultsMap, str1, testPointDetails);
      testResultsDetails.ResultsForGroup = (IList<TestResultsDetailsForGroup>) aggregatedResultsMap.Values.ToList<TestResultsDetailsForGroup>();
      aggregatedResults4.TestResultsDetails = testResultsDetails;
      aggregatedResults4.OldTestCaseRefIdMap = oldTestCaseRefMap;
      return aggregatedResults4;
    }
  }
}
