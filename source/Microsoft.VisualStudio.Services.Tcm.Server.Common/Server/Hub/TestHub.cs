// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Hub.TestHub
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.SignalR.Hubs;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server.Hub
{
  [CLSCompliant(false)]
  public class TestHub : VssHub
  {
    public override async Task OnConnected() => await base.OnConnected();

    public override async Task OnDisconnected(bool stopCalled)
    {
      TestHub testHub = this;
      // ISSUE: reference to a compiler-generated method
      await testHub.\u003C\u003En__1(stopCalled);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      await __nonvirtual (testHub.VssRequestContext).GetService<ITestHubDispatcher>().OnDisconnected(__nonvirtual (testHub.VssRequestContext), __nonvirtual (testHub.Context)?.ConnectionId, stopCalled);
    }

    public void UnWatchBuild(int buildId) => this.VssRequestContext.GetService<IBuildTestHubDispatcher>().UnWatch(this.VssRequestContext, buildId, this.Context?.ConnectionId);

    public void UnWatchRelease(int releaseId, int environmentId) => this.VssRequestContext.GetService<IReleaseTestHubDispatcher>().UnWatch(this.VssRequestContext, releaseId, environmentId, this.Context?.ConnectionId);

    public void WatchBuild(int buildId) => this.VssRequestContext.GetService<IBuildTestHubDispatcher>().Watch(this.VssRequestContext, buildId, this.Context?.ConnectionId);

    public void WatchRelease(int releaseId, int environmentId) => this.VssRequestContext.GetService<IReleaseTestHubDispatcher>().Watch(this.VssRequestContext, releaseId, environmentId, this.Context?.ConnectionId);
  }
}
