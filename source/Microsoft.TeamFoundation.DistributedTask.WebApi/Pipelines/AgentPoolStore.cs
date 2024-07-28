// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.AgentPoolStore
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class AgentPoolStore : IAgentPoolStore
  {
    private readonly Dictionary<int, TaskAgentPool> m_resourcesById = new Dictionary<int, TaskAgentPool>();
    private readonly Dictionary<string, TaskAgentPool> m_resourcesByName = new Dictionary<string, TaskAgentPool>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public AgentPoolStore(IList<TaskAgentPool> pools, IAgentPoolResolver resolver = null)
    {
      this.Resolver = resolver;
      this.Add(pools != null ? pools.ToArray<TaskAgentPool>() : (TaskAgentPool[]) null);
    }

    public IAgentPoolResolver Resolver { get; }

    public void Authorize(IList<AgentPoolReference> pools)
    {
      if (pools == null || pools.Count <= 0)
        return;
      foreach (AgentPoolReference pool in (IEnumerable<AgentPoolReference>) pools)
      {
        IAgentPoolResolver resolver = this.Resolver;
        TaskAgentPool taskAgentPool = resolver != null ? resolver.Resolve(pool) : (TaskAgentPool) null;
        if (taskAgentPool != null)
          this.Add(taskAgentPool);
      }
    }

    public IList<AgentPoolReference> GetAuthorizedReferences() => (IList<AgentPoolReference>) this.m_resourcesById.Values.Select<TaskAgentPool, AgentPoolReference>((Func<TaskAgentPool, AgentPoolReference>) (x => new AgentPoolReference()
    {
      Id = x.Id
    })).ToList<AgentPoolReference>();

    public TaskAgentPool Get(AgentPoolReference reference)
    {
      if (reference == null)
        return (TaskAgentPool) null;
      int id = reference.Id;
      string literal = reference.Name?.Literal;
      if (reference.Id == 0 && string.IsNullOrEmpty(literal))
        return (TaskAgentPool) null;
      TaskAgentPool taskAgentPool = (TaskAgentPool) null;
      if (id != 0)
      {
        if (this.m_resourcesById.TryGetValue(id, out taskAgentPool))
          return taskAgentPool;
      }
      else if (!string.IsNullOrEmpty(literal) && this.m_resourcesByName.TryGetValue(literal, out taskAgentPool))
        return taskAgentPool;
      IAgentPoolResolver resolver = this.Resolver;
      taskAgentPool = resolver != null ? resolver.Resolve(reference) : (TaskAgentPool) null;
      if (taskAgentPool != null)
        this.Add(taskAgentPool);
      return taskAgentPool;
    }

    private void Add(params TaskAgentPool[] resources)
    {
      if (resources == null || resources.Length == 0)
        return;
      foreach (TaskAgentPool resource in resources)
      {
        TaskAgentPool taskAgentPool;
        if (!this.m_resourcesById.TryGetValue(resource.Id, out taskAgentPool))
        {
          this.m_resourcesById.Add(resource.Id, resource);
          if (!this.m_resourcesByName.TryGetValue(resource.Name, out taskAgentPool))
            this.m_resourcesByName.Add(resource.Name, resource);
        }
      }
    }
  }
}
