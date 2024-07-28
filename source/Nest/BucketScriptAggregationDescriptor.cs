// Decompiled with JetBrains decompiler
// Type: Nest.BucketScriptAggregationDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class BucketScriptAggregationDescriptor : 
    PipelineAggregationDescriptorBase<BucketScriptAggregationDescriptor, IBucketScriptAggregation, MultiBucketsPath>,
    IBucketScriptAggregation,
    IPipelineAggregation,
    IAggregation
  {
    IScript IBucketScriptAggregation.Script { get; set; }

    public BucketScriptAggregationDescriptor Script(string script) => this.Assign<InlineScript>((InlineScript) script, (Action<IBucketScriptAggregation, InlineScript>) ((a, v) => a.Script = (IScript) v));

    public BucketScriptAggregationDescriptor Script(Func<ScriptDescriptor, IScript> scriptSelector) => this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<IBucketScriptAggregation, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));

    public BucketScriptAggregationDescriptor BucketsPath(
      Func<MultiBucketsPathDescriptor, IPromise<IBucketsPath>> selector)
    {
      return this.Assign<Func<MultiBucketsPathDescriptor, IPromise<IBucketsPath>>>(selector, (Action<IBucketScriptAggregation, Func<MultiBucketsPathDescriptor, IPromise<IBucketsPath>>>) ((a, v) => a.BucketsPath = v != null ? v(new MultiBucketsPathDescriptor())?.Value : (IBucketsPath) null));
    }
  }
}
