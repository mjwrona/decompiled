// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.AgentQueueStore
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
  public class AgentQueueStore : IAgentQueueStore
  {
    private static readonly Dictionary<string, string[]> s_alternateNames = new Dictionary<string, string[]>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "Hosted macOS",
        new string[1]{ "Hosted macOS Preview" }
      },
      {
        "Hosted macOS Preview",
        new string[1]{ "Hosted macOS" }
      }
    };
    private readonly Dictionary<int, TaskAgentQueue> m_resourcesById = new Dictionary<int, TaskAgentQueue>();
    private readonly Dictionary<string, List<TaskAgentQueue>> m_resourcesByName = new Dictionary<string, List<TaskAgentQueue>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public AgentQueueStore(IList<TaskAgentQueue> queues, IAgentQueueResolver resolver = null)
    {
      this.Resolver = resolver;
      this.Add(queues != null ? queues.ToArray<TaskAgentQueue>() : (TaskAgentQueue[]) null);
    }

    public IAgentQueueResolver Resolver { get; }

    public void Authorize(IList<TaskAgentQueue> queues)
    {
      if (queues == null || queues.Count <= 0)
        return;
      foreach (TaskAgentQueue queue in (IEnumerable<TaskAgentQueue>) queues)
        this.Add(queue);
    }

    public IList<AgentQueueReference> GetAuthorizedReferences() => (IList<AgentQueueReference>) this.m_resourcesById.Values.Select<TaskAgentQueue, AgentQueueReference>((Func<TaskAgentQueue, AgentQueueReference>) (x => new AgentQueueReference()
    {
      Id = x.Id
    })).ToList<AgentQueueReference>();

    public TaskAgentQueue Get(AgentQueueReference reference)
    {
      if (reference == null)
        return (TaskAgentQueue) null;
      int id = reference.Id;
      string literal = reference.Name?.Literal;
      if (reference.Id == 0 && string.IsNullOrEmpty(literal))
        return (TaskAgentQueue) null;
      TaskAgentQueue taskAgentQueue = (TaskAgentQueue) null;
      if (id != 0)
      {
        if (this.m_resourcesById.TryGetValue(id, out taskAgentQueue))
          return taskAgentQueue;
      }
      else
      {
        List<TaskAgentQueue> taskAgentQueueList;
        if (!string.IsNullOrEmpty(literal) && this.m_resourcesByName.TryGetValue(literal, out taskAgentQueueList))
          return taskAgentQueueList.Count <= 1 ? taskAgentQueueList[0] : throw new AmbiguousResourceSpecificationException(PipelineStrings.AmbiguousServiceEndpointSpecification((object) id));
      }
      IAgentQueueResolver resolver = this.Resolver;
      taskAgentQueue = resolver != null ? resolver.Resolve(reference) : (TaskAgentQueue) null;
      if (taskAgentQueue != null)
        this.Add(taskAgentQueue);
      return taskAgentQueue;
    }

    private void Add(params TaskAgentQueue[] resources)
    {
      if (resources == null || resources.Length == 0)
        return;
      foreach (TaskAgentQueue resource in resources)
      {
        if (!this.m_resourcesById.TryGetValue(resource.Id, out TaskAgentQueue _))
        {
          this.m_resourcesById.Add(resource.Id, resource);
          string name = resource.Name;
          if (!string.IsNullOrWhiteSpace(name))
          {
            List<TaskAgentQueue> taskAgentQueueList;
            if (!this.m_resourcesByName.TryGetValue(name, out taskAgentQueueList))
            {
              taskAgentQueueList = new List<TaskAgentQueue>();
              this.m_resourcesByName.Add(name, taskAgentQueueList);
            }
            if (taskAgentQueueList.Count > 0)
            {
              TaskAgentPoolReference pool1 = taskAgentQueueList[0].Pool;
              if ((pool1 != null ? (pool1.IsHosted ? 1 : 0) : 0) != 0)
              {
                TaskAgentPoolReference pool2 = resource.Pool;
                if ((pool2 != null ? (pool2.IsHosted ? 1 : 0) : 0) != 0)
                {
                  taskAgentQueueList[0] = resource;
                  goto label_11;
                }
              }
            }
            taskAgentQueueList.Add(resource);
label_11:
            TaskAgentPoolReference pool3 = resource.Pool;
            string[] strArray;
            if ((pool3 != null ? (pool3.IsHosted ? 1 : 0) : 0) != 0 && AgentQueueStore.s_alternateNames.TryGetValue(name, out strArray))
            {
              foreach (string key in strArray)
              {
                if (!this.m_resourcesByName.TryGetValue(key, out taskAgentQueueList))
                {
                  taskAgentQueueList = new List<TaskAgentQueue>();
                  this.m_resourcesByName.Add(key, taskAgentQueueList);
                }
                if (taskAgentQueueList.Count != 0)
                {
                  TaskAgentPoolReference pool4 = taskAgentQueueList[0].Pool;
                  if ((pool4 != null ? (!pool4.IsHosted ? 1 : 0) : 1) == 0)
                    continue;
                }
                taskAgentQueueList.Add(resource);
              }
            }
          }
        }
      }
    }
  }
}
