// Decompiled with JetBrains decompiler
// Type: Nest.GeoDistanceAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Nest
{
  public class GeoDistanceAggregationDescriptor<T> : 
    BucketAggregationDescriptorBase<GeoDistanceAggregationDescriptor<T>, IGeoDistanceAggregation, T>,
    IGeoDistanceAggregation,
    IBucketAggregation,
    IAggregation
    where T : class
  {
    GeoDistanceType? IGeoDistanceAggregation.DistanceType { get; set; }

    Nest.Field IGeoDistanceAggregation.Field { get; set; }

    GeoLocation IGeoDistanceAggregation.Origin { get; set; }

    IEnumerable<IAggregationRange> IGeoDistanceAggregation.Ranges { get; set; }

    DistanceUnit? IGeoDistanceAggregation.Unit { get; set; }

    public GeoDistanceAggregationDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IGeoDistanceAggregation, Nest.Field>) ((a, v) => a.Field = v));

    public GeoDistanceAggregationDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<IGeoDistanceAggregation, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public GeoDistanceAggregationDescriptor<T> Origin(double lat, double lon) => this.Assign<GeoLocation>(new GeoLocation(lat, lon), (Action<IGeoDistanceAggregation, GeoLocation>) ((a, v) => a.Origin = v));

    public GeoDistanceAggregationDescriptor<T> Origin(GeoLocation geoLocation) => this.Assign<GeoLocation>(geoLocation, (Action<IGeoDistanceAggregation, GeoLocation>) ((a, v) => a.Origin = v));

    public GeoDistanceAggregationDescriptor<T> Unit(DistanceUnit? unit) => this.Assign<DistanceUnit?>(unit, (Action<IGeoDistanceAggregation, DistanceUnit?>) ((a, v) => a.Unit = v));

    public GeoDistanceAggregationDescriptor<T> DistanceType(GeoDistanceType? geoDistance) => this.Assign<GeoDistanceType?>(geoDistance, (Action<IGeoDistanceAggregation, GeoDistanceType?>) ((a, v) => a.DistanceType = v));

    public GeoDistanceAggregationDescriptor<T> Ranges(
      params Func<AggregationRangeDescriptor, IAggregationRange>[] ranges)
    {
      return this.Assign<IEnumerable<IAggregationRange>>(ranges != null ? ((IEnumerable<Func<AggregationRangeDescriptor, IAggregationRange>>) ranges).Select<Func<AggregationRangeDescriptor, IAggregationRange>, IAggregationRange>((Func<Func<AggregationRangeDescriptor, IAggregationRange>, IAggregationRange>) (r => r(new AggregationRangeDescriptor()))) : (IEnumerable<IAggregationRange>) null, (Action<IGeoDistanceAggregation, IEnumerable<IAggregationRange>>) ((a, v) => a.Ranges = v));
    }
  }
}
