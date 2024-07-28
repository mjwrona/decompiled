// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download.PackageMetadataCacheService
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using System;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download
{
  public abstract class PackageMetadataCacheService : 
    VssMemoryCacheService<PackageMetadataCacheService.Key, ICachablePackageMetadata>,
    IPackageMetadataCacheService,
    IVssFrameworkService
  {
    private static readonly TimeSpan InactivityIntervalDefault = VssCacheExpiryProvider.NoExpiry;
    private static readonly TimeSpan CleanupIntervalDefault = TimeSpan.FromMinutes(15.0);
    private static readonly TimeSpan ExpiryIntervalDefault = TimeSpan.FromHours(1.0);
    private const int MaxElementsDefault = 10000;
    private IVssMemoryCacheGrouping<PackageMetadataCacheService.Key, ICachablePackageMetadata, PackageMetadataCacheService.PackageInvalidationKey> packageInvalidationGrouping;

    protected PackageMetadataCacheService()
      : base((IEqualityComparer<PackageMetadataCacheService.Key>) EqualityComparer<PackageMetadataCacheService.Key>.Default, PackageMetadataCacheService.GetCacheConfiguration())
    {
    }

    private static MemoryCacheConfiguration<PackageMetadataCacheService.Key, ICachablePackageMetadata> GetCacheConfiguration() => MemoryCacheConfiguration<PackageMetadataCacheService.Key, ICachablePackageMetadata>.Default.WithCleanupInterval(PackageMetadataCacheService.CleanupIntervalDefault).WithExpiryInterval(PackageMetadataCacheService.ExpiryIntervalDefault).WithInactivityInterval(PackageMetadataCacheService.InactivityIntervalDefault).WithMaxElements(10000);

    protected abstract Guid StorageInvalidationNotificationGuid { get; }

    public void SetPackageMetadata(
      IVssRequestContext requestContext,
      IPackageFileRequest request,
      ICachablePackageMetadata packageMetadata)
    {
      if (!this.IsEnabled(requestContext) || !packageMetadata.StorageId.IsCacheable)
        return;
      this.Set(requestContext, this.GetKey(request), packageMetadata);
    }

    public void InvalidatePackageMetadata(
      IVssRequestContext requestContext,
      IPackageRequest request)
    {
      if (!this.IsEnabled(requestContext))
        return;
      PackageMetadataCacheService.PackageInvalidationKey invalidationKey = new PackageMetadataCacheService.PackageInvalidationKey(request.Feed.Id, request.PackageId.Name.NormalizedName, request.PackageId.Version.NormalizedVersion);
      this.RemovePackage(requestContext, invalidationKey);
      requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, this.StorageInvalidationNotificationGuid, invalidationKey.Serialize<PackageMetadataCacheService.PackageInvalidationKey>());
    }

    private void RemovePackage(
      IVssRequestContext requestContext,
      PackageMetadataCacheService.PackageInvalidationKey invalidationKey)
    {
      IEnumerable<PackageMetadataCacheService.Key> keys;
      if (!this.packageInvalidationGrouping.TryGetKeys(invalidationKey, out keys))
        return;
      foreach (PackageMetadataCacheService.Key key in keys)
        this.Remove(requestContext, key);
    }

    public bool TryGetPackageMetadata(
      IVssRequestContext requestContext,
      IPackageFileRequest request,
      out ICachablePackageMetadata? packageMetadata)
    {
      if (this.IsEnabled(requestContext))
        return this.TryGetValue(requestContext, this.GetKey(request), out packageMetadata);
      packageMetadata = (ICachablePackageMetadata) null;
      return false;
    }

    protected override void ServiceStart(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      base.ServiceStart(requestContext);
      this.packageInvalidationGrouping = VssMemoryCacheGroupingFactory.Create<PackageMetadataCacheService.Key, ICachablePackageMetadata, PackageMetadataCacheService.PackageInvalidationKey>(requestContext, this.MemoryCache, (Func<PackageMetadataCacheService.Key, ICachablePackageMetadata, IEnumerable<PackageMetadataCacheService.PackageInvalidationKey>>) ((key, _) => (IEnumerable<PackageMetadataCacheService.PackageInvalidationKey>) new PackageMetadataCacheService.PackageInvalidationKey[1]
      {
        new PackageMetadataCacheService.PackageInvalidationKey(key.FeedId, key.NormalizedName, key.NormalizedVersion)
      }));
      requestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(requestContext, "Default", this.StorageInvalidationNotificationGuid, new SqlNotificationCallback(this.OnCachedPackageMetadataNoLongerValid), true);
    }

    protected override void ServiceEnd(IVssRequestContext requestContext)
    {
      requestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(requestContext, "Default", this.StorageInvalidationNotificationGuid, new SqlNotificationCallback(this.OnCachedPackageMetadataNoLongerValid), true);
      base.ServiceEnd(requestContext);
    }

    private bool IsEnabled(IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsHostedDeployment;

    private void OnCachedPackageMetadataNoLongerValid(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      if (!eventData.StartsWith("{"))
        return;
      PackageMetadataCacheService.PackageInvalidationKey invalidationKey = JsonUtilities.Deserialize<PackageMetadataCacheService.PackageInvalidationKey>(eventData);
      this.RemovePackage(requestContext, invalidationKey);
    }

    private PackageMetadataCacheService.Key GetKey(IPackageFileRequest request) => new PackageMetadataCacheService.Key(request.Feed.Id, request.Feed.ViewId, request.PackageId.Name.NormalizedName, request.PackageId.Version.NormalizedVersion, request.FilePath, request is IPackageInnerFileRequest innerFileRequest ? innerFileRequest.InnerFilePath : (string) null);

    public record Key(
      Guid FeedId,
      Guid? ViewId,
      string NormalizedName,
      string NormalizedVersion,
      string FilePath,
      string? InnerFilePath)
    ;

    private record PackageInvalidationKey(
      Guid FeedId,
      string NormalizedName,
      string NormalizedVersion)
    ;
  }
}
