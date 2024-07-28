// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion.BlobIdentifierWithOrWithoutBlocks
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion
{
  public class BlobIdentifierWithOrWithoutBlocks
  {
    public BlobIdentifierWithOrWithoutBlocks(Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks blobIdWithBlocks)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks>(blobIdWithBlocks, nameof (blobIdWithBlocks));
      this.BlobIdWithBlocks = blobIdWithBlocks;
      this.BlobId = blobIdWithBlocks.BlobId;
    }

    public BlobIdentifierWithOrWithoutBlocks(BlobIdentifier blobId)
    {
      ArgumentUtility.CheckForNull<BlobIdentifier>(blobId, nameof (blobId));
      this.BlobId = blobId;
    }

    public BlobIdentifier BlobId { get; }

    public Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks BlobIdWithBlocks { get; }

    public override string ToString() => !(this.BlobIdWithBlocks != (Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierWithBlocks) null) ? this.BlobId.ToString() : this.BlobIdWithBlocks.ToString();
  }
}
