// Decompiled with JetBrains decompiler
// Type: Nest.VariableWidthHistogramAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class VariableWidthHistogramAggregationDescriptor<T> : 
    BucketAggregationDescriptorBase<VariableWidthHistogramAggregationDescriptor<T>, IVariableWidthHistogramAggregation, T>,
    IVariableWidthHistogramAggregation,
    IBucketAggregation,
    IAggregation
    where T : class
  {
    Nest.Field IVariableWidthHistogramAggregation.Field { get; set; }

    int? IVariableWidthHistogramAggregation.Buckets { get; set; }

    int? IVariableWidthHistogramAggregation.InitialBuffer { get; set; }

    int? IVariableWidthHistogramAggregation.ShardSize { get; set; }

    public VariableWidthHistogramAggregationDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IVariableWidthHistogramAggregation, Nest.Field>) ((a, v) => a.Field = v));

    public VariableWidthHistogramAggregationDescriptor<T> Field<TValue>(
      Expression<Func<T, TValue>> field)
    {
      return this.Assign<Expression<Func<T, TValue>>>(field, (Action<IVariableWidthHistogramAggregation, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));
    }

    public VariableWidthHistogramAggregationDescriptor<T> Buckets(int? buckets) => this.Assign<int?>(buckets, (Action<IVariableWidthHistogramAggregation, int?>) ((a, v) => a.Buckets = v));

    public VariableWidthHistogramAggregationDescriptor<T> InitialBuffer(int? initialBuffer) => this.Assign<int?>(initialBuffer, (Action<IVariableWidthHistogramAggregation, int?>) ((a, v) => a.InitialBuffer = v));

    public VariableWidthHistogramAggregationDescriptor<T> ShardSize(int? shardSize) => this.Assign<int?>(shardSize, (Action<IVariableWidthHistogramAggregation, int?>) ((a, v) => a.ShardSize = v));
  }
}
