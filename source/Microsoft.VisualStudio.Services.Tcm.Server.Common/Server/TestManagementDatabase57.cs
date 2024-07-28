// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase57
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
  public class TestManagementDatabase57 : TestManagementDatabase56
  {
    internal TestManagementDatabase57(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase57()
    {
    }

    public override List<TestRun> GetTestRunIdsWithoutInsightsForBuild(Guid projectId, int buildId)
    {
      this.PrepareStoredProcedure("TestResult.prc_GetTestRunIdsWithoutInsightsForBuild");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@buildId", buildId);
      return new TestManagementDatabase57.FetchTestRunsByBuild().bind(this.ExecuteReader());
    }

    public override List<TestRun> GetTestRunIdsWithoutInsightsForRelease(
      Guid projectId,
      int releaseId)
    {
      this.PrepareStoredProcedure("TestResult.prc_GetTestRunIdsWithoutInsightsForRelease");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@releaseId", releaseId);
      return new TestManagementDatabase57.FetchTestRunsByRelease().bind(this.ExecuteReader());
    }

    protected class FetchTestRunsByBuild
    {
      private SqlColumnBinder testRunIdBinder = new SqlColumnBinder("TestRunId");
      private SqlColumnBinder testRunTypeBinder = new SqlColumnBinder("TestRunType");
      private SqlColumnBinder buildConfigurationIdBinder = new SqlColumnBinder("BuildConfigurationId");
      private SqlColumnBinder buildIdBinder = new SqlColumnBinder("BuildId");
      private SqlColumnBinder buildNumberBinder = new SqlColumnBinder("BuildNumber");
      private SqlColumnBinder buildUriBinder = new SqlColumnBinder("BuildUri");
      private SqlColumnBinder branchNameBinder = new SqlColumnBinder("BranchName");
      private SqlColumnBinder createdDateBinder = new SqlColumnBinder("CreatedDate");

      internal List<TestRun> bind(SqlDataReader sqlDataReader)
      {
        List<TestRun> testRunList = new List<TestRun>();
        while (sqlDataReader.Read())
          testRunList.Add(new TestRun()
          {
            TestRunId = this.testRunIdBinder.GetInt32((IDataReader) sqlDataReader),
            Type = this.testRunTypeBinder.GetByte((IDataReader) sqlDataReader),
            BuildReference = new BuildConfiguration()
            {
              BuildConfigurationId = this.buildConfigurationIdBinder.GetInt32((IDataReader) sqlDataReader),
              BuildId = this.buildIdBinder.GetInt32((IDataReader) sqlDataReader),
              BuildNumber = this.buildNumberBinder.GetString((IDataReader) sqlDataReader, false),
              BuildUri = this.buildUriBinder.GetString((IDataReader) sqlDataReader, false),
              BranchName = this.branchNameBinder.GetString((IDataReader) sqlDataReader, false),
              CreatedDate = this.createdDateBinder.GetDateTime((IDataReader) sqlDataReader, new DateTime())
            }
          });
        return testRunList;
      }
    }

    protected class FetchTestRunsByRelease
    {
      private SqlColumnBinder testRunIdBinder = new SqlColumnBinder("TestRunId");
      private SqlColumnBinder testRunTypeBinder = new SqlColumnBinder("TestRunType");
      private SqlColumnBinder releaseRefIdBinder = new SqlColumnBinder("ReleaseRefId");
      private SqlColumnBinder releaseUriBinder = new SqlColumnBinder("ReleaseUri");
      private SqlColumnBinder releaseEnvUriBinder = new SqlColumnBinder("ReleaseEnvUri");
      private SqlColumnBinder releaseIdBinder = new SqlColumnBinder("ReleaseId");
      private SqlColumnBinder releaseEnvIdBinder = new SqlColumnBinder("ReleaseEnvId");
      private SqlColumnBinder attemptBinder = new SqlColumnBinder("Attempt");
      private SqlColumnBinder releaseCreationDateBinder = new SqlColumnBinder("ReleaseCreationDate");
      private SqlColumnBinder releaseEnvDefIdBinder = new SqlColumnBinder("ReleaseEnvDefId");
      private SqlColumnBinder releaseDefIdBinder = new SqlColumnBinder("ReleaseDefId");
      private SqlColumnBinder releaseNameBinder = new SqlColumnBinder("ReleaseName");

      internal List<TestRun> bind(SqlDataReader sqlDataReader)
      {
        List<TestRun> testRunList = new List<TestRun>();
        while (sqlDataReader.Read())
          testRunList.Add(new TestRun()
          {
            TestRunId = this.testRunIdBinder.GetInt32((IDataReader) sqlDataReader),
            Type = this.testRunTypeBinder.GetByte((IDataReader) sqlDataReader),
            ReleaseReference = new ReleaseReference()
            {
              ReleaseRefId = this.releaseRefIdBinder.GetInt32((IDataReader) sqlDataReader),
              ReleaseUri = this.releaseUriBinder.GetString((IDataReader) sqlDataReader, false),
              ReleaseEnvUri = this.releaseEnvUriBinder.GetString((IDataReader) sqlDataReader, false),
              ReleaseId = this.releaseIdBinder.GetInt32((IDataReader) sqlDataReader),
              ReleaseEnvId = this.releaseEnvIdBinder.GetInt32((IDataReader) sqlDataReader),
              Attempt = this.attemptBinder.GetInt32((IDataReader) sqlDataReader),
              ReleaseCreationDate = this.releaseCreationDateBinder.GetDateTime((IDataReader) sqlDataReader, new DateTime()),
              ReleaseEnvDefId = this.releaseEnvDefIdBinder.GetInt32((IDataReader) sqlDataReader),
              ReleaseDefId = this.releaseDefIdBinder.GetInt32((IDataReader) sqlDataReader),
              ReleaseName = this.releaseNameBinder.GetString((IDataReader) sqlDataReader, false)
            }
          });
        return testRunList;
      }
    }
  }
}
