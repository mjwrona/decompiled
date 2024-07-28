// Decompiled with JetBrains decompiler
// Type: Nest.NestedAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class NestedAggregationDescriptor<T> : 
    BucketAggregationDescriptorBase<NestedAggregationDescriptor<T>, INestedAggregation, T>,
    INestedAggregation,
    IBucketAggregation,
    IAggregation
    where T : class
  {
    Field INestedAggregation.Path { get; set; }

    public NestedAggregationDescriptor<T> Path(Field path) => this.Assign<Field>(path, (Action<INestedAggregation, Field>) ((a, v) => a.Path = v));

    public NestedAggregationDescriptor<T> Path<TValue>(Expression<Func<T, TValue>> path) => this.Assign<Expression<Func<T, TValue>>>(path, (Action<INestedAggregation, Expression<Func<T, TValue>>>) ((a, v) => a.Path = (Field) (Expression) v));
  }
}
