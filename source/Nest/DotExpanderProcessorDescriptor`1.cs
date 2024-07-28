// Decompiled with JetBrains decompiler
// Type: Nest.DotExpanderProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class DotExpanderProcessorDescriptor<T> : 
    ProcessorDescriptorBase<DotExpanderProcessorDescriptor<T>, IDotExpanderProcessor>,
    IDotExpanderProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "dot_expander";

    Nest.Field IDotExpanderProcessor.Field { get; set; }

    string IDotExpanderProcessor.Path { get; set; }

    public DotExpanderProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IDotExpanderProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public DotExpanderProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IDotExpanderProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public DotExpanderProcessorDescriptor<T> Path(string path) => this.Assign<string>(path, (Action<IDotExpanderProcessor, string>) ((a, v) => a.Path = v));
  }
}
