// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Hubs.PoolHubDispatcher
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
  internal sealed class PoolHubDispatcher : IPoolHubDispatcher, IVssFrameworkService
  {
    private IHubContext m_hubContext;

    public async Task Subscribe(IVssRequestContext requestContext, int poolId, string connectionId)
    {
      string groupName = PoolHubDispatcher.GetGroupName((await requestContext.GetService<IDistributedTaskResourceService>().GetAgentPoolAsync(requestContext, poolId) ?? throw new TaskAgentPoolNotFoundException(TaskResources.AgentPoolNotFound((object) poolId))).Scope, poolId);
      await this.m_hubContext.Groups.AddTrackedConnection(requestContext, "PoolHub", groupName, connectionId).ConfigureAwait(false);
    }

    public async Task Unsubscribe(
      IVssRequestContext requestContext,
      int poolId,
      string connectionId)
    {
      TaskAgentPool agentPoolAsync = await requestContext.GetService<IDistributedTaskResourceService>().GetAgentPoolAsync(requestContext, poolId);
      if (agentPoolAsync == null)
        return;
      string groupName = PoolHubDispatcher.GetGroupName(agentPoolAsync.Scope, poolId);
      await this.m_hubContext.Groups.RemoveTrackedConnection(requestContext, "PoolHub", groupName, connectionId).ConfigureAwait(false);
    }

    internal Task Disconnect(IVssRequestContext requestContext, string connectionId) => this.m_hubContext.RemoveTrackedConnection(requestContext, "PoolHub", connectionId);

    public void NotifyAgentAdded(IVssRequestContext requestContext, int poolId, int agentId)
    {
      string groupName = PoolHubDispatcher.GetGroupName(requestContext.ServiceHost.InstanceId, poolId);
      // ISSUE: reference to a compiler-generated field
      if (PoolHubDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PoolHubDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, IVssRequestContext, int, int>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "notifyAgentAdded", (IEnumerable<Type>) null, typeof (PoolHubDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      PoolHubDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__0.Target((CallSite) PoolHubDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__0, this.m_hubContext.Clients.TrackedGroup<object>(requestContext, "PoolHub", groupName), requestContext, poolId, agentId);
    }

    public void NotifyAgentConnected(IVssRequestContext requestContext, int poolId, int agentId)
    {
      string groupName = PoolHubDispatcher.GetGroupName(requestContext.ServiceHost.InstanceId, poolId);
      // ISSUE: reference to a compiler-generated field
      if (PoolHubDispatcher.\u003C\u003Eo__4.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PoolHubDispatcher.\u003C\u003Eo__4.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, IVssRequestContext, int, int>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "notifyAgentConnected", (IEnumerable<Type>) null, typeof (PoolHubDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      PoolHubDispatcher.\u003C\u003Eo__4.\u003C\u003Ep__0.Target((CallSite) PoolHubDispatcher.\u003C\u003Eo__4.\u003C\u003Ep__0, this.m_hubContext.Clients.TrackedGroup<object>(requestContext, "PoolHub", groupName), requestContext, poolId, agentId);
    }

    public void NotifyAgentDeleted(IVssRequestContext requestContext, int poolId, int agentId)
    {
      string groupName = PoolHubDispatcher.GetGroupName(requestContext.ServiceHost.InstanceId, poolId);
      // ISSUE: reference to a compiler-generated field
      if (PoolHubDispatcher.\u003C\u003Eo__5.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PoolHubDispatcher.\u003C\u003Eo__5.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, IVssRequestContext, int, int>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "notifyAgentDeleted", (IEnumerable<Type>) null, typeof (PoolHubDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      PoolHubDispatcher.\u003C\u003Eo__5.\u003C\u003Ep__0.Target((CallSite) PoolHubDispatcher.\u003C\u003Eo__5.\u003C\u003Ep__0, this.m_hubContext.Clients.TrackedGroup<object>(requestContext, "PoolHub", groupName), requestContext, poolId, agentId);
    }

    public void NotifyAgentDisconnected(IVssRequestContext requestContext, int poolId, int agentId)
    {
      string groupName = PoolHubDispatcher.GetGroupName(requestContext.ServiceHost.InstanceId, poolId);
      // ISSUE: reference to a compiler-generated field
      if (PoolHubDispatcher.\u003C\u003Eo__6.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PoolHubDispatcher.\u003C\u003Eo__6.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, IVssRequestContext, int, int>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "notifyAgentDisconnected", (IEnumerable<Type>) null, typeof (PoolHubDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      PoolHubDispatcher.\u003C\u003Eo__6.\u003C\u003Ep__0.Target((CallSite) PoolHubDispatcher.\u003C\u003Eo__6.\u003C\u003Ep__0, this.m_hubContext.Clients.TrackedGroup<object>(requestContext, "PoolHub", groupName), requestContext, poolId, agentId);
    }

    public void NotifyAgentUpdated(IVssRequestContext requestContext, int poolId, int agentId)
    {
      string groupName = PoolHubDispatcher.GetGroupName(requestContext.ServiceHost.InstanceId, poolId);
      // ISSUE: reference to a compiler-generated field
      if (PoolHubDispatcher.\u003C\u003Eo__7.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PoolHubDispatcher.\u003C\u003Eo__7.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, IVssRequestContext, int, int>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "notifyAgentUpdated", (IEnumerable<Type>) null, typeof (PoolHubDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      PoolHubDispatcher.\u003C\u003Eo__7.\u003C\u003Ep__0.Target((CallSite) PoolHubDispatcher.\u003C\u003Eo__7.\u003C\u003Ep__0, this.m_hubContext.Clients.TrackedGroup<object>(requestContext, "PoolHub", groupName), requestContext, poolId, agentId);
    }

    public void NotifyAgentRequestQueued(
      IVssRequestContext requestContext,
      int poolId,
      long requestId)
    {
      string groupName = PoolHubDispatcher.GetGroupName(requestContext.ServiceHost.InstanceId, poolId);
      // ISSUE: reference to a compiler-generated field
      if (PoolHubDispatcher.\u003C\u003Eo__8.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PoolHubDispatcher.\u003C\u003Eo__8.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, IVssRequestContext, int, long>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "notifyAgentRequestQueued", (IEnumerable<Type>) null, typeof (PoolHubDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      PoolHubDispatcher.\u003C\u003Eo__8.\u003C\u003Ep__0.Target((CallSite) PoolHubDispatcher.\u003C\u003Eo__8.\u003C\u003Ep__0, this.m_hubContext.Clients.TrackedGroup<object>(requestContext, "PoolHub", groupName), requestContext, poolId, requestId);
    }

    public void NotifyAgentRequestAssigned(
      IVssRequestContext requestContext,
      int poolId,
      long requestId)
    {
      string groupName = PoolHubDispatcher.GetGroupName(requestContext.ServiceHost.InstanceId, poolId);
      // ISSUE: reference to a compiler-generated field
      if (PoolHubDispatcher.\u003C\u003Eo__9.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PoolHubDispatcher.\u003C\u003Eo__9.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, IVssRequestContext, int, long>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "notifyAgentRequestAssigned", (IEnumerable<Type>) null, typeof (PoolHubDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      PoolHubDispatcher.\u003C\u003Eo__9.\u003C\u003Ep__0.Target((CallSite) PoolHubDispatcher.\u003C\u003Eo__9.\u003C\u003Ep__0, this.m_hubContext.Clients.TrackedGroup<object>(requestContext, "PoolHub", groupName), requestContext, poolId, requestId);
    }

    public void NotifyAgentRequestStarted(
      IVssRequestContext requestContext,
      int poolId,
      long requestId)
    {
      string groupName = PoolHubDispatcher.GetGroupName(requestContext.ServiceHost.InstanceId, poolId);
      // ISSUE: reference to a compiler-generated field
      if (PoolHubDispatcher.\u003C\u003Eo__10.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PoolHubDispatcher.\u003C\u003Eo__10.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, IVssRequestContext, int, long>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "notifyAgentRequestStarted", (IEnumerable<Type>) null, typeof (PoolHubDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      PoolHubDispatcher.\u003C\u003Eo__10.\u003C\u003Ep__0.Target((CallSite) PoolHubDispatcher.\u003C\u003Eo__10.\u003C\u003Ep__0, this.m_hubContext.Clients.TrackedGroup<object>(requestContext, "PoolHub", groupName), requestContext, poolId, requestId);
    }

    public void NotifyAgentRequestCompleted(
      IVssRequestContext requestContext,
      int poolId,
      long requestId)
    {
      string groupName = PoolHubDispatcher.GetGroupName(requestContext.ServiceHost.InstanceId, poolId);
      // ISSUE: reference to a compiler-generated field
      if (PoolHubDispatcher.\u003C\u003Eo__11.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PoolHubDispatcher.\u003C\u003Eo__11.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, IVssRequestContext, int, long>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "notifyAgentRequestCompleted", (IEnumerable<Type>) null, typeof (PoolHubDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      PoolHubDispatcher.\u003C\u003Eo__11.\u003C\u003Ep__0.Target((CallSite) PoolHubDispatcher.\u003C\u003Eo__11.\u003C\u003Ep__0, this.m_hubContext.Clients.TrackedGroup<object>(requestContext, "PoolHub", groupName), requestContext, poolId, requestId);
    }

    private static string GetGroupName(Guid pipelinesScope) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1:N}", (object) "PoolHub", (object) pipelinesScope);

    private static string GetGroupName(Guid poolScope, int poolId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1:N}_{2}", (object) "PoolHub", (object) poolScope, (object) poolId);

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext) => this.m_hubContext = GlobalHost.ConnectionManager.GetHubContext<PoolHub>();
  }
}
