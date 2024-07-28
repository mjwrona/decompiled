// Decompiled with JetBrains decompiler
// Type: Nest.StringStatsAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class StringStatsAggregationDescriptor<T> : 
    DescriptorBase<StringStatsAggregationDescriptor<T>, IStringStatsAggregation>,
    IStringStatsAggregation,
    IAggregation
    where T : class
  {
    Nest.Field IStringStatsAggregation.Field { get; set; }

    IDictionary<string, object> IAggregation.Meta { get; set; }

    object IStringStatsAggregation.Missing { get; set; }

    string IAggregation.Name { get; set; }

    IScript IStringStatsAggregation.Script { get; set; }

    bool? IStringStatsAggregation.ShowDistribution { get; set; }

    public StringStatsAggregationDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IStringStatsAggregation, Nest.Field>) ((a, v) => a.Field = v));

    public StringStatsAggregationDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<IStringStatsAggregation, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public StringStatsAggregationDescriptor<T> Script(string script) => this.Assign<InlineScript>((InlineScript) script, (Action<IStringStatsAggregation, InlineScript>) ((a, v) => a.Script = (IScript) v));

    public StringStatsAggregationDescriptor<T> Script(Func<ScriptDescriptor, IScript> scriptSelector) => this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<IStringStatsAggregation, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));

    public StringStatsAggregationDescriptor<T> Missing(object missing) => this.Assign<object>(missing, (Action<IStringStatsAggregation, object>) ((a, v) => a.Missing = v));

    public StringStatsAggregationDescriptor<T> Meta(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> selector)
    {
      return this.Assign<Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>(selector, (Action<IStringStatsAggregation, Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>) ((a, v) => a.Meta = v != null ? (IDictionary<string, object>) v(new FluentDictionary<string, object>()) : (IDictionary<string, object>) null));
    }

    public StringStatsAggregationDescriptor<T> ShowDistribution(bool? showDistribution = true) => this.Assign<bool?>(showDistribution, (Action<IStringStatsAggregation, bool?>) ((a, v) => a.ShowDistribution = v));
  }
}
