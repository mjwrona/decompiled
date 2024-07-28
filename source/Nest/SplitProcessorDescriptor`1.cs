// Decompiled with JetBrains decompiler
// Type: Nest.SplitProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class SplitProcessorDescriptor<T> : 
    ProcessorDescriptorBase<SplitProcessorDescriptor<T>, ISplitProcessor>,
    ISplitProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "split";

    Nest.Field ISplitProcessor.Field { get; set; }

    bool? ISplitProcessor.IgnoreMissing { get; set; }

    string ISplitProcessor.Separator { get; set; }

    Nest.Field ISplitProcessor.TargetField { get; set; }

    bool? ISplitProcessor.PreserveTrailing { get; set; }

    public SplitProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<ISplitProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public SplitProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<ISplitProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public SplitProcessorDescriptor<T> TargetField(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<ISplitProcessor, Nest.Field>) ((a, v) => a.TargetField = v));

    public SplitProcessorDescriptor<T> TargetField(Expression<Func<T, object>> objectPath) => this.Assign<Expression<Func<T, object>>>(objectPath, (Action<ISplitProcessor, Expression<Func<T, object>>>) ((a, v) => a.TargetField = (Nest.Field) (Expression) v));

    public SplitProcessorDescriptor<T> Separator(string separator) => this.Assign<string>(separator, (Action<ISplitProcessor, string>) ((a, v) => a.Separator = v));

    public SplitProcessorDescriptor<T> IgnoreMissing(bool? ignoreMissing = true) => this.Assign<bool?>(ignoreMissing, (Action<ISplitProcessor, bool?>) ((a, v) => a.IgnoreMissing = v));

    public SplitProcessorDescriptor<T> PreserveTrailing(bool? preserveTrailing = true) => this.Assign<bool?>(preserveTrailing, (Action<ISplitProcessor, bool?>) ((a, v) => a.PreserveTrailing = v));
  }
}
