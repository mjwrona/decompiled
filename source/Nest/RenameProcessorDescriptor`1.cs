// Decompiled with JetBrains decompiler
// Type: Nest.RenameProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class RenameProcessorDescriptor<T> : 
    ProcessorDescriptorBase<RenameProcessorDescriptor<T>, IRenameProcessor>,
    IRenameProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "rename";

    Nest.Field IRenameProcessor.Field { get; set; }

    Nest.Field IRenameProcessor.TargetField { get; set; }

    bool? IRenameProcessor.IgnoreMissing { get; set; }

    public RenameProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IRenameProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public RenameProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IRenameProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public RenameProcessorDescriptor<T> TargetField(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IRenameProcessor, Nest.Field>) ((a, v) => a.TargetField = v));

    public RenameProcessorDescriptor<T> TargetField<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IRenameProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.TargetField = (Nest.Field) (Expression) v));

    public RenameProcessorDescriptor<T> IgnoreMissing(bool? ignoreMissing = true) => this.Assign<bool?>(ignoreMissing, (Action<IRenameProcessor, bool?>) ((a, v) => a.IgnoreMissing = v));
  }
}
