// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase60
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase60 : TestManagementDatabase59
  {
    public override void AddOrUpdateCodeCoverageSummaryWithStatus(
      Guid projectGuid,
      BuildConfiguration buildRef,
      CodeCoverageData coverageData,
      CoverageSummaryStatus status)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestManagementDatabase.AddOrUpdateCodeCoverageSummaryWithStatus");
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
        this.BindBuildRefTypeTable3("@buildRefData", (IEnumerable<BuildConfiguration>) builds);
        this.BindCoverageSummaryTypeTable("@coverageStatsDataTable", (IEnumerable<CodeCoverageStatistics>) coverageData.CoverageStats);
        this.BindInt("@coverageStatus", (int) status);
        this.ExecuteNonQuery();
      }
      catch (SqlException ex)
      {
        this.RequestContext.Trace(1015021, TraceLevel.Error, "CodeCoverageSummaryDatabase", nameof (AddOrUpdateCodeCoverageSummaryWithStatus), "BuildID: " + buildRef.BuildId.ToString() + ", Buildflavor: " + buildRef.BuildFlavor + ", BuildPlatform: " + buildRef.BuildFlavor + " ==> " + ex.Message);
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestManagementDatabase.AddOrUpdateCodeCoverageSummaryWithStatus");
      }
    }

    public override void AddOrUpdateCodeCoverageSummaryStatus(
      Guid projectGuid,
      BuildConfiguration buildRef,
      CoverageSummaryStatus status)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestManagementDatabase.AddOrUpdateCodeCoverageSummaryStatus");
        this.PrepareStoredProcedure("prc_AddOrUpdateCoverageSummaryStatus");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectGuid));
        this.BindInt("@buildId", buildRef.BuildId);
        this.BindString("@buildUri", buildRef.BuildUri, 256, true, SqlDbType.NVarChar);
        this.BindInt("@coverageStatus", (int) status);
        this.ExecuteNonQuery();
      }
      catch (SqlException ex)
      {
        this.RequestContext.Trace(1015022, TraceLevel.Error, "CodeCoverageSummaryDatabase", nameof (AddOrUpdateCodeCoverageSummaryStatus), "BuildID: " + buildRef.BuildId.ToString() + ", Buildflavor: " + buildRef.BuildFlavor + ", BuildPlatform: " + buildRef.BuildFlavor + " ==> " + ex.Message);
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestManagementDatabase.AddOrUpdateCodeCoverageSummaryStatus");
      }
    }

    internal override CodeCoverageSummary QueryCodeCoverageSummary(
      Guid projectGuid,
      string buildUri,
      string deltaBuildUri)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestManagementDatabase.QueryCodeCoverageSummary");
        CodeCoverageSummary codeCoverageSummary = new CodeCoverageSummary();
        this.PrepareStoredProcedure("prc_QueryCodeCoverageSummary");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectGuid));
        this.BindString("@buildUri", buildUri, 256, false, SqlDbType.NVarChar);
        this.BindString("@deltaBuildUri", deltaBuildUri, 256, true, SqlDbType.NVarChar);
        List<CodeCoverageData> source = new List<CodeCoverageData>();
        using (SqlDataReader reader = this.ExecuteReader())
        {
          while (reader.Read())
          {
            string buildFlavor;
            string buildPlatform;
            CodeCoverageStatistics coverageStatistics = new TestManagementDatabase.CodeCoverageSummaryColumns().Bind(reader, out buildPlatform, out buildFlavor);
            CodeCoverageData codeCoverageData = source.FirstOrDefault<CodeCoverageData>((System.Func<CodeCoverageData, bool>) (data => buildFlavor.Equals(data.BuildFlavor) && buildPlatform.Equals(data.BuildPlatform)));
            if (codeCoverageData == null)
            {
              codeCoverageData = new CodeCoverageData()
              {
                CoverageStats = (IList<CodeCoverageStatistics>) new List<CodeCoverageStatistics>(),
                BuildPlatform = buildPlatform,
                BuildFlavor = buildFlavor
              };
              source.Add(codeCoverageData);
            }
            codeCoverageData.CoverageStats.Add(coverageStatistics);
          }
          if (reader.NextResult())
          {
            if (reader.Read())
              codeCoverageSummary.SummaryStatus = (CoverageSummaryStatus) reader.GetInt32(reader.GetOrdinal("SummaryStatus"));
          }
        }
        codeCoverageSummary.CoverageData = (IList<CodeCoverageData>) source;
        codeCoverageSummary.BuildUri = buildUri;
        codeCoverageSummary.DeltaBuildUri = deltaBuildUri;
        return codeCoverageSummary;
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestManagementDatabase.QueryCodeCoverageSummary");
      }
    }

    internal override CoverageSummaryStatusResult QueryCodeCoverageSummaryStatus(
      Guid projectGuid,
      BuildConfiguration buildRef)
    {
      try
      {
        this.RequestContext.TraceEnter("Database", "TestManagementDatabase.QueryCodeCoverageSummaryStatus");
        this.PrepareStoredProcedure("prc_QueryCodeCoverageSummaryStatus");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectGuid));
        this.BindInt("@buildId", buildRef.BuildId);
        this.BindString("@buildUri", buildRef.BuildUri, 256, false, SqlDbType.NVarChar);
        using (SqlDataReader sqlDataReader = this.ExecuteReader())
        {
          if (sqlDataReader.Read())
            return new CoverageSummaryStatusResult()
            {
              SummaryStatus = (CoverageSummaryStatus) sqlDataReader.GetByte(sqlDataReader.GetOrdinal("Status")),
              RequestedDate = sqlDataReader.GetDateTime(sqlDataReader.GetOrdinal("RequestedDate"))
            };
        }
        return base.QueryCodeCoverageSummaryStatus(projectGuid, buildRef);
      }
      finally
      {
        this.RequestContext.TraceLeave("Database", "TestManagementDatabase.QueryCodeCoverageSummaryStatus");
      }
    }

    internal TestManagementDatabase60(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase60()
    {
    }

    internal override void UpdateTestResultsMetaData(
      Guid projectId,
      int testCaseReferenceId,
      int maxBranchCount,
      List<TestBranchFlakinesStateMap> markFlakyMap,
      List<TestBranchFlakinesStateMap> unMarkFlakyMap)
    {
      try
      {
        using (PerfManager.Measure(this.RequestContext, "Database", "TestResultDatabase.UpdateTestResultsMetaData"))
        {
          this.PrepareStoredProcedure("TestResult.prc_UpdateFlakyIdentifiersForTestCaseRef");
          this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
          this.BindInt("@testCaseReferenceId", testCaseReferenceId);
          this.BindInt("@maxBranchCount", maxBranchCount);
          this.BindIdsForMarkUnMarkFlakiness("@flakyEntries", markFlakyMap);
          this.BindIdsForMarkUnMarkFlakiness("@unFlakyEntries", unMarkFlakyMap);
          this.ExecuteReader();
        }
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
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
      this.BindString("@sourceWorkflow", run.SourceWorkflow, 128, false, SqlDbType.NVarChar);
      SqlDataReader reader = this.ExecuteReader();
      UpdatedRunProperties updatedRunProperties = reader.Read() ? new TestManagementDatabase60.UpdatedPropertyColumns2().bindUpdatedRunProperties(reader) : throw new UnexpectedDatabaseResultException("prc_UpdateTestRun");
      updatedRunProperties.LastUpdatedBy = updatedBy;
      if (updatedRunProperties.IsRunCompleted)
        run.CompleteDate = updatedRunProperties.CompleteDate;
      return updatedRunProperties;
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
      this.BindBoolean("@shouldPublishFlakiness", shouldPublishFlakiness);
      this.ExecuteNonQuery();
    }

    internal class UpdatedPropertyColumns2
    {
      internal SqlColumnBinder Revision = new SqlColumnBinder(nameof (Revision));
      internal SqlColumnBinder LastUpdated = new SqlColumnBinder(nameof (LastUpdated));
      internal SqlColumnBinder TotalTests = new SqlColumnBinder(nameof (TotalTests));
      internal SqlColumnBinder PassedTests = new SqlColumnBinder(nameof (PassedTests));
      internal SqlColumnBinder FailedTests = new SqlColumnBinder(nameof (FailedTests));
      internal SqlColumnBinder IsRunStarted = new SqlColumnBinder(nameof (IsRunStarted));
      internal SqlColumnBinder IsRunCompleted = new SqlColumnBinder(nameof (IsRunCompleted));
      internal SqlColumnBinder CompleteDate = new SqlColumnBinder(nameof (CompleteDate));

      internal UpdatedRunProperties bindUpdatedRunProperties(SqlDataReader reader)
      {
        UpdatedRunProperties updatedRunProperties = new UpdatedRunProperties();
        updatedRunProperties.Revision = this.Revision.GetInt32((IDataReader) reader);
        updatedRunProperties.LastUpdated = this.LastUpdated.GetDateTime((IDataReader) reader);
        updatedRunProperties.TotalTests = this.TotalTests.GetInt32((IDataReader) reader);
        updatedRunProperties.PassedTests = this.PassedTests.ColumnExists((IDataReader) reader) ? this.PassedTests.GetInt32((IDataReader) reader) : 0;
        updatedRunProperties.FailedTests = this.FailedTests.ColumnExists((IDataReader) reader) ? this.FailedTests.GetInt32((IDataReader) reader) : 0;
        updatedRunProperties.IsRunStarted = this.IsRunStarted.GetBoolean((IDataReader) reader);
        updatedRunProperties.IsRunCompleted = this.IsRunCompleted.GetBoolean((IDataReader) reader);
        updatedRunProperties.CompleteDate = this.CompleteDate.GetDateTime((IDataReader) reader);
        return updatedRunProperties;
      }
    }
  }
}
