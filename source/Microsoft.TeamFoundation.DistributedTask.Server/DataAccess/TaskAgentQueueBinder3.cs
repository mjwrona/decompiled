// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskAgentQueueBinder3
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class TaskAgentQueueBinder3 : DistributedTaskObjectBinder<TaskAgentQueue>
  {
    private SqlColumnBinder m_queueId = new SqlColumnBinder("QueueId");
    private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder m_queueName = new SqlColumnBinder("QueueName");
    private SqlColumnBinder m_poolId = new SqlColumnBinder("PoolId");
    private SqlColumnBinder m_groupScopeId = new SqlColumnBinder("GroupScopeId");
    private SqlColumnBinder m_provisioned = new SqlColumnBinder("Provisioned");

    public TaskAgentQueueBinder3(TaskResourceComponent resourceComponent)
      : base(resourceComponent)
    {
    }

    protected override TaskAgentQueue Bind()
    {
      TaskAgentQueue taskAgentQueue = new TaskAgentQueue()
      {
        Id = this.m_queueId.GetInt32((IDataReader) this.Reader)
      };
      if (this.m_dataspaceId.ColumnExists((IDataReader) this.Reader))
        taskAgentQueue.ProjectId = this.ResourceComponent.GetDataspaceIdentifier(this.m_dataspaceId.GetInt32((IDataReader) this.Reader));
      taskAgentQueue.Name = this.m_queueName.GetString((IDataReader) this.Reader, false);
      taskAgentQueue.Pool = new TaskAgentPoolReference()
      {
        Id = this.m_poolId.GetInt32((IDataReader) this.Reader)
      };
      taskAgentQueue.GroupScopeId = this.m_groupScopeId.GetGuid((IDataReader) this.Reader, true, Guid.Empty);
      taskAgentQueue.Provisioned = this.m_provisioned.GetBoolean((IDataReader) this.Reader, false);
      return taskAgentQueue;
    }
  }
}
