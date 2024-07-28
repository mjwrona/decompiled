// Decompiled with JetBrains decompiler
// Type: Nest.NestedQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class NestedQueryDescriptor<T> : 
    QueryDescriptorBase<NestedQueryDescriptor<T>, INestedQuery>,
    INestedQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => NestedQuery.IsConditionless((INestedQuery) this);

    bool? INestedQuery.IgnoreUnmapped { get; set; }

    IInnerHits INestedQuery.InnerHits { get; set; }

    Field INestedQuery.Path { get; set; }

    QueryContainer INestedQuery.Query { get; set; }

    NestedScoreMode? INestedQuery.ScoreMode { get; set; }

    public NestedQueryDescriptor<T> Query(
      Func<QueryContainerDescriptor<T>, QueryContainer> selector)
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(selector, (Action<INestedQuery, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.Query = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }

    public NestedQueryDescriptor<T> ScoreMode(NestedScoreMode? scoreMode) => this.Assign<NestedScoreMode?>(scoreMode, (Action<INestedQuery, NestedScoreMode?>) ((a, v) => a.ScoreMode = v));

    public NestedQueryDescriptor<T> Path(Field path) => this.Assign<Field>(path, (Action<INestedQuery, Field>) ((a, v) => a.Path = v));

    public NestedQueryDescriptor<T> Path<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<INestedQuery, Expression<Func<T, TValue>>>) ((a, v) => a.Path = (Field) (Expression) v));

    public NestedQueryDescriptor<T> InnerHits(Func<InnerHitsDescriptor<T>, IInnerHits> selector = null) => this.Assign<IInnerHits>(selector.InvokeOrDefault<InnerHitsDescriptor<T>, IInnerHits>(new InnerHitsDescriptor<T>()), (Action<INestedQuery, IInnerHits>) ((a, v) => a.InnerHits = v));

    public NestedQueryDescriptor<T> IgnoreUnmapped(bool? ignoreUnmapped = true) => this.Assign<bool?>(ignoreUnmapped, (Action<INestedQuery, bool?>) ((a, v) => a.IgnoreUnmapped = v));
  }
}
