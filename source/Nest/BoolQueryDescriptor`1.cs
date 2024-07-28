// Decompiled with JetBrains decompiler
// Type: Nest.BoolQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class BoolQueryDescriptor<T> : 
    QueryDescriptorBase<BoolQueryDescriptor<T>, IBoolQuery>,
    IBoolQuery,
    IQuery
    where T : class
  {
    private IList<QueryContainer> _filter;
    private IList<QueryContainer> _must;
    private IList<QueryContainer> _mustNot;
    private IList<QueryContainer> _should;

    protected override bool Conditionless => BoolQuery.IsConditionless((IBoolQuery) this);

    IEnumerable<QueryContainer> IBoolQuery.Filter
    {
      get => (IEnumerable<QueryContainer>) this._filter;
      set => this._filter = (IList<QueryContainer>) value.AsInstanceOrToListOrNull<QueryContainer>();
    }

    bool IBoolQuery.Locked => BoolQuery.Locked((IBoolQuery) this);

    Nest.MinimumShouldMatch IBoolQuery.MinimumShouldMatch { get; set; }

    IEnumerable<QueryContainer> IBoolQuery.Must
    {
      get => (IEnumerable<QueryContainer>) this._must;
      set => this._must = (IList<QueryContainer>) value.AsInstanceOrToListOrNull<QueryContainer>();
    }

    IEnumerable<QueryContainer> IBoolQuery.MustNot
    {
      get => (IEnumerable<QueryContainer>) this._mustNot;
      set => this._mustNot = (IList<QueryContainer>) value.AsInstanceOrToListOrNull<QueryContainer>();
    }

    IEnumerable<QueryContainer> IBoolQuery.Should
    {
      get => (IEnumerable<QueryContainer>) this._should;
      set => this._should = (IList<QueryContainer>) value.AsInstanceOrToListOrNull<QueryContainer>();
    }

    bool IBoolQuery.ShouldSerializeShould() => BoolQuery.ShouldSerialize(this.Self.Should);

    bool IBoolQuery.ShouldSerializeMust() => BoolQuery.ShouldSerialize(this.Self.Must);

    bool IBoolQuery.ShouldSerializeMustNot() => BoolQuery.ShouldSerialize(this.Self.MustNot);

    bool IBoolQuery.ShouldSerializeFilter() => BoolQuery.ShouldSerialize(this.Self.Filter);

    public BoolQueryDescriptor<T> MinimumShouldMatch(Nest.MinimumShouldMatch minimumShouldMatches) => this.Assign<Nest.MinimumShouldMatch>(minimumShouldMatches, (Action<IBoolQuery, Nest.MinimumShouldMatch>) ((a, v) => a.MinimumShouldMatch = v));

    public BoolQueryDescriptor<T> Must(
      params Func<QueryContainerDescriptor<T>, QueryContainer>[] queries)
    {
      return this.Assign<List<QueryContainer>>(((IEnumerable<Func<QueryContainerDescriptor<T>, QueryContainer>>) queries).Select<Func<QueryContainerDescriptor<T>, QueryContainer>, QueryContainer>((Func<Func<QueryContainerDescriptor<T>, QueryContainer>, QueryContainer>) (q => q == null ? (QueryContainer) null : q(new QueryContainerDescriptor<T>()))).ToListOrNullIfEmpty<QueryContainer>(), (Action<IBoolQuery, List<QueryContainer>>) ((a, v) => a.Must = (IEnumerable<QueryContainer>) v));
    }

    public BoolQueryDescriptor<T> Must(
      IEnumerable<Func<QueryContainerDescriptor<T>, QueryContainer>> queries)
    {
      return this.Assign<List<QueryContainer>>(queries.Select<Func<QueryContainerDescriptor<T>, QueryContainer>, QueryContainer>((Func<Func<QueryContainerDescriptor<T>, QueryContainer>, QueryContainer>) (q => q == null ? (QueryContainer) null : q(new QueryContainerDescriptor<T>()))).ToListOrNullIfEmpty<QueryContainer>(), (Action<IBoolQuery, List<QueryContainer>>) ((a, v) => a.Must = (IEnumerable<QueryContainer>) v));
    }

    public BoolQueryDescriptor<T> Must(params QueryContainer[] queries) => this.Assign<List<QueryContainer>>(((IEnumerable<QueryContainer>) queries).ToListOrNullIfEmpty<QueryContainer>(), (Action<IBoolQuery, List<QueryContainer>>) ((a, v) => a.Must = (IEnumerable<QueryContainer>) v));

    public BoolQueryDescriptor<T> MustNot(
      params Func<QueryContainerDescriptor<T>, QueryContainer>[] queries)
    {
      return this.Assign<List<QueryContainer>>(((IEnumerable<Func<QueryContainerDescriptor<T>, QueryContainer>>) queries).Select<Func<QueryContainerDescriptor<T>, QueryContainer>, QueryContainer>((Func<Func<QueryContainerDescriptor<T>, QueryContainer>, QueryContainer>) (q => q == null ? (QueryContainer) null : q(new QueryContainerDescriptor<T>()))).ToListOrNullIfEmpty<QueryContainer>(), (Action<IBoolQuery, List<QueryContainer>>) ((a, v) => a.MustNot = (IEnumerable<QueryContainer>) v));
    }

    public BoolQueryDescriptor<T> MustNot(
      IEnumerable<Func<QueryContainerDescriptor<T>, QueryContainer>> queries)
    {
      return this.Assign<List<QueryContainer>>(queries.Select<Func<QueryContainerDescriptor<T>, QueryContainer>, QueryContainer>((Func<Func<QueryContainerDescriptor<T>, QueryContainer>, QueryContainer>) (q => q == null ? (QueryContainer) null : q(new QueryContainerDescriptor<T>()))).ToListOrNullIfEmpty<QueryContainer>(), (Action<IBoolQuery, List<QueryContainer>>) ((a, v) => a.MustNot = (IEnumerable<QueryContainer>) v));
    }

    public BoolQueryDescriptor<T> MustNot(params QueryContainer[] queries) => this.Assign<List<QueryContainer>>(((IEnumerable<QueryContainer>) queries).ToListOrNullIfEmpty<QueryContainer>(), (Action<IBoolQuery, List<QueryContainer>>) ((a, v) => a.MustNot = (IEnumerable<QueryContainer>) v));

    public BoolQueryDescriptor<T> Should(
      params Func<QueryContainerDescriptor<T>, QueryContainer>[] queries)
    {
      return this.Assign<List<QueryContainer>>(((IEnumerable<Func<QueryContainerDescriptor<T>, QueryContainer>>) queries).Select<Func<QueryContainerDescriptor<T>, QueryContainer>, QueryContainer>((Func<Func<QueryContainerDescriptor<T>, QueryContainer>, QueryContainer>) (q => q == null ? (QueryContainer) null : q(new QueryContainerDescriptor<T>()))).ToListOrNullIfEmpty<QueryContainer>(), (Action<IBoolQuery, List<QueryContainer>>) ((a, v) => a.Should = (IEnumerable<QueryContainer>) v));
    }

    public BoolQueryDescriptor<T> Should(
      IEnumerable<Func<QueryContainerDescriptor<T>, QueryContainer>> queries)
    {
      return this.Assign<List<QueryContainer>>(queries.Select<Func<QueryContainerDescriptor<T>, QueryContainer>, QueryContainer>((Func<Func<QueryContainerDescriptor<T>, QueryContainer>, QueryContainer>) (q => q == null ? (QueryContainer) null : q(new QueryContainerDescriptor<T>()))).ToListOrNullIfEmpty<QueryContainer>(), (Action<IBoolQuery, List<QueryContainer>>) ((a, v) => a.Should = (IEnumerable<QueryContainer>) v));
    }

    public BoolQueryDescriptor<T> Should(params QueryContainer[] queries) => this.Assign<List<QueryContainer>>(((IEnumerable<QueryContainer>) queries).ToListOrNullIfEmpty<QueryContainer>(), (Action<IBoolQuery, List<QueryContainer>>) ((a, v) => a.Should = (IEnumerable<QueryContainer>) v));

    public BoolQueryDescriptor<T> Filter(
      params Func<QueryContainerDescriptor<T>, QueryContainer>[] queries)
    {
      return this.Assign<List<QueryContainer>>(((IEnumerable<Func<QueryContainerDescriptor<T>, QueryContainer>>) queries).Select<Func<QueryContainerDescriptor<T>, QueryContainer>, QueryContainer>((Func<Func<QueryContainerDescriptor<T>, QueryContainer>, QueryContainer>) (q => q == null ? (QueryContainer) null : q(new QueryContainerDescriptor<T>()))).ToListOrNullIfEmpty<QueryContainer>(), (Action<IBoolQuery, List<QueryContainer>>) ((a, v) => a.Filter = (IEnumerable<QueryContainer>) v));
    }

    public BoolQueryDescriptor<T> Filter(
      IEnumerable<Func<QueryContainerDescriptor<T>, QueryContainer>> queries)
    {
      return this.Assign<List<QueryContainer>>(queries.Select<Func<QueryContainerDescriptor<T>, QueryContainer>, QueryContainer>((Func<Func<QueryContainerDescriptor<T>, QueryContainer>, QueryContainer>) (q => q == null ? (QueryContainer) null : q(new QueryContainerDescriptor<T>()))).ToListOrNullIfEmpty<QueryContainer>(), (Action<IBoolQuery, List<QueryContainer>>) ((a, v) => a.Filter = (IEnumerable<QueryContainer>) v));
    }

    public BoolQueryDescriptor<T> Filter(params QueryContainer[] queries) => this.Assign<List<QueryContainer>>(((IEnumerable<QueryContainer>) queries).ToListOrNullIfEmpty<QueryContainer>(), (Action<IBoolQuery, List<QueryContainer>>) ((a, v) => a.Filter = (IEnumerable<QueryContainer>) v));
  }
}
