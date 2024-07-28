// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskDefinitionDataResult
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal sealed class TaskDefinitionDataResult : IEnumerable<TaskDefinitionData>, IEnumerable
  {
    private readonly int? m_concurrency;
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<TaskVersion, TaskDefinitionData>> m_definitions;
    private int m_count;

    internal TaskDefinitionDataResult()
      : this(new int?())
    {
    }

    internal TaskDefinitionDataResult(int? concurrency)
    {
      this.m_concurrency = concurrency;
      if (this.m_concurrency.HasValue)
        this.m_definitions = new ConcurrentDictionary<Guid, ConcurrentDictionary<TaskVersion, TaskDefinitionData>>(this.m_concurrency.Value, (IEnumerable<KeyValuePair<Guid, ConcurrentDictionary<TaskVersion, TaskDefinitionData>>>) Array.Empty<KeyValuePair<Guid, ConcurrentDictionary<TaskVersion, TaskDefinitionData>>>(), (IEqualityComparer<Guid>) EqualityComparer<Guid>.Default);
      else
        this.m_definitions = new ConcurrentDictionary<Guid, ConcurrentDictionary<TaskVersion, TaskDefinitionData>>();
    }

    internal int Count => this.m_count;

    IEnumerator<TaskDefinitionData> IEnumerable<TaskDefinitionData>.GetEnumerator()
    {
      foreach (ConcurrentDictionary<TaskVersion, TaskDefinitionData> concurrentDictionary in (IEnumerable<ConcurrentDictionary<TaskVersion, TaskDefinitionData>>) this.m_definitions.Values)
      {
        foreach (TaskDefinitionData taskDefinitionData in (IEnumerable<TaskDefinitionData>) concurrentDictionary.Values)
          yield return taskDefinitionData;
      }
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) ((IEnumerable<TaskDefinitionData>) this).GetEnumerator();

    internal void Add(IEnumerable<TaskDefinitionData> definitions)
    {
      foreach (TaskDefinitionData definition in definitions)
        this.Add(definition);
    }

    internal void Add(TaskDefinitionData definition)
    {
      if (!this.m_definitions.GetOrAdd(definition.Id, (Func<Guid, ConcurrentDictionary<TaskVersion, TaskDefinitionData>>) (x => this.m_concurrency.HasValue ? new ConcurrentDictionary<TaskVersion, TaskDefinitionData>(this.m_concurrency.Value, (IEnumerable<KeyValuePair<TaskVersion, TaskDefinitionData>>) Array.Empty<KeyValuePair<TaskVersion, TaskDefinitionData>>(), (IEqualityComparer<TaskVersion>) EqualityComparer<TaskVersion>.Default) : new ConcurrentDictionary<TaskVersion, TaskDefinitionData>())).TryAdd(definition.Version, definition))
        return;
      Interlocked.Increment(ref this.m_count);
    }

    internal IEnumerable<TaskDefinitionData> Get(Guid? id, TaskVersion version)
    {
      if (!id.HasValue)
        return (IEnumerable<TaskDefinitionData>) this;
      if (version != (TaskVersion) null)
      {
        ConcurrentDictionary<TaskVersion, TaskDefinitionData> concurrentDictionary;
        TaskDefinitionData taskDefinitionData;
        if (this.m_definitions.TryGetValue(id.Value, out concurrentDictionary) && concurrentDictionary.TryGetValue(version, out taskDefinitionData))
          return (IEnumerable<TaskDefinitionData>) new TaskDefinitionData[1]
          {
            taskDefinitionData
          };
      }
      else
      {
        ConcurrentDictionary<TaskVersion, TaskDefinitionData> concurrentDictionary;
        if (this.m_definitions.TryGetValue(id.Value, out concurrentDictionary))
          return (IEnumerable<TaskDefinitionData>) concurrentDictionary.Values;
      }
      return (IEnumerable<TaskDefinitionData>) Array.Empty<TaskDefinitionData>();
    }

    internal IEnumerable<TaskDefinitionData> Get(string[] visibility, bool allVersions)
    {
      foreach (ConcurrentDictionary<TaskVersion, TaskDefinitionData> concurrentDictionary in (IEnumerable<ConcurrentDictionary<TaskVersion, TaskDefinitionData>>) this.m_definitions.Values)
      {
        if (concurrentDictionary[concurrentDictionary.Keys.Max<TaskVersion>()].IsVisible(visibility))
        {
          if (allVersions)
          {
            foreach (TaskDefinitionData taskDefinitionData in (IEnumerable<TaskDefinitionData>) concurrentDictionary.Values)
              yield return taskDefinitionData;
          }
          else
          {
            foreach (IEnumerable<TaskDefinitionData> source in concurrentDictionary.Values.GroupBy<TaskDefinitionData, int>((Func<TaskDefinitionData, int>) (x => x.Version.Major)))
              yield return source.OrderByDescending<TaskDefinitionData, TaskVersion>((Func<TaskDefinitionData, TaskVersion>) (x => x.Version)).First<TaskDefinitionData>();
          }
        }
      }
    }
  }
}
