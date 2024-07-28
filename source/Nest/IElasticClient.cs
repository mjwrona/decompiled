// Decompiled with JetBrains decompiler
// Type: Nest.IElasticClient
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Nest.Specification.AsyncSearchApi;
using Nest.Specification.CatApi;
using Nest.Specification.ClusterApi;
using Nest.Specification.CrossClusterReplicationApi;
using Nest.Specification.DanglingIndicesApi;
using Nest.Specification.EnrichApi;
using Nest.Specification.EqlApi;
using Nest.Specification.GraphApi;
using Nest.Specification.IndexLifecycleManagementApi;
using Nest.Specification.IndicesApi;
using Nest.Specification.IngestApi;
using Nest.Specification.LicenseApi;
using Nest.Specification.MachineLearningApi;
using Nest.Specification.MigrationApi;
using Nest.Specification.NodesApi;
using Nest.Specification.RollupApi;
using Nest.Specification.SecurityApi;
using Nest.Specification.SnapshotApi;
using Nest.Specification.SnapshotLifecycleManagementApi;
using Nest.Specification.SqlApi;
using Nest.Specification.TasksApi;
using Nest.Specification.TransformApi;
using Nest.Specification.WatcherApi;
using Nest.Specification.XPackApi;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nest
{
  public interface IElasticClient
  {
    BulkAllObservable<T> BulkAll<T>(
      IEnumerable<T> documents,
      Func<BulkAllDescriptor<T>, IBulkAllRequest<T>> selector,
      CancellationToken cancellationToken = default (CancellationToken))
      where T : class;

    BulkAllObservable<T> BulkAll<T>(IBulkAllRequest<T> request, CancellationToken cancellationToken = default (CancellationToken)) where T : class;

    IObservable<BulkAllResponse> Reindex<TSource, TTarget>(
      Func<TSource, TTarget> mapper,
      Func<ReindexDescriptor<TSource, TTarget>, IReindexRequest<TSource, TTarget>> selector,
      CancellationToken cancellationToken = default (CancellationToken))
      where TSource : class
      where TTarget : class;

    IObservable<BulkAllResponse> Reindex<TSource>(
      Func<ReindexDescriptor<TSource, TSource>, IReindexRequest<TSource, TSource>> selector,
      CancellationToken cancellationToken = default (CancellationToken))
      where TSource : class;

    IObservable<BulkAllResponse> Reindex<TSource, TTarget>(
      IReindexRequest<TSource, TTarget> request,
      CancellationToken cancellationToken = default (CancellationToken))
      where TSource : class
      where TTarget : class;

    IObservable<BulkAllResponse> Reindex<TSource>(
      IReindexRequest<TSource> request,
      CancellationToken cancellationToken = default (CancellationToken))
      where TSource : class;

    IObservable<BulkAllResponse> Reindex<TSource, TTarget>(
      IndexName fromIndex,
      IndexName toIndex,
      Func<TSource, TTarget> mapper,
      Func<QueryContainerDescriptor<TSource>, QueryContainer> selector = null,
      CancellationToken cancellationToken = default (CancellationToken))
      where TSource : class
      where TTarget : class;

    IObservable<BulkAllResponse> Reindex<TSource>(
      IndexName fromIndex,
      IndexName toIndex,
      Func<QueryContainerDescriptor<TSource>, QueryContainer> selector = null,
      CancellationToken cancellationToken = default (CancellationToken))
      where TSource : class;

    IObservable<ScrollAllResponse<T>> ScrollAll<T>(
      Time scrollTime,
      int numberOfSlices,
      Func<ScrollAllDescriptor<T>, IScrollAllRequest> selector = null,
      CancellationToken cancellationToken = default (CancellationToken))
      where T : class;

    IObservable<ScrollAllResponse<T>> ScrollAll<T>(
      IScrollAllRequest request,
      CancellationToken cancellationToken = default (CancellationToken))
      where T : class;

    CreateResponse CreateDocument<TDocument>(TDocument document) where TDocument : class;

    Task<CreateResponse> CreateDocumentAsync<TDocument>(
      TDocument document,
      CancellationToken cancellationToken = default (CancellationToken))
      where TDocument : class;

    IndexResponse IndexDocument<TDocument>(TDocument document) where TDocument : class;

    Task<IndexResponse> IndexDocumentAsync<T>(T document, CancellationToken ct = default (CancellationToken)) where T : class;

    IConnectionSettingsValues ConnectionSettings { get; }

    Inferrer Infer { get; }

    IElasticLowLevelClient LowLevel { get; }

    IElasticsearchSerializer RequestResponseSerializer { get; }

    IElasticsearchSerializer SourceSerializer { get; }

    AsyncSearchNamespace AsyncSearch { get; }

    CatNamespace Cat { get; }

    ClusterNamespace Cluster { get; }

    CrossClusterReplicationNamespace CrossClusterReplication { get; }

    DanglingIndicesNamespace DanglingIndices { get; }

    EnrichNamespace Enrich { get; }

    EqlNamespace Eql { get; }

    GraphNamespace Graph { get; }

    IndexLifecycleManagementNamespace IndexLifecycleManagement { get; }

    IndicesNamespace Indices { get; }

    IngestNamespace Ingest { get; }

    LicenseNamespace License { get; }

    MachineLearningNamespace MachineLearning { get; }

    MigrationNamespace Migration { get; }

    NodesNamespace Nodes { get; }

    BulkResponse Bulk(Func<BulkDescriptor, IBulkRequest> selector);

    Task<BulkResponse> BulkAsync(Func<BulkDescriptor, IBulkRequest> selector, CancellationToken ct = default (CancellationToken));

    BulkResponse Bulk(IBulkRequest request);

    Task<BulkResponse> BulkAsync(IBulkRequest request, CancellationToken ct = default (CancellationToken));

    ClearScrollResponse ClearScroll(
      Func<ClearScrollDescriptor, IClearScrollRequest> selector = null);

    Task<ClearScrollResponse> ClearScrollAsync(
      Func<ClearScrollDescriptor, IClearScrollRequest> selector = null,
      CancellationToken ct = default (CancellationToken));

    ClearScrollResponse ClearScroll(IClearScrollRequest request);

    Task<ClearScrollResponse> ClearScrollAsync(IClearScrollRequest request, CancellationToken ct = default (CancellationToken));

    ClosePointInTimeResponse ClosePointInTime(
      Func<ClosePointInTimeDescriptor, IClosePointInTimeRequest> selector = null);

    Task<ClosePointInTimeResponse> ClosePointInTimeAsync(
      Func<ClosePointInTimeDescriptor, IClosePointInTimeRequest> selector = null,
      CancellationToken ct = default (CancellationToken));

    ClosePointInTimeResponse ClosePointInTime(IClosePointInTimeRequest request);

    Task<ClosePointInTimeResponse> ClosePointInTimeAsync(
      IClosePointInTimeRequest request,
      CancellationToken ct = default (CancellationToken));

    CountResponse Count<TDocument>(
      Func<CountDescriptor<TDocument>, ICountRequest> selector = null)
      where TDocument : class;

    Task<CountResponse> CountAsync<TDocument>(
      Func<CountDescriptor<TDocument>, ICountRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class;

    CountResponse Count(ICountRequest request);

    Task<CountResponse> CountAsync(ICountRequest request, CancellationToken ct = default (CancellationToken));

    CreateResponse Create<TDocument>(
      TDocument document,
      Func<CreateDescriptor<TDocument>, ICreateRequest<TDocument>> selector)
      where TDocument : class;

    Task<CreateResponse> CreateAsync<TDocument>(
      TDocument document,
      Func<CreateDescriptor<TDocument>, ICreateRequest<TDocument>> selector,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class;

    CreateResponse Create<TDocument>(ICreateRequest<TDocument> request) where TDocument : class;

    Task<CreateResponse> CreateAsync<TDocument>(
      ICreateRequest<TDocument> request,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class;

    DeleteResponse Delete<TDocument>(
      DocumentPath<TDocument> id,
      Func<DeleteDescriptor<TDocument>, IDeleteRequest> selector = null)
      where TDocument : class;

    Task<DeleteResponse> DeleteAsync<TDocument>(
      DocumentPath<TDocument> id,
      Func<DeleteDescriptor<TDocument>, IDeleteRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class;

    DeleteResponse Delete(IDeleteRequest request);

    Task<DeleteResponse> DeleteAsync(IDeleteRequest request, CancellationToken ct = default (CancellationToken));

    DeleteByQueryResponse DeleteByQuery<TDocument>(
      Func<DeleteByQueryDescriptor<TDocument>, IDeleteByQueryRequest> selector)
      where TDocument : class;

    Task<DeleteByQueryResponse> DeleteByQueryAsync<TDocument>(
      Func<DeleteByQueryDescriptor<TDocument>, IDeleteByQueryRequest> selector,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class;

    DeleteByQueryResponse DeleteByQuery(IDeleteByQueryRequest request);

    Task<DeleteByQueryResponse> DeleteByQueryAsync(
      IDeleteByQueryRequest request,
      CancellationToken ct = default (CancellationToken));

    ListTasksResponse DeleteByQueryRethrottle(
      TaskId taskId,
      Func<DeleteByQueryRethrottleDescriptor, IDeleteByQueryRethrottleRequest> selector = null);

    Task<ListTasksResponse> DeleteByQueryRethrottleAsync(
      TaskId taskId,
      Func<DeleteByQueryRethrottleDescriptor, IDeleteByQueryRethrottleRequest> selector = null,
      CancellationToken ct = default (CancellationToken));

    ListTasksResponse DeleteByQueryRethrottle(IDeleteByQueryRethrottleRequest request);

    Task<ListTasksResponse> DeleteByQueryRethrottleAsync(
      IDeleteByQueryRethrottleRequest request,
      CancellationToken ct = default (CancellationToken));

    DeleteScriptResponse DeleteScript(
      Id id,
      Func<DeleteScriptDescriptor, IDeleteScriptRequest> selector = null);

    Task<DeleteScriptResponse> DeleteScriptAsync(
      Id id,
      Func<DeleteScriptDescriptor, IDeleteScriptRequest> selector = null,
      CancellationToken ct = default (CancellationToken));

    DeleteScriptResponse DeleteScript(IDeleteScriptRequest request);

    Task<DeleteScriptResponse> DeleteScriptAsync(IDeleteScriptRequest request, CancellationToken ct = default (CancellationToken));

    ExistsResponse DocumentExists<TDocument>(
      DocumentPath<TDocument> id,
      Func<DocumentExistsDescriptor<TDocument>, IDocumentExistsRequest> selector = null)
      where TDocument : class;

    Task<ExistsResponse> DocumentExistsAsync<TDocument>(
      DocumentPath<TDocument> id,
      Func<DocumentExistsDescriptor<TDocument>, IDocumentExistsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class;

    ExistsResponse DocumentExists(IDocumentExistsRequest request);

    Task<ExistsResponse> DocumentExistsAsync(IDocumentExistsRequest request, CancellationToken ct = default (CancellationToken));

    ExistsResponse SourceExists<TDocument>(
      DocumentPath<TDocument> id,
      Func<SourceExistsDescriptor<TDocument>, ISourceExistsRequest> selector = null)
      where TDocument : class;

    Task<ExistsResponse> SourceExistsAsync<TDocument>(
      DocumentPath<TDocument> id,
      Func<SourceExistsDescriptor<TDocument>, ISourceExistsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class;

    ExistsResponse SourceExists(ISourceExistsRequest request);

    Task<ExistsResponse> SourceExistsAsync(ISourceExistsRequest request, CancellationToken ct = default (CancellationToken));

    ExplainResponse<TDocument> Explain<TDocument>(
      DocumentPath<TDocument> id,
      Func<ExplainDescriptor<TDocument>, IExplainRequest> selector = null)
      where TDocument : class;

    Task<ExplainResponse<TDocument>> ExplainAsync<TDocument>(
      DocumentPath<TDocument> id,
      Func<ExplainDescriptor<TDocument>, IExplainRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class;

    ExplainResponse<TDocument> Explain<TDocument>(IExplainRequest request) where TDocument : class;

    Task<ExplainResponse<TDocument>> ExplainAsync<TDocument>(
      IExplainRequest request,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class;

    FieldCapabilitiesResponse FieldCapabilities(
      Nest.Indices index = null,
      Func<FieldCapabilitiesDescriptor, IFieldCapabilitiesRequest> selector = null);

    Task<FieldCapabilitiesResponse> FieldCapabilitiesAsync(
      Nest.Indices index = null,
      Func<FieldCapabilitiesDescriptor, IFieldCapabilitiesRequest> selector = null,
      CancellationToken ct = default (CancellationToken));

    FieldCapabilitiesResponse FieldCapabilities(IFieldCapabilitiesRequest request);

    Task<FieldCapabilitiesResponse> FieldCapabilitiesAsync(
      IFieldCapabilitiesRequest request,
      CancellationToken ct = default (CancellationToken));

    GetResponse<TDocument> Get<TDocument>(
      DocumentPath<TDocument> id,
      Func<GetDescriptor<TDocument>, IGetRequest> selector = null)
      where TDocument : class;

    Task<GetResponse<TDocument>> GetAsync<TDocument>(
      DocumentPath<TDocument> id,
      Func<GetDescriptor<TDocument>, IGetRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class;

    GetResponse<TDocument> Get<TDocument>(IGetRequest request) where TDocument : class;

    Task<GetResponse<TDocument>> GetAsync<TDocument>(IGetRequest request, CancellationToken ct = default (CancellationToken)) where TDocument : class;

    GetScriptResponse GetScript(
      Id id,
      Func<GetScriptDescriptor, IGetScriptRequest> selector = null);

    Task<GetScriptResponse> GetScriptAsync(
      Id id,
      Func<GetScriptDescriptor, IGetScriptRequest> selector = null,
      CancellationToken ct = default (CancellationToken));

    GetScriptResponse GetScript(IGetScriptRequest request);

    Task<GetScriptResponse> GetScriptAsync(IGetScriptRequest request, CancellationToken ct = default (CancellationToken));

    SourceResponse<TDocument> Source<TDocument>(
      DocumentPath<TDocument> id,
      Func<SourceDescriptor<TDocument>, ISourceRequest> selector = null)
      where TDocument : class;

    Task<SourceResponse<TDocument>> SourceAsync<TDocument>(
      DocumentPath<TDocument> id,
      Func<SourceDescriptor<TDocument>, ISourceRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class;

    SourceResponse<TDocument> Source<TDocument>(ISourceRequest request) where TDocument : class;

    Task<SourceResponse<TDocument>> SourceAsync<TDocument>(
      ISourceRequest request,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class;

    IndexResponse Index<TDocument>(
      TDocument document,
      Func<IndexDescriptor<TDocument>, IIndexRequest<TDocument>> selector)
      where TDocument : class;

    Task<IndexResponse> IndexAsync<TDocument>(
      TDocument document,
      Func<IndexDescriptor<TDocument>, IIndexRequest<TDocument>> selector,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class;

    IndexResponse Index<TDocument>(IIndexRequest<TDocument> request) where TDocument : class;

    Task<IndexResponse> IndexAsync<TDocument>(
      IIndexRequest<TDocument> request,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class;

    RootNodeInfoResponse RootNodeInfo(
      Func<RootNodeInfoDescriptor, IRootNodeInfoRequest> selector = null);

    Task<RootNodeInfoResponse> RootNodeInfoAsync(
      Func<RootNodeInfoDescriptor, IRootNodeInfoRequest> selector = null,
      CancellationToken ct = default (CancellationToken));

    RootNodeInfoResponse RootNodeInfo(IRootNodeInfoRequest request);

    Task<RootNodeInfoResponse> RootNodeInfoAsync(IRootNodeInfoRequest request, CancellationToken ct = default (CancellationToken));

    MultiGetResponse MultiGet(
      Func<MultiGetDescriptor, IMultiGetRequest> selector = null);

    Task<MultiGetResponse> MultiGetAsync(
      Func<MultiGetDescriptor, IMultiGetRequest> selector = null,
      CancellationToken ct = default (CancellationToken));

    MultiGetResponse MultiGet(IMultiGetRequest request);

    Task<MultiGetResponse> MultiGetAsync(IMultiGetRequest request, CancellationToken ct = default (CancellationToken));

    MultiSearchResponse MultiSearch(
      Nest.Indices index = null,
      Func<MultiSearchDescriptor, IMultiSearchRequest> selector = null);

    Task<MultiSearchResponse> MultiSearchAsync(
      Nest.Indices index = null,
      Func<MultiSearchDescriptor, IMultiSearchRequest> selector = null,
      CancellationToken ct = default (CancellationToken));

    MultiSearchResponse MultiSearch(IMultiSearchRequest request);

    Task<MultiSearchResponse> MultiSearchAsync(IMultiSearchRequest request, CancellationToken ct = default (CancellationToken));

    MultiSearchResponse MultiSearchTemplate(
      Nest.Indices index = null,
      Func<MultiSearchTemplateDescriptor, IMultiSearchTemplateRequest> selector = null);

    Task<MultiSearchResponse> MultiSearchTemplateAsync(
      Nest.Indices index = null,
      Func<MultiSearchTemplateDescriptor, IMultiSearchTemplateRequest> selector = null,
      CancellationToken ct = default (CancellationToken));

    MultiSearchResponse MultiSearchTemplate(IMultiSearchTemplateRequest request);

    Task<MultiSearchResponse> MultiSearchTemplateAsync(
      IMultiSearchTemplateRequest request,
      CancellationToken ct = default (CancellationToken));

    MultiTermVectorsResponse MultiTermVectors(
      Func<MultiTermVectorsDescriptor, IMultiTermVectorsRequest> selector = null);

    Task<MultiTermVectorsResponse> MultiTermVectorsAsync(
      Func<MultiTermVectorsDescriptor, IMultiTermVectorsRequest> selector = null,
      CancellationToken ct = default (CancellationToken));

    MultiTermVectorsResponse MultiTermVectors(IMultiTermVectorsRequest request);

    Task<MultiTermVectorsResponse> MultiTermVectorsAsync(
      IMultiTermVectorsRequest request,
      CancellationToken ct = default (CancellationToken));

    OpenPointInTimeResponse OpenPointInTime(
      Nest.Indices index,
      Func<OpenPointInTimeDescriptor, IOpenPointInTimeRequest> selector = null);

    Task<OpenPointInTimeResponse> OpenPointInTimeAsync(
      Nest.Indices index,
      Func<OpenPointInTimeDescriptor, IOpenPointInTimeRequest> selector = null,
      CancellationToken ct = default (CancellationToken));

    OpenPointInTimeResponse OpenPointInTime(IOpenPointInTimeRequest request);

    Task<OpenPointInTimeResponse> OpenPointInTimeAsync(
      IOpenPointInTimeRequest request,
      CancellationToken ct = default (CancellationToken));

    PingResponse Ping(Func<PingDescriptor, IPingRequest> selector = null);

    Task<PingResponse> PingAsync(Func<PingDescriptor, IPingRequest> selector = null, CancellationToken ct = default (CancellationToken));

    PingResponse Ping(IPingRequest request);

    Task<PingResponse> PingAsync(IPingRequest request, CancellationToken ct = default (CancellationToken));

    PutScriptResponse PutScript(
      Id id,
      Func<PutScriptDescriptor, IPutScriptRequest> selector);

    Task<PutScriptResponse> PutScriptAsync(
      Id id,
      Func<PutScriptDescriptor, IPutScriptRequest> selector,
      CancellationToken ct = default (CancellationToken));

    PutScriptResponse PutScript(IPutScriptRequest request);

    Task<PutScriptResponse> PutScriptAsync(IPutScriptRequest request, CancellationToken ct = default (CancellationToken));

    ReindexOnServerResponse ReindexOnServer(
      Func<ReindexOnServerDescriptor, IReindexOnServerRequest> selector);

    Task<ReindexOnServerResponse> ReindexOnServerAsync(
      Func<ReindexOnServerDescriptor, IReindexOnServerRequest> selector,
      CancellationToken ct = default (CancellationToken));

    ReindexOnServerResponse ReindexOnServer(IReindexOnServerRequest request);

    Task<ReindexOnServerResponse> ReindexOnServerAsync(
      IReindexOnServerRequest request,
      CancellationToken ct = default (CancellationToken));

    ReindexRethrottleResponse ReindexRethrottle(
      TaskId taskId,
      Func<ReindexRethrottleDescriptor, IReindexRethrottleRequest> selector = null);

    Task<ReindexRethrottleResponse> ReindexRethrottleAsync(
      TaskId taskId,
      Func<ReindexRethrottleDescriptor, IReindexRethrottleRequest> selector = null,
      CancellationToken ct = default (CancellationToken));

    ReindexRethrottleResponse ReindexRethrottle(IReindexRethrottleRequest request);

    Task<ReindexRethrottleResponse> ReindexRethrottleAsync(
      IReindexRethrottleRequest request,
      CancellationToken ct = default (CancellationToken));

    RenderSearchTemplateResponse RenderSearchTemplate(
      Func<RenderSearchTemplateDescriptor, IRenderSearchTemplateRequest> selector = null);

    Task<RenderSearchTemplateResponse> RenderSearchTemplateAsync(
      Func<RenderSearchTemplateDescriptor, IRenderSearchTemplateRequest> selector = null,
      CancellationToken ct = default (CancellationToken));

    RenderSearchTemplateResponse RenderSearchTemplate(IRenderSearchTemplateRequest request);

    Task<RenderSearchTemplateResponse> RenderSearchTemplateAsync(
      IRenderSearchTemplateRequest request,
      CancellationToken ct = default (CancellationToken));

    ExecutePainlessScriptResponse<TResult> ExecutePainlessScript<TResult>(
      Func<ExecutePainlessScriptDescriptor, IExecutePainlessScriptRequest> selector = null);

    Task<ExecutePainlessScriptResponse<TResult>> ExecutePainlessScriptAsync<TResult>(
      Func<ExecutePainlessScriptDescriptor, IExecutePainlessScriptRequest> selector = null,
      CancellationToken ct = default (CancellationToken));

    ExecutePainlessScriptResponse<TResult> ExecutePainlessScript<TResult>(
      IExecutePainlessScriptRequest request);

    Task<ExecutePainlessScriptResponse<TResult>> ExecutePainlessScriptAsync<TResult>(
      IExecutePainlessScriptRequest request,
      CancellationToken ct = default (CancellationToken));

    ISearchResponse<TDocument> Scroll<TInferDocument, TDocument>(
      Time scroll,
      string scrollId,
      Func<ScrollDescriptor<TInferDocument>, IScrollRequest> selector = null)
      where TInferDocument : class
      where TDocument : class;

    Task<ISearchResponse<TDocument>> ScrollAsync<TInferDocument, TDocument>(
      Time scroll,
      string scrollId,
      Func<ScrollDescriptor<TInferDocument>, IScrollRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TInferDocument : class
      where TDocument : class;

    ISearchResponse<TDocument> Scroll<TDocument>(
      Time scroll,
      string scrollId,
      Func<ScrollDescriptor<TDocument>, IScrollRequest> selector = null)
      where TDocument : class;

    Task<ISearchResponse<TDocument>> ScrollAsync<TDocument>(
      Time scroll,
      string scrollId,
      Func<ScrollDescriptor<TDocument>, IScrollRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class;

    ISearchResponse<TDocument> Scroll<TDocument>(IScrollRequest request) where TDocument : class;

    Task<ISearchResponse<TDocument>> ScrollAsync<TDocument>(
      IScrollRequest request,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class;

    ISearchResponse<TDocument> Search<TInferDocument, TDocument>(
      Func<SearchDescriptor<TInferDocument>, ISearchRequest> selector = null)
      where TInferDocument : class
      where TDocument : class;

    Task<ISearchResponse<TDocument>> SearchAsync<TInferDocument, TDocument>(
      Func<SearchDescriptor<TInferDocument>, ISearchRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TInferDocument : class
      where TDocument : class;

    ISearchResponse<TDocument> Search<TDocument>(
      Func<SearchDescriptor<TDocument>, ISearchRequest> selector = null)
      where TDocument : class;

    Task<ISearchResponse<TDocument>> SearchAsync<TDocument>(
      Func<SearchDescriptor<TDocument>, ISearchRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class;

    ISearchResponse<TDocument> Search<TDocument>(ISearchRequest request) where TDocument : class;

    Task<ISearchResponse<TDocument>> SearchAsync<TDocument>(
      ISearchRequest request,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class;

    SearchShardsResponse SearchShards<TDocument>(
      Func<SearchShardsDescriptor<TDocument>, ISearchShardsRequest> selector = null)
      where TDocument : class;

    Task<SearchShardsResponse> SearchShardsAsync<TDocument>(
      Func<SearchShardsDescriptor<TDocument>, ISearchShardsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class;

    SearchShardsResponse SearchShards(ISearchShardsRequest request);

    Task<SearchShardsResponse> SearchShardsAsync(ISearchShardsRequest request, CancellationToken ct = default (CancellationToken));

    ISearchResponse<TDocument> SearchTemplate<TDocument>(
      Func<SearchTemplateDescriptor<TDocument>, ISearchTemplateRequest> selector = null)
      where TDocument : class;

    Task<ISearchResponse<TDocument>> SearchTemplateAsync<TDocument>(
      Func<SearchTemplateDescriptor<TDocument>, ISearchTemplateRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class;

    ISearchResponse<TDocument> SearchTemplate<TDocument>(ISearchTemplateRequest request) where TDocument : class;

    Task<ISearchResponse<TDocument>> SearchTemplateAsync<TDocument>(
      ISearchTemplateRequest request,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class;

    TermVectorsResponse TermVectors<TDocument>(
      Func<TermVectorsDescriptor<TDocument>, ITermVectorsRequest<TDocument>> selector = null)
      where TDocument : class;

    Task<TermVectorsResponse> TermVectorsAsync<TDocument>(
      Func<TermVectorsDescriptor<TDocument>, ITermVectorsRequest<TDocument>> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class;

    TermVectorsResponse TermVectors<TDocument>(ITermVectorsRequest<TDocument> request) where TDocument : class;

    Task<TermVectorsResponse> TermVectorsAsync<TDocument>(
      ITermVectorsRequest<TDocument> request,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class;

    UpdateResponse<TDocument> Update<TDocument, TPartialDocument>(
      DocumentPath<TDocument> id,
      Func<UpdateDescriptor<TDocument, TPartialDocument>, IUpdateRequest<TDocument, TPartialDocument>> selector)
      where TDocument : class
      where TPartialDocument : class;

    Task<UpdateResponse<TDocument>> UpdateAsync<TDocument, TPartialDocument>(
      DocumentPath<TDocument> id,
      Func<UpdateDescriptor<TDocument, TPartialDocument>, IUpdateRequest<TDocument, TPartialDocument>> selector,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
      where TPartialDocument : class;

    UpdateResponse<TDocument> Update<TDocument>(
      DocumentPath<TDocument> id,
      Func<UpdateDescriptor<TDocument, TDocument>, IUpdateRequest<TDocument, TDocument>> selector)
      where TDocument : class;

    Task<UpdateResponse<TDocument>> UpdateAsync<TDocument>(
      DocumentPath<TDocument> id,
      Func<UpdateDescriptor<TDocument, TDocument>, IUpdateRequest<TDocument, TDocument>> selector,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class;

    UpdateResponse<TDocument> Update<TDocument, TPartialDocument>(
      IUpdateRequest<TDocument, TPartialDocument> request)
      where TDocument : class
      where TPartialDocument : class;

    Task<UpdateResponse<TDocument>> UpdateAsync<TDocument, TPartialDocument>(
      IUpdateRequest<TDocument, TPartialDocument> request,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
      where TPartialDocument : class;

    UpdateByQueryResponse UpdateByQuery<TDocument>(
      Func<UpdateByQueryDescriptor<TDocument>, IUpdateByQueryRequest> selector = null)
      where TDocument : class;

    Task<UpdateByQueryResponse> UpdateByQueryAsync<TDocument>(
      Func<UpdateByQueryDescriptor<TDocument>, IUpdateByQueryRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class;

    UpdateByQueryResponse UpdateByQuery(IUpdateByQueryRequest request);

    Task<UpdateByQueryResponse> UpdateByQueryAsync(
      IUpdateByQueryRequest request,
      CancellationToken ct = default (CancellationToken));

    ListTasksResponse UpdateByQueryRethrottle(
      TaskId taskId,
      Func<UpdateByQueryRethrottleDescriptor, IUpdateByQueryRethrottleRequest> selector = null);

    Task<ListTasksResponse> UpdateByQueryRethrottleAsync(
      TaskId taskId,
      Func<UpdateByQueryRethrottleDescriptor, IUpdateByQueryRethrottleRequest> selector = null,
      CancellationToken ct = default (CancellationToken));

    ListTasksResponse UpdateByQueryRethrottle(IUpdateByQueryRethrottleRequest request);

    Task<ListTasksResponse> UpdateByQueryRethrottleAsync(
      IUpdateByQueryRethrottleRequest request,
      CancellationToken ct = default (CancellationToken));

    RollupNamespace Rollup { get; }

    SecurityNamespace Security { get; }

    SnapshotNamespace Snapshot { get; }

    SnapshotLifecycleManagementNamespace SnapshotLifecycleManagement { get; }

    SqlNamespace Sql { get; }

    TasksNamespace Tasks { get; }

    TransformNamespace Transform { get; }

    WatcherNamespace Watcher { get; }

    XPackNamespace XPack { get; }

    PutMappingResponse Map<T>(
      Func<PutMappingDescriptor<T>, IPutMappingRequest> selector)
      where T : class;

    PutMappingResponse Map(IPutMappingRequest request);

    Task<PutMappingResponse> MapAsync<T>(
      Func<PutMappingDescriptor<T>, IPutMappingRequest> selector,
      CancellationToken ct = default (CancellationToken))
      where T : class;

    Task<PutMappingResponse> MapAsync(IPutMappingRequest request, CancellationToken ct = default (CancellationToken));
  }
}
