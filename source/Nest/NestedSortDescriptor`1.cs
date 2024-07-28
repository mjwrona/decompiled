// Decompiled with JetBrains decompiler
// Type: Nest.NestedSortDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class NestedSortDescriptor<T> : 
    DescriptorBase<NestedSortDescriptor<T>, INestedSort>,
    INestedSort
    where T : class
  {
    QueryContainer INestedSort.Filter { get; set; }

    INestedSort INestedSort.Nested { get; set; }

    Field INestedSort.Path { get; set; }

    int? INestedSort.MaxChildren { get; set; }

    public NestedSortDescriptor<T> Path(Field path) => this.Assign<Field>(path, (Action<INestedSort, Field>) ((a, v) => a.Path = v));

    public NestedSortDescriptor<T> Path<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<INestedSort, Expression<Func<T, TValue>>>) ((a, v) => a.Path = (Field) (Expression) v));

    public NestedSortDescriptor<T> Filter(
      Func<QueryContainerDescriptor<T>, QueryContainer> filterSelector)
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(filterSelector, (Action<INestedSort, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.Filter = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }

    public NestedSortDescriptor<T> Nested(
      Func<NestedSortDescriptor<T>, INestedSort> filterSelector)
    {
      return this.Assign<Func<NestedSortDescriptor<T>, INestedSort>>(filterSelector, (Action<INestedSort, Func<NestedSortDescriptor<T>, INestedSort>>) ((a, v) => a.Nested = v != null ? v(new NestedSortDescriptor<T>()) : (INestedSort) null));
    }

    public NestedSortDescriptor<T> MaxChildren(int? maxChildren) => this.Assign<int?>(maxChildren, (Action<INestedSort, int?>) ((a, v) => a.MaxChildren = v));
  }
}
