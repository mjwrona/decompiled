// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDetailHub
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.SignalR.Hubs;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildDetailHub : VssHub
  {
    internal ITeamFoundationBuildService2 BuildService => this.VssRequestContext.GetService<ITeamFoundationBuildService2>();

    public override async Task OnConnected() => await base.OnConnected();

    public override async Task OnDisconnected(bool stopCalled)
    {
      BuildDetailHub buildDetailHub = this;
      // ISSUE: reference to a compiler-generated method
      await buildDetailHub.\u003C\u003En__1(stopCalled);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      await __nonvirtual (buildDetailHub.VssRequestContext).GetService<BuildDispatcher>().Disconnect(__nonvirtual (buildDetailHub.VssRequestContext), __nonvirtual (buildDetailHub.Context).ConnectionId, stopCalled);
    }

    public override async Task OnReconnected()
    {
      BuildDetailHub buildDetailHub = this;
      // ISSUE: reference to a compiler-generated method
      await buildDetailHub.\u003C\u003En__2();
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("SignalRReconnected", 1.0);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      __nonvirtual (buildDetailHub.VssRequestContext).GetService<CustomerIntelligenceService>().Publish(__nonvirtual (buildDetailHub.VssRequestContext), "Build", "Build.SignalR", properties);
    }

    public Task WatchBuild(Guid projectId, int buildId) => this.VssRequestContext.GetService<BuildDispatcher>().WatchBuild(this.VssRequestContext, projectId, buildId, this.Context.ConnectionId);

    public Task StopWatchingBuild(Guid projectId, int buildId) => this.VssRequestContext.GetService<BuildDispatcher>().StopWatchingBuild(this.VssRequestContext, buildId, this.Context.ConnectionId);

    public Task WatchProject(Guid projectId) => this.VssRequestContext.GetService<BuildDispatcher>().WatchProject(this.VssRequestContext, projectId, this.Context.ConnectionId);

    public Task StopWatchingProject(Guid projectId) => this.VssRequestContext.GetService<BuildDispatcher>().StopWatchingProject(this.VssRequestContext, projectId, this.Context.ConnectionId);

    public Task SyncState(Guid projectId, int buildId) => this.VssRequestContext.GetService<BuildDispatcher>().SyncState(this.VssRequestContext, projectId, buildId, this.Context.ConnectionId);

    public Task WatchCollection() => this.VssRequestContext.GetService<BuildDispatcher>().WatchCollection(this.VssRequestContext, this.Context.ConnectionId);

    public Task StopWatchingCollection() => this.VssRequestContext.GetService<BuildDispatcher>().StopWatchingCollection(this.VssRequestContext, this.Context.ConnectionId);
  }
}
