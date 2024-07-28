// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase63
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
  public class TestManagementDatabase63 : TestManagementDatabase62
  {
    internal TestManagementDatabase63(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase63()
    {
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
        this.BindBuildRefTypeTable4("@buildRefData", (IEnumerable<BuildConfiguration>) builds);
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
        List<PipelineReference> pipelineReferenceList;
        if (testRun.PipelineReference == null)
        {
          pipelineReferenceList = (List<PipelineReference>) null;
        }
        else
        {
          pipelineReferenceList = new List<PipelineReference>();
          pipelineReferenceList.Add(testRun.PipelineReference);
        }
        this.BindPipelineRefTable("@pipelineRefData", (IEnumerable<PipelineReference>) pipelineReferenceList);
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

    public override UpdatedRunProperties UpdateTestRun(
      Guid projectId,
      TestRun run,
      Guid updatedBy,
      ReleaseReference releaseRef = null,
      BuildConfiguration buildRef = null,
      bool skipRunStateTransitionCheck = false)
    {
      this.PrepareStoredProcedure("TestResult.prc_UpdateTestRun");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", run.TestRunId);
      this.BindStringPreserveNull("@title", run.Title, 256, SqlDbType.NVarChar);
      this.BindGuidPreserveNull("@owner", run.Owner);
      this.BindByte("@state", run.State, (byte) 0);
      this.BindNullableDateTime("@dueDate", run.DueDate);
      this.BindNullableInt("@iterationId", run.IterationId, 0);
      this.BindStringPreserveNull("@controller", run.Controller, 256, SqlDbType.NVarChar);
      this.BindStringPreserveNull("@errorMessage", run.ErrorMessage, 512, SqlDbType.NVarChar);
      this.BindNullableDateTime("@dateStarted", run.StartDate);
      this.BindNullableDateTime("@dateCompleted", run.CompleteDate);
      this.BindInt("@testMessageLogId", run.TestMessageLogId);
      this.BindInt("@testSettingsId", run.TestSettingsId);
      this.BindInt("@publicTestSettingsId", run.PublicTestSettingsId);
      this.BindGuid("@testEnvironmentId", run.TestEnvironmentId);
      this.BindGuid("@lastUpdatedBy", updatedBy);
      this.BindByte("@postProcessState", run.PostProcessState, (byte) 0);
      this.BindInt("@version", run.Version);
      this.BindInt("@revision", run.Revision);
      this.BindBoolean("@isBvt", run.IsBvt);
      this.BindStringPreserveNull("@comment", run.Comment, 1048576, SqlDbType.NVarChar);
      this.BindByte("@substate", run.Substate, (byte) 0);
      this.BindString("@testEnvironmentUrl", !run.RunHasDtlEnvironment || run.DtlTestEnvironment == null ? (string) null : run.DtlTestEnvironment.Url, 2048, true, SqlDbType.NVarChar);
      this.BindString("@autEnvironmentUrl", !run.RunHasDtlEnvironment || run.DtlAutEnvironment == null ? (string) null : run.DtlAutEnvironment.Url, 2048, true, SqlDbType.NVarChar);
      this.BindString("@dtlCsmParameters", !run.RunHasDtlEnvironment || run.CsmParameters == null ? (string) null : run.CsmParameters, 2048, true, SqlDbType.NVarChar);
      this.BindBoolean("@skipRunStateTransitionCheck", skipRunStateTransitionCheck);
      this.BindString("@dropLocation", run.DropLocation, 260, true, SqlDbType.NVarChar);
      List<BuildConfiguration> builds;
      if (buildRef == null)
      {
        builds = (List<BuildConfiguration>) null;
      }
      else
      {
        builds = new List<BuildConfiguration>();
        builds.Add(buildRef);
      }
      this.BindBuildRefTypeTable4("@buildRefData", (IEnumerable<BuildConfiguration>) builds);
      List<ReleaseReference> releases;
      if (releaseRef == null)
      {
        releases = (List<ReleaseReference>) null;
      }
      else
      {
        releases = new List<ReleaseReference>();
        releases.Add(releaseRef);
      }
      this.BindReleaseRefTypeTable4("@releaseRefData", (IEnumerable<ReleaseReference>) releases);
      this.BindPipelineRefTable("@pipelineRefData", (IEnumerable<PipelineReference>) null);
      this.BindString("@sourceWorkflow", run.SourceWorkflow, 128, false, SqlDbType.NVarChar);
      SqlDataReader reader = this.ExecuteReader();
      UpdatedRunProperties updatedRunProperties = reader.Read() ? new TestManagementDatabase60.UpdatedPropertyColumns2().bindUpdatedRunProperties(reader) : throw new UnexpectedDatabaseResultException("prc_UpdateTestRun");
      updatedRunProperties.LastUpdatedBy = updatedBy;
      if (updatedRunProperties.IsRunCompleted)
        run.CompleteDate = updatedRunProperties.CompleteDate;
      return updatedRunProperties;
    }

    internal override List<TestCaseResult> QueryTestResultsByPipeline(
      Guid projectId,
      PipelineReference pipelineReference,
      IList<byte> runStates,
      bool fetchFailedTestsOnly,
      int continuationTokenRunId,
      int continuationTokenResultId,
      int top)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultsByPipeline");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@pipelineId", pipelineReference.PipelineId);
      this.BindString("@stageName", pipelineReference.StageReference?.StageName, 256, true, SqlDbType.NVarChar);
      this.BindString("@phaseName", pipelineReference.PhaseReference?.PhaseName, 256, true, SqlDbType.NVarChar);
      this.BindString("@jobName", pipelineReference.JobReference?.JobName, 256, true, SqlDbType.NVarChar);
      this.BindBoolean("@fetchOnlyFailedTests", fetchFailedTestsOnly);
      this.BindTestManagement_TinyIntTypeTable("@runStates", (IEnumerable<byte>) runStates);
      this.BindInt("@continuationTokenRunId", continuationTokenRunId);
      this.BindInt("@continuationTokenResultId", continuationTokenResultId);
      this.BindInt("@top", top);
      SqlDataReader reader = this.ExecuteReader();
      List<TestCaseResult> testResults = new List<TestCaseResult>();
      if (reader.HasRows)
        TestManagementDatabase36.GetTestResultsBind(reader, testResults);
      return testResults;
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
          int int32 = new SqlColumnBinder("TestPlanId").GetInt32((IDataReader) reader, 0);
          if (int32 != 0)
          {
            foreach (ResultUpdateResponse resultUpdateResponse in dictionary.Values)
              resultUpdateResponse.TestPlanId = int32;
          }
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
  }
}
