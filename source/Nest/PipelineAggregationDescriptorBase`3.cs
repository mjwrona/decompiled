// Decompiled with JetBrains decompiler
// Type: Nest.PipelineAggregationDescriptorBase`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public abstract class PipelineAggregationDescriptorBase<TPipelineAggregation, TPipelineAggregationInterface, TBucketsPath> : 
    DescriptorBase<TPipelineAggregation, TPipelineAggregationInterface>,
    IPipelineAggregation,
    IAggregation
    where TPipelineAggregation : PipelineAggregationDescriptorBase<TPipelineAggregation, TPipelineAggregationInterface, TBucketsPath>, TPipelineAggregationInterface, IPipelineAggregation
    where TPipelineAggregationInterface : class, IPipelineAggregation
    where TBucketsPath : IBucketsPath
  {
    IBucketsPath IPipelineAggregation.BucketsPath { get; set; }

    string IPipelineAggregation.Format { get; set; }

    Nest.GapPolicy? IPipelineAggregation.GapPolicy { get; set; }

    IDictionary<string, object> IAggregation.Meta { get; set; }

    string IAggregation.Name { get; set; }

    public TPipelineAggregation Format(string format) => this.Assign<string>(format, (Action<TPipelineAggregationInterface, string>) ((a, v) => a.Format = v));

    public TPipelineAggregation GapPolicy(Nest.GapPolicy? gapPolicy) => this.Assign<Nest.GapPolicy?>(gapPolicy, (Action<TPipelineAggregationInterface, Nest.GapPolicy?>) ((a, v) => a.GapPolicy = v));

    public TPipelineAggregation BucketsPath(TBucketsPath bucketsPath) => this.Assign<TBucketsPath>(bucketsPath, (Action<TPipelineAggregationInterface, TBucketsPath>) ((a, v) => a.BucketsPath = (IBucketsPath) v));

    public TPipelineAggregation Meta(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> selector)
    {
      return this.Assign<Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>(selector, (Action<TPipelineAggregationInterface, Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>) ((a, v) => a.Meta = v != null ? (IDictionary<string, object>) v(new FluentDictionary<string, object>()) : (IDictionary<string, object>) null));
    }
  }
}
