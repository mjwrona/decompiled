// Decompiled with JetBrains decompiler
// Type: Nest.LongRangeQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class LongRangeQueryDescriptor<T> : 
    FieldNameQueryDescriptorBase<LongRangeQueryDescriptor<T>, ILongRangeQuery, T>,
    ILongRangeQuery,
    IRangeQuery,
    IFieldNameQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => LongRangeQuery.IsConditionless((ILongRangeQuery) this);

    long? ILongRangeQuery.GreaterThan { get; set; }

    long? ILongRangeQuery.GreaterThanOrEqualTo { get; set; }

    long? ILongRangeQuery.LessThan { get; set; }

    long? ILongRangeQuery.LessThanOrEqualTo { get; set; }

    RangeRelation? ILongRangeQuery.Relation { get; set; }

    long? ILongRangeQuery.From { get; set; }

    long? ILongRangeQuery.To { get; set; }

    bool? ILongRangeQuery.IncludeLower { get; set; }

    bool? ILongRangeQuery.IncludeUpper { get; set; }

    public LongRangeQueryDescriptor<T> Relation(RangeRelation? relation) => this.Assign<RangeRelation?>(relation, (Action<ILongRangeQuery, RangeRelation?>) ((a, v) => a.Relation = v));

    public LongRangeQueryDescriptor<T> GreaterThan(long? from) => this.Assign<long?>(from, (Action<ILongRangeQuery, long?>) ((a, v) => a.GreaterThan = v));

    public LongRangeQueryDescriptor<T> GreaterThanOrEquals(long? from) => this.Assign<long?>(from, (Action<ILongRangeQuery, long?>) ((a, v) => a.GreaterThanOrEqualTo = v));

    public LongRangeQueryDescriptor<T> LessThan(long? to) => this.Assign<long?>(to, (Action<ILongRangeQuery, long?>) ((a, v) => a.LessThan = v));

    public LongRangeQueryDescriptor<T> LessThanOrEquals(long? to) => this.Assign<long?>(to, (Action<ILongRangeQuery, long?>) ((a, v) => a.LessThanOrEqualTo = v));
  }
}
