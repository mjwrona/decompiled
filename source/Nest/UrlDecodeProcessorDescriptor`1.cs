// Decompiled with JetBrains decompiler
// Type: Nest.UrlDecodeProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class UrlDecodeProcessorDescriptor<T> : 
    ProcessorDescriptorBase<UrlDecodeProcessorDescriptor<T>, IUrlDecodeProcessor>,
    IUrlDecodeProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "urldecode";

    Nest.Field IUrlDecodeProcessor.Field { get; set; }

    bool? IUrlDecodeProcessor.IgnoreMissing { get; set; }

    Nest.Field IUrlDecodeProcessor.TargetField { get; set; }

    public UrlDecodeProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IUrlDecodeProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public UrlDecodeProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IUrlDecodeProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public UrlDecodeProcessorDescriptor<T> TargetField(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IUrlDecodeProcessor, Nest.Field>) ((a, v) => a.TargetField = v));

    public UrlDecodeProcessorDescriptor<T> TargetField<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IUrlDecodeProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.TargetField = (Nest.Field) (Expression) v));
    }

    public UrlDecodeProcessorDescriptor<T> IgnoreMissing(bool? ignoreMissing = true) => this.Assign<bool?>(ignoreMissing, (Action<IUrlDecodeProcessor, bool?>) ((a, v) => a.IgnoreMissing = v));
  }
}
