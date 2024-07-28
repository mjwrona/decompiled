// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase51
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
  public class TestManagementDatabase51 : TestManagementDatabase50
  {
    internal TestManagementDatabase51(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase51()
    {
    }

    public override List<TestRun> QueryTestRunsbyFilters(
      Guid projectId,
      QueryTestRunsFilter filters,
      int top,
      int batchSize,
      out int minNextBatchRunId,
      out DateTime minNextBatchLastUpdated)
    {
      try
      {
        minNextBatchRunId = -1;
        minNextBatchLastUpdated = DateTime.MaxValue;
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRunsbyFilters");
        Dictionary<int, TestRun> dictionary = new Dictionary<int, TestRun>();
        this.PrepareStoredProcedure("TestResult.prc_QueryTestRuns");
        this.BindIdTypeTable("@planIds", (IEnumerable<int>) filters.PlanIds);
        this.BindNullableInt("@state", filters.State, -1);
        DateTime? nullable;
        if (filters.MinLastUpdatedDate.HasValue && filters.MinLastUpdatedDate.HasValue)
        {
          nullable = filters.MinLastUpdatedDate;
          this.BindNullableDateTime("@minLastUpdatedDate", nullable.Value);
        }
        else
          this.BindNullableDateTime("@minLastUpdatedDate", filters.MinLastUpdatedDate);
        nullable = filters.MaxLastUpdatedDate;
        if (nullable.HasValue)
        {
          nullable = filters.MaxLastUpdatedDate;
          if (nullable.HasValue)
          {
            nullable = filters.MaxLastUpdatedDate;
            this.BindNullableDateTime("@maxLastUpdatedDate", nullable.Value);
            goto label_7;
          }
        }
        this.BindNullableDateTime("@maxLastUpdatedDate", filters.MaxLastUpdatedDate);
label_7:
        this.BindNullableBoolean("@isAutomated", filters.IsAutomated);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindIdTypeTable("@buildIds", (IEnumerable<int>) filters.BuildIds);
        this.BindIdTypeTable("@buildDefIds", (IEnumerable<int>) filters.BuildDefIds);
        this.BindIdTypeTable("@releaseIds", (IEnumerable<int>) filters.ReleaseIds);
        this.BindIdTypeTable("@releaseDefIds", (IEnumerable<int>) filters.ReleaseDefIds);
        this.BindIdTypeTable("@releaseEnvIds", (IEnumerable<int>) filters.ReleaseEnvIds);
        this.BindIdTypeTable("@releaseEnvDefIds", (IEnumerable<int>) filters.ReleaseEnvDefIds);
        this.BindString("@branchName", filters.BranchName, 400, true, SqlDbType.NVarChar);
        this.BindString("@runTitle", filters.RunTitle, 256, true, SqlDbType.NVarChar);
        this.BindString("@sourceWorkflow", filters.SourceWorkflow, 128, true, SqlDbType.NVarChar);
        this.BindInt("@top", top);
        this.BindInt("@batchSize", batchSize);
        TestManagementDatabase29.QueryTestRunColumnsByFilters columnsByFilters = new TestManagementDatabase29.QueryTestRunColumnsByFilters();
        SqlDataReader reader = this.ExecuteReader();
        while (reader.Read())
        {
          TestRun testRun = columnsByFilters.bind(reader);
          dictionary.Add(testRun.TestRunId, testRun);
        }
        if (reader.NextResult() && reader.Read())
        {
          DateTime minLastUpdatedDate;
          new TestManagementDatabase51.QueryRunsByFilterBatchingDetails().bind(reader, out minLastUpdatedDate);
          minNextBatchLastUpdated = minLastUpdatedDate;
        }
        return dictionary.Values.ToList<TestRun>();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryTestRunsbyFilters");
      }
    }

    public override Dictionary<TestArtifactDataspaceIdMap, bool> QueryBuildIdsPresent(
      List<TestArtifactDataspaceIdMap> dataSpaceBuildIds)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.IsBuildIdsPresent");
        this.PrepareStoredProcedure("prc_findTestBuildsToDelete");
        this.BindTestDataCleanupIds("@buildIdTable", dataSpaceBuildIds);
        Dictionary<TestArtifactDataspaceIdMap, bool> results = new Dictionary<TestArtifactDataspaceIdMap, bool>();
        SqlDataReader reader = this.ExecuteReader();
        if (!reader.Read())
        {
          dataSpaceBuildIds.ForEach((Action<TestArtifactDataspaceIdMap>) (x =>
          {
            if (results.ContainsKey(x))
              return;
            results.Add(x, false);
          }));
          return results;
        }
        TestManagementDatabase51.BuildIdsPresentResultColumn presentResultColumn = new TestManagementDatabase51.BuildIdsPresentResultColumn();
        do
        {
          TestArtifactDataspaceIdMap key = presentResultColumn.bind(reader);
          if (!results.ContainsKey(key))
            results.Add(key, true);
        }
        while (reader.Read());
        dataSpaceBuildIds.ForEach((Action<TestArtifactDataspaceIdMap>) (x =>
        {
          if (results.ContainsKey(x))
            return;
          results.Add(x, false);
        }));
        return results;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.IsBuildIdsPresent");
      }
    }

    public override Dictionary<TestArtifactDataspaceIdMap, bool> QueryRunIdsPresent(
      List<TestArtifactDataspaceIdMap> dataSpaceRunIds)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.IsRunIdsPresent");
        this.PrepareStoredProcedure("prc_findTestRunsToDelete");
        this.BindTestDataCleanupIds("@runIdTable", dataSpaceRunIds);
        Dictionary<TestArtifactDataspaceIdMap, bool> results = new Dictionary<TestArtifactDataspaceIdMap, bool>();
        SqlDataReader reader = this.ExecuteReader();
        if (!reader.Read())
        {
          dataSpaceRunIds.ForEach((Action<TestArtifactDataspaceIdMap>) (x =>
          {
            if (results.ContainsKey(x))
              return;
            results.Add(x, false);
          }));
          return results;
        }
        TestManagementDatabase51.TestRunIdsPresentResultColumn presentResultColumn = new TestManagementDatabase51.TestRunIdsPresentResultColumn();
        do
        {
          TestArtifactDataspaceIdMap key = presentResultColumn.bind(reader);
          if (!results.ContainsKey(key))
            results.Add(key, true);
        }
        while (reader.Read());
        dataSpaceRunIds.ForEach((Action<TestArtifactDataspaceIdMap>) (x =>
        {
          if (results.ContainsKey(x))
            return;
          results.Add(x, false);
        }));
        return results;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.IsRunIdsPresent");
      }
    }

    protected class BuildIdsPresentResultColumn
    {
      private SqlColumnBinder buildId = new SqlColumnBinder("BuildId");
      private SqlColumnBinder dataSpaceId = new SqlColumnBinder("DataspaceId");

      internal TestArtifactDataspaceIdMap bind(SqlDataReader reader) => new TestArtifactDataspaceIdMap(this.buildId.GetInt32((IDataReader) reader), this.dataSpaceId.GetInt32((IDataReader) reader));
    }

    protected class TestRunIdsPresentResultColumn
    {
      private SqlColumnBinder runId = new SqlColumnBinder("TestRunId");
      private SqlColumnBinder dataSpaceId = new SqlColumnBinder("DataspaceId");

      internal TestArtifactDataspaceIdMap bind(SqlDataReader reader) => new TestArtifactDataspaceIdMap(this.runId.GetInt32((IDataReader) reader), this.dataSpaceId.GetInt32((IDataReader) reader));
    }

    private new class QueryRunsByFilterBatchingDetails
    {
      private SqlColumnBinder MinLastUpdatedDate = new SqlColumnBinder(nameof (MinLastUpdatedDate));

      internal virtual void bind(SqlDataReader reader, out DateTime minLastUpdatedDate) => minLastUpdatedDate = this.MinLastUpdatedDate.GetDateTime((IDataReader) reader);
    }
  }
}
