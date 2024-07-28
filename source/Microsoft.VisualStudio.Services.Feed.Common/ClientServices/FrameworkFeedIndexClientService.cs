// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.ClientServices.FrameworkFeedIndexClientService
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Common.ClientServices
{
  public class FrameworkFeedIndexClientService : IFeedIndexClientService, IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public Task<Package> GetPackageAsync(
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
      return requestContext.GetClient<FeedHttpClient>().GetPackageAsync(feed.GetProjectIdentifierString(), feed.Id.ToString(), protocolType, normalizedPackageName, includeAllVersions, includeUrls, isListed, isRelease, includeDeleted);
    }

    public Task<Package> GetPackageAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      Guid packageId,
      bool includeAllVersions = false,
      bool includeUrls = true,
      bool? isListed = null,
      bool? isRelease = null,
      bool includeDeleted = false)
    {
      return requestContext.GetClient<FeedHttpClient>().GetPackageAsync(feed.GetProjectIdentifierString(), feed.Id.ToString(), packageId.ToString(), new bool?(includeAllVersions), new bool?(includeUrls), isListed, isRelease, new bool?(includeDeleted));
    }

    public Task<Guid> GetPackageIdAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string protocolType,
      string normalizedPackageName)
    {
      return requestContext.GetClient<FeedHttpClient>().GetPackageIdAsync(feed.GetProjectIdentifierString(), feed.Id.ToString(), protocolType, normalizedPackageName);
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
      bool includeDescription = false,
      bool? isCached = null)
    {
      IVssRequestContext requestContext1 = requestContext;
      ProjectReference project = feed.Project;
      string feedId = feed.Id.ToString();
      string protocolType1 = protocolType;
      string packageNameQuery1 = packageNameQuery;
      string normalizedPackageName1 = normalizedPackageName;
      bool flag1 = includeUrls;
      bool flag2 = includeAllVersions;
      bool? nullable1 = isListed;
      bool flag3 = getTopPackageVersions;
      bool? nullable2 = isRelease;
      bool flag4 = includeDescription;
      int? top1 = top;
      int? skip1 = skip;
      int num1 = flag2 ? 1 : 0;
      int num2 = flag1 ? 1 : 0;
      bool? isListed1 = nullable1;
      bool? isRelease1 = nullable2;
      int num3 = flag3 ? 1 : 0;
      int num4 = includeDeleted ? 1 : 0;
      int num5 = flag4 ? 1 : 0;
      bool? isCached1 = new bool?();
      return this.GetPackagesAsync(requestContext1, project, feedId, protocolType1, packageNameQuery1, normalizedPackageName1, top1, skip1, num1 != 0, num2 != 0, isListed1, isRelease1, num3 != 0, num4 != 0, num5 != 0, isCached1);
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
      return requestContext.GetClient<FeedHttpClient>().GetPackagesAsync(project.ToProjectIdentifierString(), feedId, protocolType, packageNameQuery, normalizedPackageName, new bool?(includeUrls), new bool?(includeAllVersions), isListed, new bool?(getTopPackageVersions), isRelease, new bool?(includeDescription), top, skip, new bool?(includeDeleted));
    }

    public Task<PackageVersion> GetPackageVersionAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      Guid packageId,
      string packageVersionId,
      bool includeUrls = true,
      bool? isListed = null,
      bool? isDeleted = null)
    {
      return requestContext.GetClient<FeedHttpClient>().GetPackageVersionAsync(feed.GetProjectIdentifierString(), feed.Id.ToString(), packageId.ToString(), Uri.EscapeDataString(packageVersionId), new bool?(includeUrls), isListed, isDeleted, (object) null, new CancellationToken());
    }

    public Task<PackageVersion> GetPackageVersionAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string protocolType,
      string normalizedPackageName,
      string packageVersionId,
      bool includeUrls = true,
      bool? isListed = null,
      bool? isDeleted = null)
    {
      return requestContext.GetClient<FeedHttpClient>().GetPackageVersionAsync(feed.GetProjectIdentifierString(), feed.Id.ToString(), protocolType, normalizedPackageName, Uri.EscapeDataString(packageVersionId), includeUrls, isListed, isDeleted);
    }

    public Task<List<PackageVersion>> GetPackageVersionsAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string packageId,
      bool includeUrls = true,
      bool? isListed = null)
    {
      return requestContext.GetClient<FeedHttpClient>().GetPackageVersionsAsync(feed.GetProjectIdentifierString(), feed.Id.ToString(), Uri.EscapeDataString(packageId), new bool?(includeUrls), isListed, new bool?(), (object) null, new CancellationToken());
    }

    public Task<List<PackageVersion>> GetPackageVersionsAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string protocolType,
      string packageName,
      bool includeUrls = true,
      bool? isListed = null)
    {
      return requestContext.GetClient<FeedHttpClient>().GetPackageVersionsAsync(feed.GetProjectIdentifierString(), feed.Id.ToString(), protocolType, packageName, includeUrls, isListed);
    }

    public Task<List<PackageVersion>> GetPackageVersionsAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      Guid packageId,
      bool includeUrls = true,
      bool? isListed = null)
    {
      return requestContext.GetClient<FeedHttpClient>().GetPackageVersionsAsync(feed.GetProjectIdentifierString(), feed.Id.ToString(), packageId.ToString(), new bool?(includeUrls), isListed, new bool?(), (object) null, new CancellationToken());
    }
  }
}
