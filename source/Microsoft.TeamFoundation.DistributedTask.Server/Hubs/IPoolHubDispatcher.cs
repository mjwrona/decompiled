// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Hubs.IPoolHubDispatcher
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Hubs
{
  [DefaultServiceImplementation(typeof (PoolHubDispatcher))]
  internal interface IPoolHubDispatcher : IVssFrameworkService
  {
    void NotifyAgentAdded(IVssRequestContext requestContext, int poolId, int agentId);

    void NotifyAgentConnected(IVssRequestContext requestContext, int poolId, int agentId);

    void NotifyAgentDeleted(IVssRequestContext requestContext, int poolId, int agentId);

    void NotifyAgentDisconnected(IVssRequestContext requestContext, int poolId, int agentId);

    void NotifyAgentRequestQueued(IVssRequestContext requestContext, int poolId, long requestId);

    void NotifyAgentRequestAssigned(IVssRequestContext requestContext, int poolId, long requestId);

    void NotifyAgentRequestStarted(IVssRequestContext requestContext, int poolId, long requestId);

    void NotifyAgentRequestCompleted(IVssRequestContext requestContext, int poolId, long requestId);

    void NotifyAgentUpdated(IVssRequestContext requestContext, int poolId, int agentId);

    Task Subscribe(IVssRequestContext requestContext, int poolId, string connectionId);

    Task Unsubscribe(IVssRequestContext requestContext, int poolId, string connectionId);
  }
}
