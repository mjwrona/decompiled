// Decompiled with JetBrains decompiler
// Type: Nest.QueryContainerDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class QueryContainerDescriptor<T> : QueryContainer where T : class
  {
    private QueryContainer WrapInContainer<TQuery, TQueryInterface>(
      Func<TQuery, TQueryInterface> create,
      Action<TQueryInterface, IQueryContainer> assign)
      where TQuery : class, TQueryInterface, IQuery, new()
      where TQueryInterface : class, IQuery
    {
      TQueryInterface queryInterface = create.InvokeOrDefault<TQuery, TQueryInterface>(new TQuery());
      QueryContainerDescriptor<T> containerDescriptor = this.ContainedQuery == null ? this : new QueryContainerDescriptor<T>();
      IQueryContainer queryContainer = (IQueryContainer) containerDescriptor;
      queryContainer.IsVerbatim = queryInterface.IsVerbatim;
      queryContainer.IsStrict = queryInterface.IsStrict;
      assign(queryInterface, (IQueryContainer) containerDescriptor);
      containerDescriptor.ContainedQuery = (IQuery) queryInterface;
      if (queryInterface.IsWritable)
        return (QueryContainer) containerDescriptor;
      if (queryInterface.IsStrict)
        throw new ArgumentException("Query is conditionless but strict is turned on");
      return (QueryContainer) null;
    }

    public QueryContainer Raw(string rawJson) => this.WrapInContainer<RawQueryDescriptor, RawQueryDescriptor>((Func<RawQueryDescriptor, RawQueryDescriptor>) (descriptor => descriptor.Raw(rawJson)), (Action<RawQueryDescriptor, IQueryContainer>) ((query, container) => container.RawQuery = (IRawQuery) query));

    public QueryContainer QueryString(
      Func<QueryStringQueryDescriptor<T>, IQueryStringQuery> selector)
    {
      return this.WrapInContainer<QueryStringQueryDescriptor<T>, IQueryStringQuery>(selector, (Action<IQueryStringQuery, IQueryContainer>) ((query, container) => container.QueryString = query));
    }

    public QueryContainer SimpleQueryString(
      Func<SimpleQueryStringQueryDescriptor<T>, ISimpleQueryStringQuery> selector)
    {
      return this.WrapInContainer<SimpleQueryStringQueryDescriptor<T>, ISimpleQueryStringQuery>(selector, (Action<ISimpleQueryStringQuery, IQueryContainer>) ((query, container) => container.SimpleQueryString = query));
    }

    public QueryContainer Terms(
      Func<TermsQueryDescriptor<T>, ITermsQuery> selector)
    {
      return this.WrapInContainer<TermsQueryDescriptor<T>, ITermsQuery>(selector, (Action<ITermsQuery, IQueryContainer>) ((query, container) => container.Terms = query));
    }

    public QueryContainer Fuzzy(
      Func<FuzzyQueryDescriptor<T>, IFuzzyQuery> selector)
    {
      return this.WrapInContainer<FuzzyQueryDescriptor<T>, IFuzzyQuery>(selector, (Action<IFuzzyQuery, IQueryContainer>) ((query, container) => container.Fuzzy = query));
    }

    public QueryContainer FuzzyNumeric(
      Func<FuzzyNumericQueryDescriptor<T>, IFuzzyQuery> selector)
    {
      return this.WrapInContainer<FuzzyNumericQueryDescriptor<T>, IFuzzyQuery>(selector, (Action<IFuzzyQuery, IQueryContainer>) ((query, container) => container.Fuzzy = query));
    }

    public QueryContainer FuzzyDate(
      Func<FuzzyDateQueryDescriptor<T>, IFuzzyQuery> selector)
    {
      return this.WrapInContainer<FuzzyDateQueryDescriptor<T>, IFuzzyQuery>(selector, (Action<IFuzzyQuery, IQueryContainer>) ((query, container) => container.Fuzzy = query));
    }

    public QueryContainer Match(
      Func<MatchQueryDescriptor<T>, IMatchQuery> selector)
    {
      return this.WrapInContainer<MatchQueryDescriptor<T>, IMatchQuery>(selector, (Action<IMatchQuery, IQueryContainer>) ((query, container) => container.Match = query));
    }

    public QueryContainer MatchPhrase(
      Func<MatchPhraseQueryDescriptor<T>, IMatchPhraseQuery> selector)
    {
      return this.WrapInContainer<MatchPhraseQueryDescriptor<T>, IMatchPhraseQuery>(selector, (Action<IMatchPhraseQuery, IQueryContainer>) ((query, container) => container.MatchPhrase = query));
    }

    public QueryContainer MatchBoolPrefix(
      Func<MatchBoolPrefixQueryDescriptor<T>, IMatchBoolPrefixQuery> selector)
    {
      return this.WrapInContainer<MatchBoolPrefixQueryDescriptor<T>, IMatchBoolPrefixQuery>(selector, (Action<IMatchBoolPrefixQuery, IQueryContainer>) ((query, container) => container.MatchBoolPrefix = query));
    }

    public QueryContainer MatchPhrasePrefix(
      Func<MatchPhrasePrefixQueryDescriptor<T>, IMatchPhrasePrefixQuery> selector)
    {
      return this.WrapInContainer<MatchPhrasePrefixQueryDescriptor<T>, IMatchPhrasePrefixQuery>(selector, (Action<IMatchPhrasePrefixQuery, IQueryContainer>) ((query, container) => container.MatchPhrasePrefix = query));
    }

    public QueryContainer MultiMatch(
      Func<MultiMatchQueryDescriptor<T>, IMultiMatchQuery> selector)
    {
      return this.WrapInContainer<MultiMatchQueryDescriptor<T>, IMultiMatchQuery>(selector, (Action<IMultiMatchQuery, IQueryContainer>) ((query, container) => container.MultiMatch = query));
    }

    public QueryContainer Nested(
      Func<NestedQueryDescriptor<T>, INestedQuery> selector)
    {
      return this.WrapInContainer<NestedQueryDescriptor<T>, INestedQuery>(selector, (Action<INestedQuery, IQueryContainer>) ((query, container) => container.Nested = query));
    }

    public QueryContainer Conditionless(
      Func<ConditionlessQueryDescriptor<T>, IConditionlessQuery> selector)
    {
      IConditionlessQuery conditionlessQuery = selector(new ConditionlessQueryDescriptor<T>());
      QueryContainer query = conditionlessQuery?.Query;
      if (query != null)
        return query;
      return conditionlessQuery?.Fallback;
    }

    public QueryContainer Range(
      Func<NumericRangeQueryDescriptor<T>, INumericRangeQuery> selector)
    {
      return this.WrapInContainer<NumericRangeQueryDescriptor<T>, INumericRangeQuery>(selector, (Action<INumericRangeQuery, IQueryContainer>) ((query, container) => container.Range = (IRangeQuery) query));
    }

    public QueryContainer LongRange(
      Func<LongRangeQueryDescriptor<T>, ILongRangeQuery> selector)
    {
      return this.WrapInContainer<LongRangeQueryDescriptor<T>, ILongRangeQuery>(selector, (Action<ILongRangeQuery, IQueryContainer>) ((query, container) => container.Range = (IRangeQuery) query));
    }

    public QueryContainer DateRange(
      Func<DateRangeQueryDescriptor<T>, IDateRangeQuery> selector)
    {
      return this.WrapInContainer<DateRangeQueryDescriptor<T>, IDateRangeQuery>(selector, (Action<IDateRangeQuery, IQueryContainer>) ((query, container) => container.Range = (IRangeQuery) query));
    }

    public QueryContainer TermRange(
      Func<TermRangeQueryDescriptor<T>, ITermRangeQuery> selector)
    {
      return this.WrapInContainer<TermRangeQueryDescriptor<T>, ITermRangeQuery>(selector, (Action<ITermRangeQuery, IQueryContainer>) ((query, container) => container.Range = (IRangeQuery) query));
    }

    public QueryContainer MoreLikeThis(
      Func<MoreLikeThisQueryDescriptor<T>, IMoreLikeThisQuery> selector)
    {
      return this.WrapInContainer<MoreLikeThisQueryDescriptor<T>, IMoreLikeThisQuery>(selector, (Action<IMoreLikeThisQuery, IQueryContainer>) ((query, container) => container.MoreLikeThis = query));
    }

    public QueryContainer GeoShape(
      Func<GeoShapeQueryDescriptor<T>, IGeoShapeQuery> selector)
    {
      return this.WrapInContainer<GeoShapeQueryDescriptor<T>, IGeoShapeQuery>(selector, (Action<IGeoShapeQuery, IQueryContainer>) ((query, container) => container.GeoShape = query));
    }

    public QueryContainer Shape(
      Func<ShapeQueryDescriptor<T>, IShapeQuery> selector)
    {
      return this.WrapInContainer<ShapeQueryDescriptor<T>, IShapeQuery>(selector, (Action<IShapeQuery, IQueryContainer>) ((query, container) => container.Shape = query));
    }

    public QueryContainer GeoPolygon(
      Func<GeoPolygonQueryDescriptor<T>, IGeoPolygonQuery> selector)
    {
      return this.WrapInContainer<GeoPolygonQueryDescriptor<T>, IGeoPolygonQuery>(selector, (Action<IGeoPolygonQuery, IQueryContainer>) ((query, container) => container.GeoPolygon = query));
    }

    public QueryContainer GeoDistance(
      Func<GeoDistanceQueryDescriptor<T>, IGeoDistanceQuery> selector)
    {
      return this.WrapInContainer<GeoDistanceQueryDescriptor<T>, IGeoDistanceQuery>(selector, (Action<IGeoDistanceQuery, IQueryContainer>) ((query, container) => container.GeoDistance = query));
    }

    public QueryContainer GeoBoundingBox(
      Func<GeoBoundingBoxQueryDescriptor<T>, IGeoBoundingBoxQuery> selector)
    {
      return this.WrapInContainer<GeoBoundingBoxQueryDescriptor<T>, IGeoBoundingBoxQuery>(selector, (Action<IGeoBoundingBoxQuery, IQueryContainer>) ((query, container) => container.GeoBoundingBox = query));
    }

    public QueryContainer CommonTerms(
      Func<CommonTermsQueryDescriptor<T>, ICommonTermsQuery> selector)
    {
      return this.WrapInContainer<CommonTermsQueryDescriptor<T>, ICommonTermsQuery>(selector, (Action<ICommonTermsQuery, IQueryContainer>) ((query, container) => container.CommonTerms = query));
    }

    public QueryContainer HasChild<TChild>(
      Func<HasChildQueryDescriptor<TChild>, IHasChildQuery> selector)
      where TChild : class
    {
      return this.WrapInContainer<HasChildQueryDescriptor<TChild>, IHasChildQuery>(selector, (Action<IHasChildQuery, IQueryContainer>) ((query, container) => container.HasChild = query));
    }

    public QueryContainer HasParent<TParent>(
      Func<HasParentQueryDescriptor<TParent>, IHasParentQuery> selector)
      where TParent : class
    {
      return this.WrapInContainer<HasParentQueryDescriptor<TParent>, IHasParentQuery>(selector, (Action<IHasParentQuery, IQueryContainer>) ((query, container) => container.HasParent = query));
    }

    public QueryContainer DisMax(
      Func<DisMaxQueryDescriptor<T>, IDisMaxQuery> selector)
    {
      return this.WrapInContainer<DisMaxQueryDescriptor<T>, IDisMaxQuery>(selector, (Action<IDisMaxQuery, IQueryContainer>) ((query, container) => container.DisMax = query));
    }

    public QueryContainer DistanceFeature(
      Func<DistanceFeatureQueryDescriptor<T>, IDistanceFeatureQuery> selector)
    {
      return this.WrapInContainer<DistanceFeatureQueryDescriptor<T>, IDistanceFeatureQuery>(selector, (Action<IDistanceFeatureQuery, IQueryContainer>) ((query, container) => container.DistanceFeature = query));
    }

    public QueryContainer ConstantScore(
      Func<ConstantScoreQueryDescriptor<T>, IConstantScoreQuery> selector)
    {
      return this.WrapInContainer<ConstantScoreQueryDescriptor<T>, IConstantScoreQuery>(selector, (Action<IConstantScoreQuery, IQueryContainer>) ((query, container) => container.ConstantScore = query));
    }

    public QueryContainer Bool(Func<BoolQueryDescriptor<T>, IBoolQuery> selector) => this.WrapInContainer<BoolQueryDescriptor<T>, IBoolQuery>(selector, (Action<IBoolQuery, IQueryContainer>) ((query, container) => container.Bool = query));

    public QueryContainer Boosting(
      Func<BoostingQueryDescriptor<T>, IBoostingQuery> selector)
    {
      return this.WrapInContainer<BoostingQueryDescriptor<T>, IBoostingQuery>(selector, (Action<IBoostingQuery, IQueryContainer>) ((query, container) => container.Boosting = query));
    }

    public QueryContainer MatchAll(
      Func<MatchAllQueryDescriptor, IMatchAllQuery> selector = null)
    {
      return this.WrapInContainer<MatchAllQueryDescriptor, IMatchAllQuery>(selector, (Action<IMatchAllQuery, IQueryContainer>) ((query, container) => container.MatchAll = query ?? (IMatchAllQuery) new MatchAllQuery()));
    }

    public QueryContainer MatchNone(
      Func<MatchNoneQueryDescriptor, IMatchNoneQuery> selector = null)
    {
      return this.WrapInContainer<MatchNoneQueryDescriptor, IMatchNoneQuery>(selector, (Action<IMatchNoneQuery, IQueryContainer>) ((query, container) => container.MatchNone = query ?? (IMatchNoneQuery) new MatchNoneQuery()));
    }

    public QueryContainer Term<TValue>(
      Expression<Func<T, TValue>> field,
      object value,
      double? boost = null,
      string name = null)
    {
      return this.Term((Func<TermQueryDescriptor<T>, ITermQuery>) (t => (ITermQuery) t.Field<TValue>(field).Value(value).Boost(boost).Name(name)));
    }

    public QueryContainer HasRelationName(Expression<Func<T, JoinField>> field, RelationName value) => this.Term((Func<TermQueryDescriptor<T>, ITermQuery>) (t => (ITermQuery) t.Field<JoinField>(field).Value((object) value)));

    public QueryContainer HasRelationName<TRelation>(Expression<Func<T, JoinField>> field) => this.Term((Func<TermQueryDescriptor<T>, ITermQuery>) (t => (ITermQuery) t.Field<JoinField>(field).Value((object) Infer.Relation<TRelation>())));

    public QueryContainer Term(Field field, object value, double? boost = null, string name = null) => this.Term((Func<TermQueryDescriptor<T>, ITermQuery>) (t => (ITermQuery) t.Field(field).Value(value).Boost(boost).Name(name)));

    public QueryContainer Term(Func<TermQueryDescriptor<T>, ITermQuery> selector) => this.WrapInContainer<TermQueryDescriptor<T>, ITermQuery>(selector, (Action<ITermQuery, IQueryContainer>) ((query, container) => container.Term = query));

    public QueryContainer Wildcard<TValue>(
      Expression<Func<T, TValue>> field,
      string value,
      double? boost = null,
      MultiTermQueryRewrite rewrite = null,
      string name = null)
    {
      return this.Wildcard((Func<WildcardQueryDescriptor<T>, IWildcardQuery>) (t => (IWildcardQuery) t.Field<TValue>(field).Value((object) value).Rewrite(rewrite).Boost(boost).Name(name)));
    }

    public QueryContainer Wildcard(
      Field field,
      string value,
      double? boost = null,
      MultiTermQueryRewrite rewrite = null,
      string name = null)
    {
      return this.Wildcard((Func<WildcardQueryDescriptor<T>, IWildcardQuery>) (t => (IWildcardQuery) t.Field(field).Value((object) value).Rewrite(rewrite).Boost(boost).Name(name)));
    }

    public QueryContainer Wildcard(
      Func<WildcardQueryDescriptor<T>, IWildcardQuery> selector)
    {
      return this.WrapInContainer<WildcardQueryDescriptor<T>, IWildcardQuery>(selector, (Action<IWildcardQuery, IQueryContainer>) ((query, container) => container.Wildcard = query));
    }

    public QueryContainer Prefix<TValue>(
      Expression<Func<T, TValue>> field,
      string value,
      double? boost = null,
      MultiTermQueryRewrite rewrite = null,
      string name = null)
    {
      return this.Prefix((Func<PrefixQueryDescriptor<T>, IPrefixQuery>) (t => (IPrefixQuery) t.Field<TValue>(field).Value((object) value).Boost(boost).Rewrite(rewrite).Name(name)));
    }

    public QueryContainer Prefix(
      Field field,
      string value,
      double? boost = null,
      MultiTermQueryRewrite rewrite = null,
      string name = null)
    {
      return this.Prefix((Func<PrefixQueryDescriptor<T>, IPrefixQuery>) (t => (IPrefixQuery) t.Field(field).Value((object) value).Boost(boost).Rewrite(rewrite).Name(name)));
    }

    public QueryContainer Prefix(
      Func<PrefixQueryDescriptor<T>, IPrefixQuery> selector)
    {
      return this.WrapInContainer<PrefixQueryDescriptor<T>, IPrefixQuery>(selector, (Action<IPrefixQuery, IQueryContainer>) ((query, container) => container.Prefix = query));
    }

    public QueryContainer Ids(Func<IdsQueryDescriptor, IIdsQuery> selector) => this.WrapInContainer<IdsQueryDescriptor, IIdsQuery>(selector, (Action<IIdsQuery, IQueryContainer>) ((query, container) => container.Ids = query));

    public QueryContainer Intervals(
      Func<IntervalsQueryDescriptor<T>, IIntervalsQuery> selector)
    {
      return this.WrapInContainer<IntervalsQueryDescriptor<T>, IIntervalsQuery>(selector, (Action<IIntervalsQuery, IQueryContainer>) ((query, container) => container.Intervals = query));
    }

    public QueryContainer RankFeature(
      Func<RankFeatureQueryDescriptor<T>, IRankFeatureQuery> selector)
    {
      return this.WrapInContainer<RankFeatureQueryDescriptor<T>, IRankFeatureQuery>(selector, (Action<IRankFeatureQuery, IQueryContainer>) ((query, container) => container.RankFeature = query));
    }

    public QueryContainer SpanTerm(
      Func<SpanTermQueryDescriptor<T>, ISpanTermQuery> selector)
    {
      return this.WrapInContainer<SpanTermQueryDescriptor<T>, ISpanTermQuery>(selector, (Action<ISpanTermQuery, IQueryContainer>) ((query, container) => container.SpanTerm = query));
    }

    public QueryContainer SpanFirst(
      Func<SpanFirstQueryDescriptor<T>, ISpanFirstQuery> selector)
    {
      return this.WrapInContainer<SpanFirstQueryDescriptor<T>, ISpanFirstQuery>(selector, (Action<ISpanFirstQuery, IQueryContainer>) ((query, container) => container.SpanFirst = query));
    }

    public QueryContainer SpanNear(
      Func<SpanNearQueryDescriptor<T>, ISpanNearQuery> selector)
    {
      return this.WrapInContainer<SpanNearQueryDescriptor<T>, ISpanNearQuery>(selector, (Action<ISpanNearQuery, IQueryContainer>) ((query, container) => container.SpanNear = query));
    }

    public QueryContainer SpanOr(
      Func<SpanOrQueryDescriptor<T>, ISpanOrQuery> selector)
    {
      return this.WrapInContainer<SpanOrQueryDescriptor<T>, ISpanOrQuery>(selector, (Action<ISpanOrQuery, IQueryContainer>) ((query, container) => container.SpanOr = query));
    }

    public QueryContainer SpanNot(
      Func<SpanNotQueryDescriptor<T>, ISpanNotQuery> selector)
    {
      return this.WrapInContainer<SpanNotQueryDescriptor<T>, ISpanNotQuery>(selector, (Action<ISpanNotQuery, IQueryContainer>) ((query, container) => container.SpanNot = query));
    }

    public QueryContainer SpanMultiTerm(
      Func<SpanMultiTermQueryDescriptor<T>, ISpanMultiTermQuery> selector)
    {
      return this.WrapInContainer<SpanMultiTermQueryDescriptor<T>, ISpanMultiTermQuery>(selector, (Action<ISpanMultiTermQuery, IQueryContainer>) ((query, container) => container.SpanMultiTerm = query));
    }

    public QueryContainer SpanContaining(
      Func<SpanContainingQueryDescriptor<T>, ISpanContainingQuery> selector)
    {
      return this.WrapInContainer<SpanContainingQueryDescriptor<T>, ISpanContainingQuery>(selector, (Action<ISpanContainingQuery, IQueryContainer>) ((query, container) => container.SpanContaining = query));
    }

    public QueryContainer SpanWithin(
      Func<SpanWithinQueryDescriptor<T>, ISpanWithinQuery> selector)
    {
      return this.WrapInContainer<SpanWithinQueryDescriptor<T>, ISpanWithinQuery>(selector, (Action<ISpanWithinQuery, IQueryContainer>) ((query, container) => container.SpanWithin = query));
    }

    public QueryContainer SpanFieldMasking(
      Func<SpanFieldMaskingQueryDescriptor<T>, ISpanFieldMaskingQuery> selector)
    {
      return this.WrapInContainer<SpanFieldMaskingQueryDescriptor<T>, ISpanFieldMaskingQuery>(selector, (Action<ISpanFieldMaskingQuery, IQueryContainer>) ((query, container) => container.SpanFieldMasking = query));
    }

    public QueryContainer Regexp(
      Func<RegexpQueryDescriptor<T>, IRegexpQuery> selector)
    {
      return this.WrapInContainer<RegexpQueryDescriptor<T>, IRegexpQuery>(selector, (Action<IRegexpQuery, IQueryContainer>) ((query, container) => container.Regexp = query));
    }

    public QueryContainer FunctionScore(
      Func<FunctionScoreQueryDescriptor<T>, IFunctionScoreQuery> selector)
    {
      return this.WrapInContainer<FunctionScoreQueryDescriptor<T>, IFunctionScoreQuery>(selector, (Action<IFunctionScoreQuery, IQueryContainer>) ((query, container) => container.FunctionScore = query));
    }

    public QueryContainer Script(
      Func<ScriptQueryDescriptor<T>, IScriptQuery> selector)
    {
      return this.WrapInContainer<ScriptQueryDescriptor<T>, IScriptQuery>(selector, (Action<IScriptQuery, IQueryContainer>) ((query, container) => container.Script = query));
    }

    public QueryContainer ScriptScore(
      Func<ScriptScoreQueryDescriptor<T>, IScriptScoreQuery> selector)
    {
      return this.WrapInContainer<ScriptScoreQueryDescriptor<T>, IScriptScoreQuery>(selector, (Action<IScriptScoreQuery, IQueryContainer>) ((query, container) => container.ScriptScore = query));
    }

    public QueryContainer Exists(
      Func<ExistsQueryDescriptor<T>, IExistsQuery> selector)
    {
      return this.WrapInContainer<ExistsQueryDescriptor<T>, IExistsQuery>(selector, (Action<IExistsQuery, IQueryContainer>) ((query, container) => container.Exists = query));
    }

    public QueryContainer Percolate(
      Func<PercolateQueryDescriptor<T>, IPercolateQuery> selector)
    {
      return this.WrapInContainer<PercolateQueryDescriptor<T>, IPercolateQuery>(selector, (Action<IPercolateQuery, IQueryContainer>) ((query, container) => container.Percolate = query));
    }

    public QueryContainer ParentId(
      Func<ParentIdQueryDescriptor<T>, IParentIdQuery> selector)
    {
      return this.WrapInContainer<ParentIdQueryDescriptor<T>, IParentIdQuery>(selector, (Action<IParentIdQuery, IQueryContainer>) ((query, container) => container.ParentId = query));
    }

    public QueryContainer TermsSet(
      Func<TermsSetQueryDescriptor<T>, ITermsSetQuery> selector)
    {
      return this.WrapInContainer<TermsSetQueryDescriptor<T>, ITermsSetQuery>(selector, (Action<ITermsSetQuery, IQueryContainer>) ((query, container) => container.TermsSet = query));
    }

    public QueryContainer Pinned(
      Func<PinnedQueryDescriptor<T>, IPinnedQuery> selector)
    {
      return this.WrapInContainer<PinnedQueryDescriptor<T>, IPinnedQuery>(selector, (Action<IPinnedQuery, IQueryContainer>) ((query, container) => container.Pinned = query));
    }

    public QueryContainer CombinedFields(
      Func<CombinedFieldsQueryDescriptor<T>, ICombinedFieldsQuery> selector)
    {
      return this.WrapInContainer<CombinedFieldsQueryDescriptor<T>, ICombinedFieldsQuery>(selector, (Action<ICombinedFieldsQuery, IQueryContainer>) ((query, container) => container.CombinedFields = query));
    }
  }
}
