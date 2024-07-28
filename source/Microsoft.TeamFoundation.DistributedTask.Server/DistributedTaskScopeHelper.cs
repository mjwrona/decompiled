// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DistributedTaskScopeHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class DistributedTaskScopeHelper
  {
    public static string GenerateAgentListenScope(int poolId, int agentId, long jobRequestId) => ScopeHelpers.ConstructScope(DistributedTaskScopes.AgentListenScope, poolId.ToString(), agentId.ToString(), jobRequestId.ToString());

    public static string GenerateAgentCloudRequestUseScope(
      int agentCloudId,
      Guid agentCloudRequestId)
    {
      return ScopeHelpers.ConstructScope(DistributedTaskScopes.AgentCloudRequestUse, agentCloudId.ToString(), agentCloudRequestId.ToString());
    }

    public static string GenerateAgentCloudRequestListenScope(
      int agentCloudId,
      Guid agentCloudRequestId)
    {
      return ScopeHelpers.ConstructScope(DistributedTaskScopes.AgentCloudRequestListen, agentCloudId.ToString(), agentCloudRequestId.ToString());
    }

    public static string GeneratePoolManageScope(int poolId) => ScopeHelpers.ConstructScope(DistributedTaskScopes.PoolManage, poolId.ToString());
  }
}
