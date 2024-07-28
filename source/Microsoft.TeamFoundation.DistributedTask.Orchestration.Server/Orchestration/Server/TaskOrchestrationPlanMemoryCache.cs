// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskOrchestrationPlanMemoryCache
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal sealed class TaskOrchestrationPlanMemoryCache : 
    VssMemoryCacheService<Guid, TaskOrchestrationPlanReference>
  {
    private static readonly MemoryCacheConfiguration<Guid, TaskOrchestrationPlanReference> s_configuration = new MemoryCacheConfiguration<Guid, TaskOrchestrationPlanReference>().WithMaxElements(64).WithCleanupInterval(TimeSpan.FromMinutes(5.0)).WithInactivityInterval(TimeSpan.FromMinutes(10.0));

    public TaskOrchestrationPlanMemoryCache()
      : base((IEqualityComparer<Guid>) EqualityComparer<Guid>.Default, TaskOrchestrationPlanMemoryCache.s_configuration)
    {
    }

    public TaskOrchestrationPlanReference GetPlan(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Func<IVssRequestContext, Guid, Guid, TaskOrchestrationPlanReference> missDelegate)
    {
      TaskOrchestrationPlanReference plan;
      if (!this.TryGetValue(requestContext, planId, out plan))
      {
        plan = missDelegate(requestContext, scopeIdentifier, planId);
        if (plan != null)
          this.Set(requestContext, planId, plan);
      }
      return plan;
    }
  }
}
