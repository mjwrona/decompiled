// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.ClientServices.IFeedIndexClientService
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Common.ClientServices
{
  [DefaultServiceImplementation(typeof (FrameworkFeedIndexClientService))]
  public interface IFeedIndexClientService : IVssFrameworkService
  {
    Task<Package> GetPackageAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string protocolType,
      string normalizedPackageName,
      bool includeAllVersions = false,
      bool includeUrls = true,
      bool? isListed = null,
      bool? isRelease = null,
      bool includeDeleted = false);

    Task<Package> GetPackageAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      Guid packageId,
      bool includeAllVersions = false,
      bool includeUrls = true,
      bool? isListed = null,
      bool? isRelease = null,
      bool includeDeleted = false);

    Task<Guid> GetPackageIdAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string protocolType,
      string normalizedPackageName);

    Task<List<Package>> GetPackagesAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
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
      bool? isCached = null);

    [Obsolete("*DO NOT USE*. Use the overload that takes a FeedCore instead. This overload only exists for the special use case of FeedIndexSearchService.")]
    Task<List<Package>> GetPackagesAsync(
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
      bool? isCached = null);

    Task<PackageVersion> GetPackageVersionAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      Guid packageId,
      string packageVersionId,
      bool includeUrls = true,
      bool? isListed = null,
      bool? isDeleted = null);

    Task<PackageVersion> GetPackageVersionAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string protocolType,
      string normalizedPackageName,
      string packageVersionId,
      bool includeUrls = true,
      bool? isListed = null,
      bool? isDeleted = null);

    Task<List<PackageVersion>> GetPackageVersionsAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageId,
      bool includeUrls = true,
      bool? isListed = null);

    Task<List<PackageVersion>> GetPackageVersionsAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string protocolType,
      string packageName,
      bool includeUrls = true,
      bool? isListed = null);

    Task<List<PackageVersion>> GetPackageVersionsAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      Guid packageId,
      bool includeUrls = true,
      bool? isListed = null);
  }
}
