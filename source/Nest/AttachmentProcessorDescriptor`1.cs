// Decompiled with JetBrains decompiler
// Type: Nest.AttachmentProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class AttachmentProcessorDescriptor<T> : 
    ProcessorDescriptorBase<AttachmentProcessorDescriptor<T>, IAttachmentProcessor>,
    IAttachmentProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "attachment";

    Nest.Field IAttachmentProcessor.Field { get; set; }

    bool? IAttachmentProcessor.IgnoreMissing { get; set; }

    long? IAttachmentProcessor.IndexedCharacters { get; set; }

    Nest.Field IAttachmentProcessor.IndexedCharactersField { get; set; }

    IEnumerable<string> IAttachmentProcessor.Properties { get; set; }

    Nest.Field IAttachmentProcessor.TargetField { get; set; }

    Nest.Field IAttachmentProcessor.ResourceName { get; set; }

    public AttachmentProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IAttachmentProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public AttachmentProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IAttachmentProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public AttachmentProcessorDescriptor<T> TargetField(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IAttachmentProcessor, Nest.Field>) ((a, v) => a.TargetField = v));

    public AttachmentProcessorDescriptor<T> TargetField<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IAttachmentProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.TargetField = (Nest.Field) (Expression) v));
    }

    public AttachmentProcessorDescriptor<T> IndexedCharacters(long? indexedCharacters) => this.Assign<long?>(indexedCharacters, (Action<IAttachmentProcessor, long?>) ((a, v) => a.IndexedCharacters = v));

    public AttachmentProcessorDescriptor<T> IndexedCharactersField(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IAttachmentProcessor, Nest.Field>) ((a, v) => a.IndexedCharactersField = v));

    public AttachmentProcessorDescriptor<T> IndexedCharactersField<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IAttachmentProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.IndexedCharactersField = (Nest.Field) (Expression) v));
    }

    public AttachmentProcessorDescriptor<T> IgnoreMissing(bool? ignoreMissing = true) => this.Assign<bool?>(ignoreMissing, (Action<IAttachmentProcessor, bool?>) ((a, v) => a.IgnoreMissing = v));

    public AttachmentProcessorDescriptor<T> Properties(IEnumerable<string> properties) => this.Assign<IEnumerable<string>>(properties, (Action<IAttachmentProcessor, IEnumerable<string>>) ((a, v) => a.Properties = v));

    public AttachmentProcessorDescriptor<T> Properties(params string[] properties) => this.Assign<string[]>(properties, (Action<IAttachmentProcessor, string[]>) ((a, v) => a.Properties = (IEnumerable<string>) v));

    public AttachmentProcessorDescriptor<T> ResourceName(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IAttachmentProcessor, Nest.Field>) ((a, v) => a.ResourceName = v));

    public AttachmentProcessorDescriptor<T> ResourceName<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IAttachmentProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.ResourceName = (Nest.Field) (Expression) v));
    }
  }
}
