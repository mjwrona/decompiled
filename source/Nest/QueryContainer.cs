// Decompiled with JetBrains decompiler
// Type: Nest.QueryContainer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  [JsonFormatter(typeof (QueryContainerFormatter))]
  public class QueryContainer : IQueryContainer, IDescriptor
  {
    private IBoolQuery _bool;
    private IBoostingQuery _boosting;
    private ICommonTermsQuery _commonTerms;
    private IConstantScoreQuery _constantScore;
    private IDisMaxQuery _disMax;
    private IDistanceFeatureQuery _distanceFeature;
    private IExistsQuery _exists;
    private IFunctionScoreQuery _functionScore;
    private IFuzzyQuery _fuzzy;
    private IGeoBoundingBoxQuery _geoBoundingBox;
    private IGeoDistanceQuery _geoDistance;
    private IGeoPolygonQuery _geoPolygon;
    private IGeoShapeQuery _geoShape;
    private IShapeQuery _shape;
    private IHasChildQuery _hasChild;
    private IHasParentQuery _hasParent;
    private IIdsQuery _ids;
    private IIntervalsQuery _intervals;
    private IMatchQuery _match;
    private IMatchAllQuery _matchAllQuery;
    private IMatchBoolPrefixQuery _matchBoolPrefixQuery;
    private IMatchNoneQuery _matchNoneQuery;
    private IMatchPhraseQuery _matchPhrase;
    private IMatchPhrasePrefixQuery _matchPhrasePrefix;
    private IMoreLikeThisQuery _moreLikeThis;
    private IMultiMatchQuery _multiMatch;
    private INestedQuery _nested;
    private IParentIdQuery _parentId;
    private IPercolateQuery _percolate;
    private IPrefixQuery _prefix;
    private IQueryStringQuery _queryString;
    private IRangeQuery _range;
    private IRawQuery _raw;
    private IRegexpQuery _regexp;
    private IScriptQuery _script;
    private IScriptScoreQuery _scriptScore;
    private ISimpleQueryStringQuery _simpleQueryString;
    private ISpanContainingQuery _spanContaining;
    private ISpanFieldMaskingQuery _spanFieldMasking;
    private ISpanFirstQuery _spanFirst;
    private ISpanMultiTermQuery _spanMultiTerm;
    private ISpanNearQuery _spanNear;
    private ISpanNotQuery _spanNot;
    private ISpanOrQuery _spanOr;
    private ISpanTermQuery _spanTerm;
    private ISpanWithinQuery _spanWithin;
    private ITermQuery _term;
    private ITermsQuery _terms;
    private ITermsSetQuery _termsSet;
    private IWildcardQuery _wildcard;
    private IRankFeatureQuery _rankFeature;
    private IPinnedQuery _pinned;
    private ICombinedFieldsQuery _combinedFieldsQuery;

    [IgnoreDataMember]
    private IQueryContainer Self => (IQueryContainer) this;

    [IgnoreDataMember]
    internal IQuery ContainedQuery { get; set; }

    IBoolQuery IQueryContainer.Bool
    {
      get => this._bool;
      set => this._bool = this.Set<IBoolQuery>(value);
    }

    IBoostingQuery IQueryContainer.Boosting
    {
      get => this._boosting;
      set => this._boosting = this.Set<IBoostingQuery>(value);
    }

    ICommonTermsQuery IQueryContainer.CommonTerms
    {
      get => this._commonTerms;
      set => this._commonTerms = this.Set<ICommonTermsQuery>(value);
    }

    IConstantScoreQuery IQueryContainer.ConstantScore
    {
      get => this._constantScore;
      set => this._constantScore = this.Set<IConstantScoreQuery>(value);
    }

    IDisMaxQuery IQueryContainer.DisMax
    {
      get => this._disMax;
      set => this._disMax = this.Set<IDisMaxQuery>(value);
    }

    IDistanceFeatureQuery IQueryContainer.DistanceFeature
    {
      get => this._distanceFeature;
      set => this._distanceFeature = this.Set<IDistanceFeatureQuery>(value);
    }

    IExistsQuery IQueryContainer.Exists
    {
      get => this._exists;
      set => this._exists = this.Set<IExistsQuery>(value);
    }

    IFunctionScoreQuery IQueryContainer.FunctionScore
    {
      get => this._functionScore;
      set => this._functionScore = this.Set<IFunctionScoreQuery>(value);
    }

    IFuzzyQuery IQueryContainer.Fuzzy
    {
      get => this._fuzzy;
      set => this._fuzzy = this.Set<IFuzzyQuery>(value);
    }

    IGeoBoundingBoxQuery IQueryContainer.GeoBoundingBox
    {
      get => this._geoBoundingBox;
      set => this._geoBoundingBox = this.Set<IGeoBoundingBoxQuery>(value);
    }

    IGeoDistanceQuery IQueryContainer.GeoDistance
    {
      get => this._geoDistance;
      set => this._geoDistance = this.Set<IGeoDistanceQuery>(value);
    }

    IGeoPolygonQuery IQueryContainer.GeoPolygon
    {
      get => this._geoPolygon;
      set => this._geoPolygon = this.Set<IGeoPolygonQuery>(value);
    }

    IGeoShapeQuery IQueryContainer.GeoShape
    {
      get => this._geoShape;
      set => this._geoShape = this.Set<IGeoShapeQuery>(value);
    }

    IShapeQuery IQueryContainer.Shape
    {
      get => this._shape;
      set => this._shape = this.Set<IShapeQuery>(value);
    }

    IHasChildQuery IQueryContainer.HasChild
    {
      get => this._hasChild;
      set => this._hasChild = this.Set<IHasChildQuery>(value);
    }

    IHasParentQuery IQueryContainer.HasParent
    {
      get => this._hasParent;
      set => this._hasParent = this.Set<IHasParentQuery>(value);
    }

    IIdsQuery IQueryContainer.Ids
    {
      get => this._ids;
      set => this._ids = this.Set<IIdsQuery>(value);
    }

    IIntervalsQuery IQueryContainer.Intervals
    {
      get => this._intervals;
      set => this._intervals = this.Set<IIntervalsQuery>(value);
    }

    IMatchQuery IQueryContainer.Match
    {
      get => this._match;
      set => this._match = this.Set<IMatchQuery>(value);
    }

    IMatchAllQuery IQueryContainer.MatchAll
    {
      get => this._matchAllQuery;
      set => this._matchAllQuery = this.Set<IMatchAllQuery>(value);
    }

    IMatchBoolPrefixQuery IQueryContainer.MatchBoolPrefix
    {
      get => this._matchBoolPrefixQuery;
      set => this._matchBoolPrefixQuery = this.Set<IMatchBoolPrefixQuery>(value);
    }

    IMatchNoneQuery IQueryContainer.MatchNone
    {
      get => this._matchNoneQuery;
      set => this._matchNoneQuery = this.Set<IMatchNoneQuery>(value);
    }

    IMatchPhraseQuery IQueryContainer.MatchPhrase
    {
      get => this._matchPhrase;
      set => this._matchPhrase = this.Set<IMatchPhraseQuery>(value);
    }

    IMatchPhrasePrefixQuery IQueryContainer.MatchPhrasePrefix
    {
      get => this._matchPhrasePrefix;
      set => this._matchPhrasePrefix = this.Set<IMatchPhrasePrefixQuery>(value);
    }

    IMoreLikeThisQuery IQueryContainer.MoreLikeThis
    {
      get => this._moreLikeThis;
      set => this._moreLikeThis = this.Set<IMoreLikeThisQuery>(value);
    }

    IMultiMatchQuery IQueryContainer.MultiMatch
    {
      get => this._multiMatch;
      set => this._multiMatch = this.Set<IMultiMatchQuery>(value);
    }

    INestedQuery IQueryContainer.Nested
    {
      get => this._nested;
      set => this._nested = this.Set<INestedQuery>(value);
    }

    IParentIdQuery IQueryContainer.ParentId
    {
      get => this._parentId;
      set => this._parentId = this.Set<IParentIdQuery>(value);
    }

    IPercolateQuery IQueryContainer.Percolate
    {
      get => this._percolate;
      set => this._percolate = this.Set<IPercolateQuery>(value);
    }

    IPrefixQuery IQueryContainer.Prefix
    {
      get => this._prefix;
      set => this._prefix = this.Set<IPrefixQuery>(value);
    }

    IQueryStringQuery IQueryContainer.QueryString
    {
      get => this._queryString;
      set => this._queryString = this.Set<IQueryStringQuery>(value);
    }

    IRangeQuery IQueryContainer.Range
    {
      get => this._range;
      set => this._range = this.Set<IRangeQuery>(value);
    }

    IRawQuery IQueryContainer.RawQuery
    {
      get => this._raw;
      set => this._raw = this.Set<IRawQuery>(value);
    }

    IRegexpQuery IQueryContainer.Regexp
    {
      get => this._regexp;
      set => this._regexp = this.Set<IRegexpQuery>(value);
    }

    IScriptQuery IQueryContainer.Script
    {
      get => this._script;
      set => this._script = this.Set<IScriptQuery>(value);
    }

    IScriptScoreQuery IQueryContainer.ScriptScore
    {
      get => this._scriptScore;
      set => this._scriptScore = this.Set<IScriptScoreQuery>(value);
    }

    ISimpleQueryStringQuery IQueryContainer.SimpleQueryString
    {
      get => this._simpleQueryString;
      set => this._simpleQueryString = this.Set<ISimpleQueryStringQuery>(value);
    }

    ISpanContainingQuery IQueryContainer.SpanContaining
    {
      get => this._spanContaining;
      set => this._spanContaining = this.Set<ISpanContainingQuery>(value);
    }

    ISpanFieldMaskingQuery IQueryContainer.SpanFieldMasking
    {
      get => this._spanFieldMasking;
      set => this._spanFieldMasking = this.Set<ISpanFieldMaskingQuery>(value);
    }

    ISpanFirstQuery IQueryContainer.SpanFirst
    {
      get => this._spanFirst;
      set => this._spanFirst = this.Set<ISpanFirstQuery>(value);
    }

    ISpanMultiTermQuery IQueryContainer.SpanMultiTerm
    {
      get => this._spanMultiTerm;
      set => this._spanMultiTerm = this.Set<ISpanMultiTermQuery>(value);
    }

    ISpanNearQuery IQueryContainer.SpanNear
    {
      get => this._spanNear;
      set => this._spanNear = this.Set<ISpanNearQuery>(value);
    }

    ISpanNotQuery IQueryContainer.SpanNot
    {
      get => this._spanNot;
      set => this._spanNot = this.Set<ISpanNotQuery>(value);
    }

    ISpanOrQuery IQueryContainer.SpanOr
    {
      get => this._spanOr;
      set => this._spanOr = this.Set<ISpanOrQuery>(value);
    }

    ISpanTermQuery IQueryContainer.SpanTerm
    {
      get => this._spanTerm;
      set => this._spanTerm = this.Set<ISpanTermQuery>(value);
    }

    ISpanWithinQuery IQueryContainer.SpanWithin
    {
      get => this._spanWithin;
      set => this._spanWithin = this.Set<ISpanWithinQuery>(value);
    }

    ITermQuery IQueryContainer.Term
    {
      get => this._term;
      set => this._term = this.Set<ITermQuery>(value);
    }

    ITermsQuery IQueryContainer.Terms
    {
      get => this._terms;
      set => this._terms = this.Set<ITermsQuery>(value);
    }

    ITermsSetQuery IQueryContainer.TermsSet
    {
      get => this._termsSet;
      set => this._termsSet = this.Set<ITermsSetQuery>(value);
    }

    IWildcardQuery IQueryContainer.Wildcard
    {
      get => this._wildcard;
      set => this._wildcard = this.Set<IWildcardQuery>(value);
    }

    IRankFeatureQuery IQueryContainer.RankFeature
    {
      get => this._rankFeature;
      set => this._rankFeature = this.Set<IRankFeatureQuery>(value);
    }

    IPinnedQuery IQueryContainer.Pinned
    {
      get => this._pinned;
      set => this._pinned = this.Set<IPinnedQuery>(value);
    }

    ICombinedFieldsQuery IQueryContainer.CombinedFields
    {
      get => this._combinedFieldsQuery;
      set => this._combinedFieldsQuery = this.Set<ICombinedFieldsQuery>(value);
    }

    private T Set<T>(T value) where T : IQuery
    {
      this.ContainedQuery = this.ContainedQuery == null ? (IQuery) value : throw new Exception("Cannot assign " + typeof (T).Name + " to QueryContainer. It can only hold a single query and already contains a " + this.ContainedQuery.GetType().Name);
      return value;
    }

    public QueryContainer()
    {
    }

    public QueryContainer(QueryBase query)
      : this()
    {
      if (query == null)
        return;
      if (query.IsStrict && !query.IsWritable)
        throw new ArgumentException("Query is conditionless but strict is turned on");
      query.WrapInContainer((IQueryContainer) this);
    }

    [IgnoreDataMember]
    internal bool HoldsOnlyShouldMusts { get; set; }

    [IgnoreDataMember]
    internal bool IsConditionless => this.Self.IsConditionless;

    [IgnoreDataMember]
    internal bool IsStrict => this.Self.IsStrict;

    [IgnoreDataMember]
    internal bool IsVerbatim => this.Self.IsVerbatim;

    [IgnoreDataMember]
    internal bool IsWritable => this.Self.IsWritable;

    [IgnoreDataMember]
    bool IQueryContainer.IsConditionless
    {
      get
      {
        IQuery containedQuery = this.ContainedQuery;
        return containedQuery == null || containedQuery.Conditionless;
      }
    }

    [IgnoreDataMember]
    bool IQueryContainer.IsStrict { get; set; }

    [IgnoreDataMember]
    bool IQueryContainer.IsVerbatim { get; set; }

    [IgnoreDataMember]
    bool IQueryContainer.IsWritable => this.Self.IsVerbatim || !this.Self.IsConditionless;

    public void Accept(IQueryVisitor visitor)
    {
      if (visitor.Scope == VisitorScope.Unknown)
        visitor.Scope = VisitorScope.Query;
      new QueryWalker().Walk((IQueryContainer) this, visitor);
    }

    public static QueryContainer operator &(
      QueryContainer leftContainer,
      QueryContainer rightContainer)
    {
      return QueryContainer.And(leftContainer, rightContainer);
    }

    internal static QueryContainer And(QueryContainer leftContainer, QueryContainer rightContainer)
    {
      QueryContainer queryContainer;
      return !QueryContainer.IfEitherIsEmptyReturnTheOtherOrEmpty(leftContainer, rightContainer, out queryContainer) ? leftContainer.CombineAsMust(rightContainer) : queryContainer;
    }

    public static QueryContainer operator |(
      QueryContainer leftContainer,
      QueryContainer rightContainer)
    {
      return QueryContainer.Or(leftContainer, rightContainer);
    }

    internal static QueryContainer Or(QueryContainer leftContainer, QueryContainer rightContainer)
    {
      QueryContainer queryContainer;
      return !QueryContainer.IfEitherIsEmptyReturnTheOtherOrEmpty(leftContainer, rightContainer, out queryContainer) ? leftContainer.CombineAsShould(rightContainer) : queryContainer;
    }

    private static bool IfEitherIsEmptyReturnTheOtherOrEmpty(
      QueryContainer leftContainer,
      QueryContainer rightContainer,
      out QueryContainer queryContainer)
    {
      queryContainer = (QueryContainer) null;
      if (leftContainer == null && rightContainer == null)
        return true;
      bool flag1 = leftContainer != null && leftContainer.IsWritable;
      bool flag2 = rightContainer != null && rightContainer.IsWritable;
      if (flag1 & flag2)
        return false;
      if (!flag1 && !flag2)
        return true;
      queryContainer = flag1 ? leftContainer : rightContainer;
      return true;
    }

    public static QueryContainer operator !(QueryContainer queryContainer)
    {
      if (queryContainer == null || !queryContainer.IsWritable)
        return (QueryContainer) null;
      return new QueryContainer((QueryBase) new BoolQuery()
      {
        MustNot = (IEnumerable<QueryContainer>) new QueryContainer[1]
        {
          queryContainer
        }
      });
    }

    public static QueryContainer operator +(QueryContainer queryContainer)
    {
      if (queryContainer == null || !queryContainer.IsWritable)
        return (QueryContainer) null;
      return new QueryContainer((QueryBase) new BoolQuery()
      {
        Filter = (IEnumerable<QueryContainer>) new QueryContainer[1]
        {
          queryContainer
        }
      });
    }

    public static bool operator false(QueryContainer a) => false;

    public static bool operator true(QueryContainer a) => false;

    internal bool ShouldSerialize(IJsonFormatterResolver formatterResolver) => this.IsWritable;
  }
}
