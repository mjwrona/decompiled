// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.ClientServices.PlatformFeedIndexClientService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common.ClientServices;
using Microsoft.VisualStudio.Services.Feed.Server.Types;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Server.ClientServices
{
  public class PlatformFeedIndexClientService : IFeedIndexClientService, IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public async Task<Package> GetPackageAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string protocolType,
      string normalizedPackageName,
      bool includeAllVersions = false,
      bool includeUrls = true,
      bool? isListed = null,
      bool? isRelease = null,
      bool includeDeleted = false)
    {
      Guid packageIdAsync = await this.GetPackageIdAsync(requestContext, feed, protocolType, normalizedPackageName);
      return await this.GetPackageAsync(requestContext, feed, packageIdAsync, includeAllVersions, includeUrls, isListed, isRelease, includeDeleted);
    }

    public Task<Package> GetPackageAsync(
      IVssRequestContext requestContext,
      FeedCore feedCore,
      Guid packageId,
      bool includeAllVersions = false,
      bool includeUrls = true,
      bool? isListed = null,
      bool? isRelease = null,
      bool includeDeleted = false)
    {
      bool? nullable = includeDeleted ? new bool?() : new bool?(false);
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed1 = requestContext.GetService<IFeedService>().GetFeed(requestContext, feedCore.Id.ToString(), feedCore.Project);
      IFeedIndexService service = requestContext.GetService<IFeedIndexService>();
      IVssRequestContext requestContext1 = requestContext;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed2 = feed1;
      string packageId1 = packageId.ToString();
      ResultOptions resultOptions = new ResultOptions();
      resultOptions.IncludeAllVersions = includeAllVersions;
      resultOptions.IncludeDescriptions = false;
      bool? isListed1 = isListed;
      bool? isRelease1 = isRelease;
      bool? isDeleted = nullable;
      Package package = service.GetPackage(requestContext1, feed2, packageId1, resultOptions, isListed1, isRelease1, isDeleted);
      if (includeUrls)
        package.IncludeUrls(requestContext, feed1);
      return Task.FromResult<Package>(package);
    }

    public async Task<Guid> GetPackageIdAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string protocolType,
      string normalizedPackageName)
    {
      List<Package> packagesAsync = await this.GetPackagesAsync(requestContext, feed, protocolType, normalizedPackageName: normalizedPackageName, includeUrls: false, includeDeleted: true);
      return packagesAsync.Count == 1 ? packagesAsync.First<Package>().Id : throw new PackageNotFoundException(protocolType, normalizedPackageName, feed.Id.ToString());
    }

    public Task<List<Package>> GetPackagesAsync(
      IVssRequestContext requestContext,
      FeedCore feedCore,
      string protocolType = null,
      string packageNameQuery = null,
      string normalizedPackageName = null,
      int? top = null,
      int? skip = null,
      bool includeAllVersions = false,
      bool includeUrls = true,
      bool? isListed = null,
      bool? isRelease = null,
      bool getTopPackageVersions = false,
      bool includeDeleted = false,
      bool includeDescription = false,
      bool? isCached = null)
    {
      return this.GetPackagesAsync(requestContext, feedCore.Project, feedCore.Id.ToString(), protocolType, packageNameQuery, normalizedPackageName, top, skip, includeAllVersions, includeUrls, isListed, isRelease, getTopPackageVersions, includeDeleted, includeDescription, isCached);
    }

    public Task<List<Package>> GetPackagesAsync(
      IVssRequestContext requestContext,
      ProjectReference project,
      string feedId,
      string protocolType = null,
      string packageNameQuery = null,
      string normalizedPackageName = null,
      int? top = null,
      int? skip = null,
      bool includeAllVersions = false,
      bool includeUrls = true,
      bool? isListed = null,
      bool? isRelease = null,
      bool getTopPackageVersions = false,
      bool includeDeleted = false,
      bool includeDescription = false,
      bool? isCached = null)
    {
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = requestContext.GetService<IFeedService>().GetFeed(requestContext, feedId, project);
      bool? nullable = includeDeleted ? new bool?() : new bool?(false);
      IFeedIndexService service = requestContext.GetService<IFeedIndexService>();
      IVssRequestContext requestContext1 = requestContext;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed1 = feed;
      string protocolType1 = protocolType;
      string nameQuery = packageNameQuery;
      string normalizedPackageName1 = normalizedPackageName;
      PagingOptions pagingOptions = new PagingOptions();
      pagingOptions.Top = top.GetValueOrDefault(1000);
      pagingOptions.Skip = skip.GetValueOrDefault(0);
      ResultOptions resultOptions = new ResultOptions();
      resultOptions.IncludeAllVersions = includeAllVersions;
      resultOptions.IncludeDescriptions = includeDescription;
      bool? isListed1 = isListed;
      int num = getTopPackageVersions ? 1 : 0;
      bool? isRelease1 = isRelease;
      bool? isDeleted = nullable;
      bool? isCached1 = isCached;
      Guid? directUpstreamSourceId = new Guid?();
      IEnumerable<Package> source = service.GetPackages(requestContext1, feed1, protocolType1, nameQuery, normalizedPackageName1, pagingOptions, resultOptions, isListed1, num != 0, isRelease1, isDeleted, isCached1, directUpstreamSourceId);
      if (includeUrls)
        source = source.Select<Package, Package>((Func<Package, Package>) (p => p.IncludeUrls(requestContext, feed)));
      return Task.FromResult<List<Package>>(source.ToList<Package>());
    }

    public Task<PackageVersion> GetPackageVersionAsync(
      IVssRequestContext requestContext,
      FeedCore feedCore,
      Guid packageGuid,
      string packageVersionId,
      bool includeUrls = true,
      bool? isListed = null,
      bool? isDeleted = null)
    {
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = requestContext.GetService<IFeedService>().GetFeed(requestContext, feedCore.Id.ToString(), feedCore.Project);
      string packageId = packageGuid.ToString();
      PackageVersion packageVersion = requestContext.GetService<IFeedIndexService>().GetPackageVersion(requestContext, feed, packageId, packageVersionId, isListed, isDeleted);
      if (includeUrls)
        packageVersion = packageVersion.IncludeUrls(requestContext, feed, packageId);
      return Task.FromResult<PackageVersion>(packageVersion);
    }

    public async Task<PackageVersion> GetPackageVersionAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string protocolType,
      string normalizedPackageName,
      string packageVersionId,
      bool includeUrls = true,
      bool? isListed = null,
      bool? isDeleted = null)
    {
      Guid packageIdAsync = await this.GetPackageIdAsync(requestContext, feed, protocolType, normalizedPackageName);
      return await this.GetPackageVersionAsync(requestContext, feed, packageIdAsync, packageVersionId, includeUrls, isListed, isDeleted);
    }

    public Task<List<PackageVersion>> GetPackageVersionsAsync(
      IVssRequestContext requestContext,
      FeedCore feedCore,
      string packageId,
      bool includeUrls = true,
      bool? isListed = null)
    {
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = requestContext.GetService<IFeedService>().GetFeed(requestContext, feedCore.Id.ToString(), feedCore.Project);
      IEnumerable<PackageVersion> source = requestContext.GetService<IFeedIndexService>().GetPackageVersions(requestContext, feed, packageId, isListed);
      if (includeUrls)
        source = source.Select<PackageVersion, PackageVersion>((Func<PackageVersion, PackageVersion>) (p => p.IncludeUrls(requestContext, feed, packageId)));
      return Task.FromResult<List<PackageVersion>>(source.ToList<PackageVersion>());
    }

    public async Task<List<PackageVersion>> GetPackageVersionsAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string protocolType,
      string packageName,
      bool includeUrls = true,
      bool? isListed = null)
    {
      Guid packageIdAsync = await this.GetPackageIdAsync(requestContext, feed, protocolType, packageName);
      return await this.GetPackageVersionsAsync(requestContext, feed, packageIdAsync, includeUrls, isListed);
    }

    public async Task<List<PackageVersion>> GetPackageVersionsAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      Guid packageId,
      bool includeUrls = true,
      bool? isListed = null)
    {
      return await this.GetPackageVersionsAsync(requestContext, feed, packageId.ToString(), includeUrls, isListed);
    }
  }
}
