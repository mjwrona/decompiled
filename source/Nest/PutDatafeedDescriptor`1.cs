// Decompiled with JetBrains decompiler
// Type: Nest.PutDatafeedDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class PutDatafeedDescriptor<TDocument> : 
    RequestDescriptorBase<PutDatafeedDescriptor<TDocument>, PutDatafeedRequestParameters, IPutDatafeedRequest>,
    IPutDatafeedRequest,
    IRequest<PutDatafeedRequestParameters>,
    IRequest
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningPutDatafeed;

    public PutDatafeedDescriptor(Id datafeedId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("datafeed_id", (IUrlParameter) datafeedId)))
    {
    }

    [SerializationConstructor]
    protected PutDatafeedDescriptor()
    {
    }

    Id IPutDatafeedRequest.DatafeedId => this.Self.RouteValues.Get<Id>("datafeed_id");

    public PutDatafeedDescriptor<TDocument> AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public PutDatafeedDescriptor<TDocument> ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public PutDatafeedDescriptor<TDocument> IgnoreThrottled(bool? ignorethrottled = true) => this.Qs("ignore_throttled", (object) ignorethrottled);

    public PutDatafeedDescriptor<TDocument> IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    AggregationDictionary IPutDatafeedRequest.Aggregations { get; set; }

    IChunkingConfig IPutDatafeedRequest.ChunkingConfig { get; set; }

    Time IPutDatafeedRequest.Frequency { get; set; }

    Nest.Indices IPutDatafeedRequest.Indices { get; set; }

    Id IPutDatafeedRequest.JobId { get; set; }

    QueryContainer IPutDatafeedRequest.Query { get; set; }

    Time IPutDatafeedRequest.QueryDelay { get; set; }

    IScriptFields IPutDatafeedRequest.ScriptFields { get; set; }

    int? IPutDatafeedRequest.ScrollSize { get; set; }

    int? IPutDatafeedRequest.MaximumEmptySearches { get; set; }

    public PutDatafeedDescriptor<TDocument> Aggregations(
      Func<AggregationContainerDescriptor<TDocument>, IAggregationContainer> aggregationsSelector)
    {
      return this.Assign<AggregationDictionary>(aggregationsSelector(new AggregationContainerDescriptor<TDocument>())?.Aggregations, (Action<IPutDatafeedRequest, AggregationDictionary>) ((a, v) => a.Aggregations = v));
    }

    public PutDatafeedDescriptor<TDocument> ChunkingConfig(
      Func<ChunkingConfigDescriptor, IChunkingConfig> selector)
    {
      return this.Assign<Func<ChunkingConfigDescriptor, IChunkingConfig>>(selector, (Action<IPutDatafeedRequest, Func<ChunkingConfigDescriptor, IChunkingConfig>>) ((a, v) => a.ChunkingConfig = v != null ? v(new ChunkingConfigDescriptor()) : (IChunkingConfig) null));
    }

    public PutDatafeedDescriptor<TDocument> Frequency(Time frequency) => this.Assign<Time>(frequency, (Action<IPutDatafeedRequest, Time>) ((a, v) => a.Frequency = v));

    public PutDatafeedDescriptor<TDocument> Indices(Nest.Indices indices) => this.Assign<Nest.Indices>(indices, (Action<IPutDatafeedRequest, Nest.Indices>) ((a, v) => a.Indices = v));

    public PutDatafeedDescriptor<TDocument> Indices<TOther>() => this.Assign<Type>(typeof (TOther), (Action<IPutDatafeedRequest, Type>) ((a, v) => a.Indices = (Nest.Indices) v));

    public PutDatafeedDescriptor<TDocument> AllIndices() => this.Indices(Nest.Indices.All);

    public PutDatafeedDescriptor<TDocument> JobId(Id jobId) => this.Assign<Id>(jobId, (Action<IPutDatafeedRequest, Id>) ((a, v) => a.JobId = v));

    public PutDatafeedDescriptor<TDocument> Query(
      Func<QueryContainerDescriptor<TDocument>, QueryContainer> query)
    {
      return this.Assign<Func<QueryContainerDescriptor<TDocument>, QueryContainer>>(query, (Action<IPutDatafeedRequest, Func<QueryContainerDescriptor<TDocument>, QueryContainer>>) ((a, v) => a.Query = v != null ? v(new QueryContainerDescriptor<TDocument>()) : (QueryContainer) null));
    }

    public PutDatafeedDescriptor<TDocument> QueryDelay(Time queryDelay) => this.Assign<Time>(queryDelay, (Action<IPutDatafeedRequest, Time>) ((a, v) => a.QueryDelay = v));

    public PutDatafeedDescriptor<TDocument> ScriptFields(
      Func<ScriptFieldsDescriptor, IPromise<IScriptFields>> selector)
    {
      return this.Assign<Func<ScriptFieldsDescriptor, IPromise<IScriptFields>>>(selector, (Action<IPutDatafeedRequest, Func<ScriptFieldsDescriptor, IPromise<IScriptFields>>>) ((a, v) => a.ScriptFields = v != null ? v(new ScriptFieldsDescriptor())?.Value : (IScriptFields) null));
    }

    public PutDatafeedDescriptor<TDocument> ScrollSize(int? scrollSize) => this.Assign<int?>(scrollSize, (Action<IPutDatafeedRequest, int?>) ((a, v) => a.ScrollSize = v));

    public PutDatafeedDescriptor<TDocument> MaximumEmptySearches(int? maximumEmptySearches) => this.Assign<int?>(maximumEmptySearches, (Action<IPutDatafeedRequest, int?>) ((a, v) => a.MaximumEmptySearches = v));
  }
}
