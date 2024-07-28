// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DefaultJobEventSender
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class DefaultJobEventSender : IJobEventSender
  {
    private const string c_layer = "DefaultJobEventSender";

    public async Task RaiseEventAsync<T>(
      IVssRequestContext requestContext,
      Guid serviceOwner,
      Guid hostId,
      Guid scopeId,
      string planType,
      Guid planId,
      T eventData)
      where T : JobEvent
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (DefaultJobEventSender), nameof (RaiseEventAsync));
      try
      {
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment && requestContext.RootContext.ServiceHost.InstanceId != hostId && requestContext.RootContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
          requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>().AddTask(requestContext, (TeamFoundationTaskCallback) ((systemContext, args) => this.RaiseEventInternal<T>(systemContext, serviceOwner, hostId, scopeId, planType, planId, eventData)));
        else
          await this.RaiseEventInternalAsync<T>(requestContext.Elevate(), serviceOwner, hostId, scopeId, planType, planId, eventData);
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    private void RaiseEventInternal<T>(
      IVssRequestContext requestContext,
      Guid serviceOwner,
      Guid hostId,
      Guid scopeId,
      string planType,
      Guid planId,
      T eventData)
      where T : JobEvent
    {
      try
      {
        if (DefaultJobEventSender.IsLocal(requestContext, serviceOwner))
        {
          IVssRequestContext vssRequestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
          using (IVssRequestContext vssRequestContext2 = vssRequestContext1.GetService<ITeamFoundationHostManagementService>().BeginRequest(vssRequestContext1, hostId, RequestContextType.SystemContext))
            vssRequestContext2.GetService<DistributedTaskHubService>().GetTaskHub(vssRequestContext2, planType).RaiseJobEvent(vssRequestContext2, scopeId, planId, eventData.JobId, eventData.Name, (JobEvent) eventData);
        }
        else
          requestContext.GetClient<TaskHttpClient>(serviceOwner, hostId).RaisePlanEventAsync<T>(scopeId, planType, planId, eventData).SyncResult();
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10015000, "ResourceService", ex);
        throw;
      }
    }

    private async Task RaiseEventInternalAsync<T>(
      IVssRequestContext requestContext,
      Guid serviceOwner,
      Guid hostId,
      Guid scopeId,
      string planType,
      Guid planId,
      T eventData)
      where T : JobEvent
    {
      try
      {
        if (DefaultJobEventSender.IsLocal(requestContext, serviceOwner))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          using (IVssRequestContext targetContext = vssRequestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(vssRequestContext, hostId, RequestContextType.SystemContext))
            await targetContext.GetService<DistributedTaskHubService>().GetTaskHub(targetContext, planType).RaiseJobEventAsync(targetContext, scopeId, planId, ((T) eventData).JobId, ((T) eventData).Name, (JobEvent) eventData);
        }
        else
          await requestContext.GetClient<TaskHttpClient>(serviceOwner, hostId).RaisePlanEventAsync<T>(scopeId, planType, planId, eventData);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10015000, "ResourceService", ex);
        throw;
      }
    }

    private static bool IsLocal(IVssRequestContext requestContext, Guid serviceOwner) => requestContext.ExecutionEnvironment.IsOnPremisesDeployment || requestContext.ServiceInstanceType() == serviceOwner;
  }
}
