// Decompiled with JetBrains decompiler
// Type: Nest.FingerprintProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class FingerprintProcessorDescriptor<T> : 
    ProcessorDescriptorBase<FingerprintProcessorDescriptor<T>, IFingerprintProcessor>,
    IFingerprintProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "fingerprint";

    Nest.Fields IFingerprintProcessor.Fields { get; set; }

    string IFingerprintProcessor.Method { get; set; }

    string IFingerprintProcessor.Salt { get; set; }

    Field IFingerprintProcessor.TargetField { get; set; }

    bool? IFingerprintProcessor.IgnoreMissing { get; set; }

    public FingerprintProcessorDescriptor<T> Fields(Nest.Fields fields) => this.Assign<Nest.Fields>(fields, (Action<IFingerprintProcessor, Nest.Fields>) ((a, v) => a.Fields = v));

    public FingerprintProcessorDescriptor<T> Fields(
      Func<FieldsDescriptor<T>, IPromise<Nest.Fields>> selector)
    {
      return this.Assign<Func<FieldsDescriptor<T>, IPromise<Nest.Fields>>>(selector, (Action<IFingerprintProcessor, Func<FieldsDescriptor<T>, IPromise<Nest.Fields>>>) ((a, v) => a.Fields = v != null ? v(new FieldsDescriptor<T>())?.Value : (Nest.Fields) null));
    }

    public FingerprintProcessorDescriptor<T> IgnoreMissing(bool? ignoreMissing = true) => this.Assign<bool?>(ignoreMissing, (Action<IFingerprintProcessor, bool?>) ((a, v) => a.IgnoreMissing = v));

    public FingerprintProcessorDescriptor<T> Method(string method) => this.Assign<string>(method, (Action<IFingerprintProcessor, string>) ((a, v) => a.Method = v));

    public FingerprintProcessorDescriptor<T> Salt(string salt) => this.Assign<string>(salt, (Action<IFingerprintProcessor, string>) ((a, v) => a.Salt = v));

    public FingerprintProcessorDescriptor<T> TargetField(Field field) => this.Assign<Field>(field, (Action<IFingerprintProcessor, Field>) ((a, v) => a.TargetField = v));

    public FingerprintProcessorDescriptor<T> TargetField<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IFingerprintProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.TargetField = (Field) (Expression) v));
    }
  }
}
