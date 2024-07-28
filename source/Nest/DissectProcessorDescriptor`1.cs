// Decompiled with JetBrains decompiler
// Type: Nest.DissectProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class DissectProcessorDescriptor<T> : 
    ProcessorDescriptorBase<DissectProcessorDescriptor<T>, IDissectProcessor>,
    IDissectProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "dissect";

    Nest.Field IDissectProcessor.Field { get; set; }

    string IDissectProcessor.Pattern { get; set; }

    bool? IDissectProcessor.IgnoreMissing { get; set; }

    string IDissectProcessor.AppendSeparator { get; set; }

    public DissectProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IDissectProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public DissectProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IDissectProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public DissectProcessorDescriptor<T> Pattern(string pattern) => this.Assign<string>(pattern, (Action<IDissectProcessor, string>) ((a, v) => a.Pattern = v));

    public DissectProcessorDescriptor<T> IgnoreMissing(bool? traceMatch = true) => this.Assign<bool?>(traceMatch, (Action<IDissectProcessor, bool?>) ((a, v) => a.IgnoreMissing = v));

    public DissectProcessorDescriptor<T> AppendSeparator(string appendSeparator) => this.Assign<string>(appendSeparator, (Action<IDissectProcessor, string>) ((a, v) => a.AppendSeparator = v));
  }
}
