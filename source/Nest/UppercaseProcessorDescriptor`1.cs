// Decompiled with JetBrains decompiler
// Type: Nest.UppercaseProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class UppercaseProcessorDescriptor<T> : 
    ProcessorDescriptorBase<UppercaseProcessorDescriptor<T>, IUppercaseProcessor>,
    IUppercaseProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "uppercase";

    Nest.Field IUppercaseProcessor.Field { get; set; }

    bool? IUppercaseProcessor.IgnoreMissing { get; set; }

    Nest.Field IUppercaseProcessor.TargetField { get; set; }

    public UppercaseProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IUppercaseProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public UppercaseProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IUppercaseProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public UppercaseProcessorDescriptor<T> TargetField(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IUppercaseProcessor, Nest.Field>) ((a, v) => a.TargetField = v));

    public UppercaseProcessorDescriptor<T> TargetField(Expression<Func<T, object>> objectPath) => this.Assign<Expression<Func<T, object>>>(objectPath, (Action<IUppercaseProcessor, Expression<Func<T, object>>>) ((a, v) => a.TargetField = (Nest.Field) (Expression) v));

    public UppercaseProcessorDescriptor<T> IgnoreMissing(bool? ignoreMissing = true) => this.Assign<bool?>(ignoreMissing, (Action<IUppercaseProcessor, bool?>) ((a, v) => a.IgnoreMissing = v));
  }
}
