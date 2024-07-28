// Decompiled with JetBrains decompiler
// Type: Nest.GeoDistanceAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class GeoDistanceAggregation : 
    BucketAggregationBase,
    IGeoDistanceAggregation,
    IBucketAggregation,
    IAggregation
  {
    internal GeoDistanceAggregation()
    {
    }

    public GeoDistanceAggregation(string name)
      : base(name)
    {
    }

    public GeoDistanceType? DistanceType { get; set; }

    public Field Field { get; set; }

    public GeoLocation Origin { get; set; }

    public IEnumerable<IAggregationRange> Ranges { get; set; }

    public DistanceUnit? Unit { get; set; }

    internal override void WrapInContainer(AggregationContainer c) => c.GeoDistance = (IGeoDistanceAggregation) this;
  }
}
