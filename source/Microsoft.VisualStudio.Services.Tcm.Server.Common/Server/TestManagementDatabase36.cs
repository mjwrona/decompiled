// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase36
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase36 : TestManagementDatabase35
  {
    internal override Dictionary<TestCaseResultIdentifier, List<int>> CreateAttachments(
      Guid projectId,
      IEnumerable<TestResultAttachment> attachments,
      bool areSessionAttachments,
      bool changeCounterInterval = false)
    {
      this.AssignTempIdToAttachments(attachments, changeCounterInterval);
      this.PrepareStoredProcedure("TestResult.prc_CreateAttachments");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindTestResultAttachmentTypeTable2("@attachments", attachments);
      this.BindBoolean("@areSessionAttachments", areSessionAttachments);
      this.BindBoolean("@changeCounterInterval", changeCounterInterval);
      SqlDataReader reader = this.ExecuteReader();
      if (!reader.Read())
        throw new UnexpectedDatabaseResultException("prc_CreateAttachments");
      List<int> intList1 = new List<int>();
      TestManagementDatabase.CreateAttachmentColumns attachmentColumns = new TestManagementDatabase.CreateAttachmentColumns();
      do
      {
        intList1.Add(attachmentColumns.AttachmentId.GetInt32((IDataReader) reader));
      }
      while (reader.Read());
      Dictionary<TestCaseResultIdentifier, List<int>> attachments1 = new Dictionary<TestCaseResultIdentifier, List<int>>();
      int num = 0;
      foreach (TestResultAttachment attachment in attachments)
      {
        TestCaseResultIdentifier key = new TestCaseResultIdentifier(attachment.TestRunId, attachment.TestResultId);
        List<int> intList2;
        if (!attachments1.TryGetValue(key, out intList2))
        {
          intList2 = new List<int>();
          attachments1.Add(key, intList2);
        }
        intList2.Add(intList1[num++]);
      }
      return attachments1;
    }

    internal TestManagementDatabase36(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase36()
    {
    }

    internal override List<int> QueryTestRunIdsByChangedDate(
      int projectId,
      int batchSize,
      string prBranchName,
      DateTime fromDate,
      out DateTime toDate,
      TestArtifactSource dataSource)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestRunIdsByChangedDate");
      this.BindInt("@dataspaceId", projectId);
      if (DateTime.Compare(fromDate, SqlDateTime.MinValue.Value) < 0)
        fromDate = SqlDateTime.MinValue.Value;
      this.BindInt("@batchSize", batchSize);
      this.BindDateTime("@fromChangedDate", fromDate, true);
      this.BindString("@prBranchName", prBranchName, 50, true, SqlDbType.NVarChar);
      SqlDataReader reader = this.ExecuteReader();
      List<int> intList = new List<int>();
      toDate = fromDate;
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("TestRunId");
      SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("LastUpdated");
      while (reader.Read())
      {
        int int32 = sqlColumnBinder1.GetInt32((IDataReader) reader);
        DateTime dateTime = sqlColumnBinder2.GetDateTime((IDataReader) reader);
        intList.Add(int32);
        toDate = dateTime;
      }
      return intList;
    }

    public override TestRun CreateTestRun(
      Guid projectId,
      TestRun testRun,
      Guid updatedBy,
      bool changeCounterInterval = false,
      bool isTcmService = false,
      bool reuseDeletedTestRunId = false,
      int reuseTestRunIdDays = 2)
    {
      try
      {
        this.PrepareStoredProcedure("prc_CreateTestRun");
        this.BindString("@title", testRun.Title, 256, false, SqlDbType.NVarChar);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindGuid("@owner", testRun.Owner);
        this.BindByte("@state", testRun.State);
        this.BindString("@dropLocation", testRun.DropLocation, 260, true, SqlDbType.NVarChar);
        this.BindInt("@testPlanId", testRun.TestPlanId);
        this.BindNullableDateTime("@dueDate", testRun.DueDate);
        this.BindInt("@iterationId", testRun.IterationId);
        this.BindString("@controller", testRun.Controller, 256, true, SqlDbType.NVarChar);
        this.BindInt("@testMessageLogId", testRun.TestMessageLogId);
        this.BindInt("@testSettingsId", testRun.TestSettingsId);
        this.BindInt("@publicTestSettingsId", testRun.PublicTestSettingsId);
        this.BindGuid("@testEnvironmentId", testRun.TestEnvironmentId);
        this.BindString("@legacySharePath", testRun.LegacySharePath, 1024, false, SqlDbType.NVarChar);
        this.BindBoolean("@isAutomated", testRun.IsAutomated);
        this.BindByte("@type", testRun.Type);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindInt("@version", testRun.Version);
        List<BuildConfiguration> builds;
        if (testRun.BuildReference == null)
        {
          builds = (List<BuildConfiguration>) null;
        }
        else
        {
          builds = new List<BuildConfiguration>();
          builds.Add(testRun.BuildReference);
        }
        this.BindBuildRefTypeTable3("@buildRefData", (IEnumerable<BuildConfiguration>) builds);
        List<ReleaseReference> releases;
        if (testRun.ReleaseReference == null)
        {
          releases = (List<ReleaseReference>) null;
        }
        else
        {
          releases = new List<ReleaseReference>();
          releases.Add(testRun.ReleaseReference);
        }
        this.BindReleaseRefTypeTable4("@releaseRefData", (IEnumerable<ReleaseReference>) releases);
        this.BindTestExtensionFieldValuesTypeTable("@additionalFields", testRun.CustomFields != null ? testRun.CustomFields.Select<TestExtensionField, Tuple<int, int, TestExtensionField>>((System.Func<TestExtensionField, Tuple<int, int, TestExtensionField>>) (f => new Tuple<int, int, TestExtensionField>(testRun.TestRunId, 0, f))) : (IEnumerable<Tuple<int, int, TestExtensionField>>) null);
        this.BindString("@sourceWorkflow", testRun.SourceWorkflow, 128, false, SqlDbType.NVarChar);
        this.BindBoolean("@isBvt", testRun.IsBvt);
        this.BindString("@testEnvironmentUrl", !testRun.RunHasDtlEnvironment || testRun.DtlTestEnvironment == null ? (string) null : testRun.DtlTestEnvironment.Url, 2048, true, SqlDbType.NVarChar);
        this.BindString("@autEnvironmentUrl", !testRun.RunHasDtlEnvironment || testRun.DtlAutEnvironment == null ? (string) null : testRun.DtlAutEnvironment.Url, 2048, true, SqlDbType.NVarChar);
        this.BindString("@sourceFilter", !testRun.RunHasDtlEnvironment || !testRun.IsAutomated ? (string) null : testRun.Filter.SourceFilter, 1024, true, SqlDbType.NVarChar);
        this.BindString("@TestCaseFilter", !testRun.RunHasDtlEnvironment || !testRun.IsAutomated ? (string) null : testRun.Filter.TestCaseFilter, 2048, true, SqlDbType.NVarChar);
        this.BindBoolean("@changeCounterInterval", changeCounterInterval);
        SqlDataReader reader = this.ExecuteReader();
        return reader.Read() ? new TestManagementDatabase13.CreateTestRunColumns().bind(reader) : throw new UnexpectedDatabaseResultException("prc_CreateTestRun");
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    internal override List<TestCaseResult> QueryTestResultsByBuildOrRelease(
      Guid projectId,
      int buildId,
      int releaseId,
      int releaseEnvId,
      string sourceWorkflow,
      IList<byte> runStates,
      bool fetchFailedTestsOnly,
      int continuationTokenRunId,
      int continuationTokenResultId,
      int top)
    {
      bool parameterValue = runStates.Contains((byte) 4);
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultsByBuildOrRelease");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@buildId", buildId);
      this.BindInt("@releaseId", releaseId);
      this.BindInt("@releaseEnvId", releaseEnvId);
      this.BindString("@sourceWorkflow", sourceWorkflow, 128, false, SqlDbType.NVarChar);
      this.BindInt("@continuationTokenRunId", continuationTokenRunId);
      this.BindInt("@continuationTokenResultId", continuationTokenResultId);
      this.BindInt("@top", top);
      this.BindBoolean("@isAbortedRunEnabled", parameterValue);
      SqlDataReader reader = this.ExecuteReader();
      List<TestCaseResult> testCaseResultList = new List<TestCaseResult>();
      List<TestCaseResult> testResults = testCaseResultList;
      TestManagementDatabase36.GetTestResultsBind(reader, testResults);
      return testCaseResultList;
    }

    protected static void GetTestResultsBind(SqlDataReader reader, List<TestCaseResult> testResults)
    {
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("TestRunId");
      SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("TestResultId");
      SqlColumnBinder sqlColumnBinder3 = new SqlColumnBinder("TestCaseRefId");
      SqlColumnBinder sqlColumnBinder4 = new SqlColumnBinder("Outcome");
      SqlColumnBinder sqlColumnBinder5 = new SqlColumnBinder("Priority");
      SqlColumnBinder sqlColumnBinder6 = new SqlColumnBinder("TestCaseOwner");
      SqlColumnBinder sqlColumnBinder7 = new SqlColumnBinder("AutomatedTestStorage");
      SqlColumnBinder sqlColumnBinder8 = new SqlColumnBinder("IsRerun");
      SqlColumnBinder sqlColumnBinder9 = new SqlColumnBinder("TestCaseTitle");
      SqlColumnBinder sqlColumnBinder10 = new SqlColumnBinder("DateStarted");
      SqlColumnBinder sqlColumnBinder11 = new SqlColumnBinder("DateCompleted");
      SqlColumnBinder sqlColumnBinder12 = new SqlColumnBinder("AutomatedTestName");
      while (reader.Read())
      {
        TestCaseResult testCaseResult = new TestCaseResult();
        testCaseResult.TestRunId = sqlColumnBinder1.GetInt32((IDataReader) reader);
        testCaseResult.TestResultId = sqlColumnBinder2.GetInt32((IDataReader) reader);
        testCaseResult.TestCaseReferenceId = sqlColumnBinder3.GetInt32((IDataReader) reader);
        testCaseResult.Outcome = sqlColumnBinder4.GetByte((IDataReader) reader);
        testCaseResult.Priority = sqlColumnBinder5.GetByte((IDataReader) reader);
        testCaseResult.AutomatedTestStorage = sqlColumnBinder7.GetString((IDataReader) reader, true);
        testCaseResult.OwnerName = sqlColumnBinder6.GetString((IDataReader) reader, true);
        testCaseResult.IsRerun = sqlColumnBinder8.GetBoolean((IDataReader) reader);
        testCaseResult.TestCaseTitle = sqlColumnBinder9.GetString((IDataReader) reader, true);
        testCaseResult.DateStarted = sqlColumnBinder10.GetNullableDateTime((IDataReader) reader, new DateTime?(new DateTime())).GetValueOrDefault();
        testCaseResult.DateCompleted = sqlColumnBinder11.GetNullableDateTime((IDataReader) reader, new DateTime?(new DateTime())).GetValueOrDefault();
        testCaseResult.AutomatedTestName = sqlColumnBinder12.ColumnExists((IDataReader) reader) ? sqlColumnBinder12.GetString((IDataReader) reader, true) : (string) null;
        testResults.Add(testCaseResult);
      }
    }

    protected List<string> ConvertGroupByFieldNameToDbColumnName(
      List<string> groupByFields,
      bool releaseEnvAllAndGroupByTestRun)
    {
      List<string> dbColumnName = new List<string>();
      if (groupByFields == null)
        return dbColumnName;
      foreach (string groupByField in groupByFields)
      {
        if (string.Equals(ValidTestResultGroupByFields.Container, groupByField, StringComparison.InvariantCultureIgnoreCase))
          dbColumnName.Add(TestResultsConstants.ContainerColumnName);
        else if (string.Equals(ValidTestResultGroupByFields.TestRun, groupByField, StringComparison.InvariantCultureIgnoreCase))
        {
          dbColumnName.Add(TestResultsConstants.TestRunIdColumnName);
          dbColumnName.Add(TestResultsConstants.GroupByTestRunTitleColumnName);
          if (releaseEnvAllAndGroupByTestRun)
          {
            dbColumnName.Add(TestResultsConstants.GroupByTestRunStateColumnName);
            dbColumnName.Add(TestResultsConstants.GroupByTestRunCompleteDateColumnName);
            dbColumnName.Add(TestResultsConstants.ReleaseEnvironmentIdColumnName);
          }
        }
        else if (string.Equals(ValidTestResultGroupByFields.Priority, groupByField, StringComparison.InvariantCultureIgnoreCase))
          dbColumnName.Add(TestResultsConstants.PriorityColumnName);
        else if (string.Equals(ValidTestResultGroupByFields.TestSuite, groupByField, StringComparison.InvariantCultureIgnoreCase))
          dbColumnName.Add(TestResultsConstants.TestSuiteColumnName);
        else if (string.Equals(ValidTestResultGroupByFields.Requirement, groupByField, StringComparison.InvariantCultureIgnoreCase))
          dbColumnName.Add(TestResultsConstants.RequirementColumnName);
        else if (string.Equals(ValidTestResultGroupByFields.Owner, groupByField, StringComparison.InvariantCultureIgnoreCase))
          dbColumnName.Add(TestResultsConstants.OwnerNameColumnName);
      }
      return dbColumnName;
    }

    protected string GetGroupByCriteria3(List<string> groupByFields)
    {
      if (groupByFields != null)
      {
        foreach (string groupByField in groupByFields)
        {
          if (string.Equals(ValidTestResultGroupByFields.TestSuite, groupByField, StringComparison.OrdinalIgnoreCase))
            return ValidTestResultGroupByFields.TestSuite;
          if (string.Equals(ValidTestResultGroupByFields.Requirement, groupByField, StringComparison.OrdinalIgnoreCase))
            return ValidTestResultGroupByFields.Requirement;
          if (string.Equals(ValidTestResultGroupByFields.TestRun, groupByField, StringComparison.OrdinalIgnoreCase))
            return ValidTestResultGroupByFields.TestRun;
        }
      }
      return TestResultsConstants.TestResultField;
    }

    protected List<int> GetOutcomeFilters(
      Dictionary<string, Tuple<string, List<string>>> filterValues)
    {
      List<int> outcomeFilters = new List<int>();
      if (filterValues == null)
        return outcomeFilters;
      string empty = string.Empty;
      foreach (KeyValuePair<string, Tuple<string, List<string>>> filterValue in filterValues)
      {
        if (string.Equals(filterValue.Key, TestResultsConstants.OutcomeColumnName, StringComparison.InvariantCultureIgnoreCase))
        {
          foreach (string str in filterValue.Value.Item2)
            outcomeFilters.Add(Convert.ToInt32(str));
        }
      }
      return outcomeFilters;
    }

    protected List<string> GetFilters(
      Dictionary<string, Tuple<string, List<string>>> filterValues,
      string filterBy)
    {
      List<string> filters = new List<string>();
      if (filterValues == null)
        return filters;
      string empty = string.Empty;
      foreach (KeyValuePair<string, Tuple<string, List<string>>> filterValue in filterValues)
      {
        if (string.Equals(filterValue.Key, filterBy, StringComparison.InvariantCultureIgnoreCase))
          filters = filterValue.Value.Item2;
      }
      return filters;
    }

    protected List<string> GetOrderByFields(Dictionary<string, string> orderBy)
    {
      List<string> orderByFields = new List<string>();
      if (orderBy == null)
        return new List<string>();
      string empty = string.Empty;
      foreach (KeyValuePair<string, string> keyValuePair in orderBy)
      {
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} {1}", (object) keyValuePair.Key, (object) keyValuePair.Value);
        orderByFields.Add(str);
      }
      return orderByFields;
    }

    protected List<string> GetPropertiesToFetchList(
      string groupByField,
      string groupByDbFieldString)
    {
      List<string> stringList = new List<string>();
      List<string> list = ((IEnumerable<string>) TestResultsConstants.DefaultIdentifierProperties.Split(',')).Select<string, string>((System.Func<string, string>) (x => x.Trim())).ToList<string>();
      if (!string.IsNullOrEmpty(groupByDbFieldString))
      {
        if (string.Equals(ValidTestResultGroupByFields.TestSuite, groupByField, StringComparison.OrdinalIgnoreCase))
          list.Add(TestResultsConstants.TestPointColumnName);
        else if (string.Equals(ValidTestResultGroupByFields.Requirement, groupByField, StringComparison.OrdinalIgnoreCase))
          list.Add(TestResultsConstants.RequirementColumnName);
        else if (!string.Equals(ValidTestResultGroupByFields.TestRun, groupByField, StringComparison.OrdinalIgnoreCase))
          list.AddRange((IEnumerable<string>) ((IEnumerable<string>) groupByDbFieldString.Split(',')).Select<string, string>((System.Func<string, string>) (x => x.Trim())).ToList<string>());
      }
      else
      {
        list.Add(TestResultsConstants.OutcomeColumnName);
        list.Add(TestResultsConstants.DurationColumnName);
      }
      return list;
    }

    protected List<string> ConvertGroupByFieldNameToDbColumnName3(List<string> groupByFields)
    {
      List<string> dbColumnName3 = new List<string>();
      if (groupByFields == null)
        return dbColumnName3;
      foreach (string groupByField in groupByFields)
      {
        if (string.Equals(ValidTestResultGroupByFields.Container, groupByField, StringComparison.InvariantCultureIgnoreCase))
          dbColumnName3.Add(TestResultsConstants.ContainerColumnName);
        else if (string.Equals(ValidTestResultGroupByFields.TestRun, groupByField, StringComparison.InvariantCultureIgnoreCase))
          dbColumnName3.Add(TestResultsConstants.TestRunIdColumnName);
        else if (string.Equals(ValidTestResultGroupByFields.Priority, groupByField, StringComparison.InvariantCultureIgnoreCase))
          dbColumnName3.Add(TestResultsConstants.PriorityColumnName);
        else if (string.Equals(ValidTestResultGroupByFields.TestSuite, groupByField, StringComparison.InvariantCultureIgnoreCase))
          dbColumnName3.Add(TestResultsConstants.TestSuiteColumnName);
        else if (string.Equals(ValidTestResultGroupByFields.Requirement, groupByField, StringComparison.InvariantCultureIgnoreCase))
          dbColumnName3.Add(TestResultsConstants.RequirementColumnName);
        else if (string.Equals(ValidTestResultGroupByFields.Owner, groupByField, StringComparison.InvariantCultureIgnoreCase))
          dbColumnName3.Add(TestResultsConstants.OwnerNameColumnName);
      }
      return dbColumnName3;
    }

    public override void FetchTestFailureDetails(
      GuidAndString projectId,
      List<int> testRunIds,
      BuildConfiguration buildToCompare,
      ReleaseReference releaseToCompare,
      string sourceWorkflow,
      bool fetchPreviousFailedResults,
      bool shouldByPassFlaky,
      out Dictionary<int, List<TestCaseResult>> currentFailedResults,
      out Dictionary<int, TestCaseResult> previousFailedResultsMap,
      out int prevTestRunContextId)
    {
      currentFailedResults = testRunIds.ToDictionary<int, int, List<TestCaseResult>>((System.Func<int, int>) (testRunId => testRunId), (System.Func<int, List<TestCaseResult>>) (testRunId => new List<TestCaseResult>()));
      previousFailedResultsMap = new Dictionary<int, TestCaseResult>();
      prevTestRunContextId = 0;
      this.PrepareStoredProcedure("TestResult.prc_FetchTestFailureDetailsInMultipleRuns");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId.GuidId));
      this.BindIdTypeTable("@testRuns", (IEnumerable<int>) testRunIds);
      List<BuildConfiguration> builds;
      if (buildToCompare != null)
        builds = new List<BuildConfiguration>()
        {
          buildToCompare
        };
      else
        builds = (List<BuildConfiguration>) null;
      this.BindBuildRefTypeTable3("@buildToCompare", (IEnumerable<BuildConfiguration>) builds);
      List<ReleaseReference> releases;
      if (releaseToCompare != null)
        releases = new List<ReleaseReference>()
        {
          releaseToCompare
        };
      else
        releases = (List<ReleaseReference>) null;
      this.BindReleaseRefTypeTable2("@releaseToCompare", (IEnumerable<ReleaseReference>) releases);
      this.BindString("@sourceWorkflow", sourceWorkflow, int.MaxValue, false, SqlDbType.NVarChar);
      this.BindBoolean("@fetchPreviousFailedResults", fetchPreviousFailedResults);
      SqlDataReader reader1 = this.ExecuteReader();
      if (fetchPreviousFailedResults)
      {
        TestManagementDatabase23.FetchTestResultFailuresColumns resultFailuresColumns = new TestManagementDatabase23.FetchTestResultFailuresColumns();
        while (reader1.Read())
        {
          TestCaseResult testCaseResult = resultFailuresColumns.bind(reader1);
          previousFailedResultsMap[testCaseResult.TestCaseReferenceId] = testCaseResult;
        }
        if (!reader1.NextResult())
          throw new UnexpectedDatabaseResultException("TestResult.prc_FetchTestFailureDetailsInMultipleRuns");
      }
      while (reader1.Read())
      {
        SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("TestRunId");
        TestManagementDatabase23.FetchTestResultFailuresColumns resultFailuresColumns = new TestManagementDatabase23.FetchTestResultFailuresColumns();
        int int32 = sqlColumnBinder.GetInt32((IDataReader) reader1);
        SqlDataReader reader2 = reader1;
        TestCaseResult testCaseResult = resultFailuresColumns.bind(reader2);
        currentFailedResults[int32].Add(testCaseResult);
      }
      if (!reader1.NextResult() || !reader1.Read())
        return;
      prevTestRunContextId = new SqlColumnBinder("TestRunContextId").GetInt32((IDataReader) reader1, 0);
    }

    public override void PublishTestSummaryAndFailureDetails(
      GuidAndString projectId,
      List<int> testRunIds,
      Dictionary<int, ResultInsights> resultInsights,
      Dictionary<int, Dictionary<int, string>> failingSinceDetails,
      List<TestResultIdentifierRecord> flakyResults,
      bool includeFailureDetails,
      bool publishPassCountOnly = false,
      bool shouldPublishFlakiness = false,
      bool shouldByPassFlaky = false)
    {
      this.PrepareStoredProcedure("TestResult.prc_PublishTestSummaryAndFailureDetails");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId.GuidId));
      this.BindTestResult_FailureCountForTestRunTypeTable("@runsFailureCount", resultInsights);
      this.BindTestResult_FailingSinceDetailsTable("@failingSinceDetails", failingSinceDetails);
      this.BindBoolean("@updateFailureDetails", includeFailureDetails);
      this.BindInt("@prevTestRunContextId", includeFailureDetails ? resultInsights.First<KeyValuePair<int, ResultInsights>>().Value.PrevRunContextId : 0);
      this.BindBoolean("@publishPassCountOnly", publishPassCountOnly);
      this.ExecuteNonQuery();
    }

    public override RunSummaryAndInsights QueryTestRunSummaryAndInsightsForBuild(
      GuidAndString projectId,
      string sourceWorkflow,
      BuildConfiguration buildRef,
      bool returnSummary,
      bool returnFailureDetails,
      string categoryName,
      out int runsCount,
      out bool isBuildOld,
      int rundIdThreshold = 0)
    {
      this.PrepareStoredProcedure("prc_QueryTestRunSummaryAndInsightsForBuild");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId.GuidId));
      this.BindString("@sourceWorkflow", sourceWorkflow, 128, false, SqlDbType.NVarChar);
      this.BindInt("@buildId", buildRef != null ? buildRef.BuildId : 0);
      this.BindString("@buildUri", buildRef?.BuildUri, 256, true, SqlDbType.NVarChar);
      this.BindBoolean("@returnSummary", returnSummary);
      this.BindBoolean("@returnFailureDetails", returnFailureDetails);
      this.BindString("@categoryName", categoryName, 128, true, SqlDbType.NVarChar);
      SqlDataReader reader = this.ExecuteReader();
      RunSummaryAndInsights runSummaryAndInsights = new RunSummaryAndInsights()
      {
        TestRunSummary = new RunSummary()
        {
          CurrentAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>(),
          PreviousAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>(),
          CurrentAggregatedRunsByState = (IList<RunSummaryByState>) new List<RunSummaryByState>(),
          CurrentAggregateDataByReportingCategory = (IList<RunSummaryByCategory>) new List<RunSummaryByCategory>()
        },
        TestResultInsights = new List<ResultInsights>()
      };
      runsCount = 0;
      isBuildOld = false;
      SqlColumnBinder sqlColumnBinder;
      if (reader.Read())
      {
        ref bool local = ref isBuildOld;
        sqlColumnBinder = new SqlColumnBinder("IsBuildOld");
        int num = sqlColumnBinder.GetBoolean((IDataReader) reader) ? 1 : 0;
        local = num != 0;
      }
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
      TestManagementDatabase36.FetchTestRunsByStateColumns runsByStateColumns = new TestManagementDatabase36.FetchTestRunsByStateColumns();
      while (reader.Read())
        runSummaryAndInsights.TestRunSummary.CurrentAggregatedRunsByState.Add(runsByStateColumns.bind(reader));
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
      if (reader.Read())
      {
        ref int local = ref runsCount;
        sqlColumnBinder = new SqlColumnBinder("RunsCount");
        int int32 = sqlColumnBinder.GetInt32((IDataReader) reader);
        local = int32;
      }
      if (runsCount > 0)
      {
        if (returnSummary)
        {
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
          TestManagementDatabase2.FetchTestRunSummaryColumns runSummaryColumns = new TestManagementDatabase2.FetchTestRunSummaryColumns();
          while (reader.Read())
            runSummaryAndInsights.TestRunSummary.CurrentAggregateDataByOutcome.Add(runSummaryColumns.bind(reader));
          if (!isBuildOld)
          {
            if (!reader.NextResult())
              throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
            while (reader.Read())
              runSummaryAndInsights.TestRunSummary.PreviousAggregateDataByOutcome.Add(runSummaryColumns.bind(reader));
          }
        }
        if (returnFailureDetails && !isBuildOld)
        {
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForBuild");
          TestManagementDatabase2.FetchTestFailureDetailsColumns failureDetailsColumns = new TestManagementDatabase2.FetchTestFailureDetailsColumns();
          while (reader.Read())
            runSummaryAndInsights.TestResultInsights.Add(failureDetailsColumns.bind(reader));
        }
        TestManagementDatabase19.PopulateAggregateDataByCategory(categoryName, reader, runSummaryAndInsights, "prc_QueryTestRunSummaryAndInsightsForBuild");
      }
      return runSummaryAndInsights;
    }

    public override RunSummaryAndInsights QueryTestRunSummaryAndInsightsForRelease(
      GuidAndString projectId,
      string sourceWorkflow,
      ReleaseReference releaseRef,
      bool returnSummary,
      bool returnFailureDetails,
      string categoryName,
      out int runsCount,
      int runIdThreshold = 0)
    {
      this.PrepareStoredProcedure("prc_QueryTestRunSummaryAndInsightsForRelease");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId.GuidId));
      this.BindString("@sourceWorkflow", sourceWorkflow, 128, false, SqlDbType.NVarChar);
      this.BindInt("@releaseId", releaseRef != null ? releaseRef.ReleaseId : 0);
      this.BindInt("@releaseEnvId", releaseRef != null ? releaseRef.ReleaseEnvId : 0);
      this.BindInt("@attempt", releaseRef != null ? releaseRef.Attempt : 0);
      this.BindBoolean("@returnSummary", returnSummary);
      this.BindBoolean("@returnFailureDetails", returnFailureDetails);
      this.BindString("@categoryName", categoryName, 128, true, SqlDbType.NVarChar);
      SqlDataReader reader = this.ExecuteReader();
      RunSummaryAndInsights runSummaryAndInsights = new RunSummaryAndInsights()
      {
        TestRunSummary = new RunSummary()
        {
          CurrentAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>(),
          PreviousAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>(),
          CurrentAggregatedRunsByState = (IList<RunSummaryByState>) new List<RunSummaryByState>(),
          CurrentAggregateDataByReportingCategory = (IList<RunSummaryByCategory>) new List<RunSummaryByCategory>()
        },
        TestResultInsights = new List<ResultInsights>()
      };
      TestManagementDatabase36.FetchTestRunsByStateColumns runsByStateColumns = new TestManagementDatabase36.FetchTestRunsByStateColumns();
      while (reader.Read())
        runSummaryAndInsights.TestRunSummary.CurrentAggregatedRunsByState.Add(runsByStateColumns.bind(reader));
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForRelease");
      runsCount = 0;
      if (reader.Read())
        runsCount = new SqlColumnBinder("RunsCount").GetInt32((IDataReader) reader);
      if (runsCount > 0)
      {
        if (returnSummary)
        {
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForRelease");
          TestManagementDatabase2.FetchTestRunSummaryColumns runSummaryColumns = new TestManagementDatabase2.FetchTestRunSummaryColumns();
          while (reader.Read())
            runSummaryAndInsights.TestRunSummary.CurrentAggregateDataByOutcome.Add(runSummaryColumns.bind(reader));
          if (reader.NextResult())
          {
            while (reader.Read())
              runSummaryAndInsights.TestRunSummary.PreviousAggregateDataByOutcome.Add(runSummaryColumns.bind(reader));
          }
        }
        if (returnFailureDetails)
        {
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsightsForRelease");
          TestManagementDatabase2.FetchTestFailureDetailsColumns failureDetailsColumns = new TestManagementDatabase2.FetchTestFailureDetailsColumns();
          while (reader.Read())
            runSummaryAndInsights.TestResultInsights.Add(failureDetailsColumns.bind(reader));
        }
        TestManagementDatabase19.PopulateAggregateDataByCategory(categoryName, reader, runSummaryAndInsights, "prc_QueryTestRunSummaryAndInsightsForRelease");
      }
      return runSummaryAndInsights;
    }

    private void AddEmptySummaryEntryIfNotPresent(
      Dictionary<ReleaseReference, RunSummaryAndInsights> runSummaryAndInsightsMap,
      ReleaseReference release)
    {
      if (runSummaryAndInsightsMap.ContainsKey(release))
        return;
      runSummaryAndInsightsMap[release] = new RunSummaryAndInsights()
      {
        TestRunSummary = new RunSummary()
        {
          CurrentAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>(),
          PreviousAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>(),
          CurrentAggregatedRunsByState = (IList<RunSummaryByState>) new List<RunSummaryByState>(),
          CurrentAggregateDataByReportingCategory = (IList<RunSummaryByCategory>) new List<RunSummaryByCategory>()
        }
      };
    }

    public override Dictionary<ReleaseReference, RunSummaryAndInsights> QueryTestRunSummaryForReleases(
      GuidAndString projectId,
      List<ReleaseReference> releases,
      string categoryName,
      int runIdThreshold = 0)
    {
      this.PrepareStoredProcedure("prc_QueryTestRunSummaryForReleases");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId.GuidId));
      this.BindReleaseRefTypeTable4("@releases", (IEnumerable<ReleaseReference>) releases);
      this.BindString("@categoryName", categoryName, 128, true, SqlDbType.NVarChar);
      SqlDataReader reader = this.ExecuteReader();
      Dictionary<ReleaseReference, RunSummaryAndInsights> runSummaryAndInsightsMap = new Dictionary<ReleaseReference, RunSummaryAndInsights>();
      TestManagementDatabase2.FetchTestRunSummaryColumns runSummaryColumns = new TestManagementDatabase2.FetchTestRunSummaryColumns();
      TestManagementDatabase36.FetchTestRunsByStateColumns runsByStateColumns = new TestManagementDatabase36.FetchTestRunsByStateColumns();
      while (reader.Read())
      {
        ReleaseReference releaseReference = runsByStateColumns.bindRelease(reader);
        this.AddEmptySummaryEntryIfNotPresent(runSummaryAndInsightsMap, releaseReference);
        runSummaryAndInsightsMap[releaseReference].TestRunSummary.CurrentAggregatedRunsByState.Add(runsByStateColumns.bind(reader));
      }
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryForReleases");
      while (reader.Read())
      {
        ReleaseReference releaseReference = runSummaryColumns.bindRelease(reader);
        this.AddEmptySummaryEntryIfNotPresent(runSummaryAndInsightsMap, releaseReference);
        runSummaryAndInsightsMap[releaseReference].TestRunSummary.CurrentAggregateDataByOutcome.Add(runSummaryColumns.bind(reader));
      }
      if (!string.IsNullOrEmpty(categoryName))
      {
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryForReleases");
        while (reader.Read())
        {
          TestManagementDatabase19.FetchTestRunSummaryByCategoryColumns byCategoryColumns = new TestManagementDatabase19.FetchTestRunSummaryByCategoryColumns();
          ReleaseReference key = runSummaryColumns.bindRelease(reader);
          runSummaryAndInsightsMap[key].TestRunSummary.CurrentAggregateDataByReportingCategory.Add(byCategoryColumns.bind(reader));
        }
      }
      return runSummaryAndInsightsMap;
    }

    public override List<AggregatedDataForResultTrend> QueryTestResultTrendForBuild3(
      Guid projectId,
      TestResultTrendFilter filter,
      bool calculateEffectiveRunDuration,
      int runIdThreshold = 0)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultTrendForBuild");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@sourceWorkflow", filter.PublishContext, 128, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindIdTypeTable("@definitionIds", filter.DefinitionIds != null ? filter.DefinitionIds.Distinct<int>() : (IEnumerable<int>) null);
      this.BindBranchNameTypeTable("@branchNames", (IEnumerable<string>) filter.BranchNames);
      this.BindNameTypeTable("@runTitles", (IEnumerable<string>) filter.TestRunTitles);
      this.BindInt("@buildCount", filter.BuildCount);
      this.BindInt("@historyDays", filter.TrendDays);
      this.BindNullableDateTime("@maxCompleteDate", filter.MaxCompleteDate.Value);
      TestManagementDatabase36.QueryAggregatedResultsForBuild4 resultsForBuild4 = new TestManagementDatabase36.QueryAggregatedResultsForBuild4();
      Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> aggregatedResultsMap = new Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>>();
      Dictionary<int, SortedSet<RunSummaryByOutcome>> executionDetailsForTestRuns = new Dictionary<int, SortedSet<RunSummaryByOutcome>>();
      using (SqlDataReader reader = this.ExecuteReader())
      {
        while (reader.Read())
          resultsForBuild4.bindAggregatedResultsForBuild(reader, aggregatedResultsMap, executionDetailsForTestRuns);
      }
      foreach (KeyValuePair<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> keyValuePair in aggregatedResultsMap)
        keyValuePair.Value.Item2.Duration = TimeSpan.FromMilliseconds((double) TestManagementServiceUtility.CalculateEffectiveTestRunDuration(executionDetailsForTestRuns[keyValuePair.Key], calculateEffectiveRunDuration));
      return aggregatedResultsMap.Values.Select<Tuple<HashSet<int>, AggregatedDataForResultTrend>, AggregatedDataForResultTrend>((System.Func<Tuple<HashSet<int>, AggregatedDataForResultTrend>, AggregatedDataForResultTrend>) (a => a.Item2)).ToList<AggregatedDataForResultTrend>();
    }

    public override List<AggregatedDataForResultTrend> QueryTestResultTrendForRelease3(
      Guid projectId,
      TestResultTrendFilter filter,
      bool calculateEffectiveRunDuration,
      int runIdThreshold = 0)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultTrendForRelease");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindString("@sourceWorkflow", filter.PublishContext, 128, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      this.BindIdTypeTable("@definitionIds", filter.DefinitionIds != null ? filter.DefinitionIds.Distinct<int>() : (IEnumerable<int>) null);
      this.BindIdTypeTable("@envDefinitionIds", filter.EnvDefinitionIds != null ? filter.EnvDefinitionIds.Distinct<int>() : (IEnumerable<int>) null);
      this.BindNameTypeTable("@runTitles", (IEnumerable<string>) filter.TestRunTitles);
      this.BindInt("@releaseCount", filter.BuildCount);
      this.BindInt("@historyDays", filter.TrendDays);
      this.BindNullableDateTime("@maxCompleteDate", filter.MaxCompleteDate.Value);
      TestManagementDatabase36.QueryAggregatedResultsForRelease4 resultsForRelease4 = new TestManagementDatabase36.QueryAggregatedResultsForRelease4();
      Dictionary<int, SortedSet<RunSummaryByOutcome>> executionDetailsForTestRuns = new Dictionary<int, SortedSet<RunSummaryByOutcome>>();
      Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> aggregatedResultsMap = new Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>>();
      using (SqlDataReader reader = this.ExecuteReader())
      {
        while (reader.Read())
          resultsForRelease4.bindAggregatedResultsForRelease(reader, aggregatedResultsMap, executionDetailsForTestRuns);
      }
      foreach (KeyValuePair<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> keyValuePair in aggregatedResultsMap)
        keyValuePair.Value.Item2.Duration = TimeSpan.FromMilliseconds((double) TestManagementServiceUtility.CalculateEffectiveTestRunDuration(executionDetailsForTestRuns[keyValuePair.Key], calculateEffectiveRunDuration));
      return aggregatedResultsMap.Values.Select<Tuple<HashSet<int>, AggregatedDataForResultTrend>, AggregatedDataForResultTrend>((System.Func<Tuple<HashSet<int>, AggregatedDataForResultTrend>, AggregatedDataForResultTrend>) (a => a.Item2)).ToList<AggregatedDataForResultTrend>();
    }

    protected class QueryGroupedTestResultsColumns : IQueryGroupedTestResultsColumns
    {
      private SqlColumnBinder AutomatedTestStorage = new SqlColumnBinder(nameof (AutomatedTestStorage));
      private SqlColumnBinder Outcome = new SqlColumnBinder(nameof (Outcome));
      private SqlColumnBinder ResultCount = new SqlColumnBinder(nameof (ResultCount));
      private SqlColumnBinder AggregatedDuration = new SqlColumnBinder(nameof (AggregatedDuration));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestRunTitle = new SqlColumnBinder("Title");
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder CompleteDate = new SqlColumnBinder(nameof (CompleteDate));
      private SqlColumnBinder ReleaseEnvId = new SqlColumnBinder(nameof (ReleaseEnvId));
      private SqlColumnBinder Priority = new SqlColumnBinder(nameof (Priority));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder TestCaseRefId = new SqlColumnBinder(nameof (TestCaseRefId));
      private SqlColumnBinder OldTestCaseRefId = new SqlColumnBinder(nameof (OldTestCaseRefId));
      private SqlColumnBinder AutomatedTestName = new SqlColumnBinder(nameof (AutomatedTestName));
      private SqlColumnBinder TestCaseTitle = new SqlColumnBinder(nameof (TestCaseTitle));
      private SqlColumnBinder TestPointId = new SqlColumnBinder(nameof (TestPointId));
      private SqlColumnBinder SuiteId = new SqlColumnBinder(nameof (SuiteId));
      private SqlColumnBinder ParentSuiteId = new SqlColumnBinder(nameof (ParentSuiteId));
      private SqlColumnBinder ParentPlanId = new SqlColumnBinder(nameof (PlanId));
      private SqlColumnBinder TestSuiteTitle = new SqlColumnBinder("Title");
      private SqlColumnBinder PlanId = new SqlColumnBinder(nameof (PlanId));
      private SqlColumnBinder PointId = new SqlColumnBinder(nameof (PointId));
      private SqlColumnBinder WorkItemId = new SqlColumnBinder(nameof (WorkItemId));
      private SqlColumnBinder OwnerName = new SqlColumnBinder("TestCaseOwner");
      private SqlColumnBinder Duration = new SqlColumnBinder(nameof (Duration));

      public void bindPointDetails(
        SqlDataReader reader,
        Dictionary<int, Tuple<int, int>> pointDetails)
      {
        int int32_1 = this.PointId.GetInt32((IDataReader) reader);
        int int32_2 = this.PlanId.GetInt32((IDataReader) reader);
        int int32_3 = this.SuiteId.GetInt32((IDataReader) reader);
        pointDetails[int32_1] = new Tuple<int, int>(int32_2, int32_3);
      }

      public void bindSuiteDetails(
        SqlDataReader reader,
        Dictionary<int, Tuple<string, int, int>> suiteDetails)
      {
        int int32_1 = this.SuiteId.GetInt32((IDataReader) reader);
        int int32_2 = this.ParentSuiteId.GetInt32((IDataReader) reader);
        string str = this.TestSuiteTitle.GetString((IDataReader) reader, false);
        int int32_3 = this.ParentPlanId.GetInt32((IDataReader) reader);
        suiteDetails.Add(int32_1, new Tuple<string, int, int>(str, int32_2, int32_3));
      }

      public void bindAggregateValues(
        SqlDataReader reader,
        Dictionary<string, TestResultsDetailsForGroup> resultsMap,
        string groupByField,
        Dictionary<int, Tuple<string, int, int>> testSuiteDetails)
      {
        string empty1 = string.Empty;
        object obj = (object) string.Empty;
        if (!string.IsNullOrEmpty(groupByField))
        {
          if (string.Equals(ValidTestResultGroupByFields.Container, groupByField, StringComparison.OrdinalIgnoreCase))
          {
            empty1 = this.AutomatedTestStorage.GetString((IDataReader) reader, true);
            obj = (object) empty1;
          }
          else if (string.Equals(ValidTestResultGroupByFields.TestRun, groupByField, StringComparison.OrdinalIgnoreCase))
          {
            int int32_1 = this.TestRunId.GetInt32((IDataReader) reader);
            empty1 = int32_1.ToString();
            string str = this.State.ColumnExists((IDataReader) reader) ? ((Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState) this.State.GetByte((IDataReader) reader)).ToString() : (string) null;
            DateTime dateTime = this.CompleteDate.ColumnExists((IDataReader) reader) ? this.CompleteDate.GetDateTime((IDataReader) reader) : new DateTime();
            int int32_2 = this.ReleaseEnvId.ColumnExists((IDataReader) reader) ? this.ReleaseEnvId.GetInt32((IDataReader) reader) : 0;
            string artiFactUri = int32_2 > 0 ? TestManagementServiceUtility.GetArtiFactUri("Environment", "ReleaseManagement", int32_2.ToString()) : (string) null;
            obj = (object) new Microsoft.TeamFoundation.TestManagement.WebApi.TestRun()
            {
              Id = int32_1,
              Name = this.TestRunTitle.GetString((IDataReader) reader, false),
              State = str,
              CompletedDate = dateTime,
              ReleaseEnvironmentUri = artiFactUri
            };
          }
          else if (string.Equals(ValidTestResultGroupByFields.Priority, groupByField, StringComparison.OrdinalIgnoreCase))
          {
            empty1 = this.Priority.GetByte((IDataReader) reader).ToString();
            obj = (object) empty1;
          }
          else if (string.Equals(ValidTestResultGroupByFields.Requirement, groupByField, StringComparison.OrdinalIgnoreCase))
          {
            if (!this.WorkItemId.IsNull((IDataReader) reader))
            {
              int int32 = this.WorkItemId.GetInt32((IDataReader) reader);
              empty1 = int32.ToString();
              obj = (object) new WorkItemReference()
              {
                Id = int32.ToString()
              };
            }
            else
              obj = (object) new WorkItemReference();
          }
          else if (string.Equals(ValidTestResultGroupByFields.TestSuite, groupByField, StringComparison.OrdinalIgnoreCase))
          {
            if (!this.SuiteId.IsNull((IDataReader) reader))
            {
              int int32 = this.SuiteId.GetInt32((IDataReader) reader);
              string empty2 = string.Empty;
              empty1 = int32.ToString();
              int num1 = testSuiteDetails[int32].Item3;
              int num2 = testSuiteDetails[int32].Item2;
              if (num2 != 0)
                empty2 = testSuiteDetails[int32].Item1;
              obj = (object) new TestSuite()
              {
                Id = int32,
                Name = empty2,
                Parent = new ShallowReference()
                {
                  Id = num2.ToString()
                },
                Plan = new ShallowReference()
                {
                  Id = num1.ToString()
                }
              };
            }
            else
              obj = (object) new TestSuite()
              {
                Id = 0,
                Name = string.Empty,
                Parent = (ShallowReference) null,
                Plan = (ShallowReference) null
              };
          }
          else if (string.Equals(ValidTestResultGroupByFields.Owner, groupByField, StringComparison.OrdinalIgnoreCase))
          {
            empty1 = this.OwnerName.GetString((IDataReader) reader, true);
            obj = (object) empty1;
          }
        }
        if (empty1 == null)
        {
          empty1 = string.Empty;
          obj = (object) string.Empty;
        }
        if (!resultsMap.ContainsKey(empty1))
          resultsMap[empty1] = new TestResultsDetailsForGroup()
          {
            Results = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>(),
            ResultsCountByOutcome = (IDictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>) new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>()
          };
        byte key = this.Outcome.ColumnExists((IDataReader) reader) ? this.Outcome.GetByte((IDataReader) reader) : (byte) 0;
        int int32_3 = this.ResultCount.ColumnExists((IDataReader) reader) ? this.ResultCount.GetInt32((IDataReader) reader) : 0;
        long int64 = this.AggregatedDuration.ColumnExists((IDataReader) reader) ? this.AggregatedDuration.GetInt64((IDataReader) reader) : 0L;
        resultsMap[empty1].GroupByValue = obj;
        if (!resultsMap[empty1].ResultsCountByOutcome.ContainsKey((Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) key))
        {
          resultsMap[empty1].ResultsCountByOutcome[(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) key] = new AggregatedResultsByOutcome()
          {
            Outcome = (Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) key,
            Count = int32_3,
            Duration = TimeSpan.FromMilliseconds((double) int64)
          };
        }
        else
        {
          resultsMap[empty1].ResultsCountByOutcome[(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) key].Count += int32_3;
          resultsMap[empty1].ResultsCountByOutcome[(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) key].Duration = Validator.CheckOverflowAndGetSafeValue(resultsMap[empty1].ResultsCountByOutcome[(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) key].Duration, TimeSpan.FromMilliseconds((double) int64));
        }
      }

      public void bindResultDetails(
        SqlDataReader reader,
        Dictionary<object, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> resultsMap,
        IGroupByHelper groupByHelper)
      {
        string str1 = this.AutomatedTestStorage.ColumnExists((IDataReader) reader) ? this.AutomatedTestStorage.GetString((IDataReader) reader, true) : (string) null;
        int num = this.Priority.ColumnExists((IDataReader) reader) ? (int) this.Priority.GetByte((IDataReader) reader) : (int) byte.MaxValue;
        int int32_1 = this.TestRunId.GetInt32((IDataReader) reader);
        int int32_2 = this.TestResultId.GetInt32((IDataReader) reader);
        int int32_3 = this.TestCaseRefId.ColumnExists((IDataReader) reader) ? this.TestCaseRefId.GetInt32((IDataReader) reader) : 0;
        int int32_4 = this.TestPointId.ColumnExists((IDataReader) reader) ? this.TestPointId.GetInt32((IDataReader) reader) : 0;
        int int32_5 = !this.WorkItemId.ColumnExists((IDataReader) reader) || this.WorkItemId.IsNull((IDataReader) reader) ? 0 : this.WorkItemId.GetInt32((IDataReader) reader);
        string str2 = this.OwnerName.ColumnExists((IDataReader) reader) ? this.OwnerName.GetString((IDataReader) reader, true) : (string) null;
        string name = this.Outcome.ColumnExists((IDataReader) reader) ? Enum.GetName(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome), (object) this.Outcome.GetByte((IDataReader) reader)) : (string) null;
        long int64 = this.Duration.ColumnExists((IDataReader) reader) ? this.Duration.GetInt64((IDataReader) reader) : 0L;
        ResultDetailsColumns resultDetails = new ResultDetailsColumns()
        {
          AutomatedTestStorage = str1,
          TestRunId = int32_1,
          TestResultId = int32_2,
          TestCaseRefId = int32_3,
          TestPointId = int32_4,
          WorkItemId = int32_5,
          Priority = num,
          Owner = str2,
          Outcome = name,
          DurationInMs = int64
        };
        groupByHelper.ReadResultDetails(resultDetails, resultsMap);
      }

      public void bindRunDetails(
        SqlDataReader reader,
        Dictionary<string, TestResultsDetailsForGroup> resultsForGroupMap)
      {
        throw new NotImplementedException();
      }

      public void bindOldTestCaseRefDetails(
        SqlDataReader reader,
        Dictionary<int, int> oldTestCaseRefMap)
      {
        int int32_1 = this.TestCaseRefId.ColumnExists((IDataReader) reader) ? this.TestCaseRefId.GetInt32((IDataReader) reader) : 0;
        int int32_2 = this.OldTestCaseRefId.ColumnExists((IDataReader) reader) ? this.OldTestCaseRefId.GetInt32((IDataReader) reader) : 0;
        if (int32_2 <= 0)
          return;
        oldTestCaseRefMap[int32_2] = int32_1;
      }
    }

    protected class QueryGroupedTestResultsColumns3 : IQueryGroupedTestResultsColumns
    {
      private SqlColumnBinder AutomatedTestStorage = new SqlColumnBinder(nameof (AutomatedTestStorage));
      private SqlColumnBinder Outcome = new SqlColumnBinder(nameof (Outcome));
      private SqlColumnBinder ResultCount = new SqlColumnBinder(nameof (ResultCount));
      private SqlColumnBinder AggregatedDuration = new SqlColumnBinder(nameof (AggregatedDuration));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestRunTitle = new SqlColumnBinder("Title");
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder CompleteDate = new SqlColumnBinder(nameof (CompleteDate));
      private SqlColumnBinder StartedDate = new SqlColumnBinder(nameof (StartedDate));
      private SqlColumnBinder ReleaseEnvId = new SqlColumnBinder(nameof (ReleaseEnvId));
      private SqlColumnBinder Priority = new SqlColumnBinder(nameof (Priority));
      private SqlColumnBinder TestResultId = new SqlColumnBinder(nameof (TestResultId));
      private SqlColumnBinder TestCaseRefId = new SqlColumnBinder(nameof (TestCaseRefId));
      private SqlColumnBinder OldTestCaseRefId = new SqlColumnBinder(nameof (OldTestCaseRefId));
      private SqlColumnBinder AutomatedTestName = new SqlColumnBinder(nameof (AutomatedTestName));
      private SqlColumnBinder TestCaseTitle = new SqlColumnBinder(nameof (TestCaseTitle));
      private SqlColumnBinder TestPointId = new SqlColumnBinder(nameof (TestPointId));
      private SqlColumnBinder SuiteId = new SqlColumnBinder(nameof (SuiteId));
      private SqlColumnBinder ParentSuiteId = new SqlColumnBinder(nameof (ParentSuiteId));
      private SqlColumnBinder ParentPlanId = new SqlColumnBinder(nameof (PlanId));
      private SqlColumnBinder TestSuiteTitle = new SqlColumnBinder("Title");
      private SqlColumnBinder PlanId = new SqlColumnBinder(nameof (PlanId));
      private SqlColumnBinder PointId = new SqlColumnBinder(nameof (PointId));
      private SqlColumnBinder WorkItemId = new SqlColumnBinder(nameof (WorkItemId));
      private SqlColumnBinder OwnerName = new SqlColumnBinder("TestCaseOwner");
      private SqlColumnBinder Duration = new SqlColumnBinder(nameof (Duration));

      public void bindPointDetails(
        SqlDataReader reader,
        Dictionary<int, Tuple<int, int>> pointDetails)
      {
        int int32_1 = this.PointId.GetInt32((IDataReader) reader);
        int int32_2 = this.PlanId.GetInt32((IDataReader) reader);
        int int32_3 = this.SuiteId.GetInt32((IDataReader) reader);
        pointDetails[int32_1] = new Tuple<int, int>(int32_2, int32_3);
      }

      public void bindSuiteDetails(
        SqlDataReader reader,
        Dictionary<int, Tuple<string, int, int>> suiteDetails)
      {
        int int32_1 = this.SuiteId.GetInt32((IDataReader) reader);
        int int32_2 = this.ParentSuiteId.GetInt32((IDataReader) reader);
        string str = this.TestSuiteTitle.GetString((IDataReader) reader, false);
        int int32_3 = this.ParentPlanId.GetInt32((IDataReader) reader);
        suiteDetails.Add(int32_1, new Tuple<string, int, int>(str, int32_2, int32_3));
      }

      public void bindRunDetails(
        SqlDataReader reader,
        Dictionary<string, TestResultsDetailsForGroup> resultsForGroupMap)
      {
        int int32_1 = this.TestRunId.GetInt32((IDataReader) reader);
        string key = int32_1.ToString();
        string str = ((Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState) this.State.GetByte((IDataReader) reader)).ToString();
        DateTime dateTime1 = this.CompleteDate.GetDateTime((IDataReader) reader);
        DateTime dateTime2 = this.StartedDate.GetDateTime((IDataReader) reader);
        int int32_2 = this.ReleaseEnvId.GetInt32((IDataReader) reader, 0);
        string artiFactUri = int32_2 > 0 ? TestManagementServiceUtility.GetArtiFactUri("Environment", "ReleaseManagement", int32_2.ToString()) : (string) null;
        object obj = (object) new Microsoft.TeamFoundation.TestManagement.WebApi.TestRun()
        {
          Id = int32_1,
          Name = this.TestRunTitle.GetString((IDataReader) reader, false),
          State = str,
          CompletedDate = dateTime1,
          StartedDate = dateTime2,
          ReleaseEnvironmentUri = artiFactUri
        };
        if (!resultsForGroupMap.ContainsKey(key))
          resultsForGroupMap[key] = new TestResultsDetailsForGroup()
          {
            Results = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>(),
            ResultsCountByOutcome = (IDictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>) new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>()
          };
        resultsForGroupMap[key].GroupByValue = obj;
      }

      public void bindAggregateValues(
        SqlDataReader reader,
        Dictionary<string, TestResultsDetailsForGroup> resultsForGroupMap,
        string groupByField,
        Dictionary<int, Tuple<string, int, int>> testSuiteDetails)
      {
        string empty1 = string.Empty;
        object obj = (object) string.Empty;
        if (!string.IsNullOrEmpty(groupByField))
        {
          if (string.Equals(ValidTestResultGroupByFields.Container, groupByField, StringComparison.OrdinalIgnoreCase))
          {
            empty1 = this.AutomatedTestStorage.GetString((IDataReader) reader, true);
            obj = (object) empty1;
          }
          else if (string.Equals(ValidTestResultGroupByFields.TestRun, groupByField, StringComparison.OrdinalIgnoreCase))
            empty1 = this.TestRunId.GetInt32((IDataReader) reader).ToString();
          else if (string.Equals(ValidTestResultGroupByFields.Priority, groupByField, StringComparison.OrdinalIgnoreCase))
          {
            empty1 = this.Priority.GetByte((IDataReader) reader).ToString();
            obj = (object) empty1;
          }
          else if (string.Equals(ValidTestResultGroupByFields.Requirement, groupByField, StringComparison.OrdinalIgnoreCase))
          {
            if (!this.WorkItemId.IsNull((IDataReader) reader))
            {
              int int32 = this.WorkItemId.GetInt32((IDataReader) reader);
              empty1 = int32.ToString();
              obj = (object) new WorkItemReference()
              {
                Id = int32.ToString()
              };
            }
            else
              obj = (object) new WorkItemReference();
          }
          else if (string.Equals(ValidTestResultGroupByFields.TestSuite, groupByField, StringComparison.OrdinalIgnoreCase))
          {
            if (!this.SuiteId.IsNull((IDataReader) reader))
            {
              int int32 = this.SuiteId.GetInt32((IDataReader) reader);
              string empty2 = string.Empty;
              empty1 = int32.ToString();
              int num1 = testSuiteDetails[int32].Item3;
              int num2 = testSuiteDetails[int32].Item2;
              if (num2 != 0)
                empty2 = testSuiteDetails[int32].Item1;
              obj = (object) new TestSuite()
              {
                Id = int32,
                Name = empty2,
                Parent = new ShallowReference()
                {
                  Id = num2.ToString()
                },
                Plan = new ShallowReference()
                {
                  Id = num1.ToString()
                }
              };
            }
            else
              obj = (object) new TestSuite()
              {
                Id = 0,
                Name = string.Empty,
                Parent = (ShallowReference) null,
                Plan = (ShallowReference) null
              };
          }
          else if (string.Equals(ValidTestResultGroupByFields.Owner, groupByField, StringComparison.OrdinalIgnoreCase))
          {
            empty1 = this.OwnerName.GetString((IDataReader) reader, true);
            obj = (object) empty1;
          }
        }
        if (empty1 == null)
        {
          empty1 = string.Empty;
          obj = (object) string.Empty;
        }
        if (!resultsForGroupMap.ContainsKey(empty1))
          resultsForGroupMap[empty1] = new TestResultsDetailsForGroup()
          {
            Results = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>(),
            ResultsCountByOutcome = (IDictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>) new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>()
          };
        byte key = this.Outcome.ColumnExists((IDataReader) reader) ? this.Outcome.GetByte((IDataReader) reader) : (byte) 0;
        int int32_1 = this.ResultCount.ColumnExists((IDataReader) reader) ? this.ResultCount.GetInt32((IDataReader) reader) : 0;
        long int64 = this.AggregatedDuration.ColumnExists((IDataReader) reader) ? this.AggregatedDuration.GetInt64((IDataReader) reader) : 0L;
        resultsForGroupMap[empty1].GroupByValue = obj;
        if (!resultsForGroupMap[empty1].ResultsCountByOutcome.ContainsKey((Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) key))
        {
          resultsForGroupMap[empty1].ResultsCountByOutcome[(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) key] = new AggregatedResultsByOutcome()
          {
            Outcome = (Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) key,
            Count = int32_1,
            Duration = TimeSpan.FromMilliseconds((double) int64)
          };
        }
        else
        {
          resultsForGroupMap[empty1].ResultsCountByOutcome[(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) key].Count += int32_1;
          resultsForGroupMap[empty1].ResultsCountByOutcome[(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) key].Duration = Validator.CheckOverflowAndGetSafeValue(resultsForGroupMap[empty1].ResultsCountByOutcome[(Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) key].Duration, TimeSpan.FromMilliseconds((double) int64));
        }
      }

      public void bindResultDetails(
        SqlDataReader reader,
        Dictionary<object, Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> resultsMap,
        IGroupByHelper groupByHelper)
      {
        string str1 = this.AutomatedTestStorage.ColumnExists((IDataReader) reader) ? this.AutomatedTestStorage.GetString((IDataReader) reader, true) : (string) null;
        int num = this.Priority.ColumnExists((IDataReader) reader) ? (int) this.Priority.GetByte((IDataReader) reader) : (int) byte.MaxValue;
        int int32_1 = this.TestRunId.GetInt32((IDataReader) reader);
        int int32_2 = this.TestResultId.GetInt32((IDataReader) reader);
        int int32_3 = this.TestCaseRefId.ColumnExists((IDataReader) reader) ? this.TestCaseRefId.GetInt32((IDataReader) reader) : 0;
        int int32_4 = this.TestPointId.ColumnExists((IDataReader) reader) ? this.TestPointId.GetInt32((IDataReader) reader) : 0;
        int int32_5 = !this.WorkItemId.ColumnExists((IDataReader) reader) || this.WorkItemId.IsNull((IDataReader) reader) ? 0 : this.WorkItemId.GetInt32((IDataReader) reader);
        string str2 = this.OwnerName.ColumnExists((IDataReader) reader) ? this.OwnerName.GetString((IDataReader) reader, true) : (string) null;
        string name = this.Outcome.ColumnExists((IDataReader) reader) ? Enum.GetName(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome), (object) this.Outcome.GetByte((IDataReader) reader)) : (string) null;
        long int64 = this.Duration.ColumnExists((IDataReader) reader) ? this.Duration.GetInt64((IDataReader) reader) : 0L;
        ResultDetailsColumns resultDetails = new ResultDetailsColumns()
        {
          AutomatedTestStorage = str1,
          TestRunId = int32_1,
          TestResultId = int32_2,
          TestCaseRefId = int32_3,
          TestPointId = int32_4,
          WorkItemId = int32_5,
          Priority = num,
          Owner = str2,
          Outcome = name,
          DurationInMs = int64
        };
        groupByHelper.ReadResultDetails(resultDetails, resultsMap);
      }

      public void bindOldTestCaseRefDetails(
        SqlDataReader reader,
        Dictionary<int, int> oldTestCaseRefMap)
      {
        int int32_1 = this.TestCaseRefId.ColumnExists((IDataReader) reader) ? this.TestCaseRefId.GetInt32((IDataReader) reader) : 0;
        int int32_2 = this.OldTestCaseRefId.ColumnExists((IDataReader) reader) ? this.OldTestCaseRefId.GetInt32((IDataReader) reader) : 0;
        if (int32_2 <= 0)
          return;
        oldTestCaseRefMap[int32_2] = int32_1;
      }
    }

    protected class FetchTestRunsByStateColumns
    {
      private SqlColumnBinder RunState = new SqlColumnBinder("State");
      private SqlColumnBinder RunsCount = new SqlColumnBinder(nameof (RunsCount));
      private SqlColumnBinder ReleaseId = new SqlColumnBinder(nameof (ReleaseId));
      private SqlColumnBinder ReleaseEnvId = new SqlColumnBinder(nameof (ReleaseEnvId));

      internal RunSummaryByState bind(SqlDataReader reader) => new RunSummaryByState()
      {
        RunState = (Microsoft.TeamFoundation.TestManagement.Client.TestRunState) this.RunState.GetByte((IDataReader) reader),
        RunsCount = this.RunsCount.GetInt32((IDataReader) reader)
      };

      internal ReleaseReference bindRelease(SqlDataReader reader) => new ReleaseReference()
      {
        ReleaseId = this.ReleaseId.ColumnExists((IDataReader) reader) ? this.ReleaseId.GetInt32((IDataReader) reader) : 0,
        ReleaseEnvId = this.ReleaseEnvId.ColumnExists((IDataReader) reader) ? this.ReleaseEnvId.GetInt32((IDataReader) reader) : 0
      };
    }

    protected class QueryAggregatedResultsForBuild4
    {
      private SqlColumnBinder BuildId = new SqlColumnBinder(nameof (BuildId));
      private SqlColumnBinder BuildNumber = new SqlColumnBinder(nameof (BuildNumber));
      private SqlColumnBinder BuildUri = new SqlColumnBinder(nameof (BuildUri));
      private SqlColumnBinder DefinitionId = new SqlColumnBinder("BuildDefinitionId");
      private SqlColumnBinder BranchName = new SqlColumnBinder(nameof (BranchName));
      private SqlColumnBinder TestOutcome = new SqlColumnBinder(nameof (TestOutcome));
      private SqlColumnBinder ResultCount = new SqlColumnBinder(nameof (ResultCount));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestRunState = new SqlColumnBinder(nameof (TestRunState));
      private SqlColumnBinder RunDuration = new SqlColumnBinder(nameof (RunDuration));
      private SqlColumnBinder CompleteDate = new SqlColumnBinder(nameof (CompleteDate));

      internal void bindAggregatedResultsForBuild(
        SqlDataReader reader,
        Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> aggregatedResultsMap,
        Dictionary<int, SortedSet<RunSummaryByOutcome>> executionDetailsForTestRuns)
      {
        int int32_1 = this.BuildId.GetInt32((IDataReader) reader);
        string str1 = this.BuildUri.GetString((IDataReader) reader, false);
        string str2 = this.BuildNumber.GetString((IDataReader) reader, true);
        int int32_2 = this.DefinitionId.GetInt32((IDataReader) reader);
        string str3 = this.BranchName.GetString((IDataReader) reader, true);
        Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome key1 = (Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) this.TestOutcome.GetByte((IDataReader) reader, (byte) 0);
        int int32_3 = this.ResultCount.GetInt32((IDataReader) reader, 0);
        int int32_4 = this.TestRunId.GetInt32((IDataReader) reader);
        Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState key2 = (Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState) this.TestRunState.GetByte((IDataReader) reader);
        long int64 = Convert.ToInt64(this.RunDuration.GetObject((IDataReader) reader));
        DateTime dateTime = this.CompleteDate.GetDateTime((IDataReader) reader);
        if (!aggregatedResultsMap.ContainsKey(int32_1))
        {
          aggregatedResultsMap[int32_1] = new Tuple<HashSet<int>, AggregatedDataForResultTrend>(new HashSet<int>(), new AggregatedDataForResultTrend()
          {
            TestResultsContext = new TestResultsContext()
            {
              ContextType = TestResultsContextType.Build,
              Build = new BuildReference()
              {
                Id = int32_1,
                Uri = str1,
                Number = str2,
                DefinitionId = int32_2,
                BranchName = str3
              }
            },
            ResultsByOutcome = (IDictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>) new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>(),
            RunSummaryByState = (IDictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState, AggregatedRunsByState>) new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState, AggregatedRunsByState>(),
            Duration = new TimeSpan()
          });
          executionDetailsForTestRuns[int32_1] = new SortedSet<RunSummaryByOutcome>((IComparer<RunSummaryByOutcome>) new TestRunComparer());
        }
        if (int32_3 > 0)
        {
          if (!aggregatedResultsMap[int32_1].Item2.ResultsByOutcome.ContainsKey(key1))
            aggregatedResultsMap[int32_1].Item2.ResultsByOutcome[key1] = new AggregatedResultsByOutcome()
            {
              Count = 0,
              Outcome = key1
            };
          aggregatedResultsMap[int32_1].Item2.ResultsByOutcome[key1].Count += int32_3;
        }
        if (aggregatedResultsMap[int32_1].Item1.Contains(int32_4))
          return;
        aggregatedResultsMap[int32_1].Item1.Add(int32_4);
        if (!aggregatedResultsMap[int32_1].Item2.RunSummaryByState.ContainsKey(key2))
          aggregatedResultsMap[int32_1].Item2.RunSummaryByState[key2] = new AggregatedRunsByState()
          {
            State = key2,
            RunsCount = 1
          };
        else
          ++aggregatedResultsMap[int32_1].Item2.RunSummaryByState[key2].RunsCount;
        executionDetailsForTestRuns[int32_1].Add(new RunSummaryByOutcome()
        {
          TestRunId = int32_4,
          RunDuration = int64,
          RunCompletedDate = dateTime
        });
      }
    }

    protected class QueryAggregatedResultsForRelease4
    {
      private SqlColumnBinder ReleaseId = new SqlColumnBinder(nameof (ReleaseId));
      private SqlColumnBinder ReleaseName = new SqlColumnBinder(nameof (ReleaseName));
      private SqlColumnBinder DefinitionId = new SqlColumnBinder("ReleaseDefId");
      private SqlColumnBinder TestOutcome = new SqlColumnBinder(nameof (TestOutcome));
      private SqlColumnBinder ResultCount = new SqlColumnBinder(nameof (ResultCount));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestRunState = new SqlColumnBinder(nameof (TestRunState));
      private SqlColumnBinder RunDuration = new SqlColumnBinder(nameof (RunDuration));
      private SqlColumnBinder CompleteDate = new SqlColumnBinder(nameof (CompleteDate));

      internal void bindAggregatedResultsForRelease(
        SqlDataReader reader,
        Dictionary<int, Tuple<HashSet<int>, AggregatedDataForResultTrend>> aggregatedResultsMap,
        Dictionary<int, SortedSet<RunSummaryByOutcome>> executionDetailsForTestRuns)
      {
        int int32_1 = this.ReleaseId.GetInt32((IDataReader) reader);
        string str = this.ReleaseName.GetString((IDataReader) reader, true);
        int int32_2 = this.DefinitionId.GetInt32((IDataReader) reader);
        Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome key1 = (Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome) this.TestOutcome.GetByte((IDataReader) reader, (byte) 0);
        int int32_3 = this.ResultCount.GetInt32((IDataReader) reader, 0);
        int int32_4 = this.TestRunId.GetInt32((IDataReader) reader);
        Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState key2 = (Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState) this.TestRunState.GetByte((IDataReader) reader);
        long int64 = Convert.ToInt64(this.RunDuration.GetObject((IDataReader) reader));
        DateTime dateTime = this.CompleteDate.GetDateTime((IDataReader) reader);
        if (!aggregatedResultsMap.ContainsKey(int32_1))
        {
          aggregatedResultsMap[int32_1] = new Tuple<HashSet<int>, AggregatedDataForResultTrend>(new HashSet<int>(), new AggregatedDataForResultTrend()
          {
            TestResultsContext = new TestResultsContext()
            {
              ContextType = TestResultsContextType.Release,
              Release = new Microsoft.TeamFoundation.TestManagement.WebApi.ReleaseReference()
              {
                Id = int32_1,
                Name = str,
                DefinitionId = int32_2
              }
            },
            ResultsByOutcome = (IDictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>) new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome, AggregatedResultsByOutcome>(),
            RunSummaryByState = (IDictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState, AggregatedRunsByState>) new Dictionary<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunState, AggregatedRunsByState>(),
            Duration = new TimeSpan()
          });
          executionDetailsForTestRuns[int32_1] = new SortedSet<RunSummaryByOutcome>((IComparer<RunSummaryByOutcome>) new TestRunComparer());
        }
        if (int32_3 > 0)
        {
          if (!aggregatedResultsMap[int32_1].Item2.ResultsByOutcome.ContainsKey(key1))
            aggregatedResultsMap[int32_1].Item2.ResultsByOutcome[key1] = new AggregatedResultsByOutcome()
            {
              Count = 0,
              Outcome = key1
            };
          aggregatedResultsMap[int32_1].Item2.ResultsByOutcome[key1].Count += int32_3;
        }
        if (aggregatedResultsMap[int32_1].Item1.Contains(int32_4))
          return;
        aggregatedResultsMap[int32_1].Item1.Add(int32_4);
        if (!aggregatedResultsMap[int32_1].Item2.RunSummaryByState.ContainsKey(key2))
          aggregatedResultsMap[int32_1].Item2.RunSummaryByState[key2] = new AggregatedRunsByState()
          {
            State = key2,
            RunsCount = 1
          };
        else
          ++aggregatedResultsMap[int32_1].Item2.RunSummaryByState[key2].RunsCount;
        executionDetailsForTestRuns[int32_1].Add(new RunSummaryByOutcome()
        {
          TestRunId = int32_4,
          RunDuration = int64,
          RunCompletedDate = dateTime
        });
      }
    }
  }
}
