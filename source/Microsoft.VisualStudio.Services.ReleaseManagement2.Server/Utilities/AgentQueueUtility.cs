// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.AgentQueueUtility
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public static class AgentQueueUtility
  {
    public static int GetFirstMatchingHostedPoolId(
      IVssRequestContext context,
      IEnumerable<TaskAgentQueue> agentQueues)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (agentQueues == null)
        throw new ArgumentNullException(nameof (agentQueues));
      List<TaskAgentPool> source = context.ExecuteAsyncAndSyncResult<List<TaskAgentPool>>((Func<Task<List<TaskAgentPool>>>) (() => context.GetClient<TaskAgentHttpClient>().GetAgentPoolsAsync()));
      List<int> list = source.Where<TaskAgentPool>((Func<TaskAgentPool, bool>) (x => x.IsHosted && string.Equals(x.Name, "Hosted Windows 2019 with VS2019", StringComparison.OrdinalIgnoreCase))).Select<TaskAgentPool, int>((Func<TaskAgentPool, int>) (x => x.Id)).ToList<int>();
      if (list.Count == 0)
        list = source.Where<TaskAgentPool>((Func<TaskAgentPool, bool>) (x => x.IsHosted)).Select<TaskAgentPool, int>((Func<TaskAgentPool, int>) (x => x.Id)).ToList<int>();
      IEnumerable<int> second = agentQueues.Where<TaskAgentQueue>((Func<TaskAgentQueue, bool>) (x => x.Pool != null)).Select<TaskAgentQueue, int>((Func<TaskAgentQueue, int>) (x => x.Pool.Id));
      return list.Intersect<int>(second).FirstOrDefault<int>();
    }

    public static TaskAgentQueue GetFirstMatchingHostedOrDefaultQueue(
      IVssRequestContext context,
      Guid projectId)
    {
      TaskAgentHttpClient taskAgentClient = context != null ? context.GetClient<TaskAgentHttpClient>() : throw new ArgumentNullException(nameof (context));
      Func<Task<List<TaskAgentQueue>>> func = (Func<Task<List<TaskAgentQueue>>>) (() => taskAgentClient.GetAgentQueuesAsync(projectId));
      List<TaskAgentQueue> result = context.ExecuteAsyncAndGetResult<List<TaskAgentQueue>>(func);
      if (result == null || result.Count <= 0)
        throw new InvalidOperationException("No Agent queue exists");
      List<TaskAgentQueue> list = result.Where<TaskAgentQueue>((Func<TaskAgentQueue, bool>) (x => x.Pool != null)).Select<TaskAgentQueue, TaskAgentQueue>((Func<TaskAgentQueue, TaskAgentQueue>) (x => x)).ToList<TaskAgentQueue>();
      int poolId = list != null && list.Count > 0 ? AgentQueueUtility.GetFirstMatchingHostedPoolId(context, (IEnumerable<TaskAgentQueue>) list) : throw new InvalidOperationException("No agent queue exists which has a non-null pool.");
      return poolId == 0 ? list.FirstOrDefault<TaskAgentQueue>() : list.Find((Predicate<TaskAgentQueue>) (q => q.Pool.Id == poolId));
    }

    public static TaskAgentQueue GetFirstMatchingDefaultQueue(
      IEnumerable<TaskAgentQueue> agentQueues)
    {
      if (agentQueues == null)
        throw new ArgumentNullException(nameof (agentQueues));
      return agentQueues.FirstOrDefault<TaskAgentQueue>((Func<TaskAgentQueue, bool>) (e => e.Name == "Default")) ?? agentQueues.FirstOrDefault<TaskAgentQueue>();
    }
  }
}
