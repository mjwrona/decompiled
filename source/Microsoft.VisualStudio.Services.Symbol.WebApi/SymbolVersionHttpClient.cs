// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Symbol.WebApi.SymbolVersionHttpClient
// Assembly: Microsoft.VisualStudio.Services.Symbol.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52CDDA61-EF2D-4AD8-A25B-09A8F04FE8C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Symbol.WebApi.dll

using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Symbol.WebApi
{
  public class SymbolVersionHttpClient : IArtifactVersionHttpClient
  {
    public const string SymbolClientVersionHeaderName = "symbol-client-version";
    private const string ClientPath = "/_apis/symbol/client/";
    private const string ExePath = "/_apis/symbol/client/exe";
    private readonly HttpClient httpClient;
    private readonly Uri baseAddress;

    public SymbolVersionHttpClient(Uri baseAddress, IAppTraceSource tracer)
    {
      this.httpClient = ArtifactHttpRetryMessageHandler.CreateHttpClientWithRetryHandler(tracer);
      this.baseAddress = baseAddress;
    }

    public async Task<string> GetVersionAsync(string toolName, CancellationToken cancellationToken) => (await this.SendAsync("/_apis/symbol/client/", cancellationToken).ConfigureAwait(false)).Headers.GetValues("symbol-client-version").Single<string>();

    public async Task<Stream> GetClientExeAsync(CancellationToken cancellationToken) => await (await this.GetAsync("/_apis/symbol/client/exe", cancellationToken).ConfigureAwait(false)).Content.ReadAsStreamAsync().ConfigureAwait(false);

    public Task<Stream> GetClientBinariesAsync(string toolName, CancellationToken cancellationToken) => this.GetClientExeAsync(cancellationToken);

    private async Task<HttpResponseMessage> SendAsync(
      string appendedPath,
      CancellationToken cancellationToken)
    {
      UriBuilder uriBuilder = new UriBuilder(this.baseAddress);
      uriBuilder.Path += appendedPath;
      HttpResponseMessage httpResponseMessage = await this.httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, uriBuilder.Uri), cancellationToken).ConfigureAwait(false);
      if (httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
        throw new VssResourceNotFoundException(string.Format("Resource not found. Check that the provided URL ({0}) is correct.", (object) this.baseAddress));
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage;
    }

    private async Task<HttpResponseMessage> GetAsync(
      string appendedPath,
      CancellationToken cancellationToken)
    {
      UriBuilder uriBuilder = new UriBuilder(this.baseAddress);
      uriBuilder.Path += appendedPath;
      HttpResponseMessage async = await this.httpClient.GetAsync(uriBuilder.Uri, cancellationToken).ConfigureAwait(false);
      if (async.StatusCode == HttpStatusCode.NotFound)
        throw new VssResourceNotFoundException(string.Format("Resource not found. Check that the provided URL ({0}) is correct.", (object) this.baseAddress));
      async.EnsureSuccessStatusCode();
      return async;
    }
  }
}
