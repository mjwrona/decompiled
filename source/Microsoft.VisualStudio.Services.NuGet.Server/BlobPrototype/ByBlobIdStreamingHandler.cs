// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.ByBlobIdStreamingHandler
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

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class ByBlobIdStreamingHandler : 
    IAsyncHandler<PackageRequest<VssNuGetPackageIdentity, IStorageId>, Stream>,
    IHaveInputType<PackageRequest<VssNuGetPackageIdentity, IStorageId>>,
    IHaveOutputType<Stream>
  {
    private readonly IContentBlobStore contentBlobStore;

    public ByBlobIdStreamingHandler(IContentBlobStore contentBlobStore) => this.contentBlobStore = contentBlobStore;

    public async Task<Stream> Handle(
      PackageRequest<VssNuGetPackageIdentity, IStorageId> request)
    {
      return request.AdditionalData is BlobStorageId additionalData ? await this.contentBlobStore.GetBlobAsync(additionalData.BlobId) : (Stream) null;
    }
  }
}
