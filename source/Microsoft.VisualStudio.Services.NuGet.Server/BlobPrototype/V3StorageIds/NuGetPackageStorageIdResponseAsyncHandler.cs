// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.V3StorageIds.NuGetPackageStorageIdResponseAsyncHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.V3StorageIds
{
  internal class NuGetPackageStorageIdResponseAsyncHandler : 
    IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, NuGetStorageInfo>,
    IHaveInputType<PackageRequest<VssNuGetPackageIdentity>>,
    IHaveOutputType<NuGetStorageInfo>
  {
    private readonly IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>> requestToPackageStorageIdHandler;
    private readonly IAsyncHandler<IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>, NuGetPackageContentStorageInfo> storageIdToPackageStorageInfoHandler;

    public NuGetPackageStorageIdResponseAsyncHandler(
      IAsyncHandler<PackageRequest<VssNuGetPackageIdentity>, IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>> requestToPackageStorageIdHandler,
      IAsyncHandler<IPackageFileRequest<VssNuGetPackageIdentity, IStorageId>, NuGetPackageContentStorageInfo> storageIdToPackageStorageInfoHandler)
    {
      this.requestToPackageStorageIdHandler = requestToPackageStorageIdHandler;
      this.storageIdToPackageStorageInfoHandler = storageIdToPackageStorageInfoHandler;
    }

    public async Task<NuGetStorageInfo> Handle(PackageRequest<VssNuGetPackageIdentity> request) => new NuGetStorageInfo(await this.storageIdToPackageStorageInfoHandler.Handle(await this.requestToPackageStorageIdHandler.Handle(request)));
  }
}
