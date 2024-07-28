// Decompiled with JetBrains decompiler
// Type: Nest.DateRangeQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class DateRangeQueryDescriptor<T> : 
    FieldNameQueryDescriptorBase<DateRangeQueryDescriptor<T>, IDateRangeQuery, T>,
    IDateRangeQuery,
    IRangeQuery,
    IFieldNameQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => DateRangeQuery.IsConditionless((IDateRangeQuery) this);

    string IDateRangeQuery.Format { get; set; }

    DateMath IDateRangeQuery.GreaterThan { get; set; }

    DateMath IDateRangeQuery.GreaterThanOrEqualTo { get; set; }

    DateMath IDateRangeQuery.LessThan { get; set; }

    DateMath IDateRangeQuery.LessThanOrEqualTo { get; set; }

    RangeRelation? IDateRangeQuery.Relation { get; set; }

    string IDateRangeQuery.TimeZone { get; set; }

    DateMath IDateRangeQuery.From { get; set; }

    DateMath IDateRangeQuery.To { get; set; }

    bool? IDateRangeQuery.IncludeLower { get; set; }

    bool? IDateRangeQuery.IncludeUpper { get; set; }

    public DateRangeQueryDescriptor<T> GreaterThan(DateMath from) => this.Assign<DateMath>(from, (Action<IDateRangeQuery, DateMath>) ((a, v) => a.GreaterThan = v));

    public DateRangeQueryDescriptor<T> GreaterThanOrEquals(DateMath from) => this.Assign<DateMath>(from, (Action<IDateRangeQuery, DateMath>) ((a, v) => a.GreaterThanOrEqualTo = v));

    public DateRangeQueryDescriptor<T> LessThan(DateMath to) => this.Assign<DateMath>(to, (Action<IDateRangeQuery, DateMath>) ((a, v) => a.LessThan = v));

    public DateRangeQueryDescriptor<T> LessThanOrEquals(DateMath to) => this.Assign<DateMath>(to, (Action<IDateRangeQuery, DateMath>) ((a, v) => a.LessThanOrEqualTo = v));

    public DateRangeQueryDescriptor<T> TimeZone(string timeZone) => this.Assign<string>(timeZone, (Action<IDateRangeQuery, string>) ((a, v) => a.TimeZone = v));

    public DateRangeQueryDescriptor<T> Format(string format) => this.Assign<string>(format, (Action<IDateRangeQuery, string>) ((a, v) => a.Format = v));

    public DateRangeQueryDescriptor<T> Relation(RangeRelation? relation) => this.Assign<RangeRelation?>(relation, (Action<IDateRangeQuery, RangeRelation?>) ((a, v) => a.Relation = v));
  }
}
