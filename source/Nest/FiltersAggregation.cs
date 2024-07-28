// Decompiled with JetBrains decompiler
// Type: Nest.FiltersAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class FiltersAggregation : 
    BucketAggregationBase,
    IFiltersAggregation,
    IBucketAggregation,
    IAggregation
  {
    internal FiltersAggregation()
    {
    }

    public FiltersAggregation(string name)
      : base(name)
    {
    }

    public Union<INamedFiltersContainer, IEnumerable<QueryContainer>> Filters { get; set; }

    public bool? OtherBucket { get; set; }

    public string OtherBucketKey { get; set; }

    internal override void WrapInContainer(AggregationContainer c) => c.Filters = (IFiltersAggregation) this;
  }
}
