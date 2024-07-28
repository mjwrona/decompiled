// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.ByContentBlobStoreStreamingHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.IO;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class ByContentBlobStoreStreamingHandler : 
    IAsyncHandler<
    #nullable disable
    PackageRequest<VssNuGetPackageIdentity>, Stream>,
    IHaveInputType<PackageRequest<VssNuGetPackageIdentity>>,
    IHaveOutputType<Stream>
  {
    private readonly IContentBlobStore contentBlobStore;
    private readonly IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry> metadataService;

    public ByContentBlobStoreStreamingHandler(
      IContentBlobStore contentBlobStore,
      IReadMetadataService<VssNuGetPackageIdentity, INuGetMetadataEntry> metadataService)
    {
      this.contentBlobStore = contentBlobStore;
      this.metadataService = metadataService;
    }

    public async Task<Stream> Handle(PackageRequest<VssNuGetPackageIdentity> request) => await this.contentBlobStore.GetBlobAsync(((BlobStorageId) (await this.metadataService.GetPackageVersionStateAsync((IPackageRequest<VssNuGetPackageIdentity>) request)).PackageStorageId).BlobId);
  }
}
