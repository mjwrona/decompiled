// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.AgentPoolResolver
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
  internal sealed class AgentPoolResolver : IAgentPoolResolver
  {
    private readonly IVssRequestContext m_requestContext;

    public AgentPoolResolver(IVssRequestContext requestContext) => this.m_requestContext = requestContext;

    public IList<TaskAgentPool> Resolve(ICollection<AgentPoolReference> references) => AgentPoolResolver.Resolve(this.m_requestContext, references);

    internal static IList<TaskAgentPool> Resolve(
      IVssRequestContext requestContext,
      ICollection<AgentPoolReference> references)
    {
      List<TaskAgentPool> taskAgentPoolList = new List<TaskAgentPool>();
      if (references != null && references.Count > 0)
      {
        IDistributedTaskPoolService service = requestContext.GetService<IDistributedTaskPoolService>();
        List<int> list1 = references.Where<AgentPoolReference>((Func<AgentPoolReference, bool>) (x => x.Id != 0)).Select<AgentPoolReference, int>((Func<AgentPoolReference, int>) (x => x.Id)).ToList<int>();
        if (list1.Count > 0)
        {
          foreach (int poolId in list1)
          {
            TaskAgentPool agentPool = service.GetAgentPool(requestContext, poolId);
            if (agentPool != null)
              taskAgentPoolList.Add(agentPool);
          }
        }
        List<string> list2 = references.Where<AgentPoolReference>((Func<AgentPoolReference, bool>) (x => !string.IsNullOrEmpty(x.Name?.Literal))).Select<AgentPoolReference, string>((Func<AgentPoolReference, string>) (x => x.Name.Literal)).ToList<string>();
        if (list2.Count > 0)
        {
          foreach (string poolName in list2)
          {
            TaskAgentPool resolvedPool = service.GetAgentPool(requestContext, poolName);
            if (resolvedPool != null && !list1.Any<int>((Func<int, bool>) (x => x == resolvedPool.Id)))
              taskAgentPoolList.Add(resolvedPool);
          }
        }
      }
      return (IList<TaskAgentPool>) taskAgentPoolList;
    }
  }
}
