// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.CreateAgentSessionResult
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Tasks;
using Microsoft.TeamFoundation.DistributedTask.WebApi;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class CreateAgentSessionResult
  {
    internal CreateAgentSessionResult() => this.ComponentRetrunsAgentCloud = false;

    public bool RecalculateRequestMatches { get; set; }

    public TaskAgent Agent { get; set; }

    public TaskAgentSessionData NewSession { get; set; }

    public TaskAgentSessionData OldSession { get; set; }

    public TaskAgentJobRequest AssignedRequest { get; set; }

    public bool ComponentRetrunsAgentCloud { get; set; }

    public TaskAgentCloud AssignedRequestAgentCloud { get; set; }

    public AgentConnectedEvent OrchestrationEvent { get; set; }
  }
}
