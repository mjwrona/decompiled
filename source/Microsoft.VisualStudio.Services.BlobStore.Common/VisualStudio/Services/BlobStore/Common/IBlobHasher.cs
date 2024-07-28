// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.IBlobHasher
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public interface IBlobHasher
  {
    byte AlgorithmId { get; }

    BlobIdentifier OfNothing { get; }

    Task<BlobIdentifier> CalculateBlobIdentifierAsync(Stream data);

    Task<BlobIdentifierWithBlocks> CalculateBlobIdentifierWithBlocksAsync(Stream data);

    BlobBlockHash CalculateBlobBlockHash(byte[] data, int length);

    BlobIdentifier CalculateBlobIdentifierFromBlobBlockHashes(IEnumerable<BlobBlockHash> blocks);

    Task WalkBlocksAsync(
      Stream data,
      bool multiBlocksInParallel,
      SingleBlockBlobCallbackAsync singleBlockCallback,
      MultiBlockBlobCallbackAsync multiBlockCallback,
      MultiBlockBlobSealCallbackAsync multiBlockSealCallback);
  }
}
