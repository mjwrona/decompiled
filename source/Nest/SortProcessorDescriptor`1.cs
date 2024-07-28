// Decompiled with JetBrains decompiler
// Type: Nest.SortProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class SortProcessorDescriptor<T> : 
    ProcessorDescriptorBase<SortProcessorDescriptor<T>, ISortProcessor>,
    ISortProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "sort";

    Nest.Field ISortProcessor.Field { get; set; }

    SortOrder? ISortProcessor.Order { get; set; }

    Nest.Field ISortProcessor.TargetField { get; set; }

    public SortProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<ISortProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public SortProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<ISortProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public SortProcessorDescriptor<T> TargetField(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<ISortProcessor, Nest.Field>) ((a, v) => a.TargetField = v));

    public SortProcessorDescriptor<T> TargetField(Expression<Func<T, object>> objectPath) => this.Assign<Expression<Func<T, object>>>(objectPath, (Action<ISortProcessor, Expression<Func<T, object>>>) ((a, v) => a.TargetField = (Nest.Field) (Expression) v));

    public SortProcessorDescriptor<T> Order(SortOrder? order = SortOrder.Ascending) => this.Assign<SortOrder?>(order, (Action<ISortProcessor, SortOrder?>) ((a, v) => a.Order = v));
  }
}
