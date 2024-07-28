// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.AgentPoolQueueBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class AgentPoolQueueBinder : ObjectBinder<AgentPoolQueue>
  {
    private SqlColumnBinder m_queueId = new SqlColumnBinder("QueueId");
    private SqlColumnBinder m_queueName = new SqlColumnBinder("QueueName");
    private SqlColumnBinder m_poolId = new SqlColumnBinder("PoolId");

    protected override AgentPoolQueue Bind() => new AgentPoolQueue()
    {
      Id = this.m_queueId.GetInt32((IDataReader) this.Reader),
      Name = DBHelper.DBPathToServerPath(this.m_queueName.GetString((IDataReader) this.Reader, false)),
      Pool = new TaskAgentPoolReference()
      {
        Id = this.m_poolId.GetInt32((IDataReader) this.Reader)
      }
    };
  }
}
