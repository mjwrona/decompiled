// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Client.ConnectionPolicy
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Microsoft.Azure.Documents.Client
{
  public sealed class ConnectionPolicy
  {
    internal UserAgentContainer UserAgentContainer;
    private const int defaultRequestTimeout = 10;
    private const int defaultMediaRequestTimeout = 300;
    private const int defaultMaxConcurrentFanoutRequests = 32;
    private const int defaultMaxConcurrentConnectionLimit = 50;
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
      this.UserAgentContainer = new UserAgentContainer();
      this.preferredLocations = new ObservableCollection<string>();
      this.EnableEndpointDiscovery = true;
      this.MaxConnectionLimit = 50;
      this.RetryOptions = new RetryOptions();
      this.EnableReadRequestsFallback = new bool?();
    }

    public void SetCurrentLocation(string location)
    {
      List<string> stringList = RegionProximityUtil.SourceRegionToTargetRegionsRTTInMs.ContainsKey(location) ? RegionProximityUtil.GeneratePreferredRegionList(location) : throw new ArgumentException("Current location is not a valid Azure region.");
      if (stringList == null)
        return;
      this.preferredLocations.Clear();
      foreach (string str in stringList)
        this.preferredLocations.Add(str);
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

    public bool UseMultipleWriteLocations { get; set; }

    public int MaxConnectionLimit { get; set; }

    public RetryOptions RetryOptions { get; set; }

    public TimeSpan? IdleTcpConnectionTimeout { get; set; }

    public TimeSpan? OpenTcpConnectionTimeout { get; set; }

    public int? MaxRequestsPerTcpConnection { get; set; }

    public int? MaxTcpConnectionsPerEndpoint { get; set; }

    public Microsoft.Azure.Documents.PortReuseMode? PortReuseMode { get; set; }

    public bool EnableTcpConnectionEndpointRediscovery { get; set; }

    internal int? MaxTcpPartitionCount { get; set; }

    internal event NotifyCollectionChangedEventHandler PreferenceChanged
    {
      add => this.preferredLocations.CollectionChanged += value;
      remove => this.preferredLocations.CollectionChanged -= value;
    }

    internal RetryWithConfiguration GetRetryWithConfiguration() => this.RetryOptions?.GetRetryWithConfiguration();
  }
}
