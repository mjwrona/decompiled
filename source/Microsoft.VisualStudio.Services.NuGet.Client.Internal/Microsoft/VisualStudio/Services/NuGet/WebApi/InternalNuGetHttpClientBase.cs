// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.WebApi.InternalNuGetHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.NuGet.Client.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E63C245C-898F-41A7-9916-45B2DC75C1BE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Client.Internal.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.NuGet.Client.Internal;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Generated;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.WebApi
{
  [ResourceArea("{B3BE7473-68EA-4A81-BFC7-9530BAAA19AD}")]
  public abstract class InternalNuGetHttpClientBase : NuGetHttpClient
  {
    public InternalNuGetHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public InternalNuGetHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public InternalNuGetHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public InternalNuGetHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public InternalNuGetHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<Stream> GetAllPackageVersionsInternalAsync(
      string project,
      string feedId,
      Guid aadTenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      InternalNuGetHttpClientBase getHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4a1dc31f-d500-41e4-9fca-2500fee6b44c");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (aadTenantId), aadTenantId.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await getHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await getHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<Stream> GetAllPackageVersionsInternalAsync(
      Guid project,
      string feedId,
      Guid aadTenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      InternalNuGetHttpClientBase getHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4a1dc31f-d500-41e4-9fca-2500fee6b44c");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (aadTenantId), aadTenantId.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await getHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await getHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<Stream> GetAllPackageVersionsInternalAsync(
      string feedId,
      Guid aadTenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      InternalNuGetHttpClientBase getHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("4a1dc31f-d500-41e4-9fca-2500fee6b44c");
      object routeValues = (object) new{ feedId = feedId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (aadTenantId), aadTenantId.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await getHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await getHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<Stream> GetNupkgInternalAsync(
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
      InternalNuGetHttpClientBase getHttpClientBase = this;
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
      if (sourceProtocolVersion != null)
        keyValuePairList.Add(nameof (sourceProtocolVersion), sourceProtocolVersion);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await getHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await getHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<Stream> GetNupkgInternalAsync(
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
      InternalNuGetHttpClientBase getHttpClientBase = this;
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
      if (sourceProtocolVersion != null)
        keyValuePairList.Add(nameof (sourceProtocolVersion), sourceProtocolVersion);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await getHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await getHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<Stream> GetNupkgInternalAsync(
      string feedId,
      string id,
      string version,
      string file,
      Guid aadTenantId,
      string sourceProtocolVersion = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      InternalNuGetHttpClientBase getHttpClientBase = this;
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
      if (sourceProtocolVersion != null)
        keyValuePairList.Add(nameof (sourceProtocolVersion), sourceProtocolVersion);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await getHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await getHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<Stream> GetNuspecInternalAsync(
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
      InternalNuGetHttpClientBase getHttpClientBase = this;
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
      if (sourceProtocolVersion != null)
        keyValuePairList.Add(nameof (sourceProtocolVersion), sourceProtocolVersion);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await getHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/xml").ConfigureAwait(false))
        httpResponseMessage = await getHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<Stream> GetNuspecInternalAsync(
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
      InternalNuGetHttpClientBase getHttpClientBase = this;
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
      if (sourceProtocolVersion != null)
        keyValuePairList.Add(nameof (sourceProtocolVersion), sourceProtocolVersion);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await getHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/xml").ConfigureAwait(false))
        httpResponseMessage = await getHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task<Stream> GetNuspecInternalAsync(
      string feedId,
      string id,
      string version,
      string file,
      Guid aadTenantId,
      string sourceProtocolVersion = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      InternalNuGetHttpClientBase getHttpClientBase = this;
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
      if (sourceProtocolVersion != null)
        keyValuePairList.Add(nameof (sourceProtocolVersion), sourceProtocolVersion);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await getHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/xml").ConfigureAwait(false))
        httpResponseMessage = await getHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<RawPackageNameEntry>> GetNamesInternalAsync(
      string project,
      string feedId,
      Guid aadTenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("52156c00-7656-4768-8e49-5f12e9347204");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (aadTenantId), aadTenantId.ToString());
      return this.SendAsync<List<RawPackageNameEntry>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<RawPackageNameEntry>> GetNamesInternalAsync(
      Guid project,
      string feedId,
      Guid aadTenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("52156c00-7656-4768-8e49-5f12e9347204");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (aadTenantId), aadTenantId.ToString());
      return this.SendAsync<List<RawPackageNameEntry>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<RawPackageNameEntry>> GetNamesInternalAsync(
      string feedId,
      Guid aadTenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("52156c00-7656-4768-8e49-5f12e9347204");
      object routeValues = (object) new{ feedId = feedId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (aadTenantId), aadTenantId.ToString());
      return this.SendAsync<List<RawPackageNameEntry>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GetNuspecsInternalResponseNuspec>> GetNuspecsInternalAsync(
      IEnumerable<string> versions,
      string feedId,
      string id,
      Guid aadTenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("387cdcb7-3fcf-4dfc-b8bc-1abb5b4f4354");
      object obj1 = (object) new{ feedId = feedId, id = id };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<string>>(versions, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (aadTenantId), aadTenantId.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GetNuspecsInternalResponseNuspec>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GetNuspecsInternalResponseNuspec>> GetNuspecsInternalAsync(
      IEnumerable<string> versions,
      string project,
      string feedId,
      string id,
      Guid aadTenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("387cdcb7-3fcf-4dfc-b8bc-1abb5b4f4354");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        id = id
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<string>>(versions, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (aadTenantId), aadTenantId.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GetNuspecsInternalResponseNuspec>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<GetNuspecsInternalResponseNuspec>> GetNuspecsInternalAsync(
      IEnumerable<string> versions,
      Guid project,
      string feedId,
      string id,
      Guid aadTenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("387cdcb7-3fcf-4dfc-b8bc-1abb5b4f4354");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        id = id
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<string>>(versions, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (aadTenantId), aadTenantId.ToString());
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GetNuspecsInternalResponseNuspec>>(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<NuGetVersionsExposedToDownstreamsResponse> GetPackageVersionsExposedToDownstreamsAsync(
      string project,
      string feedId,
      string id,
      Guid aadTenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5cdf277c-d60b-4259-945c-83144ffa814c");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (aadTenantId), aadTenantId.ToString());
      return this.SendAsync<NuGetVersionsExposedToDownstreamsResponse>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<NuGetVersionsExposedToDownstreamsResponse> GetPackageVersionsExposedToDownstreamsAsync(
      Guid project,
      string feedId,
      string id,
      Guid aadTenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5cdf277c-d60b-4259-945c-83144ffa814c");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (aadTenantId), aadTenantId.ToString());
      return this.SendAsync<NuGetVersionsExposedToDownstreamsResponse>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<NuGetVersionsExposedToDownstreamsResponse> GetPackageVersionsExposedToDownstreamsAsync(
      string feedId,
      string id,
      Guid aadTenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("5cdf277c-d60b-4259-945c-83144ffa814c");
      object routeValues = (object) new
      {
        feedId = feedId,
        id = id
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (aadTenantId), aadTenantId.ToString());
      return this.SendAsync<NuGetVersionsExposedToDownstreamsResponse>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
