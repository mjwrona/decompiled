// Decompiled with JetBrains decompiler
// Type: Nest.TermRangeQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class TermRangeQuery : 
    FieldNameQueryBase,
    ITermRangeQuery,
    IRangeQuery,
    IFieldNameQuery,
    IQuery
  {
    public string GreaterThan { get; set; }

    public string GreaterThanOrEqualTo { get; set; }

    public string LessThan { get; set; }

    public string LessThanOrEqualTo { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    public string From { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    public string To { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    public bool? IncludeLower { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    public bool? IncludeUpper { get; set; }

    protected override bool Conditionless => TermRangeQuery.IsConditionless((ITermRangeQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.Range = (IRangeQuery) this;

    internal static bool IsConditionless(ITermRangeQuery q)
    {
      if (q.Field.IsConditionless())
        return true;
      return q.GreaterThanOrEqualTo.IsNullOrEmpty() && q.LessThanOrEqualTo.IsNullOrEmpty() && q.GreaterThan.IsNullOrEmpty() && q.LessThan.IsNullOrEmpty() && q.From.IsNullOrEmpty() && q.To.IsNullOrEmpty();
    }
  }
}
