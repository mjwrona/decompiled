// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.IFeedIndexService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Server.Types;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  [DefaultServiceImplementation(typeof (FeedIndexService))]
  public interface IFeedIndexService : IVssFrameworkService
  {
    IEnumerable<Package> GetPackages(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string protocolType = null,
      string nameQuery = null,
      string normalizedPackageName = null,
      PagingOptions pagingOptions = null,
      ResultOptions resultOptions = null,
      bool? isListed = null,
      bool getTopPackageVersions = false,
      bool? isRelease = null,
      bool? isDeleted = false,
      bool? isCached = null,
      Guid? directUpstreamSourceId = null);

    Package GetPackage(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string packageId,
      ResultOptions resultOptions = null,
      bool? isListed = null,
      bool? isRelease = null,
      bool? isDeleted = false);

    IEnumerable<PackageVersion> GetPackageVersions(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string packageId,
      bool? isListed = null,
      bool? isDeleted = null);

    PackageVersion GetPackageVersion(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string packageId,
      string packageVersionId,
      bool? isListed = null,
      bool? isDeleted = null);

    IEnumerable<PackageDependencyDetails> GetPackageVersionDependencies(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      Guid packageVersionId,
      string protocolType);

    void UpdatePackageVersion(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string packageId,
      string packageVersionId,
      PackageVersion packageVersion);

    void UpdatePackageVersions(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      IEnumerable<PackageVersionUpdate> updates);

    void DeletePackageVersion(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string packageId,
      string packageVersionId,
      DateTime deletedDate,
      DateTime scheduledPermanentDeleteDate);

    void ClearCachedPackages(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, string protocolType);

    PackageIndexEntryResponse SetIndexEntry(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      PackageIndexEntry indexEntry);

    Task<IEnumerable<BuildPackage>> GetPackagesByBuildId(
      IVssRequestContext requestContext,
      string projectId,
      int buildId);
  }
}
