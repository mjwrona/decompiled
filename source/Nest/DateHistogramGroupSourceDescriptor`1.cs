// Decompiled with JetBrains decompiler
// Type: Nest.DateHistogramGroupSourceDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class DateHistogramGroupSourceDescriptor<T> : 
    SingleGroupSourceDescriptorBase<DateHistogramGroupSourceDescriptor<T>, IDateHistogramGroupSource, T>,
    IDateHistogramGroupSource,
    ISingleGroupSource
  {
    string IDateHistogramGroupSource.Format { get; set; }

    string IDateHistogramGroupSource.TimeZone { get; set; }

    Union<DateInterval, DateMathTime> IDateHistogramGroupSource.CalendarInterval { get; set; }

    Union<DateInterval, Time> IDateHistogramGroupSource.FixedInterval { get; set; }

    public DateHistogramGroupSourceDescriptor<T> Format(string format) => this.Assign<string>(format, (Action<IDateHistogramGroupSource, string>) ((a, v) => a.Format = v));

    public DateHistogramGroupSourceDescriptor<T> TimeZone(string timeZone) => this.Assign<string>(timeZone, (Action<IDateHistogramGroupSource, string>) ((a, v) => a.TimeZone = v));

    public DateHistogramGroupSourceDescriptor<T> CalendarInterval(DateInterval dateInterval) => this.Assign<DateInterval>(dateInterval, (Action<IDateHistogramGroupSource, DateInterval>) ((a, v) => a.CalendarInterval = (Union<DateInterval, DateMathTime>) v));

    public DateHistogramGroupSourceDescriptor<T> CalendarInterval(DateMathTime time) => this.Assign<DateMathTime>(time, (Action<IDateHistogramGroupSource, DateMathTime>) ((a, v) => a.CalendarInterval = (Union<DateInterval, DateMathTime>) v));

    public DateHistogramGroupSourceDescriptor<T> FixedInterval(DateInterval dateInterval) => this.Assign<DateInterval>(dateInterval, (Action<IDateHistogramGroupSource, DateInterval>) ((a, v) => a.FixedInterval = (Union<DateInterval, Time>) v));

    public DateHistogramGroupSourceDescriptor<T> FixedInterval(Time time) => this.Assign<Time>(time, (Action<IDateHistogramGroupSource, Time>) ((a, v) => a.FixedInterval = (Union<DateInterval, Time>) v));
  }
}
