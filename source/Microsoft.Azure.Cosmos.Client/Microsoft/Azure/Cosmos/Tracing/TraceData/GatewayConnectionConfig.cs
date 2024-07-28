// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tracing.TraceData.GatewayConnectionConfig
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http;

namespace Microsoft.Azure.Cosmos.Tracing.TraceData
{
  internal class GatewayConnectionConfig
  {
    private readonly Lazy<string> lazyString;
    private readonly Lazy<string> lazyJsonString;

    public GatewayConnectionConfig(
      int maxConnectionLimit,
      TimeSpan requestTimeout,
      IWebProxy webProxy,
      Func<HttpClient> httpClientFactory)
    {
      GatewayConnectionConfig connectionConfig = this;
      this.MaxConnectionLimit = maxConnectionLimit;
      this.UserRequestTimeout = (int) requestTimeout.TotalSeconds;
      this.IsWebProxyConfigured = webProxy != null;
      this.IsHttpClientFactoryConfigured = httpClientFactory != null;
      this.lazyString = new Lazy<string>((Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "(cps:{0}, urto:{1}, p:{2}, httpf: {3})", (object) maxConnectionLimit, (object) (int) requestTimeout.TotalSeconds, (object) (webProxy != null), (object) (httpClientFactory != null))));
      this.lazyJsonString = new Lazy<string>((Func<string>) (() => JsonConvert.SerializeObject((object) connectionConfig)));
    }

    public int MaxConnectionLimit { get; }

    public int UserRequestTimeout { get; }

    public bool IsWebProxyConfigured { get; }

    public bool IsHttpClientFactoryConfigured { get; }

    public override string ToString() => this.lazyString.Value;

    public string ToJsonString() => this.lazyJsonString.Value;
  }
}
