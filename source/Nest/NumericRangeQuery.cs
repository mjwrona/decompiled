// Decompiled with JetBrains decompiler
// Type: Nest.NumericRangeQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class NumericRangeQuery : 
    FieldNameQueryBase,
    INumericRangeQuery,
    IRangeQuery,
    IFieldNameQuery,
    IQuery
  {
    public double? GreaterThan { get; set; }

    public double? GreaterThanOrEqualTo { get; set; }

    public double? LessThan { get; set; }

    public double? LessThanOrEqualTo { get; set; }

    public RangeRelation? Relation { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    public double? From { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    public double? To { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    public bool? IncludeLower { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    public bool? IncludeUpper { get; set; }

    protected override bool Conditionless => NumericRangeQuery.IsConditionless((INumericRangeQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.Range = (IRangeQuery) this;

    internal static bool IsConditionless(INumericRangeQuery q)
    {
      if (q.Field.IsConditionless())
        return true;
      double? nullable = q.GreaterThanOrEqualTo;
      if (!nullable.HasValue)
      {
        nullable = q.LessThanOrEqualTo;
        if (!nullable.HasValue)
        {
          nullable = q.GreaterThan;
          if (!nullable.HasValue)
          {
            nullable = q.LessThan;
            if (!nullable.HasValue)
            {
              nullable = q.From;
              if (!nullable.HasValue)
              {
                nullable = q.To;
                return !nullable.HasValue;
              }
            }
          }
        }
      }
      return false;
    }
  }
}
