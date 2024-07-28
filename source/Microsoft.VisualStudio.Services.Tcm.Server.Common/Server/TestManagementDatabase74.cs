// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase74
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase74 : TestManagementDatabase73
  {
    internal TestManagementDatabase74(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase74()
    {
    }

    public override Dictionary<int, List<TestRunStatistic>> QueryTestRunStatistics(
      List<int> testRunIds,
      Guid projectId,
      bool isTcmService = false,
      bool shouldReturnStatsIfNotComputed = true)
    {
      Dictionary<int, TestResolutionState> resolutionStates = new Dictionary<int, TestResolutionState>();
      List<TestRunStatistic> testRunStatisticList = new List<TestRunStatistic>();
      int lazyInitialization = this.GetDataspaceIdWithLazyInitialization(projectId);
      this.PrepareStoredProcedure("TestResult.prc_QueryTestRunStatistics");
      this.BindInt("@dataspaceId", lazyInitialization);
      this.BindIdTypeTable("@testRunIdTable", (IEnumerable<int>) testRunIds);
      this.BindBoolean("@isTcmService", isTcmService);
      this.BindBoolean("@shouldReturnStatsIfNotComputed", shouldReturnStatsIfNotComputed);
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.QueryTestResolutionStatesColumns resolutionStatesColumns = new TestManagementDatabase.QueryTestResolutionStatesColumns();
      while (reader.Read())
      {
        TestResolutionState testResolutionState = resolutionStatesColumns.bind(reader);
        resolutionStates.Add(testResolutionState.Id, testResolutionState);
      }
      if (!reader.NextResult())
        throw new UnexpectedDatabaseResultException("prc_QueryTestRunStatistics");
      TestManagementDatabase.QueryTestRunStatisticsColumns statisticsColumns1 = new TestManagementDatabase.QueryTestRunStatisticsColumns();
      while (reader.Read())
      {
        TestRunStatistic testRunStatistic = statisticsColumns1.Bind(reader, (IDictionary<int, TestResolutionState>) resolutionStates);
        testRunStatisticList.Add(testRunStatistic);
      }
      if (reader.NextResult())
      {
        TestManagementDatabase.QueryTestRunStatisticsColumns statisticsColumns2 = new TestManagementDatabase.QueryTestRunStatisticsColumns();
        while (reader.Read())
        {
          TestRunStatistic testRunStatistic = statisticsColumns2.Bind(reader, (IDictionary<int, TestResolutionState>) resolutionStates);
          testRunStatisticList.Add(testRunStatistic);
        }
      }
      Dictionary<int, List<TestRunStatistic>> dictionary = new Dictionary<int, List<TestRunStatistic>>();
      foreach (TestRunStatistic testRunStatistic in testRunStatisticList)
      {
        if (!dictionary.ContainsKey(testRunStatistic.TestRunId))
          dictionary[testRunStatistic.TestRunId] = new List<TestRunStatistic>();
        dictionary[testRunStatistic.TestRunId].Add(testRunStatistic);
      }
      return dictionary;
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
      this.BindStringPreserveNull("@errorMessage", run.ErrorMessage, 4000, SqlDbType.NVarChar);
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
  }
}
