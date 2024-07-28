// Decompiled with JetBrains decompiler
// Type: Nest.DateHistogramRollupGroupingDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class DateHistogramRollupGroupingDescriptor<T> : 
    DescriptorBase<DateHistogramRollupGroupingDescriptor<T>, IDateHistogramRollupGrouping>,
    IDateHistogramRollupGrouping
    where T : class
  {
    Time IDateHistogramRollupGrouping.Delay { get; set; }

    Nest.Field IDateHistogramRollupGrouping.Field { get; set; }

    string IDateHistogramRollupGrouping.Format { get; set; }

    Time IDateHistogramRollupGrouping.Interval { get; set; }

    string IDateHistogramRollupGrouping.TimeZone { get; set; }

    public DateHistogramRollupGroupingDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IDateHistogramRollupGrouping, Nest.Field>) ((a, v) => a.Field = v));

    public DateHistogramRollupGroupingDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<IDateHistogramRollupGrouping, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public DateHistogramRollupGroupingDescriptor<T> Interval(Time interval) => this.Assign<Time>(interval, (Action<IDateHistogramRollupGrouping, Time>) ((a, v) => a.Interval = v));

    public DateHistogramRollupGroupingDescriptor<T> Delay(Time delay) => this.Assign<Time>(delay, (Action<IDateHistogramRollupGrouping, Time>) ((a, v) => a.Delay = v));

    public DateHistogramRollupGroupingDescriptor<T> TimeZone(string timeZone) => this.Assign<string>(timeZone, (Action<IDateHistogramRollupGrouping, string>) ((a, v) => a.TimeZone = v));

    public DateHistogramRollupGroupingDescriptor<T> Format(string format) => this.Assign<string>(format, (Action<IDateHistogramRollupGrouping, string>) ((a, v) => a.Format = v));
  }
}
