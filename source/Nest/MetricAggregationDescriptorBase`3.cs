// Decompiled with JetBrains decompiler
// Type: Nest.MetricAggregationDescriptorBase`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public abstract class MetricAggregationDescriptorBase<TMetricAggregation, TMetricAggregationInterface, T> : 
    DescriptorBase<TMetricAggregation, TMetricAggregationInterface>,
    IMetricAggregation,
    IAggregation
    where TMetricAggregation : MetricAggregationDescriptorBase<TMetricAggregation, TMetricAggregationInterface, T>, TMetricAggregationInterface, IMetricAggregation
    where TMetricAggregationInterface : class, IMetricAggregation
    where T : class
  {
    Nest.Field IMetricAggregation.Field { get; set; }

    IDictionary<string, object> IAggregation.Meta { get; set; }

    double? IMetricAggregation.Missing { get; set; }

    string IAggregation.Name { get; set; }

    IScript IMetricAggregation.Script { get; set; }

    public TMetricAggregation Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<TMetricAggregationInterface, Nest.Field>) ((a, v) => a.Field = v));

    public TMetricAggregation Field<TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<TMetricAggregationInterface, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public virtual TMetricAggregation Script(string script) => this.Assign<InlineScript>((InlineScript) script, (Action<TMetricAggregationInterface, InlineScript>) ((a, v) => a.Script = (IScript) v));

    public virtual TMetricAggregation Script(Func<ScriptDescriptor, IScript> scriptSelector) => this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<TMetricAggregationInterface, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));

    public TMetricAggregation Missing(double? missing) => this.Assign<double?>(missing, (Action<TMetricAggregationInterface, double?>) ((a, v) => a.Missing = v));

    public TMetricAggregation Meta(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> selector)
    {
      return this.Assign<Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>(selector, (Action<TMetricAggregationInterface, Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>) ((a, v) => a.Meta = v != null ? (IDictionary<string, object>) v(new FluentDictionary<string, object>()) : (IDictionary<string, object>) null));
    }
  }
}
