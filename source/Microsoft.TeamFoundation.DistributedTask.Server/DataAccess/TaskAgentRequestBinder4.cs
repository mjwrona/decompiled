// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskAgentRequestBinder4
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class TaskAgentRequestBinder4 : ObjectBinder<TaskAgentJobRequest>
  {
    private SqlColumnBinder m_requestId = new SqlColumnBinder("RequestId");
    private SqlColumnBinder m_queueTime = new SqlColumnBinder("QueueTime");
    private SqlColumnBinder m_assignTime = new SqlColumnBinder("AssignTime");
    private SqlColumnBinder m_startTime = new SqlColumnBinder("ReceiveTime");
    private SqlColumnBinder m_finishTime = new SqlColumnBinder("FinishTime");
    private SqlColumnBinder m_result = new SqlColumnBinder("Result");
    private SqlColumnBinder m_lockToken = new SqlColumnBinder("LockToken");
    private SqlColumnBinder m_lockedUntil = new SqlColumnBinder("LockedUntil");
    private SqlColumnBinder m_serviceOwner = new SqlColumnBinder("ServiceOwner");
    private SqlColumnBinder m_hostId = new SqlColumnBinder("HostId");
    private SqlColumnBinder m_planId = new SqlColumnBinder("PlanId");
    private SqlColumnBinder m_jobId = new SqlColumnBinder("JobId");
    private SqlColumnBinder m_demands = new SqlColumnBinder("Demands");
    private SqlColumnBinder m_agentId = new SqlColumnBinder("AgentId");
    private SqlColumnBinder m_agentName = new SqlColumnBinder("AgentName");

    protected override TaskAgentJobRequest Bind()
    {
      TaskAgentJobRequest taskAgentJobRequest1 = new TaskAgentJobRequest();
      taskAgentJobRequest1.RequestId = this.m_requestId.GetInt64((IDataReader) this.Reader);
      taskAgentJobRequest1.QueueTime = this.m_queueTime.GetDateTime((IDataReader) this.Reader);
      taskAgentJobRequest1.AssignTime = this.m_assignTime.GetNullableDateTime((IDataReader) this.Reader);
      taskAgentJobRequest1.ReceiveTime = this.m_startTime.GetNullableDateTime((IDataReader) this.Reader);
      taskAgentJobRequest1.FinishTime = this.m_finishTime.GetNullableDateTime((IDataReader) this.Reader);
      TaskAgentJobRequest taskAgentJobRequest2 = taskAgentJobRequest1;
      byte? nullableByte = this.m_result.GetNullableByte((IDataReader) this.Reader);
      TaskResult? nullable = nullableByte.HasValue ? new TaskResult?((TaskResult) nullableByte.GetValueOrDefault()) : new TaskResult?();
      taskAgentJobRequest2.Result = nullable;
      taskAgentJobRequest1.LockToken = this.m_lockToken.GetNullableGuid((IDataReader) this.Reader);
      taskAgentJobRequest1.LockedUntil = this.m_lockedUntil.GetNullableDateTime((IDataReader) this.Reader);
      taskAgentJobRequest1.ServiceOwner = this.m_serviceOwner.GetGuid((IDataReader) this.Reader, false);
      taskAgentJobRequest1.HostId = this.m_hostId.GetGuid((IDataReader) this.Reader);
      taskAgentJobRequest1.PlanId = this.m_planId.GetGuid((IDataReader) this.Reader);
      taskAgentJobRequest1.JobId = this.m_jobId.GetGuid((IDataReader) this.Reader);
      taskAgentJobRequest1.Demands = JsonUtility.Deserialize<IList<Demand>>(this.m_demands.GetBytes((IDataReader) this.Reader, true));
      if (!this.m_agentId.IsNull((IDataReader) this.Reader))
        taskAgentJobRequest1.ReservedAgent = new TaskAgentReference()
        {
          Id = this.m_agentId.GetInt32((IDataReader) this.Reader),
          Name = this.m_agentName.GetString((IDataReader) this.Reader, false)
        };
      return taskAgentJobRequest1;
    }
  }
}
