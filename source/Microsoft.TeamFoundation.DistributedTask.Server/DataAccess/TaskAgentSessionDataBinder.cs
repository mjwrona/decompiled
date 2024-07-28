// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskAgentSessionDataBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class TaskAgentSessionDataBinder : ObjectBinder<TaskAgentSessionData>
  {
    private SqlColumnBinder m_poolId = new SqlColumnBinder("PoolId");
    private SqlColumnBinder m_sessionId = new SqlColumnBinder("SessionId");
    private SqlColumnBinder m_ownerName = new SqlColumnBinder("OwnerName");
    private SqlColumnBinder m_queueName = new SqlColumnBinder("QueueName");
    private SqlColumnBinder m_agentId = new SqlColumnBinder("AgentId");
    private SqlColumnBinder m_agentName = new SqlColumnBinder("AgentName");

    protected override TaskAgentSessionData Bind() => new TaskAgentSessionData()
    {
      PoolId = this.m_poolId.GetInt32((IDataReader) this.Reader),
      SessionId = this.m_sessionId.GetGuid((IDataReader) this.Reader),
      OwnerName = this.m_ownerName.GetString((IDataReader) this.Reader, false),
      QueueName = this.m_queueName.GetString((IDataReader) this.Reader, false),
      Agent = new TaskAgentReference()
      {
        Id = this.m_agentId.GetInt32((IDataReader) this.Reader),
        Name = this.m_agentName.GetString((IDataReader) this.Reader, false)
      }
    };
  }
}
