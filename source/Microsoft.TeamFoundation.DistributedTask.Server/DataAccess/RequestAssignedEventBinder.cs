// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.RequestAssignedEventBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Tasks;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class RequestAssignedEventBinder : ObjectBinder<RequestAssignedEvent>
  {
    private SqlColumnBinder m_requestId = new SqlColumnBinder("RequestId");

    protected override RequestAssignedEvent Bind()
    {
      RequestAssignedEvent agentEvent = new RequestAssignedEvent();
      RunAgentEventBinderHelper.BindRunAgentEvent((RunAgentEvent) agentEvent, this.Reader);
      agentEvent.RequestId = this.m_requestId.GetInt64((IDataReader) this.Reader);
      return agentEvent;
    }
  }
}
