// Decompiled with JetBrains decompiler
// Type: Nest.DateHistogramAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class DateHistogramAggregationDescriptor<T> : 
    BucketAggregationDescriptorBase<DateHistogramAggregationDescriptor<T>, IDateHistogramAggregation, T>,
    IDateHistogramAggregation,
    IBucketAggregation,
    IAggregation
    where T : class
  {
    private string _format;

    Nest.ExtendedBounds<DateMath> IDateHistogramAggregation.ExtendedBounds { get; set; }

    Nest.HardBounds<DateMath> IDateHistogramAggregation.HardBounds { get; set; }

    Nest.Field IDateHistogramAggregation.Field { get; set; }

    string IDateHistogramAggregation.Format
    {
      get => string.IsNullOrEmpty(this._format) || this._format.Contains("date_optional_time") || this.Self.ExtendedBounds == null && this.Self.HardBounds == null && !this.Self.Missing.HasValue ? this._format : this._format + "||date_optional_time";
      set => this._format = value;
    }

    [Obsolete("Deprecated in version 7.2.0, use CalendarInterval or FixedInterval instead")]
    Union<DateInterval, Time> IDateHistogramAggregation.Interval { get; set; }

    Union<DateInterval?, DateMathTime> IDateHistogramAggregation.CalendarInterval { get; set; }

    Time IDateHistogramAggregation.FixedInterval { get; set; }

    int? IDateHistogramAggregation.MinimumDocumentCount { get; set; }

    DateTime? IDateHistogramAggregation.Missing { get; set; }

    string IDateHistogramAggregation.Offset { get; set; }

    HistogramOrder IDateHistogramAggregation.Order { get; set; }

    IScript IDateHistogramAggregation.Script { get; set; }

    string IDateHistogramAggregation.TimeZone { get; set; }

    public DateHistogramAggregationDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IDateHistogramAggregation, Nest.Field>) ((a, v) => a.Field = v));

    public DateHistogramAggregationDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<IDateHistogramAggregation, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public DateHistogramAggregationDescriptor<T> Script(string script) => this.Assign<InlineScript>((InlineScript) script, (Action<IDateHistogramAggregation, InlineScript>) ((a, v) => a.Script = (IScript) v));

    public DateHistogramAggregationDescriptor<T> Script(
      Func<ScriptDescriptor, IScript> scriptSelector)
    {
      return this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<IDateHistogramAggregation, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));
    }

    [Obsolete("Deprecated in version 7.2.0, use CalendarInterval or FixedInterval instead")]
    public DateHistogramAggregationDescriptor<T> Interval(Time interval) => this.Assign<Time>(interval, (Action<IDateHistogramAggregation, Time>) ((a, v) => a.Interval = (Union<DateInterval, Time>) v));

    [Obsolete("Deprecated in version 7.2.0, use CalendarInterval or FixedInterval instead")]
    public DateHistogramAggregationDescriptor<T> Interval(DateInterval interval) => this.Assign<DateInterval>(interval, (Action<IDateHistogramAggregation, DateInterval>) ((a, v) => a.Interval = (Union<DateInterval, Time>) v));

    public DateHistogramAggregationDescriptor<T> CalendarInterval(DateMathTime interval) => this.Assign<DateMathTime>(interval, (Action<IDateHistogramAggregation, DateMathTime>) ((a, v) => a.CalendarInterval = (Union<DateInterval?, DateMathTime>) v));

    public DateHistogramAggregationDescriptor<T> CalendarInterval(DateInterval? interval) => this.Assign<DateInterval?>(interval, (Action<IDateHistogramAggregation, DateInterval?>) ((a, v) => a.CalendarInterval = (Union<DateInterval?, DateMathTime>) v));

    public DateHistogramAggregationDescriptor<T> FixedInterval(Time interval) => this.Assign<Time>(interval, (Action<IDateHistogramAggregation, Time>) ((a, v) => a.FixedInterval = v));

    public DateHistogramAggregationDescriptor<T> Format(string format) => this.Assign<string>(format, (Action<IDateHistogramAggregation, string>) ((a, v) => a.Format = v));

    public DateHistogramAggregationDescriptor<T> MinimumDocumentCount(int? minimumDocumentCount) => this.Assign<int?>(minimumDocumentCount, (Action<IDateHistogramAggregation, int?>) ((a, v) => a.MinimumDocumentCount = v));

    public DateHistogramAggregationDescriptor<T> TimeZone(string timeZone) => this.Assign<string>(timeZone, (Action<IDateHistogramAggregation, string>) ((a, v) => a.TimeZone = v));

    public DateHistogramAggregationDescriptor<T> Offset(string offset) => this.Assign<string>(offset, (Action<IDateHistogramAggregation, string>) ((a, v) => a.Offset = v));

    public DateHistogramAggregationDescriptor<T> Order(HistogramOrder order) => this.Assign<HistogramOrder>(order, (Action<IDateHistogramAggregation, HistogramOrder>) ((a, v) => a.Order = v));

    public DateHistogramAggregationDescriptor<T> OrderAscending(string key) => this.Assign<HistogramOrder>(new HistogramOrder()
    {
      Key = key,
      Order = SortOrder.Descending
    }, (Action<IDateHistogramAggregation, HistogramOrder>) ((a, v) => a.Order = v));

    public DateHistogramAggregationDescriptor<T> OrderDescending(string key) => this.Assign<HistogramOrder>(new HistogramOrder()
    {
      Key = key,
      Order = SortOrder.Descending
    }, (Action<IDateHistogramAggregation, HistogramOrder>) ((a, v) => a.Order = v));

    public DateHistogramAggregationDescriptor<T> ExtendedBounds(DateMath min, DateMath max) => this.Assign<Nest.ExtendedBounds<DateMath>>(new Nest.ExtendedBounds<DateMath>()
    {
      Minimum = min,
      Maximum = max
    }, (Action<IDateHistogramAggregation, Nest.ExtendedBounds<DateMath>>) ((a, v) => a.ExtendedBounds = v));

    public DateHistogramAggregationDescriptor<T> HardBounds(DateMath min, DateMath max) => this.Assign<Nest.HardBounds<DateMath>>(new Nest.HardBounds<DateMath>()
    {
      Minimum = min,
      Maximum = max
    }, (Action<IDateHistogramAggregation, Nest.HardBounds<DateMath>>) ((a, v) => a.HardBounds = v));

    public DateHistogramAggregationDescriptor<T> Missing(DateTime? missing) => this.Assign<DateTime?>(missing, (Action<IDateHistogramAggregation, DateTime?>) ((a, v) => a.Missing = v));
  }
}
