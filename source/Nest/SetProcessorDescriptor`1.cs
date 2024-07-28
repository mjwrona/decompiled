// Decompiled with JetBrains decompiler
// Type: Nest.SetProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class SetProcessorDescriptor<T> : 
    ProcessorDescriptorBase<SetProcessorDescriptor<T>, ISetProcessor>,
    ISetProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "set";

    Nest.Field ISetProcessor.Field { get; set; }

    object ISetProcessor.Value { get; set; }

    bool? ISetProcessor.Override { get; set; }

    bool? ISetProcessor.IgnoreEmptyValue { get; set; }

    Nest.Field ISetProcessor.CopyFrom { get; set; }

    public SetProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<ISetProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public SetProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<ISetProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public SetProcessorDescriptor<T> Value<TValue>(TValue value) => this.Assign<TValue>(value, (Action<ISetProcessor, TValue>) ((a, v) => a.Value = (object) v));

    public SetProcessorDescriptor<T> Override(bool? @override = true) => this.Assign<bool?>(@override, (Action<ISetProcessor, bool?>) ((a, v) => a.Override = v));

    public SetProcessorDescriptor<T> IgnoreEmptyValue(bool? ignoreEmptyValue = true) => this.Assign<bool?>(ignoreEmptyValue, (Action<ISetProcessor, bool?>) ((a, v) => a.IgnoreEmptyValue = v));

    public SetProcessorDescriptor<T> CopyFrom(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<ISetProcessor, Nest.Field>) ((a, v) => a.CopyFrom = v));

    public SetProcessorDescriptor<T> CopyFrom<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<ISetProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.CopyFrom = (Nest.Field) (Expression) v));
  }
}
