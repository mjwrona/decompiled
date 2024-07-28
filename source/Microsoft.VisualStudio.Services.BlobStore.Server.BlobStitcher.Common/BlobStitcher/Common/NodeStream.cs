// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.BlobStitcher.Common.NodeStream
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.BlobStitcher.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E75C933D-C085-4E42-931C-50E8D8D54917
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.BlobStitcher.Common.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.BlobStitcher.Common
{
  public class NodeStream : Stream
  {
    private readonly IConcurrentIterator<DedupCompressedBuffer> producer;
    private readonly CancellationToken cancellationToken;
    private DedupCompressedBuffer currentBuffer;
    private int bytesReadFromCurrentBuffer;
    private long position;

    public NodeStream(
      long totalLength,
      IConcurrentIterator<DedupCompressedBuffer> producer,
      CancellationToken cancellationToken)
    {
      this.Length = totalLength;
      this.producer = producer;
      this.cancellationToken = cancellationToken;
      this.currentBuffer = (DedupCompressedBuffer) null;
      this.bytesReadFromCurrentBuffer = -1;
      this.position = 0L;
    }

    public override bool CanRead => true;

    public override bool CanSeek => true;

    public override bool CanWrite => false;

    public override long Length { get; }

    public override long Position
    {
      get => this.position;
      set => throw new NotImplementedException();
    }

    public override void Flush() => throw new NotImplementedException();

    public override async Task<int> ReadAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      if (offset + count > buffer.Length)
        throw new ArgumentOutOfRangeException(nameof (offset), "Reading of the stream went past the total length.");
      int bytesRead = 0;
      while (count > 0)
      {
        ArraySegment<byte> uncompressed;
        if (this.currentBuffer != null)
        {
          int fromCurrentBuffer = this.bytesReadFromCurrentBuffer;
          uncompressed = this.currentBuffer.Uncompressed;
          int count1 = uncompressed.Count;
          if (fromCurrentBuffer == count1)
          {
            this.currentBuffer.Dispose();
            this.currentBuffer = (DedupCompressedBuffer) null;
            this.bytesReadFromCurrentBuffer = -1;
          }
        }
        if (this.currentBuffer == null)
        {
          if (await this.producer.MoveNextAsync(this.cancellationToken).ConfigureAwait(false))
          {
            this.currentBuffer = this.producer.Current;
            this.bytesReadFromCurrentBuffer = 0;
          }
          else
            break;
        }
        int val1 = count;
        uncompressed = this.currentBuffer.Uncompressed;
        int val2 = uncompressed.Count - this.bytesReadFromCurrentBuffer;
        int num = Math.Min(val1, val2);
        uncompressed = this.currentBuffer.Uncompressed;
        byte[] array = uncompressed.Array;
        uncompressed = this.currentBuffer.Uncompressed;
        int srcOffset = uncompressed.Offset + this.bytesReadFromCurrentBuffer;
        byte[] dst = buffer;
        int dstOffset = offset;
        int count2 = num;
        Buffer.BlockCopy((Array) array, srcOffset, (Array) dst, dstOffset, count2);
        offset += num;
        count -= num;
        this.bytesReadFromCurrentBuffer += num;
        bytesRead += num;
        this.position += (long) num;
      }
      return bytesRead;
    }

    public override int Read(byte[] buffer, int offset, int count) => this.ReadAsync(buffer, offset, count, this.cancellationToken).SyncResult<int>();

    public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

    public override void SetLength(long value) => throw new NotImplementedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.currentBuffer?.Dispose();
      base.Dispose(disposing);
    }
  }
}
