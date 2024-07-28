// Decompiled with JetBrains decompiler
// Type: Nest.RegisteredDomainProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class RegisteredDomainProcessorDescriptor<T> : 
    ProcessorDescriptorBase<RegisteredDomainProcessorDescriptor<T>, IRegisteredDomainProcessor>,
    IRegisteredDomainProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "registered_domain";

    Nest.Field IRegisteredDomainProcessor.Field { get; set; }

    bool? IRegisteredDomainProcessor.IgnoreMissing { get; set; }

    Nest.Field IRegisteredDomainProcessor.TargetField { get; set; }

    public RegisteredDomainProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IRegisteredDomainProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public RegisteredDomainProcessorDescriptor<T> Field<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IRegisteredDomainProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));
    }

    public RegisteredDomainProcessorDescriptor<T> IgnoreMissing(bool? ignoreMissing = true) => this.Assign<bool?>(ignoreMissing, (Action<IRegisteredDomainProcessor, bool?>) ((a, v) => a.IgnoreMissing = v));

    public RegisteredDomainProcessorDescriptor<T> TargetField(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IRegisteredDomainProcessor, Nest.Field>) ((a, v) => a.TargetField = v));

    public RegisteredDomainProcessorDescriptor<T> TargetField<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IRegisteredDomainProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.TargetField = (Nest.Field) (Expression) v));
    }
  }
}
