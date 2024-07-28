// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.IPackageRecycleBinService
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Server.RecycleBin;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.AzureArtifacts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  [DefaultServiceImplementation(typeof (PackageRecycleBinService))]
  public interface IPackageRecycleBinService : IVssFrameworkService
  {
    IEnumerable<Package> GetPackages(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      string protocolType = null,
      string nameQuery = null,
      int top = 1000,
      int skip = 0,
      bool includeAllVersions = false);

    Package GetPackage(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed, Guid packageId);

    IEnumerable<RecycleBinPackageVersion> GetPackageVersions(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId);

    IEnumerable<DeletedPackageVersion> GetAllPackageVersionsByFeed(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed);

    RecycleBinPackageVersion GetPackageVersion(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      Guid packageVersionId);

    void PermanentlyDeletePackageVersion(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      Guid packageVersionId);

    void RestorePackageVersionToFeed(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      Guid packageId,
      Guid packageVersionId);

    Task<EmptyRecycleBinResult> EmptyRecycleBinAsync(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      DateTime deleteBefore);

    Guid QueueEmptyRecycleBin(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed);
  }
}
