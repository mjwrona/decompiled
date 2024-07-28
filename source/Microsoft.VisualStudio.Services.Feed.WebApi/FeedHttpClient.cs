// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.FeedHttpClient
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8DACB936-5231-4131-8ED8-082A1F46DC54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.WebApi
{
  public class FeedHttpClient : FeedHttpClientBase
  {
    public FeedHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public FeedHttpClient(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public FeedHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public FeedHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public FeedHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<Microsoft.VisualStudio.Services.Feed.WebApi.Feed> GetFeedAsync(
      string feedId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetFeedAsync(feedId, new bool?(false), userState, cancellationToken);
    }

    public virtual Task<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>> GetFeedsAsync(
      FeedRole? feedRole = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetFeedsAsync(feedRole, new bool?(false), new bool?(true), userState, cancellationToken);
    }

    public virtual Task<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>> GetFeedsAsync(
      FeedRole? feedRole,
      bool? includeDeletedUpstreams,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetFeedsAsync(feedRole, includeDeletedUpstreams, new bool?(true), userState, cancellationToken);
    }

    public virtual Task<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>> GetFeedsAsync(
      string project,
      FeedRole? feedRole,
      bool? includeDeletedUpstreams,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetFeedsAsync(project, feedRole, includeDeletedUpstreams, new bool?(true), userState, cancellationToken);
    }

    public virtual Task<List<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>> GetFeedsAsync(
      Guid project,
      FeedRole? feedRole,
      bool? includeDeletedUpstreams,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.GetFeedsAsync(project, feedRole, includeDeletedUpstreams, new bool?(true), userState, cancellationToken);
    }

    public virtual Task<List<FeedPermission>> GetFeedPermissionsAsync(
      string feedId,
      bool? includeIds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("be8c1476-86a7-44ed-b19d-aec0e9275cd8");
      object routeValues = (object) new{ feedId = feedId };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (includeIds.HasValue)
        keyValuePairList.Add(nameof (includeIds), includeIds.Value.ToString());
      return this.SendAsync<List<FeedPermission>>(method, locationId, routeValues, new ApiResourceVersion("4.1-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [Obsolete("This method is deprecated. Please use the overload that takes a project parameter, which can be set to null for feeds not associated with any project.")]
    public virtual async Task<Guid> GetPackageIdAsync(
      string feedId,
      string protocolType,
      string normalizedPackageName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.GetPackageIdAsync((string) null, feedId, protocolType, normalizedPackageName, userState, cancellationToken);
    }

    public virtual async Task<Guid> GetPackageIdAsync(
      string project,
      string feedId,
      string protocolType,
      string normalizedPackageName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      string project1 = project;
      string feedId1 = feedId;
      string protocolType1 = protocolType;
      string normalizedPackageName1 = normalizedPackageName;
      bool? nullable1 = new bool?(false);
      bool? includeUrls = new bool?(false);
      bool? includeAllVersions = nullable1;
      bool? nullable2 = new bool?(true);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      bool? isListed = new bool?();
      bool? getTopPackageVersions = new bool?();
      bool? isRelease = new bool?();
      bool? includeDescription = new bool?();
      int? top = new int?();
      int? skip = new int?();
      bool? includeDeleted = nullable2;
      bool? isCached = new bool?();
      Guid? directUpstreamId = new Guid?();
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      List<Package> packagesAsync = await this.GetPackagesAsync(project1, feedId1, protocolType1, (string) null, normalizedPackageName1, includeUrls, includeAllVersions, isListed, getTopPackageVersions, isRelease, includeDescription, top, skip, includeDeleted, isCached, directUpstreamId, userState1, cancellationToken2);
      return packagesAsync.Count == 1 ? packagesAsync.First<Package>().Id : throw new PackageNotFoundException(protocolType, normalizedPackageName, feedId);
    }

    [Obsolete("This method is deprecated. Please use the overload that takes a project parameter, which can be set to null for feeds not associated with any project.")]
    public virtual async Task<Package> GetPackageAsync(
      string feedId,
      string protocolType,
      string normalizedPackageName,
      bool includeAllVersions = false,
      bool includeUrls = true,
      bool? isListed = null,
      bool? isRelease = null,
      bool includeDeleted = false,
      bool includeDescription = false,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.GetPackageAsync((string) null, feedId, protocolType, normalizedPackageName, includeAllVersions, includeUrls, isListed, isRelease, includeDeleted, includeDescription, userState, cancellationToken);
    }

    public virtual async Task<Package> GetPackageAsync(
      string project,
      string feedId,
      string protocolType,
      string normalizedPackageName,
      bool includeAllVersions = false,
      bool includeUrls = true,
      bool? isListed = null,
      bool? isRelease = null,
      bool includeDeleted = false,
      bool includeDescription = false,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      Guid packageIdAsync = await this.GetPackageIdAsync(project, feedId, protocolType, normalizedPackageName, userState, cancellationToken);
      // ISSUE: reference to a compiler-generated method
      return await this.\u003C\u003En__0(project, feedId, packageIdAsync.ToString(), new bool?(includeAllVersions), new bool?(includeUrls), isListed, isRelease, new bool?(includeDeleted), new bool?(includeDescription), userState, cancellationToken);
    }

    [Obsolete("This method is deprecated. Please use the overload that takes a project parameter, which can be set to null for feeds not associated with any project.")]
    public virtual async Task<PackageVersion> GetPackageVersionAsync(
      string feedId,
      string protocolType,
      string normalizedPackageName,
      string packageVersionId,
      bool includeUrls = true,
      bool? isListed = null,
      bool? isDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.GetPackageVersionAsync((string) null, feedId, protocolType, normalizedPackageName, packageVersionId, includeUrls, isListed, isDeleted, userState, cancellationToken);
    }

    public virtual async Task<PackageVersion> GetPackageVersionAsync(
      string project,
      string feedId,
      string protocolType,
      string normalizedPackageName,
      string packageVersionId,
      bool includeUrls = true,
      bool? isListed = null,
      bool? isDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      Guid packageIdAsync = await this.GetPackageIdAsync(project, feedId, protocolType, normalizedPackageName, userState, cancellationToken);
      // ISSUE: reference to a compiler-generated method
      return await this.\u003C\u003En__1(project, feedId, packageIdAsync.ToString(), packageVersionId, new bool?(includeUrls), isListed, isDeleted, userState, cancellationToken);
    }

    [Obsolete("This method is deprecated. Please use the overload that takes a project parameter, which can be set to null for feeds not associated with any project.")]
    public virtual async Task<List<PackageVersion>> GetPackageVersionsAsync(
      string feedId,
      string protocolType,
      string normalizedPackageName,
      bool includeUrls = true,
      bool? isListed = null,
      bool? isDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.GetPackageVersionsAsync((string) null, feedId, protocolType, normalizedPackageName, includeUrls, isListed, isDeleted, userState, cancellationToken);
    }

    public virtual async Task<List<PackageVersion>> GetPackageVersionsAsync(
      string project,
      string feedId,
      string protocolType,
      string normalizedPackageName,
      bool includeUrls = true,
      bool? isListed = null,
      bool? isDeleted = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      Guid packageIdAsync = await this.GetPackageIdAsync(project, feedId, protocolType, normalizedPackageName, userState, cancellationToken);
      // ISSUE: reference to a compiler-generated method
      return await this.\u003C\u003En__2(project, feedId, packageIdAsync.ToString(), new bool?(includeUrls), isListed, isDeleted, userState, cancellationToken);
    }
  }
}
