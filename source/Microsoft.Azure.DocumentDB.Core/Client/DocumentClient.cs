// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Client.DocumentClient
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Collections;
using Microsoft.Azure.Documents.Common;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents.Query;
using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Client
{
  public sealed class DocumentClient : 
    IDisposable,
    IAuthorizationTokenProvider,
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
    private const string EnableAuthFailureTracesConfig = "enableAuthFailureTraces";
    private const string MacSignatureString = "to sign";
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
    private const bool EnableAuthFailureTraces = false;
    private const bool EnableTcpConnectionEndpointRediscovery = false;
    private ConnectionPolicy connectionPolicy;
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
    private bool enableAuthFailureTraces;
    private bool enableTcpConnectionEndpointRediscovery;
    private readonly IDictionary<string, List<PartitionKeyAndResourceTokenPair>> resourceTokens;
    private IComputeHash authKeyHashFunction;
    private ConsistencyLevel? desiredConsistencyLevel;
    private GatewayServiceConfigurationReader gatewayConfigurationReader;
    private ClientCollectionCache collectionCache;
    private PartitionKeyRangeCache partitionKeyRangeCache;
    internal HttpMessageHandler httpMessageHandler;
    private bool isSuccessfullyInitialized;
    private bool isDisposed;
    private object initializationSyncLock;
    private IStoreClientFactory storeClientFactory;
    private HttpClient mediaClient;
    private bool isStoreClientFactoryCreatedInternally;
    private IStoreModel storeModel;
    private IStoreModel gatewayStoreModel;
    private static int idCounter;
    private int traceId;
    private ISessionContainer sessionContainer;
    private readonly bool hasAuthKeyResourceToken;
    private readonly string authKeyResourceToken = string.Empty;
    private DocumentClientEventSource eventSource;
    private GlobalEndpointManager globalEndpointManager;
    private bool useMultipleWriteLocations;
    internal Task initializeTask;
    private JsonSerializerSettings serializerSettings;
    private Action<IQueryable> onExecuteScalarQueryCallback;

    private event EventHandler<SendingRequestEventArgs> sendingRequest;

    private event EventHandler<ReceivedResponseEventArgs> receivedResponse;

    public DocumentClient(
      Uri serviceEndpoint,
      SecureString authKey,
      ConnectionPolicy connectionPolicy = null,
      ConsistencyLevel? desiredConsistencyLevel = null)
    {
      if (authKey == null)
        throw new ArgumentNullException(nameof (authKey));
      if (authKey != null)
        this.authKeyHashFunction = (IComputeHash) new SecureStringHMACSHA256Helper(authKey);
      this.Initialize(serviceEndpoint, connectionPolicy, desiredConsistencyLevel);
    }

    [Obsolete("Please use the constructor that takes JsonSerializerSettings as the third parameter.")]
    public DocumentClient(
      Uri serviceEndpoint,
      SecureString authKey,
      ConnectionPolicy connectionPolicy,
      ConsistencyLevel? desiredConsistencyLevel,
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
      ConsistencyLevel? desiredConsistencyLevel = null)
      : this(serviceEndpoint, authKey, connectionPolicy, desiredConsistencyLevel)
    {
      this.serializerSettings = serializerSettings;
    }

    public DocumentClient(
      Uri serviceEndpoint,
      string authKeyOrResourceToken,
      ConnectionPolicy connectionPolicy = null,
      ConsistencyLevel? desiredConsistencyLevel = null)
      : this(serviceEndpoint, authKeyOrResourceToken, (EventHandler<SendingRequestEventArgs>) null, connectionPolicy, desiredConsistencyLevel)
    {
    }

    public DocumentClient(
      Uri serviceEndpoint,
      string authKeyOrResourceToken,
      HttpMessageHandler handler,
      ConnectionPolicy connectionPolicy = null,
      ConsistencyLevel? desiredConsistencyLevel = null)
      : this(serviceEndpoint, authKeyOrResourceToken, (EventHandler<SendingRequestEventArgs>) null, connectionPolicy, desiredConsistencyLevel, handler: handler)
    {
    }

    internal DocumentClient(
      Uri serviceEndpoint,
      string authKeyOrResourceToken,
      EventHandler<SendingRequestEventArgs> sendingRequestEventArgs,
      ConnectionPolicy connectionPolicy = null,
      ConsistencyLevel? desiredConsistencyLevel = null,
      JsonSerializerSettings serializerSettings = null,
      ApiType apitype = ApiType.None,
      EventHandler<ReceivedResponseEventArgs> receivedResponseEventArgs = null,
      HttpMessageHandler handler = null,
      ISessionContainer sessionContainer = null,
      bool? enableCpuMonitor = null,
      IStoreClientFactory storeClientFactory = null,
      SecureString secureAuthKey = null)
    {
      if (authKeyOrResourceToken == null && secureAuthKey == null)
        throw new ArgumentNullException("authKeyOrResourceToken | secureAuthKey");
      if (authKeyOrResourceToken != null && secureAuthKey != null)
        throw new ArgumentException("Both authKeyOrResourceToken and secureAuthKey provided");
      if (sendingRequestEventArgs != null)
        this.sendingRequest += sendingRequestEventArgs;
      if (serializerSettings != null)
        this.serializerSettings = serializerSettings;
      this.ApiType = apitype;
      if (receivedResponseEventArgs != null)
        this.receivedResponse += receivedResponseEventArgs;
      if (secureAuthKey != null)
        this.authKeyHashFunction = (IComputeHash) new SecureStringHMACSHA256Helper(secureAuthKey);
      else if (AuthorizationHelper.IsResourceToken(authKeyOrResourceToken))
      {
        this.hasAuthKeyResourceToken = true;
        this.authKeyResourceToken = authKeyOrResourceToken;
      }
      else
        this.authKeyHashFunction = (IComputeHash) new StringHMACSHA256Hash(authKeyOrResourceToken);
      this.Initialize(serviceEndpoint, connectionPolicy, desiredConsistencyLevel, handler, sessionContainer, enableCpuMonitor, storeClientFactory);
    }

    [Obsolete("Please use the constructor that takes JsonSerializerSettings as the third parameter.")]
    public DocumentClient(
      Uri serviceEndpoint,
      string authKeyOrResourceToken,
      ConnectionPolicy connectionPolicy,
      ConsistencyLevel? desiredConsistencyLevel,
      JsonSerializerSettings serializerSettings)
      : this(serviceEndpoint, authKeyOrResourceToken, connectionPolicy, desiredConsistencyLevel)
    {
      this.serializerSettings = serializerSettings;
    }

    public DocumentClient(
      Uri serviceEndpoint,
      string authKeyOrResourceToken,
      JsonSerializerSettings serializerSettings,
      ConnectionPolicy connectionPolicy = null,
      ConsistencyLevel? desiredConsistencyLevel = null)
      : this(serviceEndpoint, authKeyOrResourceToken, connectionPolicy, desiredConsistencyLevel)
    {
      this.serializerSettings = serializerSettings;
    }

    public DocumentClient(
      Uri serviceEndpoint,
      IList<Permission> permissionFeed,
      ConnectionPolicy connectionPolicy = null,
      ConsistencyLevel? desiredConsistencyLevel = null)
      : this(serviceEndpoint, (IList<ResourceToken>) DocumentClient.GetResourceTokens(permissionFeed), connectionPolicy, desiredConsistencyLevel)
    {
    }

    private static List<ResourceToken> GetResourceTokens(IList<Permission> permissionFeed)
    {
      if (permissionFeed == null)
        throw new ArgumentNullException(nameof (permissionFeed));
      return permissionFeed.Select<Permission, ResourceToken>((Func<Permission, ResourceToken>) (permission => new ResourceToken()
      {
        ResourceLink = permission.ResourceLink,
        ResourcePartitionKey = permission.ResourcePartitionKey != null ? permission.ResourcePartitionKey.InternalKey.ToObjectArray() : (object[]) null,
        Token = permission.Token
      })).ToList<ResourceToken>();
    }

    internal DocumentClient(
      Uri serviceEndpoint,
      IList<ResourceToken> resourceTokens,
      ConnectionPolicy connectionPolicy = null,
      ConsistencyLevel? desiredConsistencyLevel = null)
    {
      if (resourceTokens == null)
        throw new ArgumentNullException(nameof (resourceTokens));
      this.resourceTokens = (IDictionary<string, List<PartitionKeyAndResourceTokenPair>>) new Dictionary<string, List<PartitionKeyAndResourceTokenPair>>();
      foreach (ResourceToken resourceToken in (IEnumerable<ResourceToken>) resourceTokens)
      {
        bool isNameBased = false;
        bool isFeed = false;
        string resourceIdOrFullName;
        if (!PathsHelper.TryParsePathSegments(resourceToken.ResourceLink, out isFeed, out string _, out resourceIdOrFullName, out isNameBased))
          throw new ArgumentException(RMResources.BadUrl, "resourceToken.ResourceLink");
        List<PartitionKeyAndResourceTokenPair> resourceTokenPairList;
        if (!this.resourceTokens.TryGetValue(resourceIdOrFullName, out resourceTokenPairList))
        {
          resourceTokenPairList = new List<PartitionKeyAndResourceTokenPair>();
          this.resourceTokens.Add(resourceIdOrFullName, resourceTokenPairList);
        }
        resourceTokenPairList.Add(new PartitionKeyAndResourceTokenPair(resourceToken.ResourcePartitionKey != null ? PartitionKeyInternal.FromObjectArray((IEnumerable<object>) resourceToken.ResourcePartitionKey, true) : PartitionKeyInternal.Empty, resourceToken.Token));
      }
      if (!this.resourceTokens.Any<KeyValuePair<string, List<PartitionKeyAndResourceTokenPair>>>())
        throw new ArgumentException("permissionFeed");
      string token = resourceTokens.First<ResourceToken>().Token;
      if (AuthorizationHelper.IsResourceToken(token))
      {
        this.hasAuthKeyResourceToken = true;
        this.authKeyResourceToken = token;
        this.Initialize(serviceEndpoint, connectionPolicy, desiredConsistencyLevel);
      }
      else
      {
        this.authKeyHashFunction = (IComputeHash) new StringHMACSHA256Hash(token);
        this.Initialize(serviceEndpoint, connectionPolicy, desiredConsistencyLevel);
      }
    }

    [Obsolete("Please use the constructor that takes a permission list or a resource token list.")]
    public DocumentClient(
      Uri serviceEndpoint,
      IDictionary<string, string> resourceTokens,
      ConnectionPolicy connectionPolicy = null,
      ConsistencyLevel? desiredConsistencyLevel = null)
    {
      if (resourceTokens == null)
        throw new ArgumentNullException(nameof (resourceTokens));
      if (resourceTokens.Count<KeyValuePair<string, string>>() == 0)
        throw new DocumentClientException(RMResources.InsufficientResourceTokens, (Exception) null, new HttpStatusCode?());
      this.resourceTokens = (IDictionary<string, List<PartitionKeyAndResourceTokenPair>>) resourceTokens.ToDictionary<KeyValuePair<string, string>, string, List<PartitionKeyAndResourceTokenPair>>((Func<KeyValuePair<string, string>, string>) (pair => pair.Key), (Func<KeyValuePair<string, string>, List<PartitionKeyAndResourceTokenPair>>) (pair => new List<PartitionKeyAndResourceTokenPair>()
      {
        new PartitionKeyAndResourceTokenPair(PartitionKeyInternal.Empty, pair.Value)
      }));
      string str = resourceTokens.ElementAt<KeyValuePair<string, string>>(0).Value;
      if (string.IsNullOrEmpty(str))
        throw new DocumentClientException(RMResources.InsufficientResourceTokens, (Exception) null, new HttpStatusCode?());
      if (AuthorizationHelper.IsResourceToken(str))
      {
        this.hasAuthKeyResourceToken = true;
        this.authKeyResourceToken = str;
        this.Initialize(serviceEndpoint, connectionPolicy, desiredConsistencyLevel);
      }
      else
      {
        this.authKeyHashFunction = (IComputeHash) new StringHMACSHA256Hash(str);
        this.Initialize(serviceEndpoint, connectionPolicy, desiredConsistencyLevel);
      }
    }

    internal async Task<ClientCollectionCache> GetCollectionCacheAsync()
    {
      await this.EnsureValidClientAsync();
      return this.collectionCache;
    }

    internal async Task<PartitionKeyRangeCache> GetPartitionKeyRangeCacheAsync()
    {
      await this.EnsureValidClientAsync();
      return this.partitionKeyRangeCache;
    }

    internal GlobalAddressResolver AddressResolver { get; private set; }

    internal event EventHandler<SendingRequestEventArgs> SendingRequest
    {
      add => this.sendingRequest += value;
      remove => this.sendingRequest -= value;
    }

    internal GlobalEndpointManager GlobalEndpointManager => this.globalEndpointManager;

    public Task OpenAsync(CancellationToken cancellationToken = default (CancellationToken)) => TaskHelper.InlineIfPossible((Func<Task>) (() => this.OpenPrivateInlineAsync(cancellationToken)), (IRetryPolicy) null, cancellationToken);

    private async Task OpenPrivateInlineAsync(CancellationToken cancellationToken)
    {
      await this.EnsureValidClientAsync();
      await TaskHelper.InlineIfPossible((Func<Task>) (() => this.OpenPrivateAsync(cancellationToken)), (IRetryPolicy) this.ResetSessionTokenRetryPolicy.GetRequestPolicy(), cancellationToken);
    }

    private async Task OpenPrivateAsync(CancellationToken cancellationToken)
    {
      DocumentClient documentClient = this;
      ResourceFeedReader<Database> databaseFeedReader = documentClient.CreateDatabaseFeedReader(new FeedOptions()
      {
        MaxItemCount = new int?(-1)
      });
      try
      {
        while (databaseFeedReader.HasMoreResults)
        {
          foreach (Database database1 in await databaseFeedReader.ExecuteNextAsync(cancellationToken))
          {
            Database database = database1;
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
      }
      catch (DocumentClientException ex)
      {
        documentClient.collectionCache = new ClientCollectionCache(documentClient.sessionContainer, documentClient.gatewayStoreModel, (IAuthorizationTokenProvider) documentClient, (IRetryPolicyFactory) documentClient.retryPolicy);
        documentClient.partitionKeyRangeCache = new PartitionKeyRangeCache((IAuthorizationTokenProvider) documentClient, documentClient.gatewayStoreModel, (CollectionCache) documentClient.collectionCache);
        DefaultTrace.TraceWarning("{0} occurred while OpenAsync. Exception Message: {1}", (object) ex.ToString(), (object) ex.Message);
      }
    }

    private void Initialize(
      Uri serviceEndpoint,
      ConnectionPolicy connectionPolicy = null,
      ConsistencyLevel? desiredConsistencyLevel = null,
      HttpMessageHandler handler = null,
      ISessionContainer sessionContainer = null,
      bool? enableCpuMonitor = null,
      IStoreClientFactory storeClientFactory = null)
    {
      if (serviceEndpoint == (Uri) null)
        throw new ArgumentNullException(nameof (serviceEndpoint));
      DefaultTrace.InitEventListener();
      if (this.ConnectionPolicy != null)
      {
        TimeSpan? connectionTimeout = this.ConnectionPolicy.IdleTcpConnectionTimeout;
        if (connectionTimeout.HasValue)
        {
          connectionTimeout = this.ConnectionPolicy.IdleTcpConnectionTimeout;
          this.idleConnectionTimeoutInSeconds = (int) connectionTimeout.Value.TotalSeconds;
        }
        connectionTimeout = this.ConnectionPolicy.OpenTcpConnectionTimeout;
        if (connectionTimeout.HasValue)
        {
          connectionTimeout = this.ConnectionPolicy.OpenTcpConnectionTimeout;
          this.openConnectionTimeoutInSeconds = (int) connectionTimeout.Value.TotalSeconds;
        }
        int? nullable = this.ConnectionPolicy.MaxRequestsPerTcpConnection;
        if (nullable.HasValue)
        {
          nullable = this.ConnectionPolicy.MaxRequestsPerTcpConnection;
          this.maxRequestsPerRntbdChannel = nullable.Value;
        }
        nullable = this.ConnectionPolicy.MaxTcpPartitionCount;
        if (nullable.HasValue)
        {
          nullable = this.ConnectionPolicy.MaxTcpPartitionCount;
          this.rntbdPartitionCount = nullable.Value;
        }
        nullable = this.ConnectionPolicy.MaxTcpConnectionsPerEndpoint;
        if (nullable.HasValue)
        {
          nullable = this.ConnectionPolicy.MaxTcpConnectionsPerEndpoint;
          this.maxRntbdChannels = nullable.Value;
        }
        PortReuseMode? portReuseMode = this.ConnectionPolicy.PortReuseMode;
        if (portReuseMode.HasValue)
        {
          portReuseMode = this.ConnectionPolicy.PortReuseMode;
          this.rntbdPortReuseMode = portReuseMode.Value;
        }
        this.enableTcpConnectionEndpointRediscovery = this.ConnectionPolicy.EnableTcpConnectionEndpointRediscovery;
      }
      this.ServiceEndpoint = serviceEndpoint.OriginalString.EndsWith("/", StringComparison.Ordinal) ? serviceEndpoint : new Uri(serviceEndpoint.OriginalString + "/");
      this.connectionPolicy = connectionPolicy ?? ConnectionPolicy.Default;
      this.globalEndpointManager = new GlobalEndpointManager((IDocumentClientInternal) this, this.connectionPolicy);
      this.httpMessageHandler = (HttpMessageHandler) new DocumentClient.HttpRequestMessageHandler(this.sendingRequest, this.receivedResponse, handler);
      this.mediaClient = new HttpClient(this.httpMessageHandler);
      this.mediaClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue()
      {
        NoCache = true
      };
      this.mediaClient.AddUserAgentHeader(this.connectionPolicy.UserAgentContainer);
      this.mediaClient.AddApiTypeHeader(this.ApiType);
      this.mediaClient.DefaultRequestHeaders.Add("x-ms-version", HttpConstants.Versions.CurrentVersion);
      this.mediaClient.DefaultRequestHeaders.Add("Accept", "*/*");
      this.sessionContainer = sessionContainer == null ? (ISessionContainer) new SessionContainer(this.ServiceEndpoint.Host) : sessionContainer;
      this.retryPolicy = new RetryPolicy(this.globalEndpointManager, this.connectionPolicy);
      this.ResetSessionTokenRetryPolicy = (IRetryPolicyFactory) this.retryPolicy;
      this.mediaClient.Timeout = this.connectionPolicy.MediaRequestTimeout;
      this.desiredConsistencyLevel = desiredConsistencyLevel;
      this.initializationSyncLock = new object();
      this.PartitionResolvers = (IDictionary<string, IPartitionResolver>) new ConcurrentDictionary<string, IPartitionResolver>();
      this.eventSource = DocumentClientEventSource.Instance;
      this.initializeTask = TaskHelper.InlineIfPossible((Func<Task>) (() => this.GetInitializationTask(storeClientFactory)), (IRetryPolicy) new ResourceThrottleRetryPolicy(this.connectionPolicy.RetryOptions.MaxRetryAttemptsOnThrottledRequests, this.connectionPolicy.RetryOptions.MaxRetryWaitTimeInSeconds));
      this.initializeTask.ContinueWith((Action<Task>) (t => DefaultTrace.TraceWarning("initializeTask failed {0}", (object) t.Exception)), TaskContinuationOptions.OnlyOnFaulted);
      this.traceId = Interlocked.Increment(ref DocumentClient.idCounter);
      DefaultTrace.TraceInformation(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "DocumentClient with id {0} initialized at endpoint: {1} with ConnectionMode: {2}, connection Protocol: {3}, and consistency level: {4}", (object) this.traceId, (object) serviceEndpoint.ToString(), (object) this.connectionPolicy.ConnectionMode.ToString(), (object) this.connectionPolicy.ConnectionProtocol.ToString(), desiredConsistencyLevel.HasValue ? (object) desiredConsistencyLevel.ToString() : (object) "null"));
      this.QueryCompatibilityMode = QueryCompatibilityMode.Default;
    }

    private async Task GetInitializationTask(IStoreClientFactory storeClientFactory)
    {
      DocumentClient tokenProvider = this;
      await tokenProvider.InitializeGatewayConfigurationReader();
      if (tokenProvider.desiredConsistencyLevel.HasValue)
        tokenProvider.EnsureValidOverwrite(tokenProvider.desiredConsistencyLevel.Value);
      Microsoft.Azure.Documents.GatewayStoreModel gatewayStoreModel = new Microsoft.Azure.Documents.GatewayStoreModel(tokenProvider.globalEndpointManager, tokenProvider.sessionContainer, tokenProvider.connectionPolicy.RequestTimeout, tokenProvider.gatewayConfigurationReader.DefaultConsistencyLevel, tokenProvider.eventSource, tokenProvider.serializerSettings, tokenProvider.connectionPolicy.UserAgentContainer, tokenProvider.ApiType, tokenProvider.httpMessageHandler);
      tokenProvider.gatewayStoreModel = (IStoreModel) gatewayStoreModel;
      tokenProvider.collectionCache = new ClientCollectionCache(tokenProvider.sessionContainer, tokenProvider.gatewayStoreModel, (IAuthorizationTokenProvider) tokenProvider, (IRetryPolicyFactory) tokenProvider.retryPolicy);
      tokenProvider.partitionKeyRangeCache = new PartitionKeyRangeCache((IAuthorizationTokenProvider) tokenProvider, tokenProvider.gatewayStoreModel, (CollectionCache) tokenProvider.collectionCache);
      tokenProvider.ResetSessionTokenRetryPolicy = (IRetryPolicyFactory) new DocumentClient.ResetSessionTokenRetryPolicyFactory(tokenProvider.sessionContainer, tokenProvider.collectionCache, (IRetryPolicyFactory) tokenProvider.retryPolicy);
      if (tokenProvider.connectionPolicy.ConnectionMode == ConnectionMode.Gateway)
        tokenProvider.storeModel = tokenProvider.gatewayStoreModel;
      else
        tokenProvider.InitializeDirectConnectivity(storeClientFactory);
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
        collection = await (await this.GetCollectionCacheAsync()).ResolveCollectionAsync(request, CancellationToken.None);
        IReadOnlyList<PartitionKeyRange> overlappingRangesAsync = await this.partitionKeyRangeCache.TryGetOverlappingRangesAsync(collection.ResourceId, new Range<string>(PartitionKeyInternal.MinimumInclusiveEffectivePartitionKey, PartitionKeyInternal.MaximumExclusiveEffectivePartitionKey, true, false), false);
        if (this.AddressResolver != null)
          await this.AddressResolver.OpenAsync(databaseName, collection, cancellationToken);
      }
    }

    [Obsolete("Support for IPartitionResolver is now obsolete. It's recommended that you use partitioned collections for higher storage and throughput.")]
    public IDictionary<string, IPartitionResolver> PartitionResolvers { get; private set; }

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

    public Uri ServiceEndpoint { get; private set; }

    public Uri WriteEndpoint => this.globalEndpointManager.WriteEndpoints.FirstOrDefault<Uri>();

    public Uri ReadEndpoint => this.globalEndpointManager.ReadEndpoints.FirstOrDefault<Uri>();

    public ConnectionPolicy ConnectionPolicy => this.connectionPolicy;

    [Obsolete]
    public IDictionary<string, string> ResourceTokens => this.resourceTokens == null ? (IDictionary<string, string>) null : (IDictionary<string, string>) this.resourceTokens.ToDictionary<KeyValuePair<string, List<PartitionKeyAndResourceTokenPair>>, string, string>((Func<KeyValuePair<string, List<PartitionKeyAndResourceTokenPair>>, string>) (pair => pair.Key), (Func<KeyValuePair<string, List<PartitionKeyAndResourceTokenPair>>, string>) (pair => pair.Value.First<PartitionKeyAndResourceTokenPair>().ResourceToken));

    public SecureString AuthKey => this.authKeyHashFunction != null ? this.authKeyHashFunction.Key : (SecureString) null;

    public ConsistencyLevel ConsistencyLevel
    {
      get
      {
        TaskHelper.InlineIfPossible((Func<Task>) (() => this.EnsureValidClientAsync()), (IRetryPolicy) null).Wait();
        return !this.desiredConsistencyLevel.HasValue ? this.gatewayConfigurationReader.DefaultConsistencyLevel : this.desiredConsistencyLevel.Value;
      }
    }

    public void Dispose()
    {
      if (this.isDisposed)
        return;
      if (this.storeModel != null)
      {
        this.storeModel.Dispose();
        this.storeModel = (IStoreModel) null;
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
      if (this.mediaClient != null)
      {
        this.mediaClient.Dispose();
        this.mediaClient = (HttpClient) null;
      }
      if (this.authKeyHashFunction != null)
      {
        this.authKeyHashFunction.Dispose();
        this.authKeyHashFunction = (IComputeHash) null;
      }
      if (this.globalEndpointManager != null)
      {
        this.globalEndpointManager.Dispose();
        this.globalEndpointManager = (GlobalEndpointManager) null;
      }
      DefaultTrace.TraceInformation("DocumentClient with id {0} disposed.", (object) this.traceId);
      DefaultTrace.Flush();
      this.isDisposed = true;
    }

    internal QueryCompatibilityMode QueryCompatibilityMode { get; set; }

    internal IRetryPolicyFactory ResetSessionTokenRetryPolicy { get; private set; }

    internal IStoreModel StoreModel
    {
      get => this.storeModel;
      set => this.storeModel = value;
    }

    internal IStoreModel GatewayStoreModel
    {
      get => this.gatewayStoreModel;
      set => this.gatewayStoreModel = value;
    }

    internal Action<IQueryable> OnExecuteScalarQueryCallback
    {
      get => this.onExecuteScalarQueryCallback;
      set => this.onExecuteScalarQueryCallback = value;
    }

    internal async Task<IDictionary<string, object>> GetQueryEngineConfiguration()
    {
      await this.EnsureValidClientAsync();
      return this.gatewayConfigurationReader.QueryEngineConfiguration;
    }

    internal async Task<ConsistencyLevel> GetDefaultConsistencyLevelAsync()
    {
      await this.EnsureValidClientAsync();
      return this.gatewayConfigurationReader.DefaultConsistencyLevel;
    }

    internal Task<ConsistencyLevel?> GetDesiredConsistencyLevelAsync() => Task.FromResult<ConsistencyLevel?>(this.desiredConsistencyLevel);

    internal async Task<DocumentServiceResponse> ProcessRequestAsync(
      string verb,
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken,
      string testAuthorization = null)
    {
      DocumentClient documentClient = this;
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      if (verb == null)
        throw new ArgumentNullException(nameof (verb));
      string payload;
      string authorization = ((IAuthorizationTokenProvider) documentClient).GetUserAuthorizationToken(request.ResourceAddress, PathsHelper.GetResourcePath(request.ResourceType), verb, request.Headers, AuthorizationTokenType.PrimaryMasterKey, out payload);
      if (testAuthorization != null)
      {
        payload = testAuthorization;
        authorization = testAuthorization;
      }
      request.Headers["authorization"] = authorization;
      DocumentServiceResponse documentServiceResponse;
      try
      {
        documentServiceResponse = await documentClient.ProcessRequestAsync(request, retryPolicyInstance, cancellationToken);
      }
      catch (DocumentClientException ex)
      {
        if (payload != null && ex.Message != null && ex.StatusCode.HasValue && ex.StatusCode.Value == HttpStatusCode.Unauthorized && ex.Message.Contains("to sign"))
        {
          string str1 = DocumentClient.NormalizeAuthorizationPayload(payload);
          if (documentClient.enableAuthFailureTraces)
          {
            string str2 = HttpUtility.UrlDecode(authorization).Split('&')[2].Split('=')[1].Substring(0, 5);
            ulong num = 0;
            if (documentClient.authKeyHashFunction?.Key != null)
            {
              byte[] bytes = Encoding.UTF8.GetBytes(documentClient.authKeyHashFunction?.Key?.ToString());
              num = MurmurHash3.Hash64(bytes, bytes.Length);
            }
            DefaultTrace.TraceError("Un-expected authorization payload mis-match. Actual payload={0}, token={1}..., hash={2:X}..., error={3}", (object) str1, (object) str2, (object) num, (object) ex.Message);
          }
          else
            DefaultTrace.TraceError("Un-expected authorization payload mis-match. Actual {0} service expected {1}", (object) str1, (object) ex.Message);
        }
        throw;
      }
      return documentServiceResponse;
    }

    internal async Task<DocumentServiceResponse> ProcessRequestAsync(
      DocumentServiceRequest request,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      await this.EnsureValidClientAsync();
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

    private void ThrowIfDisposed()
    {
      if (this.isDisposed)
        throw new ObjectDisposedException(nameof (DocumentClient));
    }

    private async Task EnsureValidClientAsync()
    {
      this.ThrowIfDisposed();
      if (this.isSuccessfullyInitialized)
        return;
      Task initTask = (Task) null;
      lock (this.initializationSyncLock)
        initTask = this.initializeTask;
      try
      {
        await initTask;
        this.isSuccessfullyInitialized = true;
        return;
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceWarning("initializeTask failed {0}", (object) ex.ToString());
      }
      lock (this.initializationSyncLock)
      {
        if (this.initializeTask == initTask)
          this.initializeTask = this.GetInitializationTask((IStoreClientFactory) null);
        initTask = this.initializeTask;
      }
      await initTask;
      this.isSuccessfullyInitialized = true;
    }

    internal Task<ResourceResponse<ClientEncryptionKey>> CreateClientEncryptionKeyAsync(
      string dbLink,
      ClientEncryptionKey key,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<ClientEncryptionKey>>((Func<Task<ResourceResponse<ClientEncryptionKey>>>) (() => this.CreateClientEncryptionKeyPrivateAsync(dbLink, key, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<ClientEncryptionKey>> CreateClientEncryptionKeyPrivateAsync(
      string dbLink,
      ClientEncryptionKey dek,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (dek == null)
        throw new ArgumentNullException(nameof (dek));
      this.ValidateResource((Resource) dek);
      ResourceResponse<ClientEncryptionKey> encryptionKeyPrivateAsync;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Create, dbLink, (Resource) dek, ResourceType.ClientEncryptionKey, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        encryptionKeyPrivateAsync = new ResourceResponse<ClientEncryptionKey>(await this.CreateAsync(request, retryPolicyInstance));
      return encryptionKeyPrivateAsync;
    }

    public Task<ResourceResponse<Database>> CreateDatabaseAsync(
      Database database,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Database>>((Func<Task<ResourceResponse<Database>>>) (() => this.CreateDatabasePrivateAsync(database, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Database>> CreateDatabasePrivateAsync(
      Database database,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (database == null)
        throw new ArgumentNullException(nameof (database));
      this.ValidateResource((Resource) database);
      ResourceResponse<Database> databasePrivateAsync;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Create, "//dbs/", (Resource) database, ResourceType.Database, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        databasePrivateAsync = new ResourceResponse<Database>(await this.CreateAsync(request, retryPolicyInstance));
      return databasePrivateAsync;
    }

    public Task<ResourceResponse<Database>> CreateDatabaseIfNotExistsAsync(
      Database database,
      RequestOptions options = null)
    {
      return TaskHelper.InlineIfPossible<ResourceResponse<Database>>((Func<Task<ResourceResponse<Database>>>) (() => this.CreateDatabaseIfNotExistsPrivateAsync(database, options)), (IRetryPolicy) null);
    }

    private async Task<ResourceResponse<Database>> CreateDatabaseIfNotExistsPrivateAsync(
      Database database,
      RequestOptions options)
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
      RequestOptions options = null,
      bool disableAutomaticIdGeneration = false,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return TaskHelper.InlineIfPossible<ResourceResponse<Document>>((Func<Task<ResourceResponse<Document>>>) (() => this.CreateDocumentInlineAsync(documentsFeedOrDatabaseLink, document, options, disableAutomaticIdGeneration, cancellationToken)), (IRetryPolicy) null, cancellationToken);
    }

    private async Task<ResourceResponse<Document>> CreateDocumentInlineAsync(
      string documentsFeedOrDatabaseLink,
      object document,
      RequestOptions options,
      bool disableAutomaticIdGeneration,
      CancellationToken cancellationToken)
    {
      IPartitionResolver partitionResolver = (IPartitionResolver) null;
      IDocumentClientRetryPolicy requestRetryPolicy = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      if (this.PartitionResolvers.TryGetValue(documentsFeedOrDatabaseLink, out partitionResolver))
      {
        object partitionKey = partitionResolver.GetPartitionKey(document);
        string collectionLink = partitionResolver.ResolveForCreate(partitionKey);
        return await TaskHelper.InlineIfPossible<ResourceResponse<Document>>((Func<Task<ResourceResponse<Document>>>) (() => this.CreateDocumentPrivateAsync(collectionLink, document, options, disableAutomaticIdGeneration, requestRetryPolicy, cancellationToken)), (IRetryPolicy) requestRetryPolicy);
      }
      if (options == null || options.PartitionKey == null)
        requestRetryPolicy = (IDocumentClientRetryPolicy) new PartitionKeyMismatchRetryPolicy((CollectionCache) await this.GetCollectionCacheAsync(), requestRetryPolicy);
      return await TaskHelper.InlineIfPossible<ResourceResponse<Document>>((Func<Task<ResourceResponse<Document>>>) (() => this.CreateDocumentPrivateAsync(documentsFeedOrDatabaseLink, document, options, disableAutomaticIdGeneration, requestRetryPolicy, cancellationToken)), (IRetryPolicy) requestRetryPolicy);
    }

    private async Task<ResourceResponse<Document>> CreateDocumentPrivateAsync(
      string documentCollectionLink,
      object document,
      RequestOptions options,
      bool disableAutomaticIdGeneration,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(documentCollectionLink))
        throw new ArgumentNullException(nameof (documentCollectionLink));
      if (document == null)
        throw new ArgumentNullException(nameof (document));
      INameValueCollection requestHeaders = this.GetRequestHeaders(options);
      Document document1 = Document.FromObject(document, this.GetSerializerSettingsForRequest(options));
      this.ValidateResource((Resource) document1);
      if (string.IsNullOrEmpty(document1.Id) && !disableAutomaticIdGeneration)
        document1.Id = Guid.NewGuid().ToString();
      ResourceResponse<Document> documentPrivateAsync;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Create, documentCollectionLink, (Resource) document1, ResourceType.Document, AuthorizationTokenType.PrimaryMasterKey, requestHeaders, settings: this.GetSerializerSettingsForRequest(options)))
      {
        await this.AddPartitionKeyInformationAsync(request, document1, options);
        documentPrivateAsync = new ResourceResponse<Document>(await this.CreateAsync(request, retryPolicyInstance, cancellationToken));
      }
      return documentPrivateAsync;
    }

    public Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionAsync(
      string databaseLink,
      DocumentCollection documentCollection,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<DocumentCollection>>((Func<Task<ResourceResponse<DocumentCollection>>>) (() => this.CreateDocumentCollectionPrivateAsync(databaseLink, documentCollection, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionPrivateAsync(
      string databaseLink,
      DocumentCollection documentCollection,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(databaseLink))
        throw new ArgumentNullException(nameof (databaseLink));
      if (documentCollection == null)
        throw new ArgumentNullException(nameof (documentCollection));
      this.ValidateResource((Resource) documentCollection);
      ResourceResponse<DocumentCollection> collectionPrivateAsync;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Create, databaseLink, (Resource) documentCollection, ResourceType.Collection, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
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
      RequestOptions options = null)
    {
      return TaskHelper.InlineIfPossible<ResourceResponse<DocumentCollection>>((Func<Task<ResourceResponse<DocumentCollection>>>) (() => this.CreateDocumentCollectionIfNotExistsPrivateAsync(databaseLink, documentCollection, options)), (IRetryPolicy) null);
    }

    private async Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionIfNotExistsPrivateAsync(
      string databaseLink,
      DocumentCollection documentCollection,
      RequestOptions options)
    {
      if (string.IsNullOrEmpty(databaseLink))
        throw new ArgumentNullException(nameof (databaseLink));
      if (documentCollection == null)
        throw new ArgumentNullException(nameof (documentCollection));
      Database database = (Database) await this.ReadDatabaseAsync(databaseLink);
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
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<DocumentCollection>>((Func<Task<ResourceResponse<DocumentCollection>>>) (() => this.RestoreDocumentCollectionPrivateAsync(sourceDocumentCollectionLink, targetDocumentCollection, restoreTime, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<DocumentCollection>> RestoreDocumentCollectionPrivateAsync(
      string sourceDocumentCollectionLink,
      DocumentCollection targetDocumentCollection,
      DateTimeOffset? restoreTime,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
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
      this.ValidateResource((Resource) targetDocumentCollection);
      if (options == null)
        options = new RequestOptions();
      if (!options.RemoteStorageType.HasValue)
        options.RemoteStorageType = new RemoteStorageType?(RemoteStorageType.Standard);
      options.SourceDatabaseId = str1;
      options.SourceCollectionId = str2;
      if (restoreTime.HasValue)
        options.RestorePointInTime = new long?(Helpers.ToUnixTime(restoreTime.Value));
      INameValueCollection requestHeaders = this.GetRequestHeaders(options);
      ResourceResponse<DocumentCollection> resourceResponse1;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Create, databasePath, (Resource) targetDocumentCollection, ResourceType.Collection, AuthorizationTokenType.PrimaryMasterKey, requestHeaders))
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
      RequestOptions options = new RequestOptions();
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
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<StoredProcedure>>((Func<Task<ResourceResponse<StoredProcedure>>>) (() => this.CreateStoredProcedurePrivateAsync(collectionLink, storedProcedure, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<StoredProcedure>> CreateStoredProcedurePrivateAsync(
      string collectionLink,
      StoredProcedure storedProcedure,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(collectionLink))
        throw new ArgumentNullException(nameof (collectionLink));
      if (storedProcedure == null)
        throw new ArgumentNullException(nameof (storedProcedure));
      this.ValidateResource((Resource) storedProcedure);
      ResourceResponse<StoredProcedure> procedurePrivateAsync;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Create, collectionLink, (Resource) storedProcedure, ResourceType.StoredProcedure, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        procedurePrivateAsync = new ResourceResponse<StoredProcedure>(await this.CreateAsync(request, retryPolicyInstance));
      return procedurePrivateAsync;
    }

    public Task<ResourceResponse<Trigger>> CreateTriggerAsync(
      string collectionLink,
      Trigger trigger,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Trigger>>((Func<Task<ResourceResponse<Trigger>>>) (() => this.CreateTriggerPrivateAsync(collectionLink, trigger, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Trigger>> CreateTriggerPrivateAsync(
      string collectionLink,
      Trigger trigger,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(collectionLink))
        throw new ArgumentNullException(nameof (collectionLink));
      if (trigger == null)
        throw new ArgumentNullException(nameof (trigger));
      this.ValidateResource((Resource) trigger);
      ResourceResponse<Trigger> triggerPrivateAsync;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Create, collectionLink, (Resource) trigger, ResourceType.Trigger, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        triggerPrivateAsync = new ResourceResponse<Trigger>(await this.CreateAsync(request, retryPolicyInstance));
      return triggerPrivateAsync;
    }

    public Task<ResourceResponse<UserDefinedFunction>> CreateUserDefinedFunctionAsync(
      string collectionLink,
      UserDefinedFunction function,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<UserDefinedFunction>>((Func<Task<ResourceResponse<UserDefinedFunction>>>) (() => this.CreateUserDefinedFunctionPrivateAsync(collectionLink, function, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<UserDefinedFunction>> CreateUserDefinedFunctionPrivateAsync(
      string collectionLink,
      UserDefinedFunction function,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(collectionLink))
        throw new ArgumentNullException(nameof (collectionLink));
      if (function == null)
        throw new ArgumentNullException(nameof (function));
      this.ValidateResource((Resource) function);
      ResourceResponse<UserDefinedFunction> functionPrivateAsync;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Create, collectionLink, (Resource) function, ResourceType.UserDefinedFunction, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        functionPrivateAsync = new ResourceResponse<UserDefinedFunction>(await this.CreateAsync(request, retryPolicyInstance));
      return functionPrivateAsync;
    }

    internal Task<ResourceResponse<UserDefinedType>> CreateUserDefinedTypeAsync(
      string databaseLink,
      UserDefinedType userDefinedType,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<UserDefinedType>>((Func<Task<ResourceResponse<UserDefinedType>>>) (() => this.CreateUserDefinedTypePrivateAsync(databaseLink, userDefinedType, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<UserDefinedType>> CreateUserDefinedTypePrivateAsync(
      string databaseLink,
      UserDefinedType userDefinedType,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(databaseLink))
        throw new ArgumentNullException(nameof (databaseLink));
      if (userDefinedType == null)
        throw new ArgumentNullException(nameof (userDefinedType));
      this.ValidateResource((Resource) userDefinedType);
      ResourceResponse<UserDefinedType> typePrivateAsync;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Create, databaseLink, (Resource) userDefinedType, ResourceType.UserDefinedType, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        typePrivateAsync = new ResourceResponse<UserDefinedType>(await this.CreateAsync(request, retryPolicyInstance));
      return typePrivateAsync;
    }

    internal Task<ResourceResponse<Snapshot>> CreateSnapshotAsync(
      Snapshot snapshot,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Snapshot>>((Func<Task<ResourceResponse<Snapshot>>>) (() => this.CreateSnapshotPrivateAsync(snapshot, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Snapshot>> CreateSnapshotPrivateAsync(
      Snapshot snapshot,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (snapshot == null)
        throw new ArgumentNullException(nameof (snapshot));
      this.ValidateResource((Resource) snapshot);
      ResourceResponse<Snapshot> snapshotPrivateAsync;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Create, "//snapshots/", (Resource) snapshot, ResourceType.Snapshot, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        snapshotPrivateAsync = new ResourceResponse<Snapshot>(await this.CreateAsync(request, retryPolicyInstance));
      return snapshotPrivateAsync;
    }

    internal Task<ResourceResponse<ClientEncryptionKey>> DeleteClientEncryptionKeyAsync(
      string keyLink,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<ClientEncryptionKey>>((Func<Task<ResourceResponse<ClientEncryptionKey>>>) (() => this.DeleteClientEncryptionKeyPrivateAsync(keyLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<ClientEncryptionKey>> DeleteClientEncryptionKeyPrivateAsync(
      string dekLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(dekLink))
        throw new ArgumentNullException("databaseLink");
      ResourceResponse<ClientEncryptionKey> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Delete, ResourceType.ClientEncryptionKey, dekLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        resourceResponse = new ResourceResponse<ClientEncryptionKey>(await this.DeleteAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<Database>> DeleteDatabaseAsync(
      string databaseLink,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Database>>((Func<Task<ResourceResponse<Database>>>) (() => this.DeleteDatabasePrivateAsync(databaseLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Database>> DeleteDatabasePrivateAsync(
      string databaseLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(databaseLink))
        throw new ArgumentNullException(nameof (databaseLink));
      ResourceResponse<Database> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Delete, ResourceType.Database, databaseLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        resourceResponse = new ResourceResponse<Database>(await this.DeleteAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<Document>> DeleteDocumentAsync(
      string documentLink,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Document>>((Func<Task<ResourceResponse<Document>>>) (() => this.DeleteDocumentPrivateAsync(documentLink, options, retryPolicyInstance, cancellationToken)), (IRetryPolicy) retryPolicyInstance, cancellationToken);
    }

    private async Task<ResourceResponse<Document>> DeleteDocumentPrivateAsync(
      string documentLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(documentLink))
        throw new ArgumentNullException(nameof (documentLink));
      ResourceResponse<Document> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Delete, ResourceType.Document, documentLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
      {
        await this.AddPartitionKeyInformationAsync(request, options);
        request.SerializerSettings = this.GetSerializerSettingsForRequest(options);
        resourceResponse = new ResourceResponse<Document>(await this.DeleteAsync(request, retryPolicyInstance, cancellationToken));
      }
      return resourceResponse;
    }

    public Task<ResourceResponse<DocumentCollection>> DeleteDocumentCollectionAsync(
      string documentCollectionLink,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<DocumentCollection>>((Func<Task<ResourceResponse<DocumentCollection>>>) (() => this.DeleteDocumentCollectionPrivateAsync(documentCollectionLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<DocumentCollection>> DeleteDocumentCollectionPrivateAsync(
      string documentCollectionLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(documentCollectionLink))
        throw new ArgumentNullException(nameof (documentCollectionLink));
      ResourceResponse<DocumentCollection> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Delete, ResourceType.Collection, documentCollectionLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        resourceResponse = new ResourceResponse<DocumentCollection>(await this.DeleteAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<StoredProcedure>> DeleteStoredProcedureAsync(
      string storedProcedureLink,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<StoredProcedure>>((Func<Task<ResourceResponse<StoredProcedure>>>) (() => this.DeleteStoredProcedurePrivateAsync(storedProcedureLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<StoredProcedure>> DeleteStoredProcedurePrivateAsync(
      string storedProcedureLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(storedProcedureLink))
        throw new ArgumentNullException(nameof (storedProcedureLink));
      ResourceResponse<StoredProcedure> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Delete, ResourceType.StoredProcedure, storedProcedureLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        resourceResponse = new ResourceResponse<StoredProcedure>(await this.DeleteAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<Trigger>> DeleteTriggerAsync(
      string triggerLink,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Trigger>>((Func<Task<ResourceResponse<Trigger>>>) (() => this.DeleteTriggerPrivateAsync(triggerLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Trigger>> DeleteTriggerPrivateAsync(
      string triggerLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(triggerLink))
        throw new ArgumentNullException(nameof (triggerLink));
      ResourceResponse<Trigger> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Delete, ResourceType.Trigger, triggerLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        resourceResponse = new ResourceResponse<Trigger>(await this.DeleteAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<UserDefinedFunction>> DeleteUserDefinedFunctionAsync(
      string functionLink,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<UserDefinedFunction>>((Func<Task<ResourceResponse<UserDefinedFunction>>>) (() => this.DeleteUserDefinedFunctionPrivateAsync(functionLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<UserDefinedFunction>> DeleteUserDefinedFunctionPrivateAsync(
      string functionLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(functionLink))
        throw new ArgumentNullException(nameof (functionLink));
      ResourceResponse<UserDefinedFunction> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Delete, ResourceType.UserDefinedFunction, functionLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        resourceResponse = new ResourceResponse<UserDefinedFunction>(await this.DeleteAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<Conflict>> DeleteConflictAsync(
      string conflictLink,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Conflict>>((Func<Task<ResourceResponse<Conflict>>>) (() => this.DeleteConflictPrivateAsync(conflictLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Conflict>> DeleteConflictPrivateAsync(
      string conflictLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(conflictLink))
        throw new ArgumentNullException(nameof (conflictLink));
      ResourceResponse<Conflict> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Delete, ResourceType.Conflict, conflictLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
      {
        await this.AddPartitionKeyInformationAsync(request, options);
        resourceResponse = new ResourceResponse<Conflict>(await this.DeleteAsync(request, retryPolicyInstance));
      }
      return resourceResponse;
    }

    internal Task<ResourceResponse<Snapshot>> DeleteSnapshotAsync(
      string snapshotLink,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Snapshot>>((Func<Task<ResourceResponse<Snapshot>>>) (() => this.DeleteSnapshotPrivateAsync(snapshotLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Snapshot>> DeleteSnapshotPrivateAsync(
      string snapshotLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(snapshotLink))
        throw new ArgumentNullException(nameof (snapshotLink));
      ResourceResponse<Snapshot> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Delete, ResourceType.Snapshot, snapshotLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        resourceResponse = new ResourceResponse<Snapshot>(await this.DeleteAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    internal Task<ResourceResponse<ClientEncryptionKey>> ReplaceClientEncryptionKeyAsync(
      string keyLink,
      ClientEncryptionKey key,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<ClientEncryptionKey>>((Func<Task<ResourceResponse<ClientEncryptionKey>>>) (() => this.ReplaceKeyPrivateAsync(keyLink, key, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<ClientEncryptionKey>> ReplaceKeyPrivateAsync(
      string keyLink,
      ClientEncryptionKey key,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      string altLink = null)
    {
      await this.EnsureValidClientAsync();
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      this.ValidateResource((Resource) key);
      ResourceResponse<ClientEncryptionKey> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Replace, keyLink, (Resource) key, ResourceType.ClientEncryptionKey, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        resourceResponse = new ResourceResponse<ClientEncryptionKey>(await this.UpdateAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<DocumentCollection>> ReplaceDocumentCollectionAsync(
      DocumentCollection documentCollection,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<DocumentCollection>>((Func<Task<ResourceResponse<DocumentCollection>>>) (() => this.ReplaceDocumentCollectionPrivateAsync(documentCollection, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<DocumentCollection>> ReplaceDocumentCollectionPrivateAsync(
      DocumentCollection documentCollection,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      string altLink = null)
    {
      await this.EnsureValidClientAsync();
      if (documentCollection == null)
        throw new ArgumentNullException(nameof (documentCollection));
      this.ValidateResource((Resource) documentCollection);
      INameValueCollection requestHeaders = this.GetRequestHeaders(options);
      ResourceResponse<DocumentCollection> resourceResponse1;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Replace, altLink ?? this.GetLinkForRouting((Resource) documentCollection), (Resource) documentCollection, ResourceType.Collection, AuthorizationTokenType.PrimaryMasterKey, requestHeaders))
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
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return TaskHelper.InlineIfPossible<ResourceResponse<Document>>((Func<Task<ResourceResponse<Document>>>) (() => this.ReplaceDocumentInlineAsync(documentLink, document, options, cancellationToken)), (IRetryPolicy) null, cancellationToken);
    }

    private async Task<ResourceResponse<Document>> ReplaceDocumentInlineAsync(
      string documentLink,
      object document,
      RequestOptions options,
      CancellationToken cancellationToken)
    {
      IDocumentClientRetryPolicy requestRetryPolicy = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      if (options == null || options.PartitionKey == null)
        requestRetryPolicy = (IDocumentClientRetryPolicy) new PartitionKeyMismatchRetryPolicy((CollectionCache) await this.GetCollectionCacheAsync(), requestRetryPolicy);
      return await TaskHelper.InlineIfPossible<ResourceResponse<Document>>((Func<Task<ResourceResponse<Document>>>) (() => this.ReplaceDocumentPrivateAsync(documentLink, document, options, requestRetryPolicy, cancellationToken)), (IRetryPolicy) requestRetryPolicy, cancellationToken);
    }

    private Task<ResourceResponse<Document>> ReplaceDocumentPrivateAsync(
      string documentLink,
      object document,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      if (string.IsNullOrEmpty(documentLink))
        throw new ArgumentNullException(nameof (documentLink));
      Document document1 = document != null ? Document.FromObject(document, this.GetSerializerSettingsForRequest(options)) : throw new ArgumentNullException(nameof (document));
      this.ValidateResource((Resource) document1);
      return this.ReplaceDocumentPrivateAsync(documentLink, document1, options, retryPolicyInstance, cancellationToken);
    }

    public Task<ResourceResponse<Document>> ReplaceDocumentAsync(
      Document document,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Document>>((Func<Task<ResourceResponse<Document>>>) (() => this.ReplaceDocumentPrivateAsync(this.GetLinkForRouting((Resource) document), document, options, retryPolicyInstance, cancellationToken)), (IRetryPolicy) retryPolicyInstance, cancellationToken);
    }

    private async Task<ResourceResponse<Document>> ReplaceDocumentPrivateAsync(
      string documentLink,
      Document document,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      await this.EnsureValidClientAsync();
      if (document == null)
        throw new ArgumentNullException(nameof (document));
      this.ValidateResource((Resource) document);
      ResourceResponse<Document> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Replace, documentLink, (Resource) document, ResourceType.Document, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options), settings: this.GetSerializerSettingsForRequest(options)))
      {
        await this.AddPartitionKeyInformationAsync(request, document, options);
        resourceResponse = new ResourceResponse<Document>(await this.UpdateAsync(request, retryPolicyInstance, cancellationToken));
      }
      return resourceResponse;
    }

    public Task<ResourceResponse<StoredProcedure>> ReplaceStoredProcedureAsync(
      StoredProcedure storedProcedure,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<StoredProcedure>>((Func<Task<ResourceResponse<StoredProcedure>>>) (() => this.ReplaceStoredProcedurePrivateAsync(storedProcedure, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<StoredProcedure>> ReplaceStoredProcedurePrivateAsync(
      StoredProcedure storedProcedure,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      string altLink = null)
    {
      await this.EnsureValidClientAsync();
      if (storedProcedure == null)
        throw new ArgumentNullException(nameof (storedProcedure));
      this.ValidateResource((Resource) storedProcedure);
      INameValueCollection requestHeaders = this.GetRequestHeaders(options);
      ResourceResponse<StoredProcedure> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Replace, altLink ?? this.GetLinkForRouting((Resource) storedProcedure), (Resource) storedProcedure, ResourceType.StoredProcedure, AuthorizationTokenType.PrimaryMasterKey, requestHeaders))
        resourceResponse = new ResourceResponse<StoredProcedure>(await this.UpdateAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<Trigger>> ReplaceTriggerAsync(
      Trigger trigger,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Trigger>>((Func<Task<ResourceResponse<Trigger>>>) (() => this.ReplaceTriggerPrivateAsync(trigger, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Trigger>> ReplaceTriggerPrivateAsync(
      Trigger trigger,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      string altLink = null)
    {
      await this.EnsureValidClientAsync();
      if (trigger == null)
        throw new ArgumentNullException(nameof (trigger));
      this.ValidateResource((Resource) trigger);
      INameValueCollection requestHeaders = this.GetRequestHeaders(options);
      ResourceResponse<Trigger> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Replace, altLink ?? this.GetLinkForRouting((Resource) trigger), (Resource) trigger, ResourceType.Trigger, AuthorizationTokenType.PrimaryMasterKey, requestHeaders))
        resourceResponse = new ResourceResponse<Trigger>(await this.UpdateAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<UserDefinedFunction>> ReplaceUserDefinedFunctionAsync(
      UserDefinedFunction function,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<UserDefinedFunction>>((Func<Task<ResourceResponse<UserDefinedFunction>>>) (() => this.ReplaceUserDefinedFunctionPrivateAsync(function, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<UserDefinedFunction>> ReplaceUserDefinedFunctionPrivateAsync(
      UserDefinedFunction function,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      string altLink = null)
    {
      await this.EnsureValidClientAsync();
      if (function == null)
        throw new ArgumentNullException(nameof (function));
      this.ValidateResource((Resource) function);
      INameValueCollection requestHeaders = this.GetRequestHeaders(options);
      ResourceResponse<UserDefinedFunction> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Replace, altLink ?? this.GetLinkForRouting((Resource) function), (Resource) function, ResourceType.UserDefinedFunction, AuthorizationTokenType.PrimaryMasterKey, requestHeaders))
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
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Replace, offer.SelfLink, (Resource) offer, ResourceType.Offer, AuthorizationTokenType.PrimaryMasterKey))
        resourceResponse = new ResourceResponse<Offer>(await this.UpdateAsync(request, retryPolicyInstance), OfferTypeResolver.ResponseOfferTypeResolver);
      return resourceResponse;
    }

    internal Task<ResourceResponse<UserDefinedType>> ReplaceUserDefinedTypeAsync(
      UserDefinedType userDefinedType,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<UserDefinedType>>((Func<Task<ResourceResponse<UserDefinedType>>>) (() => this.ReplaceUserDefinedTypePrivateAsync(userDefinedType, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<UserDefinedType>> ReplaceUserDefinedTypePrivateAsync(
      UserDefinedType userDefinedType,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      string altLink = null)
    {
      await this.EnsureValidClientAsync();
      if (userDefinedType == null)
        throw new ArgumentNullException(nameof (userDefinedType));
      this.ValidateResource((Resource) userDefinedType);
      INameValueCollection requestHeaders = this.GetRequestHeaders(options);
      ResourceResponse<UserDefinedType> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Replace, altLink ?? this.GetLinkForRouting((Resource) userDefinedType), (Resource) userDefinedType, ResourceType.UserDefinedType, AuthorizationTokenType.PrimaryMasterKey, requestHeaders))
        resourceResponse = new ResourceResponse<UserDefinedType>(await this.UpdateAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    internal Task<ResourceResponse<ClientEncryptionKey>> ReadClientEncryptionKeyAsync(
      string keyLink,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<ClientEncryptionKey>>((Func<Task<ResourceResponse<ClientEncryptionKey>>>) (() => this.ReadClientEncryptionKeyPrivateAsync(keyLink, options, retryPolicyInstance, false)), (IRetryPolicy) retryPolicyInstance);
    }

    internal Task<ResourceResponse<ClientEncryptionKey>> ReadClientEncryptionKeyAsync(
      string keyLink,
      RequestOptions options,
      bool allowCachedReads)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<ClientEncryptionKey>>((Func<Task<ResourceResponse<ClientEncryptionKey>>>) (() => this.ReadClientEncryptionKeyPrivateAsync(keyLink, options, retryPolicyInstance, allowCachedReads)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<ClientEncryptionKey>> ReadClientEncryptionKeyPrivateAsync(
      string keyLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      bool allowCachedReads)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(keyLink))
        throw new ArgumentNullException("dekLink");
      ResourceResponse<ClientEncryptionKey> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.ClientEncryptionKey, keyLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
      {
        request.Headers.Add("x-ms-cosmos-allow-cachedreads", allowCachedReads.ToString());
        resourceResponse = new ResourceResponse<ClientEncryptionKey>(await this.ReadAsync(request, retryPolicyInstance));
      }
      return resourceResponse;
    }

    public Task<ResourceResponse<Database>> ReadDatabaseAsync(
      string databaseLink,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Database>>((Func<Task<ResourceResponse<Database>>>) (() => this.ReadDatabasePrivateAsync(databaseLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Database>> ReadDatabasePrivateAsync(
      string databaseLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(databaseLink))
        throw new ArgumentNullException(nameof (databaseLink));
      ResourceResponse<Database> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.Database, databaseLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        resourceResponse = new ResourceResponse<Database>(await this.ReadAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<Document>> ReadDocumentAsync(
      string documentLink,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Document>>((Func<Task<ResourceResponse<Document>>>) (() => this.ReadDocumentPrivateAsync(documentLink, options, retryPolicyInstance, cancellationToken)), (IRetryPolicy) retryPolicyInstance, cancellationToken);
    }

    private async Task<ResourceResponse<Document>> ReadDocumentPrivateAsync(
      string documentLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(documentLink))
        throw new ArgumentNullException(nameof (documentLink));
      ResourceResponse<Document> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.Document, documentLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
      {
        await this.AddPartitionKeyInformationAsync(request, options);
        request.SerializerSettings = this.GetSerializerSettingsForRequest(options);
        resourceResponse = new ResourceResponse<Document>(await this.ReadAsync(request, retryPolicyInstance, cancellationToken));
      }
      return resourceResponse;
    }

    public Task<DocumentResponse<T>> ReadDocumentAsync<T>(
      string documentLink,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<DocumentResponse<T>>((Func<Task<DocumentResponse<T>>>) (() => this.ReadDocumentPrivateAsync<T>(documentLink, options, retryPolicyInstance, cancellationToken)), (IRetryPolicy) retryPolicyInstance, cancellationToken);
    }

    private async Task<DocumentResponse<T>> ReadDocumentPrivateAsync<T>(
      string documentLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(documentLink))
        throw new ArgumentNullException(nameof (documentLink));
      DocumentResponse<T> documentResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.Document, documentLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
      {
        await this.AddPartitionKeyInformationAsync(request, options);
        request.SerializerSettings = this.GetSerializerSettingsForRequest(options);
        documentResponse = new DocumentResponse<T>(await this.ReadAsync(request, retryPolicyInstance, cancellationToken), this.GetSerializerSettingsForRequest(options));
      }
      return documentResponse;
    }

    public Task<ResourceResponse<DocumentCollection>> ReadDocumentCollectionAsync(
      string documentCollectionLink,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<DocumentCollection>>((Func<Task<ResourceResponse<DocumentCollection>>>) (() => this.ReadDocumentCollectionPrivateAsync(documentCollectionLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<DocumentCollection>> ReadDocumentCollectionPrivateAsync(
      string documentCollectionLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(documentCollectionLink))
        throw new ArgumentNullException(nameof (documentCollectionLink));
      ResourceResponse<DocumentCollection> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.Collection, documentCollectionLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        resourceResponse = new ResourceResponse<DocumentCollection>(await this.ReadAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<StoredProcedure>> ReadStoredProcedureAsync(
      string storedProcedureLink,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<StoredProcedure>>((Func<Task<ResourceResponse<StoredProcedure>>>) (() => this._ReadStoredProcedureAsync(storedProcedureLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<StoredProcedure>> _ReadStoredProcedureAsync(
      string storedProcedureLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(storedProcedureLink))
        throw new ArgumentNullException(nameof (storedProcedureLink));
      ResourceResponse<StoredProcedure> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.StoredProcedure, storedProcedureLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        resourceResponse = new ResourceResponse<StoredProcedure>(await this.ReadAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<Trigger>> ReadTriggerAsync(
      string triggerLink,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Trigger>>((Func<Task<ResourceResponse<Trigger>>>) (() => this.ReadTriggerPrivateAsync(triggerLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Trigger>> ReadTriggerPrivateAsync(
      string triggerLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(triggerLink))
        throw new ArgumentNullException(nameof (triggerLink));
      ResourceResponse<Trigger> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.Trigger, triggerLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        resourceResponse = new ResourceResponse<Trigger>(await this.ReadAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<UserDefinedFunction>> ReadUserDefinedFunctionAsync(
      string functionLink,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<UserDefinedFunction>>((Func<Task<ResourceResponse<UserDefinedFunction>>>) (() => this.ReadUserDefinedFunctionPrivateAsync(functionLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<UserDefinedFunction>> ReadUserDefinedFunctionPrivateAsync(
      string functionLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(functionLink))
        throw new ArgumentNullException(nameof (functionLink));
      ResourceResponse<UserDefinedFunction> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.UserDefinedFunction, functionLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        resourceResponse = new ResourceResponse<UserDefinedFunction>(await this.ReadAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<Conflict>> ReadConflictAsync(
      string conflictLink,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Conflict>>((Func<Task<ResourceResponse<Conflict>>>) (() => this.ReadConflictPrivateAsync(conflictLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Conflict>> ReadConflictPrivateAsync(
      string conflictLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(conflictLink))
        throw new ArgumentNullException(nameof (conflictLink));
      ResourceResponse<Conflict> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.Conflict, conflictLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
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
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(offerLink))
        throw new ArgumentNullException(nameof (offerLink));
      ResourceResponse<Offer> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.Offer, offerLink, (Stream) null, AuthorizationTokenType.PrimaryMasterKey))
        resourceResponse = new ResourceResponse<Offer>(await this.ReadAsync(request, retryPolicyInstance), OfferTypeResolver.ResponseOfferTypeResolver);
      return resourceResponse;
    }

    internal Task<ResourceResponse<Schema>> ReadSchemaAsync(
      string documentSchemaLink,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Schema>>((Func<Task<ResourceResponse<Schema>>>) (() => this.ReadSchemaPrivateAsync(documentSchemaLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Schema>> ReadSchemaPrivateAsync(
      string documentSchemaLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(documentSchemaLink))
        throw new ArgumentNullException(nameof (documentSchemaLink));
      ResourceResponse<Schema> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.Schema, documentSchemaLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
      {
        await this.AddPartitionKeyInformationAsync(request, options);
        request.SerializerSettings = this.GetSerializerSettingsForRequest(options);
        resourceResponse = new ResourceResponse<Schema>(await this.ReadAsync(request, retryPolicyInstance));
      }
      return resourceResponse;
    }

    internal Task<ResourceResponse<UserDefinedType>> ReadUserDefinedTypeAsync(
      string userDefinedTypeLink,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<UserDefinedType>>((Func<Task<ResourceResponse<UserDefinedType>>>) (() => this.ReadUserDefinedTypePrivateAsync(userDefinedTypeLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<UserDefinedType>> ReadUserDefinedTypePrivateAsync(
      string userDefinedTypeLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(userDefinedTypeLink))
        throw new ArgumentNullException(nameof (userDefinedTypeLink));
      ResourceResponse<UserDefinedType> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.UserDefinedType, userDefinedTypeLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        resourceResponse = new ResourceResponse<UserDefinedType>(await this.ReadAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    internal Task<ResourceResponse<Snapshot>> ReadSnapshotAsync(
      string snapshotLink,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Snapshot>>((Func<Task<ResourceResponse<Snapshot>>>) (() => this.ReadSnapshotPrivateAsync(snapshotLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Snapshot>> ReadSnapshotPrivateAsync(
      string snapshotLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(snapshotLink))
        throw new ArgumentNullException(nameof (snapshotLink));
      ResourceResponse<Snapshot> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.Snapshot, snapshotLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        resourceResponse = new ResourceResponse<Snapshot>(await this.ReadAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    internal Task<FeedResponse<ClientEncryptionKey>> ReadClientEncryptionKeyFeedAsync(
      string dbLink,
      FeedOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<FeedResponse<ClientEncryptionKey>>((Func<Task<FeedResponse<ClientEncryptionKey>>>) (() => this.ReadClientEncryptionKeyFeedPrivateAsync(dbLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<FeedResponse<ClientEncryptionKey>> ReadClientEncryptionKeyFeedPrivateAsync(
      string dbLink,
      FeedOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(dbLink))
        throw new ArgumentNullException(nameof (dbLink));
      return await client.CreateClientEncryptionKeyFeedReader(dbLink, options).ExecuteNextAsync();
    }

    public Task<FeedResponse<Database>> ReadDatabaseFeedAsync(FeedOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<FeedResponse<Database>>((Func<Task<FeedResponse<Database>>>) (() => this.ReadDatabaseFeedPrivateAsync(options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<FeedResponse<Database>> ReadDatabaseFeedPrivateAsync(
      FeedOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync();
      return await client.CreateDatabaseFeedReader(options).ExecuteNextAsync();
    }

    public Task<FeedResponse<PartitionKeyRange>> ReadPartitionKeyRangeFeedAsync(
      string partitionKeyRangesOrCollectionLink,
      FeedOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<FeedResponse<PartitionKeyRange>>((Func<Task<FeedResponse<PartitionKeyRange>>>) (() => this.ReadPartitionKeyRangeFeedPrivateAsync(partitionKeyRangesOrCollectionLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<FeedResponse<PartitionKeyRange>> ReadPartitionKeyRangeFeedPrivateAsync(
      string partitionKeyRangesLink,
      FeedOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(partitionKeyRangesLink))
        throw new ArgumentNullException(nameof (partitionKeyRangesLink));
      return await client.CreatePartitionKeyRangeFeedReader(partitionKeyRangesLink, options).ExecuteNextAsync();
    }

    public Task<FeedResponse<DocumentCollection>> ReadDocumentCollectionFeedAsync(
      string collectionsLink,
      FeedOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<FeedResponse<DocumentCollection>>((Func<Task<FeedResponse<DocumentCollection>>>) (() => this.ReadDocumentCollectionFeedPrivateAsync(collectionsLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<FeedResponse<DocumentCollection>> ReadDocumentCollectionFeedPrivateAsync(
      string collectionsLink,
      FeedOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(collectionsLink))
        throw new ArgumentNullException(nameof (collectionsLink));
      return await client.CreateDocumentCollectionFeedReader(collectionsLink, options).ExecuteNextAsync();
    }

    public Task<FeedResponse<StoredProcedure>> ReadStoredProcedureFeedAsync(
      string storedProceduresLink,
      FeedOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<FeedResponse<StoredProcedure>>((Func<Task<FeedResponse<StoredProcedure>>>) (() => this.ReadStoredProcedureFeedPrivateAsync(storedProceduresLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<FeedResponse<StoredProcedure>> ReadStoredProcedureFeedPrivateAsync(
      string storedProceduresLink,
      FeedOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(storedProceduresLink))
        throw new ArgumentNullException(nameof (storedProceduresLink));
      return await client.CreateStoredProcedureFeedReader(storedProceduresLink, options).ExecuteNextAsync();
    }

    public Task<FeedResponse<Trigger>> ReadTriggerFeedAsync(
      string triggersLink,
      FeedOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<FeedResponse<Trigger>>((Func<Task<FeedResponse<Trigger>>>) (() => this.ReadTriggerFeedPrivateAsync(triggersLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<FeedResponse<Trigger>> ReadTriggerFeedPrivateAsync(
      string triggersLink,
      FeedOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(triggersLink))
        throw new ArgumentNullException(nameof (triggersLink));
      return await client.CreateTriggerFeedReader(triggersLink, options).ExecuteNextAsync();
    }

    public Task<FeedResponse<UserDefinedFunction>> ReadUserDefinedFunctionFeedAsync(
      string userDefinedFunctionsLink,
      FeedOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<FeedResponse<UserDefinedFunction>>((Func<Task<FeedResponse<UserDefinedFunction>>>) (() => this.ReadUserDefinedFunctionFeedPrivateAsync(userDefinedFunctionsLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<FeedResponse<UserDefinedFunction>> ReadUserDefinedFunctionFeedPrivateAsync(
      string userDefinedFunctionsLink,
      FeedOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(userDefinedFunctionsLink))
        throw new ArgumentNullException(nameof (userDefinedFunctionsLink));
      return await client.CreateUserDefinedFunctionFeedReader(userDefinedFunctionsLink, options).ExecuteNextAsync();
    }

    public Task<FeedResponse<object>> ReadDocumentFeedAsync(
      string documentsLink,
      FeedOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return TaskHelper.InlineIfPossible<FeedResponse<object>>((Func<Task<FeedResponse<object>>>) (() => this.ReadDocumentFeedInlineAsync(documentsLink, options, cancellationToken)), (IRetryPolicy) null, cancellationToken);
    }

    private async Task<FeedResponse<object>> ReadDocumentFeedInlineAsync(
      string documentsLink,
      FeedOptions options,
      CancellationToken cancellationToken)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(documentsLink))
        throw new ArgumentNullException(nameof (documentsLink));
      FeedResponse<Document> source = await client.CreateDocumentFeedReader(documentsLink, options).ExecuteNextAsync(cancellationToken);
      return new FeedResponse<object>(source.Cast<object>(), source.Count, source.Headers, source.UseETagAsContinuation, source.QueryMetrics, source.PartitionedClientSideRequestStatistics, responseLengthBytes: source.ResponseLengthBytes);
    }

    public Task<FeedResponse<Conflict>> ReadConflictFeedAsync(
      string conflictsLink,
      FeedOptions options = null)
    {
      return TaskHelper.InlineIfPossible<FeedResponse<Conflict>>((Func<Task<FeedResponse<Conflict>>>) (() => this.ReadConflictFeedInlineAsync(conflictsLink, options)), (IRetryPolicy) null);
    }

    private async Task<FeedResponse<Conflict>> ReadConflictFeedInlineAsync(
      string conflictsLink,
      FeedOptions options)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(conflictsLink))
        throw new ArgumentNullException(nameof (conflictsLink));
      return await client.CreateConflictFeedReader(conflictsLink, options).ExecuteNextAsync();
    }

    public Task<FeedResponse<Offer>> ReadOffersFeedAsync(FeedOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<FeedResponse<Offer>>((Func<Task<FeedResponse<Offer>>>) (() => this.ReadOfferFeedPrivateAsync(options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<FeedResponse<Offer>> ReadOfferFeedPrivateAsync(
      FeedOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync();
      return await client.CreateOfferFeedReader(options).ExecuteNextAsync();
    }

    internal Task<FeedResponse<Schema>> ReadSchemaFeedAsync(
      string documentCollectionSchemaLink,
      FeedOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<FeedResponse<Schema>>((Func<Task<FeedResponse<Schema>>>) (() => this.ReadSchemaFeedPrivateAsync(documentCollectionSchemaLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<FeedResponse<Schema>> ReadSchemaFeedPrivateAsync(
      string documentCollectionSchemaLink,
      FeedOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(documentCollectionSchemaLink))
        throw new ArgumentNullException(nameof (documentCollectionSchemaLink));
      return await client.CreateSchemaFeedReader(documentCollectionSchemaLink, options).ExecuteNextAsync();
    }

    internal Task<FeedResponse<UserDefinedType>> ReadUserDefinedTypeFeedAsync(
      string userDefinedTypesLink,
      FeedOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<FeedResponse<UserDefinedType>>((Func<Task<FeedResponse<UserDefinedType>>>) (() => this.ReadUserDefinedTypeFeedPrivateAsync(userDefinedTypesLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<FeedResponse<UserDefinedType>> ReadUserDefinedTypeFeedPrivateAsync(
      string userDefinedTypesLink,
      FeedOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(userDefinedTypesLink))
        throw new ArgumentNullException(nameof (userDefinedTypesLink));
      return await client.CreateUserDefinedTypeFeedReader(userDefinedTypesLink, options).ExecuteNextAsync();
    }

    internal Task<FeedResponse<Snapshot>> ReadSnapshotFeedAsync(FeedOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<FeedResponse<Snapshot>>((Func<Task<FeedResponse<Snapshot>>>) (() => this.ReadSnapshotFeedPrivateAsync(options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<FeedResponse<Snapshot>> ReadSnapshotFeedPrivateAsync(
      FeedOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync();
      return await client.CreateSnapshotFeedReader(options).ExecuteNextAsync();
    }

    public Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(
      string storedProcedureLink,
      params object[] procedureParams)
    {
      return this.ExecuteStoredProcedureAsync<TValue>(storedProcedureLink, (RequestOptions) null, new CancellationToken(), procedureParams);
    }

    public Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(
      string storedProcedureLink,
      RequestOptions options,
      params object[] procedureParams)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<StoredProcedureResponse<TValue>>((Func<Task<StoredProcedureResponse<TValue>>>) (() => this.ExecuteStoredProcedurePrivateAsync<TValue>(storedProcedureLink, options, retryPolicyInstance, new CancellationToken(), procedureParams)), (IRetryPolicy) retryPolicyInstance);
    }

    public Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(
      string storedProcedureLink,
      RequestOptions options,
      CancellationToken cancellationToken,
      params object[] procedureParams)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<StoredProcedureResponse<TValue>>((Func<Task<StoredProcedureResponse<TValue>>>) (() => this.ExecuteStoredProcedurePrivateAsync<TValue>(storedProcedureLink, options, retryPolicyInstance, cancellationToken, procedureParams)), (IRetryPolicy) retryPolicyInstance, cancellationToken);
    }

    private async Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedurePrivateAsync<TValue>(
      string storedProcedureLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken,
      params object[] procedureParams)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(storedProcedureLink))
        throw new ArgumentNullException(nameof (storedProcedureLink));
      JsonSerializerSettings settingsForRequest = this.GetSerializerSettingsForRequest(options);
      string str = settingsForRequest == null ? JsonConvert.SerializeObject((object) procedureParams) : JsonConvert.SerializeObject((object) procedureParams, settingsForRequest);
      StoredProcedureResponse<TValue> procedureResponse;
      using (MemoryStream storedProcedureInputStream = new MemoryStream())
      {
        using (StreamWriter writer = new StreamWriter((Stream) storedProcedureInputStream))
        {
          writer.Write(str);
          writer.Flush();
          storedProcedureInputStream.Position = 0L;
          using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.ExecuteJavaScript, ResourceType.StoredProcedure, storedProcedureLink, (Stream) storedProcedureInputStream, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
          {
            request.Headers["x-ms-date"] = DateTime.UtcNow.ToString("r");
            if (options == null || options.PartitionKeyRangeId == null)
              await this.AddPartitionKeyInformationAsync(request, options);
            retryPolicyInstance?.OnBeforeSendRequest(request);
            request.SerializerSettings = this.GetSerializerSettingsForRequest(options);
            procedureResponse = new StoredProcedureResponse<TValue>(await this.ExecuteProcedureAsync(request, retryPolicyInstance, cancellationToken), this.GetSerializerSettingsForRequest(options));
          }
        }
      }
      return procedureResponse;
    }

    internal Task<ResourceResponse<Database>> UpsertDatabaseAsync(
      Database database,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Database>>((Func<Task<ResourceResponse<Database>>>) (() => this.UpsertDatabasePrivateAsync(database, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Database>> UpsertDatabasePrivateAsync(
      Database database,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (database == null)
        throw new ArgumentNullException(nameof (database));
      this.ValidateResource((Resource) database);
      ResourceResponse<Database> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Upsert, "//dbs/", (Resource) database, ResourceType.Database, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        resourceResponse = new ResourceResponse<Database>(await this.UpsertAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<Document>> UpsertDocumentAsync(
      string documentsFeedOrDatabaseLink,
      object document,
      RequestOptions options = null,
      bool disableAutomaticIdGeneration = false,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return TaskHelper.InlineIfPossible<ResourceResponse<Document>>((Func<Task<ResourceResponse<Document>>>) (() => this.UpsertDocumentInlineAsync(documentsFeedOrDatabaseLink, document, options, disableAutomaticIdGeneration, cancellationToken)), (IRetryPolicy) null, cancellationToken);
    }

    private async Task<ResourceResponse<Document>> UpsertDocumentInlineAsync(
      string documentsFeedOrDatabaseLink,
      object document,
      RequestOptions options,
      bool disableAutomaticIdGeneration,
      CancellationToken cancellationToken)
    {
      IPartitionResolver partitionResolver = (IPartitionResolver) null;
      IDocumentClientRetryPolicy requestRetryPolicy = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      if (this.PartitionResolvers.TryGetValue(documentsFeedOrDatabaseLink, out partitionResolver))
      {
        object partitionKey = partitionResolver.GetPartitionKey(document);
        string collectionLink = partitionResolver.ResolveForCreate(partitionKey);
        return await TaskHelper.InlineIfPossible<ResourceResponse<Document>>((Func<Task<ResourceResponse<Document>>>) (() => this.UpsertDocumentPrivateAsync(collectionLink, document, options, disableAutomaticIdGeneration, requestRetryPolicy, cancellationToken)), (IRetryPolicy) requestRetryPolicy, cancellationToken);
      }
      if (options == null || options.PartitionKey == null)
        requestRetryPolicy = (IDocumentClientRetryPolicy) new PartitionKeyMismatchRetryPolicy((CollectionCache) await this.GetCollectionCacheAsync(), requestRetryPolicy);
      return await TaskHelper.InlineIfPossible<ResourceResponse<Document>>((Func<Task<ResourceResponse<Document>>>) (() => this.UpsertDocumentPrivateAsync(documentsFeedOrDatabaseLink, document, options, disableAutomaticIdGeneration, requestRetryPolicy, cancellationToken)), (IRetryPolicy) requestRetryPolicy, cancellationToken);
    }

    private async Task<ResourceResponse<Document>> UpsertDocumentPrivateAsync(
      string documentCollectionLink,
      object document,
      RequestOptions options,
      bool disableAutomaticIdGeneration,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(documentCollectionLink))
        throw new ArgumentNullException(nameof (documentCollectionLink));
      if (document == null)
        throw new ArgumentNullException(nameof (document));
      INameValueCollection requestHeaders = this.GetRequestHeaders(options);
      Document document1 = Document.FromObject(document, this.GetSerializerSettingsForRequest(options));
      this.ValidateResource((Resource) document1);
      if (string.IsNullOrEmpty(document1.Id) && !disableAutomaticIdGeneration)
        document1.Id = Guid.NewGuid().ToString();
      ResourceResponse<Document> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Upsert, documentCollectionLink, (Resource) document1, ResourceType.Document, AuthorizationTokenType.PrimaryMasterKey, requestHeaders, settings: this.GetSerializerSettingsForRequest(options)))
      {
        await this.AddPartitionKeyInformationAsync(request, document1, options);
        resourceResponse = new ResourceResponse<Document>(await this.UpsertAsync(request, retryPolicyInstance, cancellationToken));
      }
      return resourceResponse;
    }

    internal Task<ResourceResponse<DocumentCollection>> UpsertDocumentCollectionAsync(
      string databaseLink,
      DocumentCollection documentCollection,
      RequestOptions options = null)
    {
      throw new NotImplementedException();
    }

    public Task<ResourceResponse<StoredProcedure>> UpsertStoredProcedureAsync(
      string collectionLink,
      StoredProcedure storedProcedure,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<StoredProcedure>>((Func<Task<ResourceResponse<StoredProcedure>>>) (() => this.UpsertStoredProcedurePrivateAsync(collectionLink, storedProcedure, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<StoredProcedure>> UpsertStoredProcedurePrivateAsync(
      string collectionLink,
      StoredProcedure storedProcedure,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(collectionLink))
        throw new ArgumentNullException(nameof (collectionLink));
      if (storedProcedure == null)
        throw new ArgumentNullException(nameof (storedProcedure));
      this.ValidateResource((Resource) storedProcedure);
      ResourceResponse<StoredProcedure> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Upsert, collectionLink, (Resource) storedProcedure, ResourceType.StoredProcedure, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        resourceResponse = new ResourceResponse<StoredProcedure>(await this.UpsertAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<Trigger>> UpsertTriggerAsync(
      string collectionLink,
      Trigger trigger,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Trigger>>((Func<Task<ResourceResponse<Trigger>>>) (() => this.UpsertTriggerPrivateAsync(collectionLink, trigger, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Trigger>> UpsertTriggerPrivateAsync(
      string collectionLink,
      Trigger trigger,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(collectionLink))
        throw new ArgumentNullException(nameof (collectionLink));
      if (trigger == null)
        throw new ArgumentNullException(nameof (trigger));
      this.ValidateResource((Resource) trigger);
      ResourceResponse<Trigger> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Upsert, collectionLink, (Resource) trigger, ResourceType.Trigger, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        resourceResponse = new ResourceResponse<Trigger>(await this.UpsertAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<UserDefinedFunction>> UpsertUserDefinedFunctionAsync(
      string collectionLink,
      UserDefinedFunction function,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<UserDefinedFunction>>((Func<Task<ResourceResponse<UserDefinedFunction>>>) (() => this.UpsertUserDefinedFunctionPrivateAsync(collectionLink, function, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<UserDefinedFunction>> UpsertUserDefinedFunctionPrivateAsync(
      string collectionLink,
      UserDefinedFunction function,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(collectionLink))
        throw new ArgumentNullException(nameof (collectionLink));
      if (function == null)
        throw new ArgumentNullException(nameof (function));
      this.ValidateResource((Resource) function);
      ResourceResponse<UserDefinedFunction> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Upsert, collectionLink, (Resource) function, ResourceType.UserDefinedFunction, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        resourceResponse = new ResourceResponse<UserDefinedFunction>(await this.UpsertAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    internal Task<ResourceResponse<UserDefinedType>> UpsertUserDefinedTypeAsync(
      string databaseLink,
      UserDefinedType userDefinedType,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<UserDefinedType>>((Func<Task<ResourceResponse<UserDefinedType>>>) (() => this.UpsertUserDefinedTypePrivateAsync(databaseLink, userDefinedType, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<UserDefinedType>> UpsertUserDefinedTypePrivateAsync(
      string databaseLink,
      UserDefinedType userDefinedType,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(databaseLink))
        throw new ArgumentNullException(nameof (databaseLink));
      if (userDefinedType == null)
        throw new ArgumentNullException(nameof (userDefinedType));
      this.ValidateResource((Resource) userDefinedType);
      ResourceResponse<UserDefinedType> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Upsert, databaseLink, (Resource) userDefinedType, ResourceType.UserDefinedType, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        resourceResponse = new ResourceResponse<UserDefinedType>(await this.UpsertAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    private bool TryGetResourceToken(
      string resourceAddress,
      PartitionKeyInternal partitionKey,
      out string resourceToken)
    {
      resourceToken = (string) null;
      List<PartitionKeyAndResourceTokenPair> source;
      if (this.resourceTokens.TryGetValue(resourceAddress, out source))
      {
        PartitionKeyAndResourceTokenPair resourceTokenPair = source.FirstOrDefault<PartitionKeyAndResourceTokenPair>((Func<PartitionKeyAndResourceTokenPair, bool>) (pair => pair.PartitionKey.Contains(partitionKey)));
        if (resourceTokenPair != null)
        {
          resourceToken = resourceTokenPair.ResourceToken;
          return true;
        }
      }
      return false;
    }

    string IAuthorizationTokenProvider.GetUserAuthorizationToken(
      string resourceAddress,
      string resourceType,
      string requestVerb,
      INameValueCollection headers,
      AuthorizationTokenType tokenType,
      out string payload)
    {
      payload = (string) null;
      if (this.hasAuthKeyResourceToken && this.resourceTokens == null)
        return HttpUtility.UrlEncode(this.authKeyResourceToken);
      if (this.authKeyHashFunction != null)
      {
        headers["x-ms-date"] = DateTime.UtcNow.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture);
        return AuthorizationHelper.GenerateKeyAuthorizationSignature(requestVerb, resourceAddress, resourceType, headers, this.authKeyHashFunction, out payload);
      }
      PartitionKeyInternal partitionKey = PartitionKeyInternal.Empty;
      string header = headers["x-ms-documentdb-partitionkey"];
      if (header != null)
        partitionKey = PartitionKeyInternal.FromJsonString(header);
      if (PathsHelper.IsNameBased(resourceAddress))
      {
        string resourceToken1 = (string) null;
        bool flag = false;
        for (int segmentIndex = 2; segmentIndex < (int) ResourceId.MaxPathFragment; segmentIndex += 2)
        {
          string parentByIndex = PathsHelper.GetParentByIndex(resourceAddress, segmentIndex);
          if (parentByIndex != null)
          {
            flag = this.TryGetResourceToken(parentByIndex, partitionKey, out resourceToken1);
            if (flag)
              break;
          }
          else
            break;
        }
        if (!flag && PathsHelper.GetCollectionPath(resourceAddress) == resourceAddress && (requestVerb == "GET" || requestVerb == "HEAD"))
        {
          string str = resourceAddress.EndsWith("/", StringComparison.Ordinal) ? resourceAddress : resourceAddress + "/";
          foreach (KeyValuePair<string, List<PartitionKeyAndResourceTokenPair>> resourceToken2 in (IEnumerable<KeyValuePair<string, List<PartitionKeyAndResourceTokenPair>>>) this.resourceTokens)
          {
            if (resourceToken2.Key.StartsWith(str, StringComparison.Ordinal))
            {
              resourceToken1 = resourceToken2.Value[0].ResourceToken;
              flag = true;
              break;
            }
          }
        }
        if (!flag)
          throw new UnauthorizedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ClientResources.AuthTokenNotFound, (object) resourceAddress));
        return HttpUtility.UrlEncode(resourceToken1);
      }
      string resourceToken3 = (string) null;
      ResourceId other = ResourceId.Parse(resourceAddress);
      bool flag1 = false;
      if (other.Attachment != 0U || other.Permission != 0UL || other.StoredProcedure != 0UL || other.Trigger != 0UL || other.UserDefinedFunction != 0UL)
        flag1 = this.TryGetResourceToken(resourceAddress, partitionKey, out resourceToken3);
      if (!flag1 && (other.Attachment != 0U || other.Document != 0UL))
        flag1 = this.TryGetResourceToken(other.DocumentId.ToString(), partitionKey, out resourceToken3);
      if (!flag1 && (other.Attachment != 0U || other.Document != 0UL || other.StoredProcedure != 0UL || other.Trigger != 0UL || other.UserDefinedFunction != 0UL || other.DocumentCollection != 0U))
        flag1 = this.TryGetResourceToken(other.DocumentCollectionId.ToString(), partitionKey, out resourceToken3);
      if (!flag1 && (other.Permission != 0UL || other.User != 0U))
        flag1 = this.TryGetResourceToken(other.UserId.ToString(), partitionKey, out resourceToken3);
      if (!flag1)
        flag1 = this.TryGetResourceToken(other.DatabaseId.ToString(), partitionKey, out resourceToken3);
      if (!flag1 && other.DocumentCollection != 0U && (requestVerb == "GET" || requestVerb == "HEAD"))
      {
        foreach (KeyValuePair<string, List<PartitionKeyAndResourceTokenPair>> resourceToken4 in (IEnumerable<KeyValuePair<string, List<PartitionKeyAndResourceTokenPair>>>) this.resourceTokens)
        {
          ResourceId rid;
          if (!PathsHelper.IsNameBased(resourceToken4.Key) && ResourceId.TryParse(resourceToken4.Key, out rid) && rid.DocumentCollectionId.Equals(other))
          {
            resourceToken3 = resourceToken4.Value[0].ResourceToken;
            flag1 = true;
            break;
          }
        }
      }
      if (!flag1)
        throw new UnauthorizedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, ClientResources.AuthTokenNotFound, (object) resourceAddress));
      return HttpUtility.UrlEncode(resourceToken3);
    }

    Task IAuthorizationTokenProvider.AddSystemAuthorizationHeaderAsync(
      DocumentServiceRequest request,
      string federationId,
      string verb,
      string resourceId)
    {
      request.Headers["x-ms-date"] = DateTime.UtcNow.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture);
      request.Headers["authorization"] = ((IAuthorizationTokenProvider) this).GetUserAuthorizationToken(resourceId ?? request.ResourceAddress, PathsHelper.GetResourcePath(request.ResourceType), verb, request.Headers, request.RequestAuthorizationTokenType, out string _);
      return (Task) Task.FromResult<int>(0);
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

    public Task<DatabaseAccount> GetDatabaseAccountAsync() => TaskHelper.InlineIfPossible<DatabaseAccount>((Func<Task<DatabaseAccount>>) (() => this.GetDatabaseAccountPrivateAsync(this.ReadEndpoint)), (IRetryPolicy) this.ResetSessionTokenRetryPolicy.GetRequestPolicy());

    Task<DatabaseAccount> IDocumentClientInternal.GetDatabaseAccountInternalAsync(
      Uri serviceEndpoint,
      CancellationToken cancellationToken)
    {
      return this.GetDatabaseAccountPrivateAsync(serviceEndpoint, cancellationToken);
    }

    private async Task<DatabaseAccount> GetDatabaseAccountPrivateAsync(
      Uri serviceEndpoint,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      await this.EnsureValidClientAsync();
      if (!(this.gatewayStoreModel is Microsoft.Azure.Documents.GatewayStoreModel gatewayStoreModel))
        return (DatabaseAccount) null;
      using (HttpRequestMessage request = new HttpRequestMessage())
      {
        INameValueCollection headers = (INameValueCollection) new DictionaryNameValueCollection();
        string str = DateTime.UtcNow.ToString("r");
        headers.Add("x-ms-date", str);
        request.Headers.Add("x-ms-date", str);
        request.Headers.Add("authorization", !this.hasAuthKeyResourceToken ? AuthorizationHelper.GenerateKeyAuthorizationSignature("GET", serviceEndpoint, headers, this.authKeyHashFunction) : HttpUtility.UrlEncode(this.authKeyResourceToken));
        request.Method = HttpMethod.Get;
        request.RequestUri = serviceEndpoint;
        DatabaseAccount databaseAccountAsync = await gatewayStoreModel.GetDatabaseAccountAsync(request, cancellationToken);
        this.useMultipleWriteLocations = this.connectionPolicy.UseMultipleWriteLocations && databaseAccountAsync.EnableMultipleWriteLocations;
        return databaseAccountAsync;
      }
    }

    private IStoreModel GetStoreProxy(DocumentServiceRequest request)
    {
      if (request.UseGatewayMode)
        return this.gatewayStoreModel;
      ResourceType resourceType = request.ResourceType;
      Microsoft.Azure.Documents.OperationType operationType = request.OperationType;
      if (resourceType == ResourceType.Offer || resourceType.IsScript() && operationType != Microsoft.Azure.Documents.OperationType.ExecuteJavaScript || resourceType == ResourceType.PartitionKeyRange || resourceType == ResourceType.Snapshot || resourceType == ResourceType.ClientEncryptionKey)
        return this.gatewayStoreModel;
      switch (operationType)
      {
        case Microsoft.Azure.Documents.OperationType.Create:
        case Microsoft.Azure.Documents.OperationType.Upsert:
          return resourceType == ResourceType.Database || resourceType == ResourceType.User || resourceType == ResourceType.Collection || resourceType == ResourceType.Permission ? this.gatewayStoreModel : this.storeModel;
        case Microsoft.Azure.Documents.OperationType.Read:
          return resourceType == ResourceType.Collection ? this.gatewayStoreModel : this.storeModel;
        case Microsoft.Azure.Documents.OperationType.Delete:
          return resourceType == ResourceType.Database || resourceType == ResourceType.User || resourceType == ResourceType.Collection ? this.gatewayStoreModel : this.storeModel;
        case Microsoft.Azure.Documents.OperationType.Replace:
          return resourceType == ResourceType.Collection ? this.gatewayStoreModel : this.storeModel;
        default:
          return this.storeModel;
      }
    }

    private string GetLinkForRouting(Resource resource) => resource.SelfLink ?? resource.AltLink;

    internal void EnsureValidOverwrite(ConsistencyLevel desiredConsistencyLevel)
    {
      ConsistencyLevel consistencyLevel = this.gatewayConfigurationReader.DefaultConsistencyLevel;
      if (!this.IsValidConsistency(consistencyLevel, desiredConsistencyLevel))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidConsistencyLevel, (object) desiredConsistencyLevel.ToString(), (object) consistencyLevel.ToString()));
    }

    private bool IsValidConsistency(
      ConsistencyLevel backendConsistency,
      ConsistencyLevel desiredConsistency)
    {
      return this.allowOverrideStrongerConsistency || ValidationHelpers.ValidateConsistencyLevel(backendConsistency, desiredConsistency);
    }

    private void InitializeDirectConnectivity(IStoreClientFactory storeClientFactory)
    {
      this.AddressResolver = new GlobalAddressResolver(this.globalEndpointManager, this.connectionPolicy.ConnectionProtocol, (IAuthorizationTokenProvider) this, (CollectionCache) this.collectionCache, this.partitionKeyRangeCache, this.connectionPolicy.UserAgentContainer, (IServiceConfigurationReader) this.gatewayConfigurationReader, this.httpMessageHandler, this.connectionPolicy, this.ApiType);
      if (storeClientFactory != null)
      {
        this.storeClientFactory = storeClientFactory;
        this.isStoreClientFactoryCreatedInternally = false;
      }
      else
      {
        this.storeClientFactory = (IStoreClientFactory) new StoreClientFactory(this.connectionPolicy.ConnectionProtocol, (int) this.connectionPolicy.RequestTimeout.TotalSeconds, this.maxConcurrentConnectionOpenRequests, this.connectionPolicy.UserAgentContainer, (ICommunicationEventSource) this.eventSource, openTimeoutInSeconds: this.openConnectionTimeoutInSeconds, idleTimeoutInSeconds: this.idleConnectionTimeoutInSeconds, timerPoolGranularityInSeconds: this.timerPoolGranularityInSeconds, maxRntbdChannels: this.maxRntbdChannels, rntbdPartitionCount: this.rntbdPartitionCount, maxRequestsPerRntbdChannel: this.maxRequestsPerRntbdChannel, rntbdPortReuseMode: this.rntbdPortReuseMode, rntbdPortPoolReuseThreshold: this.rntbdPortPoolReuseThreshold, rntbdPortPoolBindAttempts: this.rntbdPortPoolBindAttempts, receiveHangDetectionTimeSeconds: this.rntbdReceiveHangDetectionTimeSeconds, sendHangDetectionTimeSeconds: this.rntbdSendHangDetectionTimeSeconds, enableCpuMonitor: this.enableCpuMonitor, retryWithConfiguration: this.connectionPolicy.GetRetryWithConfiguration(), enableTcpConnectionEndpointRediscovery: this.connectionPolicy.EnableTcpConnectionEndpointRediscovery, addressResolver: (IAddressResolverExtension) this.AddressResolver);
        this.isStoreClientFactoryCreatedInternally = true;
      }
      this.CreateStoreModel(true);
    }

    private void CreateStoreModel(bool subscribeRntbdStatus)
    {
      StoreClient storeClient = this.storeClientFactory.CreateStoreClient((IAddressResolver) this.AddressResolver, this.sessionContainer, (IServiceConfigurationReader) this.gatewayConfigurationReader, (IAuthorizationTokenProvider) this, true, ((int) this.connectionPolicy.EnableReadRequestsFallback ?? (this.gatewayConfigurationReader.DefaultConsistencyLevel != ConsistencyLevel.BoundedStaleness ? 1 : 0)) != 0, !this.enableRntbdChannel, this.useMultipleWriteLocations && this.gatewayConfigurationReader.DefaultConsistencyLevel != 0, true);
      if (subscribeRntbdStatus)
        storeClient.AddDisableRntbdChannelCallback(new Action(this.DisableRntbdChannel));
      storeClient.SerializerSettings = this.serializerSettings;
      this.storeModel = (IStoreModel) new ServerStoreModel(storeClient, this.sendingRequest, this.receivedResponse);
    }

    private void DisableRntbdChannel()
    {
      this.enableRntbdChannel = false;
      this.CreateStoreModel(false);
    }

    private async Task InitializeGatewayConfigurationReader()
    {
      this.gatewayConfigurationReader = new GatewayServiceConfigurationReader(this.ServiceEndpoint, this.authKeyHashFunction, this.hasAuthKeyResourceToken, this.authKeyResourceToken, this.connectionPolicy, this.ApiType, this.httpMessageHandler);
      DatabaseAccount databaseAccount = await this.gatewayConfigurationReader.InitializeReaderAsync();
      this.useMultipleWriteLocations = this.connectionPolicy.UseMultipleWriteLocations && databaseAccount.EnableMultipleWriteLocations;
      await this.globalEndpointManager.RefreshLocationAsync(databaseAccount);
    }

    private void CaptureSessionToken(
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

    private void ValidateResource(Resource resource)
    {
      if (string.IsNullOrEmpty(resource.Id))
        return;
      int index = resource.Id.IndexOfAny(new char[4]
      {
        '/',
        '\\',
        '?',
        '#'
      });
      if (index != -1)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidCharacterInResourceName, (object) resource.Id[index]));
      if (resource.Id[resource.Id.Length - 1] == ' ')
        throw new ArgumentException(RMResources.InvalidSpaceEndingInResourceName);
    }

    private async Task AddPartitionKeyInformationAsync(
      DocumentServiceRequest request,
      Document document,
      RequestOptions options)
    {
      DocumentCollection documentCollection = await (await this.GetCollectionCacheAsync()).ResolveCollectionAsync(request, CancellationToken.None);
      PartitionKeyDefinition partitionKey = documentCollection.PartitionKey;
      request.Headers.Set("x-ms-documentdb-partitionkey", (options == null || options.PartitionKey == null || !options.PartitionKey.Equals((object) PartitionKey.None) ? (options == null || options.PartitionKey == null ? DocumentAnalyzer.ExtractPartitionKeyValue(document, partitionKey) : options.PartitionKey.InternalKey) : documentCollection.NonePartitionKeyValue).ToJsonString());
    }

    internal async Task AddPartitionKeyInformationAsync(
      DocumentServiceRequest request,
      RequestOptions options)
    {
      DocumentCollection documentCollection = await (await this.GetCollectionCacheAsync()).ResolveCollectionAsync(request, CancellationToken.None);
      PartitionKeyDefinition partitionKey = documentCollection.PartitionKey;
      PartitionKeyInternal partitionKeyInternal;
      if (options == null || options.PartitionKey == null)
      {
        if (partitionKey != null && partitionKey.Paths.Count != 0)
          throw new InvalidOperationException(RMResources.MissingPartitionKeyValue);
        partitionKeyInternal = PartitionKeyInternal.Empty;
      }
      else
        partitionKeyInternal = !options.PartitionKey.Equals((object) PartitionKey.None) ? options.PartitionKey.InternalKey : documentCollection.NonePartitionKeyValue;
      request.Headers.Set("x-ms-documentdb-partitionkey", partitionKeyInternal.ToJsonString());
    }

    private JsonSerializerSettings GetSerializerSettingsForRequest(RequestOptions requestOptions) => requestOptions?.JsonSerializerSettings ?? this.serializerSettings;

    private INameValueCollection GetRequestHeaders(RequestOptions options)
    {
      INameValueCollection requestHeaders = (INameValueCollection) new DictionaryNameValueCollection();
      if (this.useMultipleWriteLocations)
        requestHeaders.Set("x-ms-cosmos-allow-tentative-writes", bool.TrueString);
      if (this.desiredConsistencyLevel.HasValue)
      {
        if (!this.IsValidConsistency(this.gatewayConfigurationReader.DefaultConsistencyLevel, this.desiredConsistencyLevel.Value))
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidConsistencyLevel, (object) this.desiredConsistencyLevel.Value.ToString(), (object) this.gatewayConfigurationReader.DefaultConsistencyLevel));
        requestHeaders.Set("x-ms-consistency-level", this.desiredConsistencyLevel.Value.ToString());
      }
      if (options == null)
        return requestHeaders;
      if (options.AccessCondition != null)
      {
        if (options.AccessCondition.Type == AccessConditionType.IfMatch)
          requestHeaders.Set("If-Match", options.AccessCondition.Condition);
        else
          requestHeaders.Set("If-None-Match", options.AccessCondition.Condition);
      }
      if (options.ConsistencyLevel.HasValue)
      {
        if (!this.IsValidConsistency(this.gatewayConfigurationReader.DefaultConsistencyLevel, options.ConsistencyLevel.Value))
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidConsistencyLevel, (object) options.ConsistencyLevel.Value.ToString(), (object) this.gatewayConfigurationReader.DefaultConsistencyLevel));
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
        INameValueCollection nameValueCollection = requestHeaders;
        num = options.ResourceTokenExpirySeconds.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        nameValueCollection.Set("x-ms-documentdb-expiry-seconds", str);
      }
      if (options.OfferType != null)
        requestHeaders.Set("x-ms-offer-type", options.OfferType);
      if (options.OfferThroughput.HasValue)
      {
        INameValueCollection nameValueCollection = requestHeaders;
        num = options.OfferThroughput.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        nameValueCollection.Set("x-ms-offer-throughput", str);
      }
      if (options.OfferEnableRUPerMinuteThroughput)
        requestHeaders.Set("x-ms-offer-is-ru-per-minute-throughput-enabled", bool.TrueString);
      if (options.InsertSystemPartitionKey)
        requestHeaders.Set("x-ms-cosmos-insert-systempartitionkey", bool.TrueString);
      if (options.OfferAutopilotTier.HasValue)
        requestHeaders.Set("x-ms-cosmos-offer-autopilot-tier", options.OfferAutopilotTier.ToString());
      if (options.OfferAutopilotAutoUpgrade.HasValue)
        requestHeaders.Set("x-ms-cosmos-offer-autopilot-autoupgrade", options.OfferAutopilotAutoUpgrade.ToString());
      if (options.OfferAutopilotSettings != null)
        requestHeaders.Set("x-ms-cosmos-offer-autopilot-settings", options.OfferAutopilotSettings.ToString());
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
      if (options.ForceSideBySideIndexMigration)
        requestHeaders.Set("x-ms-cosmos-force-sidebyside-indexmigration", bool.TrueString);
      return requestHeaders;
    }

    public Task<ResourceResponse<Attachment>> CreateAttachmentAsync(
      string documentLink,
      object attachment,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Attachment>>((Func<Task<ResourceResponse<Attachment>>>) (() => this.CreateAttachmentPrivateAsync(documentLink, attachment, options, retryPolicyInstance, cancellationToken)), (IRetryPolicy) retryPolicyInstance, cancellationToken);
    }

    private async Task<ResourceResponse<Attachment>> CreateAttachmentPrivateAsync(
      string documentLink,
      object attachment,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(documentLink))
        throw new ArgumentNullException(nameof (documentLink));
      if (attachment == null)
        throw new ArgumentNullException(nameof (attachment));
      INameValueCollection requestHeaders = this.GetRequestHeaders(options);
      Attachment attachment1 = Attachment.FromObject(attachment);
      this.ValidateResource((Resource) attachment1);
      ResourceResponse<Attachment> attachmentPrivateAsync;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Create, documentLink, (Resource) attachment1, ResourceType.Attachment, AuthorizationTokenType.PrimaryMasterKey, requestHeaders))
      {
        if (!request.IsValidAddress(ResourceType.Document))
          throw new ArgumentException(RMResources.BadUrl, "link");
        await this.AddPartitionKeyInformationAsync(request, options);
        attachmentPrivateAsync = new ResourceResponse<Attachment>(await this.CreateAsync(request, retryPolicyInstance, cancellationToken));
      }
      return attachmentPrivateAsync;
    }

    public Task<ResourceResponse<Attachment>> DeleteAttachmentAsync(
      string attachmentLink,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Attachment>>((Func<Task<ResourceResponse<Attachment>>>) (() => this.DeleteAttachmentPrivateAsync(attachmentLink, options, retryPolicyInstance, cancellationToken)), (IRetryPolicy) retryPolicyInstance, cancellationToken);
    }

    private async Task<ResourceResponse<Attachment>> DeleteAttachmentPrivateAsync(
      string attachmentLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(attachmentLink))
        throw new ArgumentNullException(nameof (attachmentLink));
      ResourceResponse<Attachment> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Delete, ResourceType.Attachment, attachmentLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
      {
        retryPolicyInstance?.OnBeforeSendRequest(request);
        await this.AddPartitionKeyInformationAsync(request, options);
        resourceResponse = new ResourceResponse<Attachment>(await this.DeleteAsync(request, retryPolicyInstance, cancellationToken));
      }
      return resourceResponse;
    }

    public Task<ResourceResponse<Attachment>> ReplaceAttachmentAsync(
      Attachment attachment,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Attachment>>((Func<Task<ResourceResponse<Attachment>>>) (() => this.ReplaceAttachmentPrivateAsync(attachment, options, retryPolicyInstance, cancellationToken)), (IRetryPolicy) retryPolicyInstance, cancellationToken);
    }

    private async Task<ResourceResponse<Attachment>> ReplaceAttachmentPrivateAsync(
      Attachment attachment,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken,
      string altLink = null)
    {
      await this.EnsureValidClientAsync();
      if (attachment == null)
        throw new ArgumentNullException(nameof (attachment));
      this.ValidateResource((Resource) attachment);
      INameValueCollection requestHeaders = this.GetRequestHeaders(options);
      ResourceResponse<Attachment> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Replace, altLink ?? this.GetLinkForRouting((Resource) attachment), (Resource) attachment, ResourceType.Attachment, AuthorizationTokenType.PrimaryMasterKey, requestHeaders))
      {
        await this.AddPartitionKeyInformationAsync(request, options);
        resourceResponse = new ResourceResponse<Attachment>(await this.UpdateAsync(request, retryPolicyInstance, cancellationToken));
      }
      return resourceResponse;
    }

    public Task<ResourceResponse<Attachment>> ReadAttachmentAsync(
      string attachmentLink,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Attachment>>((Func<Task<ResourceResponse<Attachment>>>) (() => this.ReadAttachmentPrivateAsync(attachmentLink, options, retryPolicyInstance, cancellationToken)), (IRetryPolicy) retryPolicyInstance, cancellationToken);
    }

    private async Task<ResourceResponse<Attachment>> ReadAttachmentPrivateAsync(
      string attachmentLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(attachmentLink))
        throw new ArgumentNullException(nameof (attachmentLink));
      ResourceResponse<Attachment> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.Attachment, attachmentLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
      {
        await this.AddPartitionKeyInformationAsync(request, options);
        resourceResponse = new ResourceResponse<Attachment>(await this.ReadAsync(request, retryPolicyInstance, cancellationToken));
      }
      return resourceResponse;
    }

    public Task<FeedResponse<Attachment>> ReadAttachmentFeedAsync(
      string attachmentsLink,
      FeedOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<FeedResponse<Attachment>>((Func<Task<FeedResponse<Attachment>>>) (() => this.ReadAttachmentFeedPrivateAsync(attachmentsLink, options, retryPolicyInstance, cancellationToken)), (IRetryPolicy) retryPolicyInstance, cancellationToken);
    }

    private async Task<FeedResponse<Attachment>> ReadAttachmentFeedPrivateAsync(
      string attachmentsLink,
      FeedOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(attachmentsLink))
        throw new ArgumentNullException(nameof (attachmentsLink));
      return await client.CreateAttachmentFeedReader(attachmentsLink, options).ExecuteNextAsync(cancellationToken);
    }

    public Task<ResourceResponse<Attachment>> UpsertAttachmentAsync(
      string documentLink,
      object attachment,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Attachment>>((Func<Task<ResourceResponse<Attachment>>>) (() => this.UpsertAttachmentPrivateAsync(documentLink, attachment, options, retryPolicyInstance, cancellationToken)), (IRetryPolicy) retryPolicyInstance, cancellationToken);
    }

    private async Task<ResourceResponse<Attachment>> UpsertAttachmentPrivateAsync(
      string documentLink,
      object attachment,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(documentLink))
        throw new ArgumentNullException(nameof (documentLink));
      if (attachment == null)
        throw new ArgumentNullException(nameof (attachment));
      INameValueCollection requestHeaders = this.GetRequestHeaders(options);
      Attachment attachment1 = Attachment.FromObject(attachment);
      this.ValidateResource((Resource) attachment1);
      ResourceResponse<Attachment> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Upsert, documentLink, (Resource) attachment1, ResourceType.Attachment, AuthorizationTokenType.PrimaryMasterKey, requestHeaders))
      {
        if (!request.IsValidAddress(ResourceType.Document))
          throw new ArgumentException(RMResources.BadUrl, "link");
        await this.AddPartitionKeyInformationAsync(request, options);
        resourceResponse = new ResourceResponse<Attachment>(await this.UpsertAsync(request, retryPolicyInstance, cancellationToken));
      }
      return resourceResponse;
    }

    public Task<ResourceResponse<Attachment>> CreateAttachmentAsync(
      string attachmentsLink,
      Stream mediaStream,
      MediaOptions options = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.CreateAttachmentWrapperAsync(attachmentsLink, mediaStream, options, requestOptions, cancellationToken);
    }

    private async Task<ResourceResponse<Attachment>> CreateAttachmentWrapperAsync(
      string attachmentsLink,
      Stream mediaStream,
      MediaOptions options = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (mediaStream == null)
        throw new ArgumentNullException(nameof (mediaStream));
      CloneableStream cloneableStream = await StreamExtension.AsClonableStreamAsync(mediaStream);
      IDocumentClientRetryPolicy retryPolicyInstance = this.retryPolicy.GetRequestPolicy();
      return await TaskHelper.InlineIfPossible<ResourceResponse<Attachment>>((Func<Task<ResourceResponse<Attachment>>>) (() => this.CreateAttachmentPrivateAsync(attachmentsLink, cloneableStream, options, requestOptions, retryPolicyInstance, cancellationToken)), (IRetryPolicy) retryPolicyInstance, cancellationToken);
    }

    private async Task<ResourceResponse<Attachment>> CreateAttachmentPrivateAsync(
      string attachmentsLink,
      CloneableStream mediaStream,
      MediaOptions options,
      RequestOptions requestOptions,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      DocumentClient documentClient = this;
      if (string.IsNullOrEmpty(attachmentsLink))
        throw new ArgumentNullException(nameof (attachmentsLink));
      await documentClient.EnsureValidClientAsync();
      ResourceResponse<Attachment> attachmentPrivateAsync;
      using (StreamContent streamContent = new StreamContent((Stream) mediaStream.Clone()))
      {
        INameValueCollection headers = (INameValueCollection) new DictionaryNameValueCollection();
        if (options == null || string.IsNullOrEmpty(options.ContentType))
        {
          streamContent.Headers.Add("Content-Type", "application/octet-stream");
          headers.Set("Content-Type", "application/octet-stream");
        }
        if (options != null)
        {
          if (options.ContentType != null)
          {
            streamContent.Headers.Add("Content-Type", options.ContentType);
            headers.Set("Content-Type", options.ContentType);
          }
          if (options.Slug != null)
          {
            streamContent.Headers.Add("Slug", options.Slug);
            headers.Set("Slug", options.Slug);
          }
        }
        using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Create, ResourceType.Attachment, attachmentsLink, AuthorizationTokenType.PrimaryMasterKey))
        {
          if (!request.IsValidAddress(ResourceType.Document))
            throw new ArgumentException(RMResources.BadUrl, "link");
          retryPolicyInstance?.OnBeforeSendRequest(request);
          using (HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(documentClient.globalEndpointManager.ResolveServiceEndpoint(request), PathsHelper.GeneratePath(ResourceType.Attachment, request, true))))
          {
            string str1 = DateTime.UtcNow.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture);
            requestMessage.Headers.Add("x-ms-date", str1);
            headers.Set("x-ms-date", str1);
            if (requestOptions != null && requestOptions.PartitionKey != null)
            {
              string str2 = requestOptions.PartitionKey.ToString();
              if (!string.IsNullOrEmpty(str2))
              {
                requestMessage.Headers.Add("x-ms-documentdb-partitionkey", str2);
                headers.Set("x-ms-documentdb-partitionkey", str2);
              }
            }
            requestMessage.Content = (HttpContent) streamContent;
            requestMessage.Headers.Add("authorization", ((IAuthorizationTokenProvider) documentClient).GetUserAuthorizationToken(request.ResourceAddress, PathsHelper.GetResourcePath(ResourceType.Attachment), "POST", headers, AuthorizationTokenType.PrimaryMasterKey, out string _));
            using (HttpResponseMessage response = await documentClient.mediaClient.SendHttpAsync(requestMessage, cancellationToken))
              attachmentPrivateAsync = new ResourceResponse<Attachment>(await ClientExtensions.ParseResponseAsync(response));
          }
        }
      }
      return attachmentPrivateAsync;
    }

    public Task<ResourceResponse<Attachment>> UpsertAttachmentAsync(
      string attachmentsLink,
      Stream mediaStream,
      MediaOptions options = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.UpsertAttachmentPrivateWrapperAsync(attachmentsLink, mediaStream, options, requestOptions, cancellationToken);
    }

    private async Task<ResourceResponse<Attachment>> UpsertAttachmentPrivateWrapperAsync(
      string attachmentsLink,
      Stream mediaStream,
      MediaOptions options = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (mediaStream == null)
        throw new ArgumentNullException(nameof (mediaStream));
      CloneableStream cloneableStream = await StreamExtension.AsClonableStreamAsync(mediaStream);
      IDocumentClientRetryPolicy retryPolicyInstance = this.retryPolicy.GetRequestPolicy();
      return await TaskHelper.InlineIfPossible<ResourceResponse<Attachment>>((Func<Task<ResourceResponse<Attachment>>>) (() => this.UpsertAttachmentPrivateAsync(attachmentsLink, cloneableStream, options, requestOptions, retryPolicyInstance, cancellationToken)), (IRetryPolicy) retryPolicyInstance, cancellationToken);
    }

    private async Task<ResourceResponse<Attachment>> UpsertAttachmentPrivateAsync(
      string attachmentsLink,
      CloneableStream mediaStream,
      MediaOptions options,
      RequestOptions requestOptions,
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      DocumentClient documentClient = this;
      if (string.IsNullOrEmpty(attachmentsLink))
        throw new ArgumentNullException(nameof (attachmentsLink));
      await documentClient.EnsureValidClientAsync();
      ResourceResponse<Attachment> resourceResponse;
      using (StreamContent streamContent = new StreamContent((Stream) mediaStream.Clone()))
      {
        INameValueCollection headers = (INameValueCollection) new DictionaryNameValueCollection();
        if (options == null || string.IsNullOrEmpty(options.ContentType))
        {
          streamContent.Headers.Add("Content-Type", "application/octet-stream");
          headers.Set("Content-Type", "application/octet-stream");
        }
        if (options != null)
        {
          if (options.ContentType != null)
          {
            streamContent.Headers.Add("Content-Type", options.ContentType);
            headers.Set("Content-Type", options.ContentType);
          }
          if (options.Slug != null)
          {
            streamContent.Headers.Add("Slug", options.Slug);
            headers.Set("Slug", options.Slug);
          }
        }
        using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Upsert, ResourceType.Attachment, attachmentsLink, AuthorizationTokenType.PrimaryMasterKey))
        {
          if (!request.IsValidAddress(ResourceType.Document))
            throw new ArgumentException(RMResources.BadUrl, "link");
          retryPolicyInstance?.OnBeforeSendRequest(request);
          using (HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(documentClient.globalEndpointManager.ResolveServiceEndpoint(request), PathsHelper.GeneratePath(ResourceType.Attachment, request, true))))
          {
            string str1 = DateTime.UtcNow.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture);
            requestMessage.Headers.Add("x-ms-date", str1);
            requestMessage.Headers.Add("x-ms-documentdb-is-upsert", bool.TrueString);
            headers.Set("x-ms-date", str1);
            if (requestOptions != null)
            {
              if (requestOptions.PartitionKey != null)
              {
                string str2 = requestOptions.PartitionKey.ToString();
                if (!string.IsNullOrEmpty(str2))
                {
                  requestMessage.Headers.Add("x-ms-documentdb-partitionkey", str2);
                  headers.Set("x-ms-documentdb-partitionkey", str2);
                }
              }
              if (requestOptions.AccessCondition != null)
              {
                if (requestOptions.AccessCondition.Type == AccessConditionType.IfMatch)
                {
                  requestMessage.Headers.Add("If-Match", requestOptions.AccessCondition.Condition);
                  headers.Set("If-Match", requestOptions.AccessCondition.Condition);
                }
                else
                {
                  requestMessage.Headers.Add("If-None-Match", requestOptions.AccessCondition.Condition);
                  headers.Set("If-None-Match", requestOptions.AccessCondition.Condition);
                }
              }
            }
            requestMessage.Content = (HttpContent) streamContent;
            requestMessage.Headers.Add("authorization", ((IAuthorizationTokenProvider) documentClient).GetUserAuthorizationToken(request.ResourceAddress, PathsHelper.GetResourcePath(ResourceType.Attachment), "POST", headers, AuthorizationTokenType.PrimaryMasterKey, out string _));
            using (HttpResponseMessage response = await documentClient.mediaClient.SendHttpAsync(requestMessage, cancellationToken))
              resourceResponse = new ResourceResponse<Attachment>(await ClientExtensions.ParseResponseAsync(response));
          }
        }
      }
      return resourceResponse;
    }

    public Task<MediaResponse> UpdateMediaAsync(
      string mediaLink,
      Stream mediaStream,
      MediaOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return TaskHelper.InlineIfPossible<MediaResponse>((Func<Task<MediaResponse>>) (() => this.UpdateMediaPrivateAsync(mediaLink, mediaStream, options, cancellationToken)), (IRetryPolicy) this.ResetSessionTokenRetryPolicy.GetRequestPolicy(), cancellationToken);
    }

    private async Task<MediaResponse> UpdateMediaPrivateAsync(
      string mediaLink,
      Stream mediaStream,
      MediaOptions options,
      CancellationToken cancellationToken)
    {
      DocumentClient documentClient = this;
      await documentClient.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(mediaLink))
        throw new ArgumentNullException(nameof (mediaLink));
      if (mediaStream == null)
        throw new ArgumentNullException(nameof (mediaStream));
      string[] strArray = UrlUtility.SplitAndRemoveEmptyEntries(mediaLink, new char[1]
      {
        '/'
      });
      if (strArray.Length != 2)
        throw new ArgumentException(RMResources.InvalidUrl, nameof (mediaLink));
      MediaResponse mediaResponse1;
      using (StreamContent streamContent = new StreamContent(mediaStream))
      {
        if (options != null)
        {
          if (options.ContentType != null)
            streamContent.Headers.Add("Content-Type", options.ContentType);
          if (options.Slug != null)
            streamContent.Headers.Add("Slug", options.Slug);
        }
        using (HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Put, new Uri(documentClient.WriteEndpoint, mediaLink)))
        {
          string attachmentId = documentClient.GetAttachmentId(strArray[1]);
          string str = DateTime.UtcNow.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture);
          INameValueCollection headers = (INameValueCollection) new DictionaryNameValueCollection();
          headers.Set("x-ms-date", str);
          requestMessage.Headers.Add("x-ms-date", str);
          requestMessage.Content = (HttpContent) streamContent;
          requestMessage.Headers.Add("authorization", ((IAuthorizationTokenProvider) documentClient).GetUserAuthorizationToken(attachmentId, "media", "PUT", headers, AuthorizationTokenType.PrimaryMasterKey, out string _));
          using (HttpResponseMessage response = await documentClient.mediaClient.SendHttpAsync(requestMessage, cancellationToken))
          {
            using (DocumentServiceResponse responseAsync = await ClientExtensions.ParseResponseAsync(response))
            {
              MediaResponse mediaResponse2 = new MediaResponse();
              mediaResponse2.ContentType = responseAsync.Headers["Content-Type"];
              mediaResponse2.Slug = responseAsync.Headers["Slug"];
              mediaResponse2.ActivityId = responseAsync.Headers["x-ms-activity-id"];
              string header = responseAsync.Headers["Content-Length"];
              if (!string.IsNullOrEmpty(header))
                mediaResponse2.ContentLength = long.Parse(header, (IFormatProvider) CultureInfo.InvariantCulture);
              mediaResponse2.Headers = responseAsync.Headers.Clone();
              mediaResponse1 = mediaResponse2;
            }
          }
        }
      }
      return mediaResponse1;
    }

    public Task<MediaResponse> ReadMediaMetadataAsync(string mediaLink) => TaskHelper.InlineIfPossible<MediaResponse>((Func<Task<MediaResponse>>) (() => this.ReadMediaMetadataPrivateAsync(mediaLink)), (IRetryPolicy) this.ResetSessionTokenRetryPolicy.GetRequestPolicy());

    private async Task<MediaResponse> ReadMediaMetadataPrivateAsync(string mediaLink)
    {
      DocumentClient documentClient = this;
      await documentClient.EnsureValidClientAsync();
      string[] strArray = !string.IsNullOrEmpty(mediaLink) ? UrlUtility.SplitAndRemoveEmptyEntries(mediaLink, new char[1]
      {
        '/'
      }) : throw new ArgumentNullException(nameof (mediaLink));
      if (strArray.Length != 2)
        throw new ArgumentException(RMResources.InvalidUrl, nameof (mediaLink));
      MediaResponse mediaResponse1;
      using (HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Head, new Uri(documentClient.WriteEndpoint, mediaLink)))
      {
        string attachmentId = documentClient.GetAttachmentId(strArray[1]);
        string str = DateTime.UtcNow.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture);
        INameValueCollection headers = (INameValueCollection) new DictionaryNameValueCollection();
        headers.Set("x-ms-date", str);
        requestMessage.Headers.Add("x-ms-date", str);
        requestMessage.Headers.Add("authorization", ((IAuthorizationTokenProvider) documentClient).GetUserAuthorizationToken(attachmentId, "media", "HEAD", headers, AuthorizationTokenType.PrimaryMasterKey, out string _));
        using (HttpResponseMessage response = await documentClient.mediaClient.SendHttpAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead))
        {
          using (DocumentServiceResponse responseAsync = await ClientExtensions.ParseResponseAsync(response))
          {
            MediaResponse mediaResponse2 = new MediaResponse();
            mediaResponse2.ContentType = responseAsync.Headers["Content-Type"];
            mediaResponse2.Slug = responseAsync.Headers["Slug"];
            mediaResponse2.ActivityId = responseAsync.Headers["x-ms-activity-id"];
            string header = responseAsync.Headers["Content-Length"];
            if (!string.IsNullOrEmpty(header))
              mediaResponse2.ContentLength = long.Parse(header, (IFormatProvider) CultureInfo.InvariantCulture);
            mediaResponse2.Headers = responseAsync.Headers.Clone();
            mediaResponse1 = mediaResponse2;
          }
        }
      }
      return mediaResponse1;
    }

    public Task<MediaResponse> ReadMediaAsync(string mediaLink, CancellationToken cancellationToken = default (CancellationToken)) => TaskHelper.InlineIfPossible<MediaResponse>((Func<Task<MediaResponse>>) (() => this.ReadMediaPrivateAsync(mediaLink, cancellationToken)), (IRetryPolicy) this.ResetSessionTokenRetryPolicy.GetRequestPolicy(), cancellationToken);

    private async Task<MediaResponse> ReadMediaPrivateAsync(
      string mediaLink,
      CancellationToken cancellationToken)
    {
      DocumentClient documentClient = this;
      await documentClient.EnsureValidClientAsync();
      string[] strArray = !string.IsNullOrEmpty(mediaLink) ? UrlUtility.SplitAndRemoveEmptyEntries(mediaLink, new char[1]
      {
        '/'
      }) : throw new ArgumentNullException(nameof (mediaLink));
      if (strArray.Length != 2)
        throw new ArgumentException(RMResources.InvalidUrl, nameof (mediaLink));
      MediaResponse mediaResponse1;
      using (HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(documentClient.WriteEndpoint, mediaLink)))
      {
        string attachmentId = documentClient.GetAttachmentId(strArray[1]);
        string str = DateTime.UtcNow.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture);
        INameValueCollection headers = (INameValueCollection) new DictionaryNameValueCollection();
        headers.Set("x-ms-date", str);
        requestMessage.Headers.Add("x-ms-date", str);
        requestMessage.Headers.Add("authorization", ((IAuthorizationTokenProvider) documentClient).GetUserAuthorizationToken(attachmentId, "media", "GET", headers, AuthorizationTokenType.PrimaryMasterKey, out string _));
        HttpResponseMessage responseMessage = (HttpResponseMessage) null;
        if (documentClient.connectionPolicy.MediaReadMode == MediaReadMode.Streamed)
          responseMessage = await documentClient.mediaClient.SendHttpAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        else
          responseMessage = await documentClient.mediaClient.SendHttpAsync(requestMessage, cancellationToken);
        try
        {
          DocumentServiceResponse mediaResponseAsync = await ClientExtensions.ParseMediaResponseAsync(responseMessage, cancellationToken);
          MediaResponse mediaResponse2 = new MediaResponse();
          mediaResponse2.Media = mediaResponseAsync.ResponseBody;
          mediaResponse2.Slug = mediaResponseAsync.Headers["Slug"];
          mediaResponse2.ContentType = mediaResponseAsync.Headers["Content-Type"];
          mediaResponse2.ActivityId = mediaResponseAsync.Headers["x-ms-activity-id"];
          string header = mediaResponseAsync.Headers["Content-Length"];
          if (!string.IsNullOrEmpty(header))
            mediaResponse2.ContentLength = long.Parse(header, (IFormatProvider) CultureInfo.InvariantCulture);
          mediaResponse2.Headers = mediaResponseAsync.Headers;
          mediaResponse1 = mediaResponse2;
        }
        catch (Exception ex)
        {
          responseMessage.Dispose();
          throw;
        }
      }
      return mediaResponse1;
    }

    private string GetAttachmentId(string mediaId)
    {
      string attachmentId = (string) null;
      byte storageIndex = 0;
      if (!MediaIdHelper.TryParseMediaId(mediaId, out attachmentId, out storageIndex))
        throw new ArgumentException(ClientResources.MediaLinkInvalid);
      return attachmentId;
    }

    public Task<ResourceResponse<Permission>> CreatePermissionAsync(
      string userLink,
      Permission permission,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Permission>>((Func<Task<ResourceResponse<Permission>>>) (() => this.CreatePermissionPrivateAsync(userLink, permission, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Permission>> CreatePermissionPrivateAsync(
      string userLink,
      Permission permission,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(userLink))
        throw new ArgumentNullException(nameof (userLink));
      if (permission == null)
        throw new ArgumentNullException(nameof (permission));
      this.ValidateResource((Resource) permission);
      ResourceResponse<Permission> permissionPrivateAsync;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Create, userLink, (Resource) permission, ResourceType.Permission, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        permissionPrivateAsync = new ResourceResponse<Permission>(await this.CreateAsync(request, retryPolicyInstance));
      return permissionPrivateAsync;
    }

    public Task<ResourceResponse<User>> CreateUserAsync(
      string databaseLink,
      User user,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<User>>((Func<Task<ResourceResponse<User>>>) (() => this.CreateUserPrivateAsync(databaseLink, user, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<User>> CreateUserPrivateAsync(
      string databaseLink,
      User user,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(databaseLink))
        throw new ArgumentNullException(nameof (databaseLink));
      if (user == null)
        throw new ArgumentNullException(nameof (user));
      this.ValidateResource((Resource) user);
      ResourceResponse<User> userPrivateAsync;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Create, databaseLink, (Resource) user, ResourceType.User, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        userPrivateAsync = new ResourceResponse<User>(await this.CreateAsync(request, retryPolicyInstance));
      return userPrivateAsync;
    }

    public Task<ResourceResponse<Permission>> DeletePermissionAsync(
      string permissionLink,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Permission>>((Func<Task<ResourceResponse<Permission>>>) (() => this.DeletePermissionPrivateAsync(permissionLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Permission>> DeletePermissionPrivateAsync(
      string permissionLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(permissionLink))
        throw new ArgumentNullException(nameof (permissionLink));
      ResourceResponse<Permission> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Delete, ResourceType.Permission, permissionLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        resourceResponse = new ResourceResponse<Permission>(await this.DeleteAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<User>> DeleteUserAsync(string userLink, RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<User>>((Func<Task<ResourceResponse<User>>>) (() => this.DeleteUserPrivateAsync(userLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<User>> DeleteUserPrivateAsync(
      string userLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(userLink))
        throw new ArgumentNullException(nameof (userLink));
      ResourceResponse<User> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Delete, ResourceType.User, userLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        resourceResponse = new ResourceResponse<User>(await this.DeleteAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<Permission>> ReplacePermissionAsync(
      Permission permission,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Permission>>((Func<Task<ResourceResponse<Permission>>>) (() => this.ReplacePermissionPrivateAsync(permission, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Permission>> ReplacePermissionPrivateAsync(
      Permission permission,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      string altLink = null)
    {
      await this.EnsureValidClientAsync();
      if (permission == null)
        throw new ArgumentNullException(nameof (permission));
      this.ValidateResource((Resource) permission);
      INameValueCollection requestHeaders = this.GetRequestHeaders(options);
      ResourceResponse<Permission> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Replace, altLink ?? this.GetLinkForRouting((Resource) permission), (Resource) permission, ResourceType.Permission, AuthorizationTokenType.PrimaryMasterKey, requestHeaders))
        resourceResponse = new ResourceResponse<Permission>(await this.UpdateAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<User>> ReplaceUserAsync(User user, RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<User>>((Func<Task<ResourceResponse<User>>>) (() => this.ReplaceUserPrivateAsync(user, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<User>> ReplaceUserPrivateAsync(
      User user,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance,
      string altLink = null)
    {
      await this.EnsureValidClientAsync();
      if (user == null)
        throw new ArgumentNullException(nameof (user));
      this.ValidateResource((Resource) user);
      INameValueCollection requestHeaders = this.GetRequestHeaders(options);
      ResourceResponse<User> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Replace, altLink ?? this.GetLinkForRouting((Resource) user), (Resource) user, ResourceType.User, AuthorizationTokenType.PrimaryMasterKey, requestHeaders))
        resourceResponse = new ResourceResponse<User>(await this.UpdateAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<Permission>> ReadPermissionAsync(
      string permissionLink,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Permission>>((Func<Task<ResourceResponse<Permission>>>) (() => this.ReadPermissionPrivateAsync(permissionLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Permission>> ReadPermissionPrivateAsync(
      string permissionLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(permissionLink))
        throw new ArgumentNullException(nameof (permissionLink));
      ResourceResponse<Permission> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.Permission, permissionLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        resourceResponse = new ResourceResponse<Permission>(await this.ReadAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<User>> ReadUserAsync(string userLink, RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<User>>((Func<Task<ResourceResponse<User>>>) (() => this.ReadUserPrivateAsync(userLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<User>> ReadUserPrivateAsync(
      string userLink,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(userLink))
        throw new ArgumentNullException(nameof (userLink));
      ResourceResponse<User> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Read, ResourceType.User, userLink, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        resourceResponse = new ResourceResponse<User>(await this.ReadAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<FeedResponse<Permission>> ReadPermissionFeedAsync(
      string permissionsLink,
      FeedOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<FeedResponse<Permission>>((Func<Task<FeedResponse<Permission>>>) (() => this.ReadPermissionFeedPrivateAsync(permissionsLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<FeedResponse<Permission>> ReadPermissionFeedPrivateAsync(
      string permissionsLink,
      FeedOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(permissionsLink))
        throw new ArgumentNullException(nameof (permissionsLink));
      return await client.CreatePermissionFeedReader(permissionsLink, options).ExecuteNextAsync();
    }

    public Task<FeedResponse<User>> ReadUserFeedAsync(string usersLink, FeedOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<FeedResponse<User>>((Func<Task<FeedResponse<User>>>) (() => this.ReadUserFeedPrivateAsync(usersLink, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<FeedResponse<User>> ReadUserFeedPrivateAsync(
      string usersLink,
      FeedOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      DocumentClient client = this;
      await client.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(usersLink))
        throw new ArgumentNullException(nameof (usersLink));
      return await client.CreateUserFeedReader(usersLink, options).ExecuteNextAsync();
    }

    public Task<ResourceResponse<Permission>> UpsertPermissionAsync(
      string userLink,
      Permission permission,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Permission>>((Func<Task<ResourceResponse<Permission>>>) (() => this.UpsertPermissionPrivateAsync(userLink, permission, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<Permission>> UpsertPermissionPrivateAsync(
      string userLink,
      Permission permission,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(userLink))
        throw new ArgumentNullException(nameof (userLink));
      if (permission == null)
        throw new ArgumentNullException(nameof (permission));
      this.ValidateResource((Resource) permission);
      ResourceResponse<Permission> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Upsert, userLink, (Resource) permission, ResourceType.Permission, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        resourceResponse = new ResourceResponse<Permission>(await this.UpsertAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<User>> UpsertUserAsync(
      string databaseLink,
      User user,
      RequestOptions options = null)
    {
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<User>>((Func<Task<ResourceResponse<User>>>) (() => this.UpsertUserPrivateAsync(databaseLink, user, options, retryPolicyInstance)), (IRetryPolicy) retryPolicyInstance);
    }

    private async Task<ResourceResponse<User>> UpsertUserPrivateAsync(
      string databaseLink,
      User user,
      RequestOptions options,
      IDocumentClientRetryPolicy retryPolicyInstance)
    {
      await this.EnsureValidClientAsync();
      if (string.IsNullOrEmpty(databaseLink))
        throw new ArgumentNullException(nameof (databaseLink));
      if (user == null)
        throw new ArgumentNullException(nameof (user));
      this.ValidateResource((Resource) user);
      ResourceResponse<User> resourceResponse;
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Upsert, databaseLink, (Resource) user, ResourceType.User, AuthorizationTokenType.PrimaryMasterKey, this.GetRequestHeaders(options)))
        resourceResponse = new ResourceResponse<User>(await this.UpsertAsync(request, retryPolicyInstance));
      return resourceResponse;
    }

    public Task<ResourceResponse<Attachment>> CreateAttachmentAsync(
      Uri documentUri,
      object attachment,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (documentUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentUri));
      return this.CreateAttachmentAsync(documentUri.OriginalString, attachment, options, cancellationToken);
    }

    public Task<ResourceResponse<Attachment>> CreateAttachmentAsync(
      Uri documentUri,
      Stream mediaStream,
      MediaOptions options = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (documentUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentUri));
      return this.CreateAttachmentAsync(documentUri.OriginalString, mediaStream, options, requestOptions, cancellationToken);
    }

    public Task<ResourceResponse<Document>> CreateDocumentAsync(
      Uri documentCollectionUri,
      object document,
      RequestOptions options = null,
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
      RequestOptions options = null)
    {
      if (databaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (databaseUri));
      return this.CreateDocumentCollectionAsync(databaseUri.OriginalString, documentCollection, options);
    }

    public Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionIfNotExistsAsync(
      Uri databaseUri,
      DocumentCollection documentCollection,
      RequestOptions options = null)
    {
      return TaskHelper.InlineIfPossible<ResourceResponse<DocumentCollection>>((Func<Task<ResourceResponse<DocumentCollection>>>) (() => this.CreateDocumentCollectionIfNotExistsPrivateAsync(databaseUri, documentCollection, options)), (IRetryPolicy) null);
    }

    private async Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionIfNotExistsPrivateAsync(
      Uri databaseUri,
      DocumentCollection documentCollection,
      RequestOptions options)
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
      RequestOptions options = null)
    {
      if (documentCollectionUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionUri));
      return this.CreateStoredProcedureAsync(documentCollectionUri.OriginalString, storedProcedure, options);
    }

    public Task<ResourceResponse<Trigger>> CreateTriggerAsync(
      Uri documentCollectionUri,
      Trigger trigger,
      RequestOptions options = null)
    {
      if (documentCollectionUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionUri));
      return this.CreateTriggerAsync(documentCollectionUri.OriginalString, trigger, options);
    }

    public Task<ResourceResponse<UserDefinedFunction>> CreateUserDefinedFunctionAsync(
      Uri documentCollectionUri,
      UserDefinedFunction function,
      RequestOptions options = null)
    {
      if (documentCollectionUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionUri));
      return this.CreateUserDefinedFunctionAsync(documentCollectionUri.OriginalString, function, options);
    }

    public Task<ResourceResponse<Permission>> CreatePermissionAsync(
      Uri userUri,
      Permission permission,
      RequestOptions options = null)
    {
      if (userUri == (Uri) null)
        throw new ArgumentNullException(nameof (userUri));
      return this.CreatePermissionAsync(userUri.OriginalString, permission, options);
    }

    public Task<ResourceResponse<User>> CreateUserAsync(
      Uri databaseUri,
      User user,
      RequestOptions options = null)
    {
      if (databaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (databaseUri));
      return this.CreateUserAsync(databaseUri.OriginalString, user, options);
    }

    internal Task<ResourceResponse<UserDefinedType>> CreateUserDefinedTypeAsync(
      Uri databaseUri,
      UserDefinedType userDefinedType,
      RequestOptions options = null)
    {
      if (databaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (databaseUri));
      return this.CreateUserDefinedTypeAsync(databaseUri.OriginalString, userDefinedType, options);
    }

    public Task<ResourceResponse<Attachment>> UpsertAttachmentAsync(
      Uri documentUri,
      object attachment,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (documentUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentUri));
      return this.UpsertAttachmentAsync(documentUri.OriginalString, attachment, options, cancellationToken);
    }

    public Task<ResourceResponse<Attachment>> UpsertAttachmentAsync(
      Uri documentUri,
      Stream mediaStream,
      MediaOptions options = null,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (documentUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentUri));
      return this.UpsertAttachmentAsync(documentUri.OriginalString, mediaStream, options, requestOptions, cancellationToken);
    }

    public Task<ResourceResponse<Document>> UpsertDocumentAsync(
      Uri documentCollectionUri,
      object document,
      RequestOptions options = null,
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
      RequestOptions options = null)
    {
      if (documentCollectionUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionUri));
      return this.UpsertStoredProcedureAsync(documentCollectionUri.OriginalString, storedProcedure, options);
    }

    public Task<ResourceResponse<Trigger>> UpsertTriggerAsync(
      Uri documentCollectionUri,
      Trigger trigger,
      RequestOptions options = null)
    {
      if (documentCollectionUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionUri));
      return this.UpsertTriggerAsync(documentCollectionUri.OriginalString, trigger, options);
    }

    public Task<ResourceResponse<UserDefinedFunction>> UpsertUserDefinedFunctionAsync(
      Uri documentCollectionUri,
      UserDefinedFunction function,
      RequestOptions options = null)
    {
      if (documentCollectionUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionUri));
      return this.UpsertUserDefinedFunctionAsync(documentCollectionUri.OriginalString, function, options);
    }

    public Task<ResourceResponse<Permission>> UpsertPermissionAsync(
      Uri userUri,
      Permission permission,
      RequestOptions options = null)
    {
      if (userUri == (Uri) null)
        throw new ArgumentNullException(nameof (userUri));
      return this.UpsertPermissionAsync(userUri.OriginalString, permission, options);
    }

    public Task<ResourceResponse<User>> UpsertUserAsync(
      Uri databaseUri,
      User user,
      RequestOptions options = null)
    {
      if (databaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (databaseUri));
      return this.UpsertUserAsync(databaseUri.OriginalString, user, options);
    }

    internal Task<ResourceResponse<UserDefinedType>> UpsertUserDefinedTypeAsync(
      Uri databaseUri,
      UserDefinedType userDefinedType,
      RequestOptions options = null)
    {
      if (databaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (databaseUri));
      return this.UpsertUserDefinedTypeAsync(databaseUri.OriginalString, userDefinedType, options);
    }

    public Task<ResourceResponse<Attachment>> DeleteAttachmentAsync(
      Uri attachmentUri,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (attachmentUri == (Uri) null)
        throw new ArgumentNullException(nameof (attachmentUri));
      return this.DeleteAttachmentAsync(attachmentUri.OriginalString, options, cancellationToken);
    }

    public Task<ResourceResponse<Database>> DeleteDatabaseAsync(
      Uri databaseUri,
      RequestOptions options = null)
    {
      if (databaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (databaseUri));
      return this.DeleteDatabaseAsync(databaseUri.OriginalString, options);
    }

    public Task<ResourceResponse<Document>> DeleteDocumentAsync(
      Uri documentUri,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (documentUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentUri));
      return this.DeleteDocumentAsync(documentUri.OriginalString, options, cancellationToken);
    }

    public Task<ResourceResponse<DocumentCollection>> DeleteDocumentCollectionAsync(
      Uri documentCollectionUri,
      RequestOptions options = null)
    {
      if (documentCollectionUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionUri));
      return this.DeleteDocumentCollectionAsync(documentCollectionUri.OriginalString, options);
    }

    public Task<ResourceResponse<StoredProcedure>> DeleteStoredProcedureAsync(
      Uri storedProcedureUri,
      RequestOptions options = null)
    {
      if (storedProcedureUri == (Uri) null)
        throw new ArgumentNullException(nameof (storedProcedureUri));
      return this.DeleteStoredProcedureAsync(storedProcedureUri.OriginalString, options);
    }

    public Task<ResourceResponse<Trigger>> DeleteTriggerAsync(
      Uri triggerUri,
      RequestOptions options = null)
    {
      if (triggerUri == (Uri) null)
        throw new ArgumentNullException(nameof (triggerUri));
      return this.DeleteTriggerAsync(triggerUri.OriginalString, options);
    }

    public Task<ResourceResponse<UserDefinedFunction>> DeleteUserDefinedFunctionAsync(
      Uri functionUri,
      RequestOptions options = null)
    {
      if (functionUri == (Uri) null)
        throw new ArgumentNullException(nameof (functionUri));
      return this.DeleteUserDefinedFunctionAsync(functionUri.OriginalString, options);
    }

    public Task<ResourceResponse<Permission>> DeletePermissionAsync(
      Uri permissionUri,
      RequestOptions options = null)
    {
      if (permissionUri == (Uri) null)
        throw new ArgumentNullException(nameof (permissionUri));
      return this.DeletePermissionAsync(permissionUri.OriginalString, options);
    }

    public Task<ResourceResponse<User>> DeleteUserAsync(Uri userUri, RequestOptions options = null)
    {
      if (userUri == (Uri) null)
        throw new ArgumentNullException(nameof (userUri));
      return this.DeleteUserAsync(userUri.OriginalString, options);
    }

    public Task<ResourceResponse<Conflict>> DeleteConflictAsync(
      Uri conflictUri,
      RequestOptions options = null)
    {
      if (conflictUri == (Uri) null)
        throw new ArgumentNullException(nameof (conflictUri));
      return this.DeleteConflictAsync(conflictUri.OriginalString, options);
    }

    internal Task<ResourceResponse<Snapshot>> DeleteSnapshotAsync(
      Uri snapshotUri,
      RequestOptions options = null)
    {
      if (snapshotUri == (Uri) null)
        throw new ArgumentNullException(nameof (snapshotUri));
      return this.DeleteSnapshotAsync(snapshotUri.OriginalString, options);
    }

    public Task<ResourceResponse<Attachment>> ReplaceAttachmentAsync(
      Uri attachmentUri,
      Attachment attachment,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (attachmentUri == (Uri) null)
        throw new ArgumentNullException(nameof (attachmentUri));
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Attachment>>((Func<Task<ResourceResponse<Attachment>>>) (() => this.ReplaceAttachmentPrivateAsync(attachment, options, retryPolicyInstance, cancellationToken, attachmentUri.OriginalString)), (IRetryPolicy) retryPolicyInstance, cancellationToken);
    }

    public Task<ResourceResponse<Document>> ReplaceDocumentAsync(
      Uri documentUri,
      object document,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (documentUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentUri));
      return this.ReplaceDocumentAsync(documentUri.OriginalString, document, options, cancellationToken);
    }

    public Task<ResourceResponse<DocumentCollection>> ReplaceDocumentCollectionAsync(
      Uri documentCollectionUri,
      DocumentCollection documentCollection,
      RequestOptions options = null)
    {
      if (documentCollectionUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionUri));
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<DocumentCollection>>((Func<Task<ResourceResponse<DocumentCollection>>>) (() => this.ReplaceDocumentCollectionPrivateAsync(documentCollection, options, retryPolicyInstance, documentCollectionUri.OriginalString)), (IRetryPolicy) retryPolicyInstance);
    }

    public Task<ResourceResponse<StoredProcedure>> ReplaceStoredProcedureAsync(
      Uri storedProcedureUri,
      StoredProcedure storedProcedure,
      RequestOptions options = null)
    {
      if (storedProcedureUri == (Uri) null)
        throw new ArgumentNullException(nameof (storedProcedureUri));
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<StoredProcedure>>((Func<Task<ResourceResponse<StoredProcedure>>>) (() => this.ReplaceStoredProcedurePrivateAsync(storedProcedure, options, retryPolicyInstance, storedProcedureUri.OriginalString)), (IRetryPolicy) retryPolicyInstance);
    }

    public Task<ResourceResponse<Trigger>> ReplaceTriggerAsync(
      Uri triggerUri,
      Trigger trigger,
      RequestOptions options = null)
    {
      if (triggerUri == (Uri) null)
        throw new ArgumentNullException(nameof (triggerUri));
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Trigger>>((Func<Task<ResourceResponse<Trigger>>>) (() => this.ReplaceTriggerPrivateAsync(trigger, options, retryPolicyInstance, triggerUri.OriginalString)), (IRetryPolicy) retryPolicyInstance);
    }

    public Task<ResourceResponse<UserDefinedFunction>> ReplaceUserDefinedFunctionAsync(
      Uri userDefinedFunctionUri,
      UserDefinedFunction function,
      RequestOptions options = null)
    {
      if (userDefinedFunctionUri == (Uri) null)
        throw new ArgumentNullException(nameof (userDefinedFunctionUri));
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<UserDefinedFunction>>((Func<Task<ResourceResponse<UserDefinedFunction>>>) (() => this.ReplaceUserDefinedFunctionPrivateAsync(function, options, retryPolicyInstance, userDefinedFunctionUri.OriginalString)), (IRetryPolicy) retryPolicyInstance);
    }

    public Task<ResourceResponse<Permission>> ReplacePermissionAsync(
      Uri permissionUri,
      Permission permission,
      RequestOptions options = null)
    {
      if (permissionUri == (Uri) null)
        throw new ArgumentNullException(nameof (permissionUri));
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<Permission>>((Func<Task<ResourceResponse<Permission>>>) (() => this.ReplacePermissionPrivateAsync(permission, options, retryPolicyInstance, permissionUri.OriginalString)), (IRetryPolicy) retryPolicyInstance);
    }

    public Task<ResourceResponse<User>> ReplaceUserAsync(
      Uri userUri,
      User user,
      RequestOptions options = null)
    {
      if (userUri == (Uri) null)
        throw new ArgumentNullException(nameof (userUri));
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<User>>((Func<Task<ResourceResponse<User>>>) (() => this.ReplaceUserPrivateAsync(user, options, retryPolicyInstance, userUri.OriginalString)), (IRetryPolicy) retryPolicyInstance);
    }

    internal Task<ResourceResponse<UserDefinedType>> ReplaceUserDefinedTypeAsync(
      Uri userDefinedTypeUri,
      UserDefinedType userDefinedType,
      RequestOptions options = null)
    {
      if (userDefinedTypeUri == (Uri) null)
        throw new ArgumentNullException(nameof (userDefinedTypeUri));
      IDocumentClientRetryPolicy retryPolicyInstance = this.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      return TaskHelper.InlineIfPossible<ResourceResponse<UserDefinedType>>((Func<Task<ResourceResponse<UserDefinedType>>>) (() => this.ReplaceUserDefinedTypePrivateAsync(userDefinedType, options, retryPolicyInstance, userDefinedTypeUri.OriginalString)), (IRetryPolicy) retryPolicyInstance);
    }

    public Task<ResourceResponse<Attachment>> ReadAttachmentAsync(
      Uri attachmentUri,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (attachmentUri == (Uri) null)
        throw new ArgumentNullException(nameof (attachmentUri));
      return this.ReadAttachmentAsync(attachmentUri.OriginalString, options, cancellationToken);
    }

    public Task<ResourceResponse<Database>> ReadDatabaseAsync(
      Uri databaseUri,
      RequestOptions options = null)
    {
      if (databaseUri == (Uri) null)
        throw new ArgumentNullException(nameof (databaseUri));
      return this.ReadDatabaseAsync(databaseUri.OriginalString, options);
    }

    public Task<ResourceResponse<Document>> ReadDocumentAsync(
      Uri documentUri,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (documentUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentUri));
      return this.ReadDocumentAsync(documentUri.OriginalString, options, cancellationToken);
    }

    public Task<DocumentResponse<T>> ReadDocumentAsync<T>(
      Uri documentUri,
      RequestOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (documentUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentUri));
      return this.ReadDocumentAsync<T>(documentUri.OriginalString, options, cancellationToken);
    }

    public Task<ResourceResponse<DocumentCollection>> ReadDocumentCollectionAsync(
      Uri documentCollectionUri,
      RequestOptions options = null)
    {
      if (documentCollectionUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionUri));
      return this.ReadDocumentCollectionAsync(documentCollectionUri.OriginalString, options);
    }

    public Task<ResourceResponse<StoredProcedure>> ReadStoredProcedureAsync(
      Uri storedProcedureUri,
      RequestOptions options = null)
    {
      if (storedProcedureUri == (Uri) null)
        throw new ArgumentNullException(nameof (storedProcedureUri));
      return this.ReadStoredProcedureAsync(storedProcedureUri.OriginalString, options);
    }

    public Task<ResourceResponse<Trigger>> ReadTriggerAsync(Uri triggerUri, RequestOptions options = null)
    {
      if (triggerUri == (Uri) null)
        throw new ArgumentNullException(nameof (triggerUri));
      return this.ReadTriggerAsync(triggerUri.OriginalString, options);
    }

    public Task<ResourceResponse<UserDefinedFunction>> ReadUserDefinedFunctionAsync(
      Uri functionUri,
      RequestOptions options = null)
    {
      if (functionUri == (Uri) null)
        throw new ArgumentNullException(nameof (functionUri));
      return this.ReadUserDefinedFunctionAsync(functionUri.OriginalString, options);
    }

    public Task<ResourceResponse<Permission>> ReadPermissionAsync(
      Uri permissionUri,
      RequestOptions options = null)
    {
      if (permissionUri == (Uri) null)
        throw new ArgumentNullException(nameof (permissionUri));
      return this.ReadPermissionAsync(permissionUri.OriginalString, options);
    }

    public Task<ResourceResponse<User>> ReadUserAsync(Uri userUri, RequestOptions options = null)
    {
      if (userUri == (Uri) null)
        throw new ArgumentNullException(nameof (userUri));
      return this.ReadUserAsync(userUri.OriginalString, options);
    }

    public Task<ResourceResponse<Conflict>> ReadConflictAsync(
      Uri conflictUri,
      RequestOptions options = null)
    {
      if (conflictUri == (Uri) null)
        throw new ArgumentNullException(nameof (conflictUri));
      return this.ReadConflictAsync(conflictUri.OriginalString, options);
    }

    internal Task<ResourceResponse<Schema>> ReadSchemaAsync(Uri schemaUri, RequestOptions options = null)
    {
      if (schemaUri == (Uri) null)
        throw new ArgumentNullException(nameof (schemaUri));
      return this.ReadSchemaAsync(schemaUri.OriginalString, options);
    }

    internal Task<ResourceResponse<UserDefinedType>> ReadUserDefinedTypeAsync(
      Uri userDefinedTypeUri,
      RequestOptions options = null)
    {
      if (userDefinedTypeUri == (Uri) null)
        throw new ArgumentNullException(nameof (userDefinedTypeUri));
      return this.ReadUserDefinedTypeAsync(userDefinedTypeUri.OriginalString, options);
    }

    internal Task<ResourceResponse<Snapshot>> ReadSnapshotAsync(
      Uri snapshotUri,
      RequestOptions options = null)
    {
      if (snapshotUri == (Uri) null)
        throw new ArgumentNullException(nameof (snapshotUri));
      return this.ReadSnapshotAsync(snapshotUri.OriginalString, options);
    }

    public Task<FeedResponse<Attachment>> ReadAttachmentFeedAsync(
      Uri attachmentsUri,
      FeedOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (attachmentsUri == (Uri) null)
        throw new ArgumentNullException(nameof (attachmentsUri));
      return this.ReadAttachmentFeedAsync(attachmentsUri.OriginalString, options, cancellationToken);
    }

    public Task<FeedResponse<DocumentCollection>> ReadDocumentCollectionFeedAsync(
      Uri documentCollectionsUri,
      FeedOptions options = null)
    {
      if (documentCollectionsUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentCollectionsUri));
      return this.ReadDocumentCollectionFeedAsync(documentCollectionsUri.OriginalString, options);
    }

    public Task<FeedResponse<StoredProcedure>> ReadStoredProcedureFeedAsync(
      Uri storedProceduresUri,
      FeedOptions options = null)
    {
      if (storedProceduresUri == (Uri) null)
        throw new ArgumentNullException(nameof (storedProceduresUri));
      return this.ReadStoredProcedureFeedAsync(storedProceduresUri.OriginalString, options);
    }

    public Task<FeedResponse<Trigger>> ReadTriggerFeedAsync(Uri triggersUri, FeedOptions options = null)
    {
      if (triggersUri == (Uri) null)
        throw new ArgumentNullException(nameof (triggersUri));
      return this.ReadTriggerFeedAsync(triggersUri.OriginalString, options);
    }

    public Task<FeedResponse<UserDefinedFunction>> ReadUserDefinedFunctionFeedAsync(
      Uri userDefinedFunctionsUri,
      FeedOptions options = null)
    {
      if (userDefinedFunctionsUri == (Uri) null)
        throw new ArgumentNullException(nameof (userDefinedFunctionsUri));
      return this.ReadUserDefinedFunctionFeedAsync(userDefinedFunctionsUri.OriginalString, options);
    }

    public Task<FeedResponse<Permission>> ReadPermissionFeedAsync(
      Uri permissionsUri,
      FeedOptions options = null)
    {
      if (permissionsUri == (Uri) null)
        throw new ArgumentNullException(nameof (permissionsUri));
      return this.ReadPermissionFeedAsync(permissionsUri.OriginalString, options);
    }

    public Task<FeedResponse<object>> ReadDocumentFeedAsync(
      Uri documentsUri,
      FeedOptions options = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (documentsUri == (Uri) null)
        throw new ArgumentNullException(nameof (documentsUri));
      return this.ReadDocumentFeedAsync(documentsUri.OriginalString, options, cancellationToken);
    }

    public Task<FeedResponse<User>> ReadUserFeedAsync(Uri usersUri, FeedOptions options = null)
    {
      if (usersUri == (Uri) null)
        throw new ArgumentNullException(nameof (usersUri));
      return this.ReadUserFeedAsync(usersUri.OriginalString, options);
    }

    public Task<FeedResponse<Conflict>> ReadConflictFeedAsync(Uri conflictsUri, FeedOptions options = null)
    {
      if (conflictsUri == (Uri) null)
        throw new ArgumentNullException(nameof (conflictsUri));
      return this.ReadConflictFeedAsync(conflictsUri.OriginalString, options);
    }

    public Task<FeedResponse<PartitionKeyRange>> ReadPartitionKeyRangeFeedAsync(
      Uri partitionKeyRangesOrCollectionUri,
      FeedOptions options = null)
    {
      if (partitionKeyRangesOrCollectionUri == (Uri) null)
        throw new ArgumentNullException(nameof (partitionKeyRangesOrCollectionUri));
      return this.ReadPartitionKeyRangeFeedAsync(partitionKeyRangesOrCollectionUri.OriginalString, options);
    }

    internal Task<FeedResponse<UserDefinedType>> ReadUserDefinedTypeFeedAsync(
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
      RequestOptions options,
      params object[] procedureParams)
    {
      if (storedProcedureUri == (Uri) null)
        throw new ArgumentNullException(nameof (storedProcedureUri));
      return this.ExecuteStoredProcedureAsync<TValue>(storedProcedureUri.OriginalString, options, procedureParams);
    }

    public Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(
      Uri storedProcedureUri,
      RequestOptions options,
      CancellationToken cancellationToken = default (CancellationToken),
      params object[] procedureParams)
    {
      if (storedProcedureUri == (Uri) null)
        throw new ArgumentNullException(nameof (storedProcedureUri));
      return this.ExecuteStoredProcedureAsync<TValue>(storedProcedureUri.OriginalString, options, cancellationToken, procedureParams);
    }

    internal Task<FeedResponse<Schema>> ReadSchemaFeedAsync(Uri schemasUri, FeedOptions options = null)
    {
      if (schemasUri == (Uri) null)
        throw new ArgumentNullException(nameof (schemasUri));
      return this.ReadSchemaFeedAsync(schemasUri.OriginalString, options);
    }

    public IOrderedQueryable<T> CreateAttachmentQuery<T>(
      Uri attachmentsUri,
      FeedOptions feedOptions = null)
    {
      if (attachmentsUri == (Uri) null)
        throw new ArgumentNullException(nameof (attachmentsUri));
      return this.CreateAttachmentQuery<T>(attachmentsUri.OriginalString, feedOptions);
    }

    public IQueryable<T> CreateAttachmentQuery<T>(
      Uri attachmentsUri,
      string sqlExpression,
      FeedOptions feedOptions = null)
    {
      if (attachmentsUri == (Uri) null)
        throw new ArgumentNullException(nameof (attachmentsUri));
      return this.CreateAttachmentQuery<T>(attachmentsUri, new SqlQuerySpec(sqlExpression), feedOptions);
    }

    public IQueryable<T> CreateAttachmentQuery<T>(
      Uri attachmentsUri,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null)
    {
      if (attachmentsUri == (Uri) null)
        throw new ArgumentNullException(nameof (attachmentsUri));
      return this.CreateAttachmentQuery<T>(attachmentsUri.OriginalString, querySpec, feedOptions);
    }

    public IOrderedQueryable<Attachment> CreateAttachmentQuery(
      Uri attachmentsUri,
      FeedOptions feedOptions = null)
    {
      if (attachmentsUri == (Uri) null)
        throw new ArgumentNullException(nameof (attachmentsUri));
      return this.CreateAttachmentQuery(attachmentsUri.OriginalString, feedOptions);
    }

    public IQueryable<object> CreateAttachmentQuery(
      Uri attachmentsUri,
      string sqlExpression,
      FeedOptions feedOptions = null)
    {
      if (attachmentsUri == (Uri) null)
        throw new ArgumentNullException(nameof (attachmentsUri));
      return this.CreateAttachmentQuery(attachmentsUri, new SqlQuerySpec(sqlExpression), feedOptions);
    }

    public IQueryable<object> CreateAttachmentQuery(
      Uri attachmentsUri,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null)
    {
      if (attachmentsUri == (Uri) null)
        throw new ArgumentNullException(nameof (attachmentsUri));
      return this.CreateAttachmentQuery(attachmentsUri.OriginalString, querySpec, feedOptions);
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

    public IOrderedQueryable<User> CreateUserQuery(Uri usersUri, FeedOptions feedOptions = null)
    {
      if (usersUri == (Uri) null)
        throw new ArgumentNullException(nameof (usersUri));
      return this.CreateUserQuery(usersUri.OriginalString, feedOptions);
    }

    public IQueryable<object> CreateUserQuery(
      Uri usersUri,
      string sqlExpression,
      FeedOptions feedOptions = null)
    {
      if (usersUri == (Uri) null)
        throw new ArgumentNullException(nameof (usersUri));
      return this.CreateUserQuery(usersUri, new SqlQuerySpec(sqlExpression), feedOptions);
    }

    public IQueryable<object> CreateUserQuery(
      Uri usersUri,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null)
    {
      if (usersUri == (Uri) null)
        throw new ArgumentNullException(nameof (usersUri));
      return this.CreateUserQuery(usersUri.OriginalString, querySpec, feedOptions);
    }

    public IOrderedQueryable<Permission> CreatePermissionQuery(
      Uri permissionsUri,
      FeedOptions feedOptions = null)
    {
      if (permissionsUri == (Uri) null)
        throw new ArgumentNullException(nameof (permissionsUri));
      return this.CreatePermissionQuery(permissionsUri.OriginalString, feedOptions);
    }

    public IQueryable<object> CreatePermissionQuery(
      Uri permissionsUri,
      string sqlExpression,
      FeedOptions feedOptions = null)
    {
      if (permissionsUri == (Uri) null)
        throw new ArgumentNullException(nameof (permissionsUri));
      return this.CreatePermissionQuery(permissionsUri, new SqlQuerySpec(sqlExpression), feedOptions);
    }

    public IQueryable<object> CreatePermissionQuery(
      Uri permissionsUri,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null)
    {
      if (permissionsUri == (Uri) null)
        throw new ArgumentNullException(nameof (permissionsUri));
      return this.CreatePermissionQuery(permissionsUri.OriginalString, querySpec, feedOptions);
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

    public IOrderedQueryable<T> CreateAttachmentQuery<T>(
      string documentLink,
      FeedOptions feedOptions = null)
    {
      return (IOrderedQueryable<T>) new DocumentQuery<T>(this, ResourceType.Attachment, typeof (Attachment), documentLink, feedOptions);
    }

    public IQueryable<T> CreateAttachmentQuery<T>(
      string documentLink,
      string sqlExpression,
      FeedOptions feedOptions = null)
    {
      return this.CreateAttachmentQuery<T>(documentLink, new SqlQuerySpec(sqlExpression), feedOptions);
    }

    public IQueryable<T> CreateAttachmentQuery<T>(
      string documentLink,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null)
    {
      return new DocumentQuery<T>(this, ResourceType.Attachment, typeof (Attachment), documentLink, feedOptions).AsSQL<T, T>(querySpec);
    }

    public IOrderedQueryable<Attachment> CreateAttachmentQuery(
      string documentLink,
      FeedOptions feedOptions = null)
    {
      return (IOrderedQueryable<Attachment>) new DocumentQuery<Attachment>(this, ResourceType.Attachment, typeof (Attachment), documentLink, feedOptions);
    }

    public IQueryable<object> CreateAttachmentQuery(
      string documentLink,
      string sqlExpression,
      FeedOptions feedOptions = null)
    {
      return this.CreateAttachmentQuery(documentLink, new SqlQuerySpec(sqlExpression), feedOptions);
    }

    public IQueryable<object> CreateAttachmentQuery(
      string documentLink,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null)
    {
      return new DocumentQuery<Attachment>(this, ResourceType.Attachment, typeof (Attachment), documentLink, feedOptions).AsSQL<Attachment>(querySpec);
    }

    public IOrderedQueryable<Database> CreateDatabaseQuery(FeedOptions feedOptions = null) => (IOrderedQueryable<Database>) new DocumentQuery<Database>(this, ResourceType.Database, typeof (Database), "//dbs/", feedOptions);

    public IQueryable<object> CreateDatabaseQuery(string sqlExpression, FeedOptions feedOptions = null) => this.CreateDatabaseQuery(new SqlQuerySpec(sqlExpression), feedOptions);

    public IQueryable<object> CreateDatabaseQuery(SqlQuerySpec querySpec, FeedOptions feedOptions = null) => new DocumentQuery<Database>(this, ResourceType.Database, typeof (Database), "//dbs/", feedOptions).AsSQL<Database>(querySpec);

    internal IDocumentQuery<Database> CreateDatabaseChangeFeedQuery(ChangeFeedOptions feedOptions)
    {
      DocumentClient.ValidateChangeFeedOptionsForNotPartitionedResource(feedOptions);
      return (IDocumentQuery<Database>) new ChangeFeedQuery<Database>(this, ResourceType.Database, (string) null, feedOptions);
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

    public IOrderedQueryable<User> CreateUserQuery(string usersLink, FeedOptions feedOptions = null) => (IOrderedQueryable<User>) new DocumentQuery<User>(this, ResourceType.User, typeof (User), usersLink, feedOptions);

    public IQueryable<object> CreateUserQuery(
      string usersLink,
      string sqlExpression,
      FeedOptions feedOptions = null)
    {
      return this.CreateUserQuery(usersLink, new SqlQuerySpec(sqlExpression), feedOptions);
    }

    public IQueryable<object> CreateUserQuery(
      string usersLink,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null)
    {
      return new DocumentQuery<User>(this, ResourceType.User, typeof (User), usersLink, feedOptions).AsSQL<User>(querySpec);
    }

    public IOrderedQueryable<Permission> CreatePermissionQuery(
      string permissionsLink,
      FeedOptions feedOptions = null)
    {
      return (IOrderedQueryable<Permission>) new DocumentQuery<Permission>(this, ResourceType.Permission, typeof (Permission), permissionsLink, feedOptions);
    }

    public IQueryable<object> CreatePermissionQuery(
      string permissionsLink,
      string sqlExpression,
      FeedOptions feedOptions = null)
    {
      return this.CreatePermissionQuery(permissionsLink, new SqlQuerySpec(sqlExpression), feedOptions);
    }

    public IQueryable<object> CreatePermissionQuery(
      string permissionsLink,
      SqlQuerySpec querySpec,
      FeedOptions feedOptions = null)
    {
      return new DocumentQuery<Permission>(this, ResourceType.Permission, typeof (Permission), permissionsLink, feedOptions).AsSQL<Permission>(querySpec);
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
