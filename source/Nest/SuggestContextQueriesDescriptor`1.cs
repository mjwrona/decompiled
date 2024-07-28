// Decompiled with JetBrains decompiler
// Type: Nest.SuggestContextQueriesDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class SuggestContextQueriesDescriptor<T> : 
    DescriptorPromiseBase<SuggestContextQueriesDescriptor<T>, IDictionary<string, IList<ISuggestContextQuery>>>
  {
    public SuggestContextQueriesDescriptor()
      : base((IDictionary<string, IList<ISuggestContextQuery>>) new Dictionary<string, IList<ISuggestContextQuery>>())
    {
    }

    public SuggestContextQueriesDescriptor<T> Context(
      string name,
      params Func<SuggestContextQueryDescriptor<T>, ISuggestContextQuery>[] categoryDescriptors)
    {
      return this.AddContextQueries(name, categoryDescriptors != null ? ((IEnumerable<Func<SuggestContextQueryDescriptor<T>, ISuggestContextQuery>>) categoryDescriptors).Select<Func<SuggestContextQueryDescriptor<T>, ISuggestContextQuery>, ISuggestContextQuery>((Func<Func<SuggestContextQueryDescriptor<T>, ISuggestContextQuery>, ISuggestContextQuery>) (d => d == null ? (ISuggestContextQuery) null : d(new SuggestContextQueryDescriptor<T>()))).ToList<ISuggestContextQuery>() : (List<ISuggestContextQuery>) null);
    }

    private SuggestContextQueriesDescriptor<T> AddContextQueries(
      string name,
      List<ISuggestContextQuery> contextQueries)
    {
      if (contextQueries != null)
        this.PromisedValue.Add(name, (IList<ISuggestContextQuery>) contextQueries);
      return this;
    }
  }
}
