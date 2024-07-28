// Decompiled with JetBrains decompiler
// Type: Nest.SlowLogSearchDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SlowLogSearchDescriptor : 
    DescriptorBase<SlowLogSearchDescriptor, ISlowLogSearch>,
    ISlowLogSearch
  {
    ISlowLogSearchFetch ISlowLogSearch.Fetch { get; set; }

    Nest.LogLevel? ISlowLogSearch.LogLevel { get; set; }

    ISlowLogSearchQuery ISlowLogSearch.Query { get; set; }

    public SlowLogSearchDescriptor LogLevel(Nest.LogLevel? level) => this.Assign<Nest.LogLevel?>(level, (Action<ISlowLogSearch, Nest.LogLevel?>) ((a, v) => a.LogLevel = v));

    public SlowLogSearchDescriptor Query(
      Func<SlowLogSearchQueryDescriptor, ISlowLogSearchQuery> selector)
    {
      return this.Assign<Func<SlowLogSearchQueryDescriptor, ISlowLogSearchQuery>>(selector, (Action<ISlowLogSearch, Func<SlowLogSearchQueryDescriptor, ISlowLogSearchQuery>>) ((a, v) => a.Query = v != null ? v(new SlowLogSearchQueryDescriptor()) : (ISlowLogSearchQuery) null));
    }

    public SlowLogSearchDescriptor Fetch(
      Func<SlowLogSearchFetchDescriptor, ISlowLogSearchFetch> selector)
    {
      return this.Assign<Func<SlowLogSearchFetchDescriptor, ISlowLogSearchFetch>>(selector, (Action<ISlowLogSearch, Func<SlowLogSearchFetchDescriptor, ISlowLogSearchFetch>>) ((a, v) => a.Fetch = v != null ? v(new SlowLogSearchFetchDescriptor()) : (ISlowLogSearchFetch) null));
    }
  }
}
