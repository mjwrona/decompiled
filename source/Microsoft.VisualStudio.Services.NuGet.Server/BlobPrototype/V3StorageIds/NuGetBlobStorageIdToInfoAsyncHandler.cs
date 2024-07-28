// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.V3StorageIds.NuGetBlobStorageIdToInfoAsyncHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.V3StorageIds
{
  internal class NuGetBlobStorageIdToInfoAsyncHandler : 
    IAsyncHandler<IPackageFileRequest<VssNuGetPackageIdentity, BlobStorageId>, NuGetPackageContentStorageInfo>,
    IHaveInputType<IPackageFileRequest<VssNuGetPackageIdentity, BlobStorageId>>,
    IHaveOutputType<NuGetPackageContentStorageInfo>
  {
    private readonly IAsyncHandler<IPackageFileRequest<VssNuGetPackageIdentity, BlobStorageId>, Uri> storageIdToUriHandler;

    public NuGetBlobStorageIdToInfoAsyncHandler(
      IAsyncHandler<IPackageFileRequest<VssNuGetPackageIdentity, BlobStorageId>, Uri> storageIdToUriHandler)
    {
      this.storageIdToUriHandler = storageIdToUriHandler;
    }

    public async Task<NuGetPackageContentStorageInfo> Handle(
      IPackageFileRequest<VssNuGetPackageIdentity, BlobStorageId> request)
    {
      string blobId = request.AdditionalData.BlobId.ValueString;
      NuGetPackageContentStorageInfo contentStorageInfo = (NuGetPackageContentStorageInfo) new NuGetBlobPackageContentStorageInfo(blobId, await this.storageIdToUriHandler.Handle(request));
      blobId = (string) null;
      return contentStorageInfo;
    }
  }
}
