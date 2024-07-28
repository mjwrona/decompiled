// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.DedupBlobHasher
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public abstract class DedupBlobHasher : IBlobHasher
  {
    private static readonly IContentHasher chunkHasher = DedupSingleChunkHashInfo.Instance.CreateContentHasher();
    private readonly byte algorithmId;

    protected DedupBlobHasher(byte algorithmId) => this.algorithmId = algorithmId;

    byte IBlobHasher.AlgorithmId => this.algorithmId;

    public abstract BlobIdentifier OfNothing { get; }

    public BlobBlockHash CalculateBlobBlockHash(byte[] data, int length) => new BlobBlockHash(DedupBlobHasher.chunkHasher.GetContentHash(data, 0, length).ToHashByteArray());

    public async Task<BlobIdentifier> CalculateBlobIdentifierAsync(Stream stream)
    {
      BlobIdentifier blobIdentifierAsync;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        await stream.CopyToAsync((Stream) memoryStream).ConfigureAwait(false);
        blobIdentifierAsync = new BlobIdentifier(DedupBlobHasher.chunkHasher.GetContentHash(memoryStream.ToArray()).ToHashByteArray(), this.algorithmId);
      }
      return blobIdentifierAsync;
    }

    public async Task<BlobIdentifierWithBlocks> CalculateBlobIdentifierWithBlocksAsync(Stream stream)
    {
      BlobIdentifierWithBlocks identifierWithBlocksAsync;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        await stream.CopyToAsync((Stream) memoryStream).ConfigureAwait(false);
        byte[] hash = DedupBlobHasher.chunkHasher.GetContentHash(memoryStream.ToArray()).ToHashByteArray();
        memoryStream.Position = 0L;
        identifierWithBlocksAsync = new BlobIdentifierWithBlocks(await this.CalculateBlobIdentifierAsync((Stream) memoryStream).ConfigureAwait(false), (IEnumerable<BlobBlockHash>) new BlobBlockHash[1]
        {
          new BlobBlockHash(hash)
        });
      }
      return identifierWithBlocksAsync;
    }

    public BlobIdentifier CalculateBlobIdentifierFromBlobBlockHashes(
      IEnumerable<BlobBlockHash> blocks)
    {
      if (blocks.Count<BlobBlockHash>() != 1)
        throw new ArgumentException("DedupBlobs can only have one block");
      return new BlobIdentifier(blocks.FirstOrDefault<BlobBlockHash>().HashBytes, this.algorithmId);
    }

    public async Task WalkBlocksAsync(
      Stream stream,
      bool multiBlocksInParallel,
      SingleBlockBlobCallbackAsync singleBlockCallback,
      MultiBlockBlobCallbackAsync multiBlockCallback,
      MultiBlockBlobSealCallbackAsync multiBlockSealCallback)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        ConfiguredTaskAwaitable configuredTaskAwaitable = stream.CopyToAsync((Stream) memoryStream).ConfigureAwait(false);
        await configuredTaskAwaitable;
        byte[] data = memoryStream.ToArray();
        memoryStream.Position = 0L;
        configuredTaskAwaitable = singleBlockCallback(data, data.Length, await this.CalculateBlobIdentifierWithBlocksAsync((Stream) memoryStream).ConfigureAwait(false)).ConfigureAwait(false);
        await configuredTaskAwaitable;
        data = (byte[]) null;
      }
    }
  }
}
