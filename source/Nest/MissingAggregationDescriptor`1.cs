// Decompiled with JetBrains decompiler
// Type: Nest.MissingAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class MissingAggregationDescriptor<T> : 
    BucketAggregationDescriptorBase<MissingAggregationDescriptor<T>, IMissingAggregation, T>,
    IMissingAggregation,
    IBucketAggregation,
    IAggregation
    where T : class
  {
    Nest.Field IMissingAggregation.Field { get; set; }

    public MissingAggregationDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IMissingAggregation, Nest.Field>) ((a, v) => a.Field = v));

    public MissingAggregationDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<IMissingAggregation, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));
  }
}
