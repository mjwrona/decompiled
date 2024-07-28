// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.QueueAgentBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class QueueAgentBinder : DistributedTaskObjectBinder<QueueAgent>
  {
    private SqlColumnBinder m_dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder m_queueAgentId = new SqlColumnBinder("QueueAgentId");
    private SqlColumnBinder m_queueId = new SqlColumnBinder("QueueId");
    private SqlColumnBinder m_agentId = new SqlColumnBinder("AgentId");

    public QueueAgentBinder(TaskResourceComponent resourceComponent)
      : base(resourceComponent)
    {
    }

    protected override QueueAgent Bind()
    {
      QueueAgent queueAgent = new QueueAgent()
      {
        QueueAgentId = this.m_queueAgentId.GetInt32((IDataReader) this.Reader),
        QueueId = this.m_queueId.GetInt32((IDataReader) this.Reader),
        AgentId = this.m_agentId.GetInt32((IDataReader) this.Reader)
      };
      if (this.m_dataspaceId.ColumnExists((IDataReader) this.Reader) && !this.m_dataspaceId.IsNull((IDataReader) this.Reader))
        queueAgent.Project = new ProjectReference()
        {
          Id = this.ResourceComponent.GetDataspaceIdentifier(this.m_dataspaceId.GetInt32((IDataReader) this.Reader))
        };
      return queueAgent;
    }
  }
}
