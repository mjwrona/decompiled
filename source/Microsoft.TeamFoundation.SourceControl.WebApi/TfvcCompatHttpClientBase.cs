// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.TfvcCompatHttpClientBase
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TfvcCompatHttpClientBase : VssHttpClientBase
  {
    public TfvcCompatHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TfvcCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TfvcCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TfvcCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TfvcCompatHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TfvcItem> GetItemAsync(
      string project,
      string path,
      string fileName = null,
      bool? download = null,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      TfvcVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (!string.IsNullOrEmpty(scopePath))
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      return this.SendAsync<TfvcItem>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TfvcItem> GetItemAsync(
      Guid project,
      string path,
      string fileName = null,
      bool? download = null,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      TfvcVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (!string.IsNullOrEmpty(scopePath))
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      return this.SendAsync<TfvcItem>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual Task<TfvcItem> GetItemAsync(
      string path,
      string fileName = null,
      bool? download = null,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      TfvcVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (!string.IsNullOrEmpty(scopePath))
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        this.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      return this.SendAsync<TfvcItem>(method, locationId, version: new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemContentAsync(
      string project,
      string path,
      string fileName = null,
      bool? download = null,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      TfvcVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TfvcCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (!string.IsNullOrEmpty(scopePath))
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemContentAsync(
      Guid project,
      string path,
      string fileName = null,
      bool? download = null,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      TfvcVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TfvcCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (!string.IsNullOrEmpty(scopePath))
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemContentAsync(
      string path,
      string fileName = null,
      bool? download = null,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      TfvcVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TfvcCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (!string.IsNullOrEmpty(scopePath))
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, version: new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemTextAsync(
      string project,
      string path,
      string fileName = null,
      bool? download = null,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      TfvcVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TfvcCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (!string.IsNullOrEmpty(scopePath))
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemTextAsync(
      Guid project,
      string path,
      string fileName = null,
      bool? download = null,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      TfvcVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TfvcCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (!string.IsNullOrEmpty(scopePath))
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemTextAsync(
      string path,
      string fileName = null,
      bool? download = null,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      TfvcVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TfvcCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (!string.IsNullOrEmpty(scopePath))
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, version: new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemZipAsync(
      string project,
      string path,
      string fileName = null,
      bool? download = null,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      TfvcVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TfvcCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (!string.IsNullOrEmpty(scopePath))
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemZipAsync(
      Guid project,
      string path,
      string fileName = null,
      bool? download = null,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      TfvcVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TfvcCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (!string.IsNullOrEmpty(scopePath))
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete]
    public virtual async Task<Stream> GetItemZipAsync(
      string path,
      string fileName = null,
      bool? download = null,
      string scopePath = null,
      VersionControlRecursionType? recursionLevel = null,
      TfvcVersionDescriptor versionDescriptor = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TfvcCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ba9fc436-9a38-4578-89d6-e4f3241f5040");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(path))
        keyValuePairList.Add(nameof (path), path);
      if (!string.IsNullOrEmpty(fileName))
        keyValuePairList.Add(nameof (fileName), fileName);
      if (download.HasValue)
        keyValuePairList.Add(nameof (download), download.Value.ToString());
      if (!string.IsNullOrEmpty(scopePath))
        keyValuePairList.Add(nameof (scopePath), scopePath);
      if (recursionLevel.HasValue)
        keyValuePairList.Add(nameof (recursionLevel), recursionLevel.Value.ToString());
      if (versionDescriptor != null)
        compatHttpClientBase.AddModelAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (versionDescriptor), (object) versionDescriptor);
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, version: new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false))
        httpResponseMessage = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }
  }
}
