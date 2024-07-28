// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.DedupCompressedBuffer
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public sealed class DedupCompressedBuffer : IDisposable
  {
    private readonly object syncObject = new object();
    private bool disposed;
    private ArraySegment<byte>? uncompressed;
    private ArraySegment<byte>? compressed;
    private bool? isCompressable;
    private ChunkDedupIdentifier chunkIdentifier;
    private NodeDedupIdentifier nodeIdentifier;
    private IPoolHandle<byte[]> borrowedUncompressed;
    private IPoolHandle<byte[]> borrowedCompressed;

    public static DedupCompressedBuffer FromCompressed(
      IPoolHandle<byte[]> compressed,
      int offset,
      int count)
    {
      return new DedupCompressedBuffer(new ArraySegment<byte>?(), (IPoolHandle<byte[]>) null, new ArraySegment<byte>?(new ArraySegment<byte>(compressed.Value, offset, count)), compressed);
    }

    public static DedupCompressedBuffer FromUncompressed(
      IPoolHandle<byte[]> uncompressed,
      int offset,
      int count)
    {
      return new DedupCompressedBuffer(new ArraySegment<byte>?(new ArraySegment<byte>(uncompressed.Value, offset, count)), uncompressed, new ArraySegment<byte>?(), (IPoolHandle<byte[]>) null);
    }

    public static DedupCompressedBuffer FromCompressed(ArraySegment<byte> compressed) => new DedupCompressedBuffer(new ArraySegment<byte>?(), (IPoolHandle<byte[]>) null, new ArraySegment<byte>?(compressed), (IPoolHandle<byte[]>) null);

    public static DedupCompressedBuffer FromUncompressed(ArraySegment<byte> uncompressed) => new DedupCompressedBuffer(new ArraySegment<byte>?(uncompressed), (IPoolHandle<byte[]>) null, new ArraySegment<byte>?(), (IPoolHandle<byte[]>) null);

    public static DedupCompressedBuffer FromCompressed(byte[] compressed) => DedupCompressedBuffer.FromCompressed(new ArraySegment<byte>(compressed));

    public static DedupCompressedBuffer FromUncompressed(byte[] uncompressed) => DedupCompressedBuffer.FromUncompressed(new ArraySegment<byte>(uncompressed));

    public void AssertValid()
    {
      if (this.disposed)
        throw new ObjectDisposedException(nameof (DedupCompressedBuffer));
      this.AssertInternalsValid();
    }

    private void AssertInternalsValid()
    {
      this.borrowedUncompressed?.AssertValid();
      this.borrowedCompressed?.AssertValid();
    }

    public void Dispose()
    {
      lock (this.syncObject)
      {
        if (this.disposed)
          return;
        this.AssertInternalsValid();
        this.borrowedUncompressed?.Dispose();
        this.borrowedCompressed?.Dispose();
        this.disposed = true;
      }
    }

    private static void WipeBuffer(byte[] buffer)
    {
      for (int index = 0; index < buffer.Length; ++index)
        buffer[index] = (byte) 204;
    }

    private DedupCompressedBuffer(
      ArraySegment<byte>? uncompressed,
      IPoolHandle<byte[]> borrowedUncompressed,
      ArraySegment<byte>? compressed,
      IPoolHandle<byte[]> borrowedCompressed)
    {
      this.uncompressed = uncompressed;
      this.borrowedUncompressed = borrowedUncompressed;
      this.compressed = compressed;
      this.borrowedCompressed = borrowedCompressed;
      this.nodeIdentifier = (NodeDedupIdentifier) null;
      this.chunkIdentifier = (ChunkDedupIdentifier) null;
      if (this.compressed.HasValue)
        this.isCompressable = new bool?(true);
      else
        this.isCompressable = new bool?();
    }

    ~DedupCompressedBuffer() => this.Dispose();

    public bool HasCompressed => this.compressed.HasValue;

    public bool HasUncompressed => this.uncompressed.HasValue;

    public bool TryGetCompressed(out ArraySegment<byte>? compressedBytes)
    {
      lock (this.syncObject)
      {
        if (!this.isCompressable.HasValue)
        {
          this.borrowedCompressed = ChunkerHelper.BorrowChunkBuffer(this.uncompressed.Value.Array.Length);
          if (this.uncompressed.Value.Offset != 0)
            throw new NotImplementedException();
          ArraySegment<byte> arraySegment = this.uncompressed.Value;
          byte[] array = arraySegment.Array;
          arraySegment = this.uncompressed.Value;
          int count1 = arraySegment.Count;
          byte[] compressedChunk = this.borrowedCompressed.Value;
          uint? nullable1 = ChunkCompression.TryCompressChunk(array, (uint) count1, compressedChunk);
          if (nullable1.HasValue)
          {
            uint? nullable2 = nullable1;
            long? nullable3 = nullable2.HasValue ? new long?((long) nullable2.GetValueOrDefault()) : new long?();
            arraySegment = this.uncompressed.Value;
            long count2 = (long) arraySegment.Count;
            if (nullable3.GetValueOrDefault() < count2 & nullable3.HasValue)
            {
              this.compressed = new ArraySegment<byte>?(new ArraySegment<byte>(this.borrowedCompressed.Value, 0, (int) nullable1.Value));
              this.isCompressable = new bool?(true);
              goto label_8;
            }
          }
          this.isCompressable = new bool?(false);
          this.borrowedCompressed.Dispose();
          this.borrowedCompressed = (IPoolHandle<byte[]>) null;
        }
label_8:
        if (this.isCompressable.Value)
        {
          compressedBytes = new ArraySegment<byte>?(this.compressed.Value);
          return true;
        }
        compressedBytes = new ArraySegment<byte>?();
        return false;
      }
    }

    public void GetBytesTryCompress(out bool isCompressed, out ArraySegment<byte> buffer)
    {
      ArraySegment<byte>? compressedBytes;
      if (isCompressed = this.TryGetCompressed(out compressedBytes))
        buffer = compressedBytes.Value;
      else
        buffer = this.uncompressed.Value;
    }

    public void GetBytes(out bool isCompressed, out ArraySegment<byte> buffer)
    {
      lock (this.syncObject)
      {
        isCompressed = this.compressed.HasValue;
        if (isCompressed)
          buffer = this.compressed.Value;
        else
          buffer = this.uncompressed.Value;
      }
    }

    public bool IsCompressable => this.isCompressable.HasValue ? this.isCompressable.Value : this.TryGetCompressed(out ArraySegment<byte>? _);

    public bool TryGetAlreadyCompressed(out ArraySegment<byte>? compressedBytes)
    {
      lock (this.syncObject)
      {
        if (!this.compressed.HasValue)
        {
          compressedBytes = new ArraySegment<byte>?();
          return false;
        }
        compressedBytes = this.compressed;
        return true;
      }
    }

    public ArraySegment<byte> Uncompressed
    {
      get
      {
        this.EnsureUncompressedBufferAvailable();
        return this.uncompressed.Value;
      }
    }

    public ChunkDedupIdentifier ChunkIdentifier
    {
      get
      {
        if ((DedupIdentifier) this.chunkIdentifier == (DedupIdentifier) null)
        {
          this.EnsureUncompressedBufferAvailable();
          lock (this.syncObject)
          {
            if ((DedupIdentifier) this.chunkIdentifier == (DedupIdentifier) null)
              this.chunkIdentifier = ChunkDedupIdentifier.CalculateIdentifier(this.uncompressed.Value);
          }
        }
        return this.chunkIdentifier;
      }
    }

    public NodeDedupIdentifier NodeIdentifier(HashType hashType)
    {
      if ((DedupIdentifier) this.nodeIdentifier == (DedupIdentifier) null)
      {
        this.EnsureUncompressedBufferAvailable();
        lock (this.syncObject)
        {
          if ((DedupIdentifier) this.nodeIdentifier == (DedupIdentifier) null)
            this.nodeIdentifier = NodeDedupIdentifier.CalculateIdentifierFromSerializedNode(this.uncompressed.Value);
        }
      }
      return this.nodeIdentifier;
    }

    private void EnsureUncompressedBufferAvailable()
    {
      if (this.uncompressed.HasValue)
        return;
      lock (this.syncObject)
      {
        if (this.uncompressed.HasValue)
          return;
        this.borrowedUncompressed = ChunkerHelper.BorrowChunkBuffer(this.compressed.Value.Array.Length);
        if (this.compressed.Value.Offset != 0)
          throw new NotImplementedException();
        uint count;
        try
        {
          count = this.DecompressChunk();
        }
        catch (Exception ex)
        {
          this.borrowedUncompressed = ChunkerHelper.BorrowChunkBuffer(ChunkerHelper.GetMaxChunkContentSize());
          count = this.DecompressChunk();
        }
        this.uncompressed = new ArraySegment<byte>?(new ArraySegment<byte>(this.borrowedUncompressed.Value, 0, (int) count));
      }
    }

    private uint DecompressChunk()
    {
      ArraySegment<byte> arraySegment = this.compressed.Value;
      byte[] array = arraySegment.Array;
      arraySegment = this.compressed.Value;
      int count = arraySegment.Count;
      byte[] uncompressedChunk = this.borrowedUncompressed.Value;
      return ChunkCompression.DecompressChunk(array, count, uncompressedChunk);
    }
  }
}
