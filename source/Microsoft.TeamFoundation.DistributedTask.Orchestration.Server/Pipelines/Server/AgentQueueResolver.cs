// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.AgentQueueResolver
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  internal sealed class AgentQueueResolver : IAgentQueueResolver
  {
    private readonly Guid m_projectId;
    private readonly IVssRequestContext m_requestContext;

    public AgentQueueResolver(IVssRequestContext requestContext, Guid projectId)
    {
      this.m_projectId = projectId;
      this.m_requestContext = requestContext;
    }

    public IList<TaskAgentQueue> Resolve(
      ICollection<AgentQueueReference> references,
      ResourceActionFilter actionFilter = ResourceActionFilter.Use)
    {
      return AgentQueueResolver.Resolve(this.m_requestContext, this.m_projectId, references, actionFilter);
    }

    internal static IList<TaskAgentQueue> Resolve(
      IVssRequestContext requestContext,
      Guid projectId,
      ICollection<AgentQueueReference> references,
      ResourceActionFilter actionFilter = ResourceActionFilter.Use)
    {
      TaskAgentQueueActionFilter queueActionFilter = AgentQueueResolver.ParseToTaskAgentQueueActionFilter(actionFilter);
      List<TaskAgentQueue> taskAgentQueueList = new List<TaskAgentQueue>();
      if (references != null && references.Count > 0)
      {
        IDistributedTaskPoolService service = requestContext.GetService<IDistributedTaskPoolService>();
        List<int> list1 = references.Where<AgentQueueReference>((Func<AgentQueueReference, bool>) (x => x.Id != 0)).Select<AgentQueueReference, int>((Func<AgentQueueReference, int>) (x => x.Id)).ToList<int>();
        if (list1.Count > 0)
        {
          IList<TaskAgentQueue> agentQueues = service.GetAgentQueues(requestContext, projectId, (IEnumerable<int>) list1, new TaskAgentQueueActionFilter?(queueActionFilter));
          taskAgentQueueList.AddRange((IEnumerable<TaskAgentQueue>) agentQueues);
        }
        List<string> list2 = references.Where<AgentQueueReference>((Func<AgentQueueReference, bool>) (x => !string.IsNullOrEmpty(x.Name?.Literal))).Select<AgentQueueReference, string>((Func<AgentQueueReference, string>) (x => x.Name.Literal)).ToList<string>();
        if (list2.Count > 0)
        {
          IList<TaskAgentQueue> agentQueues = service.GetAgentQueues(requestContext, projectId, (IEnumerable<string>) list2, new TaskAgentQueueActionFilter?(queueActionFilter));
          taskAgentQueueList.AddRange((IEnumerable<TaskAgentQueue>) agentQueues);
        }
      }
      return (IList<TaskAgentQueue>) taskAgentQueueList;
    }

    private static TaskAgentQueueActionFilter ParseToTaskAgentQueueActionFilter(
      ResourceActionFilter actionFilter)
    {
      TaskAgentQueueActionFilter queueActionFilter = TaskAgentQueueActionFilter.Use;
      switch (actionFilter)
      {
        case ResourceActionFilter.None:
          queueActionFilter = TaskAgentQueueActionFilter.None;
          break;
        case ResourceActionFilter.Manage:
          queueActionFilter = TaskAgentQueueActionFilter.Manage;
          break;
        case ResourceActionFilter.Use:
          queueActionFilter = TaskAgentQueueActionFilter.Use;
          break;
      }
      return queueActionFilter;
    }
  }
}
