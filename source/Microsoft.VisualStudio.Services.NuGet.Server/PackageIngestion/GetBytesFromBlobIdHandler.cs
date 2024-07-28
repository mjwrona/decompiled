// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion.GetBytesFromBlobIdHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion
{
  public class GetBytesFromBlobIdHandler : 
    IAsyncHandler<BlobIdentifier, byte[]>,
    IHaveInputType<BlobIdentifier>,
    IHaveOutputType<byte[]>
  {
    private readonly IContentBlobStore contentBlobStore;

    public GetBytesFromBlobIdHandler(IContentBlobStore contentBlobStore) => this.contentBlobStore = contentBlobStore;

    public async Task<byte[]> Handle(BlobIdentifier blobId)
    {
      byte[] array;
      using (Stream blobAsync = await this.contentBlobStore.GetBlobAsync(blobId))
      {
        using (MemoryStream destination = new MemoryStream())
        {
          blobAsync.CopyTo((Stream) destination);
          array = destination.ToArray();
        }
      }
      return array;
    }
  }
}
