// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosClientOptions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Microsoft.Azure.Cosmos
{
  public class CosmosClientOptions
  {
    private const ConnectionMode DefaultConnectionMode = ConnectionMode.Direct;
    private const Protocol DefaultProtocol = Protocol.Tcp;
    private const string ConnectionStringAccountEndpoint = "AccountEndpoint";
    private const string ConnectionStringAccountKey = "AccountKey";
    private const ApiType DefaultApiType = ApiType.None;
    private int gatewayModeMaxConnectionLimit;
    private CosmosSerializationOptions serializerOptions;
    private CosmosSerializer serializerInternal;
    private ConnectionMode connectionMode;
    private Protocol connectionProtocol;
    private TimeSpan? idleTcpConnectionTimeout;
    private TimeSpan? openTcpConnectionTimeout;
    private int? maxRequestsPerTcpConnection;
    private int? maxTcpConnectionsPerEndpoint;
    private Microsoft.Azure.Cosmos.PortReuseMode? portReuseMode;
    private IWebProxy webProxy;
    private Func<HttpClient> httpClientFactory;
    private string applicationName;

    public CosmosClientOptions()
    {
      this.GatewayModeMaxConnectionLimit = ConnectionPolicy.Default.MaxConnectionLimit;
      this.RequestTimeout = ConnectionPolicy.Default.RequestTimeout;
      this.TokenCredentialBackgroundRefreshInterval = new TimeSpan?();
      this.ConnectionMode = ConnectionMode.Direct;
      this.ConnectionProtocol = Protocol.Tcp;
      this.ApiType = ApiType.None;
      this.CustomHandlers = new Collection<RequestHandler>();
    }

    public string ApplicationName
    {
      get => this.applicationName;
      set
      {
        try
        {
          new HttpRequestMessage().Headers.Add("User-Agent", value);
        }
        catch (FormatException ex)
        {
          throw new ArgumentException("Application name '" + value + "' is invalid.", (Exception) ex);
        }
        this.applicationName = value;
      }
    }

    internal ISessionContainer SessionContainer { get; set; }

    public string ApplicationRegion { get; set; }

    public IReadOnlyList<string> ApplicationPreferredRegions { get; set; }

    public int GatewayModeMaxConnectionLimit
    {
      get => this.gatewayModeMaxConnectionLimit;
      set
      {
        if (value <= 0)
          throw new ArgumentOutOfRangeException(nameof (value));
        if (this.HttpClientFactory != null && value != ConnectionPolicy.Default.MaxConnectionLimit)
          throw new ArgumentException("httpClientFactory can not be set along with GatewayModeMaxConnectionLimit. This must be set on the HttpClientHandler.MaxConnectionsPerServer property.");
        this.gatewayModeMaxConnectionLimit = value;
      }
    }

    public TimeSpan RequestTimeout { get; set; }

    public TimeSpan? TokenCredentialBackgroundRefreshInterval { get; set; }

    [JsonConverter(typeof (CosmosClientOptions.ClientOptionJsonConverter))]
    public Collection<RequestHandler> CustomHandlers { get; }

    public ConnectionMode ConnectionMode
    {
      get => this.connectionMode;
      set
      {
        switch (value)
        {
          case ConnectionMode.Gateway:
            this.ConnectionProtocol = Protocol.Https;
            break;
          case ConnectionMode.Direct:
            this.connectionProtocol = Protocol.Tcp;
            break;
        }
        this.ValidateDirectTCPSettings();
        this.connectionMode = value;
      }
    }

    public Microsoft.Azure.Cosmos.ConsistencyLevel? ConsistencyLevel { get; set; }

    public int? MaxRetryAttemptsOnRateLimitedRequests { get; set; }

    public TimeSpan? MaxRetryWaitTimeOnRateLimitedRequests { get; set; }

    public bool? EnableContentResponseOnWrite { get; set; }

    public TimeSpan? IdleTcpConnectionTimeout
    {
      get => this.idleTcpConnectionTimeout;
      set
      {
        this.idleTcpConnectionTimeout = value;
        this.ValidateDirectTCPSettings();
      }
    }

    public TimeSpan? OpenTcpConnectionTimeout
    {
      get => this.openTcpConnectionTimeout;
      set
      {
        this.openTcpConnectionTimeout = value;
        this.ValidateDirectTCPSettings();
      }
    }

    public int? MaxRequestsPerTcpConnection
    {
      get => this.maxRequestsPerTcpConnection;
      set
      {
        this.maxRequestsPerTcpConnection = value;
        this.ValidateDirectTCPSettings();
      }
    }

    public int? MaxTcpConnectionsPerEndpoint
    {
      get => this.maxTcpConnectionsPerEndpoint;
      set
      {
        this.maxTcpConnectionsPerEndpoint = value;
        this.ValidateDirectTCPSettings();
      }
    }

    public Microsoft.Azure.Cosmos.PortReuseMode? PortReuseMode
    {
      get => this.portReuseMode;
      set
      {
        this.portReuseMode = value;
        this.ValidateDirectTCPSettings();
      }
    }

    [JsonIgnore]
    public IWebProxy WebProxy
    {
      get => this.webProxy;
      set => this.webProxy = value == null || this.HttpClientFactory == null ? value : throw new ArgumentException("WebProxy cannot be set along HttpClientFactory");
    }

    public CosmosSerializationOptions SerializerOptions
    {
      get => this.serializerOptions;
      set
      {
        if (this.Serializer != null)
          throw new ArgumentException("SerializerOptions is not compatible with Serializer. Only one can be set.  ");
        this.serializerOptions = value;
      }
    }

    [JsonConverter(typeof (CosmosClientOptions.ClientOptionJsonConverter))]
    public CosmosSerializer Serializer
    {
      get => this.serializerInternal;
      set
      {
        if (this.SerializerOptions != null)
          throw new ArgumentException("Serializer is not compatible with SerializerOptions. Only one can be set.  ");
        this.serializerInternal = value;
      }
    }

    public bool LimitToEndpoint { get; set; }

    public bool AllowBulkExecution { get; set; }

    public bool EnableTcpConnectionEndpointRediscovery { get; set; } = true;

    [JsonIgnore]
    public Func<HttpClient> HttpClientFactory
    {
      get => this.httpClientFactory;
      set
      {
        if (value != null && this.WebProxy != null)
          throw new ArgumentException("HttpClientFactory cannot be set along WebProxy");
        if (this.GatewayModeMaxConnectionLimit != ConnectionPolicy.Default.MaxConnectionLimit)
          throw new ArgumentException("httpClientFactory can not be set along with GatewayModeMaxConnectionLimit. This must be set on the HttpClientHandler.MaxConnectionsPerServer property.");
        this.httpClientFactory = value;
      }
    }

    internal bool EnablePartitionLevelFailover { get; set; }

    internal bool EnableUpgradeConsistencyToLocalQuorum { get; set; }

    internal Protocol ConnectionProtocol
    {
      get => this.connectionProtocol;
      set
      {
        this.ValidateDirectTCPSettings();
        this.connectionProtocol = value;
      }
    }

    internal EventHandler<Microsoft.Azure.Documents.SendingRequestEventArgs> SendingRequestEventArgs { get; set; }

    internal Func<TransportClient, TransportClient> TransportClientHandlerFactory { get; set; }

    internal ApiType ApiType { get; set; }

    internal IStoreClientFactory StoreClientFactory { get; set; }

    internal int? InitialRetryForRetryWithMilliseconds { get; set; }

    internal int? MaximumRetryForRetryWithMilliseconds { get; set; }

    internal int? RandomSaltForRetryWithMilliseconds { get; set; }

    internal int? TotalWaitTimeForRetryWithMilliseconds { get; set; }

    internal bool? EnableCpuMonitor { get; set; }

    internal bool? EnableClientTelemetry { get; set; }

    internal void SetSerializerIfNotConfigured(CosmosSerializer serializer)
    {
      if (this.serializerInternal != null)
        return;
      this.serializerInternal = serializer ?? throw new ArgumentNullException(nameof (serializer));
    }

    internal CosmosClientOptions Clone() => (CosmosClientOptions) this.MemberwiseClone();

    internal virtual ConnectionPolicy GetConnectionPolicy(int clientId)
    {
      this.ValidateDirectTCPSettings();
      this.ValidateLimitToEndpointSettings();
      ConnectionPolicy connectionPolicy = new ConnectionPolicy()
      {
        MaxConnectionLimit = this.GatewayModeMaxConnectionLimit,
        RequestTimeout = this.RequestTimeout,
        ConnectionMode = this.ConnectionMode,
        ConnectionProtocol = this.ConnectionProtocol,
        UserAgentContainer = this.CreateUserAgentContainerWithFeatures(clientId),
        UseMultipleWriteLocations = true,
        IdleTcpConnectionTimeout = this.IdleTcpConnectionTimeout,
        OpenTcpConnectionTimeout = this.OpenTcpConnectionTimeout,
        MaxRequestsPerTcpConnection = this.MaxRequestsPerTcpConnection,
        MaxTcpConnectionsPerEndpoint = this.MaxTcpConnectionsPerEndpoint,
        EnableEndpointDiscovery = !this.LimitToEndpoint,
        EnablePartitionLevelFailover = this.EnablePartitionLevelFailover,
        PortReuseMode = this.portReuseMode,
        EnableTcpConnectionEndpointRediscovery = this.EnableTcpConnectionEndpointRediscovery,
        HttpClientFactory = this.httpClientFactory
      };
      if (this.EnableClientTelemetry.HasValue)
        connectionPolicy.EnableClientTelemetry = this.EnableClientTelemetry.Value;
      if (this.ApplicationRegion != null)
        connectionPolicy.SetCurrentLocation(this.ApplicationRegion);
      if (this.ApplicationPreferredRegions != null)
        connectionPolicy.SetPreferredLocations(this.ApplicationPreferredRegions);
      int? nullable = this.MaxRetryAttemptsOnRateLimitedRequests;
      if (nullable.HasValue)
      {
        RetryOptions retryOptions = connectionPolicy.RetryOptions;
        nullable = this.MaxRetryAttemptsOnRateLimitedRequests;
        int num = nullable.Value;
        retryOptions.MaxRetryAttemptsOnThrottledRequests = num;
      }
      TimeSpan? rateLimitedRequests = this.MaxRetryWaitTimeOnRateLimitedRequests;
      if (rateLimitedRequests.HasValue)
      {
        RetryOptions retryOptions = connectionPolicy.RetryOptions;
        rateLimitedRequests = this.MaxRetryWaitTimeOnRateLimitedRequests;
        int totalSeconds = (int) rateLimitedRequests.Value.TotalSeconds;
        retryOptions.MaxRetryWaitTimeInSeconds = totalSeconds;
      }
      nullable = this.InitialRetryForRetryWithMilliseconds;
      if (nullable.HasValue)
        connectionPolicy.RetryOptions.InitialRetryForRetryWithMilliseconds = this.InitialRetryForRetryWithMilliseconds;
      nullable = this.MaximumRetryForRetryWithMilliseconds;
      if (nullable.HasValue)
        connectionPolicy.RetryOptions.MaximumRetryForRetryWithMilliseconds = this.MaximumRetryForRetryWithMilliseconds;
      nullable = this.RandomSaltForRetryWithMilliseconds;
      if (nullable.HasValue)
        connectionPolicy.RetryOptions.RandomSaltForRetryWithMilliseconds = this.RandomSaltForRetryWithMilliseconds;
      nullable = this.TotalWaitTimeForRetryWithMilliseconds;
      if (nullable.HasValue)
        connectionPolicy.RetryOptions.TotalWaitTimeForRetryWithMilliseconds = this.TotalWaitTimeForRetryWithMilliseconds;
      return connectionPolicy;
    }

    internal Microsoft.Azure.Documents.ConsistencyLevel? GetDocumentsConsistencyLevel() => !this.ConsistencyLevel.HasValue ? new Microsoft.Azure.Documents.ConsistencyLevel?() : new Microsoft.Azure.Documents.ConsistencyLevel?((Microsoft.Azure.Documents.ConsistencyLevel) this.ConsistencyLevel.Value);

    internal static string GetAccountEndpoint(string connectionString) => CosmosClientOptions.GetValueFromConnectionString(connectionString, "AccountEndpoint");

    internal static string GetAccountKey(string connectionString) => CosmosClientOptions.GetValueFromConnectionString(connectionString, "AccountKey");

    private static string GetValueFromConnectionString(string connectionString, string keyName)
    {
      if (connectionString == null)
        throw new ArgumentNullException(nameof (connectionString));
      object obj;
      if (new DbConnectionStringBuilder()
      {
        ConnectionString = connectionString
      }.TryGetValue(keyName, out obj))
      {
        string connectionString1 = obj as string;
        if (!string.IsNullOrEmpty(connectionString1))
          return connectionString1;
      }
      throw new ArgumentException("The connection string is missing a required property: " + keyName);
    }

    private void ValidateLimitToEndpointSettings()
    {
      if (!string.IsNullOrEmpty(this.ApplicationRegion) && this.LimitToEndpoint)
        throw new ArgumentException("Cannot specify ApplicationRegion and enable LimitToEndpoint. Only one can be set.");
      IReadOnlyList<string> preferredRegions1 = this.ApplicationPreferredRegions;
      if ((preferredRegions1 != null ? (preferredRegions1.Count > 0 ? 1 : 0) : 0) != 0 && this.LimitToEndpoint)
        throw new ArgumentException("Cannot specify ApplicationPreferredRegions and enable LimitToEndpoint. Only one can be set.");
      if (string.IsNullOrEmpty(this.ApplicationRegion))
        return;
      IReadOnlyList<string> preferredRegions2 = this.ApplicationPreferredRegions;
      if ((preferredRegions2 != null ? (preferredRegions2.Count > 0 ? 1 : 0) : 0) != 0)
        throw new ArgumentException("Cannot specify ApplicationPreferredRegions and ApplicationRegion. Only one can be set.");
    }

    private void ValidateDirectTCPSettings()
    {
      string str = string.Empty;
      if (this.ConnectionMode != ConnectionMode.Direct)
      {
        if (this.IdleTcpConnectionTimeout.HasValue)
          str = "IdleTcpConnectionTimeout";
        else if (this.OpenTcpConnectionTimeout.HasValue)
          str = "OpenTcpConnectionTimeout";
        else if (this.MaxRequestsPerTcpConnection.HasValue)
          str = "MaxRequestsPerTcpConnection";
        else if (this.MaxTcpConnectionsPerEndpoint.HasValue)
          str = "MaxTcpConnectionsPerEndpoint";
        else if (this.PortReuseMode.HasValue)
          str = "PortReuseMode";
      }
      if (!string.IsNullOrEmpty(str))
        throw new ArgumentException(str + " requires ConnectionMode to be set to Direct");
    }

    internal UserAgentContainer CreateUserAgentContainerWithFeatures(int clientId)
    {
      CosmosClientOptionsFeatures clientOptionsFeatures = CosmosClientOptionsFeatures.NoFeatures;
      if (this.AllowBulkExecution)
        clientOptionsFeatures |= CosmosClientOptionsFeatures.AllowBulkExecution;
      if (this.HttpClientFactory != null)
        clientOptionsFeatures |= CosmosClientOptionsFeatures.HttpClientFactory;
      string features = (string) null;
      if (clientOptionsFeatures != CosmosClientOptionsFeatures.NoFeatures)
        features = Convert.ToString((int) clientOptionsFeatures, 2).PadLeft(8, '0');
      string regionConfiguration = this.GetRegionConfiguration();
      return new UserAgentContainer(clientId, features, regionConfiguration, this.ApplicationName);
    }

    private string GetRegionConfiguration()
    {
      string str = this.LimitToEndpoint ? "D" : string.Empty;
      if (!string.IsNullOrEmpty(this.ApplicationRegion))
        return str + "S";
      return this.ApplicationPreferredRegions != null ? str + "L" : str + "N";
    }

    internal string GetSerializedConfiguration() => JsonConvert.SerializeObject((object) this);

    internal DistributedTracingOptions DistributedTracingOptions { get; set; }

    internal bool EnableDistributedTracing { get; set; }

    private class ClientOptionJsonConverter : JsonConverter
    {
      public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
      {
        if (value is Collection<RequestHandler> source)
        {
          writer.WriteValue(string.Join<Type>(":", source.Select<RequestHandler, Type>((Func<RequestHandler, Type>) (x => x.GetType()))));
        }
        else
        {
          CosmosJsonSerializerWrapper serializerWrapper = value as CosmosJsonSerializerWrapper;
          if (value is CosmosJsonSerializerWrapper)
            writer.WriteValue(serializerWrapper.InternalJsonSerializer.GetType().ToString());
          if (!(value is CosmosSerializer cosmosSerializer))
            return;
          writer.WriteValue(cosmosSerializer.GetType().ToString());
        }
      }

      public override object ReadJson(
        JsonReader reader,
        Type objectType,
        object existingValue,
        JsonSerializer serializer)
      {
        throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
      }

      public override bool CanRead => false;

      public override bool CanConvert(Type objectType) => objectType == typeof (DateTime);
    }
  }
}
