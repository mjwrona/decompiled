// Decompiled with JetBrains decompiler
// Type: Nest.AutoDateHistogramAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class AutoDateHistogramAggregationDescriptor<T> : 
    BucketAggregationDescriptorBase<AutoDateHistogramAggregationDescriptor<T>, IAutoDateHistogramAggregation, T>,
    IAutoDateHistogramAggregation,
    IBucketAggregation,
    IAggregation
    where T : class
  {
    private string _format;

    Nest.Field IAutoDateHistogramAggregation.Field { get; set; }

    int? IAutoDateHistogramAggregation.Buckets { get; set; }

    string IAutoDateHistogramAggregation.Format
    {
      get => string.IsNullOrEmpty(this._format) || this._format.Contains("date_optional_time") || !this.Self.Missing.HasValue ? this._format : this._format + "||date_optional_time";
      set => this._format = value;
    }

    DateTime? IAutoDateHistogramAggregation.Missing { get; set; }

    string IAutoDateHistogramAggregation.Offset { get; set; }

    IDictionary<string, object> IAutoDateHistogramAggregation.Params { get; set; }

    IScript IAutoDateHistogramAggregation.Script { get; set; }

    string IAutoDateHistogramAggregation.TimeZone { get; set; }

    Nest.MinimumInterval? IAutoDateHistogramAggregation.MinimumInterval { get; set; }

    public AutoDateHistogramAggregationDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IAutoDateHistogramAggregation, Nest.Field>) ((a, v) => a.Field = v));

    public AutoDateHistogramAggregationDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<IAutoDateHistogramAggregation, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public AutoDateHistogramAggregationDescriptor<T> Buckets(int? buckets) => this.Assign<int?>(buckets, (Action<IAutoDateHistogramAggregation, int?>) ((a, v) => a.Buckets = v));

    public AutoDateHistogramAggregationDescriptor<T> Script(string script) => this.Assign<InlineScript>((InlineScript) script, (Action<IAutoDateHistogramAggregation, InlineScript>) ((a, v) => a.Script = (IScript) v));

    public AutoDateHistogramAggregationDescriptor<T> Script(
      Func<ScriptDescriptor, IScript> scriptSelector)
    {
      return this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<IAutoDateHistogramAggregation, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));
    }

    public AutoDateHistogramAggregationDescriptor<T> Format(string format) => this.Assign<string>(format, (Action<IAutoDateHistogramAggregation, string>) ((a, v) => a.Format = v));

    public AutoDateHistogramAggregationDescriptor<T> TimeZone(string timeZone) => this.Assign<string>(timeZone, (Action<IAutoDateHistogramAggregation, string>) ((a, v) => a.TimeZone = v));

    public AutoDateHistogramAggregationDescriptor<T> Offset(string offset) => this.Assign<string>(offset, (Action<IAutoDateHistogramAggregation, string>) ((a, v) => a.Offset = v));

    public AutoDateHistogramAggregationDescriptor<T> Missing(DateTime? missing) => this.Assign<DateTime?>(missing, (Action<IAutoDateHistogramAggregation, DateTime?>) ((a, v) => a.Missing = v));

    public AutoDateHistogramAggregationDescriptor<T> MinimumInterval(
      Nest.MinimumInterval? minimumInterval)
    {
      return this.Assign<Nest.MinimumInterval?>(minimumInterval, (Action<IAutoDateHistogramAggregation, Nest.MinimumInterval?>) ((a, v) => a.MinimumInterval = v));
    }
  }
}
