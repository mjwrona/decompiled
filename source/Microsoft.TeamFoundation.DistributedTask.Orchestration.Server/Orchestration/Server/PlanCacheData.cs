// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.PlanCacheData
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal class PlanCacheData
  {
    private readonly VssMemoryCacheList<Guid, TaskOrchestrationPlanReference> m_planCache;

    public PlanCacheData(TaskOrchestrationPlanCache cache, int size = 64) => this.m_planCache = new VssMemoryCacheList<Guid, TaskOrchestrationPlanReference>((IVssCachePerformanceProvider) cache, size);

    public VssMemoryCacheList<Guid, TaskOrchestrationPlanReference> Plans => this.m_planCache;

    public void AddPlan(IVssRequestContext requestContext, TaskOrchestrationPlanReference plan) => this.m_planCache.Add(plan.PlanId, plan, false);

    public PlanCacheData RemovePlan(IVssRequestContext requestContext, Guid planId)
    {
      this.m_planCache.Remove(planId);
      return this;
    }

    public bool TryGetPlan(
      IVssRequestContext requestContext,
      Guid planId,
      out TaskOrchestrationPlanReference cacheEntry)
    {
      return this.m_planCache.TryGetValue(planId, out cacheEntry);
    }
  }
}
