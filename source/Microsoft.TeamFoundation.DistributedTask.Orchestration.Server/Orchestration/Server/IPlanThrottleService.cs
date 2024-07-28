// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.IPlanThrottleService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  [DefaultServiceImplementation(typeof (PlanThrottleService))]
  public interface IPlanThrottleService : IVssFrameworkService
  {
    int GetAvailableQueueSlots(IVssRequestContext requestContext);

    int GetMaxQueuedPlans(IVssRequestContext requestContext);

    IList<PlanConcurrency> GetPlanConcurrency(IVssRequestContext requestContext);

    PlanConcurrency GetPlanConcurrency(
      IVssRequestContext requestContext,
      string hubName,
      Guid scopeIdentifier,
      TaskOrchestrationOwner definitionReference);

    IList<TaskOrchestrationPlan> GetThrottledPlans(
      IVssRequestContext requestContext,
      int maxPlans,
      string hubName = null);

    IList<PlanConcurrency> SetPlanConcurrency(
      IVssRequestContext requestContext,
      string hubName,
      Guid scopeIdentifier,
      IList<int> definitionIds,
      int? concurrency,
      string incidentId = null);

    bool ShouldThrottleNewPlans(
      IVssRequestContext requestContext,
      TaskHub hub,
      Guid scopeIdentifier,
      TaskOrchestrationOwner definitionReference);

    Task StartThrottledPlan(
      IVssRequestContext requestContext,
      TaskOrchestrationPlan plan,
      TaskHub hub);
  }
}
