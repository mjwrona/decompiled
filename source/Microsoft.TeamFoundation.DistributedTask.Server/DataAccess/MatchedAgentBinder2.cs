// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.MatchedAgentBinder2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class MatchedAgentBinder2 : ObjectBinder<MatchedAgent>
  {
    private SqlColumnBinder m_requestId = new SqlColumnBinder("RequestId");
    private SqlColumnBinder m_agentId = new SqlColumnBinder("AgentId");
    private SqlColumnBinder m_agentName = new SqlColumnBinder("AgentName");
    private SqlColumnBinder m_agentVersion = new SqlColumnBinder("AgentVersion");
    private SqlColumnBinder m_agentEnabled = new SqlColumnBinder("AgentEnabled");
    private SqlColumnBinder m_agentStatus = new SqlColumnBinder("AgentStatus");

    protected override MatchedAgent Bind() => new MatchedAgent()
    {
      RequestId = this.m_requestId.GetInt64((IDataReader) this.Reader),
      Agent = new TaskAgentReference()
      {
        Id = this.m_agentId.GetInt32((IDataReader) this.Reader),
        Name = this.m_agentName.GetString((IDataReader) this.Reader, false),
        Version = this.m_agentVersion.GetString((IDataReader) this.Reader, false),
        Enabled = new bool?(this.m_agentEnabled.GetBoolean((IDataReader) this.Reader)),
        Status = (TaskAgentStatus) this.m_agentStatus.GetInt32((IDataReader) this.Reader),
        ProvisioningState = "Provisioned"
      }
    };
  }
}
