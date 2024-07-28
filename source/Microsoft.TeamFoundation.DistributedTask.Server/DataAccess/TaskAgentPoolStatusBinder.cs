// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskAgentPoolStatusBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class TaskAgentPoolStatusBinder : ObjectBinder<TaskAgentPoolStatus>
  {
    private Guid m_scopeId;
    private SqlColumnBinder m_id = new SqlColumnBinder("PoolId");
    private SqlColumnBinder m_name = new SqlColumnBinder("PoolName");
    private SqlColumnBinder m_isHosted = new SqlColumnBinder("IsHosted");
    private SqlColumnBinder m_poolType = new SqlColumnBinder("PoolType");
    private SqlColumnBinder m_size = new SqlColumnBinder("Size");
    private SqlColumnBinder m_queuedRequestCount = new SqlColumnBinder("QueuedRequestCount");
    private SqlColumnBinder m_assignedRequestCount = new SqlColumnBinder("AssignedRequestCount");
    private SqlColumnBinder m_runningRequestCount = new SqlColumnBinder("RunningRequestCount");

    public TaskAgentPoolStatusBinder(IVssRequestContext requestContext) => this.m_scopeId = requestContext.ServiceHost.InstanceId;

    protected override TaskAgentPoolStatus Bind()
    {
      TaskAgentPoolStatus taskAgentPoolStatus = new TaskAgentPoolStatus();
      taskAgentPoolStatus.Id = this.m_id.GetInt32((IDataReader) this.Reader);
      taskAgentPoolStatus.Name = this.m_name.GetString((IDataReader) this.Reader, false);
      taskAgentPoolStatus.IsHosted = this.m_isHosted.GetBoolean((IDataReader) this.Reader, false);
      if (this.m_poolType.ColumnExists((IDataReader) this.Reader))
        taskAgentPoolStatus.PoolType = (TaskAgentPoolType) this.m_poolType.GetByte((IDataReader) this.Reader);
      taskAgentPoolStatus.Size = this.m_size.GetInt32((IDataReader) this.Reader);
      taskAgentPoolStatus.QueuedRequestCount = this.m_queuedRequestCount.GetInt32((IDataReader) this.Reader, 0);
      taskAgentPoolStatus.AssignedRequestCount = this.m_assignedRequestCount.GetInt32((IDataReader) this.Reader, 0);
      taskAgentPoolStatus.RunningRequestCount = this.m_runningRequestCount.GetInt32((IDataReader) this.Reader, 0);
      return taskAgentPoolStatus;
    }
  }
}
