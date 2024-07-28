// Decompiled with JetBrains decompiler
// Type: Nest.GeoHashGridAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class GeoHashGridAggregationDescriptor<T> : 
    BucketAggregationDescriptorBase<GeoHashGridAggregationDescriptor<T>, IGeoHashGridAggregation, T>,
    IGeoHashGridAggregation,
    IBucketAggregation,
    IAggregation
    where T : class
  {
    Nest.Field IGeoHashGridAggregation.Field { get; set; }

    Nest.GeoHashPrecision? IGeoHashGridAggregation.Precision { get; set; }

    int? IGeoHashGridAggregation.ShardSize { get; set; }

    int? IGeoHashGridAggregation.Size { get; set; }

    IBoundingBox IGeoHashGridAggregation.Bounds { get; set; }

    public GeoHashGridAggregationDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IGeoHashGridAggregation, Nest.Field>) ((a, v) => a.Field = v));

    public GeoHashGridAggregationDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<IGeoHashGridAggregation, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public GeoHashGridAggregationDescriptor<T> Size(int? size) => this.Assign<int?>(size, (Action<IGeoHashGridAggregation, int?>) ((a, v) => a.Size = v));

    public GeoHashGridAggregationDescriptor<T> ShardSize(int? shardSize) => this.Assign<int?>(shardSize, (Action<IGeoHashGridAggregation, int?>) ((a, v) => a.ShardSize = v));

    public GeoHashGridAggregationDescriptor<T> GeoHashPrecision(Nest.GeoHashPrecision? precision) => this.Assign<Nest.GeoHashPrecision?>(precision, (Action<IGeoHashGridAggregation, Nest.GeoHashPrecision?>) ((a, v) => a.Precision = v));

    public GeoHashGridAggregationDescriptor<T> Bounds(
      Func<BoundingBoxDescriptor, IBoundingBox> selector)
    {
      return this.Assign<Func<BoundingBoxDescriptor, IBoundingBox>>(selector, (Action<IGeoHashGridAggregation, Func<BoundingBoxDescriptor, IBoundingBox>>) ((a, v) => a.Bounds = v != null ? v(new BoundingBoxDescriptor()) : (IBoundingBox) null));
    }
  }
}
