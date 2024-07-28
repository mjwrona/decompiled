// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Hub.ReleaseHub
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Analytics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Hub
{
  public class ReleaseHub : VssHub<IReleaseHubClient>
  {
    public void WatchReleaseDefinitionsReleases(Guid projectId, IList<int> definitionIds)
    {
      if (definitionIds == null)
        return;
      foreach (int definitionId in (IEnumerable<int>) definitionIds)
        this.WatchReleaseDefinitionReleases(projectId, definitionId);
    }

    public Task WatchReleaseDefinitionReleases(Guid projectId, int definitionId)
    {
      CustomerIntelligenceHelper.PublishDefinitionReleasesWatched(this.VssRequestContext, definitionId);
      return this.VssRequestContext.GetService<IReleaseEventDispatcherService>().WatchReleaseDefinitionReleases(this.VssRequestContext, projectId, definitionId, this.Context.ConnectionId);
    }

    public void StopWatchingReleaseDefinitionsReleases(Guid projectId, IList<int> definitionIds)
    {
      if (definitionIds == null)
        return;
      foreach (int definitionId in (IEnumerable<int>) definitionIds)
        this.StopWatchingReleaseDefinitionReleases(projectId, definitionId);
    }

    public Task StopWatchingReleaseDefinitionReleases(Guid projectId, int definitionId) => this.VssRequestContext.GetService<IReleaseEventDispatcherService>().StopWatchingReleaseDefinitionReleases(this.VssRequestContext, projectId, definitionId, this.Context.ConnectionId);

    public void WatchRelease(Guid projectId, int releaseId)
    {
      using (ReleaseManagementTimer.Create(this.VssRequestContext, "signalR", "ReleaseHub.WatchRelease", 1999998))
      {
        ReleaseHub.PublishCustomerIntelligence(this.VssRequestContext, releaseId);
        this.VssRequestContext.GetService<IReleaseEventDispatcherService>().WatchRelease(this.VssRequestContext, projectId, releaseId, this.Context.ConnectionId);
      }
    }

    public Task StopWatchingRelease(Guid projectId, int releaseId) => this.VssRequestContext.GetService<IReleaseEventDispatcherService>().StopWatchingRelease(this.VssRequestContext, projectId, releaseId, this.Context.ConnectionId);

    public void WatchReleaseJobLogs(Guid projectId, int releaseId, Guid jobId)
    {
      using (ReleaseManagementTimer.Create(this.VssRequestContext, "signalR", "ReleaseHub.WatchReleaseJobLogs", 1999998))
        this.VssRequestContext.GetService<IReleaseEventDispatcherService>().WatchReleaseJobLogs(this.VssRequestContext, projectId, releaseId, jobId, this.Context.ConnectionId);
    }

    public override async Task OnDisconnected(bool stopCalled)
    {
      ReleaseHub releaseHub = this;
      // ISSUE: reference to a compiler-generated method
      await releaseHub.\u003C\u003En__0(stopCalled);
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      await __nonvirtual (releaseHub.VssRequestContext).GetService<IReleaseEventDispatcherService>().Disconnect(__nonvirtual (releaseHub.VssRequestContext), __nonvirtual (releaseHub.Context).ConnectionId, !stopCalled);
    }

    public void WatchCollection() => this.VssRequestContext.GetService<IReleaseEventDispatcherService>().WatchCollection(this.VssRequestContext, this.Context.ConnectionId);

    private static void PublishCustomerIntelligence(
      IVssRequestContext requestContext,
      int releaseId)
    {
      CustomerIntelligenceHelper.PublishReleaseWatched(requestContext, releaseId);
    }
  }
}
