// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Hubs.AgentHubDispatcher
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.AspNet.SignalR;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.SignalR;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Hubs
{
  internal sealed class AgentHubDispatcher : IAgentHubDispatcher, IVssFrameworkService
  {
    private IHubContext m_hubContext;

    public async Task Subscribe(
      IVssRequestContext requestContext,
      int poolId,
      int agentId,
      string connectionId)
    {
      IDistributedTaskResourceService resourceService = requestContext.GetService<IDistributedTaskResourceService>();
      TaskAgentPool pool = await resourceService.GetAgentPoolAsync(requestContext, poolId);
      if (pool == null)
        throw new TaskAgentPoolNotFoundException(TaskResources.AgentPoolNotFound((object) poolId));
      if (await resourceService.GetAgentAsync(requestContext, poolId, agentId) == null)
        throw new TaskAgentNotFoundException(TaskResources.AgentNotFound((object) poolId, (object) agentId));
      string groupName = AgentHubDispatcher.GetGroupName(pool.Scope, poolId, agentId);
      await this.m_hubContext.Groups.AddTrackedConnection(requestContext, "AgentHub", groupName, connectionId).ConfigureAwait(false);
      resourceService = (IDistributedTaskResourceService) null;
      pool = (TaskAgentPool) null;
    }

    public async Task Unsubscribe(
      IVssRequestContext requestContext,
      int poolId,
      int agentId,
      string connectionId)
    {
      IDistributedTaskResourceService resourceService = requestContext.GetService<IDistributedTaskResourceService>();
      TaskAgentPool pool = await resourceService.GetAgentPoolAsync(requestContext, poolId);
      if (pool == null)
      {
        resourceService = (IDistributedTaskResourceService) null;
        pool = (TaskAgentPool) null;
      }
      else if (await resourceService.GetAgentAsync(requestContext, poolId, agentId) == null)
      {
        resourceService = (IDistributedTaskResourceService) null;
        pool = (TaskAgentPool) null;
      }
      else
      {
        string groupName = AgentHubDispatcher.GetGroupName(pool.Scope, poolId, agentId);
        await this.m_hubContext.Groups.RemoveTrackedConnection(requestContext, "AgentHub", groupName, connectionId).ConfigureAwait(false);
        resourceService = (IDistributedTaskResourceService) null;
        pool = (TaskAgentPool) null;
      }
    }

    internal Task Disconnect(IVssRequestContext requestContext, string connectionId) => this.m_hubContext.RemoveTrackedConnection(requestContext, "AgentHub", connectionId);

    public void NotifyAgentRequestUpdated(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request)
    {
      if (request.ReservedAgent == null)
        return;
      string groupName = AgentHubDispatcher.GetGroupName(requestContext.ServiceHost.InstanceId, poolId, request.ReservedAgent.Id);
      // ISSUE: reference to a compiler-generated field
      if (AgentHubDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        AgentHubDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, IVssRequestContext, int, int, long>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "notifyAgentRequestUpdated", (IEnumerable<Type>) null, typeof (AgentHubDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[5]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      AgentHubDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__0.Target((CallSite) AgentHubDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__0, this.m_hubContext.Clients.TrackedGroup<object>(requestContext, "AgentHub", groupName), requestContext, request.PoolId, request.ReservedAgent.Id, request.RequestId);
    }

    private static string GetGroupName(Guid poolScope, int poolId, int agentId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1:N}_{2}_{3}", (object) "AgentHub", (object) poolScope, (object) poolId, (object) agentId);

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext) => this.m_hubContext = GlobalHost.ConnectionManager.GetHubContext<AgentHub>();
  }
}
