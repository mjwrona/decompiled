// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.AgentPoolQueueCache
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public class AgentPoolQueueCache
  {
    private Dictionary<Tuple<Guid, int>, TaskAgentQueue> queues;
    private IVssRequestContext context;

    public AgentPoolQueueCache(IVssRequestContext requestContext)
    {
      this.context = requestContext;
      this.queues = new Dictionary<Tuple<Guid, int>, TaskAgentQueue>();
    }

    public Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue GetQueue(
      Guid projectId,
      int queueId,
      ISecuredObject securedObject)
    {
      using (PerformanceTimer.StartMeasure(this.context, "AgentPoolQueueCache.GetQueue"))
      {
        Tuple<Guid, int> key = new Tuple<Guid, int>(projectId, queueId);
        if (this.queues.ContainsKey(key))
          return this.queues[key].AsBuildQueue(this.context, securedObject, false);
        TaskAgentQueue agentQueue = this.context.GetService<IDistributedTaskPoolService>().GetAgentQueue(this.context, projectId, queueId);
        this.queues[key] = agentQueue;
        return agentQueue.AsBuildQueue(this.context, securedObject, false);
      }
    }
  }
}
