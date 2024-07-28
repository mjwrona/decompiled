// Decompiled with JetBrains decompiler
// Type: Nest.FiltersAggregate
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class FiltersAggregate : BucketAggregateBase
  {
    public FiltersAggregate()
      : base(EmptyReadOnly<string, IAggregate>.Dictionary)
    {
    }

    public FiltersAggregate(
      IReadOnlyDictionary<string, IAggregate> aggregations)
      : base(aggregations)
    {
    }

    protected override string Sanitize(string key) => key;

    public IReadOnlyCollection<FiltersBucketItem> Buckets { get; set; } = EmptyReadOnly<FiltersBucketItem>.Collection;

    public SingleBucketAggregate NamedBucket(string key) => this.Global(key);

    public IList<FiltersBucketItem> AnonymousBuckets()
    {
      IReadOnlyCollection<FiltersBucketItem> buckets = this.Buckets;
      return buckets == null ? (IList<FiltersBucketItem>) null : (IList<FiltersBucketItem>) buckets.ToList<FiltersBucketItem>();
    }
  }
}
