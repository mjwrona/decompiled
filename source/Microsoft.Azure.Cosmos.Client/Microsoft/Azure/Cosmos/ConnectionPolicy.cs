// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ConnectionPolicy
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Telemetry;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net.Http;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class ConnectionPolicy
  {
    private const int defaultRequestTimeout = 10;
    private const int defaultMediaRequestTimeout = 300;
    private const int defaultMaxConcurrentFanoutRequests = 32;
    private const int defaultMaxConcurrentConnectionLimit = 50;
    internal UserAgentContainer UserAgentContainer;
    private static ConnectionPolicy defaultPolicy;
    private Protocol connectionProtocol;
    private ObservableCollection<string> preferredLocations;

    public ConnectionPolicy()
    {
      this.connectionProtocol = Protocol.Https;
      this.RequestTimeout = TimeSpan.FromSeconds(10.0);
      this.MediaRequestTimeout = TimeSpan.FromSeconds(300.0);
      this.ConnectionMode = ConnectionMode.Gateway;
      this.MaxConcurrentFanoutRequests = 32;
      this.MediaReadMode = MediaReadMode.Buffered;
      this.UserAgentContainer = new UserAgentContainer(0);
      this.preferredLocations = new ObservableCollection<string>();
      this.EnableEndpointDiscovery = true;
      this.MaxConnectionLimit = 50;
      this.RetryOptions = new RetryOptions();
      this.EnableReadRequestsFallback = new bool?();
      this.EnableClientTelemetry = ClientTelemetryOptions.IsClientTelemetryEnabled();
    }

    public void SetCurrentLocation(string location)
    {
      List<string> stringList = RegionProximityUtil.SourceRegionToTargetRegionsRTTInMs.ContainsKey(location) ? RegionProximityUtil.GeneratePreferredRegionList(location) : throw new ArgumentException("ApplicationRegion configuration '" + location + "' is not a valid Azure region or the current SDK version does not recognize it. If the value represents a valid region, make sure you are using the latest SDK version.");
      if (stringList == null)
        return;
      this.preferredLocations.Clear();
      foreach (string str in stringList)
        this.preferredLocations.Add(str);
    }

    public void SetPreferredLocations(IReadOnlyList<string> regions)
    {
      if (regions == null)
        throw new ArgumentNullException(nameof (regions));
      this.preferredLocations.Clear();
      foreach (string region in (IEnumerable<string>) regions)
        this.preferredLocations.Add(region);
    }

    internal int MaxConcurrentFanoutRequests { get; set; }

    public TimeSpan RequestTimeout { get; set; }

    public TimeSpan MediaRequestTimeout { get; set; }

    public ConnectionMode ConnectionMode { get; set; }

    public MediaReadMode MediaReadMode { get; set; }

    public Protocol ConnectionProtocol
    {
      get => this.connectionProtocol;
      set => this.connectionProtocol = value == Protocol.Https || value == Protocol.Tcp ? value : throw new ArgumentOutOfRangeException(nameof (value));
    }

    public bool? EnableReadRequestsFallback { get; set; }

    public bool EnableTcpConnectionEndpointRediscovery { get; set; }

    internal bool EnableClientTelemetry { get; set; }

    public static ConnectionPolicy Default
    {
      get
      {
        if (ConnectionPolicy.defaultPolicy == null)
          ConnectionPolicy.defaultPolicy = new ConnectionPolicy();
        return ConnectionPolicy.defaultPolicy;
      }
    }

    public string UserAgentSuffix
    {
      get => this.UserAgentContainer.Suffix;
      set => this.UserAgentContainer.Suffix = value;
    }

    public Collection<string> PreferredLocations => (Collection<string>) this.preferredLocations;

    public bool EnableEndpointDiscovery { get; set; }

    public bool EnablePartitionLevelFailover { get; set; }

    public bool UseMultipleWriteLocations { get; set; }

    public int MaxConnectionLimit { get; set; }

    public RetryOptions RetryOptions { get; set; }

    public TimeSpan? IdleTcpConnectionTimeout { get; set; }

    public TimeSpan? OpenTcpConnectionTimeout { get; set; }

    public int? MaxRequestsPerTcpConnection { get; set; }

    public int? MaxTcpConnectionsPerEndpoint { get; set; }

    public Microsoft.Azure.Cosmos.PortReuseMode? PortReuseMode { get; set; }

    public Func<HttpClient> HttpClientFactory { get; set; }

    internal int? MaxTcpPartitionCount { get; set; }

    internal event NotifyCollectionChangedEventHandler PreferenceChanged
    {
      add => this.preferredLocations.CollectionChanged += value;
      remove => this.preferredLocations.CollectionChanged -= value;
    }

    internal RetryWithConfiguration GetRetryWithConfiguration() => this.RetryOptions?.GetRetryWithConfiguration();
  }
}
