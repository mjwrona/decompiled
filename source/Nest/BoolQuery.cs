// Decompiled with JetBrains decompiler
// Type: Nest.BoolQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class BoolQuery : QueryBase, IBoolQuery, IQuery
  {
    private IList<QueryContainer> _filter;
    private IList<QueryContainer> _must;
    private IList<QueryContainer> _mustNot;
    private IList<QueryContainer> _should;

    public IEnumerable<QueryContainer> Filter
    {
      get => (IEnumerable<QueryContainer>) this._filter;
      set => this._filter = (IList<QueryContainer>) value.AsInstanceOrToListOrNull<QueryContainer>();
    }

    public MinimumShouldMatch MinimumShouldMatch { get; set; }

    public IEnumerable<QueryContainer> Must
    {
      get => (IEnumerable<QueryContainer>) this._must;
      set => this._must = (IList<QueryContainer>) value.AsInstanceOrToListOrNull<QueryContainer>();
    }

    public IEnumerable<QueryContainer> MustNot
    {
      get => (IEnumerable<QueryContainer>) this._mustNot;
      set => this._mustNot = (IList<QueryContainer>) value.AsInstanceOrToListOrNull<QueryContainer>();
    }

    public IEnumerable<QueryContainer> Should
    {
      get => (IEnumerable<QueryContainer>) this._should;
      set => this._should = (IList<QueryContainer>) value.AsInstanceOrToListOrNull<QueryContainer>();
    }

    protected override bool Conditionless => BoolQuery.IsConditionless((IBoolQuery) this);

    bool IBoolQuery.Locked => BoolQuery.Locked((IBoolQuery) this);

    internal static bool Locked(IBoolQuery q) => !q.Name.IsNullOrEmpty() || q.Boost.HasValue || q.MinimumShouldMatch != null;

    internal override void InternalWrapInContainer(IQueryContainer c) => c.Bool = (IBoolQuery) this;

    internal static bool IsConditionless(IBoolQuery q) => q.Must.NotWritable() && q.MustNot.NotWritable() && q.Should.NotWritable() && q.Filter.NotWritable();

    internal static bool ShouldSerialize(IEnumerable<QueryContainer> queries) => (queries != null ? new bool?(queries.Any<QueryContainer>((Func<QueryContainer, bool>) (qq => qq != null && qq.IsWritable))) : new bool?()).GetValueOrDefault(false);

    bool IBoolQuery.ShouldSerializeShould() => BoolQuery.ShouldSerialize(this.Should);

    bool IBoolQuery.ShouldSerializeMust() => BoolQuery.ShouldSerialize(this.Must);

    bool IBoolQuery.ShouldSerializeMustNot() => BoolQuery.ShouldSerialize(this.MustNot);

    bool IBoolQuery.ShouldSerializeFilter() => BoolQuery.ShouldSerialize(this.Filter);
  }
}
