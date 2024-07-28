// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.DistributedTestRunDatabase144
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class DistributedTestRunDatabase144 : DistributedTestRunDatabase
  {
    public override void CreateDistributedTestRun(
      Guid projectId,
      string environmentUri,
      string runProperties)
    {
      this.TfsRequestContext.Trace(6200852, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Creating distributed test run: environment:{0}", (object) environmentUri);
      this.PrepareStoredProcedure("prc_CreateDistributedTestRuns");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "TestManagement"));
      this.BindString("@environmentUrl", environmentUri, 2048, false, SqlDbType.NVarChar);
      this.BindString("@runProperties", runProperties, -1, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
      this.TfsRequestContext.Trace(6200860, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Successfully created distributed test run: environment:{0}", (object) environmentUri);
    }

    public override int QueryDistributedTestRun(
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
        {
          num = new SqlColumnBinder("testRunId").GetInt32((IDataReader) reader, -1, -1);
          runProperties = new SqlColumnBinder("RunProperties").GetString((IDataReader) reader, true);
        }
      }
      this.TfsRequestContext.Trace(6200880, TraceLevel.Verbose, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.DatabaseLayer, "Quering distributed test run: environment:{0}", (object) environmentUri);
      return num;
    }
  }
}
