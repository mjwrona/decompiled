// Decompiled with JetBrains decompiler
// Type: Nest.LongRangeQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class LongRangeQuery : 
    FieldNameQueryBase,
    ILongRangeQuery,
    IRangeQuery,
    IFieldNameQuery,
    IQuery
  {
    public long? GreaterThan { get; set; }

    public long? GreaterThanOrEqualTo { get; set; }

    public long? LessThan { get; set; }

    public long? LessThanOrEqualTo { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    public long? From { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    public long? To { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    public bool? IncludeLower { get; set; }

    [Obsolete("This property is considered deprecated and will be removed in the next major release. Range queries should prefer the gt, lt, gte and lte properties instead.")]
    public bool? IncludeUpper { get; set; }

    public RangeRelation? Relation { get; set; }

    protected override bool Conditionless => LongRangeQuery.IsConditionless((ILongRangeQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.Range = (IRangeQuery) this;

    internal static bool IsConditionless(ILongRangeQuery q)
    {
      if (q.Field.IsConditionless())
        return true;
      long? nullable = q.GreaterThanOrEqualTo;
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
