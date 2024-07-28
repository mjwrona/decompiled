// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.DedupIdentifierExtensions
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public static class DedupIdentifierExtensions
  {
    public static BlobIdentifier ToBlobIdentifier(this DedupIdentifier dedupId) => new BlobIdentifier(dedupId.AlgorithmResult, dedupId.AlgorithmId);

    public static DedupIdentifier ToDedupIdentifier(this BlobIdentifier blobId) => DedupIdentifier.Create(blobId.AlgorithmResultBytes, blobId.AlgorithmId);
  }
}
