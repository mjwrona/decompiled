// Decompiled with JetBrains decompiler
// Type: Nest.NumericRangeQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class NumericRangeQueryDescriptor<T> : 
    FieldNameQueryDescriptorBase<NumericRangeQueryDescriptor<T>, INumericRangeQuery, T>,
    INumericRangeQuery,
    IRangeQuery,
    IFieldNameQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => NumericRangeQuery.IsConditionless((INumericRangeQuery) this);

    double? INumericRangeQuery.GreaterThan { get; set; }

    double? INumericRangeQuery.GreaterThanOrEqualTo { get; set; }

    double? INumericRangeQuery.LessThan { get; set; }

    double? INumericRangeQuery.LessThanOrEqualTo { get; set; }

    RangeRelation? INumericRangeQuery.Relation { get; set; }

    double? INumericRangeQuery.From { get; set; }

    double? INumericRangeQuery.To { get; set; }

    bool? INumericRangeQuery.IncludeLower { get; set; }

    bool? INumericRangeQuery.IncludeUpper { get; set; }

    public NumericRangeQueryDescriptor<T> GreaterThan(double? from) => this.Assign<double?>(from, (Action<INumericRangeQuery, double?>) ((a, v) => a.GreaterThan = v));

    public NumericRangeQueryDescriptor<T> GreaterThanOrEquals(double? from) => this.Assign<double?>(from, (Action<INumericRangeQuery, double?>) ((a, v) => a.GreaterThanOrEqualTo = v));

    public NumericRangeQueryDescriptor<T> LessThan(double? to) => this.Assign<double?>(to, (Action<INumericRangeQuery, double?>) ((a, v) => a.LessThan = v));

    public NumericRangeQueryDescriptor<T> LessThanOrEquals(double? to) => this.Assign<double?>(to, (Action<INumericRangeQuery, double?>) ((a, v) => a.LessThanOrEqualTo = v));

    public NumericRangeQueryDescriptor<T> Relation(RangeRelation? relation) => this.Assign<RangeRelation?>(relation, (Action<INumericRangeQuery, RangeRelation?>) ((a, v) => a.Relation = v));
  }
}
