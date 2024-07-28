// Decompiled with JetBrains decompiler
// Type: Nest.Query`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public static class Query<T> where T : class
  {
    public static QueryContainer Bool(Func<BoolQueryDescriptor<T>, IBoolQuery> selector) => new QueryContainerDescriptor<T>().Bool(selector);

    public static QueryContainer Boosting(
      Func<BoostingQueryDescriptor<T>, IBoostingQuery> selector)
    {
      return new QueryContainerDescriptor<T>().Boosting(selector);
    }

    [Obsolete("Deprecated in 7.3.0. Use MatchQuery instead, which skips blocks of documents efficiently, without any configuration, provided that the total number of hits is not tracked.")]
    public static QueryContainer CommonTerms(
      Func<CommonTermsQueryDescriptor<T>, ICommonTermsQuery> selector)
    {
      return new QueryContainerDescriptor<T>().CommonTerms(selector);
    }

    public static QueryContainer Conditionless(
      Func<ConditionlessQueryDescriptor<T>, IConditionlessQuery> selector)
    {
      return new QueryContainerDescriptor<T>().Conditionless(selector);
    }

    public static QueryContainer ConstantScore(
      Func<ConstantScoreQueryDescriptor<T>, IConstantScoreQuery> selector)
    {
      return new QueryContainerDescriptor<T>().ConstantScore(selector);
    }

    public static QueryContainer DateRange(
      Func<DateRangeQueryDescriptor<T>, IDateRangeQuery> selector)
    {
      return new QueryContainerDescriptor<T>().DateRange(selector);
    }

    public static QueryContainer DisMax(
      Func<DisMaxQueryDescriptor<T>, IDisMaxQuery> selector)
    {
      return new QueryContainerDescriptor<T>().DisMax(selector);
    }

    public static QueryContainer DistanceFeature(
      Func<DistanceFeatureQueryDescriptor<T>, IDistanceFeatureQuery> selector)
    {
      return new QueryContainerDescriptor<T>().DistanceFeature(selector);
    }

    public static QueryContainer Exists(
      Func<ExistsQueryDescriptor<T>, IExistsQuery> selector)
    {
      return new QueryContainerDescriptor<T>().Exists(selector);
    }

    public static QueryContainer FunctionScore(
      Func<FunctionScoreQueryDescriptor<T>, IFunctionScoreQuery> selector)
    {
      return new QueryContainerDescriptor<T>().FunctionScore(selector);
    }

    public static QueryContainer Fuzzy(
      Func<FuzzyQueryDescriptor<T>, IFuzzyQuery> selector)
    {
      return new QueryContainerDescriptor<T>().Fuzzy(selector);
    }

    public static QueryContainer GeoBoundingBox(
      Func<GeoBoundingBoxQueryDescriptor<T>, IGeoBoundingBoxQuery> selector)
    {
      return new QueryContainerDescriptor<T>().GeoBoundingBox(selector);
    }

    public static QueryContainer GeoDistance(
      Func<GeoDistanceQueryDescriptor<T>, IGeoDistanceQuery> selector)
    {
      return new QueryContainerDescriptor<T>().GeoDistance(selector);
    }

    public static QueryContainer GeoPolygon(
      Func<GeoPolygonQueryDescriptor<T>, IGeoPolygonQuery> selector)
    {
      return new QueryContainerDescriptor<T>().GeoPolygon(selector);
    }

    public static QueryContainer GeoShape(
      Func<GeoShapeQueryDescriptor<T>, IGeoShapeQuery> selector)
    {
      return new QueryContainerDescriptor<T>().GeoShape(selector);
    }

    public static QueryContainer Shape(
      Func<ShapeQueryDescriptor<T>, IShapeQuery> selector)
    {
      return new QueryContainerDescriptor<T>().Shape(selector);
    }

    public static QueryContainer HasChild<TChild>(
      Func<HasChildQueryDescriptor<TChild>, IHasChildQuery> selector)
      where TChild : class
    {
      return new QueryContainerDescriptor<T>().HasChild<TChild>(selector);
    }

    public static QueryContainer HasParent<TParent>(
      Func<HasParentQueryDescriptor<TParent>, IHasParentQuery> selector)
      where TParent : class
    {
      return new QueryContainerDescriptor<T>().HasParent<TParent>(selector);
    }

    public static QueryContainer Ids(Func<IdsQueryDescriptor, IIdsQuery> selector) => new QueryContainerDescriptor<T>().Ids(selector);

    public static QueryContainer Intervals(
      Func<IntervalsQueryDescriptor<T>, IIntervalsQuery> selector)
    {
      return new QueryContainerDescriptor<T>().Intervals(selector);
    }

    public static QueryContainer Match(
      Func<MatchQueryDescriptor<T>, IMatchQuery> selector)
    {
      return new QueryContainerDescriptor<T>().Match(selector);
    }

    public static QueryContainer MatchAll(
      Func<MatchAllQueryDescriptor, IMatchAllQuery> selector = null)
    {
      return new QueryContainerDescriptor<T>().MatchAll(selector);
    }

    public static QueryContainer MatchBoolPrefix(
      Func<MatchBoolPrefixQueryDescriptor<T>, IMatchBoolPrefixQuery> selector = null)
    {
      return new QueryContainerDescriptor<T>().MatchBoolPrefix(selector);
    }

    public static QueryContainer MatchNone(
      Func<MatchNoneQueryDescriptor, IMatchNoneQuery> selector = null)
    {
      return new QueryContainerDescriptor<T>().MatchNone(selector);
    }

    public static QueryContainer MatchPhrase(
      Func<MatchPhraseQueryDescriptor<T>, IMatchPhraseQuery> selector)
    {
      return new QueryContainerDescriptor<T>().MatchPhrase(selector);
    }

    public static QueryContainer MatchPhrasePrefix(
      Func<MatchPhrasePrefixQueryDescriptor<T>, IMatchPhrasePrefixQuery> selector)
    {
      return new QueryContainerDescriptor<T>().MatchPhrasePrefix(selector);
    }

    public static QueryContainer MoreLikeThis(
      Func<MoreLikeThisQueryDescriptor<T>, IMoreLikeThisQuery> selector)
    {
      return new QueryContainerDescriptor<T>().MoreLikeThis(selector);
    }

    public static QueryContainer MultiMatch(
      Func<MultiMatchQueryDescriptor<T>, IMultiMatchQuery> selector)
    {
      return new QueryContainerDescriptor<T>().MultiMatch(selector);
    }

    public static QueryContainer Nested(
      Func<NestedQueryDescriptor<T>, INestedQuery> selector)
    {
      return new QueryContainerDescriptor<T>().Nested(selector);
    }

    public static QueryContainer ParentId(
      Func<ParentIdQueryDescriptor<T>, IParentIdQuery> selector)
    {
      return new QueryContainerDescriptor<T>().ParentId(selector);
    }

    public static QueryContainer Percolate(
      Func<PercolateQueryDescriptor<T>, IPercolateQuery> selector)
    {
      return new QueryContainerDescriptor<T>().Percolate(selector);
    }

    public static QueryContainer Prefix<TValue>(
      Expression<Func<T, TValue>> fieldDescriptor,
      string value,
      double? boost = null,
      MultiTermQueryRewrite rewrite = null,
      string name = null)
    {
      return new QueryContainerDescriptor<T>().Prefix<TValue>(fieldDescriptor, value, boost, rewrite, name);
    }

    public static QueryContainer Prefix(
      Field field,
      string value,
      double? boost = null,
      MultiTermQueryRewrite rewrite = null,
      string name = null)
    {
      return new QueryContainerDescriptor<T>().Prefix(field, value, boost, rewrite, name);
    }

    public static QueryContainer Prefix(
      Func<PrefixQueryDescriptor<T>, IPrefixQuery> selector)
    {
      return new QueryContainerDescriptor<T>().Prefix(selector);
    }

    public static QueryContainer QueryString(
      Func<QueryStringQueryDescriptor<T>, IQueryStringQuery> selector)
    {
      return new QueryContainerDescriptor<T>().QueryString(selector);
    }

    public static QueryContainer Range(
      Func<NumericRangeQueryDescriptor<T>, INumericRangeQuery> selector)
    {
      return new QueryContainerDescriptor<T>().Range(selector);
    }

    public static QueryContainer LongRange(
      Func<LongRangeQueryDescriptor<T>, ILongRangeQuery> selector)
    {
      return new QueryContainerDescriptor<T>().LongRange(selector);
    }

    public static QueryContainer Regexp(
      Func<RegexpQueryDescriptor<T>, IRegexpQuery> selector)
    {
      return new QueryContainerDescriptor<T>().Regexp(selector);
    }

    public static QueryContainer RankFeature(
      Func<RankFeatureQueryDescriptor<T>, IRankFeatureQuery> selector)
    {
      return new QueryContainerDescriptor<T>().RankFeature(selector);
    }

    public static QueryContainer Script(
      Func<ScriptQueryDescriptor<T>, IScriptQuery> selector)
    {
      return new QueryContainerDescriptor<T>().Script(selector);
    }

    public static QueryContainer ScriptScore(
      Func<ScriptScoreQueryDescriptor<T>, IScriptScoreQuery> selector)
    {
      return new QueryContainerDescriptor<T>().ScriptScore(selector);
    }

    public static QueryContainer SimpleQueryString(
      Func<SimpleQueryStringQueryDescriptor<T>, ISimpleQueryStringQuery> selector)
    {
      return new QueryContainerDescriptor<T>().SimpleQueryString(selector);
    }

    public static QueryContainer SpanContaining(
      Func<SpanContainingQueryDescriptor<T>, ISpanContainingQuery> selector)
    {
      return new QueryContainerDescriptor<T>().SpanContaining(selector);
    }

    public static QueryContainer SpanFirst(
      Func<SpanFirstQueryDescriptor<T>, ISpanFirstQuery> selector)
    {
      return new QueryContainerDescriptor<T>().SpanFirst(selector);
    }

    public static QueryContainer SpanMultiTerm(
      Func<SpanMultiTermQueryDescriptor<T>, ISpanMultiTermQuery> selector)
    {
      return new QueryContainerDescriptor<T>().SpanMultiTerm(selector);
    }

    public static QueryContainer SpanNear(
      Func<SpanNearQueryDescriptor<T>, ISpanNearQuery> selector)
    {
      return new QueryContainerDescriptor<T>().SpanNear(selector);
    }

    public static QueryContainer SpanNot(
      Func<SpanNotQueryDescriptor<T>, ISpanNotQuery> selector)
    {
      return new QueryContainerDescriptor<T>().SpanNot(selector);
    }

    public static QueryContainer SpanOr(
      Func<SpanOrQueryDescriptor<T>, ISpanOrQuery> selector)
    {
      return new QueryContainerDescriptor<T>().SpanOr(selector);
    }

    public static QueryContainer SpanTerm(
      Func<SpanTermQueryDescriptor<T>, ISpanTermQuery> selector)
    {
      return new QueryContainerDescriptor<T>().SpanTerm(selector);
    }

    public static QueryContainer SpanWithin(
      Func<SpanWithinQueryDescriptor<T>, ISpanWithinQuery> selector)
    {
      return new QueryContainerDescriptor<T>().SpanWithin(selector);
    }

    public static QueryContainer SpanFieldMasking(
      Func<SpanFieldMaskingQueryDescriptor<T>, ISpanFieldMaskingQuery> selector)
    {
      return new QueryContainerDescriptor<T>().SpanFieldMasking(selector);
    }

    public static QueryContainer Term<TValue>(
      Expression<Func<T, TValue>> fieldDescriptor,
      object value,
      double? boost = null,
      string name = null)
    {
      return new QueryContainerDescriptor<T>().Term<TValue>(fieldDescriptor, value, boost, name);
    }

    public static QueryContainer Term(Field field, object value, double? boost = null, string name = null) => new QueryContainerDescriptor<T>().Term(field, value, boost, name);

    public static QueryContainer Term(Func<TermQueryDescriptor<T>, ITermQuery> selector) => new QueryContainerDescriptor<T>().Term(selector);

    public static QueryContainer TermRange(
      Func<TermRangeQueryDescriptor<T>, ITermRangeQuery> selector)
    {
      return new QueryContainerDescriptor<T>().TermRange(selector);
    }

    public static QueryContainer Terms(
      Func<TermsQueryDescriptor<T>, ITermsQuery> selector)
    {
      return new QueryContainerDescriptor<T>().Terms(selector);
    }

    public static QueryContainer TermsSet(
      Func<TermsSetQueryDescriptor<T>, ITermsSetQuery> selector)
    {
      return new QueryContainerDescriptor<T>().TermsSet(selector);
    }

    public static QueryContainer Wildcard<TValue>(
      Expression<Func<T, TValue>> fieldDescriptor,
      string value,
      double? boost = null,
      MultiTermQueryRewrite rewrite = null,
      string name = null)
    {
      return new QueryContainerDescriptor<T>().Wildcard<TValue>(fieldDescriptor, value, boost, rewrite, name);
    }

    public static QueryContainer Wildcard(
      Field field,
      string value,
      double? boost = null,
      MultiTermQueryRewrite rewrite = null,
      string name = null)
    {
      return new QueryContainerDescriptor<T>().Wildcard(field, value, boost, rewrite, name);
    }

    public static QueryContainer Wildcard(
      Func<WildcardQueryDescriptor<T>, IWildcardQuery> selector)
    {
      return new QueryContainerDescriptor<T>().Wildcard(selector);
    }

    public static QueryContainer Pinned(
      Func<PinnedQueryDescriptor<T>, IPinnedQuery> selector)
    {
      return new QueryContainerDescriptor<T>().Pinned(selector);
    }

    public static QueryContainer CombinedFields(
      Func<CombinedFieldsQueryDescriptor<T>, ICombinedFieldsQuery> selector)
    {
      return new QueryContainerDescriptor<T>().CombinedFields(selector);
    }
  }
}
