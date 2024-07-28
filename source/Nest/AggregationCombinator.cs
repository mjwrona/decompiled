// Decompiled with JetBrains decompiler
// Type: Nest.AggregationCombinator
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  internal class AggregationCombinator : AggregationBase, IAggregation
  {
    public AggregationCombinator(string name, AggregationBase left, AggregationBase right)
      : base(name)
    {
      this.AddAggregation(left);
      this.AddAggregation(right);
    }

    internal List<AggregationBase> Aggregations { get; } = new List<AggregationBase>();

    internal override void WrapInContainer(AggregationContainer container)
    {
    }

    private void AddAggregation(AggregationBase agg)
    {
      AggregationBase aggregationBase = agg;
      if (aggregationBase == null)
        return;
      if (aggregationBase is AggregationCombinator aggregationCombinator && aggregationCombinator.Aggregations.Any<AggregationBase>())
        this.Aggregations.AddRange((IEnumerable<AggregationBase>) aggregationCombinator.Aggregations);
      else
        this.Aggregations.Add(agg);
    }
  }
}
