// Decompiled with JetBrains decompiler
// Type: Nest.SpanFieldMaskingQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class SpanFieldMaskingQueryDescriptor<T> : 
    QueryDescriptorBase<SpanFieldMaskingQueryDescriptor<T>, ISpanFieldMaskingQuery>,
    ISpanFieldMaskingQuery,
    ISpanSubQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => SpanFieldMaskingQuery.IsConditionless((ISpanFieldMaskingQuery) this);

    Nest.Field ISpanFieldMaskingQuery.Field { get; set; }

    ISpanQuery ISpanFieldMaskingQuery.Query { get; set; }

    public SpanFieldMaskingQueryDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<ISpanFieldMaskingQuery, Nest.Field>) ((a, v) => a.Field = v));

    public SpanFieldMaskingQueryDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<ISpanFieldMaskingQuery, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public SpanFieldMaskingQueryDescriptor<T> Query(
      Func<SpanQueryDescriptor<T>, ISpanQuery> selector)
    {
      return this.Assign<Func<SpanQueryDescriptor<T>, ISpanQuery>>(selector, (Action<ISpanFieldMaskingQuery, Func<SpanQueryDescriptor<T>, ISpanQuery>>) ((a, v) => a.Query = v != null ? v(new SpanQueryDescriptor<T>()) : (ISpanQuery) null));
    }
  }
}
