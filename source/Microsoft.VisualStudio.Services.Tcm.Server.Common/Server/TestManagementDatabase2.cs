// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase2
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase2 : TestManagementDatabase
  {
    public override BuildConfiguration QueryBuildConfigurationById2(
      int buildConfigurationid,
      Guid projectId)
    {
      return this.QueryBuildConfigurationById(buildConfigurationid, out Guid _);
    }

    public override BuildConfiguration QueryBuildConfigurationById(
      int buildConfigurationid,
      out Guid projectId)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "BuildReferenceDatabase.QueryBuildConfigurationById");
        projectId = Guid.Empty;
        BuildConfiguration buildConfiguration = new BuildConfiguration();
        this.PrepareStoredProcedure("prc_QueryBuildConfigurationById");
        this.BindInt("@buildConfigurationId", buildConfigurationid);
        SqlDataReader reader = this.ExecuteReader();
        if (reader.Read())
        {
          int dataspaceId;
          buildConfiguration = new TestManagementDatabase2.QueryBuildConfigurationsColumns().bind(reader, out dataspaceId);
          projectId = this.GetDataspaceIdentifier(dataspaceId);
        }
        return buildConfiguration;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "BuildReferenceDatabase.QueryBuildConfigurationById");
      }
    }

    public override void MarkTestBuildDeleted(Guid projectId, string[] buildUris)
    {
      foreach (string buildUri in buildUris)
      {
        this.PrepareStoredProcedure("prc_MarkTestBuildDeleted");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindString("@buildUri", buildUri, 256, false, SqlDbType.NVarChar);
        this.ExecuteNonQuery();
      }
    }

    public override IList<string> QueryBuildsByProject(
      Guid projectId,
      bool? queryDeletedBuild,
      int batchSize)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "BuildReferenceDatabase.QueryBuildsByProject");
        List<string> stringList = new List<string>();
        this.PrepareStoredProcedure("prc_QueryBuildsByProject");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindNullableBoolean("@queryDeletedBuild", queryDeletedBuild);
        this.BindInt("@batchSize", batchSize);
        SqlDataReader reader = this.ExecuteReader();
        SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("BuildUri");
        while (reader.Read())
          stringList.Add(sqlColumnBinder.GetString((IDataReader) reader, false));
        return (IList<string>) stringList;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "BuildReferenceDatabase.QueryBuildsByProject");
      }
    }

    public override void UpdateBuildDeletionState(
      Guid projectId,
      Dictionary<string, bool> buildUriToDeletionState)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "BuildReferenceDatabase.UpdateBuildDeletionState");
        this.PrepareStoredProcedure("prc_UpdateBuildDeletionState");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindBuildUriToDeletionState("@buildUriToDeletionState", (IEnumerable<KeyValuePair<string, bool>>) buildUriToDeletionState);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "BuildReferenceDatabase.UpdateBuildDeletionState");
      }
    }

    internal override void AddOrUpdateCodeCoverageSummary(
      Guid projectGuid,
      BuildConfiguration buildRef,
      CodeCoverageData coverageData)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestManagementDatabase.AddOrUpdateCodeCoverageSummary");
        this.PrepareStoredProcedure("prc_AddOrUpdateCodeCoverageSummary");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
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
        this.BindBuildRefTypeTable2("@buildRefData", (IEnumerable<BuildConfiguration>) builds);
        this.BindCoverageSummaryTypeTable("@coverageStatsDataTable", (IEnumerable<CodeCoverageStatistics>) coverageData.CoverageStats);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestManagementDatabase.AddOrUpdateCodeCoverageSummary");
      }
    }

    internal TestManagementDatabase2(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase2()
    {
    }

    public override ResultRetentionSettings CreateResultRetentionSettings(
      IVssRequestContext context,
      Guid projectId,
      ResultRetentionSettings settings)
    {
      context.TraceEnter(0, "TestManagement", "Database", "ResultRetentionDatabase.CreateResultRetentionSettings");
      this.PrepareStoredProcedure("prc_CreateResultRetentionSettings");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@automatedResultsRetentionDuration", settings.AutomatedResultsRetentionDuration);
      this.BindInt("@manualResultsRetentionDuration", settings.ManualResultsRetentionDuration);
      this.BindGuid("@lastUpdatedBy", new Guid(settings.LastUpdatedBy.Id));
      ResultRetentionSettings retentionSettings = TestManagementDatabase2.GetRetentionSettings(this.ExecuteReader());
      context.TraceLeave(0, "TestManagement", "Database", "ResultRetentionDatabase.CreateResultRetentionSettings");
      return retentionSettings;
    }

    public override ResultRetentionSettings GetResultRetentionSettings(
      IVssRequestContext context,
      Guid projectId)
    {
      context.TraceEnter(0, "TestManagement", "Database", "ResultRetentionDatabase.GetResultRetentionSettings");
      this.PrepareStoredProcedure("prc_GetResultRetentionSettings");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      SqlDataReader reader = this.ExecuteReader();
      ResultRetentionSettings retentionSettings = (ResultRetentionSettings) null;
      if (reader.Read())
        retentionSettings = new TestManagementDatabase2.ResultRetentionSettingsColumns().bind(reader);
      context.TraceLeave(0, "TestManagement", "Database", "ResultRetentionDatabase.GetResultRetentionSettings");
      return retentionSettings;
    }

    public override ResultRetentionSettings UpdateResultRetentionSettings(
      IVssRequestContext context,
      Guid projectId,
      ResultRetentionSettings settings)
    {
      context.TraceEnter(0, "TestManagement", "Database", "ResultRetentionDatabase.UpdateResultRetentionSettings");
      this.PrepareStoredProcedure("prc_UpdateResultRetentionSettings");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@automatedResultsRetentionDuration", settings.AutomatedResultsRetentionDuration);
      this.BindInt("@manualResultsRetentionDuration", settings.ManualResultsRetentionDuration);
      this.BindGuid("@lastUpdatedBy", new Guid(settings.LastUpdatedBy.Id));
      ResultRetentionSettings retentionSettings = TestManagementDatabase2.GetRetentionSettings(this.ExecuteReader());
      context.TraceLeave(0, "TestManagement", "Database", "ResultRetentionDatabase.UpdateResultRetentionSettings");
      return retentionSettings;
    }

    private static ResultRetentionSettings GetRetentionSettings(SqlDataReader reader)
    {
      reader.Read();
      return new TestManagementDatabase2.ResultRetentionSettingsColumns().bind(reader);
    }

    public override UpdatedRunProperties UpdateTestRun(
      Guid projectId,
      TestRun run,
      Guid updatedBy,
      ReleaseReference releaseRef = null,
      BuildConfiguration buildRef = null,
      bool skipRunStateTransitionCheck = false)
    {
      this.PrepareStoredProcedure("prc_UpdateTestRun");
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
      SqlDataReader reader = this.ExecuteReader();
      UpdatedRunProperties updatedRunProperties = reader.Read() ? new TestManagementDatabase2.UpdatedPropertyColumns().bindUpdatedRunProperties(reader) : throw new UnexpectedDatabaseResultException("prc_UpdateTestRun");
      updatedRunProperties.LastUpdatedBy = updatedBy;
      return updatedRunProperties;
    }

    public override UpdatedProperties AbortTestRun(
      Guid projectId,
      int testRunId,
      int revision,
      TestRunAbortOptions options,
      byte substate,
      Guid updatedBy,
      out string iterationUri,
      out Guid runProjGuid,
      out TestRun run,
      bool isTcmService = false)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.AbortTestRun");
        iterationUri = string.Empty;
        runProjGuid = Guid.Empty;
        this.PrepareStoredProcedure("prc_AbortTestRun");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@testRunId", testRunId);
        this.BindInt("@revision", revision);
        this.BindInt("@options", (int) options);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        this.BindByte("@substate", substate);
        SqlDataReader reader = this.ExecuteReader();
        UpdatedProperties updatedProperties = reader.Read() ? new TestManagementDatabase2.UpdatedPropertyColumns().bindUpdatedProperties(reader) : throw new UnexpectedDatabaseResultException("prc_AbortTestRun");
        updatedProperties.LastUpdatedBy = updatedBy;
        if (reader.NextResult() && reader.Read())
        {
          TestManagementDatabase2.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase2.QueryTestRunColumns();
          int dataspaceId;
          run = queryTestRunColumns.bind(reader, out dataspaceId, out iterationUri);
          runProjGuid = this.GetDataspaceIdentifier(dataspaceId);
        }
        else
          run = (TestRun) null;
        return updatedProperties;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.AbortTestRun");
      }
    }

    public override UpdatedProperties CancelTestRun(
      Guid projectId,
      int testRunId,
      Guid canceledBy,
      out string iterationUri,
      out Guid runProjGuid,
      out TestRun run)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.CancelTestRun");
        iterationUri = (string) null;
        runProjGuid = Guid.Empty;
        this.PrepareStoredProcedure("prc_CancelTestRun");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@testRunId", testRunId);
        this.BindGuid("@canceledBy", canceledBy);
        SqlDataReader reader = this.ExecuteReader();
        UpdatedProperties updatedProperties = reader.Read() ? new TestManagementDatabase2.UpdatedPropertyColumns().bindUpdatedProperties(reader) : throw new UnexpectedDatabaseResultException("prc_CancelTestRun");
        updatedProperties.LastUpdatedBy = canceledBy;
        if (reader.NextResult() && reader.Read())
        {
          TestManagementDatabase2.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase2.QueryTestRunColumns();
          int dataspaceId;
          run = queryTestRunColumns.bind(reader, out dataspaceId, out iterationUri);
          runProjGuid = this.GetDataspaceIdentifier(dataspaceId);
        }
        else
          run = (TestRun) null;
        return updatedProperties;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.CancelTestRun");
      }
    }

    public override List<TestRun> QueryTestRuns(
      int testRunId,
      Guid owner,
      string buildUri,
      Guid projectId,
      out Dictionary<int, string> iterationMap,
      out Dictionary<Guid, List<TestRun>> projectsRunsMap,
      int planId = -1,
      int skip = 0,
      int top = 2147483647,
      bool isTcmService = false)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRuns");
        Dictionary<int, TestRun> dictionary1 = new Dictionary<int, TestRun>();
        this.PrepareStoredProcedure("prc_QueryTestRuns");
        this.BindNullableInt("@testRunId", testRunId, 0);
        this.BindGuidPreserveNull("@owner", owner);
        this.BindString("@buildUri", buildUri, 256, true, SqlDbType.NVarChar);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        TestManagementDatabase2.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase2.QueryTestRunColumns();
        SqlDataReader reader = this.ExecuteReader();
        Dictionary<int, Guid> dictionary2 = new Dictionary<int, Guid>();
        iterationMap = new Dictionary<int, string>();
        projectsRunsMap = new Dictionary<Guid, List<TestRun>>();
        while (reader.Read())
        {
          int dataspaceId;
          string iterationUri;
          TestRun testRun = queryTestRunColumns.bind(reader, out dataspaceId, out iterationUri);
          if (!string.IsNullOrEmpty(iterationUri))
            iterationMap[testRun.TestRunId] = iterationUri;
          if (dictionary2.ContainsKey(dataspaceId))
          {
            Guid key = dictionary2[dataspaceId];
            projectsRunsMap[key].Add(testRun);
          }
          else
          {
            Guid dataspaceIdentifier = this.GetDataspaceIdentifier(dataspaceId);
            dictionary2[dataspaceId] = dataspaceIdentifier;
            projectsRunsMap[dataspaceIdentifier] = new List<TestRun>();
            projectsRunsMap[dataspaceIdentifier].Add(testRun);
          }
          dictionary1.Add(testRun.TestRunId, testRun);
        }
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException("prc_QueryTestRuns");
        TestManagementDatabase.FetchTestRunsExColumns testRunsExColumns = new TestManagementDatabase.FetchTestRunsExColumns();
        while (reader.Read())
        {
          Tuple<int, TestExtensionField> tuple = testRunsExColumns.bind(reader);
          if (dictionary1.ContainsKey(tuple.Item1))
            dictionary1[tuple.Item1].CustomFields.Add(tuple.Item2);
        }
        return dictionary1.Values.ToList<TestRun>();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRuns");
      }
    }

    public override List<TestRun> QueryTestRuns2(
      string whereClause,
      string orderBy,
      List<KeyValuePair<int, string>> displayNameInGroupList,
      out Dictionary<int, string> iterationMap,
      out Dictionary<Guid, List<TestRun>> projectsRunsMap)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRuns2");
        Dictionary<int, TestRun> dictionary1 = new Dictionary<int, TestRun>();
        this.PrepareDynamicProcedure("prc_QueryTestRuns2");
        this.BindString("@whereClause", whereClause, int.MaxValue, false, SqlDbType.NVarChar);
        this.BindString("@orderByClause", orderBy, int.MaxValue, false, SqlDbType.NVarChar);
        this.BindIntStringPairTypeTable("@valueListTable", (IEnumerable<KeyValuePair<int, string>>) displayNameInGroupList);
        TestManagementDatabase2.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase2.QueryTestRunColumns();
        SqlDataReader reader = this.ExecuteReader();
        Dictionary<int, Guid> dictionary2 = new Dictionary<int, Guid>();
        iterationMap = new Dictionary<int, string>();
        projectsRunsMap = new Dictionary<Guid, List<TestRun>>();
        while (reader.Read())
        {
          int dataspaceId;
          string iterationUri;
          TestRun testRun = queryTestRunColumns.bind(reader, out dataspaceId, out iterationUri);
          if (!string.IsNullOrEmpty(iterationUri))
            iterationMap[testRun.TestRunId] = iterationUri;
          if (dictionary2.ContainsKey(dataspaceId))
          {
            Guid key = dictionary2[dataspaceId];
            projectsRunsMap[key].Add(testRun);
          }
          else
          {
            Guid dataspaceIdentifier = this.GetDataspaceIdentifier(dataspaceId);
            dictionary2[dataspaceId] = dataspaceIdentifier;
            projectsRunsMap[dataspaceIdentifier] = new List<TestRun>();
            projectsRunsMap[dataspaceIdentifier].Add(testRun);
          }
          dictionary1.Add(testRun.TestRunId, testRun);
        }
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException("prc_QueryTestRuns2");
        TestManagementDatabase.FetchTestRunsExColumns testRunsExColumns = new TestManagementDatabase.FetchTestRunsExColumns();
        while (reader.Read())
        {
          Tuple<int, TestExtensionField> tuple = testRunsExColumns.bind(reader);
          if (dictionary1.ContainsKey(tuple.Item1))
          {
            TestRun testRun = dictionary1[tuple.Item1];
            testRun.CustomFields = testRun.CustomFields ?? new List<TestExtensionField>();
            testRun.CustomFields.Add(tuple.Item2);
          }
        }
        return dictionary1.Values.ToList<TestRun>();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRuns2");
      }
    }

    internal override TestRun QueryTestRunByTmiRunId(
      Guid tmiRunId,
      out string iterationUri,
      out Guid runProjGuid)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRunByTmiRunId");
        iterationUri = string.Empty;
        runProjGuid = Guid.Empty;
        this.PrepareStoredProcedure("prc_QueryTestRunByTmiRunId");
        this.BindGuid("@tmiRunId", tmiRunId);
        TestManagementDatabase2.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase2.QueryTestRunColumns();
        SqlDataReader reader = this.ExecuteReader();
        if (!reader.Read())
          return (TestRun) null;
        int dataspaceId;
        TestRun testRun = queryTestRunColumns.bind(reader, out dataspaceId, out iterationUri);
        runProjGuid = this.GetDataspaceIdentifier(dataspaceId);
        return testRun;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRunByTmiRunId");
      }
    }

    private List<Tuple<int, int, TestExtensionField>> GetExtensionFieldsMap(
      int testRunId,
      IEnumerable<TestCaseResult> results,
      bool useOrderIndex)
    {
      List<Tuple<int, int, TestExtensionField>> extensionFieldsMap = new List<Tuple<int, int, TestExtensionField>>();
      int orderIndex = 0;
      foreach (TestCaseResult result1 in results)
      {
        TestCaseResult result = result1;
        if (result.StackTrace != null)
          extensionFieldsMap.Add(new Tuple<int, int, TestExtensionField>(testRunId, useOrderIndex ? orderIndex : result.TestResultId, result.StackTrace));
        if (result.CustomFields != null && result.CustomFields.Any<TestExtensionField>())
          extensionFieldsMap.AddRange(result.CustomFields.Select<TestExtensionField, Tuple<int, int, TestExtensionField>>((System.Func<TestExtensionField, Tuple<int, int, TestExtensionField>>) (f => new Tuple<int, int, TestExtensionField>(testRunId, useOrderIndex ? orderIndex : result.TestResultId, f))));
        orderIndex++;
      }
      return extensionFieldsMap;
    }

    internal override TestCaseResult ResetTestResult(
      Guid projectId,
      int testRunId,
      int testResultId,
      Guid updatedBy,
      out string iterationUri,
      out Guid runProjGuid,
      out TestRun run,
      bool isTcmService = false)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.ResetTestResult");
        iterationUri = string.Empty;
        runProjGuid = Guid.Empty;
        this.PrepareStoredProcedure("TestResult.prc_ResetTestResult");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@testRunId", testRunId);
        this.BindInt("@testResultId", testResultId);
        this.BindGuid("@lastUpdatedBy", updatedBy);
        SqlDataReader reader = this.ExecuteReader();
        TestCaseResult testCaseResult = reader.Read() ? new TestManagementDatabase.FetchTestResultsColumns().bind(reader) : throw new UnexpectedDatabaseResultException("prc_ResetTestResult");
        if (reader.NextResult() && reader.Read())
        {
          TestManagementDatabase2.QueryTestRunColumns queryTestRunColumns = new TestManagementDatabase2.QueryTestRunColumns();
          int dataspaceId;
          run = queryTestRunColumns.bind(reader, out dataspaceId, out iterationUri);
          runProjGuid = this.GetDataspaceIdentifier(dataspaceId);
        }
        else
          run = (TestRun) null;
        return testCaseResult;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.ResetTestResult");
      }
    }

    public override void UpdateTestRunSummaryAndInsights(
      GuidAndString projectId,
      int testRunId,
      BuildConfiguration buildToCompare,
      ReleaseReference releaseToCompare,
      TestResultsContextType resultsContextType)
    {
      this.PrepareStoredProcedure("prc_UpdateTestSummaryAndInsights");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId.GuidId));
      this.BindInt("@testRunId", testRunId);
      List<BuildConfiguration> builds;
      if (buildToCompare != null)
        builds = new List<BuildConfiguration>()
        {
          buildToCompare
        };
      else
        builds = (List<BuildConfiguration>) null;
      this.BindBuildRefTypeTable2("@buildToCompare", (IEnumerable<BuildConfiguration>) builds);
      this.ExecuteReader();
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
      this.PrepareStoredProcedure("prc_QueryTestRunSummaryAndInsights");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId.GuidId));
      this.BindString("@sourceWorkflow", sourceWorkflow, 128, true, SqlDbType.NVarChar);
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
      this.BindBuildRefTypeTable2("@buildRef", (IEnumerable<BuildConfiguration>) builds);
      this.BindInt("@testRunId", 0);
      this.BindBoolean("@returnSummary", returnSummary);
      this.BindBoolean("@returnFailureDetails", returnFailureDetails);
      SqlDataReader reader = this.ExecuteReader();
      RunSummaryAndInsights summaryAndInsights = new RunSummaryAndInsights()
      {
        TestRunSummary = new RunSummary()
        {
          CurrentAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>(),
          PreviousAggregateDataByOutcome = (IList<RunSummaryByOutcome>) new List<RunSummaryByOutcome>()
        },
        TestResultInsights = new List<ResultInsights>()
      };
      if (reader.HasRows)
      {
        if (returnSummary)
        {
          TestManagementDatabase2.FetchTestRunSummaryColumns runSummaryColumns = new TestManagementDatabase2.FetchTestRunSummaryColumns();
          while (reader.Read())
            summaryAndInsights.TestRunSummary.CurrentAggregateDataByOutcome.Add(runSummaryColumns.bind(reader));
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsights");
          while (reader.Read())
            summaryAndInsights.TestRunSummary.PreviousAggregateDataByOutcome.Add(runSummaryColumns.bind(reader));
        }
        if (returnFailureDetails)
        {
          if (returnSummary && !reader.NextResult())
            throw new UnexpectedDatabaseResultException("prc_QueryTestRunSummaryAndInsights");
          TestManagementDatabase2.FetchTestFailureDetailsColumns failureDetailsColumns = new TestManagementDatabase2.FetchTestFailureDetailsColumns();
          while (reader.Read())
            summaryAndInsights.TestResultInsights.Add(failureDetailsColumns.bind(reader));
        }
      }
      runsCount = 0;
      isBuildOld = false;
      return summaryAndInsights;
    }

    public override UpdatedProperties UpdateTestSettings(
      Guid projectId,
      TestSettings settings,
      Guid updatedBy)
    {
      this.PrepareStoredProcedure("prc_UpdateTestSettings");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@settingsId", settings.Id);
      this.BindStringPreserveNull("@name", settings.Name, 256, SqlDbType.NVarChar);
      this.BindStringPreserveNull("@description", settings.Description, int.MaxValue, SqlDbType.NVarChar);
      this.BindXml("@settings", settings.Settings);
      this.BindXml("@machineRoles", TestSettingsMachineRole.ToXml(settings.MachineRoles));
      this.BindNullableInt("@areaId", settings.AreaId, 0);
      this.BindBoolean("@isAutomated", settings.IsAutomated);
      this.BindGuid("@lastUpdatedBy", updatedBy);
      this.BindInt("@revision", settings.Revision);
      SqlDataReader reader = this.ExecuteReader();
      UpdatedProperties updatedProperties = reader.Read() ? new TestManagementDatabase2.UpdatedPropertyColumns().bindUpdatedProperties(reader) : throw new UnexpectedDatabaseResultException("prc_UpdateTestSettings");
      updatedProperties.LastUpdatedBy = updatedBy;
      return updatedProperties;
    }

    private new class QueryBuildConfigurationsColumns
    {
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder BuildConfigurationId = new SqlColumnBinder(nameof (BuildConfigurationId));
      private SqlColumnBinder BuildId = new SqlColumnBinder(nameof (BuildId));
      private SqlColumnBinder BuildUri = new SqlColumnBinder(nameof (BuildUri));
      private SqlColumnBinder BuildNumber = new SqlColumnBinder(nameof (BuildNumber));
      private SqlColumnBinder BuildDefinitionId = new SqlColumnBinder(nameof (BuildDefinitionId));
      private SqlColumnBinder BuildPlatform = new SqlColumnBinder(nameof (BuildPlatform));
      private SqlColumnBinder BuildFlavor = new SqlColumnBinder(nameof (BuildFlavor));
      private SqlColumnBinder RepositoryId = new SqlColumnBinder(nameof (RepositoryId));
      private SqlColumnBinder BranchName = new SqlColumnBinder(nameof (BranchName));
      private SqlColumnBinder SourceVersion = new SqlColumnBinder(nameof (SourceVersion));
      private SqlColumnBinder BuildSystem = new SqlColumnBinder(nameof (BuildSystem));

      internal BuildConfiguration bind(SqlDataReader reader, out int dataspaceId)
      {
        dataspaceId = this.DataspaceId.GetInt32((IDataReader) reader);
        return new BuildConfiguration()
        {
          BuildConfigurationId = this.BuildConfigurationId.GetInt32((IDataReader) reader),
          BuildId = this.BuildId.GetInt32((IDataReader) reader),
          BuildUri = this.BuildUri.GetString((IDataReader) reader, false),
          BuildNumber = this.BuildNumber.GetString((IDataReader) reader, false),
          BuildDefinitionId = this.BuildDefinitionId.GetInt32((IDataReader) reader),
          BuildPlatform = this.BuildPlatform.GetString((IDataReader) reader, false),
          BuildFlavor = this.BuildFlavor.GetString((IDataReader) reader, false),
          BranchName = this.BranchName.GetString((IDataReader) reader, true),
          SourceVersion = this.SourceVersion.GetString((IDataReader) reader, true),
          BuildSystem = this.BuildSystem.GetString((IDataReader) reader, false)
        };
      }
    }

    private class ResultRetentionSettingsColumns
    {
      private SqlColumnBinder AutomatedResultsRetentionDurationId = new SqlColumnBinder("AutomatedResultsRetentionDuration");
      private SqlColumnBinder ManualResultsRetentionDurationId = new SqlColumnBinder("ManualResultsRetentionDuration");
      private SqlColumnBinder LastUpdatedBy = new SqlColumnBinder(nameof (LastUpdatedBy));
      private SqlColumnBinder LastUpdateDate = new SqlColumnBinder("LastUpdatedDate");

      internal ResultRetentionSettings bind(SqlDataReader reader)
      {
        ResultRetentionSettings retentionSettings = new ResultRetentionSettings()
        {
          AutomatedResultsRetentionDuration = this.AutomatedResultsRetentionDurationId.GetInt32((IDataReader) reader),
          ManualResultsRetentionDuration = this.ManualResultsRetentionDurationId.GetInt32((IDataReader) reader),
          LastUpdatedDate = this.LastUpdateDate.GetDateTime((IDataReader) reader),
          LastUpdatedBy = new IdentityRef()
        };
        retentionSettings.LastUpdatedBy.Id = this.LastUpdatedBy.GetGuid((IDataReader) reader).ToString();
        return retentionSettings;
      }
    }

    private new class CreateTestRunColumns
    {
      internal SqlColumnBinder testRunId = new SqlColumnBinder("TestRunId");
      internal SqlColumnBinder revision = new SqlColumnBinder("Revision");

      internal TestRun bind(SqlDataReader reader)
      {
        TestRun testRun = new TestRun();
        testRun.TestRunId = this.testRunId.GetInt32((IDataReader) reader);
        testRun.Revision = this.revision.GetInt32((IDataReader) reader);
        return testRun;
      }
    }

    private new class UpdatedPropertyColumns
    {
      internal SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      internal SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      internal SqlColumnBinder LastTestResultId = new SqlColumnBinder(nameof (LastTestResultId));
      internal SqlColumnBinder TotalTests = new SqlColumnBinder(nameof (TotalTests));
      internal SqlColumnBinder IsRunStarted = new SqlColumnBinder(nameof (IsRunStarted));
      internal SqlColumnBinder IsRunCompleted = new SqlColumnBinder(nameof (IsRunCompleted));

      internal UpdatedProperties bindUpdatedProperties(SqlDataReader reader) => new UpdatedProperties()
      {
        Revision = this.Revision.GetInt32((IDataReader) reader),
        LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader)
      };

      internal BlockedPointProperties bindBlockedTestPointProperties(SqlDataReader reader)
      {
        BlockedPointProperties blockedPointProperties = new BlockedPointProperties();
        blockedPointProperties.Revision = this.Revision.GetInt32((IDataReader) reader);
        blockedPointProperties.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        blockedPointProperties.LastTestResultId = this.LastTestResultId.GetInt32((IDataReader) reader);
        return blockedPointProperties;
      }

      internal UpdatedRunProperties bindUpdatedRunProperties(SqlDataReader reader)
      {
        UpdatedRunProperties updatedRunProperties = new UpdatedRunProperties();
        updatedRunProperties.Revision = this.Revision.GetInt32((IDataReader) reader);
        updatedRunProperties.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        updatedRunProperties.TotalTests = this.TotalTests.GetInt32((IDataReader) reader);
        updatedRunProperties.IsRunStarted = this.IsRunStarted.GetBoolean((IDataReader) reader);
        updatedRunProperties.IsRunCompleted = this.IsRunCompleted.GetBoolean((IDataReader) reader);
        return updatedRunProperties;
      }
    }

    private new class QueryTestRunColumns
    {
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder Title = new SqlColumnBinder(nameof (Title));
      private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
      private SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      private SqlColumnBinder Owner = new SqlColumnBinder(nameof (Owner));
      private SqlColumnBinder State = new SqlColumnBinder(nameof (State));
      private SqlColumnBinder BuildUri = new SqlColumnBinder(nameof (BuildUri));
      private SqlColumnBinder DropLocation = new SqlColumnBinder(nameof (DropLocation));
      private SqlColumnBinder BuildNumber = new SqlColumnBinder(nameof (BuildNumber));
      private SqlColumnBinder BuildPlatform = new SqlColumnBinder(nameof (BuildPlatform));
      private SqlColumnBinder BuildFlavor = new SqlColumnBinder(nameof (BuildFlavor));
      private SqlColumnBinder BuildConfigurationId = new SqlColumnBinder(nameof (BuildConfigurationId));
      private SqlColumnBinder BuildId = new SqlColumnBinder(nameof (BuildId));
      private SqlColumnBinder BuildDefinitionId = new SqlColumnBinder(nameof (BuildDefinitionId));
      private SqlColumnBinder RepositoryId = new SqlColumnBinder(nameof (RepositoryId));
      private SqlColumnBinder BranchName = new SqlColumnBinder(nameof (BranchName));
      private SqlColumnBinder SourceVersion = new SqlColumnBinder(nameof (SourceVersion));
      private SqlColumnBinder BuildSystem = new SqlColumnBinder(nameof (BuildSystem));
      private SqlColumnBinder StartDate = new SqlColumnBinder(nameof (StartDate));
      private SqlColumnBinder CompleteDate = new SqlColumnBinder(nameof (CompleteDate));
      private SqlColumnBinder PostProcessState = new SqlColumnBinder(nameof (PostProcessState));
      private SqlColumnBinder DueDate = new SqlColumnBinder(nameof (DueDate));
      private SqlColumnBinder IterationUri = new SqlColumnBinder(nameof (IterationUri));
      private SqlColumnBinder IterationId = new SqlColumnBinder(nameof (IterationId));
      private SqlColumnBinder Controller = new SqlColumnBinder(nameof (Controller));
      private SqlColumnBinder ErrorMessage = new SqlColumnBinder(nameof (ErrorMessage));
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder TestPlanId = new SqlColumnBinder(nameof (TestPlanId));
      private SqlColumnBinder TestMessageLogId = new SqlColumnBinder(nameof (TestMessageLogId));
      private SqlColumnBinder Guid = new SqlColumnBinder(nameof (Guid));
      private SqlColumnBinder LegacySharePath = new SqlColumnBinder(nameof (LegacySharePath));
      private SqlColumnBinder TestSettingsId = new SqlColumnBinder(nameof (TestSettingsId));
      private SqlColumnBinder PublicTestSettingsId = new SqlColumnBinder(nameof (PublicTestSettingsId));
      private SqlColumnBinder TestEnvironmentId = new SqlColumnBinder(nameof (TestEnvironmentId));
      private SqlColumnBinder LastUpdatedBy = new SqlColumnBinder(nameof (LastUpdatedBy));
      private SqlColumnBinder Type = new SqlColumnBinder(nameof (Type));
      private SqlColumnBinder IsAutomated = new SqlColumnBinder(nameof (IsAutomated));
      private SqlColumnBinder Version = new SqlColumnBinder(nameof (Version));
      private SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      private SqlColumnBinder IsBvt = new SqlColumnBinder(nameof (IsBvt));
      private SqlColumnBinder Comment = new SqlColumnBinder(nameof (Comment));
      private SqlColumnBinder RV = new SqlColumnBinder(nameof (RV));
      private SqlColumnBinder TotalTests = new SqlColumnBinder(nameof (TotalTests));
      private SqlColumnBinder IncompleteTests = new SqlColumnBinder(nameof (IncompleteTests));
      private SqlColumnBinder NotApplicableTests = new SqlColumnBinder(nameof (NotApplicableTests));
      private SqlColumnBinder PassedTests = new SqlColumnBinder(nameof (PassedTests));
      private SqlColumnBinder UnanalyzedTests = new SqlColumnBinder(nameof (UnanalyzedTests));
      private SqlColumnBinder BugsCount = new SqlColumnBinder(nameof (BugsCount));
      private SqlColumnBinder SourceFilter = new SqlColumnBinder(nameof (SourceFilter));
      private SqlColumnBinder TestCaseFilter = new SqlColumnBinder(nameof (TestCaseFilter));
      private SqlColumnBinder TestEnvironmentUrl = new SqlColumnBinder(nameof (TestEnvironmentUrl));
      private SqlColumnBinder AutEnvironmentUrl = new SqlColumnBinder(nameof (AutEnvironmentUrl));
      private SqlColumnBinder Substate = new SqlColumnBinder("SubState");
      private SqlColumnBinder CsmContent = new SqlColumnBinder(nameof (CsmContent));
      private SqlColumnBinder CsmParameters = new SqlColumnBinder(nameof (CsmParameters));
      private SqlColumnBinder SubscriptionName = new SqlColumnBinder(nameof (SubscriptionName));
      private SqlColumnBinder ReleaseUri = new SqlColumnBinder(nameof (ReleaseUri));
      private SqlColumnBinder ReleaseEnvironmentUri = new SqlColumnBinder(nameof (ReleaseEnvironmentUri));

      internal TestRun bind(SqlDataReader reader, out int dataspaceId, out string iterationUri)
      {
        TestRun testRun = new TestRun();
        testRun.TestRunId = this.TestRunId.GetInt32((IDataReader) reader);
        testRun.Title = this.Title.GetString((IDataReader) reader, false);
        testRun.CreationDate = this.CreationDate.GetDateTime((IDataReader) reader);
        testRun.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        testRun.Owner = this.Owner.GetGuid((IDataReader) reader, false);
        testRun.State = this.State.GetByte((IDataReader) reader);
        testRun.ErrorMessage = this.ErrorMessage.GetString((IDataReader) reader, false);
        testRun.BuildUri = this.BuildUri.GetString((IDataReader) reader, true);
        testRun.DropLocation = this.DropLocation.GetString((IDataReader) reader, true);
        testRun.BuildNumber = this.BuildNumber.GetString((IDataReader) reader, true);
        testRun.BuildPlatform = this.BuildPlatform.GetString((IDataReader) reader, true);
        testRun.BuildFlavor = this.BuildFlavor.GetString((IDataReader) reader, true);
        testRun.BuildConfigurationId = this.BuildConfigurationId.GetInt32((IDataReader) reader, 0);
        testRun.StartDate = this.StartDate.GetDateTime((IDataReader) reader);
        testRun.CompleteDate = this.CompleteDate.GetDateTime((IDataReader) reader);
        testRun.PostProcessState = this.PostProcessState.GetByte((IDataReader) reader);
        testRun.DueDate = this.DueDate.GetDateTime((IDataReader) reader);
        testRun.IterationId = this.IterationId.GetInt32((IDataReader) reader, 0);
        testRun.Controller = this.Controller.GetString((IDataReader) reader, true);
        testRun.TestPlanId = this.TestPlanId.GetInt32((IDataReader) reader);
        testRun.TestMessageLogId = this.TestMessageLogId.GetInt32((IDataReader) reader);
        testRun.LegacySharePath = this.LegacySharePath.GetString((IDataReader) reader, false);
        testRun.TestSettingsId = this.TestSettingsId.GetInt32((IDataReader) reader);
        testRun.PublicTestSettingsId = this.PublicTestSettingsId.GetInt32((IDataReader) reader);
        testRun.TestEnvironmentId = this.TestEnvironmentId.GetGuid((IDataReader) reader);
        testRun.LastUpdatedBy = this.LastUpdatedBy.GetGuid((IDataReader) reader, false);
        testRun.Type = this.Type.GetByte((IDataReader) reader);
        testRun.IsAutomated = this.IsAutomated.GetBoolean((IDataReader) reader);
        testRun.Version = this.Version.GetInt32((IDataReader) reader);
        testRun.Revision = this.Revision.GetInt32((IDataReader) reader);
        testRun.IsBvt = this.IsBvt.GetBoolean((IDataReader) reader);
        testRun.Comment = this.Comment.GetString((IDataReader) reader, false);
        testRun.RowVersion = this.RV.GetBytes((IDataReader) reader, false);
        testRun.TotalTests = this.TotalTests.GetInt32((IDataReader) reader);
        testRun.IncompleteTests = this.IncompleteTests.GetInt32((IDataReader) reader);
        testRun.NotApplicableTests = this.NotApplicableTests.GetInt32((IDataReader) reader);
        testRun.PassedTests = this.PassedTests.GetInt32((IDataReader) reader);
        testRun.UnanalyzedTests = this.UnanalyzedTests.GetInt32((IDataReader) reader);
        testRun.BugsCount = this.BugsCount.GetInt32((IDataReader) reader);
        testRun.ReleaseUri = this.ReleaseUri.GetString((IDataReader) reader, true);
        testRun.ReleaseEnvironmentUri = this.ReleaseEnvironmentUri.GetString((IDataReader) reader, true);
        testRun.BuildReference = new BuildConfiguration()
        {
          BuildId = this.BuildId.ColumnExists((IDataReader) reader) ? this.BuildId.GetInt32((IDataReader) reader, 0) : 0,
          BuildUri = this.BuildUri.GetString((IDataReader) reader, true),
          BuildNumber = this.BuildNumber.GetString((IDataReader) reader, true),
          BuildPlatform = this.BuildPlatform.GetString((IDataReader) reader, true),
          BuildFlavor = this.BuildFlavor.GetString((IDataReader) reader, true),
          BuildConfigurationId = this.BuildConfigurationId.GetInt32((IDataReader) reader, 0),
          BuildDefinitionId = this.BuildDefinitionId.ColumnExists((IDataReader) reader) ? this.BuildDefinitionId.GetInt32((IDataReader) reader, 0) : 0,
          BranchName = this.BranchName.ColumnExists((IDataReader) reader) ? this.BranchName.GetString((IDataReader) reader, true) : string.Empty,
          SourceVersion = this.SourceVersion.ColumnExists((IDataReader) reader) ? this.SourceVersion.GetString((IDataReader) reader, true) : string.Empty,
          BuildSystem = this.BuildSystem.ColumnExists((IDataReader) reader) ? this.BuildSystem.GetString((IDataReader) reader, true) : string.Empty,
          RepositoryId = string.Empty,
          RepositoryType = string.Empty
        };
        if (((int) testRun.Type & 16) != 0)
        {
          testRun.DtlTestEnvironment = new ShallowReference()
          {
            Url = this.TestEnvironmentUrl.GetString((IDataReader) reader, true)
          };
          testRun.DtlAutEnvironment = new ShallowReference()
          {
            Url = this.AutEnvironmentUrl.GetString((IDataReader) reader, true)
          };
          if (testRun.IsAutomated)
          {
            testRun.Filter = new RunFilter();
            testRun.Filter.SourceFilter = this.SourceFilter.GetString((IDataReader) reader, false);
            testRun.Filter.TestCaseFilter = this.TestCaseFilter.GetString((IDataReader) reader, true);
            testRun.Substate = this.Substate.GetByte((IDataReader) reader);
            testRun.CsmContent = this.CsmContent.GetString((IDataReader) reader, true);
            testRun.CsmParameters = this.CsmParameters.GetString((IDataReader) reader, true);
            testRun.SubscriptionName = this.SubscriptionName.GetString((IDataReader) reader, true);
          }
        }
        dataspaceId = this.DataspaceId.GetInt32((IDataReader) reader);
        iterationUri = this.IterationUri.GetString((IDataReader) reader, true);
        return testRun;
      }
    }

    protected class FetchTestRunSummaryColumns
    {
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder RunState = new SqlColumnBinder("TestRunState");
      private SqlColumnBinder TestRunContextId = new SqlColumnBinder(nameof (TestRunContextId));
      private SqlColumnBinder TestRunCompletedDate = new SqlColumnBinder(nameof (TestRunCompletedDate));
      private SqlColumnBinder TestOutcome = new SqlColumnBinder(nameof (TestOutcome));
      private SqlColumnBinder ResultCount = new SqlColumnBinder(nameof (ResultCount));
      private SqlColumnBinder ResultDuration = new SqlColumnBinder(nameof (ResultDuration));
      private SqlColumnBinder RunDuration = new SqlColumnBinder(nameof (RunDuration));
      private SqlColumnBinder ReleaseId = new SqlColumnBinder(nameof (ReleaseId));
      private SqlColumnBinder ReleaseEnvId = new SqlColumnBinder(nameof (ReleaseEnvId));
      private SqlColumnBinder IsRerun = new SqlColumnBinder(nameof (IsRerun));
      private SqlColumnBinder ResultMetadata = new SqlColumnBinder(nameof (ResultMetadata));

      internal RunSummaryByOutcome bind(SqlDataReader reader) => new RunSummaryByOutcome()
      {
        TestRunId = this.TestRunId.GetInt32((IDataReader) reader),
        TestRunState = this.RunState.ColumnExists((IDataReader) reader) ? (Microsoft.TeamFoundation.TestManagement.Client.TestRunState) this.RunState.GetByte((IDataReader) reader) : Microsoft.TeamFoundation.TestManagement.Client.TestRunState.Completed,
        TestRunContextId = this.TestRunContextId.GetInt32((IDataReader) reader),
        RunCompletedDate = this.TestRunCompletedDate.GetDateTime((IDataReader) reader),
        TestOutcome = (Microsoft.TeamFoundation.TestManagement.Client.TestOutcome) this.TestOutcome.GetByte((IDataReader) reader),
        ResultCount = this.ResultCount.GetInt32((IDataReader) reader),
        ResultDuration = Convert.ToInt64(this.ResultDuration.GetObject((IDataReader) reader)),
        RunDuration = Convert.ToInt64(this.RunDuration.GetObject((IDataReader) reader)),
        ResultMetadata = this.ResultMetadata.ColumnExists((IDataReader) reader) ? this.ResultMetadata.GetByte((IDataReader) reader) : (this.IsRerun.ColumnExists((IDataReader) reader) ? Convert.ToByte(this.IsRerun.GetObject((IDataReader) reader)) : (byte) 0)
      };

      internal ReleaseReference bindRelease(SqlDataReader reader) => new ReleaseReference()
      {
        ReleaseId = this.ReleaseId.ColumnExists((IDataReader) reader) ? this.ReleaseId.GetInt32((IDataReader) reader) : 0,
        ReleaseEnvId = this.ReleaseEnvId.ColumnExists((IDataReader) reader) ? this.ReleaseEnvId.GetInt32((IDataReader) reader) : 0
      };
    }

    protected class FetchTestFailureDetailsColumns
    {
      private SqlColumnBinder DataspaceId = new SqlColumnBinder(nameof (DataspaceId));
      private SqlColumnBinder TestRunId = new SqlColumnBinder(nameof (TestRunId));
      private SqlColumnBinder TestRunContextId = new SqlColumnBinder(nameof (TestRunContextId));
      private SqlColumnBinder TestRunCompletedDate = new SqlColumnBinder(nameof (TestRunCompletedDate));
      private SqlColumnBinder PrevRunContextId = new SqlColumnBinder(nameof (PrevRunContextId));
      private SqlColumnBinder NewFailures = new SqlColumnBinder(nameof (NewFailures));
      private SqlColumnBinder ExistingFailures = new SqlColumnBinder(nameof (ExistingFailures));
      private SqlColumnBinder FixedTests = new SqlColumnBinder(nameof (FixedTests));
      private SqlColumnBinder NewFailedResults = new SqlColumnBinder(nameof (NewFailedResults));
      private SqlColumnBinder ExistingFailedResults = new SqlColumnBinder(nameof (ExistingFailedResults));
      private SqlColumnBinder FixedTestResults = new SqlColumnBinder(nameof (FixedTestResults));
      private SqlColumnBinder BuildRefId = new SqlColumnBinder(nameof (BuildRefId));
      private SqlColumnBinder ReleaseRefId = new SqlColumnBinder(nameof (ReleaseRefId));

      internal ResultInsights bind(SqlDataReader reader) => new ResultInsights()
      {
        TestRunId = this.TestRunId.GetInt32((IDataReader) reader),
        TestRunContextId = this.TestRunContextId.GetInt32((IDataReader) reader),
        TestRunCompletedDate = this.TestRunCompletedDate.GetDateTime((IDataReader) reader),
        PrevRunContextId = this.PrevRunContextId.GetInt32((IDataReader) reader),
        NewFailures = this.NewFailures.GetInt32((IDataReader) reader),
        ExistingFailures = this.ExistingFailures.GetInt32((IDataReader) reader),
        FixedTests = this.FixedTests.GetInt32((IDataReader) reader),
        NewFailedResults = this.NewFailedResults.GetString((IDataReader) reader, true),
        ExistingFailedResults = this.ExistingFailedResults.GetString((IDataReader) reader, true),
        FixedTestResults = this.FixedTestResults.GetString((IDataReader) reader, true),
        PrevBuildRefId = this.BuildRefId.GetInt32((IDataReader) reader),
        PrevReleaseRefId = this.ReleaseRefId.GetInt32((IDataReader) reader)
      };
    }
  }
}
