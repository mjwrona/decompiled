// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskAgentQueueBinder2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class TaskAgentQueueBinder2 : ObjectBinder<TaskAgentQueue>
  {
    private SqlColumnBinder m_queueId = new SqlColumnBinder("QueueId");
    private SqlColumnBinder m_queueName = new SqlColumnBinder("QueueName");
    private SqlColumnBinder m_poolId = new SqlColumnBinder("PoolId");
    private SqlColumnBinder m_groupScopeId = new SqlColumnBinder("GroupScopeId");

    protected override TaskAgentQueue Bind()
    {
      TaskAgentQueue taskAgentQueue = new TaskAgentQueue()
      {
        Id = this.m_queueId.GetInt32((IDataReader) this.Reader),
        Name = this.m_queueName.GetString((IDataReader) this.Reader, false),
        Pool = new TaskAgentPoolReference()
        {
          Id = this.m_poolId.GetInt32((IDataReader) this.Reader)
        }
      };
      Guid? nullableGuid = this.m_groupScopeId.GetNullableGuid((IDataReader) this.Reader);
      if (nullableGuid.HasValue)
        taskAgentQueue.GroupScopeId = nullableGuid.Value;
      return taskAgentQueue;
    }
  }
}
