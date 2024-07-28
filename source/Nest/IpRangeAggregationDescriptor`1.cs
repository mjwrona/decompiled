// Decompiled with JetBrains decompiler
// Type: Nest.IpRangeAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Nest
{
  public class IpRangeAggregationDescriptor<T> : 
    BucketAggregationDescriptorBase<IpRangeAggregationDescriptor<T>, IIpRangeAggregation, T>,
    IIpRangeAggregation,
    IBucketAggregation,
    IAggregation
    where T : class
  {
    Nest.Field IIpRangeAggregation.Field { get; set; }

    IEnumerable<IIpRangeAggregationRange> IIpRangeAggregation.Ranges { get; set; }

    public IpRangeAggregationDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IIpRangeAggregation, Nest.Field>) ((a, v) => a.Field = v));

    public IpRangeAggregationDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<IIpRangeAggregation, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public IpRangeAggregationDescriptor<T> Ranges(
      params Func<IpRangeAggregationRangeDescriptor, IIpRangeAggregationRange>[] ranges)
    {
      return this.Assign<IEnumerable<IIpRangeAggregationRange>>(ranges != null ? ((IEnumerable<Func<IpRangeAggregationRangeDescriptor, IIpRangeAggregationRange>>) ranges).Select<Func<IpRangeAggregationRangeDescriptor, IIpRangeAggregationRange>, IIpRangeAggregationRange>((Func<Func<IpRangeAggregationRangeDescriptor, IIpRangeAggregationRange>, IIpRangeAggregationRange>) (r => r(new IpRangeAggregationRangeDescriptor()))) : (IEnumerable<IIpRangeAggregationRange>) null, (Action<IIpRangeAggregation, IEnumerable<IIpRangeAggregationRange>>) ((a, v) => a.Ranges = v));
    }
  }
}
