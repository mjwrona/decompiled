// Decompiled with JetBrains decompiler
// Type: Nest.ElasticClient
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
  public class ElasticClient : IElasticClient
  {
    public BulkAllObservable<T> BulkAll<T>(
      IEnumerable<T> documents,
      Func<BulkAllDescriptor<T>, IBulkAllRequest<T>> selector,
      CancellationToken cancellationToken = default (CancellationToken))
      where T : class
    {
      return this.BulkAll<T>(selector.InvokeOrDefault<BulkAllDescriptor<T>, IBulkAllRequest<T>>(new BulkAllDescriptor<T>(documents)), cancellationToken);
    }

    public BulkAllObservable<T> BulkAll<T>(
      IBulkAllRequest<T> request,
      CancellationToken cancellationToken = default (CancellationToken))
      where T : class
    {
      return new BulkAllObservable<T>((IElasticClient) this, request, cancellationToken);
    }

    public IObservable<BulkAllResponse> Reindex<TSource>(
      Func<ReindexDescriptor<TSource, TSource>, IReindexRequest<TSource, TSource>> selector,
      CancellationToken cancellationToken = default (CancellationToken))
      where TSource : class
    {
      return this.Reindex<TSource, TSource>(selector.InvokeOrDefault<ReindexDescriptor<TSource, TSource>, IReindexRequest<TSource, TSource>>(new ReindexDescriptor<TSource, TSource>((Func<TSource, TSource>) (s => s))), new CancellationToken());
    }

    public IObservable<BulkAllResponse> Reindex<TSource, TTarget>(
      Func<TSource, TTarget> mapper,
      Func<ReindexDescriptor<TSource, TTarget>, IReindexRequest<TSource, TTarget>> selector,
      CancellationToken cancellationToken = default (CancellationToken))
      where TSource : class
      where TTarget : class
    {
      return this.Reindex<TSource, TTarget>(selector.InvokeOrDefault<ReindexDescriptor<TSource, TTarget>, IReindexRequest<TSource, TTarget>>(new ReindexDescriptor<TSource, TTarget>(mapper)), new CancellationToken());
    }

    public IObservable<BulkAllResponse> Reindex<TSource>(
      IReindexRequest<TSource> request,
      CancellationToken cancellationToken = default (CancellationToken))
      where TSource : class
    {
      return this.Reindex<TSource, TSource>((IReindexRequest<TSource, TSource>) request, cancellationToken);
    }

    public IObservable<BulkAllResponse> Reindex<TSource, TTarget>(
      IReindexRequest<TSource, TTarget> request,
      CancellationToken cancellationToken = default (CancellationToken))
      where TSource : class
      where TTarget : class
    {
      if (request.ScrollAll == null)
        throw new ArgumentException("ScrollAll property must be set in order to get the source of a Reindex operation");
      if (request.BulkAll == null)
        throw new ArgumentException("BulkAll property must set in order to get the target of a Reindex operation");
      return (IObservable<BulkAllResponse>) new ReindexObservable<TSource, TTarget>((IElasticClient) this, this.ConnectionSettings, request, cancellationToken);
    }

    public IObservable<BulkAllResponse> Reindex<TSource, TTarget>(
      IndexName fromIndex,
      IndexName toIndex,
      Func<TSource, TTarget> mapper,
      Func<QueryContainerDescriptor<TSource>, QueryContainer> selector = null,
      CancellationToken cancellationToken = default (CancellationToken))
      where TSource : class
      where TTarget : class
    {
      return this.Reindex<TSource, TTarget>(mapper, ElasticClient.SimplifiedReindexer<TSource, TTarget>(fromIndex, toIndex, selector), cancellationToken);
    }

    public IObservable<BulkAllResponse> Reindex<TSource>(
      IndexName fromIndex,
      IndexName toIndex,
      Func<QueryContainerDescriptor<TSource>, QueryContainer> selector = null,
      CancellationToken cancellationToken = default (CancellationToken))
      where TSource : class
    {
      return this.Reindex<TSource, TSource>((Func<TSource, TSource>) (s => s), ElasticClient.SimplifiedReindexer<TSource, TSource>(fromIndex, toIndex, selector), cancellationToken);
    }

    private static Func<ReindexDescriptor<TSource, TTarget>, IReindexRequest<TSource, TTarget>> SimplifiedReindexer<TSource, TTarget>(
      IndexName fromIndex,
      IndexName toIndex,
      Func<QueryContainerDescriptor<TSource>, QueryContainer> selector)
      where TSource : class
      where TTarget : class
    {
      return (Func<ReindexDescriptor<TSource, TTarget>, IReindexRequest<TSource, TTarget>>) (r => (IReindexRequest<TSource, TTarget>) r.ScrollAll((Time) "1m", -1, (Func<ScrollAllDescriptor<TSource>, IScrollAllRequest>) (search => (IScrollAllRequest) search.Search((Func<SearchDescriptor<TSource>, ISearchRequest>) (ss => (ISearchRequest) ss.Size(new int?(CoordinatedRequestDefaults.ReindexScrollSize)).Index((Nest.Indices) fromIndex).Query(selector))))).BulkAll((Func<BulkAllDescriptor<IHitMetadata<TTarget>>, IBulkAllRequest<IHitMetadata<TTarget>>>) (b => (IBulkAllRequest<IHitMetadata<TTarget>>) b.Index(toIndex))));
    }

    public IObservable<ScrollAllResponse<T>> ScrollAll<T>(
      Time scrollTime,
      int numberOfSlices,
      Func<ScrollAllDescriptor<T>, IScrollAllRequest> selector = null,
      CancellationToken cancellationToken = default (CancellationToken))
      where T : class
    {
      return this.ScrollAll<T>(selector.InvokeOrDefault<ScrollAllDescriptor<T>, IScrollAllRequest>(new ScrollAllDescriptor<T>(scrollTime, numberOfSlices)), cancellationToken);
    }

    public IObservable<ScrollAllResponse<T>> ScrollAll<T>(
      IScrollAllRequest request,
      CancellationToken cancellationToken = default (CancellationToken))
      where T : class
    {
      return (IObservable<ScrollAllResponse<T>>) new ScrollAllObservable<T>((IElasticClient) this, request, cancellationToken);
    }

    public CreateResponse CreateDocument<TDocument>(TDocument document) where TDocument : class => this.Create<TDocument>(document, (Func<CreateDescriptor<TDocument>, ICreateRequest<TDocument>>) null);

    public Task<CreateResponse> CreateDocumentAsync<TDocument>(
      TDocument document,
      CancellationToken cancellationToken = default (CancellationToken))
      where TDocument : class
    {
      return this.CreateAsync<TDocument>(document, (Func<CreateDescriptor<TDocument>, ICreateRequest<TDocument>>) null, cancellationToken);
    }

    public IndexResponse IndexDocument<TDocument>(TDocument document) where TDocument : class => this.Index<TDocument>(document, (Func<IndexDescriptor<TDocument>, IIndexRequest<TDocument>>) (s => (IIndexRequest<TDocument>) s));

    public Task<IndexResponse> IndexDocumentAsync<TDocument>(
      TDocument document,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.IndexAsync<TDocument>(document, (Func<IndexDescriptor<TDocument>, IIndexRequest<TDocument>>) (s => (IIndexRequest<TDocument>) s), ct);
    }

    public ElasticClient()
      : this((IConnectionSettingsValues) new Nest.ConnectionSettings(new Uri("http://localhost:9200")))
    {
    }

    public ElasticClient(Uri uri)
      : this((IConnectionSettingsValues) new Nest.ConnectionSettings(uri))
    {
    }

    public ElasticClient(string cloudId, BasicAuthenticationCredentials credentials)
      : this((IConnectionSettingsValues) new Nest.ConnectionSettings(cloudId, credentials))
    {
    }

    public ElasticClient(string cloudId, ApiKeyAuthenticationCredentials credentials)
      : this((IConnectionSettingsValues) new Nest.ConnectionSettings(cloudId, credentials))
    {
    }

    public ElasticClient(IConnectionSettingsValues connectionSettings)
      : this((ITransport<IConnectionSettingsValues>) new Elasticsearch.Net.Transport<IConnectionSettingsValues>(connectionSettings ?? (IConnectionSettingsValues) new Nest.ConnectionSettings()))
    {
    }

    public ElasticClient(ITransport<IConnectionSettingsValues> transport)
    {
      transport.ThrowIfNull<ITransport<IConnectionSettingsValues>>(nameof (transport));
      transport.Settings.ThrowIfNull<IConnectionSettingsValues>("Settings");
      transport.Settings.RequestResponseSerializer.ThrowIfNull<IElasticsearchSerializer>(nameof (RequestResponseSerializer));
      transport.Settings.Inferrer.ThrowIfNull<Inferrer>("Inferrer");
      this.Transport = transport;
      this.LowLevel = (IElasticLowLevelClient) new ElasticLowLevelClient((ITransport<IConnectionConfigurationValues>) this.Transport);
      this.SetupNamespaces();
    }

    private void SetupNamespaces()
    {
      this.AsyncSearch = new AsyncSearchNamespace(this);
      this.Cat = new CatNamespace(this);
      this.Cluster = new ClusterNamespace(this);
      this.CrossClusterReplication = new CrossClusterReplicationNamespace(this);
      this.DanglingIndices = new DanglingIndicesNamespace(this);
      this.Enrich = new EnrichNamespace(this);
      this.Eql = new EqlNamespace(this);
      this.Graph = new GraphNamespace(this);
      this.IndexLifecycleManagement = new IndexLifecycleManagementNamespace(this);
      this.Indices = new IndicesNamespace(this);
      this.Ingest = new IngestNamespace(this);
      this.License = new LicenseNamespace(this);
      this.MachineLearning = new MachineLearningNamespace(this);
      this.Migration = new MigrationNamespace(this);
      this.Nodes = new NodesNamespace(this);
      this.Rollup = new RollupNamespace(this);
      this.Security = new SecurityNamespace(this);
      this.Snapshot = new SnapshotNamespace(this);
      this.SnapshotLifecycleManagement = new SnapshotLifecycleManagementNamespace(this);
      this.Sql = new SqlNamespace(this);
      this.Tasks = new TasksNamespace(this);
      this.Transform = new TransformNamespace(this);
      this.Watcher = new WatcherNamespace(this);
      this.XPack = new XPackNamespace(this);
    }

    public IConnectionSettingsValues ConnectionSettings => this.Transport.Settings;

    public Inferrer Infer => this.Transport.Settings.Inferrer;

    public IElasticLowLevelClient LowLevel { get; }

    public IElasticsearchSerializer RequestResponseSerializer => this.Transport.Settings.RequestResponseSerializer;

    public IElasticsearchSerializer SourceSerializer => this.Transport.Settings.SourceSerializer;

    private ITransport<IConnectionSettingsValues> Transport { get; }

    internal TResponse DoRequest<TRequest, TResponse>(
      TRequest p,
      IRequestParameters parameters,
      Action<IConnectionConfigurationValues, IRequestConfiguration> forceConfiguration = null)
      where TRequest : class, IRequest
      where TResponse : class, IElasticsearchResponse, new()
    {
      if (forceConfiguration != null)
        ElasticClient.ForceConfiguration((IRequest) p, (IConnectionConfigurationValues) this.ConnectionSettings, forceConfiguration);
      if (p.ContentType != null)
        this.ForceContentType<TRequest>(p, p.ContentType);
      string url = p.GetUrl(this.ConnectionSettings);
      SerializableData<TRequest> data = p.HttpMethod == HttpMethod.GET || p.HttpMethod == HttpMethod.HEAD || !parameters.SupportsBody ? (SerializableData<TRequest>) null : new SerializableData<TRequest>(p);
      return this.LowLevel.DoRequest<TResponse>(p.HttpMethod, url, (PostData) data, parameters);
    }

    internal Task<TResponse> DoRequestAsync<TRequest, TResponse>(
      TRequest p,
      IRequestParameters parameters,
      CancellationToken ct,
      Action<IConnectionConfigurationValues, IRequestConfiguration> forceConfiguration = null)
      where TRequest : class, IRequest
      where TResponse : class, IElasticsearchResponse, new()
    {
      if (forceConfiguration != null)
        ElasticClient.ForceConfiguration((IRequest) p, (IConnectionConfigurationValues) this.ConnectionSettings, forceConfiguration);
      if (p.ContentType != null)
        this.ForceContentType<TRequest>(p, p.ContentType);
      string url = p.GetUrl(this.ConnectionSettings);
      SerializableData<TRequest> data = p.HttpMethod == HttpMethod.GET || p.HttpMethod == HttpMethod.HEAD || !parameters.SupportsBody ? (SerializableData<TRequest>) null : new SerializableData<TRequest>(p);
      return this.LowLevel.DoRequestAsync<TResponse>(p.HttpMethod, url, ct, (PostData) data, parameters);
    }

    private static void ForceConfiguration(
      IRequest request,
      IConnectionConfigurationValues settings,
      Action<IConnectionConfigurationValues, IRequestConfiguration> forceConfiguration)
    {
      if (forceConfiguration == null)
        return;
      IRequestConfiguration requestConfiguration = request.RequestParameters.RequestConfiguration ?? (IRequestConfiguration) new RequestConfiguration();
      forceConfiguration(settings, requestConfiguration);
      request.RequestParameters.RequestConfiguration = requestConfiguration;
    }

    private void ForceContentType<TRequest>(TRequest request, string contentType) where TRequest : class, IRequest
    {
      IRequestConfiguration requestConfiguration = request.RequestParameters.RequestConfiguration ?? (IRequestConfiguration) new RequestConfiguration();
      requestConfiguration.Accept = contentType;
      requestConfiguration.ContentType = contentType;
      request.RequestParameters.RequestConfiguration = requestConfiguration;
    }

    internal static void ForceJson(
      IConnectionConfigurationValues settings,
      IRequestConfiguration requestConfiguration)
    {
      requestConfiguration.Accept = RequestData.DefaultJsonBasedOnConfigurationSettings(settings);
      requestConfiguration.ContentType = RequestData.DefaultJsonBasedOnConfigurationSettings(settings);
    }

    internal static void ForceTextPlain(
      IConnectionConfigurationValues settings,
      IRequestConfiguration requestConfiguration)
    {
      requestConfiguration.Accept = "text/plain";
      requestConfiguration.ContentType = "text/plain";
    }

    internal IRequestParameters ResponseBuilder(
      SourceRequestParameters parameters,
      CustomResponseBuilderBase builder)
    {
      parameters.CustomResponseBuilder = builder;
      return (IRequestParameters) parameters;
    }

    public AsyncSearchNamespace AsyncSearch { get; private set; }

    public CatNamespace Cat { get; private set; }

    public ClusterNamespace Cluster { get; private set; }

    public CrossClusterReplicationNamespace CrossClusterReplication { get; private set; }

    public DanglingIndicesNamespace DanglingIndices { get; private set; }

    public EnrichNamespace Enrich { get; private set; }

    public EqlNamespace Eql { get; private set; }

    public GraphNamespace Graph { get; private set; }

    public IndexLifecycleManagementNamespace IndexLifecycleManagement { get; private set; }

    public IndicesNamespace Indices { get; private set; }

    public IngestNamespace Ingest { get; private set; }

    public LicenseNamespace License { get; private set; }

    public MachineLearningNamespace MachineLearning { get; private set; }

    public MigrationNamespace Migration { get; private set; }

    public NodesNamespace Nodes { get; private set; }

    public RollupNamespace Rollup { get; private set; }

    public SecurityNamespace Security { get; private set; }

    public SnapshotNamespace Snapshot { get; private set; }

    public SnapshotLifecycleManagementNamespace SnapshotLifecycleManagement { get; private set; }

    public SqlNamespace Sql { get; private set; }

    public TasksNamespace Tasks { get; private set; }

    public TransformNamespace Transform { get; private set; }

    public WatcherNamespace Watcher { get; private set; }

    public XPackNamespace XPack { get; private set; }

    public BulkResponse Bulk(Func<BulkDescriptor, IBulkRequest> selector) => this.Bulk(selector.InvokeOrDefault<BulkDescriptor, IBulkRequest>(new BulkDescriptor()));

    public Task<BulkResponse> BulkAsync(
      Func<BulkDescriptor, IBulkRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.BulkAsync(selector.InvokeOrDefault<BulkDescriptor, IBulkRequest>(new BulkDescriptor()), ct);
    }

    public BulkResponse Bulk(IBulkRequest request) => this.DoRequest<IBulkRequest, BulkResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<BulkResponse> BulkAsync(IBulkRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IBulkRequest, BulkResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public ClearScrollResponse ClearScroll(
      Func<ClearScrollDescriptor, IClearScrollRequest> selector = null)
    {
      return this.ClearScroll(selector.InvokeOrDefault<ClearScrollDescriptor, IClearScrollRequest>(new ClearScrollDescriptor()));
    }

    public Task<ClearScrollResponse> ClearScrollAsync(
      Func<ClearScrollDescriptor, IClearScrollRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ClearScrollAsync(selector.InvokeOrDefault<ClearScrollDescriptor, IClearScrollRequest>(new ClearScrollDescriptor()), ct);
    }

    public ClearScrollResponse ClearScroll(IClearScrollRequest request) => this.DoRequest<IClearScrollRequest, ClearScrollResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ClearScrollResponse> ClearScrollAsync(
      IClearScrollRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IClearScrollRequest, ClearScrollResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ClosePointInTimeResponse ClosePointInTime(
      Func<ClosePointInTimeDescriptor, IClosePointInTimeRequest> selector = null)
    {
      return this.ClosePointInTime(selector.InvokeOrDefault<ClosePointInTimeDescriptor, IClosePointInTimeRequest>(new ClosePointInTimeDescriptor()));
    }

    public Task<ClosePointInTimeResponse> ClosePointInTimeAsync(
      Func<ClosePointInTimeDescriptor, IClosePointInTimeRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ClosePointInTimeAsync(selector.InvokeOrDefault<ClosePointInTimeDescriptor, IClosePointInTimeRequest>(new ClosePointInTimeDescriptor()), ct);
    }

    public ClosePointInTimeResponse ClosePointInTime(IClosePointInTimeRequest request) => this.DoRequest<IClosePointInTimeRequest, ClosePointInTimeResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ClosePointInTimeResponse> ClosePointInTimeAsync(
      IClosePointInTimeRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IClosePointInTimeRequest, ClosePointInTimeResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public CountResponse Count<TDocument>(
      Func<CountDescriptor<TDocument>, ICountRequest> selector = null)
      where TDocument : class
    {
      return this.Count(selector.InvokeOrDefault<CountDescriptor<TDocument>, ICountRequest>(new CountDescriptor<TDocument>()));
    }

    public Task<CountResponse> CountAsync<TDocument>(
      Func<CountDescriptor<TDocument>, ICountRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.CountAsync(selector.InvokeOrDefault<CountDescriptor<TDocument>, ICountRequest>(new CountDescriptor<TDocument>()), ct);
    }

    public CountResponse Count(ICountRequest request) => this.DoRequest<ICountRequest, CountResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<CountResponse> CountAsync(ICountRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<ICountRequest, CountResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public CreateResponse Create<TDocument>(
      TDocument document,
      Func<CreateDescriptor<TDocument>, ICreateRequest<TDocument>> selector)
      where TDocument : class
    {
      return this.Create<TDocument>(selector.InvokeOrDefault<CreateDescriptor<TDocument>, ICreateRequest<TDocument>>(new CreateDescriptor<TDocument>(document)));
    }

    public Task<CreateResponse> CreateAsync<TDocument>(
      TDocument document,
      Func<CreateDescriptor<TDocument>, ICreateRequest<TDocument>> selector,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.CreateAsync<TDocument>(selector.InvokeOrDefault<CreateDescriptor<TDocument>, ICreateRequest<TDocument>>(new CreateDescriptor<TDocument>(document)), ct);
    }

    public CreateResponse Create<TDocument>(ICreateRequest<TDocument> request) where TDocument : class => this.DoRequest<ICreateRequest<TDocument>, CreateResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<CreateResponse> CreateAsync<TDocument>(
      ICreateRequest<TDocument> request,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.DoRequestAsync<ICreateRequest<TDocument>, CreateResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public DeleteResponse Delete<TDocument>(
      DocumentPath<TDocument> id,
      Func<DeleteDescriptor<TDocument>, IDeleteRequest> selector = null)
      where TDocument : class
    {
      return this.Delete(selector.InvokeOrDefault<DeleteDescriptor<TDocument>, IDeleteRequest>(new DeleteDescriptor<TDocument>((object) id != null ? id.Document : default (TDocument), id?.Self?.Index, id?.Self?.Id)));
    }

    public Task<DeleteResponse> DeleteAsync<TDocument>(
      DocumentPath<TDocument> id,
      Func<DeleteDescriptor<TDocument>, IDeleteRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.DeleteAsync(selector.InvokeOrDefault<DeleteDescriptor<TDocument>, IDeleteRequest>(new DeleteDescriptor<TDocument>((object) id != null ? id.Document : default (TDocument), id?.Self?.Index, id?.Self?.Id)), ct);
    }

    public DeleteResponse Delete(IDeleteRequest request) => this.DoRequest<IDeleteRequest, DeleteResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteResponse> DeleteAsync(IDeleteRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IDeleteRequest, DeleteResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public DeleteByQueryResponse DeleteByQuery<TDocument>(
      Func<DeleteByQueryDescriptor<TDocument>, IDeleteByQueryRequest> selector)
      where TDocument : class
    {
      return this.DeleteByQuery(selector.InvokeOrDefault<DeleteByQueryDescriptor<TDocument>, IDeleteByQueryRequest>(new DeleteByQueryDescriptor<TDocument>()));
    }

    public Task<DeleteByQueryResponse> DeleteByQueryAsync<TDocument>(
      Func<DeleteByQueryDescriptor<TDocument>, IDeleteByQueryRequest> selector,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.DeleteByQueryAsync(selector.InvokeOrDefault<DeleteByQueryDescriptor<TDocument>, IDeleteByQueryRequest>(new DeleteByQueryDescriptor<TDocument>()), ct);
    }

    public DeleteByQueryResponse DeleteByQuery(IDeleteByQueryRequest request) => this.DoRequest<IDeleteByQueryRequest, DeleteByQueryResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteByQueryResponse> DeleteByQueryAsync(
      IDeleteByQueryRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteByQueryRequest, DeleteByQueryResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ListTasksResponse DeleteByQueryRethrottle(
      TaskId taskId,
      Func<DeleteByQueryRethrottleDescriptor, IDeleteByQueryRethrottleRequest> selector = null)
    {
      return this.DeleteByQueryRethrottle(selector.InvokeOrDefault<DeleteByQueryRethrottleDescriptor, IDeleteByQueryRethrottleRequest>(new DeleteByQueryRethrottleDescriptor(taskId)));
    }

    public Task<ListTasksResponse> DeleteByQueryRethrottleAsync(
      TaskId taskId,
      Func<DeleteByQueryRethrottleDescriptor, IDeleteByQueryRethrottleRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteByQueryRethrottleAsync(selector.InvokeOrDefault<DeleteByQueryRethrottleDescriptor, IDeleteByQueryRethrottleRequest>(new DeleteByQueryRethrottleDescriptor(taskId)), ct);
    }

    public ListTasksResponse DeleteByQueryRethrottle(IDeleteByQueryRethrottleRequest request) => this.DoRequest<IDeleteByQueryRethrottleRequest, ListTasksResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ListTasksResponse> DeleteByQueryRethrottleAsync(
      IDeleteByQueryRethrottleRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteByQueryRethrottleRequest, ListTasksResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public DeleteScriptResponse DeleteScript(
      Id id,
      Func<DeleteScriptDescriptor, IDeleteScriptRequest> selector = null)
    {
      return this.DeleteScript(selector.InvokeOrDefault<DeleteScriptDescriptor, IDeleteScriptRequest>(new DeleteScriptDescriptor(id)));
    }

    public Task<DeleteScriptResponse> DeleteScriptAsync(
      Id id,
      Func<DeleteScriptDescriptor, IDeleteScriptRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DeleteScriptAsync(selector.InvokeOrDefault<DeleteScriptDescriptor, IDeleteScriptRequest>(new DeleteScriptDescriptor(id)), ct);
    }

    public DeleteScriptResponse DeleteScript(IDeleteScriptRequest request) => this.DoRequest<IDeleteScriptRequest, DeleteScriptResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<DeleteScriptResponse> DeleteScriptAsync(
      IDeleteScriptRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDeleteScriptRequest, DeleteScriptResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ExistsResponse DocumentExists<TDocument>(
      DocumentPath<TDocument> id,
      Func<DocumentExistsDescriptor<TDocument>, IDocumentExistsRequest> selector = null)
      where TDocument : class
    {
      return this.DocumentExists(selector.InvokeOrDefault<DocumentExistsDescriptor<TDocument>, IDocumentExistsRequest>(new DocumentExistsDescriptor<TDocument>((object) id != null ? id.Document : default (TDocument), id?.Self?.Index, id?.Self?.Id)));
    }

    public Task<ExistsResponse> DocumentExistsAsync<TDocument>(
      DocumentPath<TDocument> id,
      Func<DocumentExistsDescriptor<TDocument>, IDocumentExistsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.DocumentExistsAsync(selector.InvokeOrDefault<DocumentExistsDescriptor<TDocument>, IDocumentExistsRequest>(new DocumentExistsDescriptor<TDocument>((object) id != null ? id.Document : default (TDocument), id?.Self?.Index, id?.Self?.Id)), ct);
    }

    public ExistsResponse DocumentExists(IDocumentExistsRequest request) => this.DoRequest<IDocumentExistsRequest, ExistsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ExistsResponse> DocumentExistsAsync(
      IDocumentExistsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IDocumentExistsRequest, ExistsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ExistsResponse SourceExists<TDocument>(
      DocumentPath<TDocument> id,
      Func<SourceExistsDescriptor<TDocument>, ISourceExistsRequest> selector = null)
      where TDocument : class
    {
      return this.SourceExists(selector.InvokeOrDefault<SourceExistsDescriptor<TDocument>, ISourceExistsRequest>(new SourceExistsDescriptor<TDocument>((object) id != null ? id.Document : default (TDocument), id?.Self?.Index, id?.Self?.Id)));
    }

    public Task<ExistsResponse> SourceExistsAsync<TDocument>(
      DocumentPath<TDocument> id,
      Func<SourceExistsDescriptor<TDocument>, ISourceExistsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.SourceExistsAsync(selector.InvokeOrDefault<SourceExistsDescriptor<TDocument>, ISourceExistsRequest>(new SourceExistsDescriptor<TDocument>((object) id != null ? id.Document : default (TDocument), id?.Self?.Index, id?.Self?.Id)), ct);
    }

    public ExistsResponse SourceExists(ISourceExistsRequest request) => this.DoRequest<ISourceExistsRequest, ExistsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ExistsResponse> SourceExistsAsync(
      ISourceExistsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<ISourceExistsRequest, ExistsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ExplainResponse<TDocument> Explain<TDocument>(
      DocumentPath<TDocument> id,
      Func<ExplainDescriptor<TDocument>, IExplainRequest> selector = null)
      where TDocument : class
    {
      return this.Explain<TDocument>(selector.InvokeOrDefault<ExplainDescriptor<TDocument>, IExplainRequest>(new ExplainDescriptor<TDocument>((object) id != null ? id.Document : default (TDocument), id?.Self?.Index, id?.Self?.Id)));
    }

    public Task<ExplainResponse<TDocument>> ExplainAsync<TDocument>(
      DocumentPath<TDocument> id,
      Func<ExplainDescriptor<TDocument>, IExplainRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.ExplainAsync<TDocument>(selector.InvokeOrDefault<ExplainDescriptor<TDocument>, IExplainRequest>(new ExplainDescriptor<TDocument>((object) id != null ? id.Document : default (TDocument), id?.Self?.Index, id?.Self?.Id)), ct);
    }

    public ExplainResponse<TDocument> Explain<TDocument>(IExplainRequest request) where TDocument : class => this.DoRequest<IExplainRequest, ExplainResponse<TDocument>>(request, (IRequestParameters) request.RequestParameters);

    public Task<ExplainResponse<TDocument>> ExplainAsync<TDocument>(
      IExplainRequest request,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.DoRequestAsync<IExplainRequest, ExplainResponse<TDocument>>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public FieldCapabilitiesResponse FieldCapabilities(
      Nest.Indices index = null,
      Func<FieldCapabilitiesDescriptor, IFieldCapabilitiesRequest> selector = null)
    {
      return this.FieldCapabilities(selector.InvokeOrDefault<FieldCapabilitiesDescriptor, IFieldCapabilitiesRequest>(new FieldCapabilitiesDescriptor().Index(index)));
    }

    public Task<FieldCapabilitiesResponse> FieldCapabilitiesAsync(
      Nest.Indices index = null,
      Func<FieldCapabilitiesDescriptor, IFieldCapabilitiesRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.FieldCapabilitiesAsync(selector.InvokeOrDefault<FieldCapabilitiesDescriptor, IFieldCapabilitiesRequest>(new FieldCapabilitiesDescriptor().Index(index)), ct);
    }

    public FieldCapabilitiesResponse FieldCapabilities(IFieldCapabilitiesRequest request) => this.DoRequest<IFieldCapabilitiesRequest, FieldCapabilitiesResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<FieldCapabilitiesResponse> FieldCapabilitiesAsync(
      IFieldCapabilitiesRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IFieldCapabilitiesRequest, FieldCapabilitiesResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetResponse<TDocument> Get<TDocument>(
      DocumentPath<TDocument> id,
      Func<GetDescriptor<TDocument>, IGetRequest> selector = null)
      where TDocument : class
    {
      return this.Get<TDocument>(selector.InvokeOrDefault<GetDescriptor<TDocument>, IGetRequest>(new GetDescriptor<TDocument>((object) id != null ? id.Document : default (TDocument), id?.Self?.Index, id?.Self?.Id)));
    }

    public Task<GetResponse<TDocument>> GetAsync<TDocument>(
      DocumentPath<TDocument> id,
      Func<GetDescriptor<TDocument>, IGetRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.GetAsync<TDocument>(selector.InvokeOrDefault<GetDescriptor<TDocument>, IGetRequest>(new GetDescriptor<TDocument>((object) id != null ? id.Document : default (TDocument), id?.Self?.Index, id?.Self?.Id)), ct);
    }

    public GetResponse<TDocument> Get<TDocument>(IGetRequest request) where TDocument : class => this.DoRequest<IGetRequest, GetResponse<TDocument>>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetResponse<TDocument>> GetAsync<TDocument>(
      IGetRequest request,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.DoRequestAsync<IGetRequest, GetResponse<TDocument>>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public GetScriptResponse GetScript(
      Id id,
      Func<GetScriptDescriptor, IGetScriptRequest> selector = null)
    {
      return this.GetScript(selector.InvokeOrDefault<GetScriptDescriptor, IGetScriptRequest>(new GetScriptDescriptor(id)));
    }

    public Task<GetScriptResponse> GetScriptAsync(
      Id id,
      Func<GetScriptDescriptor, IGetScriptRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.GetScriptAsync(selector.InvokeOrDefault<GetScriptDescriptor, IGetScriptRequest>(new GetScriptDescriptor(id)), ct);
    }

    public GetScriptResponse GetScript(IGetScriptRequest request) => this.DoRequest<IGetScriptRequest, GetScriptResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<GetScriptResponse> GetScriptAsync(IGetScriptRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IGetScriptRequest, GetScriptResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public SourceResponse<TDocument> Source<TDocument>(
      DocumentPath<TDocument> id,
      Func<SourceDescriptor<TDocument>, ISourceRequest> selector = null)
      where TDocument : class
    {
      return this.Source<TDocument>(selector.InvokeOrDefault<SourceDescriptor<TDocument>, ISourceRequest>(new SourceDescriptor<TDocument>((object) id != null ? id.Document : default (TDocument), id?.Self?.Index, id?.Self?.Id)));
    }

    public Task<SourceResponse<TDocument>> SourceAsync<TDocument>(
      DocumentPath<TDocument> id,
      Func<SourceDescriptor<TDocument>, ISourceRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.SourceAsync<TDocument>(selector.InvokeOrDefault<SourceDescriptor<TDocument>, ISourceRequest>(new SourceDescriptor<TDocument>((object) id != null ? id.Document : default (TDocument), id?.Self?.Index, id?.Self?.Id)), ct);
    }

    public SourceResponse<TDocument> Source<TDocument>(ISourceRequest request) where TDocument : class => this.DoRequest<ISourceRequest, SourceResponse<TDocument>>(request, this.ResponseBuilder(request.RequestParameters, (CustomResponseBuilderBase) SourceRequestResponseBuilder<TDocument>.Instance));

    public Task<SourceResponse<TDocument>> SourceAsync<TDocument>(
      ISourceRequest request,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.DoRequestAsync<ISourceRequest, SourceResponse<TDocument>>(request, this.ResponseBuilder(request.RequestParameters, (CustomResponseBuilderBase) SourceRequestResponseBuilder<TDocument>.Instance), ct);
    }

    public IndexResponse Index<TDocument>(
      TDocument document,
      Func<IndexDescriptor<TDocument>, IIndexRequest<TDocument>> selector)
      where TDocument : class
    {
      return this.Index<TDocument>(selector.InvokeOrDefault<IndexDescriptor<TDocument>, IIndexRequest<TDocument>>(new IndexDescriptor<TDocument>(document)));
    }

    public Task<IndexResponse> IndexAsync<TDocument>(
      TDocument document,
      Func<IndexDescriptor<TDocument>, IIndexRequest<TDocument>> selector,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.IndexAsync<TDocument>(selector.InvokeOrDefault<IndexDescriptor<TDocument>, IIndexRequest<TDocument>>(new IndexDescriptor<TDocument>(document)), ct);
    }

    public IndexResponse Index<TDocument>(IIndexRequest<TDocument> request) where TDocument : class => this.DoRequest<IIndexRequest<TDocument>, IndexResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<IndexResponse> IndexAsync<TDocument>(
      IIndexRequest<TDocument> request,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.DoRequestAsync<IIndexRequest<TDocument>, IndexResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public RootNodeInfoResponse RootNodeInfo(
      Func<RootNodeInfoDescriptor, IRootNodeInfoRequest> selector = null)
    {
      return this.RootNodeInfo(selector.InvokeOrDefault<RootNodeInfoDescriptor, IRootNodeInfoRequest>(new RootNodeInfoDescriptor()));
    }

    public Task<RootNodeInfoResponse> RootNodeInfoAsync(
      Func<RootNodeInfoDescriptor, IRootNodeInfoRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.RootNodeInfoAsync(selector.InvokeOrDefault<RootNodeInfoDescriptor, IRootNodeInfoRequest>(new RootNodeInfoDescriptor()), ct);
    }

    public RootNodeInfoResponse RootNodeInfo(IRootNodeInfoRequest request) => this.DoRequest<IRootNodeInfoRequest, RootNodeInfoResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<RootNodeInfoResponse> RootNodeInfoAsync(
      IRootNodeInfoRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IRootNodeInfoRequest, RootNodeInfoResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public MultiGetResponse MultiGet(
      Func<MultiGetDescriptor, IMultiGetRequest> selector = null)
    {
      return this.MultiGet(selector.InvokeOrDefault<MultiGetDescriptor, IMultiGetRequest>(new MultiGetDescriptor()));
    }

    public Task<MultiGetResponse> MultiGetAsync(
      Func<MultiGetDescriptor, IMultiGetRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.MultiGetAsync(selector.InvokeOrDefault<MultiGetDescriptor, IMultiGetRequest>(new MultiGetDescriptor()), ct);
    }

    public MultiGetResponse MultiGet(IMultiGetRequest request) => this.DoRequest<IMultiGetRequest, MultiGetResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<MultiGetResponse> MultiGetAsync(IMultiGetRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IMultiGetRequest, MultiGetResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public MultiSearchResponse MultiSearch(
      Nest.Indices index = null,
      Func<MultiSearchDescriptor, IMultiSearchRequest> selector = null)
    {
      return this.MultiSearch(selector.InvokeOrDefault<MultiSearchDescriptor, IMultiSearchRequest>(new MultiSearchDescriptor().Index(index)));
    }

    public Task<MultiSearchResponse> MultiSearchAsync(
      Nest.Indices index = null,
      Func<MultiSearchDescriptor, IMultiSearchRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.MultiSearchAsync(selector.InvokeOrDefault<MultiSearchDescriptor, IMultiSearchRequest>(new MultiSearchDescriptor().Index(index)), ct);
    }

    public MultiSearchResponse MultiSearch(IMultiSearchRequest request) => this.DoRequest<IMultiSearchRequest, MultiSearchResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<MultiSearchResponse> MultiSearchAsync(
      IMultiSearchRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IMultiSearchRequest, MultiSearchResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public MultiSearchResponse MultiSearchTemplate(
      Nest.Indices index = null,
      Func<MultiSearchTemplateDescriptor, IMultiSearchTemplateRequest> selector = null)
    {
      return this.MultiSearchTemplate(selector.InvokeOrDefault<MultiSearchTemplateDescriptor, IMultiSearchTemplateRequest>(new MultiSearchTemplateDescriptor().Index(index)));
    }

    public Task<MultiSearchResponse> MultiSearchTemplateAsync(
      Nest.Indices index = null,
      Func<MultiSearchTemplateDescriptor, IMultiSearchTemplateRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.MultiSearchTemplateAsync(selector.InvokeOrDefault<MultiSearchTemplateDescriptor, IMultiSearchTemplateRequest>(new MultiSearchTemplateDescriptor().Index(index)), ct);
    }

    public MultiSearchResponse MultiSearchTemplate(IMultiSearchTemplateRequest request) => this.DoRequest<IMultiSearchTemplateRequest, MultiSearchResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<MultiSearchResponse> MultiSearchTemplateAsync(
      IMultiSearchTemplateRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IMultiSearchTemplateRequest, MultiSearchResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public MultiTermVectorsResponse MultiTermVectors(
      Func<MultiTermVectorsDescriptor, IMultiTermVectorsRequest> selector = null)
    {
      return this.MultiTermVectors(selector.InvokeOrDefault<MultiTermVectorsDescriptor, IMultiTermVectorsRequest>(new MultiTermVectorsDescriptor()));
    }

    public Task<MultiTermVectorsResponse> MultiTermVectorsAsync(
      Func<MultiTermVectorsDescriptor, IMultiTermVectorsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.MultiTermVectorsAsync(selector.InvokeOrDefault<MultiTermVectorsDescriptor, IMultiTermVectorsRequest>(new MultiTermVectorsDescriptor()), ct);
    }

    public MultiTermVectorsResponse MultiTermVectors(IMultiTermVectorsRequest request) => this.DoRequest<IMultiTermVectorsRequest, MultiTermVectorsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<MultiTermVectorsResponse> MultiTermVectorsAsync(
      IMultiTermVectorsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IMultiTermVectorsRequest, MultiTermVectorsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public OpenPointInTimeResponse OpenPointInTime(
      Nest.Indices index,
      Func<OpenPointInTimeDescriptor, IOpenPointInTimeRequest> selector = null)
    {
      return this.OpenPointInTime(selector.InvokeOrDefault<OpenPointInTimeDescriptor, IOpenPointInTimeRequest>(new OpenPointInTimeDescriptor(index)));
    }

    public Task<OpenPointInTimeResponse> OpenPointInTimeAsync(
      Nest.Indices index,
      Func<OpenPointInTimeDescriptor, IOpenPointInTimeRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.OpenPointInTimeAsync(selector.InvokeOrDefault<OpenPointInTimeDescriptor, IOpenPointInTimeRequest>(new OpenPointInTimeDescriptor(index)), ct);
    }

    public OpenPointInTimeResponse OpenPointInTime(IOpenPointInTimeRequest request) => this.DoRequest<IOpenPointInTimeRequest, OpenPointInTimeResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<OpenPointInTimeResponse> OpenPointInTimeAsync(
      IOpenPointInTimeRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IOpenPointInTimeRequest, OpenPointInTimeResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public PingResponse Ping(Func<PingDescriptor, IPingRequest> selector = null) => this.Ping(selector.InvokeOrDefault<PingDescriptor, IPingRequest>(new PingDescriptor()));

    public Task<PingResponse> PingAsync(
      Func<PingDescriptor, IPingRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PingAsync(selector.InvokeOrDefault<PingDescriptor, IPingRequest>(new PingDescriptor()), ct);
    }

    public PingResponse Ping(IPingRequest request) => this.DoRequest<IPingRequest, PingResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PingResponse> PingAsync(IPingRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IPingRequest, PingResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public PutScriptResponse PutScript(
      Id id,
      Func<PutScriptDescriptor, IPutScriptRequest> selector)
    {
      return this.PutScript(selector.InvokeOrDefault<PutScriptDescriptor, IPutScriptRequest>(new PutScriptDescriptor(id)));
    }

    public Task<PutScriptResponse> PutScriptAsync(
      Id id,
      Func<PutScriptDescriptor, IPutScriptRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.PutScriptAsync(selector.InvokeOrDefault<PutScriptDescriptor, IPutScriptRequest>(new PutScriptDescriptor(id)), ct);
    }

    public PutScriptResponse PutScript(IPutScriptRequest request) => this.DoRequest<IPutScriptRequest, PutScriptResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PutScriptResponse> PutScriptAsync(IPutScriptRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IPutScriptRequest, PutScriptResponse>(request, (IRequestParameters) request.RequestParameters, ct);

    public ReindexOnServerResponse ReindexOnServer(
      Func<ReindexOnServerDescriptor, IReindexOnServerRequest> selector)
    {
      return this.ReindexOnServer(selector.InvokeOrDefault<ReindexOnServerDescriptor, IReindexOnServerRequest>(new ReindexOnServerDescriptor()));
    }

    public Task<ReindexOnServerResponse> ReindexOnServerAsync(
      Func<ReindexOnServerDescriptor, IReindexOnServerRequest> selector,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ReindexOnServerAsync(selector.InvokeOrDefault<ReindexOnServerDescriptor, IReindexOnServerRequest>(new ReindexOnServerDescriptor()), ct);
    }

    public ReindexOnServerResponse ReindexOnServer(IReindexOnServerRequest request) => this.DoRequest<IReindexOnServerRequest, ReindexOnServerResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ReindexOnServerResponse> ReindexOnServerAsync(
      IReindexOnServerRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IReindexOnServerRequest, ReindexOnServerResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ReindexRethrottleResponse ReindexRethrottle(
      TaskId taskId,
      Func<ReindexRethrottleDescriptor, IReindexRethrottleRequest> selector = null)
    {
      return this.ReindexRethrottle(selector.InvokeOrDefault<ReindexRethrottleDescriptor, IReindexRethrottleRequest>(new ReindexRethrottleDescriptor(taskId)));
    }

    public Task<ReindexRethrottleResponse> ReindexRethrottleAsync(
      TaskId taskId,
      Func<ReindexRethrottleDescriptor, IReindexRethrottleRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ReindexRethrottleAsync(selector.InvokeOrDefault<ReindexRethrottleDescriptor, IReindexRethrottleRequest>(new ReindexRethrottleDescriptor(taskId)), ct);
    }

    public ReindexRethrottleResponse ReindexRethrottle(IReindexRethrottleRequest request) => this.DoRequest<IReindexRethrottleRequest, ReindexRethrottleResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ReindexRethrottleResponse> ReindexRethrottleAsync(
      IReindexRethrottleRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IReindexRethrottleRequest, ReindexRethrottleResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public RenderSearchTemplateResponse RenderSearchTemplate(
      Func<RenderSearchTemplateDescriptor, IRenderSearchTemplateRequest> selector = null)
    {
      return this.RenderSearchTemplate(selector.InvokeOrDefault<RenderSearchTemplateDescriptor, IRenderSearchTemplateRequest>(new RenderSearchTemplateDescriptor()));
    }

    public Task<RenderSearchTemplateResponse> RenderSearchTemplateAsync(
      Func<RenderSearchTemplateDescriptor, IRenderSearchTemplateRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.RenderSearchTemplateAsync(selector.InvokeOrDefault<RenderSearchTemplateDescriptor, IRenderSearchTemplateRequest>(new RenderSearchTemplateDescriptor()), ct);
    }

    public RenderSearchTemplateResponse RenderSearchTemplate(IRenderSearchTemplateRequest request) => this.DoRequest<IRenderSearchTemplateRequest, RenderSearchTemplateResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<RenderSearchTemplateResponse> RenderSearchTemplateAsync(
      IRenderSearchTemplateRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IRenderSearchTemplateRequest, RenderSearchTemplateResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ExecutePainlessScriptResponse<TResult> ExecutePainlessScript<TResult>(
      Func<ExecutePainlessScriptDescriptor, IExecutePainlessScriptRequest> selector = null)
    {
      return this.ExecutePainlessScript<TResult>(selector.InvokeOrDefault<ExecutePainlessScriptDescriptor, IExecutePainlessScriptRequest>(new ExecutePainlessScriptDescriptor()));
    }

    public Task<ExecutePainlessScriptResponse<TResult>> ExecutePainlessScriptAsync<TResult>(
      Func<ExecutePainlessScriptDescriptor, IExecutePainlessScriptRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.ExecutePainlessScriptAsync<TResult>(selector.InvokeOrDefault<ExecutePainlessScriptDescriptor, IExecutePainlessScriptRequest>(new ExecutePainlessScriptDescriptor()), ct);
    }

    public ExecutePainlessScriptResponse<TResult> ExecutePainlessScript<TResult>(
      IExecutePainlessScriptRequest request)
    {
      return this.DoRequest<IExecutePainlessScriptRequest, ExecutePainlessScriptResponse<TResult>>(request, (IRequestParameters) request.RequestParameters);
    }

    public Task<ExecutePainlessScriptResponse<TResult>> ExecutePainlessScriptAsync<TResult>(
      IExecutePainlessScriptRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IExecutePainlessScriptRequest, ExecutePainlessScriptResponse<TResult>>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ISearchResponse<TDocument> Scroll<TInferDocument, TDocument>(
      Time scroll,
      string scrollId,
      Func<ScrollDescriptor<TInferDocument>, IScrollRequest> selector = null)
      where TInferDocument : class
      where TDocument : class
    {
      return this.Scroll<TDocument>(selector.InvokeOrDefault<ScrollDescriptor<TInferDocument>, IScrollRequest>(new ScrollDescriptor<TInferDocument>(scroll, scrollId)));
    }

    public async Task<ISearchResponse<TDocument>> ScrollAsync<TInferDocument, TDocument>(
      Time scroll,
      string scrollId,
      Func<ScrollDescriptor<TInferDocument>, IScrollRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TInferDocument : class
      where TDocument : class
    {
      return await this.ScrollAsync<TDocument>(selector.InvokeOrDefault<ScrollDescriptor<TInferDocument>, IScrollRequest>(new ScrollDescriptor<TInferDocument>(scroll, scrollId)), ct).ConfigureAwait(false);
    }

    public ISearchResponse<TDocument> Scroll<TDocument>(
      Time scroll,
      string scrollId,
      Func<ScrollDescriptor<TDocument>, IScrollRequest> selector = null)
      where TDocument : class
    {
      return this.Scroll<TDocument>(selector.InvokeOrDefault<ScrollDescriptor<TDocument>, IScrollRequest>(new ScrollDescriptor<TDocument>(scroll, scrollId)));
    }

    public async Task<ISearchResponse<TDocument>> ScrollAsync<TDocument>(
      Time scroll,
      string scrollId,
      Func<ScrollDescriptor<TDocument>, IScrollRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return await this.ScrollAsync<TDocument>(selector.InvokeOrDefault<ScrollDescriptor<TDocument>, IScrollRequest>(new ScrollDescriptor<TDocument>(scroll, scrollId)), ct).ConfigureAwait(false);
    }

    public ISearchResponse<TDocument> Scroll<TDocument>(IScrollRequest request) where TDocument : class => (ISearchResponse<TDocument>) this.DoRequest<IScrollRequest, SearchResponse<TDocument>>(request, (IRequestParameters) request.RequestParameters);

    public async Task<ISearchResponse<TDocument>> ScrollAsync<TDocument>(
      IScrollRequest request,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return (ISearchResponse<TDocument>) await this.DoRequestAsync<IScrollRequest, SearchResponse<TDocument>>(request, (IRequestParameters) request.RequestParameters, ct).ConfigureAwait(false);
    }

    public ISearchResponse<TDocument> Search<TInferDocument, TDocument>(
      Func<SearchDescriptor<TInferDocument>, ISearchRequest> selector = null)
      where TInferDocument : class
      where TDocument : class
    {
      return this.Search<TDocument>(selector.InvokeOrDefault<SearchDescriptor<TInferDocument>, ISearchRequest>(new SearchDescriptor<TInferDocument>()));
    }

    public async Task<ISearchResponse<TDocument>> SearchAsync<TInferDocument, TDocument>(
      Func<SearchDescriptor<TInferDocument>, ISearchRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TInferDocument : class
      where TDocument : class
    {
      return await this.SearchAsync<TDocument>(selector.InvokeOrDefault<SearchDescriptor<TInferDocument>, ISearchRequest>(new SearchDescriptor<TInferDocument>()), ct).ConfigureAwait(false);
    }

    public ISearchResponse<TDocument> Search<TDocument>(
      Func<SearchDescriptor<TDocument>, ISearchRequest> selector = null)
      where TDocument : class
    {
      return this.Search<TDocument>(selector.InvokeOrDefault<SearchDescriptor<TDocument>, ISearchRequest>(new SearchDescriptor<TDocument>()));
    }

    public async Task<ISearchResponse<TDocument>> SearchAsync<TDocument>(
      Func<SearchDescriptor<TDocument>, ISearchRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return await this.SearchAsync<TDocument>(selector.InvokeOrDefault<SearchDescriptor<TDocument>, ISearchRequest>(new SearchDescriptor<TDocument>()), ct).ConfigureAwait(false);
    }

    public ISearchResponse<TDocument> Search<TDocument>(ISearchRequest request) where TDocument : class => (ISearchResponse<TDocument>) this.DoRequest<ISearchRequest, SearchResponse<TDocument>>(request, (IRequestParameters) request.RequestParameters);

    public async Task<ISearchResponse<TDocument>> SearchAsync<TDocument>(
      ISearchRequest request,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return (ISearchResponse<TDocument>) await this.DoRequestAsync<ISearchRequest, SearchResponse<TDocument>>(request, (IRequestParameters) request.RequestParameters, ct).ConfigureAwait(false);
    }

    public SearchShardsResponse SearchShards<TDocument>(
      Func<SearchShardsDescriptor<TDocument>, ISearchShardsRequest> selector = null)
      where TDocument : class
    {
      return this.SearchShards(selector.InvokeOrDefault<SearchShardsDescriptor<TDocument>, ISearchShardsRequest>(new SearchShardsDescriptor<TDocument>()));
    }

    public Task<SearchShardsResponse> SearchShardsAsync<TDocument>(
      Func<SearchShardsDescriptor<TDocument>, ISearchShardsRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.SearchShardsAsync(selector.InvokeOrDefault<SearchShardsDescriptor<TDocument>, ISearchShardsRequest>(new SearchShardsDescriptor<TDocument>()), ct);
    }

    public SearchShardsResponse SearchShards(ISearchShardsRequest request) => this.DoRequest<ISearchShardsRequest, SearchShardsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<SearchShardsResponse> SearchShardsAsync(
      ISearchShardsRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<ISearchShardsRequest, SearchShardsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ISearchResponse<TDocument> SearchTemplate<TDocument>(
      Func<SearchTemplateDescriptor<TDocument>, ISearchTemplateRequest> selector = null)
      where TDocument : class
    {
      return this.SearchTemplate<TDocument>(selector.InvokeOrDefault<SearchTemplateDescriptor<TDocument>, ISearchTemplateRequest>(new SearchTemplateDescriptor<TDocument>()));
    }

    public async Task<ISearchResponse<TDocument>> SearchTemplateAsync<TDocument>(
      Func<SearchTemplateDescriptor<TDocument>, ISearchTemplateRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return await this.SearchTemplateAsync<TDocument>(selector.InvokeOrDefault<SearchTemplateDescriptor<TDocument>, ISearchTemplateRequest>(new SearchTemplateDescriptor<TDocument>()), ct).ConfigureAwait(false);
    }

    public ISearchResponse<TDocument> SearchTemplate<TDocument>(ISearchTemplateRequest request) where TDocument : class => (ISearchResponse<TDocument>) this.DoRequest<ISearchTemplateRequest, SearchResponse<TDocument>>(request, (IRequestParameters) request.RequestParameters);

    public async Task<ISearchResponse<TDocument>> SearchTemplateAsync<TDocument>(
      ISearchTemplateRequest request,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return (ISearchResponse<TDocument>) await this.DoRequestAsync<ISearchTemplateRequest, SearchResponse<TDocument>>(request, (IRequestParameters) request.RequestParameters, ct).ConfigureAwait(false);
    }

    public TermVectorsResponse TermVectors<TDocument>(
      Func<TermVectorsDescriptor<TDocument>, ITermVectorsRequest<TDocument>> selector = null)
      where TDocument : class
    {
      return this.TermVectors<TDocument>(selector.InvokeOrDefault<TermVectorsDescriptor<TDocument>, ITermVectorsRequest<TDocument>>(new TermVectorsDescriptor<TDocument>()));
    }

    public Task<TermVectorsResponse> TermVectorsAsync<TDocument>(
      Func<TermVectorsDescriptor<TDocument>, ITermVectorsRequest<TDocument>> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.TermVectorsAsync<TDocument>(selector.InvokeOrDefault<TermVectorsDescriptor<TDocument>, ITermVectorsRequest<TDocument>>(new TermVectorsDescriptor<TDocument>()), ct);
    }

    public TermVectorsResponse TermVectors<TDocument>(ITermVectorsRequest<TDocument> request) where TDocument : class => this.DoRequest<ITermVectorsRequest<TDocument>, TermVectorsResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<TermVectorsResponse> TermVectorsAsync<TDocument>(
      ITermVectorsRequest<TDocument> request,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.DoRequestAsync<ITermVectorsRequest<TDocument>, TermVectorsResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public UpdateResponse<TDocument> Update<TDocument, TPartialDocument>(
      DocumentPath<TDocument> id,
      Func<UpdateDescriptor<TDocument, TPartialDocument>, IUpdateRequest<TDocument, TPartialDocument>> selector)
      where TDocument : class
      where TPartialDocument : class
    {
      return this.Update<TDocument, TPartialDocument>(selector.InvokeOrDefault<UpdateDescriptor<TDocument, TPartialDocument>, IUpdateRequest<TDocument, TPartialDocument>>(new UpdateDescriptor<TDocument, TPartialDocument>((object) id != null ? id.Document : default (TDocument), id?.Self?.Index, id?.Self?.Id)));
    }

    public Task<UpdateResponse<TDocument>> UpdateAsync<TDocument, TPartialDocument>(
      DocumentPath<TDocument> id,
      Func<UpdateDescriptor<TDocument, TPartialDocument>, IUpdateRequest<TDocument, TPartialDocument>> selector,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
      where TPartialDocument : class
    {
      return this.UpdateAsync<TDocument, TPartialDocument>(selector.InvokeOrDefault<UpdateDescriptor<TDocument, TPartialDocument>, IUpdateRequest<TDocument, TPartialDocument>>(new UpdateDescriptor<TDocument, TPartialDocument>((object) id != null ? id.Document : default (TDocument), id?.Self?.Index, id?.Self?.Id)), ct);
    }

    public UpdateResponse<TDocument> Update<TDocument>(
      DocumentPath<TDocument> id,
      Func<UpdateDescriptor<TDocument, TDocument>, IUpdateRequest<TDocument, TDocument>> selector)
      where TDocument : class
    {
      return this.Update<TDocument, TDocument>(selector.InvokeOrDefault<UpdateDescriptor<TDocument, TDocument>, IUpdateRequest<TDocument, TDocument>>(new UpdateDescriptor<TDocument, TDocument>((object) id != null ? id.Document : default (TDocument), id?.Self?.Index, id?.Self?.Id)));
    }

    public Task<UpdateResponse<TDocument>> UpdateAsync<TDocument>(
      DocumentPath<TDocument> id,
      Func<UpdateDescriptor<TDocument, TDocument>, IUpdateRequest<TDocument, TDocument>> selector,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.UpdateAsync<TDocument, TDocument>(selector.InvokeOrDefault<UpdateDescriptor<TDocument, TDocument>, IUpdateRequest<TDocument, TDocument>>(new UpdateDescriptor<TDocument, TDocument>((object) id != null ? id.Document : default (TDocument), id?.Self?.Index, id?.Self?.Id)), ct);
    }

    public UpdateResponse<TDocument> Update<TDocument, TPartialDocument>(
      IUpdateRequest<TDocument, TPartialDocument> request)
      where TDocument : class
      where TPartialDocument : class
    {
      return this.DoRequest<IUpdateRequest<TDocument, TPartialDocument>, UpdateResponse<TDocument>>(request, (IRequestParameters) request.RequestParameters);
    }

    public Task<UpdateResponse<TDocument>> UpdateAsync<TDocument, TPartialDocument>(
      IUpdateRequest<TDocument, TPartialDocument> request,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
      where TPartialDocument : class
    {
      return this.DoRequestAsync<IUpdateRequest<TDocument, TPartialDocument>, UpdateResponse<TDocument>>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public UpdateByQueryResponse UpdateByQuery<TDocument>(
      Func<UpdateByQueryDescriptor<TDocument>, IUpdateByQueryRequest> selector = null)
      where TDocument : class
    {
      return this.UpdateByQuery(selector.InvokeOrDefault<UpdateByQueryDescriptor<TDocument>, IUpdateByQueryRequest>(new UpdateByQueryDescriptor<TDocument>()));
    }

    public Task<UpdateByQueryResponse> UpdateByQueryAsync<TDocument>(
      Func<UpdateByQueryDescriptor<TDocument>, IUpdateByQueryRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
      where TDocument : class
    {
      return this.UpdateByQueryAsync(selector.InvokeOrDefault<UpdateByQueryDescriptor<TDocument>, IUpdateByQueryRequest>(new UpdateByQueryDescriptor<TDocument>()), ct);
    }

    public UpdateByQueryResponse UpdateByQuery(IUpdateByQueryRequest request) => this.DoRequest<IUpdateByQueryRequest, UpdateByQueryResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<UpdateByQueryResponse> UpdateByQueryAsync(
      IUpdateByQueryRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IUpdateByQueryRequest, UpdateByQueryResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public ListTasksResponse UpdateByQueryRethrottle(
      TaskId taskId,
      Func<UpdateByQueryRethrottleDescriptor, IUpdateByQueryRethrottleRequest> selector = null)
    {
      return this.UpdateByQueryRethrottle(selector.InvokeOrDefault<UpdateByQueryRethrottleDescriptor, IUpdateByQueryRethrottleRequest>(new UpdateByQueryRethrottleDescriptor(taskId)));
    }

    public Task<ListTasksResponse> UpdateByQueryRethrottleAsync(
      TaskId taskId,
      Func<UpdateByQueryRethrottleDescriptor, IUpdateByQueryRethrottleRequest> selector = null,
      CancellationToken ct = default (CancellationToken))
    {
      return this.UpdateByQueryRethrottleAsync(selector.InvokeOrDefault<UpdateByQueryRethrottleDescriptor, IUpdateByQueryRethrottleRequest>(new UpdateByQueryRethrottleDescriptor(taskId)), ct);
    }

    public ListTasksResponse UpdateByQueryRethrottle(IUpdateByQueryRethrottleRequest request) => this.DoRequest<IUpdateByQueryRethrottleRequest, ListTasksResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<ListTasksResponse> UpdateByQueryRethrottleAsync(
      IUpdateByQueryRethrottleRequest request,
      CancellationToken ct = default (CancellationToken))
    {
      return this.DoRequestAsync<IUpdateByQueryRethrottleRequest, ListTasksResponse>(request, (IRequestParameters) request.RequestParameters, ct);
    }

    public PutMappingResponse Map<T>(
      Func<PutMappingDescriptor<T>, IPutMappingRequest> selector)
      where T : class
    {
      return this.Map(selector != null ? selector(new PutMappingDescriptor<T>()) : (IPutMappingRequest) null);
    }

    public PutMappingResponse Map(IPutMappingRequest request) => this.DoRequest<IPutMappingRequest, PutMappingResponse>(request, (IRequestParameters) request.RequestParameters);

    public Task<PutMappingResponse> MapAsync<T>(
      Func<PutMappingDescriptor<T>, IPutMappingRequest> selector,
      CancellationToken ct = default (CancellationToken))
      where T : class
    {
      return this.MapAsync(selector != null ? selector(new PutMappingDescriptor<T>()) : (IPutMappingRequest) null, ct);
    }

    public Task<PutMappingResponse> MapAsync(IPutMappingRequest request, CancellationToken ct = default (CancellationToken)) => this.DoRequestAsync<IPutMappingRequest, PutMappingResponse>(request, (IRequestParameters) request.RequestParameters, ct);
  }
}
