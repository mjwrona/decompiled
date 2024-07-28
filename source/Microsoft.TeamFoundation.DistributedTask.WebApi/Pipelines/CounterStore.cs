// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.CounterStore
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class CounterStore : ICounterStore
  {
    private readonly Dictionary<string, int> m_counters = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public CounterStore(IDictionary<string, int> counters = null, ICounterResolver resolver = null)
    {
      if (counters != null && counters.Count > 0)
        this.m_counters.AddRange<KeyValuePair<string, int>, Dictionary<string, int>>((IEnumerable<KeyValuePair<string, int>>) counters);
      this.Resolver = resolver;
    }

    public IReadOnlyDictionary<string, int> Counters => (IReadOnlyDictionary<string, int>) this.m_counters;

    private ICounterResolver Resolver { get; }

    public int Increment(IPipelineContext context, string prefix, int seed)
    {
      int num1;
      if (this.m_counters.TryGetValue(prefix, out num1))
        return num1;
      int num2 = seed;
      if (this.Resolver != null)
      {
        num2 = this.Resolver.Increment(context, prefix, seed);
        this.m_counters[prefix] = num2;
      }
      return num2;
    }
  }
}
