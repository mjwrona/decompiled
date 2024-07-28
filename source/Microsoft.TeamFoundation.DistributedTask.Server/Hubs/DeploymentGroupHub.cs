// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Hubs.DeploymentGroupHub
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.SignalR.Hubs;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Hubs
{
  public sealed class DeploymentGroupHub : VssHub
  {
    public Task Subscribe(Guid projectId, int deploymentGroupId) => this.VssRequestContext.GetService<IDeploymentGroupHubDispatcher>().Subscribe(this.VssRequestContext, projectId, deploymentGroupId, this.Context.ConnectionId);

    public Task Unsubscribe(Guid projectId, int deploymentGroupId) => this.VssRequestContext.GetService<IDeploymentGroupHubDispatcher>().Unsubscribe(this.VssRequestContext, projectId, deploymentGroupId, this.Context.ConnectionId);

    public override async Task OnDisconnected(bool stopCalled)
    {
      DeploymentGroupHub deploymentGroupHub = this;
      // ISSUE: reference to a compiler-generated method
      await deploymentGroupHub.\u003C\u003En__0(stopCalled);
      await deploymentGroupHub.VssRequestContext.GetService<IDeploymentGroupHubDispatcher>().Disconnect(deploymentGroupHub.VssRequestContext, deploymentGroupHub.Context.ConnectionId);
    }
  }
}
