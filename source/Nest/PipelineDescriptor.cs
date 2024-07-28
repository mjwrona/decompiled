// Decompiled with JetBrains decompiler
// Type: Nest.PipelineDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class PipelineDescriptor : DescriptorBase<PipelineDescriptor, IPipeline>, IPipeline
  {
    string IPipeline.Description { get; set; }

    IEnumerable<IProcessor> IPipeline.OnFailure { get; set; }

    IEnumerable<IProcessor> IPipeline.Processors { get; set; }

    long? IPipeline.Version { get; set; }

    public PipelineDescriptor Description(string description) => this.Assign<string>(description, (Action<IPipeline, string>) ((a, v) => a.Description = v));

    public PipelineDescriptor Processors(IEnumerable<IProcessor> processors) => this.Assign<List<IProcessor>>(processors.ToListOrNullIfEmpty<IProcessor>(), (Action<IPipeline, List<IProcessor>>) ((a, v) => a.Processors = (IEnumerable<IProcessor>) v));

    public PipelineDescriptor Processors(
      Func<ProcessorsDescriptor, IPromise<IList<IProcessor>>> selector)
    {
      return this.Assign<Func<ProcessorsDescriptor, IPromise<IList<IProcessor>>>>(selector, (Action<IPipeline, Func<ProcessorsDescriptor, IPromise<IList<IProcessor>>>>) ((a, v) => a.Processors = v != null ? (IEnumerable<IProcessor>) v(new ProcessorsDescriptor())?.Value : (IEnumerable<IProcessor>) null));
    }

    public PipelineDescriptor OnFailure(IEnumerable<IProcessor> processors) => this.Assign<List<IProcessor>>(processors.ToListOrNullIfEmpty<IProcessor>(), (Action<IPipeline, List<IProcessor>>) ((a, v) => a.OnFailure = (IEnumerable<IProcessor>) v));

    public PipelineDescriptor OnFailure(
      Func<ProcessorsDescriptor, IPromise<IList<IProcessor>>> selector)
    {
      return this.Assign<Func<ProcessorsDescriptor, IPromise<IList<IProcessor>>>>(selector, (Action<IPipeline, Func<ProcessorsDescriptor, IPromise<IList<IProcessor>>>>) ((a, v) => a.OnFailure = v != null ? (IEnumerable<IProcessor>) v(new ProcessorsDescriptor())?.Value : (IEnumerable<IProcessor>) null));
    }

    public PipelineDescriptor Version(long? version = null) => this.Assign<long?>(version, (Action<IPipeline, long?>) ((a, v) => a.Version = v));
  }
}
