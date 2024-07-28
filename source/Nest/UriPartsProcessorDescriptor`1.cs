// Decompiled with JetBrains decompiler
// Type: Nest.UriPartsProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class UriPartsProcessorDescriptor<T> : 
    ProcessorDescriptorBase<UriPartsProcessorDescriptor<T>, IUriPartsProcessor>,
    IUriPartsProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "uri_parts";

    Nest.Field IUriPartsProcessor.Field { get; set; }

    bool? IUriPartsProcessor.KeepOriginal { get; set; }

    Nest.Field IUriPartsProcessor.TargetField { get; set; }

    bool? IUriPartsProcessor.RemoveIfSuccessful { get; set; }

    public UriPartsProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IUriPartsProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public UriPartsProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IUriPartsProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public UriPartsProcessorDescriptor<T> TargetField(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IUriPartsProcessor, Nest.Field>) ((a, v) => a.TargetField = v));

    public UriPartsProcessorDescriptor<T> TargetField<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IUriPartsProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.TargetField = (Nest.Field) (Expression) v));

    public UriPartsProcessorDescriptor<T> KeepOriginal(bool? keepOriginal = true) => this.Assign<bool?>(keepOriginal, (Action<IUriPartsProcessor, bool?>) ((a, v) => a.KeepOriginal = v));

    public UriPartsProcessorDescriptor<T> RemoveIfSuccessful(bool? removeIfSuccessful = true) => this.Assign<bool?>(removeIfSuccessful, (Action<IUriPartsProcessor, bool?>) ((a, v) => a.RemoveIfSuccessful = v));
  }
}
