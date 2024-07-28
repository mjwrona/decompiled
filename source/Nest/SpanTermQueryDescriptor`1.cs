// Decompiled with JetBrains decompiler
// Type: Nest.SpanTermQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SpanTermQueryDescriptor<T> : 
    FieldNameQueryDescriptorBase<SpanTermQueryDescriptor<T>, ISpanTermQuery, T>,
    ISpanTermQuery,
    ISpanSubQuery,
    IQuery,
    IFieldNameQuery
    where T : class
  {
    protected override bool Conditionless => SpanTermQuery.IsConditionless((ISpanTermQuery) this);

    object ISpanTermQuery.Value { get; set; }

    public SpanTermQueryDescriptor<T> Value(object value) => this.Assign<object>(value, (Action<ISpanTermQuery, object>) ((a, v) => a.Value = v));
  }
}
