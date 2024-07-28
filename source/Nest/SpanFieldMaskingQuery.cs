// Decompiled with JetBrains decompiler
// Type: Nest.SpanFieldMaskingQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class SpanFieldMaskingQuery : QueryBase, ISpanFieldMaskingQuery, ISpanSubQuery, IQuery
  {
    public Field Field { get; set; }

    public ISpanQuery Query { get; set; }

    protected override bool Conditionless => SpanFieldMaskingQuery.IsConditionless((ISpanFieldMaskingQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.SpanFieldMasking = (ISpanFieldMaskingQuery) this;

    internal static bool IsConditionless(ISpanFieldMaskingQuery q) => q.Field.IsConditionless() || q.Query == null || q.Query.Conditionless;
  }
}
