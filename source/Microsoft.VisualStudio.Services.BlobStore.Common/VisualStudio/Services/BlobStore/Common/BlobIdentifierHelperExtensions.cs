// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.BlobIdentifierHelperExtensions
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public static class BlobIdentifierHelperExtensions
  {
    public static BlobIdentifier CalculateBlobIdentifier(this Stream blob, IBlobHasher blobHasher)
    {
      blob.Position = 0L;
      BlobIdentifier blobIdentifier = TaskSafety.SyncResultOnThreadPool<BlobIdentifier>((Func<Task<BlobIdentifier>>) (() => blobHasher.CalculateBlobIdentifierAsync(blob)));
      blob.Position = 0L;
      return blobIdentifier;
    }

    public static BlobIdentifierWithBlocks CalculateBlobIdentifierWithBlocks(
      this Stream blob,
      IBlobHasher blobHasher)
    {
      blob.Position = 0L;
      BlobIdentifierWithBlocks identifierWithBlocks = TaskSafety.SyncResultOnThreadPool<BlobIdentifierWithBlocks>((Func<Task<BlobIdentifierWithBlocks>>) (() => blobHasher.CalculateBlobIdentifierWithBlocksAsync(blob)));
      blob.Position = 0L;
      return identifierWithBlocks;
    }

    public static async Task<BlobIdentifierWithBlocks> CalculateBlobIdentifierWithBlocksAsync(
      this Stream blob,
      IBlobHasher blobHasher)
    {
      blob.Position = 0L;
      BlobIdentifierWithBlocks identifierWithBlocksAsync = await blobHasher.CalculateBlobIdentifierWithBlocksAsync(blob).ConfigureAwait(false);
      blob.Position = 0L;
      return identifierWithBlocksAsync;
    }

    public static BlobIdentifierWithBlocks CalculateBlobIdentifierWithBlocks(
      this byte[] blob,
      IBlobHasher blobHasher)
    {
      using (MemoryStream blob1 = new MemoryStream(blob))
        return blob1.CalculateBlobIdentifierWithBlocks(blobHasher);
    }

    public static BlobIdentifier CalculateBlobIdentifier(this byte[] blob, IBlobHasher blobHasher)
    {
      using (MemoryStream blob1 = new MemoryStream(blob))
        return blob1.CalculateBlobIdentifier(blobHasher);
    }

    public static BlobIdentifier CalculateBlobIdentifier(
      this ArraySegment<byte> blob,
      IBlobHasher blobHasher)
    {
      using (MemoryStream blob1 = blob.AsMemoryStream())
        return blob1.CalculateBlobIdentifier(blobHasher);
    }

    public static BlobBlockHash CalculateBlockHash(this byte[] block, IBlobHasher blobHasher) => blobHasher.CalculateBlobBlockHash(block, block.Length);

    public static BlobBlockHash CalculateBlockHash(
      this byte[] block,
      int length,
      IBlobHasher blobHasher)
    {
      return blobHasher.CalculateBlobBlockHash(block, length);
    }

    public static BlobBlockHash CalculateBlockHash(this Stream blob, IBlobHasher blobHasher)
    {
      using (MemoryStream destination = new MemoryStream())
      {
        blob.Position = 0L;
        blob.CopyTo((Stream) destination);
        blob.Position = 0L;
        return destination.ToArray().CalculateBlockHash(blobHasher);
      }
    }

    public static byte[] GetUTF8Bytes(this string str) => StrictEncodingWithoutBOM.UTF8.GetBytes(str);

    [CLSCompliant(false)]
    public static uint MapToIntegerRange(this BlobIdentifier blobId, uint firstValue, uint count)
    {
      if (count == 0U)
        throw new ArgumentException("count must be positive.");
      if (uint.MaxValue - count < firstValue)
        throw new OverflowException("firstValue + count exceeds int.MaxValue");
      return (uint) (((ulong) ((((long) blobId.Bytes[0] << 8 | (long) blobId.Bytes[1]) << 8 | (long) blobId.Bytes[2]) << 8) | (ulong) blobId.Bytes[3]) * (ulong) (count - 1U) / (ulong) uint.MaxValue + (ulong) firstValue);
    }

    public static IBlobHasher GetBlobHasher(this BlobIdentifier blobIdentifier)
    {
      switch (blobIdentifier.AlgorithmId)
      {
        case 0:
          return (IBlobHasher) VsoHash.Instance;
        case 1:
          return (IBlobHasher) ChunkBlobHasher.Instance;
        case 2:
          return (IBlobHasher) NodeBlobHasher.Instance;
        default:
          throw new NotSupportedException(string.Format("Unknown algorithm id encountered: {0}", (object) blobIdentifier.AlgorithmId));
      }
    }

    public static BlobDedupLevel GetBlobDedupLevel(this BlobIdentifier blobIdentifier)
    {
      switch (blobIdentifier.AlgorithmId)
      {
        case 0:
          return BlobDedupLevel.FileLevel;
        case 1:
        case 2:
          return BlobDedupLevel.ChunkLevel;
        default:
          return BlobDedupLevel.Unknown;
      }
    }

    public static bool IsOfNothing(this BlobIdentifier blobIdentifier)
    {
      IBlobHasher blobHasher = blobIdentifier.GetBlobHasher();
      return !(blobHasher is NodeBlobHasher) && blobIdentifier == blobHasher.OfNothing;
    }
  }
}
