// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tracing.TraceData.RntbdConnectionConfig
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Microsoft.Azure.Cosmos.Tracing.TraceData
{
  internal class RntbdConnectionConfig
  {
    private readonly Lazy<string> lazyString;
    private readonly Lazy<string> lazyJsonString;

    public RntbdConnectionConfig(
      int connectionTimeout,
      int idleConnectionTimeout,
      int maxRequestsPerChannel,
      int maxRequestsPerEndpoint,
      bool tcpEndpointRediscovery,
      PortReuseMode portReuseMode)
    {
      RntbdConnectionConfig connectionConfig = this;
      this.ConnectionTimeout = connectionTimeout;
      this.IdleConnectionTimeout = idleConnectionTimeout;
      this.MaxRequestsPerChannel = maxRequestsPerChannel;
      this.MaxRequestsPerEndpoint = maxRequestsPerEndpoint;
      this.TcpEndpointRediscovery = tcpEndpointRediscovery;
      this.PortReuseMode = portReuseMode;
      this.lazyString = new Lazy<string>((Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "(cto: {0}, icto: {1}, mrpc: {2}, mcpe: {3}, erd: {4}, pr: {5})", (object) connectionTimeout, (object) idleConnectionTimeout, (object) maxRequestsPerChannel, (object) maxRequestsPerEndpoint, (object) tcpEndpointRediscovery, (object) portReuseMode.ToString())));
      this.lazyJsonString = new Lazy<string>((Func<string>) (() => JsonConvert.SerializeObject((object) connectionConfig)));
    }

    public int ConnectionTimeout { get; }

    public int IdleConnectionTimeout { get; }

    public int MaxRequestsPerChannel { get; }

    public int MaxRequestsPerEndpoint { get; }

    public bool TcpEndpointRediscovery { get; }

    public PortReuseMode PortReuseMode { get; }

    public override string ToString() => this.lazyString.Value;

    public string ToJsonString() => this.lazyJsonString.Value;
  }
}
