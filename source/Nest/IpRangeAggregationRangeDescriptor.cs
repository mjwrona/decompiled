// Decompiled with JetBrains decompiler
// Type: Nest.IpRangeAggregationRangeDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class IpRangeAggregationRangeDescriptor : 
    DescriptorBase<IpRangeAggregationRangeDescriptor, IIpRangeAggregationRange>,
    IIpRangeAggregationRange
  {
    string IIpRangeAggregationRange.From { get; set; }

    string IIpRangeAggregationRange.Mask { get; set; }

    string IIpRangeAggregationRange.To { get; set; }

    public IpRangeAggregationRangeDescriptor From(string from) => this.Assign<string>(from, (Action<IIpRangeAggregationRange, string>) ((a, v) => a.From = v));

    public IpRangeAggregationRangeDescriptor To(string to) => this.Assign<string>(to, (Action<IIpRangeAggregationRange, string>) ((a, v) => a.To = v));

    public IpRangeAggregationRangeDescriptor Mask(string mask) => this.Assign<string>(mask, (Action<IIpRangeAggregationRange, string>) ((a, v) => a.Mask = v));
  }
}
