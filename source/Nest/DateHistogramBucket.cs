// Decompiled with JetBrains decompiler
// Type: Nest.DateHistogramBucket
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class DateHistogramBucket : KeyedBucket<double>
  {
    private static readonly long EpochTicks = new DateTimeOffset(1970, 1, 1, 0, 0, 0, 0, TimeSpan.Zero).Ticks;

    public DateHistogramBucket(IReadOnlyDictionary<string, IAggregate> dict)
      : base(dict)
    {
    }

    public DateTime Date => new DateTime(DateHistogramBucket.EpochTicks + (long) this.Key * 10000L, DateTimeKind.Utc);
  }
}
