// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase38
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase38 : TestManagementDatabase37
  {
    public override BuildConfiguration QueryBuildConfigurationById2(
      int buildConfigurationid,
      Guid projectId)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.QueryBuildConfigurationById2");
        BuildConfiguration buildConfiguration = new BuildConfiguration();
        int dataspaceId = this.GetDataspaceIdWithLazyInitialization(projectId);
        this.PrepareStoredProcedure("prc_QueryBuildConfigurationById");
        this.BindInt("@dataspaceId", dataspaceId);
        this.BindInt("@buildConfigurationId", buildConfigurationid);
        SqlDataReader reader = this.ExecuteReader();
        if (reader.Read())
          buildConfiguration = new TestManagementDatabase38.QueryBuildConfigurationsColumns().bind(reader, out dataspaceId);
        return buildConfiguration;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.QueryBuildConfigurationById");
      }
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
        this.BindInt("@dataspaceId", 0);
        this.BindInt("@buildConfigurationId", buildConfigurationid);
        SqlDataReader reader = this.ExecuteReader();
        if (reader.Read())
        {
          int dataspaceId;
          buildConfiguration = new TestManagementDatabase38.QueryBuildConfigurationsColumns().bind(reader, out dataspaceId);
          projectId = this.GetDataspaceIdentifier(dataspaceId);
        }
        return buildConfiguration;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "BuildReferenceDatabase.QueryBuildConfigurationById");
      }
    }

    internal TestManagementDatabase38(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase38()
    {
    }

    internal override List<TestCaseResult> QueryTestResultsByBuildOrRelease(
      Guid projectId,
      int buildId,
      int releaseId,
      int releaseEnvId,
      string sourceWorkflow,
      IList<byte> runStates,
      bool fetchOnlyFailedTests,
      int continuationTokenRunId,
      int continuationTokenResultId,
      int top)
    {
      this.PrepareStoredProcedure("TestResult.prc_QueryTestResultsByBuildOrRelease");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@buildId", buildId);
      this.BindInt("@releaseId", releaseId);
      this.BindInt("@releaseEnvId", releaseEnvId);
      this.BindString("@sourceWorkflow", sourceWorkflow, 128, false, SqlDbType.NVarChar);
      this.BindBoolean("@fetchOnlyFailedTests", fetchOnlyFailedTests);
      this.BindTestManagement_TinyIntTypeTable("@runStates", (IEnumerable<byte>) runStates);
      this.BindInt("@continuationTokenRunId", continuationTokenRunId);
      this.BindInt("@continuationTokenResultId", continuationTokenResultId);
      this.BindInt("@top", top);
      SqlDataReader reader = this.ExecuteReader();
      List<TestCaseResult> testCaseResultList = new List<TestCaseResult>();
      List<TestCaseResult> testResults = testCaseResultList;
      TestManagementDatabase36.GetTestResultsBind(reader, testResults);
      return testCaseResultList;
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
      private SqlColumnBinder RepoId = new SqlColumnBinder(nameof (RepoId));
      private SqlColumnBinder RepoType = new SqlColumnBinder(nameof (RepoType));
      private SqlColumnBinder BranchName = new SqlColumnBinder(nameof (BranchName));
      private SqlColumnBinder SourceVersion = new SqlColumnBinder(nameof (SourceVersion));
      private SqlColumnBinder BuildSystem = new SqlColumnBinder(nameof (BuildSystem));
      private SqlColumnBinder CreatedDate = new SqlColumnBinder(nameof (CreatedDate));

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
          BuildSystem = this.BuildSystem.GetString((IDataReader) reader, false),
          CreatedDate = this.CreatedDate.ColumnExists((IDataReader) reader) ? this.CreatedDate.GetDateTime((IDataReader) reader) : new DateTime(),
          RepositoryId = this.RepoId.GetString((IDataReader) reader, false),
          RepositoryType = this.RepoType.GetString((IDataReader) reader, false)
        };
      }
    }
  }
}
