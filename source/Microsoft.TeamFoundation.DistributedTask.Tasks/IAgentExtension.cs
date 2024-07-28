// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Tasks.IAgentExtension
// Assembly: Microsoft.TeamFoundation.DistributedTask.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 691D8169-F87B-47FC-8906-5680483E9D38
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Tasks
{
  public interface IAgentExtension
  {
    Task ClearAgentSlot(int poolId, int agentId);

    Task<TaskAgent> GetAgentAsync(int poolId, int agentId);

    Task<TaskAgentPool> GetAgentPoolAsync(int poolId);

    Task NotifyAgentReadyAsync(int poolId, long requestId);

    Task RunAgentAssignementAsync();

    Task<TaskAgent> SetAgentProvisioningStateAsync(
      int poolId,
      int agentId,
      string provisioningState);
  }
}
