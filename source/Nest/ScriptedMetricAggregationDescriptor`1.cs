// Decompiled with JetBrains decompiler
// Type: Nest.ScriptedMetricAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class ScriptedMetricAggregationDescriptor<T> : 
    MetricAggregationDescriptorBase<ScriptedMetricAggregationDescriptor<T>, IScriptedMetricAggregation, T>,
    IScriptedMetricAggregation,
    IMetricAggregation,
    IAggregation
    where T : class
  {
    IScript IScriptedMetricAggregation.CombineScript { get; set; }

    IScript IScriptedMetricAggregation.InitScript { get; set; }

    IScript IScriptedMetricAggregation.MapScript { get; set; }

    IDictionary<string, object> IScriptedMetricAggregation.Params { get; set; }

    IScript IScriptedMetricAggregation.ReduceScript { get; set; }

    public ScriptedMetricAggregationDescriptor<T> InitScript(string script) => this.Assign<InlineScript>((InlineScript) script, (Action<IScriptedMetricAggregation, InlineScript>) ((a, v) => a.InitScript = (IScript) v));

    public ScriptedMetricAggregationDescriptor<T> InitScript(
      Func<ScriptDescriptor, IScript> scriptSelector)
    {
      return this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<IScriptedMetricAggregation, Func<ScriptDescriptor, IScript>>) ((a, v) => a.InitScript = v != null ? v(new ScriptDescriptor()) : (IScript) null));
    }

    public ScriptedMetricAggregationDescriptor<T> MapScript(string script) => this.Assign<InlineScript>((InlineScript) script, (Action<IScriptedMetricAggregation, InlineScript>) ((a, v) => a.MapScript = (IScript) v));

    public ScriptedMetricAggregationDescriptor<T> MapScript(
      Func<ScriptDescriptor, IScript> scriptSelector)
    {
      return this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<IScriptedMetricAggregation, Func<ScriptDescriptor, IScript>>) ((a, v) => a.MapScript = v != null ? v(new ScriptDescriptor()) : (IScript) null));
    }

    public ScriptedMetricAggregationDescriptor<T> CombineScript(string script) => this.Assign<InlineScript>((InlineScript) script, (Action<IScriptedMetricAggregation, InlineScript>) ((a, v) => a.CombineScript = (IScript) v));

    public ScriptedMetricAggregationDescriptor<T> CombineScript(
      Func<ScriptDescriptor, IScript> scriptSelector)
    {
      return this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<IScriptedMetricAggregation, Func<ScriptDescriptor, IScript>>) ((a, v) => a.CombineScript = v != null ? v(new ScriptDescriptor()) : (IScript) null));
    }

    public ScriptedMetricAggregationDescriptor<T> ReduceScript(string script) => this.Assign<InlineScript>((InlineScript) script, (Action<IScriptedMetricAggregation, InlineScript>) ((a, v) => a.ReduceScript = (IScript) v));

    public ScriptedMetricAggregationDescriptor<T> ReduceScript(
      Func<ScriptDescriptor, IScript> scriptSelector)
    {
      return this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<IScriptedMetricAggregation, Func<ScriptDescriptor, IScript>>) ((a, v) => a.ReduceScript = v != null ? v(new ScriptDescriptor()) : (IScript) null));
    }

    public ScriptedMetricAggregationDescriptor<T> Params(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> paramSelector)
    {
      return this.Assign<Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>(paramSelector, (Action<IScriptedMetricAggregation, Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>) ((a, v) => a.Params = v != null ? (IDictionary<string, object>) v(new FluentDictionary<string, object>()) : (IDictionary<string, object>) null));
    }
  }
}
