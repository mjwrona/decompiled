// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskOrchestrationPlanExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal static class TaskOrchestrationPlanExtensions
  {
    public static TaskOrchestrationPlanReference AsReference(this TaskOrchestrationPlan plan) => new TaskOrchestrationPlanReference()
    {
      ArtifactUri = plan.ArtifactUri,
      PlanType = plan.PlanType,
      ProcessType = plan.ProcessType,
      Version = plan.Version,
      PlanId = plan.PlanId,
      ScopeIdentifier = plan.ScopeIdentifier,
      ContainerId = plan.ContainerId,
      Definition = plan.Definition?.Clone(),
      Owner = plan.Owner?.Clone(),
      PlanGroup = plan.PlanGroup
    };

    public static T GetEnvironment<T>(this TaskOrchestrationPlan plan) where T : class, IOrchestrationEnvironment => plan.ProcessEnvironment as T;

    public static T GetProcess<T>(this TaskOrchestrationPlan plan) where T : class, IOrchestrationProcess => plan.Process as T;

    public static TaskOrchestrationQueuedPlan AsQueuedPlan(this TaskOrchestrationPlan plan)
    {
      TaskOrchestrationQueuedPlan orchestrationQueuedPlan = new TaskOrchestrationQueuedPlan();
      orchestrationQueuedPlan.PlanGroup = plan.PlanGroup;
      orchestrationQueuedPlan.Definition = plan.Definition;
      orchestrationQueuedPlan.Owner = plan.Owner;
      orchestrationQueuedPlan.PlanId = plan.PlanId;
      orchestrationQueuedPlan.ScopeIdentifier = plan.ScopeIdentifier;
      orchestrationQueuedPlan.QueuePosition = 0;
      orchestrationQueuedPlan.PoolId = 0;
      DateTime? startTime = plan.StartTime;
      orchestrationQueuedPlan.QueueTime = startTime ?? DateTime.UtcNow;
      startTime = plan.StartTime;
      orchestrationQueuedPlan.AssignTime = new DateTime?(startTime ?? DateTime.UtcNow);
      return orchestrationQueuedPlan;
    }
  }
}
