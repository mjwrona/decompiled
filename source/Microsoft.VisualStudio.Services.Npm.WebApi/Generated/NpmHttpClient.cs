// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.Generated.NpmHttpClient
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.WebApi.Generated
{
  [ResourceArea("{4C83CFC1-F33A-477E-A789-29D38FFCA52E}")]
  public class NpmHttpClient : VssHttpClientBase
  {
    public NpmHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public NpmHttpClient(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public NpmHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public NpmHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public NpmHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task<Stream> GetContentScopedPackageAsync(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("09a4eafd-123a-495c-979c-0eda7bdb9a14");
      object routeValues = (object) new
      {
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName,
        packageVersion = packageVersion
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await npmHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await npmHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetContentScopedPackageAsync(
      string project,
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("09a4eafd-123a-495c-979c-0eda7bdb9a14");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName,
        packageVersion = packageVersion
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await npmHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await npmHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetContentScopedPackageAsync(
      Guid project,
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("09a4eafd-123a-495c-979c-0eda7bdb9a14");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName,
        packageVersion = packageVersion
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await npmHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await npmHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetContentUnscopedPackageAsync(
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("75caa482-cb1e-47cd-9f2c-c048a4b7a43e");
      object routeValues = (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await npmHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await npmHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetContentUnscopedPackageAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("75caa482-cb1e-47cd-9f2c-c048a4b7a43e");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await npmHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await npmHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetContentUnscopedPackageAsync(
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("75caa482-cb1e-47cd-9f2c-c048a4b7a43e");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await npmHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await npmHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task UpdatePackagesAsync(
      NpmPackagesBatchRequest batchRequest,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("06f34005-bbb2-41f4-88f5-23e03a99bb12");
      object obj1 = (object) new{ feedId = feedId };
      HttpContent httpContent = (HttpContent) new ObjectContent<NpmPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NpmHttpClient npmHttpClient2 = npmHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await npmHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task UpdatePackagesAsync(
      NpmPackagesBatchRequest batchRequest,
      string project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("06f34005-bbb2-41f4-88f5-23e03a99bb12");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<NpmPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NpmHttpClient npmHttpClient2 = npmHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await npmHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task UpdatePackagesAsync(
      NpmPackagesBatchRequest batchRequest,
      Guid project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("06f34005-bbb2-41f4-88f5-23e03a99bb12");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<NpmPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NpmHttpClient npmHttpClient2 = npmHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await npmHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task<Stream> GetReadmeScopedPackageAsync(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6d4db777-7e4a-43b2-afad-779a1d197301");
      object routeValues = (object) new
      {
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName,
        packageVersion = packageVersion
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await npmHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await npmHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetReadmeScopedPackageAsync(
      string project,
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6d4db777-7e4a-43b2-afad-779a1d197301");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName,
        packageVersion = packageVersion
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await npmHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await npmHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetReadmeScopedPackageAsync(
      Guid project,
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6d4db777-7e4a-43b2-afad-779a1d197301");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName,
        packageVersion = packageVersion
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await npmHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await npmHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetReadmeUnscopedPackageAsync(
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1099a396-b310-41d4-a4b6-33d134ce3fcf");
      object routeValues = (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await npmHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await npmHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetReadmeUnscopedPackageAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1099a396-b310-41d4-a4b6-33d134ce3fcf");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await npmHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await npmHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> GetReadmeUnscopedPackageAsync(
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("1099a396-b310-41d4-a4b6-33d134ce3fcf");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await npmHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "text/plain").ConfigureAwait(false))
        httpResponseMessage = await npmHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task UpdateRecycleBinPackagesAsync(
      NpmPackagesBatchRequest batchRequest,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("eefe03ef-a6a2-4a7a-a0ec-2e65a5efd64c");
      object obj1 = (object) new{ feedId = feedId };
      HttpContent httpContent = (HttpContent) new ObjectContent<NpmPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NpmHttpClient npmHttpClient2 = npmHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await npmHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task UpdateRecycleBinPackagesAsync(
      NpmPackagesBatchRequest batchRequest,
      string project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("eefe03ef-a6a2-4a7a-a0ec-2e65a5efd64c");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<NpmPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NpmHttpClient npmHttpClient2 = npmHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await npmHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task UpdateRecycleBinPackagesAsync(
      NpmPackagesBatchRequest batchRequest,
      Guid project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("eefe03ef-a6a2-4a7a-a0ec-2e65a5efd64c");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<NpmPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NpmHttpClient npmHttpClient2 = npmHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await npmHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task DeleteScopedPackageVersionFromRecycleBinAsync(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("220f45eb-94a5-432c-902a-5b8c6372e415"), (object) new
      {
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task DeleteScopedPackageVersionFromRecycleBinAsync(
      string project,
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("220f45eb-94a5-432c-902a-5b8c6372e415"), (object) new
      {
        project = project,
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task DeleteScopedPackageVersionFromRecycleBinAsync(
      Guid project,
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("220f45eb-94a5-432c-902a-5b8c6372e415"), (object) new
      {
        project = project,
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<NpmPackageVersionDeletionState> GetScopedPackageVersionMetadataFromRecycleBinAsync(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<NpmPackageVersionDeletionState>(new HttpMethod("GET"), new Guid("220f45eb-94a5-432c-902a-5b8c6372e415"), (object) new
      {
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<NpmPackageVersionDeletionState> GetScopedPackageVersionMetadataFromRecycleBinAsync(
      string project,
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<NpmPackageVersionDeletionState>(new HttpMethod("GET"), new Guid("220f45eb-94a5-432c-902a-5b8c6372e415"), (object) new
      {
        project = project,
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<NpmPackageVersionDeletionState> GetScopedPackageVersionMetadataFromRecycleBinAsync(
      Guid project,
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<NpmPackageVersionDeletionState>(new HttpMethod("GET"), new Guid("220f45eb-94a5-432c-902a-5b8c6372e415"), (object) new
      {
        project = project,
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task RestoreScopedPackageVersionFromRecycleBinAsync(
      NpmRecycleBinPackageVersionDetails packageVersionDetails,
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("220f45eb-94a5-432c-902a-5b8c6372e415");
      object obj1 = (object) new
      {
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<NpmRecycleBinPackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NpmHttpClient npmHttpClient2 = npmHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await npmHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task RestoreScopedPackageVersionFromRecycleBinAsync(
      NpmRecycleBinPackageVersionDetails packageVersionDetails,
      string project,
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("220f45eb-94a5-432c-902a-5b8c6372e415");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<NpmRecycleBinPackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NpmHttpClient npmHttpClient2 = npmHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await npmHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task RestoreScopedPackageVersionFromRecycleBinAsync(
      NpmRecycleBinPackageVersionDetails packageVersionDetails,
      Guid project,
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("220f45eb-94a5-432c-902a-5b8c6372e415");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<NpmRecycleBinPackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NpmHttpClient npmHttpClient2 = npmHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await npmHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task DeletePackageVersionFromRecycleBinAsync(
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("63a4f31f-e92b-4ee4-bf92-22d485e73bef"), (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task DeletePackageVersionFromRecycleBinAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("63a4f31f-e92b-4ee4-bf92-22d485e73bef"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public async Task DeletePackageVersionFromRecycleBinAsync(
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("63a4f31f-e92b-4ee4-bf92-22d485e73bef"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<NpmPackageVersionDeletionState> GetPackageVersionMetadataFromRecycleBinAsync(
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<NpmPackageVersionDeletionState>(new HttpMethod("GET"), new Guid("63a4f31f-e92b-4ee4-bf92-22d485e73bef"), (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<NpmPackageVersionDeletionState> GetPackageVersionMetadataFromRecycleBinAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<NpmPackageVersionDeletionState>(new HttpMethod("GET"), new Guid("63a4f31f-e92b-4ee4-bf92-22d485e73bef"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<NpmPackageVersionDeletionState> GetPackageVersionMetadataFromRecycleBinAsync(
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<NpmPackageVersionDeletionState>(new HttpMethod("GET"), new Guid("63a4f31f-e92b-4ee4-bf92-22d485e73bef"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task RestorePackageVersionFromRecycleBinAsync(
      NpmRecycleBinPackageVersionDetails packageVersionDetails,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("63a4f31f-e92b-4ee4-bf92-22d485e73bef");
      object obj1 = (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<NpmRecycleBinPackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NpmHttpClient npmHttpClient2 = npmHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await npmHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task RestorePackageVersionFromRecycleBinAsync(
      NpmRecycleBinPackageVersionDetails packageVersionDetails,
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("63a4f31f-e92b-4ee4-bf92-22d485e73bef");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<NpmRecycleBinPackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NpmHttpClient npmHttpClient2 = npmHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await npmHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task RestorePackageVersionFromRecycleBinAsync(
      NpmRecycleBinPackageVersionDetails packageVersionDetails,
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("63a4f31f-e92b-4ee4-bf92-22d485e73bef");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<NpmRecycleBinPackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NpmHttpClient npmHttpClient2 = npmHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await npmHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public Task<UpstreamingBehavior> GetScopedUpstreamingBehaviorAsync(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<UpstreamingBehavior>(new HttpMethod("GET"), new Guid("9859c187-f6ec-41b0-862d-8003b3b404e0"), (object) new
      {
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<UpstreamingBehavior> GetScopedUpstreamingBehaviorAsync(
      string project,
      string feedId,
      string packageScope,
      string unscopedPackageName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<UpstreamingBehavior>(new HttpMethod("GET"), new Guid("9859c187-f6ec-41b0-862d-8003b3b404e0"), (object) new
      {
        project = project,
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<UpstreamingBehavior> GetScopedUpstreamingBehaviorAsync(
      Guid project,
      string feedId,
      string packageScope,
      string unscopedPackageName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<UpstreamingBehavior>(new HttpMethod("GET"), new Guid("9859c187-f6ec-41b0-862d-8003b3b404e0"), (object) new
      {
        project = project,
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task SetScopedUpstreamingBehaviorAsync(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      UpstreamingBehavior behavior,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("9859c187-f6ec-41b0-862d-8003b3b404e0");
      object obj1 = (object) new
      {
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpstreamingBehavior>(behavior, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NpmHttpClient npmHttpClient2 = npmHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await npmHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task SetScopedUpstreamingBehaviorAsync(
      string project,
      string feedId,
      string packageScope,
      string unscopedPackageName,
      UpstreamingBehavior behavior,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("9859c187-f6ec-41b0-862d-8003b3b404e0");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpstreamingBehavior>(behavior, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NpmHttpClient npmHttpClient2 = npmHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await npmHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task SetScopedUpstreamingBehaviorAsync(
      Guid project,
      string feedId,
      string packageScope,
      string unscopedPackageName,
      UpstreamingBehavior behavior,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("9859c187-f6ec-41b0-862d-8003b3b404e0");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpstreamingBehavior>(behavior, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NpmHttpClient npmHttpClient2 = npmHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await npmHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public Task<UpstreamingBehavior> GetUpstreamingBehaviorAsync(
      string feedId,
      string packageName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<UpstreamingBehavior>(new HttpMethod("GET"), new Guid("e27a45d3-711b-41cb-a47a-ae669b6e9076"), (object) new
      {
        feedId = feedId,
        packageName = packageName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<UpstreamingBehavior> GetUpstreamingBehaviorAsync(
      string project,
      string feedId,
      string packageName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<UpstreamingBehavior>(new HttpMethod("GET"), new Guid("e27a45d3-711b-41cb-a47a-ae669b6e9076"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<UpstreamingBehavior> GetUpstreamingBehaviorAsync(
      Guid project,
      string feedId,
      string packageName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<UpstreamingBehavior>(new HttpMethod("GET"), new Guid("e27a45d3-711b-41cb-a47a-ae669b6e9076"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task SetUpstreamingBehaviorAsync(
      string feedId,
      string packageName,
      UpstreamingBehavior behavior,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("e27a45d3-711b-41cb-a47a-ae669b6e9076");
      object obj1 = (object) new
      {
        feedId = feedId,
        packageName = packageName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpstreamingBehavior>(behavior, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NpmHttpClient npmHttpClient2 = npmHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await npmHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task SetUpstreamingBehaviorAsync(
      string project,
      string feedId,
      string packageName,
      UpstreamingBehavior behavior,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("e27a45d3-711b-41cb-a47a-ae669b6e9076");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpstreamingBehavior>(behavior, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NpmHttpClient npmHttpClient2 = npmHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await npmHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task SetUpstreamingBehaviorAsync(
      Guid project,
      string feedId,
      string packageName,
      UpstreamingBehavior behavior,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmHttpClient npmHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("e27a45d3-711b-41cb-a47a-ae669b6e9076");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpstreamingBehavior>(behavior, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NpmHttpClient npmHttpClient2 = npmHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await npmHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public Task<Package> GetScopedPackageInfoAsync(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Package>(new HttpMethod("GET"), new Guid("e93d9ec3-4022-401e-96b0-83ea5d911e09"), (object) new
      {
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Package> GetScopedPackageInfoAsync(
      string project,
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Package>(new HttpMethod("GET"), new Guid("e93d9ec3-4022-401e-96b0-83ea5d911e09"), (object) new
      {
        project = project,
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Package> GetScopedPackageInfoAsync(
      Guid project,
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Package>(new HttpMethod("GET"), new Guid("e93d9ec3-4022-401e-96b0-83ea5d911e09"), (object) new
      {
        project = project,
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Package> UnpublishScopedPackageAsync(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Package>(new HttpMethod("DELETE"), new Guid("e93d9ec3-4022-401e-96b0-83ea5d911e09"), (object) new
      {
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Package> UnpublishScopedPackageAsync(
      string project,
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Package>(new HttpMethod("DELETE"), new Guid("e93d9ec3-4022-401e-96b0-83ea5d911e09"), (object) new
      {
        project = project,
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Package> UnpublishScopedPackageAsync(
      Guid project,
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Package>(new HttpMethod("DELETE"), new Guid("e93d9ec3-4022-401e-96b0-83ea5d911e09"), (object) new
      {
        project = project,
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Package> UpdateScopedPackageAsync(
      PackageVersionDetails packageVersionDetails,
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("e93d9ec3-4022-401e-96b0-83ea5d911e09");
      object obj1 = (object) new
      {
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Package>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<Package> UpdateScopedPackageAsync(
      PackageVersionDetails packageVersionDetails,
      string project,
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("e93d9ec3-4022-401e-96b0-83ea5d911e09");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Package>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<Package> UpdateScopedPackageAsync(
      PackageVersionDetails packageVersionDetails,
      Guid project,
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("e93d9ec3-4022-401e-96b0-83ea5d911e09");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Package>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<Package> GetPackageInfoAsync(
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Package>(new HttpMethod("GET"), new Guid("ed579d62-67c9-4271-be66-9b029af5bcf9"), (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Package> GetPackageInfoAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Package>(new HttpMethod("GET"), new Guid("ed579d62-67c9-4271-be66-9b029af5bcf9"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Package> GetPackageInfoAsync(
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Package>(new HttpMethod("GET"), new Guid("ed579d62-67c9-4271-be66-9b029af5bcf9"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Package> UnpublishPackageAsync(
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Package>(new HttpMethod("DELETE"), new Guid("ed579d62-67c9-4271-be66-9b029af5bcf9"), (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Package> UnpublishPackageAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Package>(new HttpMethod("DELETE"), new Guid("ed579d62-67c9-4271-be66-9b029af5bcf9"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Package> UnpublishPackageAsync(
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Package>(new HttpMethod("DELETE"), new Guid("ed579d62-67c9-4271-be66-9b029af5bcf9"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Package> UpdatePackageAsync(
      PackageVersionDetails packageVersionDetails,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("ed579d62-67c9-4271-be66-9b029af5bcf9");
      object obj1 = (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Package>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<Package> UpdatePackageAsync(
      PackageVersionDetails packageVersionDetails,
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("ed579d62-67c9-4271-be66-9b029af5bcf9");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Package>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<Package> UpdatePackageAsync(
      PackageVersionDetails packageVersionDetails,
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("ed579d62-67c9-4271-be66-9b029af5bcf9");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Package>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
