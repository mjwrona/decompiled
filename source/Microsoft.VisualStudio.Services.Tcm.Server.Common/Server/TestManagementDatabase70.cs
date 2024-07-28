// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase70
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase70 : TestManagementDatabase69
  {
    internal TestManagementDatabase70(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase70()
    {
    }

    public override TestRunContextBackfillStatus BackfillTestRunContextId(
      int waterMarkDataspaceId,
      int waterMarkTestRunId,
      int batchSize = 10000)
    {
      this.PrepareStoredProcedure("TestResult.prc_BackfillRunContextId");
      this.BindInt("@waterMarkDataspaceId", waterMarkDataspaceId);
      this.BindInt("@waterMarkTestRunId", waterMarkTestRunId);
      this.BindInt("@batchSize", batchSize);
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase70.BackfillTestRunContextIdColumns contextIdColumns = new TestManagementDatabase70.BackfillTestRunContextIdColumns();
      TestRunContextBackfillStatus contextBackfillStatus = new TestRunContextBackfillStatus();
      if (reader.Read())
      {
        contextBackfillStatus = contextIdColumns.bind(reader, batchSize);
      }
      else
      {
        contextBackfillStatus.IsComplete = true;
        contextBackfillStatus.LastDataspaceId = -1;
        contextBackfillStatus.LastTestRunId = -1;
      }
      return contextBackfillStatus;
    }

    private class BackfillTestRunContextIdColumns
    {
      private SqlColumnBinder BatchCountCol = new SqlColumnBinder("BatchCount");
      private SqlColumnBinder InsertedRunContextCountCol = new SqlColumnBinder("InsertedRunContextCount");
      private SqlColumnBinder UpdatedRunsCountCol = new SqlColumnBinder("UpdatedRunsCount");
      private SqlColumnBinder LastDataspaceIdCol = new SqlColumnBinder("LastDataspaceId");
      private SqlColumnBinder LastTestRunIdCol = new SqlColumnBinder("LastTestRunId");

      internal TestRunContextBackfillStatus bind(SqlDataReader reader, int batchSize)
      {
        TestRunContextBackfillStatus contextBackfillStatus = new TestRunContextBackfillStatus();
        contextBackfillStatus.IsComplete = false;
        contextBackfillStatus.BatchCount = this.BatchCountCol.GetInt32((IDataReader) reader);
        contextBackfillStatus.InsertedRunContextCount = this.InsertedRunContextCountCol.GetInt32((IDataReader) reader);
        contextBackfillStatus.UpdatedRunsCount = this.UpdatedRunsCountCol.GetInt32((IDataReader) reader);
        if (contextBackfillStatus.BatchCount < batchSize)
        {
          contextBackfillStatus.IsComplete = true;
          contextBackfillStatus.LastDataspaceId = -1;
          contextBackfillStatus.LastTestRunId = -1;
        }
        else
        {
          contextBackfillStatus.LastDataspaceId = this.LastDataspaceIdCol.GetInt32((IDataReader) reader, 0);
          contextBackfillStatus.LastTestRunId = this.LastTestRunIdCol.GetInt32((IDataReader) reader, 0);
        }
        return contextBackfillStatus;
      }
    }
  }
}
