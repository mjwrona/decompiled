// Decompiled with JetBrains decompiler
// Type: Nest.GeoDistanceSortDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class GeoDistanceSortDescriptor<T> : 
    SortDescriptorBase<GeoDistanceSortDescriptor<T>, IGeoDistanceSort, T>,
    IGeoDistanceSort,
    ISort
    where T : class
  {
    protected override Nest.Field SortKey => (Nest.Field) "_geo_distance";

    GeoDistanceType? IGeoDistanceSort.DistanceType { get; set; }

    Nest.Field IGeoDistanceSort.Field { get; set; }

    DistanceUnit? IGeoDistanceSort.Unit { get; set; }

    bool? IGeoDistanceSort.IgnoreUnmapped { get; set; }

    IEnumerable<GeoLocation> IGeoDistanceSort.Points { get; set; }

    public GeoDistanceSortDescriptor<T> Points(params GeoLocation[] geoLocations) => this.Assign<GeoLocation[]>(geoLocations, (Action<IGeoDistanceSort, GeoLocation[]>) ((a, v) => a.Points = (IEnumerable<GeoLocation>) v));

    public GeoDistanceSortDescriptor<T> Points(IEnumerable<GeoLocation> geoLocations) => this.Assign<IEnumerable<GeoLocation>>(geoLocations, (Action<IGeoDistanceSort, IEnumerable<GeoLocation>>) ((a, v) => a.Points = v));

    public GeoDistanceSortDescriptor<T> Unit(DistanceUnit? unit) => this.Assign<DistanceUnit?>(unit, (Action<IGeoDistanceSort, DistanceUnit?>) ((a, v) => a.Unit = v));

    public GeoDistanceSortDescriptor<T> DistanceType(GeoDistanceType? distanceType) => this.Assign<GeoDistanceType?>(distanceType, (Action<IGeoDistanceSort, GeoDistanceType?>) ((a, v) => a.DistanceType = v));

    public GeoDistanceSortDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IGeoDistanceSort, Nest.Field>) ((a, v) => a.Field = v));

    public GeoDistanceSortDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IGeoDistanceSort, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public GeoDistanceSortDescriptor<T> IgnoreUnmapped(bool? ignoreUnmapped = true) => this.Assign<bool?>(ignoreUnmapped, (Action<IGeoDistanceSort, bool?>) ((a, v) => a.IgnoreUnmapped = v));
  }
}
