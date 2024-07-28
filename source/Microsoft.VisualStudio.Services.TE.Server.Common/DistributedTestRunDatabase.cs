// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.DistributedTestRunDatabase
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestExecution.Server.Database.Model;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class DistributedTestRunDatabase : 
    TestExecutionServiceDatabaseBase,
    IDistributedTestRunDatabase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<DistributedTestRunDatabase144>(5),
      (IComponentCreator) new ComponentCreator<DistributedTestRunDatabase144>(6)
    }, "TestExecutionService");
    protected const string MDataspaceCategory = "TestManagement";

    public DistributedTestRunDatabase()
    {
    }

    internal DistributedTestRunDatabase(string connectionString, IVssRequestContext requestContext)
      : base(connectionString, requestContext)
    {
    }

    public virtual void CreateDistributedTestRun(
      Guid projectId,
      string environmentUri,
      string runProperties)
    {
      this.TfsRequestContext.Trace(6200852, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Creating distributed test run: environment:{0}", (object) environmentUri);
      this.PrepareStoredProcedure("prc_CreateDistributedTestRuns");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "TestManagement"));
      this.BindString("@environmentUrl", environmentUri, 2048, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
      this.TfsRequestContext.Trace(6200860, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Successfully created distributed test run: environment:{0}", (object) environmentUri);
    }

    public virtual void UpdateDistributedTestRun(
      Guid projectId,
      string environmentUri,
      int testRunId)
    {
      this.TfsRequestContext.Trace(6200862, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Updating distributed test run: environment:{0} with runId:{1}", (object) environmentUri, (object) testRunId);
      this.PrepareStoredProcedure("prc_UpdateDistributedTestRuns");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "TestManagement"));
      this.BindString("@environmentUrl", environmentUri, 2048, false, SqlDbType.NVarChar);
      this.BindInt("@testRunId", testRunId);
      this.ExecuteNonQuery();
      this.TfsRequestContext.Trace(6200870, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Successfully updated distributed test run: environment:{0} with runId:{1}", (object) environmentUri, (object) testRunId);
    }

    public virtual void DeleteDistributedTestRun(Guid projectId, string environmentUri)
    {
      this.TfsRequestContext.Trace(6200882, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Deleting distributed test run: environment:{0}", (object) environmentUri);
      this.PrepareStoredProcedure("prc_DeleteDistributedTestRuns");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "TestManagement"));
      this.BindString("@environmentUrl", environmentUri, 2048, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
      this.TfsRequestContext.Trace(6200890, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Successfully deleted distributed test run: environment:{0}", (object) environmentUri);
    }

    public virtual void DeleteDistributedTestRuns(int numberOfDaysOlder = 7)
    {
      this.TfsRequestContext.Trace(6200892, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Deleting the soft deleted distributed test runs older than: {0} days", (object) numberOfDaysOlder);
      this.PrepareStoredProcedure("prc_BulkDeleteDistributedTestRunsJob");
      this.BindInt("@waitDaysForCleanup", numberOfDaysOlder);
      this.ExecuteNonQuery();
      this.TfsRequestContext.Trace(6200900, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Deleted the distributed test runs older than: {0} days", (object) numberOfDaysOlder);
    }

    public virtual int QueryDistributedTestRun(
      Guid projectId,
      string environmentUri,
      out string runProperties)
    {
      this.TfsRequestContext.Trace(6200872, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Quering distributed test run: environment:{0}", (object) environmentUri);
      int num = -1;
      runProperties = (string) null;
      this.PrepareStoredProcedure("prc_QueryDistributedTestRuns");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "TestManagement"));
      this.BindString("@environmentUrl", environmentUri, 2048, false, SqlDbType.NVarChar);
      using (SqlDataReader reader = this.ExecuteReader())
      {
        if (reader.Read())
          num = new SqlColumnBinder("testRunId").GetInt32((IDataReader) reader, -1, -1);
      }
      this.TfsRequestContext.Trace(6200880, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Quering distributed test run: environment:{0}", (object) environmentUri);
      return num;
    }

    public virtual DistributedTestRunDbModel QueryDistributedTestRun(
      Guid projectId,
      string environmentUri)
    {
      this.TfsRequestContext.Trace(6200330, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Quering distributed test run: environment:" + environmentUri);
      DistributedTestRunDbModel distributedTestRunDbModel1 = (DistributedTestRunDbModel) null;
      this.PrepareStoredProcedure("prc_QueryDistributedTestRuns");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "TestManagement"));
      this.BindString("@environmentUrl", environmentUri, 2048, false, SqlDbType.NVarChar);
      using (SqlDataReader reader = this.ExecuteReader())
      {
        if (reader.Read())
        {
          SqlColumnBinder sqlColumnBinder = new SqlColumnBinder("CreationDate");
          if (sqlColumnBinder.ColumnExists((IDataReader) reader))
          {
            distributedTestRunDbModel1 = new DistributedTestRunDbModel();
            DistributedTestRunDbModel distributedTestRunDbModel2 = distributedTestRunDbModel1;
            sqlColumnBinder = new SqlColumnBinder("TestRunId");
            int int32 = sqlColumnBinder.GetInt32((IDataReader) reader, -1, -1);
            distributedTestRunDbModel2.TestRunId = int32;
            distributedTestRunDbModel1.EnvironmentUri = environmentUri;
            DistributedTestRunDbModel distributedTestRunDbModel3 = distributedTestRunDbModel1;
            sqlColumnBinder = new SqlColumnBinder("RunProperties");
            string str = sqlColumnBinder.GetString((IDataReader) reader, true);
            distributedTestRunDbModel3.RunProperties = str;
            DistributedTestRunDbModel distributedTestRunDbModel4 = distributedTestRunDbModel1;
            sqlColumnBinder = new SqlColumnBinder("CreationDate");
            DateTime dateTime = sqlColumnBinder.GetDateTime((IDataReader) reader);
            distributedTestRunDbModel4.CreationDate = dateTime;
          }
        }
      }
      this.TfsRequestContext.Trace(6200331, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Quering completed for distributed test run: environment:" + environmentUri);
      return distributedTestRunDbModel1;
    }

    public virtual DistributedTestRunDbModel QueryDistributedTestRun(Guid projectId, int testRunId)
    {
      this.TfsRequestContext.Trace(6200872, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Quering distributed test run: test run id:{0}", (object) testRunId);
      this.PrepareStoredProcedure("prc_QueryDistributedTestRunsByTestRunId");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "TestManagement"));
      this.BindInt("@testRunId", testRunId);
      DistributedTestRunDbModel distributedTestRunDbModel = new DistributedTestRunDbModel()
      {
        TestRunId = testRunId
      };
      using (SqlDataReader reader = this.ExecuteReader())
      {
        if (reader.Read())
        {
          distributedTestRunDbModel.EnvironmentUri = new SqlColumnBinder("EnvironmentUrl").GetString((IDataReader) reader, true);
          distributedTestRunDbModel.RunProperties = new SqlColumnBinder("RunProperties").GetString((IDataReader) reader, true);
        }
      }
      this.TfsRequestContext.Trace(6200880, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Quering distributed test run: test run id:{0}", (object) testRunId);
      return distributedTestRunDbModel;
    }
  }
}
