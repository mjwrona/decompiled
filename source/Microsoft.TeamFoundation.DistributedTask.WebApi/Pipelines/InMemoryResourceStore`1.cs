// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.InMemoryResourceStore`1
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  public abstract class InMemoryResourceStore<T> where T : Resource
  {
    private Dictionary<string, T> m_resources;

    protected InMemoryResourceStore(IEnumerable<T> resources) => this.m_resources = (resources != null ? resources.ToDictionary<T, string, T>((Func<T, string>) (x => x.Alias), (Func<T, T>) (x => x), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (Dictionary<string, T>) null) ?? new Dictionary<string, T>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public int Count => this.m_resources.Count;

    public void Add(T resource) => this.m_resources.Add(resource.Alias, resource);

    public void Add(IEnumerable<T> resources)
    {
      foreach (T resource in resources)
        this.m_resources.Add(resource.Alias, resource);
    }

    public T Get(string alias)
    {
      T obj;
      return this.m_resources.TryGetValue(alias, out obj) ? obj : default (T);
    }

    public IEnumerable<T> GetAll() => (IEnumerable<T>) this.m_resources.Values.ToList<T>();
  }
}
