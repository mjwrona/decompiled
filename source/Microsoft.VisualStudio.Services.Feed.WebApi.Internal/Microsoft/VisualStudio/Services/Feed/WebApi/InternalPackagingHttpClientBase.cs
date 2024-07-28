// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.InternalPackagingHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4BC34C1F-0F07-4DDD-8B37-907579B359F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.Internal.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.WebApi
{
  [ResourceArea("7AB4E64E-C4D8-4F50-AE73-5EF2E21642A5")]
  public abstract class InternalPackagingHttpClientBase : FeedHttpClient
  {
    public InternalPackagingHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public InternalPackagingHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public InternalPackagingHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public InternalPackagingHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public InternalPackagingHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task ClearPackageCacheAsync(
      string feedId,
      string protocolType,
      string upstreamId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("6210ac73-e579-4eea-94c9-3d0b0a784f35"), (object) new
      {
        feedId = feedId,
        protocolType = protocolType,
        upstreamId = upstreamId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task ClearPackageCacheAsync(
      string project,
      string feedId,
      string protocolType,
      string upstreamId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("6210ac73-e579-4eea-94c9-3d0b0a784f35"), (object) new
      {
        project = project,
        feedId = feedId,
        protocolType = protocolType,
        upstreamId = upstreamId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task ClearPackageCacheAsync(
      Guid project,
      string feedId,
      string protocolType,
      string upstreamId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("6210ac73-e579-4eea-94c9-3d0b0a784f35"), (object) new
      {
        project = project,
        feedId = feedId,
        protocolType = protocolType,
        upstreamId = upstreamId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeedByIdForAnyScopeAsync(
      Guid feedId,
      bool? includeSoftDeletedFeeds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("afeaa2f0-9a40-41b4-a53e-d6674e31fa27");
      object routeValues = (object) new{ feedId = feedId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeSoftDeletedFeeds.HasValue)
        keyValuePairList.Add(nameof (includeSoftDeletedFeeds), includeSoftDeletedFeeds.Value.ToString());
      return this.SendAsync<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PackageIndexEntryResponse> SetPackageAsync(
      PackageIndexEntry indexEntry,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("599f9c96-f9de-4bcc-9f71-bf6ab23b2b93");
      object obj1 = (object) new{ feedId = feedId };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageIndexEntry>(indexEntry, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PackageIndexEntryResponse>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<PackageIndexEntryResponse> SetPackageAsync(
      PackageIndexEntry indexEntry,
      string project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("599f9c96-f9de-4bcc-9f71-bf6ab23b2b93");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageIndexEntry>(indexEntry, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PackageIndexEntryResponse>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<PackageIndexEntryResponse> SetPackageAsync(
      PackageIndexEntry indexEntry,
      Guid project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("599f9c96-f9de-4bcc-9f71-bf6ab23b2b93");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PackageIndexEntry>(indexEntry, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<PackageIndexEntryResponse>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task UpdatePackagesAsync(
      IEnumerable<PackageVersionIndexEntryUpdate> updates,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      InternalPackagingHttpClientBase packagingHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("599f9c96-f9de-4bcc-9f71-bf6ab23b2b93");
      object obj1 = (object) new{ feedId = feedId };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<PackageVersionIndexEntryUpdate>>(updates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      InternalPackagingHttpClientBase packagingHttpClientBase2 = packagingHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await packagingHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdatePackagesAsync(
      IEnumerable<PackageVersionIndexEntryUpdate> updates,
      string project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      InternalPackagingHttpClientBase packagingHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("599f9c96-f9de-4bcc-9f71-bf6ab23b2b93");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<PackageVersionIndexEntryUpdate>>(updates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      InternalPackagingHttpClientBase packagingHttpClientBase2 = packagingHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await packagingHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdatePackagesAsync(
      IEnumerable<PackageVersionIndexEntryUpdate> updates,
      Guid project,
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      InternalPackagingHttpClientBase packagingHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("599f9c96-f9de-4bcc-9f71-bf6ab23b2b93");
      object obj1 = (object) new
      {
        project = project,
        feedId = feedId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<PackageVersionIndexEntryUpdate>>(updates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      InternalPackagingHttpClientBase packagingHttpClientBase2 = packagingHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await packagingHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<HttpStatusCode> PublishMetricsAsync(
      IEnumerable<PackageMetricsData> packageMetricsUpdates,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("a907c63a-23ca-444e-b847-2ccc6e2ee4f8");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<PackageMetricsData>>(packageMetricsUpdates, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<HttpStatusCode>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
