// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingHistoryComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicingHistoryComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<ServicingHistoryComponent>(1, true)
    }, "ServicingHistory");
    private static readonly string s_queryServicingStepGroupHistoryStmt = EmbeddedResourceUtil.GetResourceAsString("stmt_QueryServicingStepGroupHistory.sql");

    public ServicingHistoryComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual void AddServicingStepGroupHistory(
      Guid jobId,
      string servicingOperation,
      string stepGroupId,
      ServicingStepState groupState)
    {
      this.PrepareStoredProcedure("prc_AddServicingStepGroupHistory");
      this.BindGuid("@jobId", jobId);
      this.BindString("@servicingOperation", servicingOperation, 128, false, SqlDbType.NVarChar);
      this.BindString("@stepGroupId", stepGroupId, 128, false, SqlDbType.NVarChar);
      this.BindByte("@executionResult", (byte) groupState);
      this.ExecuteNonQuery();
    }

    public List<ServicingStepGroupHistoryEntry> QueryStepStepGroupHistory(Guid jobId)
    {
      this.PrepareSqlBatch(ServicingHistoryComponent.s_queryServicingStepGroupHistoryStmt.Length);
      this.AddStatement(ServicingHistoryComponent.s_queryServicingStepGroupHistoryStmt);
      this.BindGuid("@jobId", jobId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "QueryServicingStepGroupHistory", (IVssRequestContext) null))
      {
        resultCollection.AddBinder<ServicingStepGroupHistoryEntry>((ObjectBinder<ServicingStepGroupHistoryEntry>) new ServicingHistoryComponent.ServicingStepGroupHistoryBinder());
        return resultCollection.GetCurrent<ServicingStepGroupHistoryEntry>().Items;
      }
    }

    private class ServicingStepGroupHistoryBinder : ObjectBinder<ServicingStepGroupHistoryEntry>
    {
      private SqlColumnBinder m_idColumn = new SqlColumnBinder("Id");
      private SqlColumnBinder m_executionTimeColumn = new SqlColumnBinder("ExecutionTime");
      private SqlColumnBinder m_jobIdColumn = new SqlColumnBinder("JobId");
      private SqlColumnBinder m_servicingOperationColumn = new SqlColumnBinder("ServicingOperation");
      private SqlColumnBinder m_stepGroupColumn = new SqlColumnBinder("StepGroupId");
      private SqlColumnBinder m_executionResultColumn = new SqlColumnBinder("ExecutionResult");

      protected override ServicingStepGroupHistoryEntry Bind() => new ServicingStepGroupHistoryEntry()
      {
        Id = this.m_idColumn.GetInt32((IDataReader) this.Reader),
        ExecutionTime = this.m_executionTimeColumn.GetDateTime((IDataReader) this.Reader),
        JobId = this.m_jobIdColumn.GetGuid((IDataReader) this.Reader),
        ServicingOperation = this.m_servicingOperationColumn.GetString((IDataReader) this.Reader, true),
        StepGroup = this.m_stepGroupColumn.GetString((IDataReader) this.Reader, true),
        ExecutionResult = (ServicingStepState) this.m_executionResultColumn.GetByte((IDataReader) this.Reader)
      };
    }
  }
}
