// Decompiled with JetBrains decompiler
// Type: Nest.ReverseNestedAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class ReverseNestedAggregationDescriptor<T> : 
    BucketAggregationDescriptorBase<ReverseNestedAggregationDescriptor<T>, IReverseNestedAggregation, T>,
    IReverseNestedAggregation,
    IBucketAggregation,
    IAggregation
    where T : class
  {
    Field IReverseNestedAggregation.Path { get; set; }

    public ReverseNestedAggregationDescriptor<T> Path(Field path) => this.Assign<Field>(path, (Action<IReverseNestedAggregation, Field>) ((a, v) => a.Path = v));

    public ReverseNestedAggregationDescriptor<T> Path<TValue>(Expression<Func<T, TValue>> path) => this.Assign<Expression<Func<T, TValue>>>(path, (Action<IReverseNestedAggregation, Expression<Func<T, TValue>>>) ((a, v) => a.Path = (Field) (Expression) v));
  }
}
