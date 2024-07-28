// Decompiled with JetBrains decompiler
// Type: Nest.SpanTermQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class SpanTermQuery : 
    FieldNameQueryBase,
    ISpanTermQuery,
    ISpanSubQuery,
    IQuery,
    IFieldNameQuery
  {
    public object Value { get; set; }

    protected override bool Conditionless => SpanTermQuery.IsConditionless((ISpanTermQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.SpanTerm = (ISpanTermQuery) this;

    internal static bool IsConditionless(ISpanTermQuery q) => q.Value == null || q.Value.ToString().IsNullOrEmpty() || q.Field.IsConditionless();
  }
}
