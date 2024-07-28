// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.FeedIndexClientFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common.ClientServices;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class FeedIndexClientFacade : IFeedIndexClient
  {
    private readonly IVssRequestContext requestContext;

    public FeedIndexClientFacade(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public async Task<Package> GetPackageAsync(
      FeedCore feed,
      string protocolType,
      string normalizedPackageName,
      bool includeDeleted,
      bool throwIfNotFound)
    {
      IFeedIndexClientService service = this.requestContext.GetService<IFeedIndexClientService>();
      IVssRequestContext requestContext = this.requestContext;
      FeedCore feed1 = feed;
      string protocolType1 = protocolType;
      string normalizedPackageName1 = normalizedPackageName;
      bool flag = includeDeleted;
      int? top = new int?();
      int? skip = new int?();
      bool? isListed = new bool?();
      bool? isRelease = new bool?();
      int num = flag ? 1 : 0;
      bool? isCached = new bool?();
      List<Package> packagesAsync = await service.GetPackagesAsync(requestContext, feed1, protocolType1, normalizedPackageName: normalizedPackageName1, top: top, skip: skip, isListed: isListed, isRelease: isRelease, includeDeleted: num != 0, isCached: isCached);
      return !(!packagesAsync.Any<Package>() & throwIfNotFound) ? packagesAsync.FirstOrDefault<Package>() : throw new PackageNotFoundException(protocolType, normalizedPackageName, feed.Id.ToString());
    }

    public async Task<PackageVersion> GetPackageVersionAsync(
      FeedCore feed,
      Guid packageId,
      string packageVersion,
      bool throwIfNotFound)
    {
      try
      {
        return await this.requestContext.GetService<IFeedIndexClientService>().GetPackageVersionAsync(this.requestContext, feed, packageId, packageVersion);
      }
      catch (PackageVersionNotFoundException ex) when (!throwIfNotFound)
      {
        return (PackageVersion) null;
      }
    }

    public async Task<PackageIndexEntryResponse> SetIndexEntryAsync(
      FeedCore feed,
      PackageIndexEntry indexEntry)
    {
      return await this.requestContext.GetService<IFeedIndexInternalClientService>().SetIndexEntryAsync(this.requestContext, feed, indexEntry);
    }

    public async Task UpdatePackageVersionAsync(
      FeedCore feed,
      string protocolType,
      string normalizedPackageName,
      string packageVersionId,
      bool? isListed)
    {
      await this.requestContext.GetService<IFeedIndexInternalClientService>().UpdatePackageVersionAsync(this.requestContext, feed, protocolType, normalizedPackageName, packageVersionId, isListed);
    }

    public async Task UpdatePackageVersionsAsync(
      FeedCore feed,
      List<PackageVersionIndexEntryUpdate> updates)
    {
      await this.requestContext.GetService<IFeedIndexInternalClientService>().UpdatePackageVersionsAsync(this.requestContext, feed, updates);
    }

    public async Task UpdatePackageVersionAsync(
      FeedCore feed,
      string packageId,
      string packageVersionId,
      IEnumerable<PackageFile> files)
    {
      await this.requestContext.GetService<IFeedIndexInternalClientService>().UpdatePackageVersionAsync(this.requestContext, feed, packageId, packageVersionId, files);
    }

    public async Task DeletePackageVersionAsync(
      FeedCore feed,
      Guid packageId,
      string packageVersionId,
      DateTime deletedDate,
      DateTime scheduledPermanentDeleteDate)
    {
      await this.requestContext.GetService<IFeedIndexInternalClientService>().DeletePackageVersionAsync(this.requestContext, feed, packageId.ToString(), packageVersionId, deletedDate, scheduledPermanentDeleteDate);
    }

    public async Task PackageVersionViewOperationAsync(
      FeedCore feed,
      Guid packageId,
      string packageVersionId,
      IEnumerable<string> views,
      bool removeViews)
    {
      await this.requestContext.GetService<IFeedIndexInternalClientService>().PackageVersionViewOperationAsync(this.requestContext, feed, packageId, packageVersionId, views, removeViews);
    }

    public async Task RestorePackageVersionToFeedAsync(
      FeedCore feed,
      Guid packageId,
      Guid packageVersionId)
    {
      await this.requestContext.GetService<IFeedIndexInternalClientService>().RestorePackageVersionToFeedAsync(this.requestContext, feed, packageId, packageVersionId);
    }

    public async Task PermanentlyDeletePackageVersionAsync(
      FeedCore feed,
      Guid packageId,
      Guid packageVersionId)
    {
      await this.requestContext.GetService<IFeedIndexInternalClientService>().PermanentlyDeletePackageVersionAsync(this.requestContext, feed, packageId, packageVersionId);
    }

    public async Task ClearCachedPackagesAsync(
      FeedCore feed,
      string protocolType,
      string upstreamId)
    {
      await this.requestContext.GetService<ICachedPackagesInternalClientService>().ClearCachedPackagesAsync(this.requestContext, feed, protocolType, upstreamId);
    }
  }
}
