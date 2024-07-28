// Decompiled with JetBrains decompiler
// Type: Nest.TermsAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class TermsAggregation : 
    BucketAggregationBase,
    ITermsAggregation,
    IBucketAggregation,
    IAggregation
  {
    internal TermsAggregation()
    {
    }

    public TermsAggregation(string name)
      : base(name)
    {
    }

    public TermsAggregationCollectMode? CollectMode { get; set; }

    public TermsExclude Exclude { get; set; }

    public TermsAggregationExecutionHint? ExecutionHint { get; set; }

    public Field Field { get; set; }

    public TermsInclude Include { get; set; }

    public int? MinimumDocumentCount { get; set; }

    public object Missing { get; set; }

    public IList<TermsOrder> Order { get; set; }

    public IScript Script { get; set; }

    public int? ShardSize { get; set; }

    public bool? ShowTermDocCountError { get; set; }

    public int? Size { get; set; }

    internal override void WrapInContainer(AggregationContainer c) => c.Terms = (ITermsAggregation) this;
  }
}
