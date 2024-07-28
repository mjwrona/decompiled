// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.WebApi.InternalNuGetHttpClient
// Assembly: Microsoft.VisualStudio.Services.NuGet.Client.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E63C245C-898F-41A7-9916-45B2DC75C1BE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Client.Internal.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.WebApi
{
  public class InternalNuGetHttpClient : InternalNuGetHttpClientBase
  {
    public InternalNuGetHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public InternalNuGetHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public InternalNuGetHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public InternalNuGetHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public InternalNuGetHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public override async Task<Stream> GetNupkgInternalAsync(
      string feedId,
      string id,
      string version,
      string file,
      Guid aadTenantId,
      string sourceProtocolVersion = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      InternalNuGetHttpClient internalNuGetHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fa0b8197-5517-47a7-8992-f6f9ce86cd05");
      object routeValues = (object) new
      {
        feedId = feedId,
        id = id,
        version = version,
        file = file
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (aadTenantId), aadTenantId.ToString());
      if (!string.IsNullOrEmpty(sourceProtocolVersion))
        keyValuePairList.Add(nameof (sourceProtocolVersion), sourceProtocolVersion);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await internalNuGetHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await internalNuGetHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      if (InternalNuGetHttpClient.IsRedirect(httpResponseMessage.StatusCode) && httpResponseMessage.Headers.Location != (Uri) null)
        return await new HttpClient().GetStreamAsync(httpResponseMessage.Headers.Location.ToString());
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public override async Task<Stream> GetNupkgInternalAsync(
      string project,
      string feedId,
      string id,
      string version,
      string file,
      Guid aadTenantId,
      string sourceProtocolVersion = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      InternalNuGetHttpClient internalNuGetHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fa0b8197-5517-47a7-8992-f6f9ce86cd05");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        id = id,
        version = version,
        file = file
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (aadTenantId), aadTenantId.ToString());
      if (!string.IsNullOrEmpty(sourceProtocolVersion))
        keyValuePairList.Add(nameof (sourceProtocolVersion), sourceProtocolVersion);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await internalNuGetHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await internalNuGetHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      if (InternalNuGetHttpClient.IsRedirect(httpResponseMessage.StatusCode) && httpResponseMessage.Headers.Location != (Uri) null)
        return await new HttpClient().GetStreamAsync(httpResponseMessage.Headers.Location.ToString());
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public override async Task<Stream> GetNupkgInternalAsync(
      Guid project,
      string feedId,
      string id,
      string version,
      string file,
      Guid aadTenantId,
      string sourceProtocolVersion = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      InternalNuGetHttpClient internalNuGetHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("fa0b8197-5517-47a7-8992-f6f9ce86cd05");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        id = id,
        version = version,
        file = file
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (aadTenantId), aadTenantId.ToString());
      if (!string.IsNullOrEmpty(sourceProtocolVersion))
        keyValuePairList.Add(nameof (sourceProtocolVersion), sourceProtocolVersion);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await internalNuGetHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await internalNuGetHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      if (InternalNuGetHttpClient.IsRedirect(httpResponseMessage.StatusCode) && httpResponseMessage.Headers.Location != (Uri) null)
        return await new HttpClient().GetStreamAsync(httpResponseMessage.Headers.Location.ToString());
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    protected override Task HandleResponseAsync(
      HttpResponseMessage response,
      CancellationToken cancellationToken)
    {
      return response.StatusCode != HttpStatusCode.SeeOther && !InternalNuGetHttpClient.IsRedirect(response.StatusCode) ? base.HandleResponseAsync(response, cancellationToken) : (Task) Task.FromResult<bool>(true);
    }

    private static bool IsRedirect(HttpStatusCode statusCode) => statusCode == HttpStatusCode.MovedPermanently || statusCode == HttpStatusCode.Found || statusCode == HttpStatusCode.SeeOther;
  }
}
