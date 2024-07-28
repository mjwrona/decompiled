// Decompiled with JetBrains decompiler
// Type: Nest.DateRangeAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Nest
{
  public class DateRangeAggregationDescriptor<T> : 
    BucketAggregationDescriptorBase<DateRangeAggregationDescriptor<T>, IDateRangeAggregation, T>,
    IDateRangeAggregation,
    IBucketAggregation,
    IAggregation
    where T : class
  {
    Nest.Field IDateRangeAggregation.Field { get; set; }

    string IDateRangeAggregation.Format { get; set; }

    object IDateRangeAggregation.Missing { get; set; }

    IEnumerable<IDateRangeExpression> IDateRangeAggregation.Ranges { get; set; }

    string IDateRangeAggregation.TimeZone { get; set; }

    public DateRangeAggregationDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IDateRangeAggregation, Nest.Field>) ((a, v) => a.Field = v));

    public DateRangeAggregationDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<IDateRangeAggregation, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public DateRangeAggregationDescriptor<T> Format(string format) => this.Assign<string>(format, (Action<IDateRangeAggregation, string>) ((a, v) => a.Format = v));

    public DateRangeAggregationDescriptor<T> Missing(object missing) => this.Assign<object>(missing, (Action<IDateRangeAggregation, object>) ((a, v) => a.Missing = v));

    public DateRangeAggregationDescriptor<T> Ranges(params IDateRangeExpression[] ranges) => this.Assign<List<IDateRangeExpression>>(((IEnumerable<IDateRangeExpression>) ranges).ToListOrNullIfEmpty<IDateRangeExpression>(), (Action<IDateRangeAggregation, List<IDateRangeExpression>>) ((a, v) => a.Ranges = (IEnumerable<IDateRangeExpression>) v));

    public DateRangeAggregationDescriptor<T> TimeZone(string timeZone) => this.Assign<string>(timeZone, (Action<IDateRangeAggregation, string>) ((a, v) => a.TimeZone = v));

    public DateRangeAggregationDescriptor<T> Ranges(
      params Func<DateRangeExpressionDescriptor, IDateRangeExpression>[] ranges)
    {
      return this.Assign<List<IDateRangeExpression>>(ranges != null ? ((IEnumerable<Func<DateRangeExpressionDescriptor, IDateRangeExpression>>) ranges).Select<Func<DateRangeExpressionDescriptor, IDateRangeExpression>, IDateRangeExpression>((Func<Func<DateRangeExpressionDescriptor, IDateRangeExpression>, IDateRangeExpression>) (r => r(new DateRangeExpressionDescriptor()))).ToListOrNullIfEmpty<IDateRangeExpression>() : (List<IDateRangeExpression>) null, (Action<IDateRangeAggregation, List<IDateRangeExpression>>) ((a, v) => a.Ranges = (IEnumerable<IDateRangeExpression>) v));
    }

    public DateRangeAggregationDescriptor<T> Ranges(
      IEnumerable<Func<DateRangeExpressionDescriptor, IDateRangeExpression>> ranges)
    {
      return this.Assign<List<IDateRangeExpression>>(ranges != null ? ranges.Select<Func<DateRangeExpressionDescriptor, IDateRangeExpression>, IDateRangeExpression>((Func<Func<DateRangeExpressionDescriptor, IDateRangeExpression>, IDateRangeExpression>) (r => r(new DateRangeExpressionDescriptor()))).ToListOrNullIfEmpty<IDateRangeExpression>() : (List<IDateRangeExpression>) null, (Action<IDateRangeAggregation, List<IDateRangeExpression>>) ((a, v) => a.Ranges = (IEnumerable<IDateRangeExpression>) v));
    }
  }
}
