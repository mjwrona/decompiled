// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.VsoHashAlgorithm
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public class VsoHashAlgorithm : HashAlgorithm
  {
    private readonly byte[] buffer = new byte[2097152];
    private List<BlobBlockHash> blockHashes = new List<BlobBlockHash>();
    private int currentOffset;
    private BlobIdentifierWithBlocks blobIdentifierWithBlocks;

    public override void Initialize()
    {
      this.currentOffset = 0;
      this.blockHashes.Clear();
      this.blobIdentifierWithBlocks = (BlobIdentifierWithBlocks) null;
    }

    protected override void HashCore(byte[] array, int start, int size)
    {
      int count;
      for (; size > 0; size -= count)
      {
        if (this.currentOffset == this.buffer.Length)
        {
          this.blockHashes.Add(VsoHash.HashBlock(this.buffer, this.buffer.Length));
          this.currentOffset = 0;
        }
        count = Math.Min(size, this.buffer.Length - this.currentOffset);
        Buffer.BlockCopy((Array) array, start, (Array) this.buffer, this.currentOffset, count);
        this.currentOffset += count;
        start += count;
      }
    }

    protected override byte[] HashFinal()
    {
      VsoHash.RollingBlobIdentifierWithBlocks identifierWithBlocks = new VsoHash.RollingBlobIdentifierWithBlocks();
      if (this.currentOffset != 0)
        this.blockHashes.Add(VsoHash.HashBlock(this.buffer, this.currentOffset));
      if (this.blockHashes.Count == 0)
        this.blockHashes.Add(VsoHash.HashBlock(new byte[0], 0));
      for (int index = 0; index < this.blockHashes.Count - 1; ++index)
        identifierWithBlocks.Update(this.blockHashes[index]);
      this.blobIdentifierWithBlocks = identifierWithBlocks.Finalize(this.blockHashes[this.blockHashes.Count - 1]);
      return this.blobIdentifierWithBlocks.BlobId.Bytes;
    }

    public BlobIdentifierWithBlocks GetBlobIdentifierWithBlocks() => (object) this.blobIdentifierWithBlocks != null ? this.blobIdentifierWithBlocks : throw new CryptographicUnexpectedOperationException(Resources.VsoHashAlgorithmNotFinalized());
  }
}
