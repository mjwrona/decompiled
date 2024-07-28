// Decompiled with JetBrains decompiler
// Type: Nest.UpdateDatafeedDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class UpdateDatafeedDescriptor<TDocument> : 
    RequestDescriptorBase<UpdateDatafeedDescriptor<TDocument>, UpdateDatafeedRequestParameters, IUpdateDatafeedRequest>,
    IUpdateDatafeedRequest,
    IRequest<UpdateDatafeedRequestParameters>,
    IRequest
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningUpdateDatafeed;

    public UpdateDatafeedDescriptor(Id datafeedId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("datafeed_id", (IUrlParameter) datafeedId)))
    {
    }

    [SerializationConstructor]
    protected UpdateDatafeedDescriptor()
    {
    }

    Id IUpdateDatafeedRequest.DatafeedId => this.Self.RouteValues.Get<Id>("datafeed_id");

    public UpdateDatafeedDescriptor<TDocument> AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public UpdateDatafeedDescriptor<TDocument> ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public UpdateDatafeedDescriptor<TDocument> IgnoreThrottled(bool? ignorethrottled = true) => this.Qs("ignore_throttled", (object) ignorethrottled);

    public UpdateDatafeedDescriptor<TDocument> IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    AggregationDictionary IUpdateDatafeedRequest.Aggregations { get; set; }

    IChunkingConfig IUpdateDatafeedRequest.ChunkingConfig { get; set; }

    Time IUpdateDatafeedRequest.Frequency { get; set; }

    Nest.Indices IUpdateDatafeedRequest.Indices { get; set; }

    Id IUpdateDatafeedRequest.JobId { get; set; }

    QueryContainer IUpdateDatafeedRequest.Query { get; set; }

    Time IUpdateDatafeedRequest.QueryDelay { get; set; }

    IScriptFields IUpdateDatafeedRequest.ScriptFields { get; set; }

    int? IUpdateDatafeedRequest.ScrollSize { get; set; }

    int? IUpdateDatafeedRequest.MaximumEmptySearches { get; set; }

    public UpdateDatafeedDescriptor<TDocument> Aggregations(
      Func<AggregationContainerDescriptor<TDocument>, IAggregationContainer> aggregationsSelector)
    {
      return this.Assign<AggregationDictionary>(aggregationsSelector(new AggregationContainerDescriptor<TDocument>())?.Aggregations, (Action<IUpdateDatafeedRequest, AggregationDictionary>) ((a, v) => a.Aggregations = v));
    }

    public UpdateDatafeedDescriptor<TDocument> ChunkingConfig(
      Func<ChunkingConfigDescriptor, IChunkingConfig> selector)
    {
      return this.Assign<IChunkingConfig>(selector.InvokeOrDefault<ChunkingConfigDescriptor, IChunkingConfig>(new ChunkingConfigDescriptor()), (Action<IUpdateDatafeedRequest, IChunkingConfig>) ((a, v) => a.ChunkingConfig = v));
    }

    public UpdateDatafeedDescriptor<TDocument> Frequency(Time frequency) => this.Assign<Time>(frequency, (Action<IUpdateDatafeedRequest, Time>) ((a, v) => a.Frequency = v));

    public UpdateDatafeedDescriptor<TDocument> Indices(Nest.Indices indices) => this.Assign<Nest.Indices>(indices, (Action<IUpdateDatafeedRequest, Nest.Indices>) ((a, v) => a.Indices = v));

    public UpdateDatafeedDescriptor<TDocument> Indices<TOther>() => this.Assign<Type>(typeof (TOther), (Action<IUpdateDatafeedRequest, Type>) ((a, v) => a.Indices = (Nest.Indices) v));

    public UpdateDatafeedDescriptor<TDocument> AllIndices() => this.Indices(Nest.Indices.All);

    [Obsolete("As of 7.4.0 the ability to associate a feed with a different job is being deprecated as it adds unnecessary complexity")]
    public UpdateDatafeedDescriptor<TDocument> JobId(Id jobId) => this.Assign<Id>(jobId, (Action<IUpdateDatafeedRequest, Id>) ((a, v) => a.JobId = v));

    public UpdateDatafeedDescriptor<TDocument> Query(
      Func<QueryContainerDescriptor<TDocument>, QueryContainer> query)
    {
      return this.Assign<Func<QueryContainerDescriptor<TDocument>, QueryContainer>>(query, (Action<IUpdateDatafeedRequest, Func<QueryContainerDescriptor<TDocument>, QueryContainer>>) ((a, v) => a.Query = v != null ? v(new QueryContainerDescriptor<TDocument>()) : (QueryContainer) null));
    }

    public UpdateDatafeedDescriptor<TDocument> QueryDelay(Time queryDelay) => this.Assign<Time>(queryDelay, (Action<IUpdateDatafeedRequest, Time>) ((a, v) => a.QueryDelay = v));

    public UpdateDatafeedDescriptor<TDocument> ScriptFields(
      Func<ScriptFieldsDescriptor, IPromise<IScriptFields>> selector)
    {
      return this.Assign<Func<ScriptFieldsDescriptor, IPromise<IScriptFields>>>(selector, (Action<IUpdateDatafeedRequest, Func<ScriptFieldsDescriptor, IPromise<IScriptFields>>>) ((a, v) => a.ScriptFields = v != null ? v(new ScriptFieldsDescriptor())?.Value : (IScriptFields) null));
    }

    public UpdateDatafeedDescriptor<TDocument> ScrollSize(int? scrollSize) => this.Assign<int?>(scrollSize, (Action<IUpdateDatafeedRequest, int?>) ((a, v) => a.ScrollSize = v));

    public UpdateDatafeedDescriptor<TDocument> MaximumEmptySearches(int? maximumEmptySearches) => this.Assign<int?>(maximumEmptySearches, (Action<IUpdateDatafeedRequest, int?>) ((a, v) => a.MaximumEmptySearches = v));
  }
}
