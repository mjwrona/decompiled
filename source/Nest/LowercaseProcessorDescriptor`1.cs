// Decompiled with JetBrains decompiler
// Type: Nest.LowercaseProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class LowercaseProcessorDescriptor<T> : 
    ProcessorDescriptorBase<LowercaseProcessorDescriptor<T>, ILowercaseProcessor>,
    ILowercaseProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "lowercase";

    Nest.Field ILowercaseProcessor.Field { get; set; }

    bool? ILowercaseProcessor.IgnoreMissing { get; set; }

    Nest.Field ILowercaseProcessor.TargetField { get; set; }

    public LowercaseProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<ILowercaseProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public LowercaseProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<ILowercaseProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public LowercaseProcessorDescriptor<T> TargetField(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<ILowercaseProcessor, Nest.Field>) ((a, v) => a.TargetField = v));

    public LowercaseProcessorDescriptor<T> TargetField(Expression<Func<T, object>> objectPath) => this.Assign<Expression<Func<T, object>>>(objectPath, (Action<ILowercaseProcessor, Expression<Func<T, object>>>) ((a, v) => a.TargetField = (Nest.Field) (Expression) v));

    public LowercaseProcessorDescriptor<T> IgnoreMissing(bool? ignoreMissing = true) => this.Assign<bool?>(ignoreMissing, (Action<ILowercaseProcessor, bool?>) ((a, v) => a.IgnoreMissing = v));
  }
}
