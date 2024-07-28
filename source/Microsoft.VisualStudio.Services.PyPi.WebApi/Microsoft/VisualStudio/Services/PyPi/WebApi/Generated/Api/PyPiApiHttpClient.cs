// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.WebApi.Generated.Api.PyPiApiHttpClient
// Assembly: Microsoft.VisualStudio.Services.PyPi.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17E1C323-94FE-4FF1-903A-ED51BA3159D2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API;
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

namespace Microsoft.VisualStudio.Services.PyPi.WebApi.Generated.Api
{
  [ResourceArea("92F0314B-06C5-46E0-ABE7-15FD9D13276A")]
  public class PyPiApiHttpClient : VssHttpClientBase
  {
    public PyPiApiHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public PyPiApiHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public PyPiApiHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public PyPiApiHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public PyPiApiHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task<Stream> DownloadPackageAsync(
      string feedId,
      string packageName,
      string packageVersion,
      string fileName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PyPiApiHttpClient pyPiApiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("97218bae-a64d-4381-9257-b5b7951f0b98");
      object routeValues = (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion,
        fileName = fileName
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await pyPiApiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await pyPiApiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> DownloadPackageAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      string fileName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PyPiApiHttpClient pyPiApiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("97218bae-a64d-4381-9257-b5b7951f0b98");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion,
        fileName = fileName
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await pyPiApiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await pyPiApiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<Stream> DownloadPackageAsync(
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      string fileName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PyPiApiHttpClient pyPiApiHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("97218bae-a64d-4381-9257-b5b7951f0b98");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion,
        fileName = fileName
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await pyPiApiHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await pyPiApiHttpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task UpdatePackageVersionsAsync(
      string feedId,
      PyPiPackagesBatchRequest batchRequest,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PyPiApiHttpClient pyPiApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("4e53d561-70c1-4c98-b937-0f22acb27b0b");
      object obj1 = (object) new{ feedId = feedId };
      HttpContent httpContent = (HttpContent) new ObjectContent<PyPiPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      PyPiApiHttpClient pyPiApiHttpClient2 = pyPiApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await pyPiApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task UpdatePackageVersionsAsync(
      string project,
      string feedId,
      PyPiPackagesBatchRequest batchRequest,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PyPiApiHttpClient pyPiApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("4e53d561-70c1-4c98-b937-0f22acb27b0b");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PyPiPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      PyPiApiHttpClient pyPiApiHttpClient2 = pyPiApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await pyPiApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task UpdatePackageVersionsAsync(
      Guid project,
      string feedId,
      PyPiPackagesBatchRequest batchRequest,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PyPiApiHttpClient pyPiApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("4e53d561-70c1-4c98-b937-0f22acb27b0b");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PyPiPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      PyPiApiHttpClient pyPiApiHttpClient2 = pyPiApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await pyPiApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task UpdateRecycleBinPackageVersionsAsync(
      string feedId,
      PyPiPackagesBatchRequest batchRequest,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PyPiApiHttpClient pyPiApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d2d89918-c69e-4ef4-b357-1c3ccb4d28d2");
      object obj1 = (object) new{ feedId = feedId };
      HttpContent httpContent = (HttpContent) new ObjectContent<PyPiPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      PyPiApiHttpClient pyPiApiHttpClient2 = pyPiApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await pyPiApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task UpdateRecycleBinPackageVersionsAsync(
      string project,
      string feedId,
      PyPiPackagesBatchRequest batchRequest,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PyPiApiHttpClient pyPiApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d2d89918-c69e-4ef4-b357-1c3ccb4d28d2");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PyPiPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      PyPiApiHttpClient pyPiApiHttpClient2 = pyPiApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await pyPiApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task UpdateRecycleBinPackageVersionsAsync(
      Guid project,
      string feedId,
      PyPiPackagesBatchRequest batchRequest,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PyPiApiHttpClient pyPiApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("d2d89918-c69e-4ef4-b357-1c3ccb4d28d2");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PyPiPackagesBatchRequest>(batchRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      PyPiApiHttpClient pyPiApiHttpClient2 = pyPiApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await pyPiApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task DeletePackageVersionFromRecycleBinAsync(
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("07143752-3d94-45fd-86c2-0c77ed87847b"), (object) new
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
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("07143752-3d94-45fd-86c2-0c77ed87847b"), (object) new
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
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("07143752-3d94-45fd-86c2-0c77ed87847b"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<PyPiPackageVersionDeletionState> GetPackageVersionMetadataFromRecycleBinAsync(
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PyPiPackageVersionDeletionState>(new HttpMethod("GET"), new Guid("07143752-3d94-45fd-86c2-0c77ed87847b"), (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<PyPiPackageVersionDeletionState> GetPackageVersionMetadataFromRecycleBinAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PyPiPackageVersionDeletionState>(new HttpMethod("GET"), new Guid("07143752-3d94-45fd-86c2-0c77ed87847b"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<PyPiPackageVersionDeletionState> GetPackageVersionMetadataFromRecycleBinAsync(
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PyPiPackageVersionDeletionState>(new HttpMethod("GET"), new Guid("07143752-3d94-45fd-86c2-0c77ed87847b"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task RestorePackageVersionFromRecycleBinAsync(
      string feedId,
      string packageName,
      string packageVersion,
      PyPiRecycleBinPackageVersionDetails packageVersionDetails,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PyPiApiHttpClient pyPiApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("07143752-3d94-45fd-86c2-0c77ed87847b");
      object obj1 = (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PyPiRecycleBinPackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      PyPiApiHttpClient pyPiApiHttpClient2 = pyPiApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await pyPiApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task RestorePackageVersionFromRecycleBinAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      PyPiRecycleBinPackageVersionDetails packageVersionDetails,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PyPiApiHttpClient pyPiApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("07143752-3d94-45fd-86c2-0c77ed87847b");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PyPiRecycleBinPackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      PyPiApiHttpClient pyPiApiHttpClient2 = pyPiApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await pyPiApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task RestorePackageVersionFromRecycleBinAsync(
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      PyPiRecycleBinPackageVersionDetails packageVersionDetails,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PyPiApiHttpClient pyPiApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("07143752-3d94-45fd-86c2-0c77ed87847b");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PyPiRecycleBinPackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      PyPiApiHttpClient pyPiApiHttpClient2 = pyPiApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await pyPiApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public Task<UpstreamingBehavior> GetUpstreamingBehaviorAsync(
      string feedId,
      string packageName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<UpstreamingBehavior>(new HttpMethod("GET"), new Guid("21b8c9a7-7080-45be-a5ba-e50bb4c18130"), (object) new
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
      return this.SendAsync<UpstreamingBehavior>(new HttpMethod("GET"), new Guid("21b8c9a7-7080-45be-a5ba-e50bb4c18130"), (object) new
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
      return this.SendAsync<UpstreamingBehavior>(new HttpMethod("GET"), new Guid("21b8c9a7-7080-45be-a5ba-e50bb4c18130"), (object) new
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
      PyPiApiHttpClient pyPiApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("21b8c9a7-7080-45be-a5ba-e50bb4c18130");
      object obj1 = (object) new
      {
        feedId = feedId,
        packageName = packageName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpstreamingBehavior>(behavior, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      PyPiApiHttpClient pyPiApiHttpClient2 = pyPiApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await pyPiApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
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
      PyPiApiHttpClient pyPiApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("21b8c9a7-7080-45be-a5ba-e50bb4c18130");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpstreamingBehavior>(behavior, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      PyPiApiHttpClient pyPiApiHttpClient2 = pyPiApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await pyPiApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
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
      PyPiApiHttpClient pyPiApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("21b8c9a7-7080-45be-a5ba-e50bb4c18130");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpstreamingBehavior>(behavior, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      PyPiApiHttpClient pyPiApiHttpClient2 = pyPiApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await pyPiApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public Task<Package> DeletePackageVersionAsync(
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Package>(new HttpMethod("DELETE"), new Guid("d146ac7e-9e3f-4448-b956-f9bb3bdf9b2e"), (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Package> DeletePackageVersionAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Package>(new HttpMethod("DELETE"), new Guid("d146ac7e-9e3f-4448-b956-f9bb3bdf9b2e"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Package> DeletePackageVersionAsync(
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Package>(new HttpMethod("DELETE"), new Guid("d146ac7e-9e3f-4448-b956-f9bb3bdf9b2e"), (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Package> GetPackageVersionAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      bool? showDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d146ac7e-9e3f-4448-b956-f9bb3bdf9b2e");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (showDeleted.HasValue)
        keyValuePairList.Add(nameof (showDeleted), showDeleted.Value.ToString());
      return this.SendAsync<Package>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Package> GetPackageVersionAsync(
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      bool? showDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d146ac7e-9e3f-4448-b956-f9bb3bdf9b2e");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (showDeleted.HasValue)
        keyValuePairList.Add(nameof (showDeleted), showDeleted.Value.ToString());
      return this.SendAsync<Package>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<Package> GetPackageVersionAsync(
      string feedId,
      string packageName,
      string packageVersion,
      bool? showDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d146ac7e-9e3f-4448-b956-f9bb3bdf9b2e");
      object routeValues = (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (showDeleted.HasValue)
        keyValuePairList.Add(nameof (showDeleted), showDeleted.Value.ToString());
      return this.SendAsync<Package>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public async Task UpdatePackageVersionAsync(
      string feedId,
      string packageName,
      string packageVersion,
      PackageVersionDetails packageVersionDetails,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PyPiApiHttpClient pyPiApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d146ac7e-9e3f-4448-b956-f9bb3bdf9b2e");
      object obj1 = (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      PyPiApiHttpClient pyPiApiHttpClient2 = pyPiApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await pyPiApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task UpdatePackageVersionAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      PackageVersionDetails packageVersionDetails,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PyPiApiHttpClient pyPiApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d146ac7e-9e3f-4448-b956-f9bb3bdf9b2e");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      PyPiApiHttpClient pyPiApiHttpClient2 = pyPiApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await pyPiApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task UpdatePackageVersionAsync(
      Guid project,
      string feedId,
      string packageName,
      string packageVersion,
      PackageVersionDetails packageVersionDetails,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PyPiApiHttpClient pyPiApiHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("d146ac7e-9e3f-4448-b956-f9bb3bdf9b2e");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageVersionDetails>(packageVersionDetails, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      PyPiApiHttpClient pyPiApiHttpClient2 = pyPiApiHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await pyPiApiHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }
  }
}
