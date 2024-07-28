// Decompiled with JetBrains decompiler
// Type: Nest.TrimProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class TrimProcessorDescriptor<T> : 
    ProcessorDescriptorBase<TrimProcessorDescriptor<T>, ITrimProcessor>,
    ITrimProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "trim";

    Nest.Field ITrimProcessor.Field { get; set; }

    bool? ITrimProcessor.IgnoreMissing { get; set; }

    Nest.Field ITrimProcessor.TargetField { get; set; }

    public TrimProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<ITrimProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public TrimProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<ITrimProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public TrimProcessorDescriptor<T> TargetField(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<ITrimProcessor, Nest.Field>) ((a, v) => a.TargetField = v));

    public TrimProcessorDescriptor<T> TargetField(Expression<Func<T, object>> objectPath) => this.Assign<Expression<Func<T, object>>>(objectPath, (Action<ITrimProcessor, Expression<Func<T, object>>>) ((a, v) => a.TargetField = (Nest.Field) (Expression) v));

    public TrimProcessorDescriptor<T> IgnoreMissing(bool? ignoreMissing = true) => this.Assign<bool?>(ignoreMissing, (Action<ITrimProcessor, bool?>) ((a, v) => a.IgnoreMissing = v));
  }
}
