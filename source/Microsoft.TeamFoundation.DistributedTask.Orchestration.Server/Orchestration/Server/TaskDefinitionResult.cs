// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskDefinitionResult
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal sealed class TaskDefinitionResult : IEnumerable<TaskDefinition>, IEnumerable
  {
    private readonly int? m_concurrency;
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<TaskVersion, TaskDefinition>> m_definitions;
    private int m_count;

    internal TaskDefinitionResult()
      : this(new int?())
    {
    }

    internal TaskDefinitionResult(int? concurrency)
    {
      this.m_concurrency = concurrency;
      if (this.m_concurrency.HasValue)
        this.m_definitions = new ConcurrentDictionary<Guid, ConcurrentDictionary<TaskVersion, TaskDefinition>>(this.m_concurrency.Value, (IEnumerable<KeyValuePair<Guid, ConcurrentDictionary<TaskVersion, TaskDefinition>>>) Array.Empty<KeyValuePair<Guid, ConcurrentDictionary<TaskVersion, TaskDefinition>>>(), (IEqualityComparer<Guid>) EqualityComparer<Guid>.Default);
      else
        this.m_definitions = new ConcurrentDictionary<Guid, ConcurrentDictionary<TaskVersion, TaskDefinition>>();
    }

    internal int Count => this.m_count;

    IEnumerator<TaskDefinition> IEnumerable<TaskDefinition>.GetEnumerator()
    {
      foreach (ConcurrentDictionary<TaskVersion, TaskDefinition> concurrentDictionary in (IEnumerable<ConcurrentDictionary<TaskVersion, TaskDefinition>>) this.m_definitions.Values)
      {
        foreach (TaskDefinition taskDefinition in (IEnumerable<TaskDefinition>) concurrentDictionary.Values)
          yield return taskDefinition;
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) ((IEnumerable<TaskDefinition>) this).GetEnumerator();

    internal void Add(IEnumerable<TaskDefinition> definitions)
    {
      foreach (TaskDefinition definition in definitions)
        this.Add(definition);
    }

    internal void Add(TaskDefinition definition)
    {
      if (!this.m_definitions.GetOrAdd(definition.Id, (Func<Guid, ConcurrentDictionary<TaskVersion, TaskDefinition>>) (x => this.m_concurrency.HasValue ? new ConcurrentDictionary<TaskVersion, TaskDefinition>(this.m_concurrency.Value, (IEnumerable<KeyValuePair<TaskVersion, TaskDefinition>>) Array.Empty<KeyValuePair<TaskVersion, TaskDefinition>>(), (IEqualityComparer<TaskVersion>) EqualityComparer<TaskVersion>.Default) : new ConcurrentDictionary<TaskVersion, TaskDefinition>())).TryAdd(definition.Version, definition))
        return;
      Interlocked.Increment(ref this.m_count);
    }

    internal IEnumerable<TaskDefinition> Get(Guid? id, TaskVersion version)
    {
      if (!id.HasValue)
        return (IEnumerable<TaskDefinition>) this;
      if (version != (TaskVersion) null)
      {
        ConcurrentDictionary<TaskVersion, TaskDefinition> concurrentDictionary;
        TaskDefinition taskDefinition;
        if (this.m_definitions.TryGetValue(id.Value, out concurrentDictionary) && concurrentDictionary.TryGetValue(version, out taskDefinition))
          return (IEnumerable<TaskDefinition>) new TaskDefinition[1]
          {
            taskDefinition
          };
      }
      else
      {
        ConcurrentDictionary<TaskVersion, TaskDefinition> concurrentDictionary;
        if (this.m_definitions.TryGetValue(id.Value, out concurrentDictionary))
          return (IEnumerable<TaskDefinition>) concurrentDictionary.Values;
      }
      return (IEnumerable<TaskDefinition>) Array.Empty<TaskDefinition>();
    }

    internal IEnumerable<TaskDefinition> Get(string[] visibility, bool allVersions = false)
    {
      foreach (ConcurrentDictionary<TaskVersion, TaskDefinition> concurrentDictionary in (IEnumerable<ConcurrentDictionary<TaskVersion, TaskDefinition>>) this.m_definitions.Values)
      {
        if (concurrentDictionary[concurrentDictionary.Keys.Max<TaskVersion>()].IsVisible(visibility))
        {
          if (allVersions)
          {
            foreach (TaskDefinition taskDefinition in (IEnumerable<TaskDefinition>) concurrentDictionary.Values)
              yield return taskDefinition;
          }
          else
          {
            foreach (IEnumerable<TaskDefinition> source in concurrentDictionary.Values.GroupBy<TaskDefinition, int>((Func<TaskDefinition, int>) (x => x.Version.Major)))
              yield return source.OrderByDescending<TaskDefinition, TaskVersion>((Func<TaskDefinition, TaskVersion>) (x => x.Version)).First<TaskDefinition>();
          }
        }
      }
    }
  }
}
