// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Fluent.CosmosClientBuilder
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Azure;
using Azure.Core;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Fluent
{
  public class CosmosClientBuilder
  {
    private readonly CosmosClientOptions clientOptions = new CosmosClientOptions();
    private readonly string accountEndpoint;
    private readonly string accountKey;
    private readonly AzureKeyCredential azureKeyCredential;
    private readonly TokenCredential tokenCredential;

    public CosmosClientBuilder(string accountEndpoint, string authKeyOrResourceToken)
    {
      if (string.IsNullOrEmpty(accountEndpoint))
        throw new ArgumentNullException(nameof (accountEndpoint));
      if (string.IsNullOrEmpty(authKeyOrResourceToken))
        throw new ArgumentNullException(nameof (authKeyOrResourceToken));
      this.accountEndpoint = accountEndpoint;
      this.accountKey = authKeyOrResourceToken;
    }

    public CosmosClientBuilder(
      string accountEndpoint,
      AzureKeyCredential authKeyOrResourceTokenCredential)
    {
      this.accountEndpoint = !string.IsNullOrEmpty(accountEndpoint) ? accountEndpoint : throw new ArgumentNullException(nameof (accountEndpoint));
      this.azureKeyCredential = authKeyOrResourceTokenCredential ?? throw new ArgumentNullException(nameof (authKeyOrResourceTokenCredential));
    }

    public CosmosClientBuilder(string connectionString)
    {
      this.accountEndpoint = connectionString != null ? CosmosClientOptions.GetAccountEndpoint(connectionString) : throw new ArgumentNullException(nameof (connectionString));
      this.accountKey = CosmosClientOptions.GetAccountKey(connectionString);
    }

    public CosmosClientBuilder(string accountEndpoint, TokenCredential tokenCredential)
    {
      this.accountEndpoint = !string.IsNullOrEmpty(accountEndpoint) ? accountEndpoint : throw new ArgumentNullException(nameof (accountEndpoint));
      this.tokenCredential = tokenCredential ?? throw new ArgumentNullException(nameof (tokenCredential));
    }

    public CosmosClient Build()
    {
      DefaultTrace.TraceInformation("CosmosClientBuilder.Build with configuration: " + this.clientOptions.GetSerializedConfiguration());
      if (this.tokenCredential != null)
        return new CosmosClient(this.accountEndpoint, this.tokenCredential, this.clientOptions);
      return this.azureKeyCredential != null ? new CosmosClient(this.accountEndpoint, this.azureKeyCredential, this.clientOptions) : new CosmosClient(this.accountEndpoint, this.accountKey, this.clientOptions);
    }

    public Task<CosmosClient> BuildAndInitializeAsync(
      IReadOnlyList<(string databaseId, string containerId)> containers,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (this.tokenCredential != null)
        return CosmosClient.CreateAndInitializeAsync(this.accountEndpoint, this.tokenCredential, containers, this.clientOptions, cancellationToken);
      return this.azureKeyCredential != null ? CosmosClient.CreateAndInitializeAsync(this.accountEndpoint, this.azureKeyCredential, containers, this.clientOptions, cancellationToken) : CosmosClient.CreateAndInitializeAsync(this.accountEndpoint, this.accountKey, containers, this.clientOptions, cancellationToken);
    }

    internal virtual CosmosClient Build(DocumentClient documentClient)
    {
      DefaultTrace.TraceInformation("CosmosClientBuilder.Build(DocumentClient) with configuration: " + this.clientOptions.GetSerializedConfiguration());
      return new CosmosClient(this.accountEndpoint, this.accountKey, this.clientOptions, documentClient);
    }

    public CosmosClientBuilder WithApplicationName(string applicationName)
    {
      this.clientOptions.ApplicationName = applicationName;
      return this;
    }

    public CosmosClientBuilder WithApplicationRegion(string applicationRegion)
    {
      this.clientOptions.ApplicationRegion = applicationRegion;
      return this;
    }

    public CosmosClientBuilder WithApplicationPreferredRegions(
      IReadOnlyList<string> applicationPreferredRegions)
    {
      this.clientOptions.ApplicationPreferredRegions = applicationPreferredRegions;
      return this;
    }

    public CosmosClientBuilder WithLimitToEndpoint(bool limitToEndpoint)
    {
      this.clientOptions.LimitToEndpoint = limitToEndpoint;
      return this;
    }

    public CosmosClientBuilder WithRequestTimeout(TimeSpan requestTimeout)
    {
      this.clientOptions.RequestTimeout = requestTimeout;
      return this;
    }

    public CosmosClientBuilder WithConnectionModeDirect()
    {
      this.clientOptions.ConnectionMode = ConnectionMode.Direct;
      this.clientOptions.ConnectionProtocol = Protocol.Tcp;
      return this;
    }

    public CosmosClientBuilder WithConnectionModeDirect(
      TimeSpan? idleTcpConnectionTimeout = null,
      TimeSpan? openTcpConnectionTimeout = null,
      int? maxRequestsPerTcpConnection = null,
      int? maxTcpConnectionsPerEndpoint = null,
      Microsoft.Azure.Cosmos.PortReuseMode? portReuseMode = null,
      bool? enableTcpConnectionEndpointRediscovery = null)
    {
      this.clientOptions.IdleTcpConnectionTimeout = idleTcpConnectionTimeout;
      this.clientOptions.OpenTcpConnectionTimeout = openTcpConnectionTimeout;
      this.clientOptions.MaxRequestsPerTcpConnection = maxRequestsPerTcpConnection;
      this.clientOptions.MaxTcpConnectionsPerEndpoint = maxTcpConnectionsPerEndpoint;
      this.clientOptions.PortReuseMode = portReuseMode;
      if (enableTcpConnectionEndpointRediscovery.HasValue)
        this.clientOptions.EnableTcpConnectionEndpointRediscovery = enableTcpConnectionEndpointRediscovery.Value;
      this.clientOptions.ConnectionMode = ConnectionMode.Direct;
      this.clientOptions.ConnectionProtocol = Protocol.Tcp;
      return this;
    }

    public CosmosClientBuilder WithConsistencyLevel(Microsoft.Azure.Cosmos.ConsistencyLevel consistencyLevel)
    {
      this.clientOptions.ConsistencyLevel = new Microsoft.Azure.Cosmos.ConsistencyLevel?(consistencyLevel);
      return this;
    }

    internal CosmosClientBuilder WithDistributingTracing(DistributedTracingOptions options)
    {
      this.clientOptions.DistributedTracingOptions = options;
      return this;
    }

    public CosmosClientBuilder WithConnectionModeGateway(
      int? maxConnectionLimit = null,
      IWebProxy webProxy = null)
    {
      this.clientOptions.ConnectionMode = ConnectionMode.Gateway;
      this.clientOptions.ConnectionProtocol = Protocol.Https;
      if (maxConnectionLimit.HasValue)
        this.clientOptions.GatewayModeMaxConnectionLimit = maxConnectionLimit.Value;
      this.clientOptions.WebProxy = webProxy;
      return this;
    }

    public CosmosClientBuilder AddCustomHandlers(params RequestHandler[] customHandlers)
    {
      foreach (RequestHandler customHandler in customHandlers)
      {
        if (customHandler.InnerHandler != null)
          throw new ArgumentException("customHandlers requires all DelegatingHandler.InnerHandler to be null. The CosmosClient uses the inner handler in building the pipeline.");
        this.clientOptions.CustomHandlers.Add(customHandler);
      }
      return this;
    }

    public CosmosClientBuilder WithThrottlingRetryOptions(
      TimeSpan maxRetryWaitTimeOnThrottledRequests,
      int maxRetryAttemptsOnThrottledRequests)
    {
      this.clientOptions.MaxRetryWaitTimeOnRateLimitedRequests = new TimeSpan?(maxRetryWaitTimeOnThrottledRequests);
      this.clientOptions.MaxRetryAttemptsOnRateLimitedRequests = new int?(maxRetryAttemptsOnThrottledRequests);
      return this;
    }

    public CosmosClientBuilder WithSerializerOptions(
      CosmosSerializationOptions cosmosSerializerOptions)
    {
      this.clientOptions.SerializerOptions = cosmosSerializerOptions;
      return this;
    }

    public CosmosClientBuilder WithCustomSerializer(CosmosSerializer cosmosJsonSerializer)
    {
      this.clientOptions.Serializer = cosmosJsonSerializer;
      return this;
    }

    public CosmosClientBuilder WithBulkExecution(bool enabled)
    {
      this.clientOptions.AllowBulkExecution = enabled;
      return this;
    }

    public CosmosClientBuilder WithHttpClientFactory(Func<HttpClient> httpClientFactory)
    {
      CosmosClientOptions clientOptions = this.clientOptions;
      clientOptions.HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof (httpClientFactory));
      return this;
    }

    public CosmosClientBuilder WithContentResponseOnWrite(bool contentResponseOnWrite)
    {
      this.clientOptions.EnableContentResponseOnWrite = new bool?(contentResponseOnWrite);
      return this;
    }

    internal CosmosClientBuilder WithSendingRequestEventArgs(
      EventHandler<SendingRequestEventArgs> sendingRequestEventArgs)
    {
      this.clientOptions.SendingRequestEventArgs = sendingRequestEventArgs;
      return this;
    }

    internal CosmosClientBuilder WithSessionContainer(ISessionContainer sessionContainer)
    {
      this.clientOptions.SessionContainer = sessionContainer;
      return this;
    }

    internal CosmosClientBuilder WithTransportClientHandlerFactory(
      Func<TransportClient, TransportClient> transportClientHandlerFactory)
    {
      this.clientOptions.TransportClientHandlerFactory = transportClientHandlerFactory;
      return this;
    }

    internal CosmosClientBuilder WithApiType(ApiType apiType)
    {
      this.clientOptions.ApiType = apiType;
      return this;
    }

    internal CosmosClientBuilder WithStoreClientFactory(IStoreClientFactory storeClientFactory)
    {
      this.clientOptions.StoreClientFactory = storeClientFactory;
      return this;
    }

    internal CosmosClientBuilder WithCpuMonitorDisabled()
    {
      this.clientOptions.EnableCpuMonitor = new bool?(false);
      return this;
    }

    internal CosmosClientBuilder WithTelemetryDisabled()
    {
      this.clientOptions.EnableClientTelemetry = new bool?(false);
      return this;
    }

    internal CosmosClientBuilder WithTelemetryEnabled()
    {
      this.clientOptions.EnableClientTelemetry = new bool?(true);
      return this;
    }

    internal CosmosClientBuilder WithPartitionLevelFailoverEnabled()
    {
      this.clientOptions.EnablePartitionLevelFailover = true;
      return this;
    }

    internal CosmosClientBuilder AllowUpgradeConsistencyToLocalQuorum()
    {
      this.clientOptions.EnableUpgradeConsistencyToLocalQuorum = true;
      return this;
    }

    internal CosmosClientBuilder WithRetryWithOptions(
      int? initialRetryForRetryWithMilliseconds,
      int? maximumRetryForRetryWithMilliseconds,
      int? randomSaltForRetryWithMilliseconds,
      int? totalWaitTimeForRetryWithMilliseconds)
    {
      this.clientOptions.InitialRetryForRetryWithMilliseconds = initialRetryForRetryWithMilliseconds;
      this.clientOptions.MaximumRetryForRetryWithMilliseconds = maximumRetryForRetryWithMilliseconds;
      this.clientOptions.RandomSaltForRetryWithMilliseconds = randomSaltForRetryWithMilliseconds;
      this.clientOptions.TotalWaitTimeForRetryWithMilliseconds = totalWaitTimeForRetryWithMilliseconds;
      return this;
    }
  }
}
