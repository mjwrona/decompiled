// Decompiled with JetBrains decompiler
// Type: Nest.ConvertProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class ConvertProcessorDescriptor<T> : 
    ProcessorDescriptorBase<ConvertProcessorDescriptor<T>, IConvertProcessor>,
    IConvertProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "convert";

    Nest.Field IConvertProcessor.Field { get; set; }

    Nest.Field IConvertProcessor.TargetField { get; set; }

    bool? IConvertProcessor.IgnoreMissing { get; set; }

    ConvertProcessorType? IConvertProcessor.Type { get; set; }

    public ConvertProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IConvertProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public ConvertProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IConvertProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public ConvertProcessorDescriptor<T> TargetField(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IConvertProcessor, Nest.Field>) ((a, v) => a.TargetField = v));

    public ConvertProcessorDescriptor<T> TargetField<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IConvertProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.TargetField = (Nest.Field) (Expression) v));

    public ConvertProcessorDescriptor<T> Type(ConvertProcessorType? type) => this.Assign<ConvertProcessorType?>(type, (Action<IConvertProcessor, ConvertProcessorType?>) ((a, v) => a.Type = v));

    public ConvertProcessorDescriptor<T> IgnoreMissing(bool? ignoreMissing = true) => this.Assign<bool?>(ignoreMissing, (Action<IConvertProcessor, bool?>) ((a, v) => a.IgnoreMissing = v));
  }
}
