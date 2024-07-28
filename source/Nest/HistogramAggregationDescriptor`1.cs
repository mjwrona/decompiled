// Decompiled with JetBrains decompiler
// Type: Nest.HistogramAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class HistogramAggregationDescriptor<T> : 
    BucketAggregationDescriptorBase<HistogramAggregationDescriptor<T>, IHistogramAggregation, T>,
    IHistogramAggregation,
    IBucketAggregation,
    IAggregation
    where T : class
  {
    Nest.ExtendedBounds<double> IHistogramAggregation.ExtendedBounds { get; set; }

    Nest.HardBounds<double> IHistogramAggregation.HardBounds { get; set; }

    Nest.Field IHistogramAggregation.Field { get; set; }

    double? IHistogramAggregation.Interval { get; set; }

    int? IHistogramAggregation.MinimumDocumentCount { get; set; }

    double? IHistogramAggregation.Missing { get; set; }

    double? IHistogramAggregation.Offset { get; set; }

    HistogramOrder IHistogramAggregation.Order { get; set; }

    IScript IHistogramAggregation.Script { get; set; }

    public HistogramAggregationDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IHistogramAggregation, Nest.Field>) ((a, v) => a.Field = v));

    public HistogramAggregationDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<IHistogramAggregation, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public HistogramAggregationDescriptor<T> Script(string script) => this.Assign<InlineScript>((InlineScript) script, (Action<IHistogramAggregation, InlineScript>) ((a, v) => a.Script = (IScript) v));

    public HistogramAggregationDescriptor<T> Script(Func<ScriptDescriptor, IScript> scriptSelector) => this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<IHistogramAggregation, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));

    public HistogramAggregationDescriptor<T> Interval(double? interval) => this.Assign<double?>(interval, (Action<IHistogramAggregation, double?>) ((a, v) => a.Interval = v));

    public HistogramAggregationDescriptor<T> MinimumDocumentCount(int? minimumDocumentCount) => this.Assign<int?>(minimumDocumentCount, (Action<IHistogramAggregation, int?>) ((a, v) => a.MinimumDocumentCount = v));

    public HistogramAggregationDescriptor<T> Order(HistogramOrder order) => this.Assign<HistogramOrder>(order, (Action<IHistogramAggregation, HistogramOrder>) ((a, v) => a.Order = v));

    public HistogramAggregationDescriptor<T> OrderAscending(string key) => this.Assign<HistogramOrder>(new HistogramOrder()
    {
      Key = key,
      Order = SortOrder.Descending
    }, (Action<IHistogramAggregation, HistogramOrder>) ((a, v) => a.Order = v));

    public HistogramAggregationDescriptor<T> OrderDescending(string key) => this.Assign<HistogramOrder>(new HistogramOrder()
    {
      Key = key,
      Order = SortOrder.Descending
    }, (Action<IHistogramAggregation, HistogramOrder>) ((a, v) => a.Order = v));

    public HistogramAggregationDescriptor<T> ExtendedBounds(double min, double max) => this.Assign<Nest.ExtendedBounds<double>>(new Nest.ExtendedBounds<double>()
    {
      Minimum = min,
      Maximum = max
    }, (Action<IHistogramAggregation, Nest.ExtendedBounds<double>>) ((a, v) => a.ExtendedBounds = v));

    public HistogramAggregationDescriptor<T> HardBounds(double min, double max) => this.Assign<Nest.HardBounds<double>>(new Nest.HardBounds<double>()
    {
      Minimum = min,
      Maximum = max
    }, (Action<IHistogramAggregation, Nest.HardBounds<double>>) ((a, v) => a.HardBounds = v));

    public HistogramAggregationDescriptor<T> Offset(double? offset) => this.Assign<double?>(offset, (Action<IHistogramAggregation, double?>) ((a, v) => a.Offset = v));

    public HistogramAggregationDescriptor<T> Missing(double? missing) => this.Assign<double?>(missing, (Action<IHistogramAggregation, double?>) ((a, v) => a.Missing = v));
  }
}
