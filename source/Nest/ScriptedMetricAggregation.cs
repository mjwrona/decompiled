// Decompiled with JetBrains decompiler
// Type: Nest.ScriptedMetricAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class ScriptedMetricAggregation : 
    MetricAggregationBase,
    IScriptedMetricAggregation,
    IMetricAggregation,
    IAggregation
  {
    internal ScriptedMetricAggregation()
    {
    }

    public ScriptedMetricAggregation(string name)
      : base(name, (Field) null)
    {
    }

    public IScript CombineScript { get; set; }

    public IScript InitScript { get; set; }

    public IScript MapScript { get; set; }

    public IDictionary<string, object> Params { get; set; }

    public IScript ReduceScript { get; set; }

    internal override void WrapInContainer(AggregationContainer c) => c.ScriptedMetric = (IScriptedMetricAggregation) this;
  }
}
