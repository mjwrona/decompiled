// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.PlanLogTableMemoryCache
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
  internal sealed class PlanLogTableMemoryCache : VssMemoryCacheService<Guid, PlanLogTable>
  {
    private static readonly MemoryCacheConfiguration<Guid, PlanLogTable> s_configuration = new MemoryCacheConfiguration<Guid, PlanLogTable>().WithInactivityInterval(TimeSpan.FromMinutes(30.0)).WithMaxElements(10000).WithCleanupInterval(new TimeSpan(0, 5, 0));

    public PlanLogTableMemoryCache()
      : base((IEqualityComparer<Guid>) EqualityComparer<Guid>.Default, PlanLogTableMemoryCache.s_configuration)
    {
    }

    public async Task<PlanLogTable> GetLogTableAsync(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      Guid planId,
      Func<IVssRequestContext, Guid, Guid, Task<PlanLogTable>> missDelegate)
    {
      PlanLogTableMemoryCache tableMemoryCache = this;
      PlanLogTable logTableAsync;
      if (!tableMemoryCache.TryGetValue(requestContext, planId, out logTableAsync))
      {
        logTableAsync = await missDelegate(requestContext, scopeIdentifier, planId);
        tableMemoryCache.Set(requestContext, planId, logTableAsync);
      }
      return logTableAsync;
    }

    public void RemoveLogTable(IVssRequestContext requestContext, Guid planId) => this.Remove(requestContext, planId);
  }
}
