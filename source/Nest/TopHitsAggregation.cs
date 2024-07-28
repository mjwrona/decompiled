// Decompiled with JetBrains decompiler
// Type: Nest.TopHitsAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class TopHitsAggregation : 
    MetricAggregationBase,
    ITopHitsAggregation,
    IMetricAggregation,
    IAggregation
  {
    internal TopHitsAggregation()
    {
    }

    public TopHitsAggregation(string name)
      : base(name, (Field) null)
    {
    }

    public Fields DocValueFields { get; set; }

    public bool? Explain { get; set; }

    public int? From { get; set; }

    public IHighlight Highlight { get; set; }

    public IScriptFields ScriptFields { get; set; }

    public int? Size { get; set; }

    public IList<ISort> Sort { get; set; }

    public Union<bool, ISourceFilter> Source { get; set; }

    public Fields StoredFields { get; set; }

    public bool? TrackScores { get; set; }

    public bool? Version { get; set; }

    internal override void WrapInContainer(AggregationContainer c) => c.TopHits = (ITopHitsAggregation) this;
  }
}
