// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.ClientServices.IFeedIndexInternalClientService
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
  [DefaultServiceImplementation(typeof (FrameworkFeedIndexInternalClientService))]
  public interface IFeedIndexInternalClientService : IVssFrameworkService
  {
    Task DeletePackageVersionAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageId,
      string packageVersionId,
      DateTime deletedDate,
      DateTime scheduledPermanentDeleteDate);

    Task PackageVersionViewOperationAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string protocolType,
      string normalizedPackageName,
      string packageVersionId,
      IEnumerable<string> views,
      bool removeViews);

    Task PackageVersionViewOperationAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      Guid packageId,
      string packageVersionId,
      IEnumerable<string> views,
      bool removeViews);

    Task<PackageIndexEntryResponse> SetIndexEntryAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      PackageIndexEntry indexEntry);

    Task UpdatePackageVersionAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string protocolType,
      string normalizedPackageName,
      string packageVersionId,
      bool? isListed);

    Task UpdatePackageVersionAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      Guid packageId,
      string packageVersionId,
      bool? isListed);

    Task UpdatePackageVersionAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageId,
      string packageVersionId,
      IEnumerable<PackageFile> files);

    Task UpdatePackageVersionsAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      List<PackageVersionIndexEntryUpdate> updates);

    Task RestorePackageVersionToFeedAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      Guid packageId,
      Guid packageVersionId);

    Task PermanentlyDeletePackageVersionAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      Guid packageId,
      Guid packageVersionId);
  }
}
