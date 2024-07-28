// Decompiled with JetBrains decompiler
// Type: Nest.BucketAggregate
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class BucketAggregate : IAggregate
  {
    public CompositeKey AfterKey { get; set; }

    public long BgCount { get; set; }

    public long DocCount { get; set; }

    public long? DocCountErrorUpperBound { get; set; }

    public IReadOnlyCollection<IBucket> Items { get; set; } = EmptyReadOnly<IBucket>.Collection;

    public IReadOnlyDictionary<string, object> Meta { get; set; } = EmptyReadOnly<string, object>.Dictionary;

    public long? SumOtherDocCount { get; set; }

    [Obsolete("Use AutoInterval. This property is incorrectly mapped to the wrong type")]
    public Time Interval { get; set; }

    public DateMathTime AutoInterval { get; set; }
  }
}
