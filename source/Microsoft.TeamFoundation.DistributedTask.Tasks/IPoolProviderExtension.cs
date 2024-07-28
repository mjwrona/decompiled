// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Tasks.IPoolProviderExtension
// Assembly: Microsoft.TeamFoundation.DistributedTask.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 691D8169-F87B-47FC-8906-5680483E9D38
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Tasks
{
  public interface IPoolProviderExtension
  {
    Task<AgentProvisionResponse> ProvisionAgentAsync(
      int agentCloudId,
      TaskAgentCloudRequest cloudRequest,
      TaskAgentPoolReference pool,
      TaskAgentReference agent);

    Task<PoolProviderResponse> DeprovisionAgentAsync(
      int agentCloudId,
      Guid requestId,
      JObject agentData,
      string poolName,
      string orchestrationId);
  }
}
