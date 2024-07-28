// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Hubs.PoolsHubDispatcher
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
  internal sealed class PoolsHubDispatcher : IPoolsHubDispatcher, IVssFrameworkService
  {
    private IHubContext m_hubContext;

    public async Task Subscribe(IVssRequestContext requestContext, Guid orgId, string connectionId)
    {
      string groupName = PoolsHubDispatcher.GetGroupName(orgId);
      await this.m_hubContext.Groups.AddTrackedConnection(requestContext, "PoolsHub", groupName, connectionId).ConfigureAwait(false);
    }

    public async Task Unsubscribe(
      IVssRequestContext requestContext,
      Guid orgId,
      string connectionId)
    {
      string groupName = PoolsHubDispatcher.GetGroupName(orgId);
      await this.m_hubContext.Groups.RemoveTrackedConnection(requestContext, "PoolsHub", groupName, connectionId).ConfigureAwait(false);
    }

    internal Task Disconnect(IVssRequestContext requestContext, string connectionId) => this.m_hubContext.RemoveTrackedConnection(requestContext, "PoolsHub", connectionId);

    public void NotifyAgentRequestUpdated(
      IVssRequestContext requestContext,
      TaskAgentJobRequest request)
    {
      string groupName = PoolsHubDispatcher.GetGroupName(requestContext);
      // ISSUE: reference to a compiler-generated field
      if (PoolsHubDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        PoolsHubDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__0 = CallSite<Action<CallSite, object, IVssRequestContext, long, int>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "notifyAgentRequestUpdated", (IEnumerable<Type>) null, typeof (PoolsHubDispatcher), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[4]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      PoolsHubDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__0.Target((CallSite) PoolsHubDispatcher.\u003C\u003Eo__3.\u003C\u003Ep__0, this.m_hubContext.Clients.TrackedGroup<object>(requestContext, "PoolsHub", groupName), requestContext, request.RequestId, request.PoolId);
    }

    private static string GetGroupName(IVssRequestContext requestContext) => PoolsHubDispatcher.GetGroupName(requestContext.ServiceHost.InstanceId);

    private static string GetGroupName(Guid orgId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1:N}", (object) "PoolsHub", (object) orgId);

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext) => this.m_hubContext = GlobalHost.ConnectionManager.GetHubContext<PoolsHub>();
  }
}
