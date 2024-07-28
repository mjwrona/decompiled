// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Hubs.PoolHub
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.SignalR.Hubs;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Hubs
{
  public sealed class PoolHub : VssHub
  {
    public Task Subscribe(int poolId)
    {
      IVssRequestContext poolRequestContext = this.VssRequestContext.ToPoolRequestContext();
      return poolRequestContext.GetService<PoolHubDispatcher>().Subscribe(poolRequestContext, poolId, this.Context.ConnectionId);
    }

    public Task Unsubscribe(int poolId)
    {
      IVssRequestContext poolRequestContext = this.VssRequestContext.ToPoolRequestContext();
      return poolRequestContext.GetService<PoolHubDispatcher>().Unsubscribe(poolRequestContext, poolId, this.Context.ConnectionId);
    }

    public override async Task OnDisconnected(bool stopCalled)
    {
      PoolHub poolHub = this;
      // ISSUE: reference to a compiler-generated method
      await poolHub.\u003C\u003En__0(stopCalled);
      IVssRequestContext poolRequestContext = poolHub.VssRequestContext.ToPoolRequestContext();
      await poolRequestContext.GetService<PoolHubDispatcher>().Disconnect(poolRequestContext, poolHub.Context.ConnectionId);
    }
  }
}
