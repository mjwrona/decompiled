// Decompiled with JetBrains decompiler
// Type: Nest.SpanGapQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SpanGapQuery : QueryBase, ISpanGapQuery, ISpanSubQuery, IQuery
  {
    public Field Field { get; set; }

    public int? Width { get; set; }

    protected override bool Conditionless => SpanGapQuery.IsConditionless((ISpanGapQuery) this);

    internal static bool IsConditionless(ISpanGapQuery q) => (q != null ? (!q.Width.HasValue ? 1 : 0) : 1) != 0 || q.Field.IsConditionless();

    internal override void InternalWrapInContainer(IQueryContainer c) => throw new Exception("span_gap may only appear as a span near clause");
  }
}
