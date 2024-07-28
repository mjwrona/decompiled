// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.FeedHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8DACB936-5231-4131-8ED8-082A1F46DC54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Operations;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.WebApi
{
  [ResourceArea("7AB4E64E-C4D8-4F50-AE73-5EF2E21642A5")]
  public abstract class FeedHttpClientBase : VssHttpClientBase
  {
    public FeedHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public FeedHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public FeedHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public FeedHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public FeedHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task<Stream> GetBadgeAsync(
      string feedId,
      Guid packageId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeedHttpClientBase feedHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("61d885fd-10f3-4a55-82b6-476d866b673f");
      object routeValues = (object) new
      {
        feedId = feedId,
        packageId = packageId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await feedHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "image/svg+xml").ConfigureAwait(false))
        httpResponseMessage = await feedHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetBadgeAsync(
      string project,
      string feedId,
      Guid packageId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeedHttpClientBase feedHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("61d885fd-10f3-4a55-82b6-476d866b673f");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageId = packageId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await feedHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "image/svg+xml").ConfigureAwait(false))
        httpResponseMessage = await feedHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public virtual async Task<Stream> GetBadgeAsync(
      Guid project,
      string feedId,
      Guid packageId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeedHttpClientBase feedHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("61d885fd-10f3-4a55-82b6-476d866b673f");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageId = packageId
      };
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await feedHttpClientBase.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("7.2-preview.1"), cancellationToken: cancellationToken, mediaType: "image/svg+xml").ConfigureAwait(false))
        httpResponseMessage = await feedHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<FeedCapabilities> GetCapabilitiesAsync(
      Guid feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<FeedCapabilities>(new HttpMethod("GET"), new Guid("2fbf20fd-376a-4d12-95c6-6876b759cd25"), (object) new
      {
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<FeedCapabilities> GetCapabilitiesAsync(
      string project,
      Guid feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<FeedCapabilities>(new HttpMethod("GET"), new Guid("2fbf20fd-376a-4d12-95c6-6876b759cd25"), (object) new
      {
        project = project,
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<FeedCapabilities> GetCapabilitiesAsync(
      Guid project,
      Guid feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<FeedCapabilities>(new HttpMethod("GET"), new Guid("2fbf20fd-376a-4d12-95c6-6876b759cd25"), (object) new
      {
        project = project,
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task UpdateCapabilityAsync(
      FeedCapabilities capabilities,
      Guid feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeedHttpClientBase feedHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("2fbf20fd-376a-4d12-95c6-6876b759cd25");
      object obj1 = (object) new{ feedId = feedId };
      HttpContent httpContent = (HttpContent) new ObjectContent<FeedCapabilities>(capabilities, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      FeedHttpClientBase feedHttpClientBase2 = feedHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await feedHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task UpdateCapabilityAsync(
      FeedCapabilities capabilities,
      string project,
      Guid feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeedHttpClientBase feedHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("2fbf20fd-376a-4d12-95c6-6876b759cd25");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<FeedCapabilities>(capabilities, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      FeedHttpClientBase feedHttpClientBase2 = feedHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await feedHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task UpdateCapabilityAsync(
      FeedCapabilities capabilities,
      Guid project,
      Guid feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeedHttpClientBase feedHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("2fbf20fd-376a-4d12-95c6-6876b759cd25");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<FeedCapabilities>(capabilities, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      FeedHttpClientBase feedHttpClientBase2 = feedHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await feedHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task RunBatchAsync(
      FeedBatchData data,
      Guid feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeedHttpClientBase feedHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("858e27b5-e7f3-4237-8feb-730e72821b8a");
      object obj1 = (object) new{ feedId = feedId };
      HttpContent httpContent = (HttpContent) new ObjectContent<FeedBatchData>(data, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      FeedHttpClientBase feedHttpClientBase2 = feedHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await feedHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task RunBatchAsync(
      FeedBatchData data,
      string project,
      Guid feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeedHttpClientBase feedHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("858e27b5-e7f3-4237-8feb-730e72821b8a");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<FeedBatchData>(data, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      FeedHttpClientBase feedHttpClientBase2 = feedHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await feedHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task RunBatchAsync(
      FeedBatchData data,
      Guid project,
      Guid feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeedHttpClientBase feedHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("858e27b5-e7f3-4237-8feb-730e72821b8a");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<FeedBatchData>(data, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      FeedHttpClientBase feedHttpClientBase2 = feedHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await feedHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<FeedChange> GetFeedChangeAsync(
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<FeedChange>(new HttpMethod("GET"), new Guid("29ba2dad-389a-4661-b5d3-de76397ca05b"), (object) new
      {
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<FeedChange> GetFeedChangeAsync(
      string project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<FeedChange>(new HttpMethod("GET"), new Guid("29ba2dad-389a-4661-b5d3-de76397ca05b"), (object) new
      {
        project = project,
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<FeedChange> GetFeedChangeAsync(
      Guid project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<FeedChange>(new HttpMethod("GET"), new Guid("29ba2dad-389a-4661-b5d3-de76397ca05b"), (object) new
      {
        project = project,
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<FeedChangesResponse> GetFeedChangesAsync(
      string project,
      bool? includeDeleted = null,
      long? continuationToken = null,
      int? batchSize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("29ba2dad-389a-4661-b5d3-de76397ca05b");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeDeleted.HasValue)
        keyValuePairList.Add(nameof (includeDeleted), includeDeleted.Value.ToString());
      if (continuationToken.HasValue)
        keyValuePairList.Add(nameof (continuationToken), continuationToken.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (batchSize.HasValue)
        keyValuePairList.Add(nameof (batchSize), batchSize.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<FeedChangesResponse>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<FeedChangesResponse> GetFeedChangesAsync(
      Guid project,
      bool? includeDeleted = null,
      long? continuationToken = null,
      int? batchSize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("29ba2dad-389a-4661-b5d3-de76397ca05b");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeDeleted.HasValue)
        keyValuePairList.Add(nameof (includeDeleted), includeDeleted.Value.ToString());
      if (continuationToken.HasValue)
        keyValuePairList.Add(nameof (continuationToken), continuationToken.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (batchSize.HasValue)
        keyValuePairList.Add(nameof (batchSize), batchSize.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<FeedChangesResponse>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<FeedChangesResponse> GetFeedChangesAsync(
      bool? includeDeleted = null,
      long? continuationToken = null,
      int? batchSize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("29ba2dad-389a-4661-b5d3-de76397ca05b");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeDeleted.HasValue)
        keyValuePairList.Add(nameof (includeDeleted), includeDeleted.Value.ToString());
      if (continuationToken.HasValue)
        keyValuePairList.Add(nameof (continuationToken), continuationToken.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (batchSize.HasValue)
        keyValuePairList.Add(nameof (batchSize), batchSize.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<FeedChangesResponse>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<FeedIdsResult>> GetFeedIdsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<FeedIdsResult>>(new HttpMethod("GET"), new Guid("84a58441-8918-432f-b2ad-332814928cbc"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<FeedIdsResult>> GetFeedIdsAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<FeedIdsResult>>(new HttpMethod("GET"), new Guid("84a58441-8918-432f-b2ad-332814928cbc"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<FeedIdsResult>> GetFeedIdsAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<FeedIdsResult>>(new HttpMethod("GET"), new Guid("84a58441-8918-432f-b2ad-332814928cbc"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>> GetFeedsFromRecycleBinAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>>(new HttpMethod("GET"), new Guid("0cee643d-beb9-41f8-9368-3ada763a8344"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>> GetFeedsFromRecycleBinAsync(
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>>(new HttpMethod("GET"), new Guid("0cee643d-beb9-41f8-9368-3ada763a8344"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>> GetFeedsFromRecycleBinAsync(
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>>(new HttpMethod("GET"), new Guid("0cee643d-beb9-41f8-9368-3ada763a8344"), (object) new
      {
        project = project
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task PermanentDeleteFeedAsync(
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("0cee643d-beb9-41f8-9368-3ada763a8344"), (object) new
      {
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task PermanentDeleteFeedAsync(
      string project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("0cee643d-beb9-41f8-9368-3ada763a8344"), (object) new
      {
        project = project,
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task PermanentDeleteFeedAsync(
      Guid project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("0cee643d-beb9-41f8-9368-3ada763a8344"), (object) new
      {
        project = project,
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task RestoreDeletedFeedAsync(
      JsonPatchDocument patchJson,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeedHttpClientBase feedHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("0cee643d-beb9-41f8-9368-3ada763a8344");
      object obj1 = (object) new{ feedId = feedId };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchJson, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      FeedHttpClientBase feedHttpClientBase2 = feedHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await feedHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task RestoreDeletedFeedAsync(
      JsonPatchDocument patchJson,
      string project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeedHttpClientBase feedHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("0cee643d-beb9-41f8-9368-3ada763a8344");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchJson, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      FeedHttpClientBase feedHttpClientBase2 = feedHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await feedHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task RestoreDeletedFeedAsync(
      JsonPatchDocument patchJson,
      Guid project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeedHttpClientBase feedHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("0cee643d-beb9-41f8-9368-3ada763a8344");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchJson, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      FeedHttpClientBase feedHttpClientBase2 = feedHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await feedHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> CreateFeedAsync(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("c65009a7-474a-4ad1-8b42-7d852107ef8c");
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(feed, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> CreateFeedAsync(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("c65009a7-474a-4ad1-8b42-7d852107ef8c");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(feed, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> CreateFeedAsync(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid project,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("c65009a7-474a-4ad1-8b42-7d852107ef8c");
      object obj1 = (object) new{ project = project };
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(feed, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteFeedAsync(
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("c65009a7-474a-4ad1-8b42-7d852107ef8c"), (object) new
      {
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteFeedAsync(
      string project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("c65009a7-474a-4ad1-8b42-7d852107ef8c"), (object) new
      {
        project = project,
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteFeedAsync(
      Guid project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("c65009a7-474a-4ad1-8b42-7d852107ef8c"), (object) new
      {
        project = project,
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeedAsync(
      string project,
      string feedId,
      bool? includeDeletedUpstreams = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c65009a7-474a-4ad1-8b42-7d852107ef8c");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeDeletedUpstreams.HasValue)
        keyValuePairList.Add(nameof (includeDeletedUpstreams), includeDeletedUpstreams.Value.ToString());
      return this.SendAsync<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeedAsync(
      Guid project,
      string feedId,
      bool? includeDeletedUpstreams = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c65009a7-474a-4ad1-8b42-7d852107ef8c");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeDeletedUpstreams.HasValue)
        keyValuePairList.Add(nameof (includeDeletedUpstreams), includeDeletedUpstreams.Value.ToString());
      return this.SendAsync<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeedAsync(
      string feedId,
      bool? includeDeletedUpstreams = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c65009a7-474a-4ad1-8b42-7d852107ef8c");
      object routeValues = (object) new{ feedId = feedId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeDeletedUpstreams.HasValue)
        keyValuePairList.Add(nameof (includeDeletedUpstreams), includeDeletedUpstreams.Value.ToString());
      return this.SendAsync<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>> GetFeedsAsync(
      string project,
      FeedRole? feedRole = null,
      bool? includeDeletedUpstreams = null,
      bool? includeUrls = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c65009a7-474a-4ad1-8b42-7d852107ef8c");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (feedRole.HasValue)
        keyValuePairList.Add(nameof (feedRole), feedRole.Value.ToString());
      bool flag;
      if (includeDeletedUpstreams.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDeletedUpstreams.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeletedUpstreams), str);
      }
      if (includeUrls.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeUrls.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeUrls), str);
      }
      return this.SendAsync<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>> GetFeedsAsync(
      Guid project,
      FeedRole? feedRole = null,
      bool? includeDeletedUpstreams = null,
      bool? includeUrls = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c65009a7-474a-4ad1-8b42-7d852107ef8c");
      object routeValues = (object) new{ project = project };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (feedRole.HasValue)
        keyValuePairList.Add(nameof (feedRole), feedRole.Value.ToString());
      bool flag;
      if (includeDeletedUpstreams.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDeletedUpstreams.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeletedUpstreams), str);
      }
      if (includeUrls.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeUrls.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeUrls), str);
      }
      return this.SendAsync<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>> GetFeedsAsync(
      FeedRole? feedRole = null,
      bool? includeDeletedUpstreams = null,
      bool? includeUrls = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("c65009a7-474a-4ad1-8b42-7d852107ef8c");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (feedRole.HasValue)
        keyValuePairList.Add(nameof (feedRole), feedRole.Value.ToString());
      bool flag;
      if (includeDeletedUpstreams.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDeletedUpstreams.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeletedUpstreams), str);
      }
      if (includeUrls.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeUrls.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeUrls), str);
      }
      return this.SendAsync<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> UpdateFeedAsync(
      FeedUpdate feed,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("c65009a7-474a-4ad1-8b42-7d852107ef8c");
      object obj1 = (object) new{ feedId = feedId };
      HttpContent httpContent = (HttpContent) new ObjectContent<FeedUpdate>(feed, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> UpdateFeedAsync(
      FeedUpdate feed,
      string project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("c65009a7-474a-4ad1-8b42-7d852107ef8c");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<FeedUpdate>(feed, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> UpdateFeedAsync(
      FeedUpdate feed,
      Guid project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("c65009a7-474a-4ad1-8b42-7d852107ef8c");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<FeedUpdate>(feed, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<GlobalPermission>> GetGlobalPermissionsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<GlobalPermission>>(new HttpMethod("GET"), new Guid("a74419ef-b477-43df-8758-3cd1cd5f56c6"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GlobalPermission>> GetGlobalPermissionsAsync(
      bool includeIds,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a74419ef-b477-43df-8758-3cd1cd5f56c6");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (includeIds), includeIds.ToString());
      return this.SendAsync<List<GlobalPermission>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<GlobalPermission>> SetGlobalPermissionsAsync(
      IEnumerable<GlobalPermission> globalPermissions,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("a74419ef-b477-43df-8758-3cd1cd5f56c6");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<GlobalPermission>>(globalPermissions, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<GlobalPermission>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<PackageChangesResponse> GetPackageChangesAsync(
      string project,
      string feedId,
      long? continuationToken = null,
      int? batchSize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("323a0631-d083-4005-85ae-035114dfb681");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken.HasValue)
        keyValuePairList.Add(nameof (continuationToken), continuationToken.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (batchSize.HasValue)
        keyValuePairList.Add(nameof (batchSize), batchSize.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<PackageChangesResponse>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PackageChangesResponse> GetPackageChangesAsync(
      Guid project,
      string feedId,
      long? continuationToken = null,
      int? batchSize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("323a0631-d083-4005-85ae-035114dfb681");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken.HasValue)
        keyValuePairList.Add(nameof (continuationToken), continuationToken.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (batchSize.HasValue)
        keyValuePairList.Add(nameof (batchSize), batchSize.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<PackageChangesResponse>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PackageChangesResponse> GetPackageChangesAsync(
      string feedId,
      long? continuationToken = null,
      int? batchSize = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("323a0631-d083-4005-85ae-035114dfb681");
      object routeValues = (object) new{ feedId = feedId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (continuationToken.HasValue)
        keyValuePairList.Add(nameof (continuationToken), continuationToken.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      if (batchSize.HasValue)
        keyValuePairList.Add(nameof (batchSize), batchSize.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<PackageChangesResponse>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<PackageMetrics>> QueryPackageMetricsAsync(
      PackageMetricsQuery packageIdQuery,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("bddc9b3c-8a59-4a9f-9b40-ee1dcaa2cc0d");
      object obj1 = (object) new{ feedId = feedId };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageMetricsQuery>(packageIdQuery, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<PackageMetrics>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<PackageMetrics>> QueryPackageMetricsAsync(
      PackageMetricsQuery packageIdQuery,
      string project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("bddc9b3c-8a59-4a9f-9b40-ee1dcaa2cc0d");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageMetricsQuery>(packageIdQuery, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<PackageMetrics>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<PackageMetrics>> QueryPackageMetricsAsync(
      PackageMetricsQuery packageIdQuery,
      Guid project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("bddc9b3c-8a59-4a9f-9b40-ee1dcaa2cc0d");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageMetricsQuery>(packageIdQuery, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<PackageMetrics>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<Package> GetPackageAsync(
      string project,
      string feedId,
      string packageId,
      bool? includeAllVersions = null,
      bool? includeUrls = null,
      bool? isListed = null,
      bool? isRelease = null,
      bool? includeDeleted = null,
      bool? includeDescription = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7a20d846-c929-4acc-9ea2-0d5a7df1b197");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageId = packageId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeAllVersions.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeAllVersions.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAllVersions), str);
      }
      if (includeUrls.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeUrls.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeUrls), str);
      }
      if (isListed.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = isListed.Value;
        string str = flag.ToString();
        collection.Add(nameof (isListed), str);
      }
      if (isRelease.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = isRelease.Value;
        string str = flag.ToString();
        collection.Add(nameof (isRelease), str);
      }
      if (includeDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeleted), str);
      }
      if (includeDescription.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeDescription.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDescription), str);
      }
      return this.SendAsync<Package>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Package> GetPackageAsync(
      Guid project,
      string feedId,
      string packageId,
      bool? includeAllVersions = null,
      bool? includeUrls = null,
      bool? isListed = null,
      bool? isRelease = null,
      bool? includeDeleted = null,
      bool? includeDescription = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7a20d846-c929-4acc-9ea2-0d5a7df1b197");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageId = packageId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeAllVersions.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeAllVersions.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAllVersions), str);
      }
      if (includeUrls.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeUrls.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeUrls), str);
      }
      if (isListed.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = isListed.Value;
        string str = flag.ToString();
        collection.Add(nameof (isListed), str);
      }
      if (isRelease.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = isRelease.Value;
        string str = flag.ToString();
        collection.Add(nameof (isRelease), str);
      }
      if (includeDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeleted), str);
      }
      if (includeDescription.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeDescription.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDescription), str);
      }
      return this.SendAsync<Package>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Package> GetPackageAsync(
      string feedId,
      string packageId,
      bool? includeAllVersions = null,
      bool? includeUrls = null,
      bool? isListed = null,
      bool? isRelease = null,
      bool? includeDeleted = null,
      bool? includeDescription = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7a20d846-c929-4acc-9ea2-0d5a7df1b197");
      object routeValues = (object) new
      {
        feedId = feedId,
        packageId = packageId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeAllVersions.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeAllVersions.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAllVersions), str);
      }
      if (includeUrls.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeUrls.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeUrls), str);
      }
      if (isListed.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = isListed.Value;
        string str = flag.ToString();
        collection.Add(nameof (isListed), str);
      }
      if (isRelease.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = isRelease.Value;
        string str = flag.ToString();
        collection.Add(nameof (isRelease), str);
      }
      if (includeDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeleted), str);
      }
      if (includeDescription.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeDescription.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDescription), str);
      }
      return this.SendAsync<Package>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Package>> GetPackagesAsync(
      string project,
      string feedId,
      string protocolType = null,
      string packageNameQuery = null,
      string normalizedPackageName = null,
      bool? includeUrls = null,
      bool? includeAllVersions = null,
      bool? isListed = null,
      bool? getTopPackageVersions = null,
      bool? isRelease = null,
      bool? includeDescription = null,
      int? top = null,
      int? skip = null,
      bool? includeDeleted = null,
      bool? isCached = null,
      Guid? directUpstreamId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7a20d846-c929-4acc-9ea2-0d5a7df1b197");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (protocolType != null)
        keyValuePairList.Add(nameof (protocolType), protocolType);
      if (packageNameQuery != null)
        keyValuePairList.Add(nameof (packageNameQuery), packageNameQuery);
      if (normalizedPackageName != null)
        keyValuePairList.Add(nameof (normalizedPackageName), normalizedPackageName);
      bool flag;
      if (includeUrls.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeUrls.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeUrls), str);
      }
      if (includeAllVersions.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeAllVersions.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAllVersions), str);
      }
      if (isListed.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isListed.Value;
        string str = flag.ToString();
        collection.Add(nameof (isListed), str);
      }
      if (getTopPackageVersions.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = getTopPackageVersions.Value;
        string str = flag.ToString();
        collection.Add(nameof (getTopPackageVersions), str);
      }
      if (isRelease.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isRelease.Value;
        string str = flag.ToString();
        collection.Add(nameof (isRelease), str);
      }
      if (includeDescription.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDescription.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDescription), str);
      }
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (includeDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeleted), str);
      }
      if (isCached.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isCached.Value;
        string str = flag.ToString();
        collection.Add(nameof (isCached), str);
      }
      if (directUpstreamId.HasValue)
        keyValuePairList.Add(nameof (directUpstreamId), directUpstreamId.Value.ToString());
      return this.SendAsync<List<Package>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Package>> GetPackagesAsync(
      Guid project,
      string feedId,
      string protocolType = null,
      string packageNameQuery = null,
      string normalizedPackageName = null,
      bool? includeUrls = null,
      bool? includeAllVersions = null,
      bool? isListed = null,
      bool? getTopPackageVersions = null,
      bool? isRelease = null,
      bool? includeDescription = null,
      int? top = null,
      int? skip = null,
      bool? includeDeleted = null,
      bool? isCached = null,
      Guid? directUpstreamId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7a20d846-c929-4acc-9ea2-0d5a7df1b197");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (protocolType != null)
        keyValuePairList.Add(nameof (protocolType), protocolType);
      if (packageNameQuery != null)
        keyValuePairList.Add(nameof (packageNameQuery), packageNameQuery);
      if (normalizedPackageName != null)
        keyValuePairList.Add(nameof (normalizedPackageName), normalizedPackageName);
      bool flag;
      if (includeUrls.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeUrls.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeUrls), str);
      }
      if (includeAllVersions.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeAllVersions.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAllVersions), str);
      }
      if (isListed.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isListed.Value;
        string str = flag.ToString();
        collection.Add(nameof (isListed), str);
      }
      if (getTopPackageVersions.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = getTopPackageVersions.Value;
        string str = flag.ToString();
        collection.Add(nameof (getTopPackageVersions), str);
      }
      if (isRelease.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isRelease.Value;
        string str = flag.ToString();
        collection.Add(nameof (isRelease), str);
      }
      if (includeDescription.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDescription.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDescription), str);
      }
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (includeDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeleted), str);
      }
      if (isCached.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isCached.Value;
        string str = flag.ToString();
        collection.Add(nameof (isCached), str);
      }
      if (directUpstreamId.HasValue)
        keyValuePairList.Add(nameof (directUpstreamId), directUpstreamId.Value.ToString());
      return this.SendAsync<List<Package>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Package>> GetPackagesAsync(
      string feedId,
      string protocolType = null,
      string packageNameQuery = null,
      string normalizedPackageName = null,
      bool? includeUrls = null,
      bool? includeAllVersions = null,
      bool? isListed = null,
      bool? getTopPackageVersions = null,
      bool? isRelease = null,
      bool? includeDescription = null,
      int? top = null,
      int? skip = null,
      bool? includeDeleted = null,
      bool? isCached = null,
      Guid? directUpstreamId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7a20d846-c929-4acc-9ea2-0d5a7df1b197");
      object routeValues = (object) new{ feedId = feedId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (protocolType != null)
        keyValuePairList.Add(nameof (protocolType), protocolType);
      if (packageNameQuery != null)
        keyValuePairList.Add(nameof (packageNameQuery), packageNameQuery);
      if (normalizedPackageName != null)
        keyValuePairList.Add(nameof (normalizedPackageName), normalizedPackageName);
      bool flag;
      if (includeUrls.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeUrls.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeUrls), str);
      }
      if (includeAllVersions.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeAllVersions.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAllVersions), str);
      }
      if (isListed.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isListed.Value;
        string str = flag.ToString();
        collection.Add(nameof (isListed), str);
      }
      if (getTopPackageVersions.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = getTopPackageVersions.Value;
        string str = flag.ToString();
        collection.Add(nameof (getTopPackageVersions), str);
      }
      if (isRelease.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isRelease.Value;
        string str = flag.ToString();
        collection.Add(nameof (isRelease), str);
      }
      if (includeDescription.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDescription.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDescription), str);
      }
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (includeDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeleted), str);
      }
      if (isCached.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = isCached.Value;
        string str = flag.ToString();
        collection.Add(nameof (isCached), str);
      }
      if (directUpstreamId.HasValue)
        keyValuePairList.Add(nameof (directUpstreamId), directUpstreamId.Value.ToString());
      return this.SendAsync<List<Package>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<FeedPermission>> GetFeedPermissionsAsync(
      string project,
      string feedId,
      bool? includeIds = null,
      bool? excludeInheritedPermissions = null,
      string identityDescriptor = null,
      bool? includeDeletedFeeds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("be8c1476-86a7-44ed-b19d-aec0e9275cd8");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeIds.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeIds.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeIds), str);
      }
      if (excludeInheritedPermissions.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = excludeInheritedPermissions.Value;
        string str = flag.ToString();
        collection.Add(nameof (excludeInheritedPermissions), str);
      }
      if (identityDescriptor != null)
        keyValuePairList.Add(nameof (identityDescriptor), identityDescriptor);
      if (includeDeletedFeeds.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDeletedFeeds.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeletedFeeds), str);
      }
      return this.SendAsync<List<FeedPermission>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<FeedPermission>> GetFeedPermissionsAsync(
      Guid project,
      string feedId,
      bool? includeIds = null,
      bool? excludeInheritedPermissions = null,
      string identityDescriptor = null,
      bool? includeDeletedFeeds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("be8c1476-86a7-44ed-b19d-aec0e9275cd8");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeIds.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeIds.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeIds), str);
      }
      if (excludeInheritedPermissions.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = excludeInheritedPermissions.Value;
        string str = flag.ToString();
        collection.Add(nameof (excludeInheritedPermissions), str);
      }
      if (identityDescriptor != null)
        keyValuePairList.Add(nameof (identityDescriptor), identityDescriptor);
      if (includeDeletedFeeds.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDeletedFeeds.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeletedFeeds), str);
      }
      return this.SendAsync<List<FeedPermission>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<FeedPermission>> GetFeedPermissionsAsync(
      string feedId,
      bool? includeIds = null,
      bool? excludeInheritedPermissions = null,
      string identityDescriptor = null,
      bool? includeDeletedFeeds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("be8c1476-86a7-44ed-b19d-aec0e9275cd8");
      object routeValues = (object) new{ feedId = feedId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeIds.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeIds.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeIds), str);
      }
      if (excludeInheritedPermissions.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = excludeInheritedPermissions.Value;
        string str = flag.ToString();
        collection.Add(nameof (excludeInheritedPermissions), str);
      }
      if (identityDescriptor != null)
        keyValuePairList.Add(nameof (identityDescriptor), identityDescriptor);
      if (includeDeletedFeeds.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeDeletedFeeds.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeDeletedFeeds), str);
      }
      return this.SendAsync<List<FeedPermission>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<FeedPermission>> SetFeedPermissionsAsync(
      IEnumerable<FeedPermission> feedPermission,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("be8c1476-86a7-44ed-b19d-aec0e9275cd8");
      object obj1 = (object) new{ feedId = feedId };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<FeedPermission>>(feedPermission, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<FeedPermission>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<FeedPermission>> SetFeedPermissionsAsync(
      IEnumerable<FeedPermission> feedPermission,
      string project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("be8c1476-86a7-44ed-b19d-aec0e9275cd8");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<FeedPermission>>(feedPermission, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<FeedPermission>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<FeedPermission>> SetFeedPermissionsAsync(
      IEnumerable<FeedPermission> feedPermission,
      Guid project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("be8c1476-86a7-44ed-b19d-aec0e9275cd8");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<FeedPermission>>(feedPermission, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<FeedPermission>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<PackageVersionProvenance> GetPackageVersionProvenanceAsync(
      string feedId,
      Guid packageId,
      Guid packageVersionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PackageVersionProvenance>(new HttpMethod("GET"), new Guid("0aaeabd4-85cd-4686-8a77-8d31c15690b8"), (object) new
      {
        feedId = feedId,
        packageId = packageId,
        packageVersionId = packageVersionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PackageVersionProvenance> GetPackageVersionProvenanceAsync(
      string project,
      string feedId,
      Guid packageId,
      Guid packageVersionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PackageVersionProvenance>(new HttpMethod("GET"), new Guid("0aaeabd4-85cd-4686-8a77-8d31c15690b8"), (object) new
      {
        project = project,
        feedId = feedId,
        packageId = packageId,
        packageVersionId = packageVersionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PackageVersionProvenance> GetPackageVersionProvenanceAsync(
      Guid project,
      string feedId,
      Guid packageId,
      Guid packageVersionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PackageVersionProvenance>(new HttpMethod("GET"), new Guid("0aaeabd4-85cd-4686-8a77-8d31c15690b8"), (object) new
      {
        project = project,
        feedId = feedId,
        packageId = packageId,
        packageVersionId = packageVersionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<OperationReference> EmptyRecycleBinAsync(
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<OperationReference>(new HttpMethod("DELETE"), new Guid("2704e72c-f541-4141-99be-2004b50b05fa"), (object) new
      {
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<OperationReference> EmptyRecycleBinAsync(
      string project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<OperationReference>(new HttpMethod("DELETE"), new Guid("2704e72c-f541-4141-99be-2004b50b05fa"), (object) new
      {
        project = project,
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<OperationReference> EmptyRecycleBinAsync(
      Guid project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<OperationReference>(new HttpMethod("DELETE"), new Guid("2704e72c-f541-4141-99be-2004b50b05fa"), (object) new
      {
        project = project,
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Package> GetRecycleBinPackageAsync(
      string project,
      string feedId,
      Guid packageId,
      bool? includeUrls = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2704e72c-f541-4141-99be-2004b50b05fa");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageId = packageId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeUrls.HasValue)
        keyValuePairList.Add(nameof (includeUrls), includeUrls.Value.ToString());
      return this.SendAsync<Package>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Package> GetRecycleBinPackageAsync(
      Guid project,
      string feedId,
      Guid packageId,
      bool? includeUrls = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2704e72c-f541-4141-99be-2004b50b05fa");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageId = packageId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeUrls.HasValue)
        keyValuePairList.Add(nameof (includeUrls), includeUrls.Value.ToString());
      return this.SendAsync<Package>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Package> GetRecycleBinPackageAsync(
      string feedId,
      Guid packageId,
      bool? includeUrls = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2704e72c-f541-4141-99be-2004b50b05fa");
      object routeValues = (object) new
      {
        feedId = feedId,
        packageId = packageId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeUrls.HasValue)
        keyValuePairList.Add(nameof (includeUrls), includeUrls.Value.ToString());
      return this.SendAsync<Package>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Package>> GetRecycleBinPackagesAsync(
      string project,
      string feedId,
      string protocolType = null,
      string packageNameQuery = null,
      bool? includeUrls = null,
      int? top = null,
      int? skip = null,
      bool? includeAllVersions = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2704e72c-f541-4141-99be-2004b50b05fa");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (protocolType != null)
        keyValuePairList.Add(nameof (protocolType), protocolType);
      if (packageNameQuery != null)
        keyValuePairList.Add(nameof (packageNameQuery), packageNameQuery);
      bool flag;
      if (includeUrls.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeUrls.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeUrls), str);
      }
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (includeAllVersions.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeAllVersions.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAllVersions), str);
      }
      return this.SendAsync<List<Package>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Package>> GetRecycleBinPackagesAsync(
      Guid project,
      string feedId,
      string protocolType = null,
      string packageNameQuery = null,
      bool? includeUrls = null,
      int? top = null,
      int? skip = null,
      bool? includeAllVersions = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2704e72c-f541-4141-99be-2004b50b05fa");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (protocolType != null)
        keyValuePairList.Add(nameof (protocolType), protocolType);
      if (packageNameQuery != null)
        keyValuePairList.Add(nameof (packageNameQuery), packageNameQuery);
      bool flag;
      if (includeUrls.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeUrls.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeUrls), str);
      }
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (includeAllVersions.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeAllVersions.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAllVersions), str);
      }
      return this.SendAsync<List<Package>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<Package>> GetRecycleBinPackagesAsync(
      string feedId,
      string protocolType = null,
      string packageNameQuery = null,
      bool? includeUrls = null,
      int? top = null,
      int? skip = null,
      bool? includeAllVersions = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("2704e72c-f541-4141-99be-2004b50b05fa");
      object routeValues = (object) new{ feedId = feedId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (protocolType != null)
        keyValuePairList.Add(nameof (protocolType), protocolType);
      if (packageNameQuery != null)
        keyValuePairList.Add(nameof (packageNameQuery), packageNameQuery);
      bool flag;
      if (includeUrls.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeUrls.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeUrls), str);
      }
      int num;
      if (top.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = top.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$top", str);
      }
      if (skip.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        num = skip.Value;
        string str = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        collection.Add("$skip", str);
      }
      if (includeAllVersions.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = includeAllVersions.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeAllVersions), str);
      }
      return this.SendAsync<List<Package>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<RecycleBinPackageVersion> GetRecycleBinPackageVersionAsync(
      string project,
      string feedId,
      Guid packageId,
      Guid packageVersionId,
      bool? includeUrls = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("aceb4be7-8737-4820-834c-4c549e10fdc7");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageId = packageId,
        packageVersionId = packageVersionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeUrls.HasValue)
        keyValuePairList.Add(nameof (includeUrls), includeUrls.Value.ToString());
      return this.SendAsync<RecycleBinPackageVersion>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<RecycleBinPackageVersion> GetRecycleBinPackageVersionAsync(
      Guid project,
      string feedId,
      Guid packageId,
      Guid packageVersionId,
      bool? includeUrls = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("aceb4be7-8737-4820-834c-4c549e10fdc7");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageId = packageId,
        packageVersionId = packageVersionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeUrls.HasValue)
        keyValuePairList.Add(nameof (includeUrls), includeUrls.Value.ToString());
      return this.SendAsync<RecycleBinPackageVersion>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<RecycleBinPackageVersion> GetRecycleBinPackageVersionAsync(
      string feedId,
      Guid packageId,
      Guid packageVersionId,
      bool? includeUrls = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("aceb4be7-8737-4820-834c-4c549e10fdc7");
      object routeValues = (object) new
      {
        feedId = feedId,
        packageId = packageId,
        packageVersionId = packageVersionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeUrls.HasValue)
        keyValuePairList.Add(nameof (includeUrls), includeUrls.Value.ToString());
      return this.SendAsync<RecycleBinPackageVersion>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<RecycleBinPackageVersion>> GetRecycleBinPackageVersionsAsync(
      string project,
      string feedId,
      Guid packageId,
      bool? includeUrls = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("aceb4be7-8737-4820-834c-4c549e10fdc7");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageId = packageId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeUrls.HasValue)
        keyValuePairList.Add(nameof (includeUrls), includeUrls.Value.ToString());
      return this.SendAsync<List<RecycleBinPackageVersion>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<RecycleBinPackageVersion>> GetRecycleBinPackageVersionsAsync(
      Guid project,
      string feedId,
      Guid packageId,
      bool? includeUrls = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("aceb4be7-8737-4820-834c-4c549e10fdc7");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageId = packageId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeUrls.HasValue)
        keyValuePairList.Add(nameof (includeUrls), includeUrls.Value.ToString());
      return this.SendAsync<List<RecycleBinPackageVersion>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<RecycleBinPackageVersion>> GetRecycleBinPackageVersionsAsync(
      string feedId,
      Guid packageId,
      bool? includeUrls = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("aceb4be7-8737-4820-834c-4c549e10fdc7");
      object routeValues = (object) new
      {
        feedId = feedId,
        packageId = packageId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeUrls.HasValue)
        keyValuePairList.Add(nameof (includeUrls), includeUrls.Value.ToString());
      return this.SendAsync<List<RecycleBinPackageVersion>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task PermanentlyDeletePackageVersionAsync(
      string feedId,
      Guid packageId,
      Guid packageVersionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("aceb4be7-8737-4820-834c-4c549e10fdc7"), (object) new
      {
        feedId = feedId,
        packageId = packageId,
        packageVersionId = packageVersionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task PermanentlyDeletePackageVersionAsync(
      string project,
      string feedId,
      Guid packageId,
      Guid packageVersionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("aceb4be7-8737-4820-834c-4c549e10fdc7"), (object) new
      {
        project = project,
        feedId = feedId,
        packageId = packageId,
        packageVersionId = packageVersionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task PermanentlyDeletePackageVersionAsync(
      Guid project,
      string feedId,
      Guid packageId,
      Guid packageVersionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("aceb4be7-8737-4820-834c-4c549e10fdc7"), (object) new
      {
        project = project,
        feedId = feedId,
        packageId = packageId,
        packageVersionId = packageVersionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task UpdateRecycleBinPackageVersionAsync(
      JsonPatchDocument patchJson,
      string feedId,
      Guid packageId,
      Guid packageVersionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeedHttpClientBase feedHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("aceb4be7-8737-4820-834c-4c549e10fdc7");
      object obj1 = (object) new
      {
        feedId = feedId,
        packageId = packageId,
        packageVersionId = packageVersionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchJson, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      FeedHttpClientBase feedHttpClientBase2 = feedHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await feedHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task UpdateRecycleBinPackageVersionAsync(
      JsonPatchDocument patchJson,
      string project,
      string feedId,
      Guid packageId,
      Guid packageVersionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeedHttpClientBase feedHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("aceb4be7-8737-4820-834c-4c549e10fdc7");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageId = packageId,
        packageVersionId = packageVersionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchJson, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      FeedHttpClientBase feedHttpClientBase2 = feedHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await feedHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task UpdateRecycleBinPackageVersionAsync(
      JsonPatchDocument patchJson,
      Guid project,
      string feedId,
      Guid packageId,
      Guid packageVersionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeedHttpClientBase feedHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("aceb4be7-8737-4820-834c-4c549e10fdc7");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageId = packageId,
        packageVersionId = packageVersionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchJson, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      FeedHttpClientBase feedHttpClientBase2 = feedHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await feedHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteFeedRetentionPoliciesAsync(
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("ed52a011-0112-45b5-9f9e-e14efffb3193"), (object) new
      {
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteFeedRetentionPoliciesAsync(
      string project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("ed52a011-0112-45b5-9f9e-e14efffb3193"), (object) new
      {
        project = project,
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteFeedRetentionPoliciesAsync(
      Guid project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("ed52a011-0112-45b5-9f9e-e14efffb3193"), (object) new
      {
        project = project,
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<FeedRetentionPolicy> GetFeedRetentionPoliciesAsync(
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<FeedRetentionPolicy>(new HttpMethod("GET"), new Guid("ed52a011-0112-45b5-9f9e-e14efffb3193"), (object) new
      {
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<FeedRetentionPolicy> GetFeedRetentionPoliciesAsync(
      string project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<FeedRetentionPolicy>(new HttpMethod("GET"), new Guid("ed52a011-0112-45b5-9f9e-e14efffb3193"), (object) new
      {
        project = project,
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<FeedRetentionPolicy> GetFeedRetentionPoliciesAsync(
      Guid project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<FeedRetentionPolicy>(new HttpMethod("GET"), new Guid("ed52a011-0112-45b5-9f9e-e14efffb3193"), (object) new
      {
        project = project,
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<FeedRetentionPolicy> SetFeedRetentionPoliciesAsync(
      FeedRetentionPolicy policy,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("ed52a011-0112-45b5-9f9e-e14efffb3193");
      object obj1 = (object) new{ feedId = feedId };
      HttpContent httpContent = (HttpContent) new ObjectContent<FeedRetentionPolicy>(policy, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<FeedRetentionPolicy>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<FeedRetentionPolicy> SetFeedRetentionPoliciesAsync(
      FeedRetentionPolicy policy,
      string project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("ed52a011-0112-45b5-9f9e-e14efffb3193");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<FeedRetentionPolicy>(policy, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<FeedRetentionPolicy>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<FeedRetentionPolicy> SetFeedRetentionPoliciesAsync(
      FeedRetentionPolicy policy,
      Guid project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("ed52a011-0112-45b5-9f9e-e14efffb3193");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<FeedRetentionPolicy>(policy, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<FeedRetentionPolicy>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<PackageVersionMetrics>> QueryPackageVersionMetricsAsync(
      PackageVersionMetricsQuery packageVersionIdQuery,
      string feedId,
      Guid packageId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e6ae8caa-b6a8-4809-b840-91b2a42c19ad");
      object obj1 = (object) new
      {
        feedId = feedId,
        packageId = packageId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageVersionMetricsQuery>(packageVersionIdQuery, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<PackageVersionMetrics>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<PackageVersionMetrics>> QueryPackageVersionMetricsAsync(
      PackageVersionMetricsQuery packageVersionIdQuery,
      string project,
      string feedId,
      Guid packageId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e6ae8caa-b6a8-4809-b840-91b2a42c19ad");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageId = packageId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageVersionMetricsQuery>(packageVersionIdQuery, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<PackageVersionMetrics>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<PackageVersionMetrics>> QueryPackageVersionMetricsAsync(
      PackageVersionMetricsQuery packageVersionIdQuery,
      Guid project,
      string feedId,
      Guid packageId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("e6ae8caa-b6a8-4809-b840-91b2a42c19ad");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageId = packageId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageVersionMetricsQuery>(packageVersionIdQuery, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<PackageVersionMetrics>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeletePackageVersionAsync(
      string project,
      string feedId,
      string packageId,
      string packageVersionId,
      DateTime deletedDate,
      DateTime scheduledPermanentDeleteDate,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeedHttpClientBase feedHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("3b331909-6a86-44cc-b9ec-c1834c35498f");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageId = packageId,
        packageVersionId = packageVersionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      feedHttpClientBase.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (deletedDate), deletedDate);
      feedHttpClientBase.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (scheduledPermanentDeleteDate), scheduledPermanentDeleteDate);
      using (await feedHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeletePackageVersionAsync(
      Guid project,
      string feedId,
      string packageId,
      string packageVersionId,
      DateTime deletedDate,
      DateTime scheduledPermanentDeleteDate,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeedHttpClientBase feedHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("3b331909-6a86-44cc-b9ec-c1834c35498f");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageId = packageId,
        packageVersionId = packageVersionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      feedHttpClientBase.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (deletedDate), deletedDate);
      feedHttpClientBase.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (scheduledPermanentDeleteDate), scheduledPermanentDeleteDate);
      using (await feedHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task DeletePackageVersionAsync(
      string feedId,
      string packageId,
      string packageVersionId,
      DateTime deletedDate,
      DateTime scheduledPermanentDeleteDate,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeedHttpClientBase feedHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("3b331909-6a86-44cc-b9ec-c1834c35498f");
      object routeValues = (object) new
      {
        feedId = feedId,
        packageId = packageId,
        packageVersionId = packageVersionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      feedHttpClientBase.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (deletedDate), deletedDate);
      feedHttpClientBase.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (scheduledPermanentDeleteDate), scheduledPermanentDeleteDate);
      using (await feedHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<PackageVersion> GetPackageVersionAsync(
      string project,
      string feedId,
      string packageId,
      string packageVersionId,
      bool? includeUrls = null,
      bool? isListed = null,
      bool? isDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3b331909-6a86-44cc-b9ec-c1834c35498f");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageId = packageId,
        packageVersionId = packageVersionId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeUrls.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeUrls.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeUrls), str);
      }
      if (isListed.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = isListed.Value;
        string str = flag.ToString();
        collection.Add(nameof (isListed), str);
      }
      if (isDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = isDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (isDeleted), str);
      }
      return this.SendAsync<PackageVersion>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PackageVersion> GetPackageVersionAsync(
      Guid project,
      string feedId,
      string packageId,
      string packageVersionId,
      bool? includeUrls = null,
      bool? isListed = null,
      bool? isDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3b331909-6a86-44cc-b9ec-c1834c35498f");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageId = packageId,
        packageVersionId = packageVersionId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeUrls.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeUrls.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeUrls), str);
      }
      if (isListed.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = isListed.Value;
        string str = flag.ToString();
        collection.Add(nameof (isListed), str);
      }
      if (isDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = isDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (isDeleted), str);
      }
      return this.SendAsync<PackageVersion>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PackageVersion> GetPackageVersionAsync(
      string feedId,
      string packageId,
      string packageVersionId,
      bool? includeUrls = null,
      bool? isListed = null,
      bool? isDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3b331909-6a86-44cc-b9ec-c1834c35498f");
      object routeValues = (object) new
      {
        feedId = feedId,
        packageId = packageId,
        packageVersionId = packageVersionId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeUrls.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeUrls.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeUrls), str);
      }
      if (isListed.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = isListed.Value;
        string str = flag.ToString();
        collection.Add(nameof (isListed), str);
      }
      if (isDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = isDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (isDeleted), str);
      }
      return this.SendAsync<PackageVersion>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<PackageVersion>> GetPackageVersionsAsync(
      string project,
      string feedId,
      string packageId,
      bool? includeUrls = null,
      bool? isListed = null,
      bool? isDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3b331909-6a86-44cc-b9ec-c1834c35498f");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageId = packageId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeUrls.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeUrls.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeUrls), str);
      }
      if (isListed.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = isListed.Value;
        string str = flag.ToString();
        collection.Add(nameof (isListed), str);
      }
      if (isDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = isDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (isDeleted), str);
      }
      return this.SendAsync<List<PackageVersion>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<PackageVersion>> GetPackageVersionsAsync(
      Guid project,
      string feedId,
      string packageId,
      bool? includeUrls = null,
      bool? isListed = null,
      bool? isDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3b331909-6a86-44cc-b9ec-c1834c35498f");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageId = packageId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeUrls.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeUrls.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeUrls), str);
      }
      if (isListed.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = isListed.Value;
        string str = flag.ToString();
        collection.Add(nameof (isListed), str);
      }
      if (isDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = isDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (isDeleted), str);
      }
      return this.SendAsync<List<PackageVersion>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<PackageVersion>> GetPackageVersionsAsync(
      string feedId,
      string packageId,
      bool? includeUrls = null,
      bool? isListed = null,
      bool? isDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("3b331909-6a86-44cc-b9ec-c1834c35498f");
      object routeValues = (object) new
      {
        feedId = feedId,
        packageId = packageId
      };
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      bool flag;
      if (includeUrls.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = includeUrls.Value;
        string str = flag.ToString();
        collection.Add(nameof (includeUrls), str);
      }
      if (isListed.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = isListed.Value;
        string str = flag.ToString();
        collection.Add(nameof (isListed), str);
      }
      if (isDeleted.HasValue)
      {
        List<KeyValuePair<string, string>> collection = queryParameters;
        flag = isDeleted.Value;
        string str = flag.ToString();
        collection.Add(nameof (isDeleted), str);
      }
      return this.SendAsync<List<PackageVersion>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task UpdatePackageVersionAsync(
      JsonPatchDocument patchJson,
      string feedId,
      string packageId,
      string packageVersionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeedHttpClientBase feedHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("3b331909-6a86-44cc-b9ec-c1834c35498f");
      object obj1 = (object) new
      {
        feedId = feedId,
        packageId = packageId,
        packageVersionId = packageVersionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchJson, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      FeedHttpClientBase feedHttpClientBase2 = feedHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await feedHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task UpdatePackageVersionAsync(
      JsonPatchDocument patchJson,
      string project,
      string feedId,
      string packageId,
      string packageVersionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeedHttpClientBase feedHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("3b331909-6a86-44cc-b9ec-c1834c35498f");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageId = packageId,
        packageVersionId = packageVersionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchJson, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      FeedHttpClientBase feedHttpClientBase2 = feedHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await feedHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task UpdatePackageVersionAsync(
      JsonPatchDocument patchJson,
      Guid project,
      string feedId,
      string packageId,
      string packageVersionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeedHttpClientBase feedHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("3b331909-6a86-44cc-b9ec-c1834c35498f");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        packageId = packageId,
        packageVersionId = packageVersionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<JsonPatchDocument>(patchJson, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), "application/json-patch+json");
      FeedHttpClientBase feedHttpClientBase2 = feedHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await feedHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<FeedView> CreateFeedViewAsync(
      FeedView view,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("42a8502a-6785-41bc-8c16-89477d930877");
      object obj1 = (object) new{ feedId = feedId };
      HttpContent httpContent = (HttpContent) new ObjectContent<FeedView>(view, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<FeedView>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<FeedView> CreateFeedViewAsync(
      FeedView view,
      string project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("42a8502a-6785-41bc-8c16-89477d930877");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<FeedView>(view, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<FeedView>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<FeedView> CreateFeedViewAsync(
      FeedView view,
      Guid project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("42a8502a-6785-41bc-8c16-89477d930877");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<FeedView>(view, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<FeedView>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteFeedViewAsync(
      string feedId,
      string viewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("42a8502a-6785-41bc-8c16-89477d930877"), (object) new
      {
        feedId = feedId,
        viewId = viewId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteFeedViewAsync(
      string project,
      string feedId,
      string viewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("42a8502a-6785-41bc-8c16-89477d930877"), (object) new
      {
        project = project,
        feedId = feedId,
        viewId = viewId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task DeleteFeedViewAsync(
      Guid project,
      string feedId,
      string viewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("42a8502a-6785-41bc-8c16-89477d930877"), (object) new
      {
        project = project,
        feedId = feedId,
        viewId = viewId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<FeedView> GetFeedViewAsync(
      string feedId,
      string viewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<FeedView>(new HttpMethod("GET"), new Guid("42a8502a-6785-41bc-8c16-89477d930877"), (object) new
      {
        feedId = feedId,
        viewId = viewId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<FeedView> GetFeedViewAsync(
      string project,
      string feedId,
      string viewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<FeedView>(new HttpMethod("GET"), new Guid("42a8502a-6785-41bc-8c16-89477d930877"), (object) new
      {
        project = project,
        feedId = feedId,
        viewId = viewId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<FeedView> GetFeedViewAsync(
      Guid project,
      string feedId,
      string viewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<FeedView>(new HttpMethod("GET"), new Guid("42a8502a-6785-41bc-8c16-89477d930877"), (object) new
      {
        project = project,
        feedId = feedId,
        viewId = viewId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<FeedView>> GetFeedViewsAsync(
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<FeedView>>(new HttpMethod("GET"), new Guid("42a8502a-6785-41bc-8c16-89477d930877"), (object) new
      {
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<FeedView>> GetFeedViewsAsync(
      string project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<FeedView>>(new HttpMethod("GET"), new Guid("42a8502a-6785-41bc-8c16-89477d930877"), (object) new
      {
        project = project,
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<FeedView>> GetFeedViewsAsync(
      Guid project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<FeedView>>(new HttpMethod("GET"), new Guid("42a8502a-6785-41bc-8c16-89477d930877"), (object) new
      {
        project = project,
        feedId = feedId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<FeedView> UpdateFeedViewAsync(
      FeedView view,
      string feedId,
      string viewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("42a8502a-6785-41bc-8c16-89477d930877");
      object obj1 = (object) new
      {
        feedId = feedId,
        viewId = viewId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<FeedView>(view, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<FeedView>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<FeedView> UpdateFeedViewAsync(
      FeedView view,
      string project,
      string feedId,
      string viewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("42a8502a-6785-41bc-8c16-89477d930877");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        viewId = viewId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<FeedView>(view, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<FeedView>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<FeedView> UpdateFeedViewAsync(
      FeedView view,
      Guid project,
      string feedId,
      string viewId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("42a8502a-6785-41bc-8c16-89477d930877");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId,
        viewId = viewId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<FeedView>(view, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<FeedView>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
