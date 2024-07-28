// Decompiled with JetBrains decompiler
// Type: Nest.MultiTermsAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class MultiTermsAggregation : 
    BucketAggregationBase,
    IMultiTermsAggregation,
    IBucketAggregation,
    IAggregation
  {
    internal MultiTermsAggregation()
    {
    }

    public MultiTermsAggregation(string name)
      : base(name)
    {
    }

    public TermsAggregationCollectMode? CollectMode { get; set; }

    public int? MinimumDocumentCount { get; set; }

    public IList<TermsOrder> Order { get; set; }

    public IScript Script { get; set; }

    public int? ShardMinimumDocumentCount { get; set; }

    public int? ShardSize { get; set; }

    public bool? ShowTermDocCountError { get; set; }

    public int? Size { get; set; }

    public IEnumerable<ITerm> Terms { get; set; }

    internal override void WrapInContainer(AggregationContainer c) => c.MultiTerms = (IMultiTermsAggregation) this;
  }
}
