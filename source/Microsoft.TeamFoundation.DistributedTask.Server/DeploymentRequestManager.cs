// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DeploymentRequestManager
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public sealed class DeploymentRequestManager : IDeploymentRequestManager
  {
    private readonly IVssRequestContext requestContext;

    public DeploymentRequestManager(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Task DeploymentRequestStarted(int environmentId, long requestId, DateTime startTime)
    {
      this.requestContext.GetService<IEnvironmentDeploymentExecutionHistoryService>().UpdateEnvironmentDeploymentRequest(this.requestContext, environmentId, requestId, new DateTime?(startTime), new DateTime?(), new TaskResult?());
      return Task.CompletedTask;
    }

    public Task DeploymentRequestCompleted(
      int environmentId,
      long requestId,
      DateTime finishTime,
      TaskResult result)
    {
      this.requestContext.GetService<IEnvironmentDeploymentExecutionHistoryService>().UpdateEnvironmentDeploymentRequest(this.requestContext, environmentId, requestId, new DateTime?(), new DateTime?(finishTime), new TaskResult?(result));
      return Task.CompletedTask;
    }

    public Task EnvironmentResourceDeploymentRequestQueued(
      int environmentId,
      long requestId,
      int resourceId,
      DateTime startTime)
    {
      IEnvironmentDeploymentExecutionHistoryService service = this.requestContext.GetService<IEnvironmentDeploymentExecutionHistoryService>();
      EnvironmentResourceDeploymentExecutionRecord deploymentExecutionRecord = new EnvironmentResourceDeploymentExecutionRecord()
      {
        EnvironmentId = environmentId,
        RequestId = requestId,
        ResourceId = resourceId,
        StartTime = startTime
      };
      IVssRequestContext requestContext = this.requestContext;
      EnvironmentResourceDeploymentExecutionRecord request = deploymentExecutionRecord;
      service.QueueEnvironmentResourceDeploymentRequest(requestContext, request);
      return Task.CompletedTask;
    }

    public Task EnvironmentResourceDeploymentRequestCompleted(
      int environmentId,
      long requestId,
      int resourceId,
      DateTime finishTime,
      TaskResult result)
    {
      this.requestContext.GetService<IEnvironmentDeploymentExecutionHistoryService>().UpdateEnvironmentResourceDeploymentRequest(this.requestContext, environmentId, requestId, resourceId, new DateTime?(finishTime), new TaskResult?(result));
      return Task.CompletedTask;
    }
  }
}
