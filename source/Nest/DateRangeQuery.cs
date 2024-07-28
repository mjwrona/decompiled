// Decompiled with JetBrains decompiler
// Type: Nest.DateRangeQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class DateRangeQuery : 
    FieldNameQueryBase,
    IDateRangeQuery,
    IRangeQuery,
    IFieldNameQuery,
    IQuery
  {
    public string Format { get; set; }

    public DateMath GreaterThan { get; set; }

    public DateMath GreaterThanOrEqualTo { get; set; }

    public DateMath LessThan { get; set; }

    public DateMath LessThanOrEqualTo { get; set; }

    public RangeRelation? Relation { get; set; }

    public string TimeZone { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    public DateMath From { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    public DateMath To { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    public bool? IncludeLower { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    public bool? IncludeUpper { get; set; }

    protected override bool Conditionless => DateRangeQuery.IsConditionless((IDateRangeQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.Range = (IRangeQuery) this;

    internal static bool IsConditionless(IDateRangeQuery q)
    {
      if (q.Field.IsConditionless())
        return true;
      if (q.GreaterThanOrEqualTo != null && q.GreaterThanOrEqualTo.IsValid || q.LessThanOrEqualTo != null && q.LessThanOrEqualTo.IsValid || q.GreaterThan != null && q.GreaterThan.IsValid || q.LessThan != null && q.LessThan.IsValid || q.From != null && q.From.IsValid)
        return false;
      return q.To == null || !q.To.IsValid;
    }
  }
}
