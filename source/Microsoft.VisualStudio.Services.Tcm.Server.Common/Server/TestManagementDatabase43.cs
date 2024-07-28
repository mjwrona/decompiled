// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase43
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase43 : TestManagementDatabase42
  {
    internal TestManagementDatabase43(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase43()
    {
    }

    public override int[] CreateTestResultsExtension2(
      Guid projectId,
      int testRunId,
      IEnumerable<TestCaseResult> results,
      int passedTestsCount,
      Guid updatedBy,
      bool isTcmService)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.CreateTestResultsExtension2");
        this.PrepareStoredProcedure("TestResult.prc_CreateTestResultsExt2");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@testRunId", testRunId);
        this.BindTestResult_TestCaseResult2TypeTable("@testresultsTable", results, true);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindTestExtensionFieldValuesTypeTable("@additionalFields", (IEnumerable<Tuple<int, int, TestExtensionField>>) this.GetExtensionFieldsMap(testRunId, results, true));
        this.BindInt("@passedTestsCount", passedTestsCount);
        this.BindBoolean("@isTcmService", isTcmService);
        SqlDataReader reader = this.ExecuteReader();
        SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("TestResultId");
        List<int> intList = new List<int>(results.Count<TestCaseResult>());
        while (reader.Read())
          intList.Add(sqlColumnBinder.GetInt32((IDataReader) reader, 0));
        intList.Sort();
        return intList.ToArray();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.CreateTestResultsExtension2");
      }
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
        this.BindBoolean("@isTcmService", isTcmService);
        SqlDataReader reader = this.ExecuteReader();
        return reader.Read() ? new TestManagementDatabase13.CreateTestRunColumns().bind(reader) : throw new UnexpectedDatabaseResultException("prc_CreateTestRun");
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    public override void CreateTestResults(
      Guid projectId,
      int testRunId,
      IEnumerable<TestCaseResult> results,
      Guid updatedBy,
      bool updateRunSummary,
      bool isTcmService)
    {
      this.PrepareStoredProcedure("TestResult.prc_CreateTestResults");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", testRunId);
      this.BindTestResult_TestCaseResult5TypeTable("@testresultsTable", results);
      this.BindGuid("@lastUpdatedBy", updatedBy);
      this.BindTestExtensionFieldValuesTypeTable("@additionalFields", (IEnumerable<Tuple<int, int, TestExtensionField>>) this.GetExtensionFieldsMap(testRunId, results, false));
      this.BindInt("@testResultStartId", 0);
      this.BindBoolean("@updateRunSummary", updateRunSummary);
      this.BindBoolean("@isTcmService", isTcmService);
      this.ExecuteNonQuery();
    }

    public override List<TestCaseResult> GetTestCaseResultsByPointIds(
      Guid projectId,
      int planId,
      List<int> pointIds)
    {
      this.PrepareStoredProcedure("TestResult.prc_GetTestResultsByPointIds");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@planId", planId);
      this.BindInt32TypeTable("@pointIds", (IEnumerable<int>) pointIds);
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.FetchTestResultsColumns testResultsColumns = new TestManagementDatabase.FetchTestResultsColumns();
      List<TestCaseResult> resultsByPointIds = new List<TestCaseResult>();
      while (reader.Read())
        resultsByPointIds.Add(testResultsColumns.bind(reader));
      return resultsByPointIds;
    }

    internal override List<ResultUpdateResponse> UpdateTestResults2(
      Guid projectId,
      int testRunId,
      IEnumerable<ResultUpdateRequest> resultsForUpdate,
      Guid updatedBy,
      bool autoComputeTestRunState,
      bool updateRunSummary,
      bool isTcmService,
      out string iterationUri,
      out Guid runProjGuid,
      out TestRun run)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.UpdateTestResults2");
        iterationUri = string.Empty;
        runProjGuid = Guid.Empty;
        run = (TestRun) null;
        TestActionResultUpdateComparator comparer1 = new TestActionResultUpdateComparator();
        TestCaseResultUpdateResultComparator comparer2 = new TestCaseResultUpdateResultComparator();
        TestResultParameterUpdateResultComparator comparer3 = new TestResultParameterUpdateResultComparator();
        HashSet<TestCaseResult> results = new HashSet<TestCaseResult>((IEqualityComparer<TestCaseResult>) comparer2);
        HashSet<TestActionResult> testActionResultSet1 = new HashSet<TestActionResult>((IEqualityComparer<TestActionResult>) comparer1);
        HashSet<TestActionResult> testActionResultSet2 = new HashSet<TestActionResult>((IEqualityComparer<TestActionResult>) comparer1);
        HashSet<TestResultParameter> testResultParameterSet1 = new HashSet<TestResultParameter>((IEqualityComparer<TestResultParameter>) comparer3);
        HashSet<TestResultParameter> testResultParameterSet2 = new HashSet<TestResultParameter>((IEqualityComparer<TestResultParameter>) comparer3);
        Dictionary<int, ResultUpdateResponse> dictionary = new Dictionary<int, ResultUpdateResponse>();
        List<ResultUpdateResponse> resultUpdateResponseList = new List<ResultUpdateResponse>();
        if (!this.GetTestCaseResultsFromResultUpdateRequests(resultsForUpdate, resultUpdateResponseList, results, testActionResultSet1, testActionResultSet2, testResultParameterSet1, testResultParameterSet2))
          return resultUpdateResponseList;
        if (results.Count == 1)
          this.PrepareStoredProcedure("TestResult.prc_UpdateTestResultsSingularly2");
        else
          this.PrepareStoredProcedure("TestResult.prc_UpdateTestResults2");
        this.BindTestResult_TestCaseResultUpdate3TypeTable("@resultsTable", (IEnumerable<TestCaseResult>) results);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@testRunId", testRunId);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindTestActionResultUpdateTypeTable("@actionsUpdatedTable", (IEnumerable<TestActionResult>) testActionResultSet1);
        this.BindTestActionResultForDeleteTypeTable("@actionsDeletedTable", (IEnumerable<TestActionResult>) testActionResultSet2);
        this.BindTestResultParameterTypeTable("@parametersUpdatedTable", (IEnumerable<TestResultParameter>) testResultParameterSet1);
        this.BindTestResultParameterForDeleteTypeTable("@parametersDeletedTable", (IEnumerable<TestResultParameter>) testResultParameterSet2);
        this.BindBoolean("@autoComputeTestRunState", autoComputeTestRunState);
        this.BindTestExtensionFieldValuesTypeTable("@additionalResultFields", (IEnumerable<Tuple<int, int, TestExtensionField>>) this.GetExtensionFieldsMap(testRunId, (IEnumerable<TestCaseResult>) results, false));
        this.BindBoolean("@updateRunSummary", updateRunSummary);
        this.BindBoolean("@isTcmService", isTcmService);
        SqlDataReader reader = this.ExecuteReader();
        if (!reader.Read())
          throw new UnexpectedDatabaseResultException("prc_UpdateTestResults2");
        TestManagementDatabase.UpdateTestResultColumns testResultColumns = new TestManagementDatabase.UpdateTestResultColumns();
        do
        {
          ResultUpdateResponse resultUpdateResponse = testResultColumns.bind(reader);
          dictionary.Add(resultUpdateResponse.TestResultId, resultUpdateResponse);
        }
        while (reader.Read());
        TestManagementDatabase39.QueryTestResultMaxSubResultIds resultMaxSubResultIds = new TestManagementDatabase39.QueryTestResultMaxSubResultIds();
        if (reader.NextResult() && reader.Read())
        {
          do
          {
            int testResultId;
            int num = resultMaxSubResultIds.bind(reader, out testResultId);
            if (dictionary.ContainsKey(testResultId))
              dictionary[testResultId].MaxReservedSubResultId = num;
          }
          while (reader.Read());
        }
        if (reader.NextResult() && reader.Read())
        {
          TestManagementDatabase8.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase8.QueryTestRunColumns();
          int dataspaceId;
          run = queryTestRunColumns.bind(reader, out dataspaceId, out iterationUri);
          runProjGuid = this.GetDataspaceIdentifier(dataspaceId);
        }
        if (resultUpdateResponseList != null && resultUpdateResponseList.Count<ResultUpdateResponse>() > 0)
        {
          foreach (ResultUpdateResponse resultUpdateResponse in resultUpdateResponseList)
          {
            if (!dictionary.ContainsKey(resultUpdateResponse.TestResultId))
              dictionary.Add(resultUpdateResponse.TestResultId, resultUpdateResponse);
          }
        }
        return dictionary.Values.ToList<ResultUpdateResponse>();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.UpdateTestResults2");
      }
    }

    private static TestCaseResultIdentifier GetLastResultIdentifier(SqlDataReader reader)
    {
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("LastRunId");
      SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("LastResultId");
      return reader.Read() ? new TestCaseResultIdentifier(sqlColumnBinder1.GetInt32((IDataReader) reader), sqlColumnBinder2.GetInt32((IDataReader) reader)) : (TestCaseResultIdentifier) null;
    }

    private static void GetTestResultsGroupResponse(
      SqlDataReader reader,
      TestResultsGroupsData testResultsGroupsData)
    {
      SqlColumnBinder sqlColumnBinder1 = new SqlColumnBinder("AutomatedTestStorage");
      SqlColumnBinder sqlColumnBinder2 = new SqlColumnBinder("Owner");
      while (reader.Read())
      {
        testResultsGroupsData.AutomatedTestStorage.Add(sqlColumnBinder1.GetString((IDataReader) reader, true));
        testResultsGroupsData.Owner.Add(sqlColumnBinder2.GetString((IDataReader) reader, true));
      }
    }

    internal override TestRun GetTestRunBasic(Guid projectId, int runId)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.GetTestRunBasic");
        this.PrepareStoredProcedure("prc_GetTestRunBasic");
        this.BindInt("@testRunId", runId);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        SqlDataReader reader = this.ExecuteReader();
        TestManagementDatabase8.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase8.QueryTestRunColumns();
        TestRun testRunBasic = (TestRun) null;
        if (reader.Read())
          testRunBasic = queryTestRunColumns.bind(reader, out int _, out string _);
        return testRunBasic;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.GetTestRunBasic");
      }
    }

    public override List<TestMessageLogEntry> QueryLogEntriesForRun(
      Guid projectId,
      int testRunId,
      int testMessageLogId)
    {
      List<TestMessageLogEntry> testMessageLogEntryList = new List<TestMessageLogEntry>();
      this.PrepareStoredProcedure("prc_QueryTestMessageLogEntriesForRun");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", testRunId);
      this.BindInt("@testMessageLogId", testMessageLogId);
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.QueryTestMessageLogentryColumns messageLogentryColumns = new TestManagementDatabase.QueryTestMessageLogentryColumns();
      while (reader.Read())
        testMessageLogEntryList.Add(messageLogentryColumns.bind(reader));
      return testMessageLogEntryList;
    }
  }
}
