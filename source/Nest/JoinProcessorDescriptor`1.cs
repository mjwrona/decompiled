// Decompiled with JetBrains decompiler
// Type: Nest.JoinProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class JoinProcessorDescriptor<T> : 
    ProcessorDescriptorBase<JoinProcessorDescriptor<T>, IJoinProcessor>,
    IJoinProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "join";

    Nest.Field IJoinProcessor.Field { get; set; }

    Nest.Field IJoinProcessor.TargetField { get; set; }

    string IJoinProcessor.Separator { get; set; }

    public JoinProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IJoinProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public JoinProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IJoinProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public JoinProcessorDescriptor<T> TargetField(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IJoinProcessor, Nest.Field>) ((a, v) => a.TargetField = v));

    public JoinProcessorDescriptor<T> TargetField(Expression<Func<T, object>> objectPath) => this.Assign<Expression<Func<T, object>>>(objectPath, (Action<IJoinProcessor, Expression<Func<T, object>>>) ((a, v) => a.TargetField = (Nest.Field) (Expression) v));

    public JoinProcessorDescriptor<T> Separator(string separator) => this.Assign<string>(separator, (Action<IJoinProcessor, string>) ((a, v) => a.Separator = v));
  }
}
