// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.TaskAgentExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TaskAgentExtensions
  {
    public static TaskAgent TrimForRuntime(this TaskAgent agent)
    {
      if (agent == null)
        return agent;
      TaskAgent taskAgent = new TaskAgent();
      taskAgent.Id = agent.Id;
      taskAgent.Name = agent.Name;
      taskAgent.Status = agent.Status;
      taskAgent.Enabled = agent.Enabled;
      taskAgent.AssignedRequest = agent.AssignedRequest.TrimForRuntime();
      return taskAgent;
    }
  }
}
