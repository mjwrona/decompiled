// Decompiled with JetBrains decompiler
// Type: Nest.RemoveProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class RemoveProcessorDescriptor<T> : 
    ProcessorDescriptorBase<RemoveProcessorDescriptor<T>, IRemoveProcessor>,
    IRemoveProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "remove";

    Fields IRemoveProcessor.Field { get; set; }

    bool? IRemoveProcessor.IgnoreMissing { get; set; }

    public RemoveProcessorDescriptor<T> Field(Fields fields) => this.Assign<Fields>(fields, (Action<IRemoveProcessor, Fields>) ((a, v) => a.Field = v));

    public RemoveProcessorDescriptor<T> Field(
      Func<FieldsDescriptor<T>, IPromise<Fields>> selector)
    {
      return this.Assign<Func<FieldsDescriptor<T>, IPromise<Fields>>>(selector, (Action<IRemoveProcessor, Func<FieldsDescriptor<T>, IPromise<Fields>>>) ((a, v) => a.Field = v != null ? v(new FieldsDescriptor<T>())?.Value : (Fields) null));
    }

    public RemoveProcessorDescriptor<T> IgnoreMissing(bool? ignoreMissing = true) => this.Assign<bool?>(ignoreMissing, (Action<IRemoveProcessor, bool?>) ((a, v) => a.IgnoreMissing = v));
  }
}
