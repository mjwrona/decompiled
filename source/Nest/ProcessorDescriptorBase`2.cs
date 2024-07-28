// Decompiled with JetBrains decompiler
// Type: Nest.ProcessorDescriptorBase`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public abstract class ProcessorDescriptorBase<TProcessorDescriptor, TProcessorInterface> : 
    DescriptorBase<TProcessorDescriptor, TProcessorInterface>,
    IProcessor
    where TProcessorDescriptor : ProcessorDescriptorBase<TProcessorDescriptor, TProcessorInterface>, TProcessorInterface
    where TProcessorInterface : class, IProcessor
  {
    protected abstract string Name { get; }

    string IProcessor.Name => this.Name;

    string IProcessor.Description { get; set; }

    IEnumerable<IProcessor> IProcessor.OnFailure { get; set; }

    string IProcessor.If { get; set; }

    string IProcessor.Tag { get; set; }

    bool? IProcessor.IgnoreFailure { get; set; }

    public TProcessorDescriptor Description(string description) => this.Assign<string>(description, (Action<TProcessorInterface, string>) ((a, v) => a.Description = v));

    public TProcessorDescriptor OnFailure(IEnumerable<IProcessor> processors) => this.Assign<List<IProcessor>>(processors.ToListOrNullIfEmpty<IProcessor>(), (Action<TProcessorInterface, List<IProcessor>>) ((a, v) => a.OnFailure = (IEnumerable<IProcessor>) v));

    public TProcessorDescriptor OnFailure(
      Func<ProcessorsDescriptor, IPromise<IList<IProcessor>>> selector)
    {
      return this.Assign<Func<ProcessorsDescriptor, IPromise<IList<IProcessor>>>>(selector, (Action<TProcessorInterface, Func<ProcessorsDescriptor, IPromise<IList<IProcessor>>>>) ((a, v) => a.OnFailure = v != null ? (IEnumerable<IProcessor>) v(new ProcessorsDescriptor())?.Value : (IEnumerable<IProcessor>) null));
    }

    public TProcessorDescriptor If(string painlessPredicate) => this.Assign<string>(painlessPredicate, (Action<TProcessorInterface, string>) ((a, v) => a.If = v));

    public TProcessorDescriptor Tag(string tag) => this.Assign<string>(tag, (Action<TProcessorInterface, string>) ((a, v) => a.Tag = v));

    public TProcessorDescriptor IgnoreFailure(bool? ignoreFailure = true) => this.Assign<bool?>(ignoreFailure, (Action<TProcessorInterface, bool?>) ((a, v) => a.IgnoreFailure = v));
  }
}
