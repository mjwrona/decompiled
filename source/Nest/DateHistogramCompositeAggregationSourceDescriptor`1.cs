// Decompiled with JetBrains decompiler
// Type: Nest.DateHistogramCompositeAggregationSourceDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class DateHistogramCompositeAggregationSourceDescriptor<T> : 
    CompositeAggregationSourceDescriptorBase<DateHistogramCompositeAggregationSourceDescriptor<T>, IDateHistogramCompositeAggregationSource, T>,
    IDateHistogramCompositeAggregationSource,
    ICompositeAggregationSource
  {
    public DateHistogramCompositeAggregationSourceDescriptor(string name)
      : base(name, "date_histogram")
    {
    }

    string IDateHistogramCompositeAggregationSource.Format { get; set; }

    Union<DateInterval?, Time> IDateHistogramCompositeAggregationSource.Interval { get; set; }

    Union<DateInterval?, DateMathTime> IDateHistogramCompositeAggregationSource.CalendarInterval { get; set; }

    Time IDateHistogramCompositeAggregationSource.FixedInterval { get; set; }

    string IDateHistogramCompositeAggregationSource.TimeZone { get; set; }

    [Obsolete("Use FixedInterval or CalendarInterval")]
    public DateHistogramCompositeAggregationSourceDescriptor<T> Interval(DateInterval? interval) => this.Assign<DateInterval?>(interval, (Action<IDateHistogramCompositeAggregationSource, DateInterval?>) ((a, v) => a.Interval = (Union<DateInterval?, Time>) v));

    [Obsolete("Use FixedInterval or CalendarInterval")]
    public DateHistogramCompositeAggregationSourceDescriptor<T> Interval(Time interval) => this.Assign<Time>(interval, (Action<IDateHistogramCompositeAggregationSource, Time>) ((a, v) => a.Interval = (Union<DateInterval?, Time>) v));

    public DateHistogramCompositeAggregationSourceDescriptor<T> CalendarInterval(
      DateInterval? interval)
    {
      return this.Assign<DateInterval?>(interval, (Action<IDateHistogramCompositeAggregationSource, DateInterval?>) ((a, v) => a.CalendarInterval = (Union<DateInterval?, DateMathTime>) v));
    }

    public DateHistogramCompositeAggregationSourceDescriptor<T> CalendarInterval(
      DateMathTime interval)
    {
      return this.Assign<DateMathTime>(interval, (Action<IDateHistogramCompositeAggregationSource, DateMathTime>) ((a, v) => a.CalendarInterval = (Union<DateInterval?, DateMathTime>) v));
    }

    public DateHistogramCompositeAggregationSourceDescriptor<T> FixedInterval(Time interval) => this.Assign<Time>(interval, (Action<IDateHistogramCompositeAggregationSource, Time>) ((a, v) => a.FixedInterval = v));

    public DateHistogramCompositeAggregationSourceDescriptor<T> TimeZone(string timezone) => this.Assign<string>(timezone, (Action<IDateHistogramCompositeAggregationSource, string>) ((a, v) => a.TimeZone = v));

    public DateHistogramCompositeAggregationSourceDescriptor<T> Format(string format) => this.Assign<string>(format, (Action<IDateHistogramCompositeAggregationSource, string>) ((a, v) => a.Format = v));
  }
}
