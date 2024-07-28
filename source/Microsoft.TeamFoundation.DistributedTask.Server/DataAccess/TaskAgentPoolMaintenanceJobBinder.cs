// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskAgentPoolMaintenanceJobBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class TaskAgentPoolMaintenanceJobBinder : ObjectBinder<TaskAgentPoolMaintenanceJob>
  {
    private SqlColumnBinder m_poolId = new SqlColumnBinder("PoolId");
    private SqlColumnBinder m_definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder m_jobId = new SqlColumnBinder("JobId");
    private SqlColumnBinder m_orchestrationId = new SqlColumnBinder("OrchestrationId");
    private SqlColumnBinder m_status = new SqlColumnBinder("Status");
    private SqlColumnBinder m_result = new SqlColumnBinder("Result");
    private SqlColumnBinder m_queueTime = new SqlColumnBinder("QueueTime");
    private SqlColumnBinder m_startTime = new SqlColumnBinder("StartTime");
    private SqlColumnBinder m_finishTime = new SqlColumnBinder("FinishTime");
    private SqlColumnBinder m_requestedBy = new SqlColumnBinder("RequestedBy");

    protected override TaskAgentPoolMaintenanceJob Bind()
    {
      TaskAgentPoolMaintenanceJob poolMaintenanceJob = new TaskAgentPoolMaintenanceJob();
      poolMaintenanceJob.Pool = new TaskAgentPoolReference()
      {
        Id = this.m_poolId.GetInt32((IDataReader) this.Reader)
      };
      poolMaintenanceJob.DefinitionId = this.m_definitionId.GetInt32((IDataReader) this.Reader);
      poolMaintenanceJob.JobId = this.m_jobId.GetInt32((IDataReader) this.Reader);
      poolMaintenanceJob.OrchestrationId = this.m_orchestrationId.GetGuid((IDataReader) this.Reader);
      poolMaintenanceJob.Status = (TaskAgentPoolMaintenanceJobStatus) this.m_status.GetByte((IDataReader) this.Reader);
      if (!this.m_result.IsNull((IDataReader) this.Reader))
        poolMaintenanceJob.Result = new TaskAgentPoolMaintenanceJobResult?((TaskAgentPoolMaintenanceJobResult) this.m_result.GetByte((IDataReader) this.Reader));
      poolMaintenanceJob.QueueTime = this.m_queueTime.GetNullableDateTime((IDataReader) this.Reader);
      poolMaintenanceJob.StartTime = this.m_startTime.GetNullableDateTime((IDataReader) this.Reader);
      poolMaintenanceJob.FinishTime = this.m_finishTime.GetNullableDateTime((IDataReader) this.Reader);
      poolMaintenanceJob.RequestedBy = new IdentityRef()
      {
        Id = this.m_requestedBy.GetGuid((IDataReader) this.Reader).ToString("D")
      };
      return poolMaintenanceJob;
    }
  }
}
