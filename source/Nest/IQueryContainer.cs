﻿// Decompiled with JetBrains decompiler
// Type: Nest.IQueryContainer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [JsonFormatter(typeof (QueryContainerInterfaceFormatter))]
  public interface IQueryContainer
  {
    [DataMember(Name = "bool")]
    IBoolQuery Bool { get; set; }

    [DataMember(Name = "boosting")]
    IBoostingQuery Boosting { get; set; }

    [DataMember(Name = "common")]
    ICommonTermsQuery CommonTerms { get; set; }

    [DataMember(Name = "constant_score")]
    IConstantScoreQuery ConstantScore { get; set; }

    [DataMember(Name = "dis_max")]
    IDisMaxQuery DisMax { get; set; }

    [DataMember(Name = "exists")]
    IExistsQuery Exists { get; set; }

    [DataMember(Name = "function_score")]
    IFunctionScoreQuery FunctionScore { get; set; }

    [DataMember(Name = "fuzzy")]
    IFuzzyQuery Fuzzy { get; set; }

    [DataMember(Name = "geo_bounding_box")]
    IGeoBoundingBoxQuery GeoBoundingBox { get; set; }

    [DataMember(Name = "geo_distance")]
    IGeoDistanceQuery GeoDistance { get; set; }

    [DataMember(Name = "geo_polygon")]
    IGeoPolygonQuery GeoPolygon { get; set; }

    [DataMember(Name = "geo_shape")]
    IGeoShapeQuery GeoShape { get; set; }

    [DataMember(Name = "shape")]
    IShapeQuery Shape { get; set; }

    [DataMember(Name = "has_child")]
    IHasChildQuery HasChild { get; set; }

    [DataMember(Name = "has_parent")]
    IHasParentQuery HasParent { get; set; }

    [DataMember(Name = "ids")]
    IIdsQuery Ids { get; set; }

    [DataMember(Name = "intervals")]
    IIntervalsQuery Intervals { get; set; }

    [IgnoreDataMember]
    bool IsConditionless { get; }

    [IgnoreDataMember]
    bool IsStrict { get; set; }

    [IgnoreDataMember]
    bool IsVerbatim { get; set; }

    [IgnoreDataMember]
    bool IsWritable { get; }

    [DataMember(Name = "match")]
    IMatchQuery Match { get; set; }

    [DataMember(Name = "match_all")]
    IMatchAllQuery MatchAll { get; set; }

    [DataMember(Name = "match_bool_prefix")]
    IMatchBoolPrefixQuery MatchBoolPrefix { get; set; }

    [DataMember(Name = "match_none")]
    IMatchNoneQuery MatchNone { get; set; }

    [DataMember(Name = "match_phrase")]
    IMatchPhraseQuery MatchPhrase { get; set; }

    [DataMember(Name = "match_phrase_prefix")]
    IMatchPhrasePrefixQuery MatchPhrasePrefix { get; set; }

    [DataMember(Name = "more_like_this")]
    IMoreLikeThisQuery MoreLikeThis { get; set; }

    [DataMember(Name = "multi_match")]
    IMultiMatchQuery MultiMatch { get; set; }

    [DataMember(Name = "nested")]
    INestedQuery Nested { get; set; }

    [DataMember(Name = "parent_id")]
    IParentIdQuery ParentId { get; set; }

    [DataMember(Name = "percolate")]
    IPercolateQuery Percolate { get; set; }

    [DataMember(Name = "prefix")]
    IPrefixQuery Prefix { get; set; }

    [DataMember(Name = "query_string")]
    IQueryStringQuery QueryString { get; set; }

    [DataMember(Name = "range")]
    IRangeQuery Range { get; set; }

    [IgnoreDataMember]
    IRawQuery RawQuery { get; set; }

    [DataMember(Name = "regexp")]
    IRegexpQuery Regexp { get; set; }

    [DataMember(Name = "script")]
    IScriptQuery Script { get; set; }

    [DataMember(Name = "script_score")]
    IScriptScoreQuery ScriptScore { get; set; }

    [DataMember(Name = "simple_query_string")]
    ISimpleQueryStringQuery SimpleQueryString { get; set; }

    [DataMember(Name = "span_containing")]
    ISpanContainingQuery SpanContaining { get; set; }

    [DataMember(Name = "field_masking_span")]
    ISpanFieldMaskingQuery SpanFieldMasking { get; set; }

    [DataMember(Name = "span_first")]
    ISpanFirstQuery SpanFirst { get; set; }

    [DataMember(Name = "span_multi")]
    ISpanMultiTermQuery SpanMultiTerm { get; set; }

    [DataMember(Name = "span_near")]
    ISpanNearQuery SpanNear { get; set; }

    [DataMember(Name = "span_not")]
    ISpanNotQuery SpanNot { get; set; }

    [DataMember(Name = "span_or")]
    ISpanOrQuery SpanOr { get; set; }

    [DataMember(Name = "span_term")]
    ISpanTermQuery SpanTerm { get; set; }

    [DataMember(Name = "span_within")]
    ISpanWithinQuery SpanWithin { get; set; }

    [DataMember(Name = "term")]
    ITermQuery Term { get; set; }

    [DataMember(Name = "terms")]
    ITermsQuery Terms { get; set; }

    [DataMember(Name = "terms_set")]
    ITermsSetQuery TermsSet { get; set; }

    [DataMember(Name = "wildcard")]
    IWildcardQuery Wildcard { get; set; }

    [DataMember(Name = "rank_feature")]
    IRankFeatureQuery RankFeature { get; set; }

    [DataMember(Name = "distance_feature")]
    IDistanceFeatureQuery DistanceFeature { get; set; }

    [DataMember(Name = "pinned")]
    IPinnedQuery Pinned { get; set; }

    [DataMember(Name = "combined_fields")]
    ICombinedFieldsQuery CombinedFields { get; set; }

    void Accept(IQueryVisitor visitor);
  }
}
