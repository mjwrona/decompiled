// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskOrchestrationPlanCache
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal sealed class TaskOrchestrationPlanCache : VssVersionedCacheService<PlanCacheData>
  {
    private const string c_layer = "PlanCache";
    private const string c_planCacheSize = "/Service/DistributedTask/Settings/PlanCacheSize";
    private const int c_defaultCacheSize = 64;

    public TaskOrchestrationPlanReference GetPlan(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Func<IVssRequestContext, Guid, Guid, TaskOrchestrationPlanReference> missDelegate)
    {
      TaskOrchestrationPlanReference cacheEntry = (TaskOrchestrationPlanReference) null;
      if (!this.TryRead(requestContext, (Func<PlanCacheData, bool>) (cache => cache.TryGetPlan(requestContext, planId, out cacheEntry))))
        cacheEntry = this.Synchronize<TaskOrchestrationPlanReference>(requestContext, (Func<TaskOrchestrationPlanReference>) (() => missDelegate(requestContext, scopeIdentifier, planId)), (Action<PlanCacheData, TaskOrchestrationPlanReference>) ((cache, delegateResult) =>
        {
          if (delegateResult == null)
            return;
          cache.AddPlan(requestContext, delegateResult);
        }));
      return cacheEntry;
    }

    public void RemovePlan(IVssRequestContext requestContext, Guid planId) => this.Invalidate<PlanCacheData>(requestContext, (Func<PlanCacheData, PlanCacheData>) (cache => cache.RemovePlan(requestContext, planId)));

    public void SetPlan(IVssRequestContext requestContext, TaskOrchestrationPlanReference plan) => this.Synchronize<TaskOrchestrationPlanReference>(requestContext, (Func<TaskOrchestrationPlanReference>) (() => plan), (Action<PlanCacheData, TaskOrchestrationPlanReference>) ((cache, delegateResult) => cache.AddPlan(requestContext, plan)));

    protected override PlanCacheData InitializeCache(IVssRequestContext requestContext) => new PlanCacheData(this, requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/DistributedTask/Settings/PlanCacheSize", 64));

    protected override void ServiceStart(IVssRequestContext systemRequestContext) => base.ServiceStart(systemRequestContext);

    protected override void ServiceEnd(IVssRequestContext systemRequestContext) => base.ServiceEnd(systemRequestContext);
  }
}
