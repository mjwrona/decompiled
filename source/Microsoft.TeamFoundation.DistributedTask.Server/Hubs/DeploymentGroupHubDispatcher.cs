// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Hubs.DeploymentGroupHubDispatcher
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
  internal sealed class DeploymentGroupHubDispatcher : 
    IDeploymentGroupHubDispatcher,
    IVssFrameworkService
  {
    private IHubContext m_hubContext;

    public Task Subscribe(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      string connectionId)
    {
      if (requestContext.GetService<IDeploymentGroupService>().GetDeploymentGroup(requestContext, projectId, deploymentGroupId, includeMachines: false) == null)
        throw new DeploymentGroupNotFoundException(TaskResources.DeploymentMachineGroupNotFound((object) deploymentGroupId));
      string groupName = DeploymentGroupHubDispatcher.GetGroupName(requestContext, projectId, deploymentGroupId);
      return this.m_hubContext.Groups.AddTrackedConnection(requestContext, "DeploymentGroupHub", groupName, connectionId);
    }

    public Task Unsubscribe(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      string connectionId)
    {
      if (requestContext.GetService<IDeploymentGroupService>().GetDeploymentGroup(requestContext, projectId, deploymentGroupId, includeMachines: false) == null)
        throw new DeploymentGroupNotFoundException(TaskResources.DeploymentMachineGroupNotFound((object) deploymentGroupId));
      string groupName = DeploymentGroupHubDispatcher.GetGroupName(requestContext, projectId, deploymentGroupId);
      return this.m_hubContext.Groups.RemoveTrackedConnection(requestContext, "DeploymentGroupHub", groupName, connectionId);
    }

    public Task Disconnect(IVssRequestContext requestContext, string connectionId) => this.m_hubContext.RemoveTrackedConnection(requestContext, "DeploymentGroupHub", connectionId);

    public void NotifyDeploymentMachineAdded(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      DeploymentMachine machine)
    {
      string groupName = DeploymentGroupHubDispatcher.GetGroupName(requestContext, projectId, deploymentGroupId);
      // ISSUE: reference to a compiler-generated field
      if (DeploymentGroupHubDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        DeploymentGroupHubDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, IVssRequestContext, int, DeploymentMachine>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "notifyDeploymentMachineAdded", (IEnumerable<Type>) null, typeof (DeploymentGroupHubDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      DeploymentGroupHubDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__0.Target((CallSite) DeploymentGroupHubDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__0, this.m_hubContext.Clients.TrackedGroup<object>(requestContext, "DeploymentGroupHub", groupName), requestContext, deploymentGroupId, machine);
    }

    public void NotifyDeploymentMachineConnected(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      int machineId)
    {
      string groupName = DeploymentGroupHubDispatcher.GetGroupName(requestContext, projectId, deploymentGroupId);
      // ISSUE: reference to a compiler-generated field
      if (DeploymentGroupHubDispatcher.\u003C\u003Eo__4.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        DeploymentGroupHubDispatcher.\u003C\u003Eo__4.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, IVssRequestContext, int, int>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "notifyDeploymentMachineConnected", (IEnumerable<Type>) null, typeof (DeploymentGroupHubDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      DeploymentGroupHubDispatcher.\u003C\u003Eo__4.\u003C\u003Ep__0.Target((CallSite) DeploymentGroupHubDispatcher.\u003C\u003Eo__4.\u003C\u003Ep__0, this.m_hubContext.Clients.TrackedGroup<object>(requestContext, "DeploymentGroupHub", groupName), requestContext, deploymentGroupId, machineId);
    }

    public void NotifyDeploymentMachineDeleted(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      int machineId)
    {
      string groupName = DeploymentGroupHubDispatcher.GetGroupName(requestContext, projectId, deploymentGroupId);
      // ISSUE: reference to a compiler-generated field
      if (DeploymentGroupHubDispatcher.\u003C\u003Eo__5.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        DeploymentGroupHubDispatcher.\u003C\u003Eo__5.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, IVssRequestContext, int, int>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "notifyDeploymentMachineDeleted", (IEnumerable<Type>) null, typeof (DeploymentGroupHubDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      DeploymentGroupHubDispatcher.\u003C\u003Eo__5.\u003C\u003Ep__0.Target((CallSite) DeploymentGroupHubDispatcher.\u003C\u003Eo__5.\u003C\u003Ep__0, this.m_hubContext.Clients.TrackedGroup<object>(requestContext, "DeploymentGroupHub", groupName), requestContext, deploymentGroupId, machineId);
    }

    public void NotifyDeploymentMachineDisconnected(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      int machineId)
    {
      string groupName = DeploymentGroupHubDispatcher.GetGroupName(requestContext, projectId, deploymentGroupId);
      // ISSUE: reference to a compiler-generated field
      if (DeploymentGroupHubDispatcher.\u003C\u003Eo__6.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        DeploymentGroupHubDispatcher.\u003C\u003Eo__6.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, IVssRequestContext, int, int>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "notifyDeploymentMachineDisconnected", (IEnumerable<Type>) null, typeof (DeploymentGroupHubDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      DeploymentGroupHubDispatcher.\u003C\u003Eo__6.\u003C\u003Ep__0.Target((CallSite) DeploymentGroupHubDispatcher.\u003C\u003Eo__6.\u003C\u003Ep__0, this.m_hubContext.Clients.TrackedGroup<object>(requestContext, "DeploymentGroupHub", groupName), requestContext, deploymentGroupId, machineId);
    }

    public void NotifyDeploymentMachinesUpdated(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      IList<DeploymentMachine> deploymentMachines)
    {
      string groupName = DeploymentGroupHubDispatcher.GetGroupName(requestContext, projectId, deploymentGroupId);
      // ISSUE: reference to a compiler-generated field
      if (DeploymentGroupHubDispatcher.\u003C\u003Eo__7.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        DeploymentGroupHubDispatcher.\u003C\u003Eo__7.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, IVssRequestContext, int, IList<DeploymentMachine>>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "notifyDeploymentMachinesUpdated", (IEnumerable<Type>) null, typeof (DeploymentGroupHubDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      DeploymentGroupHubDispatcher.\u003C\u003Eo__7.\u003C\u003Ep__0.Target((CallSite) DeploymentGroupHubDispatcher.\u003C\u003Eo__7.\u003C\u003Ep__0, this.m_hubContext.Clients.TrackedGroup<object>(requestContext, "DeploymentGroupHub", groupName), requestContext, deploymentGroupId, deploymentMachines);
    }

    public void NotifyAgentRequestQueued(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      TaskAgentJobRequest request)
    {
      string groupName = DeploymentGroupHubDispatcher.GetGroupName(requestContext, projectId, deploymentGroupId);
      // ISSUE: reference to a compiler-generated field
      if (DeploymentGroupHubDispatcher.\u003C\u003Eo__8.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        DeploymentGroupHubDispatcher.\u003C\u003Eo__8.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, IVssRequestContext, int, TaskAgentJobRequest>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "notifyAgentRequestQueued", (IEnumerable<Type>) null, typeof (DeploymentGroupHubDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      DeploymentGroupHubDispatcher.\u003C\u003Eo__8.\u003C\u003Ep__0.Target((CallSite) DeploymentGroupHubDispatcher.\u003C\u003Eo__8.\u003C\u003Ep__0, this.m_hubContext.Clients.TrackedGroup<object>(requestContext, "DeploymentGroupHub", groupName), requestContext, deploymentGroupId, request);
    }

    public void NotifyAgentRequestAssigned(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      TaskAgentJobRequest request)
    {
      string groupName = DeploymentGroupHubDispatcher.GetGroupName(requestContext, projectId, deploymentGroupId);
      // ISSUE: reference to a compiler-generated field
      if (DeploymentGroupHubDispatcher.\u003C\u003Eo__9.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        DeploymentGroupHubDispatcher.\u003C\u003Eo__9.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, IVssRequestContext, int, TaskAgentJobRequest>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "notifyAgentRequestAssigned", (IEnumerable<Type>) null, typeof (DeploymentGroupHubDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      DeploymentGroupHubDispatcher.\u003C\u003Eo__9.\u003C\u003Ep__0.Target((CallSite) DeploymentGroupHubDispatcher.\u003C\u003Eo__9.\u003C\u003Ep__0, this.m_hubContext.Clients.TrackedGroup<object>(requestContext, "DeploymentGroupHub", groupName), requestContext, deploymentGroupId, request);
    }

    public void NotifyAgentRequestStarted(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      TaskAgentJobRequest request)
    {
      string groupName = DeploymentGroupHubDispatcher.GetGroupName(requestContext, projectId, deploymentGroupId);
      // ISSUE: reference to a compiler-generated field
      if (DeploymentGroupHubDispatcher.\u003C\u003Eo__10.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        DeploymentGroupHubDispatcher.\u003C\u003Eo__10.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, IVssRequestContext, int, TaskAgentJobRequest>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "notifyAgentRequestStarted", (IEnumerable<Type>) null, typeof (DeploymentGroupHubDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      DeploymentGroupHubDispatcher.\u003C\u003Eo__10.\u003C\u003Ep__0.Target((CallSite) DeploymentGroupHubDispatcher.\u003C\u003Eo__10.\u003C\u003Ep__0, this.m_hubContext.Clients.TrackedGroup<object>(requestContext, "DeploymentGroupHub", groupName), requestContext, deploymentGroupId, request);
    }

    public void NotifyAgentRequestCompleted(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      TaskAgentJobRequest request)
    {
      string groupName = DeploymentGroupHubDispatcher.GetGroupName(requestContext, projectId, deploymentGroupId);
      // ISSUE: reference to a compiler-generated field
      if (DeploymentGroupHubDispatcher.\u003C\u003Eo__11.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        DeploymentGroupHubDispatcher.\u003C\u003Eo__11.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, IVssRequestContext, int, TaskAgentJobRequest>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "notifyAgentRequestCompleted", (IEnumerable<Type>) null, typeof (DeploymentGroupHubDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      DeploymentGroupHubDispatcher.\u003C\u003Eo__11.\u003C\u003Ep__0.Target((CallSite) DeploymentGroupHubDispatcher.\u003C\u003Eo__11.\u003C\u003Ep__0, this.m_hubContext.Clients.TrackedGroup<object>(requestContext, "DeploymentGroupHub", groupName), requestContext, deploymentGroupId, request);
    }

    private static string GetGroupName(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}_{2}_{3}", (object) "DeploymentGroupHub", (object) requestContext.ServiceHost.CollectionServiceHost.InstanceId, (object) projectId, (object) deploymentGroupId);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext) => this.m_hubContext = GlobalHost.ConnectionManager.GetHubContext<DeploymentGroupHub>();
  }
}
