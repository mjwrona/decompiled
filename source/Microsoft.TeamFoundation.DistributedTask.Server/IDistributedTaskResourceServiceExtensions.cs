// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.IDistributedTaskResourceServiceExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class IDistributedTaskResourceServiceExtensions
  {
    public static List<TaskAgentPool> GetAgentPoolsByIds(
      this IDistributedTaskResourceService resourceService,
      IVssRequestContext requestContext,
      IList<int> poolIds,
      IList<string> propertyFilters = null,
      TaskAgentPoolActionFilter actionFilter = TaskAgentPoolActionFilter.None)
    {
      return requestContext.RunSynchronously<List<TaskAgentPool>>((Func<Task<List<TaskAgentPool>>>) (() => resourceService.GetAgentPoolsByIdsAsync(requestContext, poolIds, propertyFilters, actionFilter)));
    }

    public static void FinishAgentRequest(
      this IDistributedTaskResourceService resourceService,
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      TaskResult? jobResult = null,
      bool agentShuttingDown = false)
    {
      requestContext.RunSynchronously((Func<Task>) (() => resourceService.FinishAgentRequestAsync(requestContext, poolId, requestId, jobResult, agentShuttingDown)));
    }

    internal static Microsoft.VisualStudio.Services.Identity.Identity ProvisionServiceIdentity(
      this DistributedTaskResourceService resourceService,
      IVssRequestContext requestContext,
      int poolId,
      AgentPoolServiceAccountRoles role)
    {
      TaskAgentPoolData poolData = resourceService.GetTaskAgentPoolInternal(requestContext, (IList<int>) new int[1]
      {
        poolId
      }).FirstOrDefault<TaskAgentPoolData>();
      return resourceService.ProvisionServiceIdentity(requestContext, poolData, role);
    }
  }
}
