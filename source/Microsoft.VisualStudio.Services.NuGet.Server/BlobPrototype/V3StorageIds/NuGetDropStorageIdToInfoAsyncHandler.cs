// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.V3StorageIds.NuGetDropStorageIdToInfoAsyncHandler
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
  internal class NuGetDropStorageIdToInfoAsyncHandler : 
    IAsyncHandler<IPackageFileRequest<VssNuGetPackageIdentity, DropStorageId>, NuGetPackageContentStorageInfo>,
    IHaveInputType<IPackageFileRequest<VssNuGetPackageIdentity, DropStorageId>>,
    IHaveOutputType<NuGetPackageContentStorageInfo>
  {
    public Task<NuGetPackageContentStorageInfo> Handle(
      IPackageFileRequest<VssNuGetPackageIdentity, DropStorageId> request)
    {
      return Task.FromResult<NuGetPackageContentStorageInfo>((NuGetPackageContentStorageInfo) new NuGetDropPackageContentStorageInfo(request.AdditionalData.DropName));
    }
  }
}
