// Decompiled with JetBrains decompiler
// Type: Nest.GsubProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class GsubProcessorDescriptor<T> : 
    ProcessorDescriptorBase<GsubProcessorDescriptor<T>, IGsubProcessor>,
    IGsubProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "gsub";

    Nest.Field IGsubProcessor.Field { get; set; }

    Nest.Field IGsubProcessor.TargetField { get; set; }

    string IGsubProcessor.Pattern { get; set; }

    string IGsubProcessor.Replacement { get; set; }

    bool? IGsubProcessor.IgnoreMissing { get; set; }

    public GsubProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IGsubProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public GsubProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IGsubProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public GsubProcessorDescriptor<T> TargetField(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IGsubProcessor, Nest.Field>) ((a, v) => a.TargetField = v));

    public GsubProcessorDescriptor<T> TargetField(Expression<Func<T, object>> objectPath) => this.Assign<Expression<Func<T, object>>>(objectPath, (Action<IGsubProcessor, Expression<Func<T, object>>>) ((a, v) => a.TargetField = (Nest.Field) (Expression) v));

    public GsubProcessorDescriptor<T> Pattern(string pattern) => this.Assign<string>(pattern, (Action<IGsubProcessor, string>) ((a, v) => a.Pattern = v));

    public GsubProcessorDescriptor<T> Replacement(string replacement) => this.Assign<string>(replacement, (Action<IGsubProcessor, string>) ((a, v) => a.Replacement = v));

    public GsubProcessorDescriptor<T> IgnoreMissing(bool? ignoreMissing = true) => this.Assign<bool?>(ignoreMissing, (Action<IGsubProcessor, bool?>) ((a, v) => a.IgnoreMissing = v));
  }
}
