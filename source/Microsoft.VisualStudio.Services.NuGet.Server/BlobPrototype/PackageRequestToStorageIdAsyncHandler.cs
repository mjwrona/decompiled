// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.PackageRequestToStorageIdAsyncHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Download;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class PackageRequestToStorageIdAsyncHandler : 
    IAsyncHandler<
    #nullable disable
    PackageRequest<VssNuGetPackageIdentity>, IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>>,
    IHaveInputType<PackageRequest<VssNuGetPackageIdentity>>,
    IHaveOutputType<IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>>
  {
    private readonly IMetadataCacheService cacheService;
    private readonly IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry> metadataService;
    private readonly ICache<string, object> requestContextItems;

    public static IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>> Create(
      IMetadataCacheService cacheService,
      IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry> metadataService,
      ICache<string, object> requestContextItems)
    {
      return (IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>>) new PackageRequestToStorageIdAsyncHandler(cacheService, metadataService, requestContextItems);
    }

    public PackageRequestToStorageIdAsyncHandler(
      IMetadataCacheService cacheService,
      IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry> metadataService,
      ICache<string, object> requestContextItems)
    {
      this.cacheService = cacheService;
      this.metadataService = metadataService;
      this.requestContextItems = requestContextItems;
    }

    public async Task<IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>> Handle(
      PackageRequest<VssNuGetPackageIdentity> request)
    {
      PackageFileRequest<VssNuGetPackageIdentity> fileRequest = new PackageFileRequest<VssNuGetPackageIdentity>((IFeedRequest) request, request.PackageId, request.PackageId.ToNupkgFilePath());
      ICachablePackageMetadata packageMetadata;
      if (this.cacheService.TryGetPackageMetadata((IPackageFileRequest) fileRequest, out packageMetadata))
      {
        this.requestContextItems.AddDownloadTelemetry(packageMetadata);
        return this.StorageIdDecoratedRequest((IPackageFileRequest<VssNuGetPackageIdentity>) fileRequest, packageMetadata.StorageId);
      }
      INuGetMetadataEntry versionStateAsync = await this.metadataService.GetPackageVersionStateAsync((IPackageRequest<VssNuGetPackageIdentity>) request);
      this.requestContextItems.AddDownloadTelemetry((IMetadataEntry) versionStateAsync);
      this.cacheService.SetPackageMetadata((IPackageFileRequest) fileRequest, (ICachablePackageMetadata) new CachablePackageMetadata((IMetadataEntry) versionStateAsync));
      return this.StorageIdDecoratedRequest((IPackageFileRequest<VssNuGetPackageIdentity>) fileRequest, versionStateAsync.PackageStorageId);
    }

    private IPackageFileRequest<VssNuGetPackageIdentity, IStorageId> StorageIdDecoratedRequest(
      IPackageFileRequest<VssNuGetPackageIdentity> request,
      IStorageId packageMetadataStorageId)
    {
      switch (packageMetadataStorageId)
      {
        case BlobStorageId data1:
          return (IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>) new PackageFileRequest<VssNuGetPackageIdentity, BlobStorageId>(request, data1);
        case DropStorageId data2:
          return (IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>) new PackageFileRequest<VssNuGetPackageIdentity, DropStorageId>(request, data2);
        default:
          return (IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>) new PackageFileRequest<VssNuGetPackageIdentity, IStorageId>(request, packageMetadataStorageId);
      }
    }
  }
}
