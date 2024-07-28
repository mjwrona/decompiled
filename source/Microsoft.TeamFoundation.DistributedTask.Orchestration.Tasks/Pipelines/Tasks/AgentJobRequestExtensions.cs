// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.AgentJobRequestExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class AgentJobRequestExtensions
  {
    public static TaskAgentJobRequest TrimForRuntime(this TaskAgentJobRequest request)
    {
      if (request != null)
      {
        request.Data.Clear();
        request.Definition = (TaskOrchestrationOwner) null;
        request.Demands.Clear();
        request.JobId = Guid.Empty;
        request.JobName = (string) null;
        request.MatchedAgents.Clear();
        request.Owner = (TaskOrchestrationOwner) null;
        request.PlanId = Guid.Empty;
        request.ScopeId = Guid.Empty;
        request.ServiceOwner = Guid.Empty;
      }
      return request;
    }
  }
}
