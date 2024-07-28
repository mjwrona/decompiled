// Decompiled with JetBrains decompiler
// Type: Nest.GeoTileGridAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class GeoTileGridAggregationDescriptor<T> : 
    BucketAggregationDescriptorBase<GeoTileGridAggregationDescriptor<T>, IGeoTileGridAggregation, T>,
    IGeoTileGridAggregation,
    IBucketAggregation,
    IAggregation
    where T : class
  {
    Nest.Field IGeoTileGridAggregation.Field { get; set; }

    GeoTilePrecision? IGeoTileGridAggregation.Precision { get; set; }

    int? IGeoTileGridAggregation.ShardSize { get; set; }

    int? IGeoTileGridAggregation.Size { get; set; }

    public GeoTileGridAggregationDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IGeoTileGridAggregation, Nest.Field>) ((a, v) => a.Field = v));

    public GeoTileGridAggregationDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<IGeoTileGridAggregation, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public GeoTileGridAggregationDescriptor<T> Size(int? size) => this.Assign<int?>(size, (Action<IGeoTileGridAggregation, int?>) ((a, v) => a.Size = v));

    public GeoTileGridAggregationDescriptor<T> ShardSize(int? shardSize) => this.Assign<int?>(shardSize, (Action<IGeoTileGridAggregation, int?>) ((a, v) => a.ShardSize = v));

    public GeoTileGridAggregationDescriptor<T> Precision(GeoTilePrecision? precision) => this.Assign<GeoTilePrecision?>(precision, (Action<IGeoTileGridAggregation, GeoTilePrecision?>) ((a, v) => a.Precision = v));
  }
}
