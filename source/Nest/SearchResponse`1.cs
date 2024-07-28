// Decompiled with JetBrains decompiler
// Type: Nest.SearchResponse`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Nest
{
  public class SearchResponse<TDocument> : 
    ResponseBase,
    ISearchResponse<TDocument>,
    IResponse,
    IElasticsearchResponse
    where TDocument : class
  {
    private IReadOnlyCollection<TDocument> _documents;
    private IReadOnlyCollection<FieldValues> _fields;
    private IReadOnlyCollection<IHit<TDocument>> _hits;

    [DataMember(Name = "aggregations")]
    public AggregateDictionary Aggregations { get; internal set; } = AggregateDictionary.Default;

    [DataMember(Name = "_clusters")]
    public ClusterStatistics Clusters { get; internal set; }

    [IgnoreDataMember]
    public IReadOnlyCollection<TDocument> Documents => this._documents ?? (this._documents = (IReadOnlyCollection<TDocument>) this.Hits.Select<IHit<TDocument>, TDocument>((Func<IHit<TDocument>, TDocument>) (h => h.Source)).ToList<TDocument>());

    [IgnoreDataMember]
    public IReadOnlyCollection<FieldValues> Fields => this._fields ?? (this._fields = (IReadOnlyCollection<FieldValues>) this.Hits.Select<IHit<TDocument>, FieldValues>((Func<IHit<TDocument>, FieldValues>) (h => h.Fields)).ToList<FieldValues>());

    [IgnoreDataMember]
    public IReadOnlyCollection<IHit<TDocument>> Hits => this._hits ?? (this._hits = this.HitsMetadata?.Hits ?? EmptyReadOnly<IHit<TDocument>>.Collection);

    [DataMember(Name = "hits")]
    public IHitsMetadata<TDocument> HitsMetadata { get; internal set; }

    [IgnoreDataMember]
    public double MaxScore => ((double?) this.HitsMetadata?.MaxScore).GetValueOrDefault();

    [DataMember(Name = "num_reduce_phases")]
    public long NumberOfReducePhases { get; internal set; }

    [DataMember(Name = "pit_id")]
    public string PointInTimeId { get; internal set; }

    [DataMember(Name = "profile")]
    public Profile Profile { get; internal set; }

    [DataMember(Name = "_scroll_id")]
    public string ScrollId { get; internal set; }

    [DataMember(Name = "_shards")]
    public ShardStatistics Shards { get; internal set; }

    [DataMember(Name = "suggest")]
    public ISuggestDictionary<TDocument> Suggest { get; internal set; } = (ISuggestDictionary<TDocument>) SuggestDictionary<TDocument>.Default;

    [DataMember(Name = "terminated_early")]
    public bool TerminatedEarly { get; internal set; }

    [DataMember(Name = "timed_out")]
    public bool TimedOut { get; internal set; }

    [DataMember(Name = "took")]
    public long Took { get; internal set; }

    [IgnoreDataMember]
    public long Total => this.HitsMetadata?.Total?.Value ?? -1L;
  }
}
