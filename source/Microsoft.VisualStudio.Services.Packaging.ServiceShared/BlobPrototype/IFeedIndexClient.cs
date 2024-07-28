// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.IFeedIndexClient
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public interface IFeedIndexClient
  {
    Task<Package> GetPackageAsync(
      FeedCore feed,
      string protocolType,
      string normalizedPackageName,
      bool includeDeleted,
      bool throwIfNotFound);

    Task<PackageVersion> GetPackageVersionAsync(
      FeedCore feed,
      Guid packageId,
      string packageVersion,
      bool throwIfNotFound);

    Task<PackageIndexEntryResponse> SetIndexEntryAsync(FeedCore feed, PackageIndexEntry indexEntry);

    Task UpdatePackageVersionAsync(
      FeedCore feed,
      string protocolType,
      string normalizedPackageName,
      string packageVersionId,
      bool? isListed);

    Task UpdatePackageVersionAsync(
      FeedCore feed,
      string packageId,
      string packageVersionId,
      IEnumerable<PackageFile> files);

    Task UpdatePackageVersionsAsync(FeedCore feed, List<PackageVersionIndexEntryUpdate> updates);

    Task DeletePackageVersionAsync(
      FeedCore feed,
      Guid packageId,
      string packageVersionId,
      DateTime deletedDate,
      DateTime scheduledPermanentDeleteDate);

    Task PackageVersionViewOperationAsync(
      FeedCore feed,
      Guid packageId,
      string packageVersionId,
      IEnumerable<string> views,
      bool removeViews);

    Task RestorePackageVersionToFeedAsync(FeedCore feed, Guid packageId, Guid packageVersionId);

    Task PermanentlyDeletePackageVersionAsync(FeedCore feed, Guid packageId, Guid packageVersionId);

    Task ClearCachedPackagesAsync(FeedCore feed, string protocolType, string upstreamId);
  }
}
