// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.AgentConnecteEventBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Tasks;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class AgentConnecteEventBinder : ObjectBinder<AgentConnectedEvent>
  {
    private SqlColumnBinder m_connectedOn = new SqlColumnBinder("ConnectedOn");

    protected override AgentConnectedEvent Bind()
    {
      AgentConnectedEvent agentEvent = new AgentConnectedEvent();
      RunAgentEventBinderHelper.BindRunAgentEvent((RunAgentEvent) agentEvent, this.Reader);
      agentEvent.ConnectedOn = this.m_connectedOn.GetDateTime((IDataReader) this.Reader);
      return agentEvent;
    }
  }
}
