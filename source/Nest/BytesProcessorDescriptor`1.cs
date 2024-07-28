// Decompiled with JetBrains decompiler
// Type: Nest.BytesProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class BytesProcessorDescriptor<T> : 
    ProcessorDescriptorBase<BytesProcessorDescriptor<T>, IBytesProcessor>,
    IBytesProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "bytes";

    Nest.Field IBytesProcessor.Field { get; set; }

    bool? IBytesProcessor.IgnoreMissing { get; set; }

    Nest.Field IBytesProcessor.TargetField { get; set; }

    public BytesProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IBytesProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public BytesProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IBytesProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public BytesProcessorDescriptor<T> TargetField(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IBytesProcessor, Nest.Field>) ((a, v) => a.TargetField = v));

    public BytesProcessorDescriptor<T> TargetField<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IBytesProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.TargetField = (Nest.Field) (Expression) v));

    public BytesProcessorDescriptor<T> IgnoreMissing(bool? ignoreMissing = true) => this.Assign<bool?>(ignoreMissing, (Action<IBytesProcessor, bool?>) ((a, v) => a.IgnoreMissing = v));
  }
}
