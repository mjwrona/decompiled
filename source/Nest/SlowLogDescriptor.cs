// Decompiled with JetBrains decompiler
// Type: Nest.SlowLogDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SlowLogDescriptor : DescriptorBase<SlowLogDescriptor, ISlowLog>, ISlowLog
  {
    ISlowLogIndexing ISlowLog.Indexing { get; set; }

    ISlowLogSearch ISlowLog.Search { get; set; }

    public SlowLogDescriptor Search(
      Func<SlowLogSearchDescriptor, ISlowLogSearch> selector)
    {
      return this.Assign<Func<SlowLogSearchDescriptor, ISlowLogSearch>>(selector, (Action<ISlowLog, Func<SlowLogSearchDescriptor, ISlowLogSearch>>) ((a, v) => a.Search = v != null ? v(new SlowLogSearchDescriptor()) : (ISlowLogSearch) null));
    }

    public SlowLogDescriptor Indexing(
      Func<SlowLogIndexingDescriptor, ISlowLogIndexing> selector)
    {
      return this.Assign<Func<SlowLogIndexingDescriptor, ISlowLogIndexing>>(selector, (Action<ISlowLog, Func<SlowLogIndexingDescriptor, ISlowLogIndexing>>) ((a, v) => a.Indexing = v != null ? v(new SlowLogIndexingDescriptor()) : (ISlowLogIndexing) null));
    }
  }
}
