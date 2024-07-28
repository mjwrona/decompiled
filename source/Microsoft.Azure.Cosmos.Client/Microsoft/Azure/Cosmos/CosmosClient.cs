// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosClient
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Azure;
using Azure.Core;
using Microsoft.Azure.Cosmos.Authorization;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Handlers;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.QueryPlan;
using Microsoft.Azure.Cosmos.Telemetry;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Cosmos.Tracing.TraceData;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  public class CosmosClient : IDisposable
  {
    internal readonly string Id = Guid.NewGuid().ToString();
    private readonly object disposedLock = new object();
    private readonly string DatabaseRootUri = "//dbs/";
    private ConsistencyLevel? accountConsistencyLevel;
    private bool isDisposed;
    internal static int numberOfClientsCreated;
    internal static int NumberOfActiveClients;

    internal DateTime? DisposedDateTimeUtc { get; private set; }

    static CosmosClient()
    {
      HttpConstants.Versions.CurrentVersion = HttpConstants.Versions.v2018_12_31;
      HttpConstants.Versions.CurrentVersionUTF8 = Encoding.UTF8.GetBytes(HttpConstants.Versions.CurrentVersion);
      ServiceInteropWrapper.AssembliesExist = new Lazy<bool>((Func<bool>) (() =>
      {
        TryCatch<IntPtr> serviceProvider = QueryPartitionProvider.TryCreateServiceProvider("{}");
        if (serviceProvider.Failed)
          return false;
        Marshal.Release(serviceProvider.Result);
        return true;
      }));
      DefaultTrace.InitEventListener();
      if (Debugger.IsAttached)
        return;
      CosmosClient.RemoveDefaultTraceListener();
    }

    protected CosmosClient()
    {
    }

    public CosmosClient(string connectionString, CosmosClientOptions clientOptions = null)
      : this(CosmosClientOptions.GetAccountEndpoint(connectionString), CosmosClientOptions.GetAccountKey(connectionString), clientOptions)
    {
    }

    public CosmosClient(
      string accountEndpoint,
      string authKeyOrResourceToken,
      CosmosClientOptions clientOptions = null)
      : this(accountEndpoint, AuthorizationTokenProvider.CreateWithResourceTokenOrAuthKey(authKeyOrResourceToken), clientOptions)
    {
    }

    public CosmosClient(
      string accountEndpoint,
      AzureKeyCredential authKeyOrResourceTokenCredential,
      CosmosClientOptions clientOptions = null)
      : this(accountEndpoint, (AuthorizationTokenProvider) new AzureKeyCredentialAuthorizationTokenProvider(authKeyOrResourceTokenCredential), clientOptions)
    {
    }

    public CosmosClient(
      string accountEndpoint,
      TokenCredential tokenCredential,
      CosmosClientOptions clientOptions = null)
      : this(accountEndpoint, (AuthorizationTokenProvider) new AuthorizationTokenProviderTokenCredential(tokenCredential, new Uri(accountEndpoint), (TimeSpan?) clientOptions?.TokenCredentialBackgroundRefreshInterval), clientOptions)
    {
    }

    internal CosmosClient(
      string accountEndpoint,
      AuthorizationTokenProvider authorizationTokenProvider,
      CosmosClientOptions clientOptions)
    {
      this.Endpoint = !string.IsNullOrEmpty(accountEndpoint) ? new Uri(accountEndpoint) : throw new ArgumentNullException(nameof (accountEndpoint));
      this.AuthorizationTokenProvider = authorizationTokenProvider ?? throw new ArgumentNullException(nameof (authorizationTokenProvider));
      if (clientOptions == null)
        clientOptions = new CosmosClientOptions();
      this.ClientId = this.IncrementNumberOfClientsCreated();
      this.ClientContext = ClientContextCore.Create(this, clientOptions);
      this.ClientConfigurationTraceDatum = new ClientConfigurationTraceDatum(this.ClientContext, DateTime.UtcNow);
    }

    public static async Task<CosmosClient> CreateAndInitializeAsync(
      string accountEndpoint,
      string authKeyOrResourceToken,
      IReadOnlyList<(string databaseId, string containerId)> containers,
      CosmosClientOptions cosmosClientOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (containers == null)
        throw new ArgumentNullException(nameof (containers));
      CosmosClient cosmosClient = new CosmosClient(accountEndpoint, authKeyOrResourceToken, cosmosClientOptions);
      await cosmosClient.InitializeContainersAsync(containers, cancellationToken);
      CosmosClient andInitializeAsync = cosmosClient;
      cosmosClient = (CosmosClient) null;
      return andInitializeAsync;
    }

    public static async Task<CosmosClient> CreateAndInitializeAsync(
      string accountEndpoint,
      AzureKeyCredential authKeyOrResourceTokenCredential,
      IReadOnlyList<(string databaseId, string containerId)> containers,
      CosmosClientOptions cosmosClientOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (containers == null)
        throw new ArgumentNullException(nameof (containers));
      CosmosClient cosmosClient = new CosmosClient(accountEndpoint, authKeyOrResourceTokenCredential, cosmosClientOptions);
      await cosmosClient.InitializeContainersAsync(containers, cancellationToken);
      CosmosClient andInitializeAsync = cosmosClient;
      cosmosClient = (CosmosClient) null;
      return andInitializeAsync;
    }

    public static async Task<CosmosClient> CreateAndInitializeAsync(
      string connectionString,
      IReadOnlyList<(string databaseId, string containerId)> containers,
      CosmosClientOptions cosmosClientOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (containers == null)
        throw new ArgumentNullException(nameof (containers));
      CosmosClient cosmosClient = new CosmosClient(connectionString, cosmosClientOptions);
      await cosmosClient.InitializeContainersAsync(containers, cancellationToken);
      CosmosClient andInitializeAsync = cosmosClient;
      cosmosClient = (CosmosClient) null;
      return andInitializeAsync;
    }

    public static async Task<CosmosClient> CreateAndInitializeAsync(
      string accountEndpoint,
      TokenCredential tokenCredential,
      IReadOnlyList<(string databaseId, string containerId)> containers,
      CosmosClientOptions cosmosClientOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (containers == null)
        throw new ArgumentNullException(nameof (containers));
      CosmosClient cosmosClient = new CosmosClient(accountEndpoint, tokenCredential, cosmosClientOptions);
      await cosmosClient.InitializeContainersAsync(containers, cancellationToken);
      CosmosClient andInitializeAsync = cosmosClient;
      cosmosClient = (CosmosClient) null;
      return andInitializeAsync;
    }

    internal CosmosClient(
      string accountEndpoint,
      string authKeyOrResourceToken,
      CosmosClientOptions cosmosClientOptions,
      DocumentClient documentClient)
    {
      if (string.IsNullOrEmpty(accountEndpoint))
        throw new ArgumentNullException(nameof (accountEndpoint));
      if (string.IsNullOrEmpty(authKeyOrResourceToken))
        throw new ArgumentNullException(nameof (authKeyOrResourceToken));
      if (cosmosClientOptions == null)
        throw new ArgumentNullException(nameof (cosmosClientOptions));
      if (documentClient == null)
        throw new ArgumentNullException(nameof (documentClient));
      this.Endpoint = new Uri(accountEndpoint);
      this.AccountKey = authKeyOrResourceToken;
      this.AuthorizationTokenProvider = AuthorizationTokenProvider.CreateWithResourceTokenOrAuthKey(authKeyOrResourceToken);
      this.ClientContext = ClientContextCore.Create(this, documentClient, cosmosClientOptions);
      this.ClientConfigurationTraceDatum = new ClientConfigurationTraceDatum(this.ClientContext, DateTime.UtcNow);
    }

    public virtual CosmosClientOptions ClientOptions => this.ClientContext.ClientOptions;

    public virtual CosmosResponseFactory ResponseFactory => (CosmosResponseFactory) this.ClientContext.ResponseFactory;

    public virtual Uri Endpoint { get; }

    internal string AccountKey { get; }

    internal AuthorizationTokenProvider AuthorizationTokenProvider { get; }

    internal DocumentClient DocumentClient => this.ClientContext.DocumentClient;

    internal RequestInvokerHandler RequestHandler => this.ClientContext.RequestHandler;

    internal CosmosClientContext ClientContext { get; }

    internal ClientConfigurationTraceDatum ClientConfigurationTraceDatum { get; }

    internal int ClientId { get; }

    public virtual Task<AccountProperties> ReadAccountAsync() => this.ClientContext.OperationHelperAsync<AccountProperties>(nameof (ReadAccountAsync), (RequestOptions) null, (Func<ITrace, Task<AccountProperties>>) (trace => ((IDocumentClientInternal) this.DocumentClient).GetDatabaseAccountInternalAsync(this.Endpoint)));

    public virtual Database GetDatabase(string id) => (Database) new DatabaseInlineCore(this.ClientContext, id);

    public virtual Container GetContainer(string databaseId, string containerId)
    {
      if (string.IsNullOrEmpty(databaseId))
        throw new ArgumentNullException(nameof (databaseId));
      return !string.IsNullOrEmpty(containerId) ? this.GetDatabase(databaseId).GetContainer(containerId) : throw new ArgumentNullException(nameof (containerId));
    }

    public virtual Task<DatabaseResponse> CreateDatabaseAsync(
      string id,
      int? throughput = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (string.IsNullOrEmpty(id))
        throw new ArgumentNullException(nameof (id));
      return this.ClientContext.OperationHelperAsync<DatabaseResponse>(nameof (CreateDatabaseAsync), requestOptions, (Func<ITrace, Task<DatabaseResponse>>) (trace => this.CreateDatabaseInternalAsync(this.PrepareDatabaseProperties(id), ThroughputProperties.CreateManualThroughput(throughput), requestOptions, trace, cancellationToken)), (Func<DatabaseResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<DatabaseProperties>((Response<DatabaseProperties>) response, databaseName: response.Resource?.Id)));
    }

    public virtual Task<DatabaseResponse> CreateDatabaseAsync(
      string id,
      ThroughputProperties throughputProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (string.IsNullOrEmpty(id))
        throw new ArgumentNullException(nameof (id));
      return this.ClientContext.OperationHelperAsync<DatabaseResponse>(nameof (CreateDatabaseAsync), requestOptions, (Func<ITrace, Task<DatabaseResponse>>) (trace => this.CreateDatabaseInternalAsync(this.PrepareDatabaseProperties(id), throughputProperties, requestOptions, trace, cancellationToken)), (Func<DatabaseResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<DatabaseProperties>((Response<DatabaseProperties>) response, databaseName: response.Resource?.Id)));
    }

    public virtual Task<DatabaseResponse> CreateDatabaseIfNotExistsAsync(
      string id,
      ThroughputProperties throughputProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (!string.IsNullOrEmpty(id))
        return this.ClientContext.OperationHelperAsync<DatabaseResponse>(nameof (CreateDatabaseIfNotExistsAsync), requestOptions, (Func<ITrace, Task<DatabaseResponse>>) (async trace =>
        {
          double totalRequestCharge = 0.0;
          DatabaseProperties databaseProperties = this.PrepareDatabaseProperties(id);
          DatabaseCore database = (DatabaseCore) this.GetDatabase(id);
          using (ResponseMessage responseMessage = await database.ReadStreamAsync(requestOptions, trace, cancellationToken))
          {
            totalRequestCharge = responseMessage.Headers.RequestCharge;
            if (responseMessage.StatusCode != HttpStatusCode.NotFound)
              return this.ClientContext.ResponseFactory.CreateDatabaseResponse((Database) database, responseMessage);
          }
          using (ResponseMessage streamInternalAsync = await this.CreateDatabaseStreamInternalAsync(databaseProperties, throughputProperties, requestOptions, trace, cancellationToken))
          {
            totalRequestCharge += streamInternalAsync.Headers.RequestCharge;
            streamInternalAsync.Headers.RequestCharge = totalRequestCharge;
            if (streamInternalAsync.StatusCode != HttpStatusCode.Conflict)
              return this.ClientContext.ResponseFactory.CreateDatabaseResponse(this.GetDatabase(databaseProperties.Id), streamInternalAsync);
          }
          using (ResponseMessage responseMessage = await database.ReadStreamAsync(requestOptions, trace, cancellationToken))
          {
            totalRequestCharge += responseMessage.Headers.RequestCharge;
            responseMessage.Headers.RequestCharge = totalRequestCharge;
            return this.ClientContext.ResponseFactory.CreateDatabaseResponse(this.GetDatabase(databaseProperties.Id), responseMessage);
          }
        }), (Func<DatabaseResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<DatabaseProperties>((Response<DatabaseProperties>) response, databaseName: response.Resource?.Id)));
      throw new ArgumentNullException(nameof (id));
    }

    public virtual Task<DatabaseResponse> CreateDatabaseIfNotExistsAsync(
      string id,
      int? throughput = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ThroughputProperties manualThroughput = ThroughputProperties.CreateManualThroughput(throughput);
      return this.CreateDatabaseIfNotExistsAsync(id, manualThroughput, requestOptions, cancellationToken);
    }

    public virtual FeedIterator<T> GetDatabaseQueryIterator<T>(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator<T>) new FeedIteratorInlineCore<T>(this.GetDatabaseQueryIteratorHelper<T>(queryDefinition, continuationToken, requestOptions), this.ClientContext);
    }

    public virtual FeedIterator GetDatabaseQueryStreamIterator(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator) new FeedIteratorInlineCore(this.GetDatabaseQueryStreamIteratorHelper(queryDefinition, continuationToken, requestOptions), this.ClientContext);
    }

    public virtual FeedIterator<T> GetDatabaseQueryIterator<T>(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      QueryDefinition queryDefinition = (QueryDefinition) null;
      if (queryText != null)
        queryDefinition = new QueryDefinition(queryText);
      return (FeedIterator<T>) new FeedIteratorInlineCore<T>(this.GetDatabaseQueryIteratorHelper<T>(queryDefinition, continuationToken, requestOptions), this.ClientContext);
    }

    public virtual FeedIterator GetDatabaseQueryStreamIterator(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      QueryDefinition queryDefinition = (QueryDefinition) null;
      if (queryText != null)
        queryDefinition = new QueryDefinition(queryText);
      return (FeedIterator) new FeedIteratorInlineCore(this.GetDatabaseQueryStreamIterator(queryDefinition, continuationToken, requestOptions), this.ClientContext);
    }

    public virtual Task<ResponseMessage> CreateDatabaseStreamAsync(
      DatabaseProperties databaseProperties,
      int? throughput = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (databaseProperties == null)
        throw new ArgumentNullException(nameof (databaseProperties));
      return this.ClientContext.OperationHelperAsync<ResponseMessage>(nameof (CreateDatabaseStreamAsync), requestOptions, (Func<ITrace, Task<ResponseMessage>>) (trace =>
      {
        this.ClientContext.ValidateResource(databaseProperties.Id);
        return this.CreateDatabaseStreamInternalAsync(databaseProperties, ThroughputProperties.CreateManualThroughput(throughput), requestOptions, trace, cancellationToken);
      }), (Func<ResponseMessage, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse(response)));
    }

    private static void RemoveDefaultTraceListener()
    {
      if (DefaultTrace.TraceSource.Listeners.Count <= 0)
        return;
      List<DefaultTraceListener> defaultTraceListenerList = new List<DefaultTraceListener>();
      foreach (object listener in DefaultTrace.TraceSource.Listeners)
      {
        if (listener is DefaultTraceListener defaultTraceListener)
          defaultTraceListenerList.Add(defaultTraceListener);
      }
      foreach (TraceListener listener in defaultTraceListenerList)
        DefaultTrace.TraceSource.Listeners.Remove(listener);
    }

    internal virtual async Task<ConsistencyLevel> GetAccountConsistencyLevelAsync()
    {
      if (!this.accountConsistencyLevel.HasValue)
        this.accountConsistencyLevel = new ConsistencyLevel?(await this.DocumentClient.GetDefaultConsistencyLevelAsync());
      return this.accountConsistencyLevel.Value;
    }

    internal DatabaseProperties PrepareDatabaseProperties(string id)
    {
      DatabaseProperties databaseProperties = !string.IsNullOrWhiteSpace(id) ? new DatabaseProperties()
      {
        Id = id
      } : throw new ArgumentNullException(nameof (id));
      this.ClientContext.ValidateResource(databaseProperties.Id);
      return databaseProperties;
    }

    internal virtual Task<ResponseMessage> CreateDatabaseStreamAsync(
      DatabaseProperties databaseProperties,
      ThroughputProperties throughputProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (databaseProperties == null)
        throw new ArgumentNullException(nameof (databaseProperties));
      return this.ClientContext.OperationHelperAsync<ResponseMessage>("CreateDatabaseIfNotExistsAsync", requestOptions, (Func<ITrace, Task<ResponseMessage>>) (trace =>
      {
        this.ClientContext.ValidateResource(databaseProperties.Id);
        return this.CreateDatabaseStreamInternalAsync(databaseProperties, throughputProperties, requestOptions, trace, cancellationToken);
      }), (Func<ResponseMessage, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse(response)));
    }

    private async Task<DatabaseResponse> CreateDatabaseInternalAsync(
      DatabaseProperties databaseProperties,
      ThroughputProperties throughputProperties,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      ResponseMessage responseMessage = await this.ClientContext.ProcessResourceOperationStreamAsync(this.DatabaseRootUri, ResourceType.Database, OperationType.Create, requestOptions, (ContainerInternal) null, (FeedRange) null, this.ClientContext.SerializerCore.ToStream<DatabaseProperties>(databaseProperties), (Action<RequestMessage>) (httpRequestMessage => httpRequestMessage.AddThroughputPropertiesHeader(throughputProperties)), trace, cancellationToken);
      return this.ClientContext.ResponseFactory.CreateDatabaseResponse(this.GetDatabase(databaseProperties.Id), responseMessage);
    }

    private Task<ResponseMessage> CreateDatabaseStreamInternalAsync(
      DatabaseProperties databaseProperties,
      ThroughputProperties throughputProperties,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.ClientContext.ProcessResourceOperationAsync<ResponseMessage>(this.DatabaseRootUri, ResourceType.Database, OperationType.Create, requestOptions, (ContainerInternal) null, (FeedRange) null, this.ClientContext.SerializerCore.ToStream<DatabaseProperties>(databaseProperties), (Action<RequestMessage>) (httpRequestMessage => httpRequestMessage.AddThroughputPropertiesHeader(throughputProperties)), (Func<ResponseMessage, ResponseMessage>) (response => response), trace, cancellationToken);
    }

    private FeedIteratorInternal<T> GetDatabaseQueryIteratorHelper<T>(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIteratorInternal<T>) new FeedIteratorCore<T>(this.GetDatabaseQueryStreamIteratorHelper(queryDefinition, continuationToken, requestOptions) ?? throw new InvalidOperationException("Expected a FeedIteratorInternal."), (Func<ResponseMessage, FeedResponse<T>>) (response => this.ClientContext.ResponseFactory.CreateQueryFeedResponse<T>(response, ResourceType.Database)));
    }

    private FeedIteratorInternal GetDatabaseQueryStreamIteratorHelper(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIteratorInternal) new FeedIteratorCore(this.ClientContext, this.DatabaseRootUri, ResourceType.Database, queryDefinition, continuationToken, requestOptions, (ContainerInternal) null);
    }

    internal async Task InitializeContainersAsync(
      IReadOnlyList<(string databaseId, string containerId)> containers,
      CancellationToken cancellationToken)
    {
      try
      {
        List<Task> taskList = new List<Task>();
        foreach ((string databaseId, string containerId) in (IEnumerable<(string, string)>) containers)
          taskList.Add(this.InitializeContainerAsync(databaseId, containerId, cancellationToken));
        await Task.WhenAll((IEnumerable<Task>) taskList);
      }
      catch
      {
        this.Dispose();
        throw;
      }
    }

    private int IncrementNumberOfClientsCreated()
    {
      this.IncrementNumberOfActiveClients();
      return Interlocked.Increment(ref CosmosClient.numberOfClientsCreated);
    }

    private int IncrementNumberOfActiveClients() => Interlocked.Increment(ref CosmosClient.NumberOfActiveClients);

    private int DecrementNumberOfActiveClients() => CosmosClient.NumberOfActiveClients > 0 ? Interlocked.Decrement(ref CosmosClient.NumberOfActiveClients) : 0;

    private async Task InitializeContainerAsync(
      string databaseId,
      string containerId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ContainerInternal container = (ContainerInternal) this.GetContainer(databaseId, containerId);
      IReadOnlyList<FeedRange> feedRangesAsync = await container.GetFeedRangesAsync(cancellationToken);
      List<Task> taskList = new List<Task>();
      foreach (FeedRange feedRange in (IEnumerable<FeedRange>) feedRangesAsync)
        taskList.Add(CosmosClient.InitializeFeedRangeAsync(container, feedRange, cancellationToken));
      await Task.WhenAll((IEnumerable<Task>) taskList);
      container = (ContainerInternal) null;
    }

    private static async Task InitializeFeedRangeAsync(
      ContainerInternal container,
      FeedRange feedRange,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (FeedIterator feedIterator = container.GetItemQueryStreamIterator(feedRange, new QueryDefinition("select * from c where c.id = '" + Guid.NewGuid().ToString() + "'"), (string) null, new QueryRequestOptions()))
      {
        while (feedIterator.HasMoreResults)
        {
          using (ResponseMessage responseMessage = await feedIterator.ReadNextAsync(cancellationToken))
            responseMessage.EnsureSuccessStatusCode();
        }
      }
    }

    public void Dispose() => this.Dispose(true);

    protected virtual void Dispose(bool disposing)
    {
      lock (this.disposedLock)
      {
        if (this.isDisposed)
          return;
        this.isDisposed = true;
      }
      this.DisposedDateTimeUtc = new DateTime?(DateTime.UtcNow);
      if (!disposing)
        return;
      this.ClientContext.Dispose();
      this.DecrementNumberOfActiveClients();
    }
  }
}
