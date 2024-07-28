// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.ClientServices.PlatformFeedIndexInternalClientService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.Common.ClientServices;
using Microsoft.VisualStudio.Services.Feed.Server.Types;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Server.ClientServices
{
  public class PlatformFeedIndexInternalClientService : 
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
      FeedCore feedCore,
      string packageId,
      string packageVersionId,
      DateTime deletedDate,
      DateTime scheduledPermanentDeleteDate)
    {
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = requestContext.GetService<IFeedService>().GetFeed(requestContext, feedCore.Id.ToString(), feedCore.Project).ThrowIfReadOnly(FeedSecurityHelper.CanBypassUnderMaintenance(requestContext));
      requestContext.GetService<IFeedIndexService>().DeletePackageVersion(requestContext, feed, packageId, packageVersionId, deletedDate, scheduledPermanentDeleteDate);
      return Task.CompletedTask;
    }

    public async Task PackageVersionViewOperationAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string protocolType,
      string normalizedPackageName,
      string packageVersionId,
      IEnumerable<string> views,
      bool removeViews)
    {
      Guid packageIdAsync = await this.GetPackageIdAsync(requestContext, feed, protocolType, normalizedPackageName);
      await this.PackageVersionViewOperationAsync(requestContext, feed, packageIdAsync, packageVersionId, views, removeViews);
    }

    public Task PackageVersionViewOperationAsync(
      IVssRequestContext requestContext,
      FeedCore feedCore,
      Guid packageGuid,
      string packageVersionId,
      IEnumerable<string> views,
      bool removeViews)
    {
      string packageId = packageGuid.ToString();
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = requestContext.GetService<IFeedService>().GetFeed(requestContext, feedCore.Id.ToString(), feedCore.Project).ThrowIfReadOnly(FeedSecurityHelper.CanBypassUnderMaintenance(requestContext));
      PackageVersion packageVersion = requestContext.GetService<IFeedIndexService>().GetPackageVersion(requestContext, feed, packageId, packageVersionId);
      if (removeViews)
      {
        packageVersion.AddViews = (IEnumerable<string>) Array.Empty<string>();
        packageVersion.RemoveViews = views;
      }
      else
      {
        packageVersion.AddViews = views;
        packageVersion.RemoveViews = (IEnumerable<string>) Array.Empty<string>();
      }
      requestContext.GetService<IFeedIndexService>().UpdatePackageVersion(requestContext, feed, packageId, packageVersionId, packageVersion);
      return Task.CompletedTask;
    }

    public Task<PackageIndexEntryResponse> SetIndexEntryAsync(
      IVssRequestContext requestContext,
      FeedCore feedCore,
      PackageIndexEntry indexEntry)
    {
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = requestContext.GetService<IFeedService>().GetFeed(requestContext, feedCore.Id.ToString(), feedCore.Project).ThrowIfReadOnly(FeedSecurityHelper.CanBypassUnderMaintenance(requestContext));
      return Task.FromResult<PackageIndexEntryResponse>(requestContext.GetService<IFeedIndexService>().SetIndexEntry(requestContext, feed, indexEntry));
    }

    public async Task UpdatePackageVersionAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string protocolType,
      string normalizedPackageName,
      string packageVersionId,
      bool? isListed)
    {
      Guid packageIdAsync = await this.GetPackageIdAsync(requestContext, feed, protocolType, normalizedPackageName);
      await this.UpdatePackageVersionAsync(requestContext, feed, packageIdAsync, packageVersionId, isListed);
    }

    public Task UpdatePackageVersionAsync(
      IVssRequestContext requestContext,
      FeedCore feedCore,
      Guid packageGuid,
      string packageVersionId,
      bool? isListed)
    {
      string packageId = packageGuid.ToString();
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = requestContext.GetService<IFeedService>().GetFeed(requestContext, feedCore.Id.ToString(), feedCore.Project).ThrowIfReadOnly(FeedSecurityHelper.CanBypassUnderMaintenance(requestContext));
      PackageVersion packageVersion = requestContext.GetService<IFeedIndexService>().GetPackageVersion(requestContext, feed, packageId, packageVersionId);
      if (isListed.HasValue)
        packageVersion.IsListed = isListed.Value;
      requestContext.GetService<IFeedIndexService>().UpdatePackageVersion(requestContext, feed, packageId, packageVersionId, packageVersion);
      return Task.CompletedTask;
    }

    public Task UpdatePackageVersionAsync(
      IVssRequestContext requestContext,
      FeedCore feedCore,
      string packageId,
      string packageVersionId,
      IEnumerable<PackageFile> files)
    {
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = requestContext.GetService<IFeedService>().GetFeed(requestContext, feedCore.Id.ToString(), feedCore.Project).ThrowIfReadOnly(FeedSecurityHelper.CanBypassUnderMaintenance(requestContext));
      PackageVersion packageVersion = requestContext.GetService<IFeedIndexService>().GetPackageVersion(requestContext, feed, packageId, packageVersionId);
      packageVersion.Files = files;
      requestContext.GetService<IFeedIndexService>().UpdatePackageVersion(requestContext, feed, packageId, packageVersionId, packageVersion);
      return Task.CompletedTask;
    }

    public Task UpdatePackageVersionsAsync(
      IVssRequestContext requestContext,
      FeedCore feedCore,
      List<PackageVersionIndexEntryUpdate> updates)
    {
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = requestContext.GetService<IFeedService>().GetFeed(requestContext, feedCore.Id.ToString(), feedCore.Project).ThrowIfReadOnly(FeedSecurityHelper.CanBypassUnderMaintenance(requestContext));
      IEnumerable<PackageVersionUpdate> updates1 = updates.Select<PackageVersionIndexEntryUpdate, PackageVersionUpdate>((Func<PackageVersionIndexEntryUpdate, PackageVersionUpdate>) (x => new PackageVersionUpdate(x.PackageId, x.NormalizedVersion, x.SortableVersion, x.Metadata)));
      requestContext.GetService<IFeedIndexService>().UpdatePackageVersions(requestContext, feed, updates1);
      return Task.CompletedTask;
    }

    public Task RestorePackageVersionToFeedAsync(
      IVssRequestContext requestContext,
      FeedCore feedCore,
      Guid packageId,
      Guid packageVersionId)
    {
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = requestContext.GetService<IFeedService>().GetFeed(requestContext, feedCore.Id.ToString(), feedCore.Project).ThrowIfReadOnly(FeedSecurityHelper.CanBypassUnderMaintenance(requestContext));
      requestContext.GetService<IPackageRecycleBinService>().RestorePackageVersionToFeed(requestContext, feed, packageId, packageVersionId);
      return Task.CompletedTask;
    }

    public Task PermanentlyDeletePackageVersionAsync(
      IVssRequestContext requestContext,
      FeedCore feedCore,
      Guid packageId,
      Guid packageVersionId)
    {
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed = requestContext.GetService<IFeedService>().GetFeed(requestContext, feedCore.Id.ToString(), feedCore.Project).ThrowIfReadOnly(FeedSecurityHelper.CanBypassUnderMaintenance(requestContext));
      requestContext.GetService<IPackageRecycleBinService>().PermanentlyDeletePackageVersion(requestContext, feed, packageId, packageVersionId);
      return Task.CompletedTask;
    }

    private async Task<Guid> GetPackageIdAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string protocolType,
      string normalizedPackageName)
    {
      return await requestContext.GetService<PlatformFeedIndexClientService>().GetPackageIdAsync(requestContext, feed, protocolType, normalizedPackageName);
    }
  }
}
