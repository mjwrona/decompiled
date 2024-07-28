// Decompiled with JetBrains decompiler
// Type: Nest.JsonProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class JsonProcessorDescriptor<T> : 
    ProcessorDescriptorBase<JsonProcessorDescriptor<T>, IJsonProcessor>,
    IJsonProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "json";

    bool? IJsonProcessor.AddToRoot { get; set; }

    Nest.Field IJsonProcessor.Field { get; set; }

    Nest.Field IJsonProcessor.TargetField { get; set; }

    public JsonProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IJsonProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public JsonProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IJsonProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public JsonProcessorDescriptor<T> TargetField(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IJsonProcessor, Nest.Field>) ((a, v) => a.TargetField = v));

    public JsonProcessorDescriptor<T> TargetField<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IJsonProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.TargetField = (Nest.Field) (Expression) v));

    public JsonProcessorDescriptor<T> AddToRoot(bool? addToRoot = true) => this.Assign<bool?>(addToRoot, (Action<IJsonProcessor, bool?>) ((a, v) => a.AddToRoot = v));
  }
}
