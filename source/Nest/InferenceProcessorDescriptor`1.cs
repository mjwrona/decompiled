// Decompiled with JetBrains decompiler
// Type: Nest.InferenceProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class InferenceProcessorDescriptor<T> : 
    ProcessorDescriptorBase<InferenceProcessorDescriptor<T>, IInferenceProcessor>,
    IInferenceProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "inference";

    Field IInferenceProcessor.TargetField { get; set; }

    string IInferenceProcessor.ModelId { get; set; }

    IInferenceConfig IInferenceProcessor.InferenceConfig { get; set; }

    IDictionary<Field, Field> IInferenceProcessor.FieldMappings { get; set; }

    public InferenceProcessorDescriptor<T> TargetField(Field field) => this.Assign<Field>(field, (Action<IInferenceProcessor, Field>) ((a, v) => a.TargetField = v));

    public InferenceProcessorDescriptor<T> TargetField<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IInferenceProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.TargetField = (Field) (Expression) v));
    }

    public InferenceProcessorDescriptor<T> ModelId(string modelId) => this.Assign<string>(modelId, (Action<IInferenceProcessor, string>) ((a, v) => a.ModelId = v));

    public InferenceProcessorDescriptor<T> InferenceConfig(
      Func<InferenceConfigDescriptor<T>, IInferenceConfig> selector)
    {
      return this.Assign<Func<InferenceConfigDescriptor<T>, IInferenceConfig>>(selector, (Action<IInferenceProcessor, Func<InferenceConfigDescriptor<T>, IInferenceConfig>>) ((a, v) => a.InferenceConfig = v.InvokeOrDefault<InferenceConfigDescriptor<T>, IInferenceConfig>(new InferenceConfigDescriptor<T>())));
    }

    public InferenceProcessorDescriptor<T> FieldMappings(
      Func<FluentDictionary<Field, Field>, FluentDictionary<Field, Field>> selector = null)
    {
      return this.Assign<Func<FluentDictionary<Field, Field>, FluentDictionary<Field, Field>>>(selector, (Action<IInferenceProcessor, Func<FluentDictionary<Field, Field>, FluentDictionary<Field, Field>>>) ((a, v) => a.FieldMappings = (IDictionary<Field, Field>) v.InvokeOrDefault<FluentDictionary<Field, Field>, FluentDictionary<Field, Field>>(new FluentDictionary<Field, Field>())));
    }
  }
}
