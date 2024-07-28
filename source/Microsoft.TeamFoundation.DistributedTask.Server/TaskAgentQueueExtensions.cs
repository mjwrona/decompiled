// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskAgentQueueExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class TaskAgentQueueExtensions
  {
    public static TaskAgentQueue PopulateReferences(
      this TaskAgentQueue queue,
      IVssRequestContext requestContext)
    {
      if (queue == null || queue.Pool == null)
        return queue;
      return new List<TaskAgentQueue>() { queue }.PopulateReferences(requestContext).FirstOrDefault<TaskAgentQueue>();
    }

    public static IList<TaskAgentQueue> PopulateReferences(
      this IList<TaskAgentQueue> queues,
      IVssRequestContext requestContext)
    {
      if (queues == null || !queues.Any<TaskAgentQueue>())
        return queues;
      IDistributedTaskResourceService service = requestContext.GetService<IDistributedTaskResourceService>();
      List<int> list = queues.Where<TaskAgentQueue>((Func<TaskAgentQueue, bool>) (x => x.Pool != null)).Select<TaskAgentQueue, int>((Func<TaskAgentQueue, int>) (x => x.Pool.Id)).ToList<int>();
      IVssRequestContext requestContext1 = requestContext.Elevate();
      List<int> poolIds = list;
      Dictionary<int, TaskAgentPool> dictionary = service.GetAgentPoolsByIds(requestContext1, (IList<int>) poolIds).ToDictionary<TaskAgentPool, int>((Func<TaskAgentPool, int>) (x => x.Id));
      foreach (TaskAgentQueue queue in (IEnumerable<TaskAgentQueue>) queues)
      {
        TaskAgentPool taskAgentPool;
        if (queue.Pool != null && dictionary.TryGetValue(queue.Pool.Id, out taskAgentPool))
        {
          queue.Pool.Name = taskAgentPool.Name;
          queue.Pool.Scope = taskAgentPool.Scope;
          queue.Pool.IsHosted = taskAgentPool.IsHosted;
          queue.Pool.Size = taskAgentPool.Size;
          queue.Pool.IsLegacy = taskAgentPool.IsLegacy;
          queue.Pool.Options = taskAgentPool.Options;
        }
      }
      return queues;
    }
  }
}
