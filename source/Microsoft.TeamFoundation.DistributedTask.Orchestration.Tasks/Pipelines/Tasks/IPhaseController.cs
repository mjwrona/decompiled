// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.IPhaseController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  public interface IPhaseController
  {
    Task Cancel(Guid planId, IList<string> instanceIds, string reason);

    Task<PhaseExecutionState> Expand(
      Guid scopeId,
      Guid planId,
      string stageName,
      string phaseName,
      IDictionary<string, PhaseExecutionState> dependencies,
      int stageAttempt,
      int phaseAttempt,
      IList<string> configurations);

    Task<PhaseExecutionState> ExpandTemplate(
      Guid scopeId,
      Guid planId,
      string phaseName,
      IList<PhaseExecutionState> dependencies);

    Task UpdateTimelineRecordPoolData(
      Guid planId,
      Guid jobId,
      int poolId,
      JObject agentSpecification);

    Task JobAssigned(string instanceId, JobAssignedEventData eventData);

    Task JobStarted(
      Guid scopeId,
      Guid planId,
      JobParameters jobParameters,
      JobStartedEventData eventData);

    Task CreateJobAttempt(
      Guid scopeIdentifier,
      Guid planId,
      string stageName,
      string phaseName,
      string jobName,
      int attempt,
      int previousAttempt);
  }
}
