// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IDistributedTaskPoolServiceExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class IDistributedTaskPoolServiceExtensions
  {
    public static void VerifyQueuePermission(
      this IDistributedTaskPoolService poolService,
      IVssRequestContext requestContext,
      Guid projectId,
      AgentPoolQueue queue,
      AgentPoolQueue previousQueue = null)
    {
      if (queue == null)
        return;
      if (queue.Id == 0 && !string.IsNullOrEmpty(queue.Name))
        queue.Id = (poolService.GetAgentQueues(requestContext, projectId, queue.Name).SingleOrDefault<TaskAgentQueue>() ?? throw new TaskAgentQueueNotFoundException(BuildServerResources.QueueNotFound((object) queue.Name))).Id;
      if (queue.Id == 0 || previousQueue != null && previousQueue.Id == queue.Id)
        return;
      poolService.CheckUsePermissionForQueue(requestContext, projectId, queue.Id);
    }

    public static void VerifyQueuePermissions(
      this IDistributedTaskPoolService poolService,
      IVssRequestContext requestContext,
      Guid projectId,
      BuildProcess process,
      HashSet<int> verifiedIds)
    {
      if (!(process is DesignerProcess designerProcess))
        return;
      HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);
      foreach (Phase phase in designerProcess.Phases)
      {
        if (phase.Target is AgentPoolQueueTarget target)
        {
          AgentPoolQueue queue = target.Queue;
          if (queue != null && !verifiedIds.Contains(queue.Id) && !stringSet.Contains(queue.Name))
          {
            poolService.VerifyQueuePermission(requestContext, projectId, queue);
            verifiedIds.Add(queue.Id);
            if (!string.IsNullOrEmpty(queue.Name))
              stringSet.Add(queue.Name);
          }
        }
      }
    }
  }
}
