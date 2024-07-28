// Decompiled with JetBrains decompiler
// Type: Nest.HasChildQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class HasChildQueryDescriptor<T> : 
    QueryDescriptorBase<HasChildQueryDescriptor<T>, IHasChildQuery>,
    IHasChildQuery,
    IQuery
    where T : class
  {
    public HasChildQueryDescriptor() => this.Self.Type = RelationName.Create<T>();

    protected override bool Conditionless => HasChildQuery.IsConditionless((IHasChildQuery) this);

    bool? IHasChildQuery.IgnoreUnmapped { get; set; }

    IInnerHits IHasChildQuery.InnerHits { get; set; }

    int? IHasChildQuery.MaxChildren { get; set; }

    int? IHasChildQuery.MinChildren { get; set; }

    QueryContainer IHasChildQuery.Query { get; set; }

    ChildScoreMode? IHasChildQuery.ScoreMode { get; set; }

    RelationName IHasChildQuery.Type { get; set; }

    public HasChildQueryDescriptor<T> Query(
      Func<QueryContainerDescriptor<T>, QueryContainer> selector)
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(selector, (Action<IHasChildQuery, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.Query = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }

    public HasChildQueryDescriptor<T> Type(string type) => this.Assign<string>(type, (Action<IHasChildQuery, string>) ((a, v) => a.Type = (RelationName) v));

    public HasChildQueryDescriptor<T> ScoreMode(ChildScoreMode? scoreMode) => this.Assign<ChildScoreMode?>(scoreMode, (Action<IHasChildQuery, ChildScoreMode?>) ((a, v) => a.ScoreMode = v));

    public HasChildQueryDescriptor<T> MinChildren(int? minChildren) => this.Assign<int?>(minChildren, (Action<IHasChildQuery, int?>) ((a, v) => a.MinChildren = v));

    public HasChildQueryDescriptor<T> MaxChildren(int? maxChildren) => this.Assign<int?>(maxChildren, (Action<IHasChildQuery, int?>) ((a, v) => a.MaxChildren = v));

    public HasChildQueryDescriptor<T> InnerHits(Func<InnerHitsDescriptor<T>, IInnerHits> selector = null) => this.Assign<IInnerHits>(selector.InvokeOrDefault<InnerHitsDescriptor<T>, IInnerHits>(new InnerHitsDescriptor<T>()), (Action<IHasChildQuery, IInnerHits>) ((a, v) => a.InnerHits = v));

    public HasChildQueryDescriptor<T> IgnoreUnmapped(bool? ignoreUnmapped = true) => this.Assign<bool?>(ignoreUnmapped, (Action<IHasChildQuery, bool?>) ((a, v) => a.IgnoreUnmapped = v));
  }
}
