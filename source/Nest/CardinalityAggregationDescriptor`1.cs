// Decompiled with JetBrains decompiler
// Type: Nest.CardinalityAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class CardinalityAggregationDescriptor<T> : 
    MetricAggregationDescriptorBase<CardinalityAggregationDescriptor<T>, ICardinalityAggregation, T>,
    ICardinalityAggregation,
    IMetricAggregation,
    IAggregation
    where T : class
  {
    int? ICardinalityAggregation.PrecisionThreshold { get; set; }

    bool? ICardinalityAggregation.Rehash { get; set; }

    public CardinalityAggregationDescriptor<T> PrecisionThreshold(int? precisionThreshold) => this.Assign<int?>(precisionThreshold, (Action<ICardinalityAggregation, int?>) ((a, v) => a.PrecisionThreshold = v));

    public CardinalityAggregationDescriptor<T> Rehash(bool? rehash = true) => this.Assign<bool?>(rehash, (Action<ICardinalityAggregation, bool?>) ((a, v) => a.Rehash = v));
  }
}
