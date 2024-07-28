// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.LifeCycleManager
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public sealed class LifeCycleManager : ILifeCycleManager
  {
    private readonly IVssRequestContext requestContext;

    public LifeCycleManager(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public async Task CancelLifeCycleHookAsync(
      string lifeCycleHookOrchestrationId,
      CanceledEvent cancelEvent)
    {
      await EnvironmentTaskHubHelper.PublishOrchestrationEvent(this.requestContext, EnvironmentConstants.DeploymentPhaseHubName, lifeCycleHookOrchestrationId, "LifeCycleHookCanceled", (object) cancelEvent);
    }

    public async Task CancelLifeCycleAsync(
      string lifeCycleOrchestrationId,
      CanceledEvent cancelEvent)
    {
      await EnvironmentTaskHubHelper.PublishOrchestrationEvent(this.requestContext, EnvironmentConstants.DeploymentPhaseHubName, lifeCycleOrchestrationId, "LifeCycleCanceled", (object) cancelEvent);
    }
  }
}
