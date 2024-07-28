// Decompiled with JetBrains decompiler
// Type: Nest.SpanGapQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class SpanGapQueryDescriptor<T> : 
    QueryDescriptorBase<SpanGapQueryDescriptor<T>, ISpanGapQuery>,
    ISpanGapQuery,
    ISpanSubQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => SpanGapQuery.IsConditionless((ISpanGapQuery) this);

    Nest.Field ISpanGapQuery.Field { get; set; }

    int? ISpanGapQuery.Width { get; set; }

    public SpanGapQueryDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<ISpanGapQuery, Nest.Field>) ((a, v) => a.Field = v));

    public SpanGapQueryDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<ISpanGapQuery, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public SpanGapQueryDescriptor<T> Width(int? width) => this.Assign<int?>(width, (Action<ISpanGapQuery, int?>) ((a, v) => a.Width = v));
  }
}
