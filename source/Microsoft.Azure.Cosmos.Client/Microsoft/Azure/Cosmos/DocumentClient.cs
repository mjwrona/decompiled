// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.DocumentClient
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Azure.Core;
using Microsoft.Azure.Cosmos.Common;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Cosmos.Query;
using Microsoft.Azure.Cosmos.Query.Core;
using Microsoft.Azure.Cosmos.Resource.CosmosExceptions;
using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Telemetry;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Cosmos.Tracing.TraceData;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Collections;
using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal class DocumentClient : 
    IDisposable,
    IAuthorizationTokenProvider,
    ICosmosAuthorizationTokenProvider,
    IDocumentClient,
    IDocumentClientInternal
  {
    private const string AllowOverrideStrongerConsistency = "AllowOverrideStrongerConsistency";
    private const string MaxConcurrentConnectionOpenConfig = "MaxConcurrentConnectionOpenRequests";
    private const string IdleConnectionTimeoutInSecondsConfig = "IdleConnectionTimeoutInSecondsConfig";
    private const string OpenConnectionTimeoutInSecondsConfig = "OpenConnectionTimeoutInSecondsConfig";
    private const string TransportTimerPoolGranularityInSecondsConfig = "TransportTimerPoolGranularityInSecondsConfig";
    private const string EnableTcpChannelConfig = "CosmosDbEnableTcpChannel";
    private const string MaxRequestsPerChannelConfig = "CosmosDbMaxRequestsPerTcpChannel";
    private const string TcpPartitionCount = "CosmosDbTcpPartitionCount";
    private const string MaxChannelsPerHostConfig = "CosmosDbMaxTcpChannelsPerHost";
    private const string RntbdPortReuseMode = "CosmosDbTcpPortReusePolicy";
    private const string RntbdPortPoolReuseThreshold = "CosmosDbTcpPortReuseThreshold";
    private const string RntbdPortPoolBindAttempts = "CosmosDbTcpPortReuseBindAttempts";
    private const string RntbdReceiveHangDetectionTimeConfig = "CosmosDbTcpReceiveHangDetectionTimeSeconds";
    private const string RntbdSendHangDetectionTimeConfig = "CosmosDbTcpSendHangDetectionTimeSeconds";
    private const string EnableCpuMonitorConfig = "CosmosDbEnableCpuMonitor";
    private const string RntbdMaxConcurrentOpeningConnectionCountConfig = "AZURE_COSMOS_TCP_MAX_CONCURRENT_OPENING_CONNECTION_COUNT";
    private const int MaxConcurrentConnectionOpenRequestsPerProcessor = 25;
    private const int DefaultMaxRequestsPerRntbdChannel = 30;
    private const int DefaultRntbdPartitionCount = 1;
    private const int DefaultMaxRntbdChannelsPerHost = 65535;
    private const PortReuseMode DefaultRntbdPortReuseMode = PortReuseMode.ReuseUnicastPort;
    private const int DefaultRntbdPortPoolReuseThreshold = 256;
    private const int DefaultRntbdPortPoolBindAttempts = 5;
    private const int DefaultRntbdReceiveHangDetectionTimeSeconds = 65;
    private const int DefaultRntbdSendHangDetectionTimeSeconds = 10;
    private const bool DefaultEnableCpuMonitor = true;
    private const string DefaultInitTaskKey = "InitTaskKey";
    private readonly bool IsLocalQuorumConsistency;
    internal readonly AuthorizationTokenProvider cosmosAuthorization;
    private RetryPolicy retryPolicy;
    private bool allowOverrideStrongerConsistency;
    private int maxConcurrentConnectionOpenRequests = Environment.ProcessorCount * 25;
    private int openConnectionTimeoutInSeconds = 5;
    private int idleConnectionTimeoutInSeconds = -1;
    private int timerPoolGranularityInSeconds = 1;
    private bool enableRntbdChannel = true;
    private int maxRequestsPerRntbdChannel = 30;
    private int rntbdPartitionCount = 1;
    private int maxRntbdChannels = (int) ushort.MaxValue;
    private PortReuseMode rntbdPortReuseMode;
    private int rntbdPortPoolReuseThreshold = 256;
    private int rntbdPortPoolBindAttempts = 5;
    private int rntbdReceiveHangDetectionTimeSeconds = 65;
    private int rntbdSendHangDetectionTimeSeconds = 10;
    private bool enableCpuMonitor = true;
    private int rntbdMaxConcurrentOpeningConnectionCount = 5;
    private Microsoft.Azure.Documents.ConsistencyLevel? desiredConsistencyLevel;
    private ClientCollectionCache collectionCache;
    private PartitionKeyRangeCache partitionKeyRangeCache;
    private bool isSuccessfullyInitialized;
    private bool isDisposed;
    private IStoreClientFactory storeClientFactory;
    private bool isStoreClientFactoryCreatedInternally;
    private static int idCounter;
    private int traceId;
    internal ISessionContainer sessionContainer;
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private Microsoft.Azure.Cosmos.Common.AsyncLazy<Microsoft.Azure.Cosmos.Query.Core.QueryPlan.QueryPartitionProvider> queryPartitionProvider;
    private DocumentClientEventSource eventSource;
    private Func<bool, Task<bool>> initializeTaskFactory;
    internal AsyncCacheNonBlocking<string, bool> initTaskCache;
    private JsonSerializerSettings serializerSettings;
    private Func<TransportClient, TransportClient> transportClientHandlerFactory;

    internal CosmosAccountServiceConfiguration accountServiceConfiguration { get; private set; }

    internal CosmosHttpClient httpClient { get; private set; }

    private event EventHandler<SendingRequestEventArgs> sendingRequest;

    private event EventHandler<ReceivedResponseEventArgs> receivedResponse;

    public DocumentClient(
      Uri serviceEndpoint,
      SecureString authKey,
      ConnectionPolicy connectionPolicy = null,
      Microsoft.Azure.Documents.ConsistencyLevel? desiredConsistencyLevel = null)
    {
      if (authKey == null)
        throw new ArgumentNullException(nameof (authKey));
      if (authKey != null)
        this.cosmosAuthorization = (AuthorizationTokenProvider) new AuthorizationTokenProviderMasterKey(authKey);
      this.Initialize(serviceEndpoint, connectionPolicy, desiredConsistencyLevel);
      this.initTaskCache = new AsyncCacheNonBlocking<string, bool>(cancellationToken: this.cancellationTokenSource.Token);
    }

    [Obsolete("Please use the constructor that takes JsonSerializerSettings as the third parameter.")]
    public DocumentClient(
      Uri serviceEndpoint,
      SecureString authKey,
      ConnectionPolicy connectionPolicy,
      Microsoft.Azure.Documents.ConsistencyLevel? desiredConsistencyLevel,
      JsonSerializerSettings serializerSettings)
      : this(serviceEndpoint, authKey, connectionPolicy, desiredConsistencyLevel)
    {
      this.serializerSettings = serializerSettings;
    }

    public DocumentClient(
      Uri serviceEndpoint,
      SecureString authKey,
      JsonSerializerSettings serializerSettings,
      ConnectionPolicy connectionPolicy = null,
      Microsoft.Azure.Documents.ConsistencyLevel? desiredConsistencyLevel = null)
      : this(serviceEndpoint, authKey, connectionPolicy, desiredConsistencyLevel)
    {
      this.serializerSettings = serializerSettings;
    }

    public DocumentClient(
      Uri serviceEndpoint,
      string authKeyOrResourceToken,
      ConnectionPolicy connectionPolicy = null,
      Microsoft.Azure.Documents.ConsistencyLevel? desiredConsistencyLevel = null)
      : this(serviceEndpoint, authKeyOrResourceToken, (EventHandler<SendingRequestEventArgs>) null, connectionPolicy, desiredConsistencyLevel)
    {
    }

    public DocumentClient(
      Uri serviceEndpoint,
      string authKeyOrResourceToken,
      HttpMessageHandler handler,
      ConnectionPolicy connectionPolicy = null,
      Microsoft.Azure.Documents.ConsistencyLevel? desiredConsistencyLevel = null)
      : this(serviceEndpoint, authKeyOrResourceToken, (EventHandler<SendingRequestEventArgs>) null, connectionPolicy, desiredConsistencyLevel, handler: handler)
    {
    }

    internal DocumentClient(
      Uri serviceEndpoint,
      string authKeyOrResourceToken,
      EventHandler<SendingRequestEventArgs> sendingRequestEventArgs,
      ConnectionPolicy connectionPolicy = null,
      Microsoft.Azure.Documents.ConsistencyLevel? desiredConsistencyLevel = null,
      JsonSerializerSettings serializerSettings = null,
      ApiType apitype = ApiType.None,
      EventHandler<ReceivedResponseEventArgs> receivedResponseEventArgs = null,
      HttpMessageHandler handler = null,
      ISessionContainer sessionContainer = null,
      bool? enableCpuMonitor = null,
      Func<TransportClient, TransportClient> transportClientHandlerFactory = null,
      IStoreClientFactory storeClientFactory = null)
      : this(serviceEndpoint, AuthorizationTokenProvider.CreateWithResourceTokenOrAuthKey(authKeyOrResourceToken), sendingRequestEventArgs, connectionPolicy, desiredConsistencyLevel, serializerSettings, apitype, receivedResponseEventArgs, handler, sessionContainer, enableCpuMonitor, transportClientHandlerFactory, storeClientFactory)
    {
    }

    internal DocumentClient(
      Uri serviceEndpoint,
      AuthorizationTokenProvider cosmosAuthorization,
      EventHandler<SendingRequestEventArgs> sendingRequestEventArgs,
      ConnectionPolicy connectionPolicy = null,
      Microsoft.Azure.Documents.ConsistencyLevel? desiredConsistencyLevel = null,
      JsonSerializerSettings serializerSettings = null,
      ApiType apitype = ApiType.None,
      EventHandler<ReceivedResponseEventArgs> receivedResponseEventArgs = null,
      HttpMessageHandler handler = null,
      ISessionContainer sessionContainer = null,
      bool? enableCpuMonitor = null,
      Func<TransportClient, TransportClient> transportClientHandlerFactory = null,
      IStoreClientFactory storeClientFactory = null,
      bool isLocalQuorumConsistency = false)
    {
      if (sendingRequestEventArgs != null)
        this.sendingRequest += sendingRequestEventArgs;
      if (serializerSettings != null)
        this.serializerSettings = serializerSettings;
      this.ApiType = apitype;
      if (receivedResponseEventArgs != null)
        this.receivedResponse += receivedResponseEventArgs;
      this.cosmosAuthorization = cosmosAuthorization ?? throw new ArgumentNullException(nameof (cosmosAuthorization));
      this.transportClientHandlerFactory = transportClientHandlerFactory;
      this.IsLocalQuorumConsistency = isLocalQuorumConsistency;
      this.initTaskCache = new AsyncCacheNonBlocking<string, bool>(cancellationToken: this.cancellationTokenSource.Token);
      this.Initialize(serviceEndpoint, connectionPolicy, desiredConsistencyLevel, handler, sessionContainer, enableCpuMonitor, storeClientFactory);
    }

    [Obsolete("Please use the constructor that takes JsonSerializerSettings as the third parameter.")]
    public DocumentClient(
      Uri serviceEndpoint,
      string authKeyOrResourceToken,
      ConnectionPolicy connectionPolicy,
      Microsoft.Azure.Documents.ConsistencyLevel? desiredConsistencyLevel,
      JsonSerializerSettings serializerSettings)
      : this(serviceEndpoint, authKeyOrResourceToken, (HttpMessageHandler) null, connectionPolicy, desiredConsistencyLevel)
    {
      this.serializerSettings = serializerSettings;
    }

    public DocumentClient(
      Uri serviceEndpoint,
      string authKeyOrResourceToken,
      JsonSerializerSettings serializerSettings,
      ConnectionPolicy connectionPolicy = null,
      Microsoft.Azure.Documents.ConsistencyLevel? desiredConsistencyLevel = null)
      : this(serviceEndpoint, authKeyOrResourceToken, (HttpMessageHandler) null, connectionPolicy, desiredConsistencyLevel)
    {
      this.serializerSettings = serializerSettings;
    }

    internal DocumentClient(Uri serviceEndpoint, string authKey)
    {
      this.ServiceEndpoint = serviceEndpoint;
      this.ConnectionPolicy = new ConnectionPolicy();
    }

    internal virtual async Task<ClientCollectionCache> GetCollectionCacheAsync(ITrace trace)
    {
      ClientCollectionCache collectionCache;
      using (ITrace childTrace = trace.StartChild("Get Collection Cache", TraceComponent.Routing, TraceLevel.Info))
      {
        await this.EnsureValidClientAsync(childTrace);
        collectionCache = this.collectionCache;
      }
      return collectionCache;
    }

    internal virtual async Task<PartitionKeyRangeCache> GetPartitionKeyRangeCacheAsync(ITrace trace)
    {
      PartitionKeyRangeCache partitionKeyRangeCache;
      using (ITrace childTrace = trace.StartChild("Get Partition Key Range Cache", TraceComponent.Routing, TraceLevel.Info))
      {
        await this.EnsureValidClientAsync(childTrace);
        partitionKeyRangeCache = this.partitionKeyRangeCache;
      }
      return partitionKeyRangeCache;
    }

    internal GlobalAddressResolver AddressResolver { get; private set; }

    internal GlobalEndpointManager GlobalEndpointManager { get; private set; }

    internal GlobalPartitionEndpointManager PartitionKeyRangeLocation { get; private set; }

    public Task OpenAsync(CancellationToken cancellationToken = default (CancellationToken)) => TaskHelper.InlineIfPossibleAsync((Func<Task>) (() => this.OpenPrivateInlineAsync(cancellationToken)), (IRetryPolicy) null, cancellationToken);

    private async Task OpenPrivateInlineAsync(CancellationToken cancellationToken)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      await TaskHelper.InlineIfPossibleAsync((Func<Task>) (() => this.OpenPrivateAsync(cancellationToken)), (IRetryPolicy) this.ResetSessionTokenRetryPolicy.GetRequestPolicy(), cancellationToken);
    }

    private async Task OpenPrivateAsync(CancellationToken cancellationToken)
    {
      DocumentClient documentClient = this;
      ResourceFeedReader<Microsoft.Azure.Documents.Database> databaseFeedReader = documentClient.CreateDatabaseFeedReader(new FeedOptions()
      {
        MaxItemCount = new int?(-1)
      });
      try
      {
        while (databaseFeedReader.HasMoreResults)
        {
          foreach (Microsoft.Azure.Documents.Database database1 in await databaseFeedReader.ExecuteNextAsync(cancellationToken))
          {
            Microsoft.Azure.Documents.Database database = database1;
            ResourceFeedReader<DocumentCollection> collectionFeedReader = documentClient.CreateDocumentCollectionFeedReader(database.SelfLink, new FeedOptions()
            {
              MaxItemCount = new int?(-1)
            });
            List<Task> tasks = new List<Task>();
            while (collectionFeedReader.HasMoreResults)
            {
              List<Task> taskList = tasks;
              taskList.AddRange((await collectionFeedReader.ExecuteNextAsync(cancellationToken)).Select<DocumentCollection, Task>((Func<DocumentCollection, Task>) (collection => this.InitializeCachesAsync(database.Id, collection, cancellationToken))));
              taskList = (List<Task>) null;
            }
            await Task.WhenAll((IEnumerable<Task>) tasks);
            collectionFeedReader = (ResourceFeedReader<DocumentCollection>) null;
            tasks = (List<Task>) null;
          }
        }
        databaseFeedReader = (ResourceFeedReader<Microsoft.Azure.Documents.Database>) null;
      }
      catch (DocumentClientException ex)
      {
        documentClient.collectionCache = new ClientCollectionCache(documentClient.sessionContainer, documentClient.GatewayStoreModel, (ICosmosAuthorizationTokenProvider) documentClient, (IRetryPolicyFactory) documentClient.retryPolicy);
        documentClient.partitionKeyRangeCache = new PartitionKeyRangeCache((ICosmosAuthorizationTokenProvider) documentClient, documentClient.GatewayStoreModel, (CollectionCache) documentClient.collectionCache);
        DefaultTrace.TraceWarning("{0} occurred while OpenAsync. Exception Message: {1}", (object) ex.ToString(), (object) ex.Message);
        databaseFeedReader = (ResourceFeedReader<Microsoft.Azure.Documents.Database>) null;
      }
    }

    internal virtual void Initialize(
      Uri serviceEndpoint,
      ConnectionPolicy connectionPolicy = null,
      Microsoft.Azure.Documents.ConsistencyLevel? desiredConsistencyLevel = null,
      HttpMessageHandler handler = null,
      ISessionContainer sessionContainer = null,
      bool? enableCpuMonitor = null,
      IStoreClientFactory storeClientFactory = null,
      TokenCredential tokenCredential = null)
    {
      if (serviceEndpoint == (Uri) null)
        throw new ArgumentNullException(nameof (serviceEndpoint));
      this.queryPartitionProvider = new Microsoft.Azure.Cosmos.Common.AsyncLazy<Microsoft.Azure.Cosmos.Query.Core.QueryPlan.QueryPartitionProvider>((Func<Task<Microsoft.Azure.Cosmos.Query.Core.QueryPlan.QueryPartitionProvider>>) (async () =>
      {
        await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
        return new Microsoft.Azure.Cosmos.Query.Core.QueryPlan.QueryPartitionProvider(this.accountServiceConfiguration.QueryEngineConfiguration);
      }), CancellationToken.None);
      if (Assembly.GetEntryAssembly() != (Assembly) null)
      {
        string appSetting1 = ConfigurationManager.AppSettings["AllowOverrideStrongerConsistency"];
        if (!string.IsNullOrEmpty(appSetting1) && !bool.TryParse(appSetting1, out this.allowOverrideStrongerConsistency))
          this.allowOverrideStrongerConsistency = false;
        string appSetting2 = ConfigurationManager.AppSettings["MaxConcurrentConnectionOpenRequests"];
        if (!string.IsNullOrEmpty(appSetting2))
        {
          int result = 0;
          if (int.TryParse(appSetting2, out result))
            this.maxConcurrentConnectionOpenRequests = result;
        }
        string appSetting3 = ConfigurationManager.AppSettings["OpenConnectionTimeoutInSecondsConfig"];
        if (!string.IsNullOrEmpty(appSetting3))
        {
          int result = 0;
          if (int.TryParse(appSetting3, out result))
            this.openConnectionTimeoutInSeconds = result;
        }
        string appSetting4 = ConfigurationManager.AppSettings["IdleConnectionTimeoutInSecondsConfig"];
        if (!string.IsNullOrEmpty(appSetting4))
        {
          int result = 0;
          if (int.TryParse(appSetting4, out result))
            this.idleConnectionTimeoutInSeconds = result;
        }
        string appSetting5 = ConfigurationManager.AppSettings["TransportTimerPoolGranularityInSecondsConfig"];
        if (!string.IsNullOrEmpty(appSetting5))
        {
          int result = 0;
          if (int.TryParse(appSetting5, out result) && result > this.timerPoolGranularityInSeconds)
            this.timerPoolGranularityInSeconds = result;
        }
        string appSetting6 = ConfigurationManager.AppSettings["CosmosDbEnableTcpChannel"];
        if (!string.IsNullOrEmpty(appSetting6))
        {
          bool result = false;
          if (bool.TryParse(appSetting6, out result))
            this.enableRntbdChannel = result;
        }
        string appSetting7 = ConfigurationManager.AppSettings["CosmosDbMaxRequestsPerTcpChannel"];
        if (!string.IsNullOrEmpty(appSetting7))
        {
          int result = 30;
          if (int.TryParse(appSetting7, out result))
            this.maxRequestsPerRntbdChannel = result;
        }
        string appSetting8 = ConfigurationManager.AppSettings["CosmosDbTcpPartitionCount"];
        if (!string.IsNullOrEmpty(appSetting8))
        {
          int result = 1;
          if (int.TryParse(appSetting8, out result))
            this.rntbdPartitionCount = result;
        }
        string appSetting9 = ConfigurationManager.AppSettings["CosmosDbMaxTcpChannelsPerHost"];
        if (!string.IsNullOrEmpty(appSetting9))
        {
          int result = (int) ushort.MaxValue;
          if (int.TryParse(appSetting9, out result))
            this.maxRntbdChannels = result;
        }
        string appSetting10 = ConfigurationManager.AppSettings["CosmosDbTcpPortReusePolicy"];
        if (!string.IsNullOrEmpty(appSetting10))
        {
          PortReuseMode result = PortReuseMode.ReuseUnicastPort;
          if (Enum.TryParse<PortReuseMode>(appSetting10, out result))
            this.rntbdPortReuseMode = result;
        }
        string appSetting11 = ConfigurationManager.AppSettings["CosmosDbTcpPortReuseThreshold"];
        if (!string.IsNullOrEmpty(appSetting11))
        {
          int result = 256;
          if (int.TryParse(appSetting11, out result))
            this.rntbdPortPoolReuseThreshold = result;
        }
        string appSetting12 = ConfigurationManager.AppSettings["CosmosDbTcpPortReuseBindAttempts"];
        if (!string.IsNullOrEmpty(appSetting12))
        {
          int result = 5;
          if (int.TryParse(appSetting12, out result))
            this.rntbdPortPoolBindAttempts = result;
        }
        string appSetting13 = ConfigurationManager.AppSettings["CosmosDbTcpReceiveHangDetectionTimeSeconds"];
        if (!string.IsNullOrEmpty(appSetting13))
        {
          int result = 65;
          if (int.TryParse(appSetting13, out result))
            this.rntbdReceiveHangDetectionTimeSeconds = result;
        }
        string appSetting14 = ConfigurationManager.AppSettings["CosmosDbTcpSendHangDetectionTimeSeconds"];
        if (!string.IsNullOrEmpty(appSetting14))
        {
          int result = 10;
          if (int.TryParse(appSetting14, out result))
            this.rntbdSendHangDetectionTimeSeconds = result;
        }
        if (enableCpuMonitor.HasValue)
        {
          this.enableCpuMonitor = enableCpuMonitor.Value;
        }
        else
        {
          string appSetting15 = ConfigurationManager.AppSettings["CosmosDbEnableCpuMonitor"];
          if (!string.IsNullOrEmpty(appSetting15))
          {
            bool result = true;
            if (bool.TryParse(appSetting15, out result))
              this.enableCpuMonitor = result;
          }
        }
      }
      string environmentVariable = Environment.GetEnvironmentVariable("AZURE_COSMOS_TCP_MAX_CONCURRENT_OPENING_CONNECTION_COUNT");
      int result1;
      if (!string.IsNullOrEmpty(environmentVariable) && int.TryParse(environmentVariable, out result1))
        this.rntbdMaxConcurrentOpeningConnectionCount = result1 > 0 ? result1 : throw new ArgumentException("RntbdMaxConcurrentOpeningConnectionCountConfig should be larger than 0");
      if (connectionPolicy != null)
      {
        TimeSpan? connectionTimeout;
        TimeSpan timeSpan;
        if (connectionPolicy.IdleTcpConnectionTimeout.HasValue)
        {
          connectionTimeout = connectionPolicy.IdleTcpConnectionTimeout;
          timeSpan = connectionTimeout.Value;
          this.idleConnectionTimeoutInSeconds = (int) timeSpan.TotalSeconds;
        }
        connectionTimeout = connectionPolicy.OpenTcpConnectionTimeout;
        if (connectionTimeout.HasValue)
        {
          connectionTimeout = connectionPolicy.OpenTcpConnectionTimeout;
          timeSpan = connectionTimeout.Value;
          this.openConnectionTimeoutInSeconds = (int) timeSpan.TotalSeconds;
        }
        int? nullable;
        if (connectionPolicy.MaxRequestsPerTcpConnection.HasValue)
        {
          nullable = connectionPolicy.MaxRequestsPerTcpConnection;
          this.maxRequestsPerRntbdChannel = nullable.Value;
        }
        nullable = connectionPolicy.MaxTcpPartitionCount;
        if (nullable.HasValue)
        {
          nullable = connectionPolicy.MaxTcpPartitionCount;
          this.rntbdPartitionCount = nullable.Value;
        }
        nullable = connectionPolicy.MaxTcpConnectionsPerEndpoint;
        if (nullable.HasValue)
        {
          nullable = connectionPolicy.MaxTcpConnectionsPerEndpoint;
          this.maxRntbdChannels = nullable.Value;
        }
        if (connectionPolicy.PortReuseMode.HasValue)
          this.rntbdPortReuseMode = connectionPolicy.PortReuseMode.Value;
      }
      this.ServiceEndpoint = serviceEndpoint.OriginalString.EndsWith("/", StringComparison.Ordinal) ? serviceEndpoint : new Uri(serviceEndpoint.OriginalString + "/");
      this.ConnectionPolicy = connectionPolicy ?? ConnectionPolicy.Default;
      ServicePointAccessor.FindServicePoint(this.ServiceEndpoint).ConnectionLimit = this.ConnectionPolicy.MaxConnectionLimit;
      this.GlobalEndpointManager = new GlobalEndpointManager((IDocumentClientInternal) this, this.ConnectionPolicy);
      this.PartitionKeyRangeLocation = this.ConnectionPolicy.EnablePartitionLevelFailover ? (GlobalPartitionEndpointManager) new GlobalPartitionEndpointManagerCore((IGlobalEndpointManager) this.GlobalEndpointManager) : GlobalPartitionEndpointManagerNoOp.Instance;
      this.httpClient = CosmosHttpClientCore.CreateWithConnectionPolicy(this.ApiType, (ICommunicationEventSource) DocumentClientEventSource.Instance, this.ConnectionPolicy, handler, this.sendingRequest, this.receivedResponse);
      VmMetadataApiHandler.TryInitialize(this.httpClient);
      this.sessionContainer = sessionContainer == null ? (ISessionContainer) new SessionContainer(this.ServiceEndpoint.Host) : sessionContainer;
      this.retryPolicy = new RetryPolicy(this.GlobalEndpointManager, this.ConnectionPolicy, this.PartitionKeyRangeLocation);
      this.ResetSessionTokenRetryPolicy = (IRetryPolicyFactory) this.retryPolicy;
      this.desiredConsistencyLevel = desiredConsistencyLevel;
      this.eventSource = DocumentClientEventSource.Instance;
      this.initializeTaskFactory = (Func<bool, Task<bool>>) (_ => TaskHelper.InlineIfPossible<bool>((Func<Task<bool>>) (() => this.GetInitializationTaskAsync(storeClientFactory)), (IRetryPolicy) new ResourceThrottleRetryPolicy(this.ConnectionPolicy.RetryOptions.MaxRetryAttemptsOnThrottledRequests, this.ConnectionPolicy.RetryOptions.MaxRetryWaitTimeInSeconds)));
      this.initTaskCache.GetAsync("InitTaskKey", this.initializeTaskFactory, (Func<bool, bool>) (_ => false)).ContinueWith((Action<Task>) (t => DefaultTrace.TraceWarning("initializeTask failed {0}", (object) t.Exception)), TaskContinuationOptions.OnlyOnFaulted);
      this.traceId = Interlocked.Increment(ref DocumentClient.idCounter);
      DefaultTrace.TraceInformation(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DocumentClient with id {0} initialized at endpoint: {1} with ConnectionMode: {2}, connection Protocol: {3}, and consistency level: {4}", (object) this.traceId, (object) serviceEndpoint.ToString(), (object) this.ConnectionPolicy.ConnectionMode.ToString(), (object) this.ConnectionPolicy.ConnectionProtocol.ToString(), desiredConsistencyLevel.HasValue ? (object) desiredConsistencyLevel.ToString() : (object) "null"));
      this.QueryCompatibilityMode = QueryCompatibilityMode.Default;
    }

    private async Task<bool> GetInitializationTaskAsync(IStoreClientFactory storeClientFactory)
    {
      DocumentClient tokenProvider = this;
      await tokenProvider.InitializeGatewayConfigurationReaderAsync();
      if (tokenProvider.desiredConsistencyLevel.HasValue)
        tokenProvider.EnsureValidOverwrite(tokenProvider.desiredConsistencyLevel.Value);
      Microsoft.Azure.Cosmos.GatewayStoreModel gatewayStoreModel = new Microsoft.Azure.Cosmos.GatewayStoreModel(tokenProvider.GlobalEndpointManager, tokenProvider.sessionContainer, (ConsistencyLevel) tokenProvider.accountServiceConfiguration.DefaultConsistencyLevel, tokenProvider.eventSource, tokenProvider.serializerSettings, tokenProvider.httpClient);
      tokenProvider.GatewayStoreModel = (IStoreModel) gatewayStoreModel;
      tokenProvider.collectionCache = new ClientCollectionCache(tokenProvider.sessionContainer, tokenProvider.GatewayStoreModel, (ICosmosAuthorizationTokenProvider) tokenProvider, (IRetryPolicyFactory) tokenProvider.retryPolicy);
      tokenProvider.partitionKeyRangeCache = new PartitionKeyRangeCache((ICosmosAuthorizationTokenProvider) tokenProvider, tokenProvider.GatewayStoreModel, (CollectionCache) tokenProvider.collectionCache);
      tokenProvider.ResetSessionTokenRetryPolicy = (IRetryPolicyFactory) new DocumentClient.ResetSessionTokenRetryPolicyFactory(tokenProvider.sessionContainer, tokenProvider.collectionCache, (IRetryPolicyFactory) tokenProvider.retryPolicy);
      gatewayStoreModel.SetCaches(tokenProvider.partitionKeyRangeCache, tokenProvider.collectionCache);
      // ISSUE: explicit non-virtual call
      if (__nonvirtual (tokenProvider.ConnectionPolicy).ConnectionMode == ConnectionMode.Gateway)
        tokenProvider.StoreModel = tokenProvider.GatewayStoreModel;
      else
        tokenProvider.InitializeDirectConnectivity(storeClientFactory);
      return true;
    }

    private async Task InitializeCachesAsync(
      string databaseName,
      DocumentCollection collection,
      CancellationToken cancellationToken)
    {
      if (databaseName == null)
        throw new ArgumentNullException(nameof (databaseName));
      if (collection == null)
        throw new ArgumentNullException(nameof (collection));
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Query, ResourceType.Document, collection.SelfLink, AuthorizationTokenType.PrimaryMasterKey))
      {
        ContainerProperties resolvedCollection = await (await this.GetCollectionCacheAsync((ITrace) NoOpTrace.Singleton)).ResolveCollectionAsync(request, CancellationToken.None, (ITrace) NoOpTrace.Singleton);
        IReadOnlyList<PartitionKeyRange> overlappingRangesAsync = await this.partitionKeyRangeCache.TryGetOverlappingRangesAsync(resolvedCollection.ResourceId, new Range<string>(PartitionKeyInternal.MinimumInclusiveEffectivePartitionKey, PartitionKeyInternal.MaximumExclusiveEffectivePartitionKey, true, false), (ITrace) NoOpTrace.Singleton, false);
        if (this.AddressResolver != null)
          await this.AddressResolver.OpenAsync(databaseName, resolvedCollection, cancellationToken);
        resolvedCollection = (ContainerProperties) null;
      }
    }

    public object Session
    {
      get => (object) this.sessionContainer;
      set
      {
        if (!(value is SessionContainer comrade))
          throw new ArgumentNullException(nameof (value));
        if (!string.Equals(this.ServiceEndpoint.Host, comrade.HostName, StringComparison.OrdinalIgnoreCase))
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ClientResources.BadSession, (object) comrade.HostName, (object) this.ServiceEndpoint.Host));
        if (!(this.sessionContainer is SessionContainer sessionContainer))
          throw new ArgumentNullException("currentSessionContainer");
        sessionContainer.ReplaceCurrrentStateWithStateOf(comrade);
      }
    }

    internal string GetSessionToken(string collectionLink) => this.sessionContainer is SessionContainer sessionContainer ? sessionContainer.GetSessionToken(collectionLink) : throw new ArgumentNullException("sessionContainerInternal");

    internal ApiType ApiType { get; private set; }

    internal bool UseMultipleWriteLocations { get; private set; }

    public Uri ServiceEndpoint { get; private set; }

    public Uri WriteEndpoint => this.GlobalEndpointManager.WriteEndpoints.FirstOrDefault<Uri>();

    public Uri ReadEndpoint => this.GlobalEndpointManager.ReadEndpoints.FirstOrDefault<Uri>();

    public ConnectionPolicy ConnectionPolicy { get; private set; }

    public SecureString AuthKey => throw new NotSupportedException("Please use CosmosAuthorization");

    public virtual Microsoft.Azure.Documents.ConsistencyLevel ConsistencyLevel
    {
      get
      {
        TaskHelper.InlineIfPossibleAsync((Func<Task>) (() => this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton)), (IRetryPolicy) null).Wait();
        return !this.desiredConsistencyLevel.HasValue ? this.accountServiceConfiguration.DefaultConsistencyLevel : this.desiredConsistencyLevel.Value;
      }
    }

    public void Dispose()
    {
      if (this.isDisposed)
        return;
      if (!this.cancellationTokenSource.IsCancellationRequested)
        this.cancellationTokenSource.Cancel();
      this.cancellationTokenSource.Dispose();
      if (this.StoreModel != null)
      {
        this.StoreModel.Dispose();
        this.StoreModel = (IStoreModel) null;
      }
      if (this.storeClientFactory != null)
      {
        if (this.isStoreClientFactoryCreatedInternally)
          this.storeClientFactory.Dispose();
        this.storeClientFactory = (IStoreClientFactory) null;
      }
      if (this.AddressResolver != null)
      {
        this.AddressResolver.Dispose();
        this.AddressResolver = (GlobalAddressResolver) null;
      }
      if (this.httpClient != null)
      {
        try
        {
          this.httpClient.Dispose();
        }
        catch (Exception ex)
        {
          DefaultTrace.TraceWarning("Exception {0} thrown during dispose of HttpClient, this could happen if there are inflight request during the dispose of client", (object) ex);
        }
        this.httpClient = (CosmosHttpClient) null;
      }
      if (this.cosmosAuthorization != null)
        this.cosmosAuthorization.Dispose();
      if (this.GlobalEndpointManager != null)
      {
        this.GlobalEndpointManager.Dispose();
        this.GlobalEndpointManager = (GlobalEndpointManager) null;
      }
      if (this.queryPartitionProvider != null && this.queryPartitionProvider.IsValueCreated)
        this.queryPartitionProvider.Value.Dispose();
      if (this.initTaskCache != null)
      {
        this.initTaskCache.Dispose();
        this.initTaskCache = (AsyncCacheNonBlocking<string, bool>) null;
      }
      DefaultTrace.TraceInformation("DocumentClient with id {0} disposed.", (object) this.traceId);
      DefaultTrace.Flush();
      this.isDisposed = true;
    }

    internal QueryCompatibilityMode QueryCompatibilityMode { get; set; }

    internal virtual IRetryPolicyFactory ResetSessionTokenRetryPolicy { get; private set; }

    internal IStoreModel StoreModel { get; set; }

    internal IStoreModel GatewayStoreModel { get; set; }

    internal Action<IQueryable> OnExecuteScalarQueryCallback { get; set; }

    internal virtual Task<Microsoft.Azure.Cosmos.Query.Core.QueryPlan.QueryPartitionProvider> QueryPartitionProvider => this.queryPartitionProvider.Value;

    internal virtual async Task<ConsistencyLevel> GetDefaultConsistencyLevelAsync()
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      return (ConsistencyLevel) this.accountServiceConfiguration.DefaultConsistencyLevel;
    }

    internal Task<Microsoft.Azure.Documents.ConsistencyLevel?> GetDesiredConsistencyLevelAsync() => Task.FromResult<Microsoft.Azure.Documents.ConsistencyLevel?>(this.desiredConsistencyLevel);

    internal async Task<DocumentServiceResponse> ProcessRequestAsync(
      string verb,
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken,
      string testAuthorization = null)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      if (verb == null)
        throw new ArgumentNullException(nameof (verb));
      (string authorizationToken, string payload) = await this.cosmosAuthorization.GetUserAuthorizationAsync(request.ResourceAddress, PathsHelper.GetResourcePath(request.ResourceType), verb, request.Headers, AuthorizationTokenType.PrimaryMasterKey);
      if (testAuthorization != null)
      {
        payload = testAuthorization;
        authorizationToken = testAuthorization;
      }
      request.Headers["authorization"] = authorizationToken;
      DocumentServiceResponse documentServiceResponse;
      try
      {
        documentServiceResponse = await this.ProcessRequestAsync(request, retryPolicyInstance, cancellationToken);
      }
      catch (DocumentClientException ex)
      {
        this.cosmosAuthorization.TraceUnauthorized(ex, authorizationToken, payload);
        throw;
      }
      authorizationToken = (string) null;
      payload = (string) null;
      return documentServiceResponse;
    }

    internal Task<DocumentServiceResponse> ProcessRequestAsync(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      return this.ProcessRequestAsync(request, retryPolicyInstance, (ITrace) NoOpTrace.Singleton, cancellationToken);
    }

    internal async Task<DocumentServiceResponse> ProcessRequestAsync(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicyInstance,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      await this.EnsureValidClientAsync(trace);
      retryPolicyInstance?.OnBeforeSendRequest(request);
      DocumentServiceResponse documentServiceResponse;
      using (new ActivityScope(Guid.NewGuid()))
        documentServiceResponse = await this.GetStoreProxy(request).ProcessMessageAsync(request, cancellationToken);
      return documentServiceResponse;
    }

    private static string NormalizeAuthorizationPayload(string input)
    {
      StringBuilder stringBuilder = new StringBuilder(input.Length + 12);
      for (int index = 0; index < input.Length; ++index)
      {
        switch (input[index])
        {
          case '\n':
            stringBuilder.Append("\\n");
            break;
          case '/':
            stringBuilder.Append("\\/");
            break;
          default:
            stringBuilder.Append(input[index]);
            break;
        }
      }
      return stringBuilder.ToString();
    }

    internal RntbdConnectionConfig RecordTcpSettings(
      ClientConfigurationTraceDatum clientConfigurationTraceDatum)
    {
      return new RntbdConnectionConfig(this.openConnectionTimeoutInSeconds, this.idleConnectionTimeoutInSeconds, this.maxRequestsPerRntbdChannel, this.maxRntbdChannels, this.ConnectionPolicy.EnableTcpConnectionEndpointRediscovery, this.rntbdPortReuseMode);
    }

    internal virtual async Task EnsureValidClientAsync(ITrace trace)
    {
      if (this.cancellationTokenSource.IsCancellationRequested || this.isSuccessfullyInitialized)
        return;
      using (ITrace childTrace = trace.StartChild("Waiting for Initialization of client to complete", TraceComponent.Unknown, TraceLevel.Info))
      {
        try
        {
          this.isSuccessfullyInitialized = await this.initTaskCache.GetAsync("InitTaskKey", this.initializeTaskFactory, (Func<bool, bool>) (_ => false));
        }
        catch (DocumentClientException ex)
        {
          ITrace trace1 = trace;
          throw CosmosExceptionFactory.Create(ex, trace1);
        }
        catch (Exception ex)
        {
          DefaultTrace.TraceWarning("EnsureValidClientAsync initializeTask failed {0}", (object) ex);
          childTrace.AddDatum("initializeTask failed", (object) ex);
          throw;
        }
      }
    }

    public Task<ResourceResponse<Microsoft.Azure.Documents.Database>> CreateDatabaseAsync(
      Microsoft.Azure.Documents.Database database,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Microsoft.Azure.Documents.Database>>((Func<Task<ResourceResponse<Microsoft.Azure.Documents.Database>>>) (() => this.CreateDatabasePrivateAsync(database, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Microsoft.Azure.Documents.Database>> CreateDatabasePrivateAsync(
      Microsoft.Azure.Documents.Database database,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (database == null)
        throw new ArgumentNullException(nameof (database));
      this.ValidateResource((Microsoft.Azure.Documents.Resource) database);
      ResourceResponse<Microsoft.Azure.Documents.Database> databasePrivateAsync;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Create, "//dbs/", (Microsoft.Azure.Documents.Resource) database, ResourceType.Database, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Create, ResourceType.Database)))
        databasePrivateAsync = new ResourceResponse<Microsoft.Azure.Documents.Database>(await this.CreateAsync(request, retryPolicyInstance));
      return databasePrivateAsync;
    }

    public Task<ResourceResponse<Microsoft.Azure.Documents.Database>> CreateDatabaseIfNotExistsAsync(
      Microsoft.Azure.Documents.Database database,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      return TaskHelper.InlineIfPossible<ResourceResponse<Microsoft.Azure.Documents.Database>>((Func<Task<ResourceResponse<Microsoft.Azure.Documents.Database>>>) (() => this.CreateDatabaseIfNotExistsPrivateAsync(database, options)), (IRetryPolicy) null);
    }

    private async Task<ResourceResponse<Microsoft.Azure.Documents.Database>> CreateDatabaseIfNotExistsPrivateAsync(
      Microsoft.Azure.Documents.Database database,
      Microsoft.Azure.Documents.Client.RequestOptions options)
    {
      if (database == null)
        throw new ArgumentNullException(nameof (database));
      try
      {
        return await this.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(database.Id));
      }
      catch (DocumentClientException ex)
      {
        HttpStatusCode? statusCode = ex.StatusCode;
        HttpStatusCode httpStatusCode = HttpStatusCode.NotFound;
        if (!(statusCode.GetValueOrDefault() == httpStatusCode & statusCode.HasValue))
          throw;
      }
      try
      {
        return await this.CreateDatabaseAsync(database, options);
      }
      catch (DocumentClientException ex)
      {
        HttpStatusCode? statusCode = ex.StatusCode;
        HttpStatusCode httpStatusCode = HttpStatusCode.Conflict;
        if (!(statusCode.GetValueOrDefault() == httpStatusCode & statusCode.HasValue))
          throw;
      }
      return await this.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(database.Id));
    }

    public Task<ResourceResponse<Document>> CreateDocumentAsync(
      string documentsFeedOrDatabaseLink,
      object document,
      Microsoft.Azure.Documents.Client.RequestOptions options = null,
      bool disableAutomaticIdGeneration = false,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return TaskHelper.InlineIfPossible<ResourceResponse<Document>>((Func<Task<ResourceResponse<Document>>>) (() => this.CreateDocumentInlineAsync(documentsFeedOrDatabaseLink, document, options, disableAutomaticIdGeneration, cancellationToken)), (IRetryPolicy) null, cancellationToken);
    }

    private async Task<ResourceResponse<Document>> CreateDocumentInlineAsync(
      string documentsFeedOrDatabaseLink,
      object document,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      bool disableAutomaticIdGeneration,
      CancellationToken cancellationToken)
    {
      IDocumentClientRetryPolicy requestRetryPolicy = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      if (options?.PartitionKey == null)
        requestRetryPolicy = (IDocumentClientRetryPolicy) new PartitionKeyMismatchRetryPolicy((CollectionCache) await this.GetCollectionCacheAsync((ITrace) NoOpTrace.Singleton), requestRetryPolicy);
      return await TaskHelper.InlineIfPossible<ResourceResponse<Document>>((Func<Task<ResourceResponse<Document>>>) (() => this.CreateDocumentPrivateAsync(documentsFeedOrDatabaseLink, document, options, disableAutomaticIdGeneration, requestRetryPolicy, cancellationToken)), (IRetryPolicy) requestRetryPolicy);
    }

    private async Task<ResourceResponse<Document>> CreateDocumentPrivateAsync(
      string documentCollectionLink,
      object document,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      bool disableAutomaticIdGeneration,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(documentCollectionLink))
        throw new ArgumentNullException(nameof (documentCollectionLink));
      if (document == null)
        throw new ArgumentNullException(nameof (document));
      INameValueCollection requestHeaders = this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Create, ResourceType.Document);
      Document document1 = Document.FromObject(document, this.GetSerializerSettingsForRequest(options));
      this.ValidateResource((Microsoft.Azure.Documents.Resource) document1);
      if (string.IsNullOrEmpty(document1.Id) && !disableAutomaticIdGeneration)
        document1.Id = Guid.NewGuid().ToString();
      ResourceResponse<Document> documentPrivateAsync;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Create, documentCollectionLink, (Microsoft.Azure.Documents.Resource) document1, ResourceType.Document, AuthorizationTokenType.PrimaryMasterKey, requestHeaders, settings: this.GetSerializerSettingsForRequest(options)))
      {
        await this.AddPartitionKeyInformationAsync(request, document1, options);
        documentPrivateAsync = new ResourceResponse<Document>(await this.CreateAsync(request, retryPolicyInstance, cancellationToken));
      }
      return documentPrivateAsync;
    }

    public Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionAsync(
      string databaseLink,
      DocumentCollection documentCollection,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<DocumentCollection>>((Func<Task<ResourceResponse<DocumentCollection>>>) (() => this.CreateDocumentCollectionPrivateAsync(databaseLink, documentCollection, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionPrivateAsync(
      string databaseLink,
      DocumentCollection documentCollection,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(databaseLink))
        throw new ArgumentNullException(nameof (databaseLink));
      if (documentCollection == null)
        throw new ArgumentNullException(nameof (documentCollection));
      this.ValidateResource((Microsoft.Azure.Documents.Resource) documentCollection);
      ResourceResponse<DocumentCollection> collectionPrivateAsync;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Create, databaseLink, (Microsoft.Azure.Documents.Resource) documentCollection, ResourceType.Collection, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Create, ResourceType.Collection)))
      {
        ResourceResponse<DocumentCollection> resourceResponse = new ResourceResponse<DocumentCollection>(await this.CreateAsync(request, retryPolicyInstance));
        this.sessionContainer.SetSessionToken(resourceResponse.Resource.ResourceId, resourceResponse.Resource.AltLink, resourceResponse.Headers);
        collectionPrivateAsync = resourceResponse;
      }
      return collectionPrivateAsync;
    }

    public Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionIfNotExistsAsync(
      string databaseLink,
      DocumentCollection documentCollection,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      return TaskHelper.InlineIfPossible<ResourceResponse<DocumentCollection>>((Func<Task<ResourceResponse<DocumentCollection>>>) (() => this.CreateDocumentCollectionIfNotExistsPrivateAsync(databaseLink, documentCollection, options)), (IRetryPolicy) null);
    }

    private async Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionIfNotExistsPrivateAsync(
      string databaseLink,
      DocumentCollection documentCollection,
      Microsoft.Azure.Documents.Client.RequestOptions options)
    {
      if (string.IsNullOrEmpty(databaseLink))
        throw new ArgumentNullException(nameof (databaseLink));
      if (documentCollection == null)
        throw new ArgumentNullException(nameof (documentCollection));
      Microsoft.Azure.Documents.Database database = (Microsoft.Azure.Documents.Database) await this.ReadDatabaseAsync(databaseLink);
      try
      {
        return await this.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(database.Id, documentCollection.Id));
      }
      catch (DocumentClientException ex)
      {
        HttpStatusCode? statusCode = ex.StatusCode;
        HttpStatusCode httpStatusCode = HttpStatusCode.NotFound;
        if (!(statusCode.GetValueOrDefault() == httpStatusCode & statusCode.HasValue))
          throw;
      }
      try
      {
        return await this.CreateDocumentCollectionAsync(databaseLink, documentCollection, options);
      }
      catch (DocumentClientException ex)
      {
        HttpStatusCode? statusCode = ex.StatusCode;
        HttpStatusCode httpStatusCode = HttpStatusCode.Conflict;
        if (!(statusCode.GetValueOrDefault() == httpStatusCode & statusCode.HasValue))
          throw;
      }
      return await this.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(database.Id, documentCollection.Id));
    }

    internal Task<ResourceResponse<DocumentCollection>> RestoreDocumentCollectionAsync(
      string sourceDocumentCollectionLink,
      DocumentCollection targetDocumentCollection,
      DateTimeOffset? restoreTime = null,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<DocumentCollection>>((Func<Task<ResourceResponse<DocumentCollection>>>) (() => this.RestoreDocumentCollectionPrivateAsync(sourceDocumentCollectionLink, targetDocumentCollection, restoreTime, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<DocumentCollection>> RestoreDocumentCollectionPrivateAsync(
      string sourceDocumentCollectionLink,
      DocumentCollection targetDocumentCollection,
      DateTimeOffset? restoreTime,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(sourceDocumentCollectionLink))
        throw new ArgumentNullException(nameof (sourceDocumentCollectionLink));
      if (targetDocumentCollection == null)
        throw new ArgumentNullException(nameof (targetDocumentCollection));
      string databasePath = PathsHelper.GetDatabasePath(sourceDocumentCollectionLink);
      bool isFeed;
      string resourcePath;
      string resourceIdOrFullName;
      bool isNameBased;
      if (!(PathsHelper.TryParsePathSegments(databasePath, out isFeed, out resourcePath, out resourceIdOrFullName, out isNameBased) & isNameBased) || isFeed)
        throw new ArgumentNullException(nameof (sourceDocumentCollectionLink));
      string[] strArray1 = resourceIdOrFullName.Split(new char[1]
      {
        '/'
      }, StringSplitOptions.RemoveEmptyEntries);
      string str1 = strArray1[strArray1.Length - 1];
      if (!(PathsHelper.TryParsePathSegments(sourceDocumentCollectionLink, out isFeed, out resourcePath, out resourceIdOrFullName, out isNameBased) & isNameBased) || isFeed)
        throw new ArgumentNullException(nameof (sourceDocumentCollectionLink));
      string[] strArray2 = resourceIdOrFullName.Split(new char[1]
      {
        '/'
      }, StringSplitOptions.RemoveEmptyEntries);
      string str2 = strArray2[strArray2.Length - 1];
      this.ValidateResource((Microsoft.Azure.Documents.Resource) targetDocumentCollection);
      if (options == null)
        options = new Microsoft.Azure.Documents.Client.RequestOptions();
      if (!options.RemoteStorageType.HasValue)
        options.RemoteStorageType = new RemoteStorageType?(RemoteStorageType.Standard);
      options.SourceDatabaseId = str1;
      options.SourceCollectionId = str2;
      if (restoreTime.HasValue)
        options.RestorePointInTime = new long?(Helpers.ToUnixTime(restoreTime.Value));
      INameValueCollection requestHeaders = this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Create, ResourceType.Collection);
      ResourceResponse<DocumentCollection> resourceResponse1;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Create, databasePath, (Microsoft.Azure.Documents.Resource) targetDocumentCollection, ResourceType.Collection, AuthorizationTokenType.PrimaryMasterKey, requestHeaders))
      {
        ResourceResponse<DocumentCollection> resourceResponse2 = new ResourceResponse<DocumentCollection>(await this.CreateAsync(request, retryPolicyInstance));
        this.sessionContainer.SetSessionToken(resourceResponse2.Resource.ResourceId, resourceResponse2.Resource.AltLink, resourceResponse2.Headers);
        resourceResponse1 = resourceResponse2;
      }
      return resourceResponse1;
    }

    internal Task<DocumentCollectionRestoreStatus> GetDocumentCollectionRestoreStatusAsync(
      string targetDocumentCollectionLink)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<DocumentCollectionRestoreStatus>((Func<Task<DocumentCollectionRestoreStatus>>) (() => this.GetDocumentCollectionRestoreStatusPrivateAsync(targetDocumentCollectionLink, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<DocumentCollectionRestoreStatus> GetDocumentCollectionRestoreStatusPrivateAsync(
      string targetDocumentCollectionLink,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      DocumentClient documentClient1 = this;
      if (string.IsNullOrEmpty(targetDocumentCollectionLink))
        throw new ArgumentNullException(nameof (targetDocumentCollectionLink));
      DocumentClient documentClient2 = documentClient1;
      string documentCollectionLink = targetDocumentCollectionLink;
      Microsoft.Azure.Documents.Client.RequestOptions options = new Microsoft.Azure.Documents.Client.RequestOptions();
      options.PopulateRestoreStatus = true;
      IDocumentClientRetryPolicy retryPolicyInstance1 = retryPolicyInstance;
      string str = (await documentClient2.ReadDocumentCollectionPrivateAsync(documentCollectionLink, options, retryPolicyInstance1)).ResponseHeaders.Get("x-ms-restore-state") ?? RestoreState.RestoreCompleted.ToString();
      return new DocumentCollectionRestoreStatus()
      {
        State = str
      };
    }

    public Task<ResourceResponse<StoredProcedure>> CreateStoredProcedureAsync(
      string collectionLink,
      StoredProcedure storedProcedure,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<StoredProcedure>>((Func<Task<ResourceResponse<StoredProcedure>>>) (() => this.CreateStoredProcedurePrivateAsync(collectionLink, storedProcedure, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<StoredProcedure>> CreateStoredProcedurePrivateAsync(
      string collectionLink,
      StoredProcedure storedProcedure,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(collectionLink))
        throw new ArgumentNullException(nameof (collectionLink));
      if (storedProcedure == null)
        throw new ArgumentNullException(nameof (storedProcedure));
      this.ValidateResource((Microsoft.Azure.Documents.Resource) storedProcedure);
      ResourceResponse<StoredProcedure> procedurePrivateAsync;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Create, collectionLink, (Microsoft.Azure.Documents.Resource) storedProcedure, ResourceType.StoredProcedure, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Create, ResourceType.StoredProcedure)))
        procedurePrivateAsync = new ResourceResponse<StoredProcedure>(await this.CreateAsync(request, retryPolicyInstance));
      return procedurePrivateAsync;
    }

    public Task<ResourceResponse<Trigger>> CreateTriggerAsync(
      string collectionLink,
      Trigger trigger,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Trigger>>((Func<Task<ResourceResponse<Trigger>>>) (() => this.CreateTriggerPrivateAsync(collectionLink, trigger, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Trigger>> CreateTriggerPrivateAsync(
      string collectionLink,
      Trigger trigger,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(collectionLink))
        throw new ArgumentNullException(nameof (collectionLink));
      if (trigger == null)
        throw new ArgumentNullException(nameof (trigger));
      this.ValidateResource((Microsoft.Azure.Documents.Resource) trigger);
      ResourceResponse<Trigger> triggerPrivateAsync;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Create, collectionLink, (Microsoft.Azure.Documents.Resource) trigger, ResourceType.Trigger, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Create, ResourceType.Trigger)))
        triggerPrivateAsync = new ResourceResponse<Trigger>(await this.CreateAsync(request, retryPolicyInstance));
      return triggerPrivateAsync;
    }

    public Task<ResourceResponse<UserDefinedFunction>> CreateUserDefinedFunctionAsync(
      string collectionLink,
      UserDefinedFunction function,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<UserDefinedFunction>>((Func<Task<ResourceResponse<UserDefinedFunction>>>) (() => this.CreateUserDefinedFunctionPrivateAsync(collectionLink, function, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<UserDefinedFunction>> CreateUserDefinedFunctionPrivateAsync(
      string collectionLink,
      UserDefinedFunction function,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(collectionLink))
        throw new ArgumentNullException(nameof (collectionLink));
      if (function == null)
        throw new ArgumentNullException(nameof (function));
      this.ValidateResource((Microsoft.Azure.Documents.Resource) function);
      ResourceResponse<UserDefinedFunction> functionPrivateAsync;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Create, collectionLink, (Microsoft.Azure.Documents.Resource) function, ResourceType.UserDefinedFunction, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Create, ResourceType.UserDefinedFunction)))
        functionPrivateAsync = new ResourceResponse<UserDefinedFunction>(await this.CreateAsync(request, retryPolicyInstance));
      return functionPrivateAsync;
    }

    internal Task<ResourceResponse<UserDefinedType>> CreateUserDefinedTypeAsync(
      string databaseLink,
      UserDefinedType userDefinedType,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<UserDefinedType>>((Func<Task<ResourceResponse<UserDefinedType>>>) (() => this.CreateUserDefinedTypePrivateAsync(databaseLink, userDefinedType, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<UserDefinedType>> CreateUserDefinedTypePrivateAsync(
      string databaseLink,
      UserDefinedType userDefinedType,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(databaseLink))
        throw new ArgumentNullException(nameof (databaseLink));
      if (userDefinedType == null)
        throw new ArgumentNullException(nameof (userDefinedType));
      this.ValidateResource((Microsoft.Azure.Documents.Resource) userDefinedType);
      ResourceResponse<UserDefinedType> typePrivateAsync;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Create, databaseLink, (Microsoft.Azure.Documents.Resource) userDefinedType, ResourceType.UserDefinedType, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Create, ResourceType.UserDefinedType)))
        typePrivateAsync = new ResourceResponse<UserDefinedType>(await this.CreateAsync(request, retryPolicyInstance));
      return typePrivateAsync;
    }

    internal Task<ResourceResponse<Snapshot>> CreateSnapshotAsync(
      Snapshot snapshot,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Snapshot>>((Func<Task<ResourceResponse<Snapshot>>>) (() => this.CreateSnapshotPrivateAsync(snapshot, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Snapshot>> CreateSnapshotPrivateAsync(
      Snapshot snapshot,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (snapshot == null)
        throw new ArgumentNullException(nameof (snapshot));
      this.ValidateResource((Microsoft.Azure.Documents.Resource) snapshot);
      ResourceResponse<Snapshot> snapshotPrivateAsync;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Create, "//snapshots/", (Microsoft.Azure.Documents.Resource) snapshot, ResourceType.Snapshot, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Create, ResourceType.Snapshot)))
        snapshotPrivateAsync = new ResourceResponse<Snapshot>(await this.CreateAsync(request, retryPolicyInstance));
      return snapshotPrivateAsync;
    }

    public Task<ResourceResponse<Microsoft.Azure.Documents.Database>> DeleteDatabaseAsync(
      string databaseLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Microsoft.Azure.Documents.Database>>((Func<Task<ResourceResponse<Microsoft.Azure.Documents.Database>>>) (() => this.DeleteDatabasePrivateAsync(databaseLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Microsoft.Azure.Documents.Database>> DeleteDatabasePrivateAsync(
      string databaseLink,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(databaseLink))
        throw new ArgumentNullException(nameof (databaseLink));
      ResourceResponse<Microsoft.Azure.Documents.Database> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Delete, ResourceType.Database, databaseLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Delete, ResourceType.Database)))
        resourceResponse = new ResourceResponse<Microsoft.Azure.Documents.Database>(await this.DeleteAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<Document>> DeleteDocumentAsync(
      string documentLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Document>>((Func<Task<ResourceResponse<Document>>>) (() => this.DeleteDocumentPrivateAsync(documentLink, options, retryPolicyInstance, cancellationToken)), (IRetryPolicy) retryPolicyInstance, cancellationToken);
    }

    private async Task<ResourceResponse<Document>> DeleteDocumentPrivateAsync(
      string documentLink,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(documentLink))
        throw new ArgumentNullException(nameof (documentLink));
      ResourceResponse<Document> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Delete, ResourceType.Document, documentLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Delete, ResourceType.Document)))
      {
        await this.AddPartitionKeyInformationAsync(request, options);
        request.SerializerSettings = this.GetSerializerSettingsForRequest(options);
        resourceResponse = new ResourceResponse<Document>(await this.DeleteAsync(request, retryPolicyInstance, cancellationToken));
      }
      return resourceResponse;
    }

    public Task<ResourceResponse<DocumentCollection>> DeleteDocumentCollectionAsync(
      string documentCollectionLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<DocumentCollection>>((Func<Task<ResourceResponse<DocumentCollection>>>) (() => this.DeleteDocumentCollectionPrivateAsync(documentCollectionLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<DocumentCollection>> DeleteDocumentCollectionPrivateAsync(
      string documentCollectionLink,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(documentCollectionLink))
        throw new ArgumentNullException(nameof (documentCollectionLink));
      ResourceResponse<DocumentCollection> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Delete, ResourceType.Collection, documentCollectionLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Delete, ResourceType.Collection)))
        resourceResponse = new ResourceResponse<DocumentCollection>(await this.DeleteAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<StoredProcedure>> DeleteStoredProcedureAsync(
      string storedProcedureLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<StoredProcedure>>((Func<Task<ResourceResponse<StoredProcedure>>>) (() => this.DeleteStoredProcedurePrivateAsync(storedProcedureLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<StoredProcedure>> DeleteStoredProcedurePrivateAsync(
      string storedProcedureLink,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(storedProcedureLink))
        throw new ArgumentNullException(nameof (storedProcedureLink));
      ResourceResponse<StoredProcedure> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Delete, ResourceType.StoredProcedure, storedProcedureLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Delete, ResourceType.StoredProcedure)))
        resourceResponse = new ResourceResponse<StoredProcedure>(await this.DeleteAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<Trigger>> DeleteTriggerAsync(
      string triggerLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Trigger>>((Func<Task<ResourceResponse<Trigger>>>) (() => this.DeleteTriggerPrivateAsync(triggerLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Trigger>> DeleteTriggerPrivateAsync(
      string triggerLink,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(triggerLink))
        throw new ArgumentNullException(nameof (triggerLink));
      ResourceResponse<Trigger> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Delete, ResourceType.Trigger, triggerLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Delete, ResourceType.Trigger)))
        resourceResponse = new ResourceResponse<Trigger>(await this.DeleteAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<UserDefinedFunction>> DeleteUserDefinedFunctionAsync(
      string functionLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<UserDefinedFunction>>((Func<Task<ResourceResponse<UserDefinedFunction>>>) (() => this.DeleteUserDefinedFunctionPrivateAsync(functionLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<UserDefinedFunction>> DeleteUserDefinedFunctionPrivateAsync(
      string functionLink,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(functionLink))
        throw new ArgumentNullException(nameof (functionLink));
      ResourceResponse<UserDefinedFunction> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Delete, ResourceType.UserDefinedFunction, functionLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Delete, ResourceType.UserDefinedFunction)))
        resourceResponse = new ResourceResponse<UserDefinedFunction>(await this.DeleteAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<Conflict>> DeleteConflictAsync(
      string conflictLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Conflict>>((Func<Task<ResourceResponse<Conflict>>>) (() => this.DeleteConflictPrivateAsync(conflictLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Conflict>> DeleteConflictPrivateAsync(
      string conflictLink,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(conflictLink))
        throw new ArgumentNullException(nameof (conflictLink));
      ResourceResponse<Conflict> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Delete, ResourceType.Conflict, conflictLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Delete, ResourceType.Conflict)))
      {
        await this.AddPartitionKeyInformationAsync(request, options);
        resourceResponse = new ResourceResponse<Conflict>(await this.DeleteAsync(request, retryPolicyInstance));
      }
      return resourceResponse;
    }

    internal Task<ResourceResponse<Snapshot>> DeleteSnapshotAsync(
      string snapshotLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Snapshot>>((Func<Task<ResourceResponse<Snapshot>>>) (() => this.DeleteSnapshotPrivateAsync(snapshotLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Snapshot>> DeleteSnapshotPrivateAsync(
      string snapshotLink,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(snapshotLink))
        throw new ArgumentNullException(nameof (snapshotLink));
      ResourceResponse<Snapshot> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Delete, ResourceType.Snapshot, snapshotLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Delete, ResourceType.Snapshot)))
        resourceResponse = new ResourceResponse<Snapshot>(await this.DeleteAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<DocumentCollection>> ReplaceDocumentCollectionAsync(
      DocumentCollection documentCollection,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<DocumentCollection>>((Func<Task<ResourceResponse<DocumentCollection>>>) (() => this.ReplaceDocumentCollectionPrivateAsync(documentCollection, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<DocumentCollection>> ReplaceDocumentCollectionPrivateAsync(
      DocumentCollection documentCollection,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      string altLink = null)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (documentCollection == null)
        throw new ArgumentNullException(nameof (documentCollection));
      this.ValidateResource((Microsoft.Azure.Documents.Resource) documentCollection);
      INameValueCollection requestHeaders = this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Replace, ResourceType.Collection);
      ResourceResponse<DocumentCollection> resourceResponse1;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Replace, altLink ?? this.GetLinkForRouting((Microsoft.Azure.Documents.Resource) documentCollection), (Microsoft.Azure.Documents.Resource) documentCollection, ResourceType.Collection, AuthorizationTokenType.PrimaryMasterKey, requestHeaders))
      {
        ResourceResponse<DocumentCollection> resourceResponse2 = new ResourceResponse<DocumentCollection>(await this.UpdateAsync(request, retryPolicyInstance));
        if (resourceResponse2.Resource != null)
          this.sessionContainer.SetSessionToken(resourceResponse2.Resource.ResourceId, resourceResponse2.Resource.AltLink, resourceResponse2.Headers);
        resourceResponse1 = resourceResponse2;
      }
      return resourceResponse1;
    }

    public Task<ResourceResponse<Document>> ReplaceDocumentAsync(
      string documentLink,
      object document,
      Microsoft.Azure.Documents.Client.RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return TaskHelper.InlineIfPossible<ResourceResponse<Document>>((Func<Task<ResourceResponse<Document>>>) (() => this.ReplaceDocumentInlineAsync(documentLink, document, options, cancellationToken)), (IRetryPolicy) null, cancellationToken);
    }

    private async Task<ResourceResponse<Document>> ReplaceDocumentInlineAsync(
      string documentLink,
      object document,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      CancellationToken cancellationToken)
    {
      IDocumentClientRetryPolicy requestRetryPolicy = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      if (options == null || options.PartitionKey == null)
        requestRetryPolicy = (IDocumentClientRetryPolicy) new PartitionKeyMismatchRetryPolicy((CollectionCache) await this.GetCollectionCacheAsync((ITrace) NoOpTrace.Singleton), requestRetryPolicy);
      return await TaskHelper.InlineIfPossible<ResourceResponse<Document>>((Func<Task<ResourceResponse<Document>>>) (() => this.ReplaceDocumentPrivateAsync(documentLink, document, options, requestRetryPolicy, cancellationToken)), (IRetryPolicy) requestRetryPolicy, cancellationToken);
    }

    private Task<ResourceResponse<Document>> ReplaceDocumentPrivateAsync(
      string documentLink,
      object document,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      if (string.IsNullOrEmpty(documentLink))
        throw new ArgumentNullException(nameof (documentLink));
      Document document1 = document != null ? Document.FromObject(document, this.GetSerializerSettingsForRequest(options)) : throw new ArgumentNullException(nameof (document));
      this.ValidateResource((Microsoft.Azure.Documents.Resource) document1);
      return this.ReplaceDocumentPrivateAsync(documentLink, document1, options, retryPolicyInstance, cancellationToken);
    }

    public Task<ResourceResponse<Document>> ReplaceDocumentAsync(
      Document document,
      Microsoft.Azure.Documents.Client.RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Document>>((Func<Task<ResourceResponse<Document>>>) (() => this.ReplaceDocumentPrivateAsync(this.GetLinkForRouting((Microsoft.Azure.Documents.Resource) document), document, options, retryPolicyInstance, cancellationToken)), (IRetryPolicy) retryPolicyInstance, cancellationToken);
    }

    private async Task<ResourceResponse<Document>> ReplaceDocumentPrivateAsync(
      string documentLink,
      Document document,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (document == null)
        throw new ArgumentNullException(nameof (document));
      this.ValidateResource((Microsoft.Azure.Documents.Resource) document);
      ResourceResponse<Document> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Replace, documentLink, (Microsoft.Azure.Documents.Resource) document, ResourceType.Document, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Replace, ResourceType.Document), settings: this.GetSerializerSettingsForRequest(options)))
      {
        await this.AddPartitionKeyInformationAsync(request, document, options);
        resourceResponse = new ResourceResponse<Document>(await this.UpdateAsync(request, retryPolicyInstance, cancellationToken));
      }
      return resourceResponse;
    }

    public Task<ResourceResponse<StoredProcedure>> ReplaceStoredProcedureAsync(
      StoredProcedure storedProcedure,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<StoredProcedure>>((Func<Task<ResourceResponse<StoredProcedure>>>) (() => this.ReplaceStoredProcedurePrivateAsync(storedProcedure, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<StoredProcedure>> ReplaceStoredProcedurePrivateAsync(
      StoredProcedure storedProcedure,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      string altLink = null)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (storedProcedure == null)
        throw new ArgumentNullException(nameof (storedProcedure));
      this.ValidateResource((Microsoft.Azure.Documents.Resource) storedProcedure);
      INameValueCollection requestHeaders = this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Replace, ResourceType.StoredProcedure);
      ResourceResponse<StoredProcedure> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Replace, altLink ?? this.GetLinkForRouting((Microsoft.Azure.Documents.Resource) storedProcedure), (Microsoft.Azure.Documents.Resource) storedProcedure, ResourceType.StoredProcedure, AuthorizationTokenType.PrimaryMasterKey, requestHeaders))
        resourceResponse = new ResourceResponse<StoredProcedure>(await this.UpdateAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<Trigger>> ReplaceTriggerAsync(
      Trigger trigger,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Trigger>>((Func<Task<ResourceResponse<Trigger>>>) (() => this.ReplaceTriggerPrivateAsync(trigger, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Trigger>> ReplaceTriggerPrivateAsync(
      Trigger trigger,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      string altLink = null)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (trigger == null)
        throw new ArgumentNullException(nameof (trigger));
      this.ValidateResource((Microsoft.Azure.Documents.Resource) trigger);
      INameValueCollection requestHeaders = this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Replace, ResourceType.Trigger);
      ResourceResponse<Trigger> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Replace, altLink ?? this.GetLinkForRouting((Microsoft.Azure.Documents.Resource) trigger), (Microsoft.Azure.Documents.Resource) trigger, ResourceType.Trigger, AuthorizationTokenType.PrimaryMasterKey, requestHeaders))
        resourceResponse = new ResourceResponse<Trigger>(await this.UpdateAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<UserDefinedFunction>> ReplaceUserDefinedFunctionAsync(
      UserDefinedFunction function,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<UserDefinedFunction>>((Func<Task<ResourceResponse<UserDefinedFunction>>>) (() => this.ReplaceUserDefinedFunctionPrivateAsync(function, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<UserDefinedFunction>> ReplaceUserDefinedFunctionPrivateAsync(
      UserDefinedFunction function,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      string altLink = null)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (function == null)
        throw new ArgumentNullException(nameof (function));
      this.ValidateResource((Microsoft.Azure.Documents.Resource) function);
      INameValueCollection requestHeaders = this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Replace, ResourceType.UserDefinedFunction);
      ResourceResponse<UserDefinedFunction> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Replace, altLink ?? this.GetLinkForRouting((Microsoft.Azure.Documents.Resource) function), (Microsoft.Azure.Documents.Resource) function, ResourceType.UserDefinedFunction, AuthorizationTokenType.PrimaryMasterKey, requestHeaders))
        resourceResponse = new ResourceResponse<UserDefinedFunction>(await this.UpdateAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<Offer>> ReplaceOfferAsync(Offer offer)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Offer>>((Func<Task<ResourceResponse<Offer>>>) (() => this.ReplaceOfferPrivateAsync(offer, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Offer>> ReplaceOfferPrivateAsync(
      Offer offer,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      if (offer == null)
        throw new ArgumentNullException(nameof (offer));
      ResourceResponse<Offer> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Replace, offer.SelfLink, (Microsoft.Azure.Documents.Resource) offer, ResourceType.Offer, AuthorizationTokenType.PrimaryMasterKey))
        resourceResponse = new ResourceResponse<Offer>(await this.UpdateAsync(request, retryPolicyInstance), OfferTypeResolver.ResponseOfferTypeResolver);
      return resourceResponse;
    }

    internal Task<ResourceResponse<UserDefinedType>> ReplaceUserDefinedTypeAsync(
      UserDefinedType userDefinedType,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<UserDefinedType>>((Func<Task<ResourceResponse<UserDefinedType>>>) (() => this.ReplaceUserDefinedTypePrivateAsync(userDefinedType, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<UserDefinedType>> ReplaceUserDefinedTypePrivateAsync(
      UserDefinedType userDefinedType,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      string altLink = null)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (userDefinedType == null)
        throw new ArgumentNullException(nameof (userDefinedType));
      this.ValidateResource((Microsoft.Azure.Documents.Resource) userDefinedType);
      INameValueCollection requestHeaders = this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Replace, ResourceType.UserDefinedType);
      ResourceResponse<UserDefinedType> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Replace, altLink ?? this.GetLinkForRouting((Microsoft.Azure.Documents.Resource) userDefinedType), (Microsoft.Azure.Documents.Resource) userDefinedType, ResourceType.UserDefinedType, AuthorizationTokenType.PrimaryMasterKey, requestHeaders))
        resourceResponse = new ResourceResponse<UserDefinedType>(await this.UpdateAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<Microsoft.Azure.Documents.Database>> ReadDatabaseAsync(
      string databaseLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Microsoft.Azure.Documents.Database>>((Func<Task<ResourceResponse<Microsoft.Azure.Documents.Database>>>) (() => this.ReadDatabasePrivateAsync(databaseLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Microsoft.Azure.Documents.Database>> ReadDatabasePrivateAsync(
      string databaseLink,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(databaseLink))
        throw new ArgumentNullException(nameof (databaseLink));
      ResourceResponse<Microsoft.Azure.Documents.Database> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.Database, databaseLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Read, ResourceType.Database)))
        resourceResponse = new ResourceResponse<Microsoft.Azure.Documents.Database>(await this.ReadAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<Document>> ReadDocumentAsync(
      string documentLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Document>>((Func<Task<ResourceResponse<Document>>>) (() => this.ReadDocumentPrivateAsync(documentLink, options, retryPolicyInstance, cancellationToken)), (IRetryPolicy) retryPolicyInstance, cancellationToken);
    }

    private async Task<ResourceResponse<Document>> ReadDocumentPrivateAsync(
      string documentLink,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(documentLink))
        throw new ArgumentNullException(nameof (documentLink));
      ResourceResponse<Document> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.Document, documentLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Read, ResourceType.Document)))
      {
        await this.AddPartitionKeyInformationAsync(request, options);
        request.SerializerSettings = this.GetSerializerSettingsForRequest(options);
        resourceResponse = new ResourceResponse<Document>(await this.ReadAsync(request, retryPolicyInstance, cancellationToken));
      }
      return resourceResponse;
    }

    public Task<DocumentResponse<T>> ReadDocumentAsync<T>(
      string documentLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<DocumentResponse<T>>((Func<Task<DocumentResponse<T>>>) (() => this.ReadDocumentPrivateAsync<T>(documentLink, options, retryPolicyInstance, cancellationToken)), (IRetryPolicy) retryPolicyInstance, cancellationToken);
    }

    private async Task<DocumentResponse<T>> ReadDocumentPrivateAsync<T>(
      string documentLink,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(documentLink))
        throw new ArgumentNullException(nameof (documentLink));
      DocumentResponse<T> documentResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.Document, documentLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Read, ResourceType.Document)))
      {
        await this.AddPartitionKeyInformationAsync(request, options);
        request.SerializerSettings = this.GetSerializerSettingsForRequest(options);
        documentResponse = new DocumentResponse<T>(await this.ReadAsync(request, retryPolicyInstance, cancellationToken), this.GetSerializerSettingsForRequest(options));
      }
      return documentResponse;
    }

    public Task<ResourceResponse<DocumentCollection>> ReadDocumentCollectionAsync(
      string documentCollectionLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<DocumentCollection>>((Func<Task<ResourceResponse<DocumentCollection>>>) (() => this.ReadDocumentCollectionPrivateAsync(documentCollectionLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<DocumentCollection>> ReadDocumentCollectionPrivateAsync(
      string documentCollectionLink,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(documentCollectionLink))
        throw new ArgumentNullException(nameof (documentCollectionLink));
      ResourceResponse<DocumentCollection> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.Collection, documentCollectionLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Read, ResourceType.Collection)))
        resourceResponse = new ResourceResponse<DocumentCollection>(await this.ReadAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<StoredProcedure>> ReadStoredProcedureAsync(
      string storedProcedureLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<StoredProcedure>>((Func<Task<ResourceResponse<StoredProcedure>>>) (() => this.ReadStoredProcedureAsync(storedProcedureLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<StoredProcedure>> ReadStoredProcedureAsync(
      string storedProcedureLink,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(storedProcedureLink))
        throw new ArgumentNullException(nameof (storedProcedureLink));
      ResourceResponse<StoredProcedure> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.StoredProcedure, storedProcedureLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Read, ResourceType.StoredProcedure)))
        resourceResponse = new ResourceResponse<StoredProcedure>(await this.ReadAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<Trigger>> ReadTriggerAsync(
      string triggerLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Trigger>>((Func<Task<ResourceResponse<Trigger>>>) (() => this.ReadTriggerPrivateAsync(triggerLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Trigger>> ReadTriggerPrivateAsync(
      string triggerLink,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(triggerLink))
        throw new ArgumentNullException(nameof (triggerLink));
      ResourceResponse<Trigger> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.Trigger, triggerLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Read, ResourceType.Trigger)))
        resourceResponse = new ResourceResponse<Trigger>(await this.ReadAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<UserDefinedFunction>> ReadUserDefinedFunctionAsync(
      string functionLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<UserDefinedFunction>>((Func<Task<ResourceResponse<UserDefinedFunction>>>) (() => this.ReadUserDefinedFunctionPrivateAsync(functionLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<UserDefinedFunction>> ReadUserDefinedFunctionPrivateAsync(
      string functionLink,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(functionLink))
        throw new ArgumentNullException(nameof (functionLink));
      ResourceResponse<UserDefinedFunction> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.UserDefinedFunction, functionLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Read, ResourceType.UserDefinedFunction)))
        resourceResponse = new ResourceResponse<UserDefinedFunction>(await this.ReadAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<Conflict>> ReadConflictAsync(
      string conflictLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Conflict>>((Func<Task<ResourceResponse<Conflict>>>) (() => this.ReadConflictPrivateAsync(conflictLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Conflict>> ReadConflictPrivateAsync(
      string conflictLink,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(conflictLink))
        throw new ArgumentNullException(nameof (conflictLink));
      ResourceResponse<Conflict> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.Conflict, conflictLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Read, ResourceType.Conflict)))
      {
        await this.AddPartitionKeyInformationAsync(request, options);
        resourceResponse = new ResourceResponse<Conflict>(await this.ReadAsync(request, retryPolicyInstance));
      }
      return resourceResponse;
    }

    public Task<ResourceResponse<Offer>> ReadOfferAsync(string offerLink)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Offer>>((Func<Task<ResourceResponse<Offer>>>) (() => this.ReadOfferPrivateAsync(offerLink, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Offer>> ReadOfferPrivateAsync(
      string offerLink,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(offerLink))
        throw new ArgumentNullException(nameof (offerLink));
      ResourceResponse<Offer> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.Offer, offerLink, (Stream) null, AuthorizationTokenType.PrimaryMasterKey))
        resourceResponse = new ResourceResponse<Offer>(await this.ReadAsync(request, retryPolicyInstance), OfferTypeResolver.ResponseOfferTypeResolver);
      return resourceResponse;
    }

    internal Task<ResourceResponse<Schema>> ReadSchemaAsync(
      string documentSchemaLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Schema>>((Func<Task<ResourceResponse<Schema>>>) (() => this.ReadSchemaPrivateAsync(documentSchemaLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Schema>> ReadSchemaPrivateAsync(
      string documentSchemaLink,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(documentSchemaLink))
        throw new ArgumentNullException(nameof (documentSchemaLink));
      ResourceResponse<Schema> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.Schema, documentSchemaLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Read, ResourceType.Schema)))
      {
        await this.AddPartitionKeyInformationAsync(request, options);
        request.SerializerSettings = this.GetSerializerSettingsForRequest(options);
        resourceResponse = new ResourceResponse<Schema>(await this.ReadAsync(request, retryPolicyInstance));
      }
      return resourceResponse;
    }

    internal Task<ResourceResponse<UserDefinedType>> ReadUserDefinedTypeAsync(
      string userDefinedTypeLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<UserDefinedType>>((Func<Task<ResourceResponse<UserDefinedType>>>) (() => this.ReadUserDefinedTypePrivateAsync(userDefinedTypeLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<UserDefinedType>> ReadUserDefinedTypePrivateAsync(
      string userDefinedTypeLink,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(userDefinedTypeLink))
        throw new ArgumentNullException(nameof (userDefinedTypeLink));
      ResourceResponse<UserDefinedType> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.UserDefinedType, userDefinedTypeLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Read, ResourceType.UserDefinedType)))
        resourceResponse = new ResourceResponse<UserDefinedType>(await this.ReadAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    internal Task<ResourceResponse<Snapshot>> ReadSnapshotAsync(
      string snapshotLink,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Snapshot>>((Func<Task<ResourceResponse<Snapshot>>>) (() => this.ReadSnapshotPrivateAsync(snapshotLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Snapshot>> ReadSnapshotPrivateAsync(
      string snapshotLink,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(snapshotLink))
        throw new ArgumentNullException(nameof (snapshotLink));
      ResourceResponse<Snapshot> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.Snapshot, snapshotLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Read, ResourceType.Snapshot)))
        resourceResponse = new ResourceResponse<Snapshot>(await this.ReadAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<DocumentFeedResponse<Microsoft.Azure.Documents.Database>> ReadDatabaseFeedAsync(
      FeedOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<DocumentFeedResponse<Microsoft.Azure.Documents.Database>>((Func<Task<DocumentFeedResponse<Microsoft.Azure.Documents.Database>>>) (() => this.ReadDatabaseFeedPrivateAsync(options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<DocumentFeedResponse<Microsoft.Azure.Documents.Database>> ReadDatabaseFeedPrivateAsync(
      FeedOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      return await client.CreateDatabaseFeedReader(options).ExecuteNextAsync();
    }

    public Task<DocumentFeedResponse<PartitionKeyRange>> ReadPartitionKeyRangeFeedAsync(
      string partitionKeyRangesOrCollectionLink,
      FeedOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<DocumentFeedResponse<PartitionKeyRange>>((Func<Task<DocumentFeedResponse<PartitionKeyRange>>>) (() => this.ReadPartitionKeyRangeFeedPrivateAsync(partitionKeyRangesOrCollectionLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<DocumentFeedResponse<PartitionKeyRange>> ReadPartitionKeyRangeFeedPrivateAsync(
      string partitionKeyRangesLink,
      FeedOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(partitionKeyRangesLink))
        throw new ArgumentNullException(nameof (partitionKeyRangesLink));
      return await client.CreatePartitionKeyRangeFeedReader(partitionKeyRangesLink, options).ExecuteNextAsync();
    }

    public Task<DocumentFeedResponse<DocumentCollection>> ReadDocumentCollectionFeedAsync(
      string collectionsLink,
      FeedOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<DocumentFeedResponse<DocumentCollection>>((Func<Task<DocumentFeedResponse<DocumentCollection>>>) (() => this.ReadDocumentCollectionFeedPrivateAsync(collectionsLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<DocumentFeedResponse<DocumentCollection>> ReadDocumentCollectionFeedPrivateAsync(
      string collectionsLink,
      FeedOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(collectionsLink))
        throw new ArgumentNullException(nameof (collectionsLink));
      return await client.CreateDocumentCollectionFeedReader(collectionsLink, options).ExecuteNextAsync();
    }

    public Task<DocumentFeedResponse<StoredProcedure>> ReadStoredProcedureFeedAsync(
      string storedProceduresLink,
      FeedOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<DocumentFeedResponse<StoredProcedure>>((Func<Task<DocumentFeedResponse<StoredProcedure>>>) (() => this.ReadStoredProcedureFeedPrivateAsync(storedProceduresLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<DocumentFeedResponse<StoredProcedure>> ReadStoredProcedureFeedPrivateAsync(
      string storedProceduresLink,
      FeedOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(storedProceduresLink))
        throw new ArgumentNullException(nameof (storedProceduresLink));
      return await client.CreateStoredProcedureFeedReader(storedProceduresLink, options).ExecuteNextAsync();
    }

    public Task<DocumentFeedResponse<Trigger>> ReadTriggerFeedAsync(
      string triggersLink,
      FeedOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<DocumentFeedResponse<Trigger>>((Func<Task<DocumentFeedResponse<Trigger>>>) (() => this.ReadTriggerFeedPrivateAsync(triggersLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<DocumentFeedResponse<Trigger>> ReadTriggerFeedPrivateAsync(
      string triggersLink,
      FeedOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(triggersLink))
        throw new ArgumentNullException(nameof (triggersLink));
      return await client.CreateTriggerFeedReader(triggersLink, options).ExecuteNextAsync();
    }

    public Task<DocumentFeedResponse<UserDefinedFunction>> ReadUserDefinedFunctionFeedAsync(
      string userDefinedFunctionsLink,
      FeedOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<DocumentFeedResponse<UserDefinedFunction>>((Func<Task<DocumentFeedResponse<UserDefinedFunction>>>) (() => this.ReadUserDefinedFunctionFeedPrivateAsync(userDefinedFunctionsLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<DocumentFeedResponse<UserDefinedFunction>> ReadUserDefinedFunctionFeedPrivateAsync(
      string userDefinedFunctionsLink,
      FeedOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(userDefinedFunctionsLink))
        throw new ArgumentNullException(nameof (userDefinedFunctionsLink));
      return await client.CreateUserDefinedFunctionFeedReader(userDefinedFunctionsLink, options).ExecuteNextAsync();
    }

    public Task<DocumentFeedResponse<object>> ReadDocumentFeedAsync(
      string documentsLink,
      FeedOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return TaskHelper.InlineIfPossible<DocumentFeedResponse<object>>((Func<Task<DocumentFeedResponse<object>>>) (() => this.ReadDocumentFeedInlineAsync(documentsLink, options, cancellationToken)), (IRetryPolicy) null, cancellationToken);
    }

    private async Task<DocumentFeedResponse<object>> ReadDocumentFeedInlineAsync(
      string documentsLink,
      FeedOptions options,
      CancellationToken cancellationToken)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(documentsLink))
        throw new ArgumentNullException(nameof (documentsLink));
      DocumentFeedResponse<Document> source = await client.CreateDocumentFeedReader(documentsLink, options).ExecuteNextAsync(cancellationToken);
      return new DocumentFeedResponse<object>(source.Cast<object>(), source.Count, source.Headers, source.UseETagAsContinuation, source.QueryMetrics, source.RequestStatistics, responseLengthBytes: source.ResponseLengthBytes);
    }

    public Task<DocumentFeedResponse<Conflict>> ReadConflictFeedAsync(
      string conflictsLink,
      FeedOptions options = null)
    {
      return TaskHelper.InlineIfPossible<DocumentFeedResponse<Conflict>>((Func<Task<DocumentFeedResponse<Conflict>>>) (() => this.ReadConflictFeedInlineAsync(conflictsLink, options)), (IRetryPolicy) null);
    }

    private async Task<DocumentFeedResponse<Conflict>> ReadConflictFeedInlineAsync(
      string conflictsLink,
      FeedOptions options)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(conflictsLink))
        throw new ArgumentNullException(nameof (conflictsLink));
      return await client.CreateConflictFeedReader(conflictsLink, options).ExecuteNextAsync();
    }

    public Task<DocumentFeedResponse<Offer>> ReadOffersFeedAsync(FeedOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<DocumentFeedResponse<Offer>>((Func<Task<DocumentFeedResponse<Offer>>>) (() => this.ReadOfferFeedPrivateAsync(options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<DocumentFeedResponse<Offer>> ReadOfferFeedPrivateAsync(
      FeedOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      return await client.CreateOfferFeedReader(options).ExecuteNextAsync();
    }

    internal Task<DocumentFeedResponse<Schema>> ReadSchemaFeedAsync(
      string documentCollectionSchemaLink,
      FeedOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<DocumentFeedResponse<Schema>>((Func<Task<DocumentFeedResponse<Schema>>>) (() => this.ReadSchemaFeedPrivateAsync(documentCollectionSchemaLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<DocumentFeedResponse<Schema>> ReadSchemaFeedPrivateAsync(
      string documentCollectionSchemaLink,
      FeedOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(documentCollectionSchemaLink))
        throw new ArgumentNullException(nameof (documentCollectionSchemaLink));
      return await client.CreateSchemaFeedReader(documentCollectionSchemaLink, options).ExecuteNextAsync();
    }

    internal Task<DocumentFeedResponse<UserDefinedType>> ReadUserDefinedTypeFeedAsync(
      string userDefinedTypesLink,
      FeedOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<DocumentFeedResponse<UserDefinedType>>((Func<Task<DocumentFeedResponse<UserDefinedType>>>) (() => this.ReadUserDefinedTypeFeedPrivateAsync(userDefinedTypesLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<DocumentFeedResponse<UserDefinedType>> ReadUserDefinedTypeFeedPrivateAsync(
      string userDefinedTypesLink,
      FeedOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(userDefinedTypesLink))
        throw new ArgumentNullException(nameof (userDefinedTypesLink));
      return await client.CreateUserDefinedTypeFeedReader(userDefinedTypesLink, options).ExecuteNextAsync();
    }

    internal Task<DocumentFeedResponse<Snapshot>> ReadSnapshotFeedAsync(FeedOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<DocumentFeedResponse<Snapshot>>((Func<Task<DocumentFeedResponse<Snapshot>>>) (() => this.ReadSnapshotFeedPrivateAsync(options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<DocumentFeedResponse<Snapshot>> ReadSnapshotFeedPrivateAsync(
      FeedOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      return await client.CreateSnapshotFeedReader(options).ExecuteNextAsync();
    }

    public Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(
      string storedProcedureLink,
      params object[] procedureParams)
    {
      return this.ExecuteStoredProcedureAsync<TValue>(storedProcedureLink, (Microsoft.Azure.Documents.Client.RequestOptions) null, new CancellationToken(), procedureParams);
    }

    public Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(
      string storedProcedureLink,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      params object[] procedureParams)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<StoredProcedureResponse<TValue>>((Func<Task<StoredProcedureResponse<TValue>>>) (() => this.ExecuteStoredProcedurePrivateAsync<TValue>(storedProcedureLink, options, retryPolicyInstance, new CancellationToken(), procedureParams)), (IRetryPolicy) retryPolicyInstance);
    }

    public Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(
      string storedProcedureLink,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      CancellationToken cancellationToken,
      params object[] procedureParams)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<StoredProcedureResponse<TValue>>((Func<Task<StoredProcedureResponse<TValue>>>) (() => this.ExecuteStoredProcedurePrivateAsync<TValue>(storedProcedureLink, options, retryPolicyInstance, cancellationToken, procedureParams)), (IRetryPolicy) retryPolicyInstance, cancellationToken);
    }

    private async Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedurePrivateAsync<TValue>(
      string storedProcedureLink,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken,
      params object[] procedureParams)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(storedProcedureLink))
        throw new ArgumentNullException(nameof (storedProcedureLink));
      JsonSerializerSettings settingsForRequest = this.GetSerializerSettingsForRequest(options);
      string str = settingsForRequest == null ? JsonConvert.SerializeObject((object) procedureParams) : JsonConvert.SerializeObject((object) procedureParams, settingsForRequest);
      StoredProcedureResponse<TValue> procedureResponse;
      using (MemoryStream storedProcedureInputStream = new MemoryStream())
      {
        using (StreamWriter writer = new StreamWriter((Stream) storedProcedureInputStream))
        {
          await writer.WriteAsync(str);
          await writer.FlushAsync();
          storedProcedureInputStream.Position = 0L;
          using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.ExecuteJavaScript, ResourceType.StoredProcedure, storedProcedureLink, (Stream) storedProcedureInputStream, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.ExecuteJavaScript, ResourceType.StoredProcedure)))
          {
            request.Headers["x-ms-date"] = Rfc1123DateTimeCache.UtcNow();
            if (options?.PartitionKeyRangeId == null)
              await this.AddPartitionKeyInformationAsync(request, options);
            retryPolicyInstance?.OnBeforeSendRequest(request);
            request.SerializerSettings = this.GetSerializerSettingsForRequest(options);
            procedureResponse = new StoredProcedureResponse<TValue>(await this.ExecuteProcedureAsync(request, retryPolicyInstance, cancellationToken), this.GetSerializerSettingsForRequest(options));
          }
        }
      }
      return procedureResponse;
    }

    internal Task<ResourceResponse<Microsoft.Azure.Documents.Database>> UpsertDatabaseAsync(
      Microsoft.Azure.Documents.Database database,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Microsoft.Azure.Documents.Database>>((Func<Task<ResourceResponse<Microsoft.Azure.Documents.Database>>>) (() => this.UpsertDatabasePrivateAsync(database, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Microsoft.Azure.Documents.Database>> UpsertDatabasePrivateAsync(
      Microsoft.Azure.Documents.Database database,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (database == null)
        throw new ArgumentNullException(nameof (database));
      this.ValidateResource((Microsoft.Azure.Documents.Resource) database);
      ResourceResponse<Microsoft.Azure.Documents.Database> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Upsert, "//dbs/", (Microsoft.Azure.Documents.Resource) database, ResourceType.Database, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Upsert, ResourceType.Database)))
        resourceResponse = new ResourceResponse<Microsoft.Azure.Documents.Database>(await this.UpsertAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<Document>> UpsertDocumentAsync(
      string documentsFeedOrDatabaseLink,
      object document,
      Microsoft.Azure.Documents.Client.RequestOptions options = null,
      bool disableAutomaticIdGeneration = false,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return TaskHelper.InlineIfPossible<ResourceResponse<Document>>((Func<Task<ResourceResponse<Document>>>) (() => this.UpsertDocumentInlineAsync(documentsFeedOrDatabaseLink, document, options, disableAutomaticIdGeneration, cancellationToken)), (IRetryPolicy) null, cancellationToken);
    }

    private async Task<ResourceResponse<Document>> UpsertDocumentInlineAsync(
      string documentsFeedOrDatabaseLink,
      object document,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      bool disableAutomaticIdGeneration,
      CancellationToken cancellationToken)
    {
      IDocumentClientRetryPolicy requestRetryPolicy = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      if (options?.PartitionKey == null)
        requestRetryPolicy = (IDocumentClientRetryPolicy) new PartitionKeyMismatchRetryPolicy((CollectionCache) await this.GetCollectionCacheAsync((ITrace) NoOpTrace.Singleton), requestRetryPolicy);
      return await TaskHelper.InlineIfPossible<ResourceResponse<Document>>((Func<Task<ResourceResponse<Document>>>) (() => this.UpsertDocumentPrivateAsync(documentsFeedOrDatabaseLink, document, options, disableAutomaticIdGeneration, requestRetryPolicy, cancellationToken)), (IRetryPolicy) requestRetryPolicy, cancellationToken);
    }

    private async Task<ResourceResponse<Document>> UpsertDocumentPrivateAsync(
      string documentCollectionLink,
      object document,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      bool disableAutomaticIdGeneration,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(documentCollectionLink))
        throw new ArgumentNullException(nameof (documentCollectionLink));
      if (document == null)
        throw new ArgumentNullException(nameof (document));
      INameValueCollection requestHeaders = this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Upsert, ResourceType.Document);
      Document document1 = Document.FromObject(document, this.GetSerializerSettingsForRequest(options));
      this.ValidateResource((Microsoft.Azure.Documents.Resource) document1);
      if (string.IsNullOrEmpty(document1.Id) && !disableAutomaticIdGeneration)
        document1.Id = Guid.NewGuid().ToString();
      ResourceResponse<Document> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Upsert, documentCollectionLink, (Microsoft.Azure.Documents.Resource) document1, ResourceType.Document, AuthorizationTokenType.PrimaryMasterKey, requestHeaders, settings: this.GetSerializerSettingsForRequest(options)))
      {
        await this.AddPartitionKeyInformationAsync(request, document1, options);
        resourceResponse = new ResourceResponse<Document>(await this.UpsertAsync(request, retryPolicyInstance, cancellationToken));
      }
      return resourceResponse;
    }

    internal Task<ResourceResponse<DocumentCollection>> UpsertDocumentCollectionAsync(
      string databaseLink,
      DocumentCollection documentCollection,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      throw new NotImplementedException();
    }

    public Task<ResourceResponse<StoredProcedure>> UpsertStoredProcedureAsync(
      string collectionLink,
      StoredProcedure storedProcedure,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<StoredProcedure>>((Func<Task<ResourceResponse<StoredProcedure>>>) (() => this.UpsertStoredProcedurePrivateAsync(collectionLink, storedProcedure, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<StoredProcedure>> UpsertStoredProcedurePrivateAsync(
      string collectionLink,
      StoredProcedure storedProcedure,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(collectionLink))
        throw new ArgumentNullException(nameof (collectionLink));
      if (storedProcedure == null)
        throw new ArgumentNullException(nameof (storedProcedure));
      this.ValidateResource((Microsoft.Azure.Documents.Resource) storedProcedure);
      ResourceResponse<StoredProcedure> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Upsert, collectionLink, (Microsoft.Azure.Documents.Resource) storedProcedure, ResourceType.StoredProcedure, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Upsert, ResourceType.StoredProcedure)))
        resourceResponse = new ResourceResponse<StoredProcedure>(await this.UpsertAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<Trigger>> UpsertTriggerAsync(
      string collectionLink,
      Trigger trigger,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Trigger>>((Func<Task<ResourceResponse<Trigger>>>) (() => this.UpsertTriggerPrivateAsync(collectionLink, trigger, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Trigger>> UpsertTriggerPrivateAsync(
      string collectionLink,
      Trigger trigger,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(collectionLink))
        throw new ArgumentNullException(nameof (collectionLink));
      if (trigger == null)
        throw new ArgumentNullException(nameof (trigger));
      this.ValidateResource((Microsoft.Azure.Documents.Resource) trigger);
      ResourceResponse<Trigger> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Upsert, collectionLink, (Microsoft.Azure.Documents.Resource) trigger, ResourceType.Trigger, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Upsert, ResourceType.Trigger)))
        resourceResponse = new ResourceResponse<Trigger>(await this.UpsertAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<UserDefinedFunction>> UpsertUserDefinedFunctionAsync(
      string collectionLink,
      UserDefinedFunction function,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<UserDefinedFunction>>((Func<Task<ResourceResponse<UserDefinedFunction>>>) (() => this.UpsertUserDefinedFunctionPrivateAsync(collectionLink, function, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<UserDefinedFunction>> UpsertUserDefinedFunctionPrivateAsync(
      string collectionLink,
      UserDefinedFunction function,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(collectionLink))
        throw new ArgumentNullException(nameof (collectionLink));
      if (function == null)
        throw new ArgumentNullException(nameof (function));
      this.ValidateResource((Microsoft.Azure.Documents.Resource) function);
      ResourceResponse<UserDefinedFunction> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Upsert, collectionLink, (Microsoft.Azure.Documents.Resource) function, ResourceType.UserDefinedFunction, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Upsert, ResourceType.UserDefinedFunction)))
        resourceResponse = new ResourceResponse<UserDefinedFunction>(await this.UpsertAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    internal Task<ResourceResponse<UserDefinedType>> UpsertUserDefinedTypeAsync(
      string databaseLink,
      UserDefinedType userDefinedType,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<UserDefinedType>>((Func<Task<ResourceResponse<UserDefinedType>>>) (() => this.UpsertUserDefinedTypePrivateAsync(databaseLink, userDefinedType, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<UserDefinedType>> UpsertUserDefinedTypePrivateAsync(
      string databaseLink,
      UserDefinedType userDefinedType,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (string.IsNullOrEmpty(databaseLink))
        throw new ArgumentNullException(nameof (databaseLink));
      if (userDefinedType == null)
        throw new ArgumentNullException(nameof (userDefinedType));
      this.ValidateResource((Microsoft.Azure.Documents.Resource) userDefinedType);
      ResourceResponse<UserDefinedType> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Upsert, databaseLink, (Microsoft.Azure.Documents.Resource) userDefinedType, ResourceType.UserDefinedType, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options, Microsoft.Azure.Documents.OperationType.Upsert, ResourceType.UserDefinedType)))
        resourceResponse = new ResourceResponse<UserDefinedType>(await this.UpsertAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    ValueTask<(string token, string payload)> IAuthorizationTokenProvider.GetUserAuthorizationAsync(
      string resourceAddress,
      string resourceType,
      string requestVerb,
      INameValueCollection headers,
      AuthorizationTokenType tokenType)
    {
      return this.cosmosAuthorization.GetUserAuthorizationAsync(resourceAddress, resourceType, requestVerb, headers, tokenType);
    }

    ValueTask<string> ICosmosAuthorizationTokenProvider.GetUserAuthorizationTokenAsync(
      string resourceAddress,
      string resourceType,
      string requestVerb,
      INameValueCollection headers,
      AuthorizationTokenType tokenType,
      ITrace trace)
    {
      return this.cosmosAuthorization.GetUserAuthorizationTokenAsync(resourceAddress, resourceType, requestVerb, headers, tokenType, trace);
    }

    Task IAuthorizationTokenProvider.AddSystemAuthorizationHeaderAsync(
      DocumentServiceRequest request,
      string federationId,
      string verb,
      string resourceId)
    {
      return this.cosmosAuthorization.AddSystemAuthorizationHeaderAsync(request, federationId, verb, resourceId);
    }

    internal Task<DocumentServiceResponse> CreateAsync(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicy,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      return this.ProcessRequestAsync("POST", request, retryPolicy, cancellationToken);
    }

    internal Task<DocumentServiceResponse> UpdateAsync(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicy,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      return this.ProcessRequestAsync("PUT", request, retryPolicy, cancellationToken);
    }

    internal Task<DocumentServiceResponse> ReadAsync(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicy,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      return this.ProcessRequestAsync("GET", request, retryPolicy, cancellationToken);
    }

    internal Task<DocumentServiceResponse> ReadFeedAsync(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicy,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      return this.ProcessRequestAsync("GET", request, retryPolicy, cancellationToken);
    }

    internal Task<DocumentServiceResponse> DeleteAsync(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicy,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      return this.ProcessRequestAsync("DELETE", request, retryPolicy, cancellationToken);
    }

    internal Task<DocumentServiceResponse> ExecuteProcedureAsync(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicy,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      return this.ProcessRequestAsync("POST", request, retryPolicy, cancellationToken);
    }

    internal Task<DocumentServiceResponse> ExecuteQueryAsync(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicy,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      return this.ProcessRequestAsync("POST", request, retryPolicy, cancellationToken);
    }

    internal Task<DocumentServiceResponse> UpsertAsync(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicy,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      request.Headers["x-ms-documentdb-is-upsert"] = bool.TrueString;
      return this.ProcessRequestAsync("POST", request, retryPolicy, cancellationToken);
    }

    public Task<AccountProperties> GetDatabaseAccountAsync() => TaskHelper.InlineIfPossible<AccountProperties>((Func<Task<AccountProperties>>) (() => this.GetDatabaseAccountPrivateAsync(this.ReadEndpoint)), (IRetryPolicy) this.ResetSessionTokenRetryPolicy.GetRequestPolicy());

    Task<AccountProperties> IDocumentClientInternal.GetDatabaseAccountInternalAsync(
      Uri serviceEndpoint,
      CancellationToken cancellationToken)
    {
      return this.GetDatabaseAccountPrivateAsync(serviceEndpoint, cancellationToken);
    }

    private async Task<AccountProperties> GetDatabaseAccountPrivateAsync(
      Uri serviceEndpoint,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      await this.EnsureValidClientAsync((ITrace) NoOpTrace.Singleton);
      if (!(this.GatewayStoreModel is Microsoft.Azure.Cosmos.GatewayStoreModel gatewayStoreModel))
        return (AccountProperties) null;
      Func<ValueTask<HttpRequestMessage>> requestMessage = new Func<ValueTask<HttpRequestMessage>>(CreateRequestMessage);
      CancellationToken cancellationToken1 = new CancellationToken();
      AccountProperties databaseAccountAsync = await gatewayStoreModel.GetDatabaseAccountAsync(requestMessage, (IClientSideRequestStatistics) null, cancellationToken1);
      this.UseMultipleWriteLocations = this.ConnectionPolicy.UseMultipleWriteLocations && databaseAccountAsync.EnableMultipleWriteLocations;
      return databaseAccountAsync;

      async ValueTask<HttpRequestMessage> CreateRequestMessage()
      {
        HttpRequestMessage request = new HttpRequestMessage()
        {
          Method = HttpMethod.Get,
          RequestUri = serviceEndpoint
        };
        INameValueCollection headersCollection = (INameValueCollection) new StoreResponseNameValueCollection();
        await this.cosmosAuthorization.AddAuthorizationHeaderAsync(headersCollection, serviceEndpoint, "GET", AuthorizationTokenType.PrimaryMasterKey);
        foreach (string allKey in headersCollection.AllKeys())
          request.Headers.Add(allKey, headersCollection[allKey]);
        HttpRequestMessage requestMessage = request;
        request = (HttpRequestMessage) null;
        headersCollection = (INameValueCollection) null;
        return requestMessage;
      }
    }

    internal IStoreModel GetStoreProxy(DocumentServiceRequest request)
    {
      if (request.UseGatewayMode)
        return this.GatewayStoreModel;
      ResourceType resourceType = request.ResourceType;
      Microsoft.Azure.Documents.OperationType operationType = request.OperationType;
      if (resourceType != ResourceType.Offer && (!resourceType.IsScript() || operationType == Microsoft.Azure.Documents.OperationType.ExecuteJavaScript))
      {
        switch (resourceType)
        {
          case ResourceType.PartitionKeyRange:
          case ResourceType.Snapshot:
          case ResourceType.ClientEncryptionKey:
            goto label_5;
          case ResourceType.PartitionKey:
            if (operationType != Microsoft.Azure.Documents.OperationType.Delete)
              break;
            goto label_5;
        }
        return operationType == Microsoft.Azure.Documents.OperationType.Create || operationType == Microsoft.Azure.Documents.OperationType.Upsert ? (resourceType == ResourceType.Database || resourceType == ResourceType.User || resourceType == ResourceType.Collection || resourceType == ResourceType.Permission ? this.GatewayStoreModel : this.StoreModel) : (operationType == Microsoft.Azure.Documents.OperationType.Delete ? (resourceType == ResourceType.Database || resourceType == ResourceType.User || resourceType == ResourceType.Collection ? this.GatewayStoreModel : this.StoreModel) : (operationType == Microsoft.Azure.Documents.OperationType.Replace || operationType == Microsoft.Azure.Documents.OperationType.CollectionTruncate ? (resourceType == ResourceType.Collection ? this.GatewayStoreModel : this.StoreModel) : (operationType == Microsoft.Azure.Documents.OperationType.Read && resourceType == ResourceType.Collection ? this.GatewayStoreModel : this.StoreModel)));
      }
label_5:
      return this.GatewayStoreModel;
    }

    private string GetLinkForRouting(Microsoft.Azure.Documents.Resource resource) => resource.SelfLink ?? resource.AltLink;

    internal void EnsureValidOverwrite(
      Microsoft.Azure.Documents.ConsistencyLevel desiredConsistencyLevel,
      Microsoft.Azure.Documents.OperationType? operationType = null,
      ResourceType? resourceType = null)
    {
      Microsoft.Azure.Documents.ConsistencyLevel consistencyLevel = this.accountServiceConfiguration.DefaultConsistencyLevel;
      if (!this.IsValidConsistency(consistencyLevel, desiredConsistencyLevel, operationType, resourceType))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidConsistencyLevel, (object) desiredConsistencyLevel.ToString(), (object) consistencyLevel.ToString()));
    }

    private bool IsValidConsistency(
      Microsoft.Azure.Documents.ConsistencyLevel backendConsistency,
      Microsoft.Azure.Documents.ConsistencyLevel desiredConsistency,
      Microsoft.Azure.Documents.OperationType? operationType,
      ResourceType? resourceType)
    {
      return this.allowOverrideStrongerConsistency || ValidationHelpers.IsValidConsistencyLevelOverwrite(backendConsistency, desiredConsistency, this.IsLocalQuorumConsistency, operationType, resourceType);
    }

    private void InitializeDirectConnectivity(IStoreClientFactory storeClientFactory)
    {
      this.AddressResolver = new GlobalAddressResolver(this.GlobalEndpointManager, this.PartitionKeyRangeLocation, this.ConnectionPolicy.ConnectionProtocol, (ICosmosAuthorizationTokenProvider) this, (CollectionCache) this.collectionCache, this.partitionKeyRangeCache, (IServiceConfigurationReader) this.accountServiceConfiguration, this.ConnectionPolicy, this.httpClient);
      if (storeClientFactory != null)
      {
        this.storeClientFactory = storeClientFactory;
        this.isStoreClientFactoryCreatedInternally = false;
      }
      else
      {
        int connectionProtocol = (int) this.ConnectionPolicy.ConnectionProtocol;
        int totalSeconds = (int) this.ConnectionPolicy.RequestTimeout.TotalSeconds;
        int connectionOpenRequests = this.maxConcurrentConnectionOpenRequests;
        UserAgentContainer userAgentContainer = this.ConnectionPolicy.UserAgentContainer;
        DocumentClientEventSource eventSource = this.eventSource;
        int timeoutInSeconds1 = this.openConnectionTimeoutInSeconds;
        int timeoutInSeconds2 = this.idleConnectionTimeoutInSeconds;
        int granularityInSeconds = this.timerPoolGranularityInSeconds;
        int maxRntbdChannels = this.maxRntbdChannels;
        int rntbdPartitionCount = this.rntbdPartitionCount;
        int requestsPerRntbdChannel = this.maxRequestsPerRntbdChannel;
        int rntbdPortReuseMode = (int) this.rntbdPortReuseMode;
        int poolReuseThreshold = this.rntbdPortPoolReuseThreshold;
        int poolBindAttempts = this.rntbdPortPoolBindAttempts;
        int detectionTimeSeconds1 = this.rntbdReceiveHangDetectionTimeSeconds;
        int detectionTimeSeconds2 = this.rntbdSendHangDetectionTimeSeconds;
        RetryWithConfiguration withConfiguration = this.ConnectionPolicy.RetryOptions?.GetRetryWithConfiguration();
        int num = this.ConnectionPolicy.EnableTcpConnectionEndpointRediscovery ? 1 : 0;
        GlobalAddressResolver addressResolver = this.AddressResolver;
        int openingConnectionCount = this.rntbdMaxConcurrentOpeningConnectionCount;
        TimeSpan localRegionOpenTimeout = new TimeSpan();
        int rntbdMaxConcurrentOpeningConnectionCount = openingConnectionCount;
        StoreClientFactory storeClientFactory1 = new StoreClientFactory((Protocol) connectionProtocol, totalSeconds, connectionOpenRequests, (Microsoft.Azure.Documents.UserAgentContainer) userAgentContainer, (ICommunicationEventSource) eventSource, openTimeoutInSeconds: timeoutInSeconds1, idleTimeoutInSeconds: timeoutInSeconds2, timerPoolGranularityInSeconds: granularityInSeconds, maxRntbdChannels: maxRntbdChannels, rntbdPartitionCount: rntbdPartitionCount, maxRequestsPerRntbdChannel: requestsPerRntbdChannel, rntbdPortReuseMode: (Microsoft.Azure.Documents.PortReuseMode) rntbdPortReuseMode, rntbdPortPoolReuseThreshold: poolReuseThreshold, rntbdPortPoolBindAttempts: poolBindAttempts, receiveHangDetectionTimeSeconds: detectionTimeSeconds1, sendHangDetectionTimeSeconds: detectionTimeSeconds2, retryWithConfiguration: withConfiguration, enableTcpConnectionEndpointRediscovery: num != 0, addressResolver: (IAddressResolver) addressResolver, localRegionOpenTimeout: localRegionOpenTimeout, rntbdMaxConcurrentOpeningConnectionCount: rntbdMaxConcurrentOpeningConnectionCount);
        if (this.transportClientHandlerFactory != null)
          storeClientFactory1.WithTransportInterceptor(this.transportClientHandlerFactory);
        this.storeClientFactory = (IStoreClientFactory) storeClientFactory1;
        this.isStoreClientFactoryCreatedInternally = true;
      }
      this.CreateStoreModel(true);
    }

    private void CreateStoreModel(bool subscribeRntbdStatus)
    {
      StoreClient storeClient = this.storeClientFactory.CreateStoreClient((IAddressResolver) this.AddressResolver, this.sessionContainer, (IServiceConfigurationReader) this.accountServiceConfiguration, (IAuthorizationTokenProvider) this, true, ((int) this.ConnectionPolicy.EnableReadRequestsFallback ?? (this.accountServiceConfiguration.DefaultConsistencyLevel != Microsoft.Azure.Documents.ConsistencyLevel.BoundedStaleness ? 1 : 0)) != 0, !this.enableRntbdChannel, this.UseMultipleWriteLocations && this.accountServiceConfiguration.DefaultConsistencyLevel != 0, true);
      if (subscribeRntbdStatus)
        storeClient.AddDisableRntbdChannelCallback(new Action(this.DisableRntbdChannel));
      storeClient.SerializerSettings = this.serializerSettings;
      this.StoreModel = (IStoreModel) new ServerStoreModel(storeClient, this.sendingRequest, this.receivedResponse);
    }

    private void DisableRntbdChannel()
    {
      this.enableRntbdChannel = false;
      this.CreateStoreModel(false);
    }

    private async Task InitializeGatewayConfigurationReaderAsync()
    {
      this.accountServiceConfiguration = new CosmosAccountServiceConfiguration(new Func<Task<AccountProperties>>(new GatewayAccountReader(this.ServiceEndpoint, this.cosmosAuthorization, this.ConnectionPolicy, this.httpClient, this.cancellationTokenSource.Token).InitializeReaderAsync));
      await this.accountServiceConfiguration.InitializeAsync();
      AccountProperties accountProperties = this.accountServiceConfiguration.AccountProperties;
      this.UseMultipleWriteLocations = this.ConnectionPolicy.UseMultipleWriteLocations && accountProperties.EnableMultipleWriteLocations;
      this.GlobalEndpointManager.InitializeAccountPropertiesAndStartBackgroundRefresh(accountProperties);
    }

    internal void CaptureSessionToken(
      DocumentServiceRequest request,
      DocumentServiceResponse response)
    {
      this.sessionContainer.SetSessionToken(request, response.Headers);
    }

    internal DocumentServiceRequest CreateDocumentServiceRequest(
      Microsoft.Azure.Documents.OperationType operationType,
      string resourceLink,
      ResourceType resourceType,
      INameValueCollection headers)
    {
      return resourceType == ResourceType.Database || resourceType == ResourceType.Offer ? DocumentServiceRequest.Create(operationType, (string) null, resourceType, AuthorizationTokenType.PrimaryMasterKey, headers) : DocumentServiceRequest.Create(operationType, resourceType, resourceLink, AuthorizationTokenType.PrimaryMasterKey, headers);
    }

    internal void ValidateResource(Microsoft.Azure.Documents.Resource resource) => this.ValidateResource(resource.Id);

    internal void ValidateResource(string resourceId)
    {
      if (string.IsNullOrEmpty(resourceId))
        return;
      int index = resourceId.IndexOfAny(new char[4]
      {
        '/',
        '\\',
        '?',
        '#'
      });
      if (index != -1)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidCharacterInResourceName, (object) resourceId[index]));
      if (resourceId[resourceId.Length - 1] == ' ')
        throw new ArgumentException(RMResources.InvalidSpaceEndingInResourceName);
    }

    private async Task AddPartitionKeyInformationAsync(
      DocumentServiceRequest request,
      Document document,
      Microsoft.Azure.Documents.Client.RequestOptions options)
    {
      ContainerProperties containerProperties = await (await this.GetCollectionCacheAsync((ITrace) NoOpTrace.Singleton)).ResolveCollectionAsync(request, CancellationToken.None, (ITrace) NoOpTrace.Singleton);
      PartitionKeyDefinition partitionKey = containerProperties.PartitionKey;
      request.Headers.Set("x-ms-documentdb-partitionkey", (options?.PartitionKey == null || !options.PartitionKey.Equals((object) Microsoft.Azure.Documents.PartitionKey.None) ? (options?.PartitionKey == null ? DocumentAnalyzer.ExtractPartitionKeyValue(document, partitionKey) : options.PartitionKey.InternalKey) : containerProperties.GetNoneValue()).ToJsonString());
    }

    internal async Task AddPartitionKeyInformationAsync(
      DocumentServiceRequest request,
      Microsoft.Azure.Documents.Client.RequestOptions options)
    {
      ContainerProperties containerProperties = await (await this.GetCollectionCacheAsync((ITrace) NoOpTrace.Singleton)).ResolveCollectionAsync(request, CancellationToken.None, (ITrace) NoOpTrace.Singleton);
      PartitionKeyDefinition partitionKey = containerProperties.PartitionKey;
      PartitionKeyInternal partitionKeyInternal;
      if (options?.PartitionKey == null)
      {
        if (partitionKey != null && partitionKey.Paths.Count != 0)
          throw new InvalidOperationException(RMResources.MissingPartitionKeyValue);
        partitionKeyInternal = PartitionKeyInternal.Empty;
      }
      else
        partitionKeyInternal = !options.PartitionKey.Equals((object) Microsoft.Azure.Documents.PartitionKey.None) ? options.PartitionKey.InternalKey : containerProperties.GetNoneValue();
      request.Headers.Set("x-ms-documentdb-partitionkey", partitionKeyInternal.ToJsonString());
    }

    private JsonSerializerSettings GetSerializerSettingsForRequest(Microsoft.Azure.Documents.Client.RequestOptions requestOptions) => requestOptions?.JsonSerializerSettings ?? this.serializerSettings;

    private INameValueCollection GetRequestHeaders(
      Microsoft.Azure.Documents.Client.RequestOptions options,
      Microsoft.Azure.Documents.OperationType operationType,
      ResourceType resourceType)
    {
      RequestNameValueCollection requestHeaders = new RequestNameValueCollection();
      if (this.UseMultipleWriteLocations)
        requestHeaders.Set("x-ms-cosmos-allow-tentative-writes", bool.TrueString);
      if (this.desiredConsistencyLevel.HasValue)
      {
        if (!this.IsValidConsistency(this.accountServiceConfiguration.DefaultConsistencyLevel, this.desiredConsistencyLevel.Value, new Microsoft.Azure.Documents.OperationType?(operationType), new ResourceType?(resourceType)))
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidConsistencyLevel, (object) options.ConsistencyLevel.Value.ToString(), (object) this.accountServiceConfiguration.DefaultConsistencyLevel));
        requestHeaders.ConsistencyLevel = this.desiredConsistencyLevel.Value.ToString();
      }
      if (options == null)
        return (INameValueCollection) requestHeaders;
      if (options.AccessCondition != null)
      {
        if (options.AccessCondition.Type == AccessConditionType.IfMatch)
          requestHeaders.IfMatch = options.AccessCondition.Condition;
        else
          requestHeaders.IfNoneMatch = options.AccessCondition.Condition;
      }
      if (options.ConsistencyLevel.HasValue)
      {
        if (!this.IsValidConsistency(this.accountServiceConfiguration.DefaultConsistencyLevel, options.ConsistencyLevel.Value, new Microsoft.Azure.Documents.OperationType?(operationType), new ResourceType?(resourceType)))
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidConsistencyLevel, (object) options.ConsistencyLevel.Value.ToString(), (object) this.accountServiceConfiguration.DefaultConsistencyLevel));
        requestHeaders.Set("x-ms-consistency-level", options.ConsistencyLevel.ToString());
      }
      if (options.IndexingDirective.HasValue)
        requestHeaders.Set("x-ms-indexing-directive", options.IndexingDirective.ToString());
      if (options.PostTriggerInclude != null && options.PostTriggerInclude.Count > 0)
      {
        string str = string.Join(",", options.PostTriggerInclude.AsEnumerable<string>());
        requestHeaders.Set("x-ms-documentdb-post-trigger-include", str);
      }
      if (options.PreTriggerInclude != null && options.PreTriggerInclude.Count > 0)
      {
        string str = string.Join(",", options.PreTriggerInclude.AsEnumerable<string>());
        requestHeaders.Set("x-ms-documentdb-pre-trigger-include", str);
      }
      if (!string.IsNullOrEmpty(options.SessionToken))
        requestHeaders["x-ms-session-token"] = options.SessionToken;
      int num;
      if (options.ResourceTokenExpirySeconds.HasValue)
      {
        RequestNameValueCollection nameValueCollection = requestHeaders;
        num = options.ResourceTokenExpirySeconds.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        nameValueCollection.Set("x-ms-documentdb-expiry-seconds", str);
      }
      if (options.OfferType != null)
        requestHeaders.Set("x-ms-offer-type", options.OfferType);
      if (options.OfferThroughput.HasValue)
      {
        RequestNameValueCollection nameValueCollection = requestHeaders;
        num = options.OfferThroughput.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        nameValueCollection.Set("x-ms-offer-throughput", str);
      }
      if (options.OfferEnableRUPerMinuteThroughput)
        requestHeaders.Set("x-ms-offer-is-ru-per-minute-throughput-enabled", bool.TrueString);
      if (options.InsertSystemPartitionKey)
        requestHeaders.Set("x-ms-cosmos-insert-systempartitionkey", bool.TrueString);
      if (options.EnableScriptLogging)
        requestHeaders.Set("x-ms-documentdb-script-enable-logging", bool.TrueString);
      if (options.PopulateQuotaInfo)
        requestHeaders.Set("x-ms-documentdb-populatequotainfo", bool.TrueString);
      if (options.PopulateRestoreStatus)
        requestHeaders.Set("x-ms-cosmosdb-populaterestorestatus", bool.TrueString);
      if (options.PopulatePartitionKeyRangeStatistics)
        requestHeaders.Set("x-ms-documentdb-populatepartitionstatistics", bool.TrueString);
      if (options.DisableRUPerMinuteUsage)
        requestHeaders.Set("x-ms-documentdb-disable-ru-per-minute-usage", bool.TrueString);
      if (options.RemoteStorageType.HasValue)
        requestHeaders.Set("x-ms-remote-storage-type", options.RemoteStorageType.ToString());
      if (options.PartitionKeyRangeId != null)
        requestHeaders.Set("x-ms-documentdb-partitionkeyrangeid", options.PartitionKeyRangeId);
      if (options.SourceDatabaseId != null)
        requestHeaders.Set("x-ms-source-database-Id", options.SourceDatabaseId);
      if (options.SourceCollectionId != null)
        requestHeaders.Set("x-ms-source-collection-Id", options.SourceCollectionId);
      if (options.RestorePointInTime.HasValue)
        requestHeaders.Set("x-ms-restore-point-in-time", options.RestorePointInTime.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (options.IsReadOnlyScript)
        requestHeaders.Set("x-ms-is-readonly-script", bool.TrueString);
      if (options.IncludeSnapshotDirectories)
        requestHeaders.Set("x-ms-cosmos-include-snapshot-directories", bool.TrueString);
      if (options.ExcludeSystemProperties.HasValue)
        requestHeaders.Set("x-ms-exclude-system-properties", options.ExcludeSystemProperties.Value.ToString());
      if (options.MergeStaticId != null)
        requestHeaders.Set("x-ms-cosmos-merge-static-id", options.MergeStaticId);
      if (options.PreserveFullContent)
        requestHeaders.Set("x-ms-cosmos-preserve-full-content", bool.TrueString);
      return (INameValueCollection) requestHeaders;
    }

    public Task<ResourceResponse<Document>> CreateDocumentAsync(
      Uri documentCollectionUri,
      object document,
      Microsoft.Azure.Documents.Client.RequestOptions options = null,
      bool disableAutomaticIdGeneration = false,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (documentCollectionUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionUri));
      return this.CreateDocumentAsync(documentCollectionUri.OriginalString, document, options, disableAutomaticIdGeneration, cancellationToken);
    }

    public Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionAsync(
      Uri databaseUri,
      DocumentCollection documentCollection,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (databaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (databaseUri));
      return this.CreateDocumentCollectionAsync(databaseUri.OriginalString, documentCollection, options);
    }

    public Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionIfNotExistsAsync(
      Uri databaseUri,
      DocumentCollection documentCollection,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      return TaskHelper.InlineIfPossible<ResourceResponse<DocumentCollection>>((Func<Task<ResourceResponse<DocumentCollection>>>) (() => this.CreateDocumentCollectionIfNotExistsPrivateAsync(databaseUri, documentCollection, options)), (IRetryPolicy) null);
    }

    private async Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionIfNotExistsPrivateAsync(
      Uri databaseUri,
      DocumentCollection documentCollection,
      Microsoft.Azure.Documents.Client.RequestOptions options)
    {
      if (databaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (databaseUri));
      if (documentCollection == null)
        throw new ArgumentNullException(nameof (documentCollection));
      Uri documentCollectionUri = new Uri(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}/{2}", (object) databaseUri.OriginalString, (object) "colls", (object) Uri.EscapeUriString(documentCollection.Id)), UriKind.Relative);
      try
      {
        return await this.ReadDocumentCollectionAsync(documentCollectionUri, options);
      }
      catch (DocumentClientException ex)
      {
        HttpStatusCode? statusCode = ex.StatusCode;
        HttpStatusCode httpStatusCode = HttpStatusCode.NotFound;
        if (!(statusCode.GetValueOrDefault() == httpStatusCode & statusCode.HasValue))
          throw;
      }
      try
      {
        return await this.CreateDocumentCollectionAsync(databaseUri, documentCollection, options);
      }
      catch (DocumentClientException ex)
      {
        HttpStatusCode? statusCode = ex.StatusCode;
        HttpStatusCode httpStatusCode = HttpStatusCode.Conflict;
        if (!(statusCode.GetValueOrDefault() == httpStatusCode & statusCode.HasValue))
          throw;
      }
      return await this.ReadDocumentCollectionAsync(documentCollectionUri, options);
    }

    public Task<ResourceResponse<StoredProcedure>> CreateStoredProcedureAsync(
      Uri documentCollectionUri,
      StoredProcedure storedProcedure,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (documentCollectionUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionUri));
      return this.CreateStoredProcedureAsync(documentCollectionUri.OriginalString, storedProcedure, options);
    }

    public Task<ResourceResponse<Trigger>> CreateTriggerAsync(
      Uri documentCollectionUri,
      Trigger trigger,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (documentCollectionUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionUri));
      return this.CreateTriggerAsync(documentCollectionUri.OriginalString, trigger, options);
    }

    public Task<ResourceResponse<UserDefinedFunction>> CreateUserDefinedFunctionAsync(
      Uri documentCollectionUri,
      UserDefinedFunction function,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (documentCollectionUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionUri));
      return this.CreateUserDefinedFunctionAsync(documentCollectionUri.OriginalString, function, options);
    }

    internal Task<ResourceResponse<UserDefinedType>> CreateUserDefinedTypeAsync(
      Uri databaseUri,
      UserDefinedType userDefinedType,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (databaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (databaseUri));
      return this.CreateUserDefinedTypeAsync(databaseUri.OriginalString, userDefinedType, options);
    }

    public Task<ResourceResponse<Document>> UpsertDocumentAsync(
      Uri documentCollectionUri,
      object document,
      Microsoft.Azure.Documents.Client.RequestOptions options = null,
      bool disableAutomaticIdGeneration = false,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (documentCollectionUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionUri));
      return this.UpsertDocumentAsync(documentCollectionUri.OriginalString, document, options, disableAutomaticIdGeneration, cancellationToken);
    }

    public Task<ResourceResponse<StoredProcedure>> UpsertStoredProcedureAsync(
      Uri documentCollectionUri,
      StoredProcedure storedProcedure,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (documentCollectionUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionUri));
      return this.UpsertStoredProcedureAsync(documentCollectionUri.OriginalString, storedProcedure, options);
    }

    public Task<ResourceResponse<Trigger>> UpsertTriggerAsync(
      Uri documentCollectionUri,
      Trigger trigger,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (documentCollectionUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionUri));
      return this.UpsertTriggerAsync(documentCollectionUri.OriginalString, trigger, options);
    }

    public Task<ResourceResponse<UserDefinedFunction>> UpsertUserDefinedFunctionAsync(
      Uri documentCollectionUri,
      UserDefinedFunction function,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (documentCollectionUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionUri));
      return this.UpsertUserDefinedFunctionAsync(documentCollectionUri.OriginalString, function, options);
    }

    internal Task<ResourceResponse<UserDefinedType>> UpsertUserDefinedTypeAsync(
      Uri databaseUri,
      UserDefinedType userDefinedType,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (databaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (databaseUri));
      return this.UpsertUserDefinedTypeAsync(databaseUri.OriginalString, userDefinedType, options);
    }

    public Task<ResourceResponse<Microsoft.Azure.Documents.Database>> DeleteDatabaseAsync(
      Uri databaseUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (databaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (databaseUri));
      return this.DeleteDatabaseAsync(databaseUri.OriginalString, options);
    }

    public Task<ResourceResponse<Document>> DeleteDocumentAsync(
      Uri documentUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (documentUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentUri));
      return this.DeleteDocumentAsync(documentUri.OriginalString, options, cancellationToken);
    }

    public Task<ResourceResponse<DocumentCollection>> DeleteDocumentCollectionAsync(
      Uri documentCollectionUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (documentCollectionUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionUri));
      return this.DeleteDocumentCollectionAsync(documentCollectionUri.OriginalString, options);
    }

    public Task<ResourceResponse<StoredProcedure>> DeleteStoredProcedureAsync(
      Uri storedProcedureUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (storedProcedureUri == (Uri) null)
        throw new ArgumentNullException(nameof (storedProcedureUri));
      return this.DeleteStoredProcedureAsync(storedProcedureUri.OriginalString, options);
    }

    public Task<ResourceResponse<Trigger>> DeleteTriggerAsync(
      Uri triggerUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (triggerUri == (Uri) null)
        throw new ArgumentNullException(nameof (triggerUri));
      return this.DeleteTriggerAsync(triggerUri.OriginalString, options);
    }

    public Task<ResourceResponse<UserDefinedFunction>> DeleteUserDefinedFunctionAsync(
      Uri functionUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (functionUri == (Uri) null)
        throw new ArgumentNullException(nameof (functionUri));
      return this.DeleteUserDefinedFunctionAsync(functionUri.OriginalString, options);
    }

    public Task<ResourceResponse<Conflict>> DeleteConflictAsync(
      Uri conflictUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (conflictUri == (Uri) null)
        throw new ArgumentNullException(nameof (conflictUri));
      return this.DeleteConflictAsync(conflictUri.OriginalString, options);
    }

    public Task<ResourceResponse<Document>> ReplaceDocumentAsync(
      Uri documentUri,
      object document,
      Microsoft.Azure.Documents.Client.RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (documentUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentUri));
      return this.ReplaceDocumentAsync(documentUri.OriginalString, document, options, cancellationToken);
    }

    public Task<ResourceResponse<DocumentCollection>> ReplaceDocumentCollectionAsync(
      Uri documentCollectionUri,
      DocumentCollection documentCollection,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (documentCollectionUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionUri));
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<DocumentCollection>>((Func<Task<ResourceResponse<DocumentCollection>>>) (() => this.ReplaceDocumentCollectionPrivateAsync(documentCollection, options, retryPolicyInstance, documentCollectionUri.OriginalString)), (IRetryPolicy) retryPolicyInstance);
    }

    public Task<ResourceResponse<StoredProcedure>> ReplaceStoredProcedureAsync(
      Uri storedProcedureUri,
      StoredProcedure storedProcedure,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (storedProcedureUri == (Uri) null)
        throw new ArgumentNullException(nameof (storedProcedureUri));
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<StoredProcedure>>((Func<Task<ResourceResponse<StoredProcedure>>>) (() => this.ReplaceStoredProcedurePrivateAsync(storedProcedure, options, retryPolicyInstance, storedProcedureUri.OriginalString)), (IRetryPolicy) retryPolicyInstance);
    }

    public Task<ResourceResponse<Trigger>> ReplaceTriggerAsync(
      Uri triggerUri,
      Trigger trigger,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (triggerUri == (Uri) null)
        throw new ArgumentNullException(nameof (triggerUri));
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Trigger>>((Func<Task<ResourceResponse<Trigger>>>) (() => this.ReplaceTriggerPrivateAsync(trigger, options, retryPolicyInstance, triggerUri.OriginalString)), (IRetryPolicy) retryPolicyInstance);
    }

    public Task<ResourceResponse<UserDefinedFunction>> ReplaceUserDefinedFunctionAsync(
      Uri userDefinedFunctionUri,
      UserDefinedFunction function,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (userDefinedFunctionUri == (Uri) null)
        throw new ArgumentNullException(nameof (userDefinedFunctionUri));
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<UserDefinedFunction>>((Func<Task<ResourceResponse<UserDefinedFunction>>>) (() => this.ReplaceUserDefinedFunctionPrivateAsync(function, options, retryPolicyInstance, userDefinedFunctionUri.OriginalString)), (IRetryPolicy) retryPolicyInstance);
    }

    internal Task<ResourceResponse<UserDefinedType>> ReplaceUserDefinedTypeAsync(
      Uri userDefinedTypeUri,
      UserDefinedType userDefinedType,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (userDefinedTypeUri == (Uri) null)
        throw new ArgumentNullException(nameof (userDefinedTypeUri));
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<UserDefinedType>>((Func<Task<ResourceResponse<UserDefinedType>>>) (() => this.ReplaceUserDefinedTypePrivateAsync(userDefinedType, options, retryPolicyInstance, userDefinedTypeUri.OriginalString)), (IRetryPolicy) retryPolicyInstance);
    }

    public Task<ResourceResponse<Microsoft.Azure.Documents.Database>> ReadDatabaseAsync(
      Uri databaseUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (databaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (databaseUri));
      return this.ReadDatabaseAsync(databaseUri.OriginalString, options);
    }

    public Task<ResourceResponse<Document>> ReadDocumentAsync(
      Uri documentUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (documentUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentUri));
      return this.ReadDocumentAsync(documentUri.OriginalString, options, cancellationToken);
    }

    public Task<DocumentResponse<T>> ReadDocumentAsync<T>(
      Uri documentUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (documentUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentUri));
      return this.ReadDocumentAsync<T>(documentUri.OriginalString, options, cancellationToken);
    }

    public Task<ResourceResponse<DocumentCollection>> ReadDocumentCollectionAsync(
      Uri documentCollectionUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (documentCollectionUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionUri));
      return this.ReadDocumentCollectionAsync(documentCollectionUri.OriginalString, options);
    }

    public Task<ResourceResponse<StoredProcedure>> ReadStoredProcedureAsync(
      Uri storedProcedureUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (storedProcedureUri == (Uri) null)
        throw new ArgumentNullException(nameof (storedProcedureUri));
      return this.ReadStoredProcedureAsync(storedProcedureUri.OriginalString, options);
    }

    public Task<ResourceResponse<Trigger>> ReadTriggerAsync(Uri triggerUri, Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (triggerUri == (Uri) null)
        throw new ArgumentNullException(nameof (triggerUri));
      return this.ReadTriggerAsync(triggerUri.OriginalString, options);
    }

    public Task<ResourceResponse<UserDefinedFunction>> ReadUserDefinedFunctionAsync(
      Uri functionUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (functionUri == (Uri) null)
        throw new ArgumentNullException(nameof (functionUri));
      return this.ReadUserDefinedFunctionAsync(functionUri.OriginalString, options);
    }

    public Task<ResourceResponse<Conflict>> ReadConflictAsync(
      Uri conflictUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (conflictUri == (Uri) null)
        throw new ArgumentNullException(nameof (conflictUri));
      return this.ReadConflictAsync(conflictUri.OriginalString, options);
    }

    internal Task<ResourceResponse<Schema>> ReadSchemaAsync(Uri schemaUri, Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (schemaUri == (Uri) null)
        throw new ArgumentNullException(nameof (schemaUri));
      return this.ReadSchemaAsync(schemaUri.OriginalString, options);
    }

    internal Task<ResourceResponse<UserDefinedType>> ReadUserDefinedTypeAsync(
      Uri userDefinedTypeUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (userDefinedTypeUri == (Uri) null)
        throw new ArgumentNullException(nameof (userDefinedTypeUri));
      return this.ReadUserDefinedTypeAsync(userDefinedTypeUri.OriginalString, options);
    }

    internal Task<ResourceResponse<Snapshot>> ReadSnapshotAsync(
      Uri snapshotUri,
      Microsoft.Azure.Documents.Client.RequestOptions options = null)
    {
      if (snapshotUri == (Uri) null)
        throw new ArgumentNullException(nameof (snapshotUri));
      return this.ReadSnapshotAsync(snapshotUri.OriginalString, options);
    }

    public Task<DocumentFeedResponse<DocumentCollection>> ReadDocumentCollectionFeedAsync(
      Uri documentCollectionsUri,
      FeedOptions options = null)
    {
      if (documentCollectionsUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionsUri));
      return this.ReadDocumentCollectionFeedAsync(documentCollectionsUri.OriginalString, options);
    }

    public Task<DocumentFeedResponse<StoredProcedure>> ReadStoredProcedureFeedAsync(
      Uri storedProceduresUri,
      FeedOptions options = null)
    {
      if (storedProceduresUri == (Uri) null)
        throw new ArgumentNullException(nameof (storedProceduresUri));
      return this.ReadStoredProcedureFeedAsync(storedProceduresUri.OriginalString, options);
    }

    public Task<DocumentFeedResponse<Trigger>> ReadTriggerFeedAsync(
      Uri triggersUri,
      FeedOptions options = null)
    {
      if (triggersUri == (Uri) null)
        throw new ArgumentNullException(nameof (triggersUri));
      return this.ReadTriggerFeedAsync(triggersUri.OriginalString, options);
    }

    public Task<DocumentFeedResponse<UserDefinedFunction>> ReadUserDefinedFunctionFeedAsync(
      Uri userDefinedFunctionsUri,
      FeedOptions options = null)
    {
      if (userDefinedFunctionsUri == (Uri) null)
        throw new ArgumentNullException(nameof (userDefinedFunctionsUri));
      return this.ReadUserDefinedFunctionFeedAsync(userDefinedFunctionsUri.OriginalString, options);
    }

    public Task<DocumentFeedResponse<object>> ReadDocumentFeedAsync(
      Uri documentsUri,
      FeedOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (documentsUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentsUri));
      return this.ReadDocumentFeedAsync(documentsUri.OriginalString, options, cancellationToken);
    }

    public Task<DocumentFeedResponse<Conflict>> ReadConflictFeedAsync(
      Uri conflictsUri,
      FeedOptions options = null)
    {
      if (conflictsUri == (Uri) null)
        throw new ArgumentNullException(nameof (conflictsUri));
      return this.ReadConflictFeedAsync(conflictsUri.OriginalString, options);
    }

    public Task<DocumentFeedResponse<PartitionKeyRange>> ReadPartitionKeyRangeFeedAsync(
      Uri partitionKeyRangesOrCollectionUri,
      FeedOptions options = null)
    {
      if (partitionKeyRangesOrCollectionUri == (Uri) null)
        throw new ArgumentNullException(nameof (partitionKeyRangesOrCollectionUri));
      return this.ReadPartitionKeyRangeFeedAsync(partitionKeyRangesOrCollectionUri.OriginalString, options);
    }

    internal Task<DocumentFeedResponse<UserDefinedType>> ReadUserDefinedTypeFeedAsync(
      Uri userDefinedTypesUri,
      FeedOptions options = null)
    {
      if (userDefinedTypesUri == (Uri) null)
        throw new ArgumentNullException(nameof (userDefinedTypesUri));
      return this.ReadUserDefinedTypeFeedAsync(userDefinedTypesUri.OriginalString, options);
    }

    public Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(
      Uri storedProcedureUri,
      params object[] procedureParams)
    {
      if (storedProcedureUri == (Uri) null)
        throw new ArgumentNullException(nameof (storedProcedureUri));
      return this.ExecuteStoredProcedureAsync<TValue>(storedProcedureUri.OriginalString, procedureParams);
    }

    public Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(
      Uri storedProcedureUri,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      params object[] procedureParams)
    {
      if (storedProcedureUri == (Uri) null)
        throw new ArgumentNullException(nameof (storedProcedureUri));
      return this.ExecuteStoredProcedureAsync<TValue>(storedProcedureUri.OriginalString, options, procedureParams);
    }

    public Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(
      Uri storedProcedureUri,
      Microsoft.Azure.Documents.Client.RequestOptions options,
      CancellationToken cancellationToken = default (CancellationToken),
      params object[] procedureParams)
    {
      if (storedProcedureUri == (Uri) null)
        throw new ArgumentNullException(nameof (storedProcedureUri));
      return this.ExecuteStoredProcedureAsync<TValue>(storedProcedureUri.OriginalString, options, cancellationToken, procedureParams);
    }

    internal Task<DocumentFeedResponse<Schema>> ReadSchemaFeedAsync(
      Uri schemasUri,
      FeedOptions options = null)
    {
      if (schemasUri == (Uri) null)
        throw new ArgumentNullException(nameof (schemasUri));
      return this.ReadSchemaFeedAsync(schemasUri.OriginalString, options);
    }

    public IOrderedQueryable<DocumentCollection> CreateDocumentCollectionQuery(
      Uri databaseUri,
      FeedOptions feedOptions = null)
    {
      if (databaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (databaseUri));
      return this.CreateDocumentCollectionQuery(databaseUri.OriginalString, feedOptions);
    }

    public IQueryable<object> CreateDocumentCollectionQuery(
      Uri databaseUri,
      string sqlExpression,
      FeedOptions feedOptions = null)
    {
      if (databaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (databaseUri));
      return this.CreateDocumentCollectionQuery(databaseUri, new SqlQuerySpec(sqlExpression), feedOptions);
    }

    public IQueryable<object> CreateDocumentCollectionQuery(
      Uri databaseUri,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null)
    {
      if (databaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (databaseUri));
      return this.CreateDocumentCollectionQuery(databaseUri.OriginalString, querySpec, feedOptions);
    }

    internal IDocumentQuery<DocumentCollection> CreateDocumentCollectionChangeFeedQuery(
      Uri databaseUri,
      ChangeFeedOptions feedOptions)
    {
      if (databaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (databaseUri));
      return this.CreateDocumentCollectionChangeFeedQuery(databaseUri.OriginalString, feedOptions);
    }

    public IOrderedQueryable<StoredProcedure> CreateStoredProcedureQuery(
      Uri storedProceduresUri,
      FeedOptions feedOptions = null)
    {
      if (storedProceduresUri == (Uri) null)
        throw new ArgumentNullException(nameof (storedProceduresUri));
      return this.CreateStoredProcedureQuery(storedProceduresUri.OriginalString, feedOptions);
    }

    public IQueryable<object> CreateStoredProcedureQuery(
      Uri storedProceduresUri,
      string sqlExpression,
      FeedOptions feedOptions = null)
    {
      if (storedProceduresUri == (Uri) null)
        throw new ArgumentNullException(nameof (storedProceduresUri));
      return this.CreateStoredProcedureQuery(storedProceduresUri, new SqlQuerySpec(sqlExpression), feedOptions);
    }

    public IQueryable<object> CreateStoredProcedureQuery(
      Uri storedProceduresUri,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null)
    {
      if (storedProceduresUri == (Uri) null)
        throw new ArgumentNullException(nameof (storedProceduresUri));
      return this.CreateStoredProcedureQuery(storedProceduresUri.OriginalString, querySpec, feedOptions);
    }

    public IOrderedQueryable<Trigger> CreateTriggerQuery(Uri triggersUri, FeedOptions feedOptions = null)
    {
      if (triggersUri == (Uri) null)
        throw new ArgumentNullException(nameof (triggersUri));
      return this.CreateTriggerQuery(triggersUri.OriginalString, feedOptions);
    }

    public IQueryable<object> CreateTriggerQuery(
      Uri triggersUri,
      string sqlExpression,
      FeedOptions feedOptions = null)
    {
      if (triggersUri == (Uri) null)
        throw new ArgumentNullException(nameof (triggersUri));
      return this.CreateTriggerQuery(triggersUri, new SqlQuerySpec(sqlExpression), feedOptions);
    }

    public IQueryable<object> CreateTriggerQuery(
      Uri triggersUri,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null)
    {
      if (triggersUri == (Uri) null)
        throw new ArgumentNullException(nameof (triggersUri));
      return this.CreateTriggerQuery(triggersUri.OriginalString, querySpec, feedOptions);
    }

    public IOrderedQueryable<UserDefinedFunction> CreateUserDefinedFunctionQuery(
      Uri userDefinedFunctionsUri,
      FeedOptions feedOptions = null)
    {
      if (userDefinedFunctionsUri == (Uri) null)
        throw new ArgumentNullException(nameof (userDefinedFunctionsUri));
      return this.CreateUserDefinedFunctionQuery(userDefinedFunctionsUri.OriginalString, feedOptions);
    }

    public IQueryable<object> CreateUserDefinedFunctionQuery(
      Uri userDefinedFunctionsUri,
      string sqlExpression,
      FeedOptions feedOptions = null)
    {
      if (userDefinedFunctionsUri == (Uri) null)
        throw new ArgumentNullException(nameof (userDefinedFunctionsUri));
      return this.CreateUserDefinedFunctionQuery(userDefinedFunctionsUri, new SqlQuerySpec(sqlExpression), feedOptions);
    }

    public IQueryable<object> CreateUserDefinedFunctionQuery(
      Uri userDefinedFunctionsUri,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null)
    {
      if (userDefinedFunctionsUri == (Uri) null)
        throw new ArgumentNullException(nameof (userDefinedFunctionsUri));
      return this.CreateUserDefinedFunctionQuery(userDefinedFunctionsUri.OriginalString, querySpec, feedOptions);
    }

    public IOrderedQueryable<Conflict> CreateConflictQuery(
      Uri conflictsUri,
      FeedOptions feedOptions = null)
    {
      if (conflictsUri == (Uri) null)
        throw new ArgumentNullException(nameof (conflictsUri));
      return this.CreateConflictQuery(conflictsUri.OriginalString, feedOptions);
    }

    public IQueryable<object> CreateConflictQuery(
      Uri conflictsUri,
      string sqlExpression,
      FeedOptions feedOptions = null)
    {
      if (conflictsUri == (Uri) null)
        throw new ArgumentNullException(nameof (conflictsUri));
      return this.CreateConflictQuery(conflictsUri, new SqlQuerySpec(sqlExpression), feedOptions);
    }

    public IQueryable<object> CreateConflictQuery(
      Uri conflictsUri,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null)
    {
      if (conflictsUri == (Uri) null)
        throw new ArgumentNullException(nameof (conflictsUri));
      return this.CreateConflictQuery(conflictsUri.OriginalString, querySpec, feedOptions);
    }

    public IOrderedQueryable<T> CreateDocumentQuery<T>(
      Uri documentCollectionUri,
      FeedOptions feedOptions = null)
    {
      if (documentCollectionUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionUri));
      return this.CreateDocumentQuery<T>(documentCollectionUri.OriginalString, feedOptions);
    }

    [Obsolete("Support for IPartitionResolver based method overloads is now obsolete. Please use the override that does not take a partitionKey parameter.")]
    public IOrderedQueryable<T> CreateDocumentQuery<T>(
      Uri documentCollectionOrDatabaseUri,
      FeedOptions feedOptions,
      object partitionKey)
    {
      if (documentCollectionOrDatabaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionOrDatabaseUri));
      return this.CreateDocumentQuery<T>(documentCollectionOrDatabaseUri.OriginalString, feedOptions, partitionKey);
    }

    public IQueryable<T> CreateDocumentQuery<T>(
      Uri documentCollectionOrDatabaseUri,
      string sqlExpression,
      FeedOptions feedOptions = null)
    {
      if (documentCollectionOrDatabaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionOrDatabaseUri));
      return this.CreateDocumentQuery<T>(documentCollectionOrDatabaseUri, new SqlQuerySpec(sqlExpression), feedOptions);
    }

    [Obsolete("Support for IPartitionResolver based method overloads is now obsolete. It's recommended that you use partitioned collections for higher storage and throughput. Please use the override that does not take a partitionKey parameter.")]
    public IQueryable<T> CreateDocumentQuery<T>(
      Uri documentCollectionOrDatabaseUri,
      string sqlExpression,
      FeedOptions feedOptions,
      object partitionKey)
    {
      if (documentCollectionOrDatabaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionOrDatabaseUri));
      return this.CreateDocumentQuery<T>(documentCollectionOrDatabaseUri, new SqlQuerySpec(sqlExpression), feedOptions, partitionKey);
    }

    public IQueryable<T> CreateDocumentQuery<T>(
      Uri documentCollectionOrDatabaseUri,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null)
    {
      if (documentCollectionOrDatabaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionOrDatabaseUri));
      return this.CreateDocumentQuery<T>(documentCollectionOrDatabaseUri.OriginalString, querySpec, feedOptions);
    }

    [Obsolete("Support for IPartitionResolver based method overloads is now obsolete. It's recommended that you use partitioned collections for higher storage and throughput. Please use the override that does not take a partitionKey parameter.")]
    public IQueryable<T> CreateDocumentQuery<T>(
      Uri documentCollectionOrDatabaseUri,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions,
      object partitionKey)
    {
      if (documentCollectionOrDatabaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionOrDatabaseUri));
      return this.CreateDocumentQuery<T>(documentCollectionOrDatabaseUri.OriginalString, querySpec, feedOptions, partitionKey);
    }

    public IOrderedQueryable<Document> CreateDocumentQuery(
      Uri documentCollectionOrDatabaseUri,
      FeedOptions feedOptions = null)
    {
      if (documentCollectionOrDatabaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionOrDatabaseUri));
      return this.CreateDocumentQuery(documentCollectionOrDatabaseUri.OriginalString, feedOptions);
    }

    [Obsolete("Support for IPartitionResolver based method overloads is now obsolete. It's recommended that you use partitioned collections for higher storage and throughput. Please use the override that does not take a partitionKey parameter.")]
    public IOrderedQueryable<Document> CreateDocumentQuery(
      Uri documentCollectionOrDatabaseUri,
      FeedOptions feedOptions,
      object partitionKey)
    {
      if (documentCollectionOrDatabaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionOrDatabaseUri));
      return this.CreateDocumentQuery(documentCollectionOrDatabaseUri.OriginalString, feedOptions, partitionKey);
    }

    public IQueryable<object> CreateDocumentQuery(
      Uri documentCollectionOrDatabaseUri,
      string sqlExpression,
      FeedOptions feedOptions = null)
    {
      if (documentCollectionOrDatabaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionOrDatabaseUri));
      return this.CreateDocumentQuery(documentCollectionOrDatabaseUri, new SqlQuerySpec(sqlExpression), feedOptions);
    }

    [Obsolete("Support for IPartitionResolver based method overloads is now obsolete. It's recommended that you use partitioned collections for higher storage and throughput. Please use the override that does not take a partitionKey parameter.")]
    public IQueryable<object> CreateDocumentQuery(
      Uri documentCollectionOrDatabaseUri,
      string sqlExpression,
      FeedOptions feedOptions,
      object partitionKey)
    {
      if (documentCollectionOrDatabaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionOrDatabaseUri));
      return this.CreateDocumentQuery(documentCollectionOrDatabaseUri, new SqlQuerySpec(sqlExpression), feedOptions, partitionKey);
    }

    public IQueryable<object> CreateDocumentQuery(
      Uri documentCollectionOrDatabaseUri,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null)
    {
      if (documentCollectionOrDatabaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionOrDatabaseUri));
      return this.CreateDocumentQuery(documentCollectionOrDatabaseUri.OriginalString, querySpec, feedOptions);
    }

    [Obsolete("Support for IPartitionResolver based method overloads is now obsolete. It's recommended that you use partitioned collections for higher storage and throughput. Please use the override that does not take a partitionKey parameter.")]
    public IQueryable<object> CreateDocumentQuery(
      Uri documentCollectionOrDatabaseUri,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions,
      object partitionKey)
    {
      if (documentCollectionOrDatabaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionOrDatabaseUri));
      return this.CreateDocumentQuery(documentCollectionOrDatabaseUri.OriginalString, querySpec, feedOptions, partitionKey);
    }

    public IDocumentQuery<Document> CreateDocumentChangeFeedQuery(
      Uri collectionLink,
      ChangeFeedOptions feedOptions)
    {
      if (collectionLink == (Uri) null)
        throw new ArgumentNullException(nameof (collectionLink));
      return this.CreateDocumentChangeFeedQuery(collectionLink.OriginalString, feedOptions);
    }

    internal IOrderedQueryable<UserDefinedType> CreateUserDefinedTypeQuery(
      Uri userDefinedTypesUri,
      FeedOptions feedOptions = null)
    {
      if (userDefinedTypesUri == (Uri) null)
        throw new ArgumentNullException(nameof (userDefinedTypesUri));
      return this.CreateUserDefinedTypeQuery(userDefinedTypesUri.OriginalString, feedOptions);
    }

    internal IQueryable<object> CreateUserDefinedTypeQuery(
      Uri userDefinedTypesUri,
      string sqlExpression,
      FeedOptions feedOptions = null)
    {
      if (userDefinedTypesUri == (Uri) null)
        throw new ArgumentNullException(nameof (userDefinedTypesUri));
      return this.CreateUserDefinedTypeQuery(userDefinedTypesUri, new SqlQuerySpec(sqlExpression), feedOptions);
    }

    internal IQueryable<object> CreateUserDefinedTypeQuery(
      Uri userDefinedTypesUri,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null)
    {
      if (userDefinedTypesUri == (Uri) null)
        throw new ArgumentNullException(nameof (userDefinedTypesUri));
      return this.CreateUserDefinedTypeQuery(userDefinedTypesUri.OriginalString, querySpec, feedOptions);
    }

    internal IDocumentQuery<UserDefinedType> CreateUserDefinedTypeChangeFeedQuery(
      Uri databaseUri,
      ChangeFeedOptions feedOptions)
    {
      if (databaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (databaseUri));
      return this.CreateUserDefinedTypeChangeFeedQuery(databaseUri.OriginalString, feedOptions);
    }

    public IOrderedQueryable<Microsoft.Azure.Documents.Database> CreateDatabaseQuery(
      FeedOptions feedOptions = null)
    {
      return (IOrderedQueryable<Microsoft.Azure.Documents.Database>) new DocumentQuery<Microsoft.Azure.Documents.Database>(this, ResourceType.Database, typeof (Database), "//dbs/", feedOptions);
    }

    public IQueryable<object> CreateDatabaseQuery(string sqlExpression, FeedOptions feedOptions = null) => this.CreateDatabaseQuery(new SqlQuerySpec(sqlExpression), feedOptions);

    public IQueryable<object> CreateDatabaseQuery(SqlQuerySpec querySpec, FeedOptions feedOptions = null) => new DocumentQuery<Database>(this, ResourceType.Database, typeof (Database), "//dbs/", feedOptions).AsSQL<Database>(querySpec);

    internal IDocumentQuery<Microsoft.Azure.Documents.Database> CreateDatabaseChangeFeedQuery(
      ChangeFeedOptions feedOptions)
    {
      DocumentClient.ValidateChangeFeedOptionsForNotPartitionedResource(feedOptions);
      return (IDocumentQuery<Microsoft.Azure.Documents.Database>) new ChangeFeedQuery<Microsoft.Azure.Documents.Database>(this, ResourceType.Database, (string) null, feedOptions);
    }

    public IOrderedQueryable<DocumentCollection> CreateDocumentCollectionQuery(
      string databaseLink,
      FeedOptions feedOptions = null)
    {
      return (IOrderedQueryable<DocumentCollection>) new DocumentQuery<DocumentCollection>(this, ResourceType.Collection, typeof (DocumentCollection), databaseLink, feedOptions);
    }

    public IQueryable<object> CreateDocumentCollectionQuery(
      string databaseLink,
      string sqlExpression,
      FeedOptions feedOptions = null)
    {
      return this.CreateDocumentCollectionQuery(databaseLink, new SqlQuerySpec(sqlExpression), feedOptions);
    }

    public IQueryable<object> CreateDocumentCollectionQuery(
      string databaseLink,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null)
    {
      return new DocumentQuery<DocumentCollection>(this, ResourceType.Collection, typeof (DocumentCollection), databaseLink, feedOptions).AsSQL<DocumentCollection>(querySpec);
    }

    internal IDocumentQuery<DocumentCollection> CreateDocumentCollectionChangeFeedQuery(
      string databaseLink,
      ChangeFeedOptions feedOptions)
    {
      if (string.IsNullOrEmpty(databaseLink))
        throw new ArgumentException(nameof (databaseLink));
      DocumentClient.ValidateChangeFeedOptionsForNotPartitionedResource(feedOptions);
      return (IDocumentQuery<DocumentCollection>) new ChangeFeedQuery<DocumentCollection>(this, ResourceType.Collection, databaseLink, feedOptions);
    }

    public IOrderedQueryable<StoredProcedure> CreateStoredProcedureQuery(
      string collectionLink,
      FeedOptions feedOptions = null)
    {
      return (IOrderedQueryable<StoredProcedure>) new DocumentQuery<StoredProcedure>(this, ResourceType.StoredProcedure, typeof (StoredProcedure), collectionLink, feedOptions);
    }

    public IQueryable<object> CreateStoredProcedureQuery(
      string collectionLink,
      string sqlExpression,
      FeedOptions feedOptions = null)
    {
      return this.CreateStoredProcedureQuery(collectionLink, new SqlQuerySpec(sqlExpression), feedOptions);
    }

    public IQueryable<object> CreateStoredProcedureQuery(
      string collectionLink,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null)
    {
      return new DocumentQuery<StoredProcedure>(this, ResourceType.StoredProcedure, typeof (StoredProcedure), collectionLink, feedOptions).AsSQL<StoredProcedure>(querySpec);
    }

    public IOrderedQueryable<Trigger> CreateTriggerQuery(
      string collectionLink,
      FeedOptions feedOptions = null)
    {
      return (IOrderedQueryable<Trigger>) new DocumentQuery<Trigger>(this, ResourceType.Trigger, typeof (Trigger), collectionLink, feedOptions);
    }

    public IQueryable<object> CreateTriggerQuery(
      string collectionLink,
      string sqlExpression,
      FeedOptions feedOptions = null)
    {
      return this.CreateTriggerQuery(collectionLink, new SqlQuerySpec(sqlExpression), feedOptions);
    }

    public IQueryable<object> CreateTriggerQuery(
      string collectionLink,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null)
    {
      return new DocumentQuery<Trigger>(this, ResourceType.Trigger, typeof (Trigger), collectionLink, feedOptions).AsSQL<Trigger>(querySpec);
    }

    public IOrderedQueryable<UserDefinedFunction> CreateUserDefinedFunctionQuery(
      string collectionLink,
      FeedOptions feedOptions = null)
    {
      return (IOrderedQueryable<UserDefinedFunction>) new DocumentQuery<UserDefinedFunction>(this, ResourceType.UserDefinedFunction, typeof (UserDefinedFunction), collectionLink, feedOptions);
    }

    public IQueryable<object> CreateUserDefinedFunctionQuery(
      string collectionLink,
      string sqlExpression,
      FeedOptions feedOptions = null)
    {
      return this.CreateUserDefinedFunctionQuery(collectionLink, new SqlQuerySpec(sqlExpression), feedOptions);
    }

    public IQueryable<object> CreateUserDefinedFunctionQuery(
      string collectionLink,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null)
    {
      return new DocumentQuery<UserDefinedFunction>(this, ResourceType.UserDefinedFunction, typeof (UserDefinedFunction), collectionLink, feedOptions).AsSQL<UserDefinedFunction>(querySpec);
    }

    public IOrderedQueryable<Conflict> CreateConflictQuery(
      string collectionLink,
      FeedOptions feedOptions = null)
    {
      return (IOrderedQueryable<Conflict>) new DocumentQuery<Conflict>(this, ResourceType.Conflict, typeof (Conflict), collectionLink, feedOptions);
    }

    public IQueryable<object> CreateConflictQuery(
      string collectionLink,
      string sqlExpression,
      FeedOptions feedOptions = null)
    {
      return this.CreateConflictQuery(collectionLink, new SqlQuerySpec(sqlExpression), feedOptions);
    }

    public IQueryable<object> CreateConflictQuery(
      string collectionLink,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null)
    {
      return new DocumentQuery<Conflict>(this, ResourceType.Conflict, typeof (Conflict), collectionLink, feedOptions).AsSQL<Conflict>(querySpec);
    }

    public IOrderedQueryable<T> CreateDocumentQuery<T>(
      string collectionLink,
      FeedOptions feedOptions = null)
    {
      return (IOrderedQueryable<T>) new DocumentQuery<T>(this, ResourceType.Document, typeof (Document), collectionLink, feedOptions);
    }

    [Obsolete("Support for IPartitionResolver based method overloads is now obsolete. It's recommended that you use partitioned collections for higher storage and throughput. Please use the override that does not take a partitionKey parameter.")]
    public IOrderedQueryable<T> CreateDocumentQuery<T>(
      string documentsFeedOrDatabaseLink,
      FeedOptions feedOptions,
      object partitionKey)
    {
      return (IOrderedQueryable<T>) new DocumentQuery<T>(this, ResourceType.Document, typeof (Document), documentsFeedOrDatabaseLink, feedOptions, partitionKey);
    }

    public IQueryable<T> CreateDocumentQuery<T>(
      string collectionLink,
      string sqlExpression,
      FeedOptions feedOptions = null)
    {
      return this.CreateDocumentQuery<T>(collectionLink, new SqlQuerySpec(sqlExpression), feedOptions);
    }

    [Obsolete("Support for IPartitionResolver based method overloads is now obsolete. It's recommended that you use partitioned collections for higher storage and throughput. Please use the override that does not take a partitionKey parameter.")]
    public IQueryable<T> CreateDocumentQuery<T>(
      string collectionLink,
      string sqlExpression,
      FeedOptions feedOptions,
      object partitionKey)
    {
      return this.CreateDocumentQuery<T>(collectionLink, new SqlQuerySpec(sqlExpression), feedOptions, partitionKey);
    }

    public IQueryable<T> CreateDocumentQuery<T>(
      string collectionLink,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null)
    {
      return new DocumentQuery<T>(this, ResourceType.Document, typeof (Document), collectionLink, feedOptions).AsSQL<T, T>(querySpec);
    }

    [Obsolete("Support for IPartitionResolver based method overloads is now obsolete. It's recommended that you use partitioned collections for higher storage and throughput. Please use the override that does not take a partitionKey parameter.")]
    public IQueryable<T> CreateDocumentQuery<T>(
      string collectionLink,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions,
      object partitionKey)
    {
      return new DocumentQuery<T>(this, ResourceType.Document, typeof (Document), collectionLink, feedOptions, partitionKey).AsSQL<T, T>(querySpec);
    }

    public IOrderedQueryable<Document> CreateDocumentQuery(
      string collectionLink,
      FeedOptions feedOptions = null)
    {
      return (IOrderedQueryable<Document>) new DocumentQuery<Document>(this, ResourceType.Document, typeof (Document), collectionLink, feedOptions);
    }

    [Obsolete("Support for IPartitionResolver based method overloads is now obsolete. It's recommended that you use partitioned collections for higher storage and throughput. Please use the override that does not take a partitionKey parameter.")]
    public IOrderedQueryable<Document> CreateDocumentQuery(
      string collectionLink,
      FeedOptions feedOptions,
      object partitionKey)
    {
      return (IOrderedQueryable<Document>) new DocumentQuery<Document>(this, ResourceType.Document, typeof (Document), collectionLink, feedOptions, partitionKey);
    }

    public IQueryable<object> CreateDocumentQuery(
      string collectionLink,
      string sqlExpression,
      FeedOptions feedOptions = null)
    {
      return this.CreateDocumentQuery(collectionLink, new SqlQuerySpec(sqlExpression), feedOptions);
    }

    [Obsolete("Support for IPartitionResolver based method overloads is now obsolete. It's recommended that you use partitioned collections for higher storage and throughput. Please use the override that does not take a partitionKey parameter.")]
    public IQueryable<object> CreateDocumentQuery(
      string collectionLink,
      string sqlExpression,
      FeedOptions feedOptions,
      object partitionKey)
    {
      return this.CreateDocumentQuery(collectionLink, new SqlQuerySpec(sqlExpression), feedOptions, partitionKey);
    }

    public IQueryable<object> CreateDocumentQuery(
      string collectionLink,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null)
    {
      return new DocumentQuery<Document>(this, ResourceType.Document, typeof (Document), collectionLink, feedOptions).AsSQL<Document>(querySpec);
    }

    [Obsolete("Support for IPartitionResolver based method overloads is now obsolete. It's recommended that you use partitioned collections for higher storage and throughput. Please use the override that does not take a partitionKey parameter.")]
    public IQueryable<object> CreateDocumentQuery(
      string collectionLink,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions,
      object partitionKey)
    {
      return new DocumentQuery<Document>(this, ResourceType.Document, typeof (Document), collectionLink, feedOptions, partitionKey).AsSQL<Document>(querySpec);
    }

    public IDocumentQuery<Document> CreateDocumentChangeFeedQuery(
      string collectionLink,
      ChangeFeedOptions feedOptions)
    {
      return collectionLink != null ? (IDocumentQuery<Document>) new ChangeFeedQuery<Document>(this, ResourceType.Document, collectionLink, feedOptions) : throw new ArgumentNullException(nameof (collectionLink));
    }

    public IOrderedQueryable<Offer> CreateOfferQuery(FeedOptions feedOptions = null) => (IOrderedQueryable<Offer>) new DocumentQuery<Offer>(this, ResourceType.Offer, typeof (Offer), "//offers/", feedOptions);

    public IQueryable<object> CreateOfferQuery(string sqlExpression, FeedOptions feedOptions = null) => new DocumentQuery<Offer>(this, ResourceType.Offer, typeof (Offer), "//offers/", feedOptions).AsSQL<Offer>(new SqlQuerySpec(sqlExpression));

    public IQueryable<object> CreateOfferQuery(SqlQuerySpec querySpec, FeedOptions feedOptions = null) => new DocumentQuery<Offer>(this, ResourceType.Offer, typeof (Offer), "//offers/", feedOptions).AsSQL<Offer>(querySpec);

    internal IOrderedQueryable<UserDefinedType> CreateUserDefinedTypeQuery(
      string userDefinedTypesLink,
      FeedOptions feedOptions = null)
    {
      return (IOrderedQueryable<UserDefinedType>) new DocumentQuery<UserDefinedType>(this, ResourceType.UserDefinedType, typeof (UserDefinedType), userDefinedTypesLink, feedOptions);
    }

    internal IQueryable<object> CreateUserDefinedTypeQuery(
      string userDefinedTypesLink,
      string sqlExpression,
      FeedOptions feedOptions = null)
    {
      return this.CreateUserDefinedTypeQuery(userDefinedTypesLink, new SqlQuerySpec(sqlExpression), feedOptions);
    }

    internal IQueryable<object> CreateUserDefinedTypeQuery(
      string userDefinedTypesLink,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null)
    {
      return new DocumentQuery<UserDefinedType>(this, ResourceType.UserDefinedType, typeof (UserDefinedType), userDefinedTypesLink, feedOptions).AsSQL<UserDefinedType>(querySpec);
    }

    internal IDocumentQuery<UserDefinedType> CreateUserDefinedTypeChangeFeedQuery(
      string databaseLink,
      ChangeFeedOptions feedOptions)
    {
      if (string.IsNullOrEmpty(databaseLink))
        throw new ArgumentException(nameof (databaseLink));
      DocumentClient.ValidateChangeFeedOptionsForNotPartitionedResource(feedOptions);
      return (IDocumentQuery<UserDefinedType>) new ChangeFeedQuery<UserDefinedType>(this, ResourceType.UserDefinedType, databaseLink, feedOptions);
    }

    private static void ValidateChangeFeedOptionsForNotPartitionedResource(
      ChangeFeedOptions feedOptions)
    {
      if (feedOptions != null && (feedOptions.PartitionKey != null || !string.IsNullOrEmpty(feedOptions.PartitionKeyRangeId)))
        throw new ArgumentException(RMResources.CannotSpecifyPKRangeForNonPartitionedResource);
    }

    private class ResetSessionTokenRetryPolicyFactory : IRetryPolicyFactory
    {
      private readonly IRetryPolicyFactory retryPolicy;
      private readonly ISessionContainer sessionContainer;
      private readonly ClientCollectionCache collectionCache;

      public ResetSessionTokenRetryPolicyFactory(
        ISessionContainer sessionContainer,
        ClientCollectionCache collectionCache,
        IRetryPolicyFactory retryPolicy)
      {
        this.retryPolicy = retryPolicy;
        this.sessionContainer = sessionContainer;
        this.collectionCache = collectionCache;
      }

      public IDocumentClientRetryPolicy GetRequestPolicy() => (IDocumentClientRetryPolicy) new RenameCollectionAwareClientRetryPolicy(this.sessionContainer, this.collectionCache, this.retryPolicy.GetRequestPolicy());
    }

    private class HttpRequestMessageHandler : DelegatingHandler
    {
      private readonly EventHandler<SendingRequestEventArgs> sendingRequest;
      private readonly EventHandler<ReceivedResponseEventArgs> receivedResponse;

      public HttpRequestMessageHandler(
        EventHandler<SendingRequestEventArgs> sendingRequest,
        EventHandler<ReceivedResponseEventArgs> receivedResponse,
        HttpMessageHandler innerHandler)
      {
        this.sendingRequest = sendingRequest;
        this.receivedResponse = receivedResponse;
        this.InnerHandler = innerHandler ?? (HttpMessageHandler) new HttpClientHandler();
      }

      protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
      {
        DocumentClient.HttpRequestMessageHandler sender = this;
        EventHandler<SendingRequestEventArgs> sendingRequest = sender.sendingRequest;
        if (sendingRequest != null)
          sendingRequest((object) sender, new SendingRequestEventArgs(request));
        // ISSUE: reference to a compiler-generated method
        HttpResponseMessage response = await sender.\u003C\u003En__0(request, cancellationToken);
        EventHandler<ReceivedResponseEventArgs> receivedResponse = sender.receivedResponse;
        if (receivedResponse != null)
          receivedResponse((object) sender, new ReceivedResponseEventArgs(request, response));
        return response;
      }
    }
  }
}
