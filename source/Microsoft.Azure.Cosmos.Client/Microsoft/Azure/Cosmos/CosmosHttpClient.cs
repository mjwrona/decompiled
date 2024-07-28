// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosHttpClient
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Collections;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal abstract class CosmosHttpClient : IDisposable
  {
    public static readonly TimeSpan GatewayRequestTimeout = TimeSpan.FromSeconds(65.0);

    public abstract HttpMessageHandler HttpMessageHandler { get; }

    public abstract Task<HttpResponseMessage> GetAsync(
      Uri uri,
      INameValueCollection additionalHeaders,
      ResourceType resourceType,
      HttpTimeoutPolicy timeoutPolicy,
      IClientSideRequestStatistics clientSideRequestStatistics,
      CancellationToken cancellationToken);

    public abstract Task<HttpResponseMessage> SendHttpAsync(
      Func<ValueTask<HttpRequestMessage>> createRequestMessageAsync,
      ResourceType resourceType,
      HttpTimeoutPolicy timeoutPolicy,
      IClientSideRequestStatistics clientSideRequestStatistics,
      CancellationToken cancellationToken);

    protected abstract void Dispose(bool disposing);

    public abstract void Dispose();
  }
}
