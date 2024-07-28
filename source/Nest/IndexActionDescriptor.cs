// Decompiled with JetBrains decompiler
// Type: Nest.IndexActionDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class IndexActionDescriptor : 
    ActionsDescriptorBase<IndexActionDescriptor, IIndexAction>,
    IIndexAction,
    IAction
  {
    public IndexActionDescriptor(string name)
      : base(name)
    {
    }

    protected override ActionType ActionType => ActionType.Index;

    Field IIndexAction.ExecutionTimeField { get; set; }

    IndexName IIndexAction.Index { get; set; }

    Time IIndexAction.Timeout { get; set; }

    public IndexActionDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<IIndexAction, IndexName>) ((a, v) => a.Index = v));

    public IndexActionDescriptor Index<T>() => this.Assign<Type>(typeof (T), (Action<IIndexAction, Type>) ((a, v) => a.Index = (IndexName) v));

    public IndexActionDescriptor ExecutionTimeField(Field field) => this.Assign<Field>(field, (Action<IIndexAction, Field>) ((a, v) => a.ExecutionTimeField = v));

    public IndexActionDescriptor ExecutionTimeField<T, TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IIndexAction, Expression<Func<T, TValue>>>) ((a, v) => a.ExecutionTimeField = (Field) (Expression) v));
    }

    public IndexActionDescriptor Timeout(Time timeout) => this.Assign<Time>(timeout, (Action<IIndexAction, Time>) ((a, v) => a.Timeout = v));
  }
}
