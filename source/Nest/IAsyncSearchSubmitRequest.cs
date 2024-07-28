// Decompiled with JetBrains decompiler
// Type: Nest.IAsyncSearchSubmitRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.AsyncSearchApi;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [MapsApi("async_search.submit.json")]
  [ReadAs(typeof (AsyncSearchSubmitRequest))]
  public interface IAsyncSearchSubmitRequest : 
    IRequest<AsyncSearchSubmitRequestParameters>,
    IRequest,
    ITypedSearchRequest
  {
    [IgnoreDataMember]
    Indices Index { get; }

    [DataMember(Name = "docvalue_fields")]
    Fields DocValueFields { get; set; }

    [DataMember(Name = "stored_fields")]
    Fields StoredFields { get; set; }

    [DataMember(Name = "track_total_hits")]
    bool? TrackTotalHits { get; set; }

    [DataMember(Name = "aggs")]
    AggregationDictionary Aggregations { get; set; }

    [DataMember(Name = "collapse")]
    IFieldCollapse Collapse { get; set; }

    [DataMember(Name = "explain")]
    bool? Explain { get; set; }

    [DataMember(Name = "fields")]
    Fields Fields { get; set; }

    [DataMember(Name = "from")]
    int? From { get; set; }

    [DataMember(Name = "highlight")]
    IHighlight Highlight { get; set; }

    [DataMember(Name = "indices_boost")]
    [JsonFormatter(typeof (IndicesBoostFormatter))]
    IDictionary<IndexName, double> IndicesBoost { get; set; }

    [DataMember(Name = "min_score")]
    double? MinScore { get; set; }

    [DataMember(Name = "post_filter")]
    QueryContainer PostFilter { get; set; }

    [DataMember(Name = "profile")]
    bool? Profile { get; set; }

    [DataMember(Name = "query")]
    QueryContainer Query { get; set; }

    [DataMember(Name = "rescore")]
    IList<IRescore> Rescore { get; set; }

    [DataMember(Name = "script_fields")]
    IScriptFields ScriptFields { get; set; }

    [DataMember(Name = "search_after")]
    IList<object> SearchAfter { get; set; }

    [DataMember(Name = "size")]
    int? Size { get; set; }

    [DataMember(Name = "sort")]
    IList<ISort> Sort { get; set; }

    [DataMember(Name = "_source")]
    Union<bool, ISourceFilter> Source { get; set; }

    [DataMember(Name = "suggest")]
    ISuggestContainer Suggest { get; set; }

    [DataMember(Name = "terminate_after")]
    long? TerminateAfter { get; set; }

    [DataMember(Name = "timeout")]
    string Timeout { get; set; }

    [DataMember(Name = "track_scores")]
    bool? TrackScores { get; set; }

    [DataMember(Name = "version")]
    bool? Version { get; set; }

    [DataMember(Name = "pit")]
    IPointInTime PointInTime { get; set; }

    [DataMember(Name = "runtime_mappings")]
    IRuntimeFields RuntimeFields { get; set; }
  }
}
