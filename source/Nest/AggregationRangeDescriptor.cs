// Decompiled with JetBrains decompiler
// Type: Nest.AggregationRangeDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class AggregationRangeDescriptor : 
    DescriptorBase<AggregationRangeDescriptor, IAggregationRange>,
    IAggregationRange
  {
    double? IAggregationRange.From { get; set; }

    string IAggregationRange.Key { get; set; }

    double? IAggregationRange.To { get; set; }

    public AggregationRangeDescriptor Key(string key) => this.Assign<string>(key, (Action<IAggregationRange, string>) ((a, v) => a.Key = v));

    public AggregationRangeDescriptor From(double? from) => this.Assign<double?>(from, (Action<IAggregationRange, double?>) ((a, v) => a.From = v));

    public AggregationRangeDescriptor To(double? to) => this.Assign<double?>(to, (Action<IAggregationRange, double?>) ((a, v) => a.To = v));
  }
}
