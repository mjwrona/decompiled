// Decompiled with JetBrains decompiler
// Type: Nest.HasParentQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class HasParentQueryDescriptor<T> : 
    QueryDescriptorBase<HasParentQueryDescriptor<T>, IHasParentQuery>,
    IHasParentQuery,
    IQuery
    where T : class
  {
    public HasParentQueryDescriptor() => this.Self.ParentType = RelationName.Create<T>();

    protected override bool Conditionless => HasParentQuery.IsConditionless((IHasParentQuery) this);

    bool? IHasParentQuery.IgnoreUnmapped { get; set; }

    IInnerHits IHasParentQuery.InnerHits { get; set; }

    RelationName IHasParentQuery.ParentType { get; set; }

    QueryContainer IHasParentQuery.Query { get; set; }

    bool? IHasParentQuery.Score { get; set; }

    public HasParentQueryDescriptor<T> Query(
      Func<QueryContainerDescriptor<T>, QueryContainer> selector)
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(selector, (Action<IHasParentQuery, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.Query = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }

    public HasParentQueryDescriptor<T> ParentType(string type) => this.Assign<string>(type, (Action<IHasParentQuery, string>) ((a, v) => a.ParentType = (RelationName) v));

    public HasParentQueryDescriptor<T> Score(bool? score = true) => this.Assign<bool?>(score, (Action<IHasParentQuery, bool?>) ((a, v) => a.Score = v));

    public HasParentQueryDescriptor<T> InnerHits(Func<InnerHitsDescriptor<T>, IInnerHits> selector = null) => this.Assign<IInnerHits>(selector.InvokeOrDefault<InnerHitsDescriptor<T>, IInnerHits>(new InnerHitsDescriptor<T>()), (Action<IHasParentQuery, IInnerHits>) ((a, v) => a.InnerHits = v));

    public HasParentQueryDescriptor<T> IgnoreUnmapped(bool? ignoreUnmapped = true) => this.Assign<bool?>(ignoreUnmapped, (Action<IHasParentQuery, bool?>) ((a, v) => a.IgnoreUnmapped = v));
  }
}
