// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.IAgentPoolExtension2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  public interface IAgentPoolExtension2
  {
    Task<AbandonJobResult> AbandonJob(
      int poolId,
      long requestId,
      DateTime expirationTime,
      Guid hostId,
      Guid planId);

    Task<TaskAgentJobRequest> QueueJob(
      int poolId,
      IList<Demand> demands,
      Guid hostId,
      Guid planId,
      Guid jobId,
      string orchestrationId);

    Task CancelJob(
      int poolId,
      long requestId,
      Guid hostId,
      Guid planId,
      Guid jobId,
      TimeSpan timeout);

    Task StartJob(
      int poolId,
      long requestId,
      Guid hostId,
      Guid planId,
      Guid jobId,
      Microsoft.TeamFoundation.DistributedTask.WebApi.Legacy.JobEnvironment environment);

    Task FinishJob(int poolId, long requestId, Guid hostId, Guid planId, Guid jobId);
  }
}
