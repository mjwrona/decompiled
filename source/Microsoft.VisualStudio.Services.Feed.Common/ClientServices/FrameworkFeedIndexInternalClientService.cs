// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.ClientServices.FrameworkFeedIndexInternalClientService
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Common.ClientServices
{
  public class FrameworkFeedIndexInternalClientService : 
    IFeedIndexInternalClientService,
    IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public Task DeletePackageVersionAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageId,
      string packageVersionId,
      DateTime deletedDate,
      DateTime scheduledPermanentDeleteDate)
    {
      return requestContext.GetClient<FeedInternalHttpClient>().DeletePackageVersionAsync(feed.GetProjectIdentifierString(), feed.Id.ToString(), Uri.EscapeDataString(packageId), Uri.EscapeDataString(packageVersionId), deletedDate, scheduledPermanentDeleteDate);
    }

    public Task<Guid> GetPackageIdAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string protocolType,
      string normalizedPackageName)
    {
      return requestContext.GetClient<FeedInternalHttpClient>().GetPackageIdAsync(feed.GetProjectIdentifierString(), feed.Id.ToString(), protocolType, normalizedPackageName);
    }

    public Task<List<Package>> GetPackagesAsync(
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
      bool includeDescription = false)
    {
      return requestContext.GetClient<FeedInternalHttpClient>().GetPackagesAsync(feed.GetProjectIdentifierString(), feed.Id.ToString(), protocolType, packageNameQuery, normalizedPackageName, new bool?(includeUrls), new bool?(includeAllVersions), isListed, new bool?(getTopPackageVersions), isRelease, new bool?(includeDescription), top, skip, new bool?(includeDeleted));
    }

    public Task PackageVersionViewOperationAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string protocolType,
      string normalizedPackageName,
      string packageVersionId,
      IEnumerable<string> views,
      bool removeViews)
    {
      return requestContext.GetClient<FeedInternalHttpClient>().PackageVersionViewOperationAsync(feed.GetProjectIdentifierString(), feed.Id.ToString(), protocolType, normalizedPackageName, packageVersionId, views, removeViews);
    }

    public Task PackageVersionViewOperationAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      Guid packageId,
      string packageVersionId,
      IEnumerable<string> views,
      bool removeViews)
    {
      return requestContext.GetClient<FeedInternalHttpClient>().PackageVersionViewOperationAsync(feed.GetProjectIdentifierString(), feed.Id.ToString(), packageId, packageVersionId, views, removeViews);
    }

    public Task<PackageIndexEntryResponse> SetIndexEntryAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      PackageIndexEntry indexEntry)
    {
      return requestContext.GetClient<FeedInternalHttpClient>().SetPackageAsync(indexEntry, feed.GetProjectIdentifierString(), feed.Id.ToString());
    }

    public Task UpdatePackageVersionAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string protocolType,
      string normalizedPackageName,
      string packageVersionId,
      bool? isListed)
    {
      return requestContext.GetClient<FeedInternalHttpClient>().UpdatePackageVersionAsync(feed.GetProjectIdentifierString(), feed.Id.ToString(), protocolType, normalizedPackageName, packageVersionId, isListed);
    }

    public Task UpdatePackageVersionAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      Guid packageId,
      string packageVersionId,
      bool? isListed)
    {
      return requestContext.GetClient<FeedInternalHttpClient>().UpdatePackageVersionAsync(feed.GetProjectIdentifierString(), feed.Id.ToString(), packageId, packageVersionId, isListed);
    }

    public Task UpdatePackageVersionAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageId,
      string packageVersionId,
      IEnumerable<PackageFile> files)
    {
      return requestContext.GetClient<FeedInternalHttpClient>().UpdatePackageVersionAsync(feed.GetProjectIdentifierString(), feed.Id.ToString(), packageId, packageVersionId, files);
    }

    public Task UpdatePackageVersionsAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      List<PackageVersionIndexEntryUpdate> updates)
    {
      return requestContext.GetClient<FeedInternalHttpClient>().UpdatePackagesAsync((IEnumerable<PackageVersionIndexEntryUpdate>) updates, feed.GetProjectIdentifierString(), feed.Id.ToString());
    }

    public Task RestorePackageVersionToFeedAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      Guid packageId,
      Guid packageVersionId)
    {
      return requestContext.GetClient<FeedInternalHttpClient>().RestorePackageVersionToFeedAsync(feed.GetProjectIdentifierString(), feed.Id.ToString(), packageId, packageVersionId);
    }

    public Task PermanentlyDeletePackageVersionAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      Guid packageId,
      Guid packageVersionId)
    {
      return requestContext.GetClient<FeedInternalHttpClient>().PermanentlyDeletePackageVersionAsync(feed.GetProjectIdentifierString(), feed.Id.ToString(), packageId, packageVersionId);
    }
  }
}
