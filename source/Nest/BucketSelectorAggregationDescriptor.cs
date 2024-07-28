// Decompiled with JetBrains decompiler
// Type: Nest.BucketSelectorAggregationDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class BucketSelectorAggregationDescriptor : 
    PipelineAggregationDescriptorBase<BucketSelectorAggregationDescriptor, IBucketSelectorAggregation, MultiBucketsPath>,
    IBucketSelectorAggregation,
    IPipelineAggregation,
    IAggregation
  {
    IScript IBucketSelectorAggregation.Script { get; set; }

    public BucketSelectorAggregationDescriptor Script(string script) => this.Assign<InlineScript>((InlineScript) script, (Action<IBucketSelectorAggregation, InlineScript>) ((a, v) => a.Script = (IScript) v));

    public BucketSelectorAggregationDescriptor Script(Func<ScriptDescriptor, IScript> scriptSelector) => this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<IBucketSelectorAggregation, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));

    public BucketSelectorAggregationDescriptor BucketsPath(
      Func<MultiBucketsPathDescriptor, IPromise<IBucketsPath>> selector)
    {
      return this.Assign<Func<MultiBucketsPathDescriptor, IPromise<IBucketsPath>>>(selector, (Action<IBucketSelectorAggregation, Func<MultiBucketsPathDescriptor, IPromise<IBucketsPath>>>) ((a, v) => a.BucketsPath = v != null ? v(new MultiBucketsPathDescriptor())?.Value : (IBucketsPath) null));
    }
  }
}
