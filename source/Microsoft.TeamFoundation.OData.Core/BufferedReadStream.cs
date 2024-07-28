// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.BufferedReadStream
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal sealed class BufferedReadStream : Stream
  {
    private readonly List<BufferedReadStream.DataBuffer> buffers;
    private Stream inputStream;
    private int currentBufferIndex;
    private int currentBufferReadCount;

    private BufferedReadStream(Stream inputStream)
    {
      this.buffers = new List<BufferedReadStream.DataBuffer>();
      this.inputStream = inputStream;
      this.currentBufferIndex = -1;
    }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
      get => throw new NotSupportedException();
      set => throw new NotSupportedException();
    }

    public override void Flush() => throw new NotSupportedException();

    public override int Read(byte[] buffer, int offset, int count)
    {
      ExceptionUtils.CheckArgumentNotNull<byte[]>(buffer, nameof (buffer));
      if (this.currentBufferIndex == -1)
        return 0;
      BufferedReadStream.DataBuffer buffer1;
      for (buffer1 = this.buffers[this.currentBufferIndex]; this.currentBufferReadCount >= buffer1.StoredCount; this.currentBufferReadCount = 0)
      {
        ++this.currentBufferIndex;
        if (this.currentBufferIndex >= this.buffers.Count)
        {
          this.currentBufferIndex = -1;
          return 0;
        }
        buffer1 = this.buffers[this.currentBufferIndex];
      }
      int length = count;
      if (count > buffer1.StoredCount - this.currentBufferReadCount)
        length = buffer1.StoredCount - this.currentBufferReadCount;
      Array.Copy((Array) buffer1.Buffer, this.currentBufferReadCount, (Array) buffer, offset, length);
      this.currentBufferReadCount += length;
      return length;
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    internal static Task<BufferedReadStream> BufferStreamAsync(Stream inputStream)
    {
      BufferedReadStream bufferedReadStream = new BufferedReadStream(inputStream);
      return Task.Factory.Iterate(bufferedReadStream.BufferInputStream()).FollowAlwaysWith((Action<Task>) (task => inputStream.Dispose())).FollowOnSuccessWith<BufferedReadStream>((Func<Task, BufferedReadStream>) (task =>
      {
        bufferedReadStream.ResetForReading();
        return bufferedReadStream;
      }));
    }

    internal void ResetForReading()
    {
      this.currentBufferIndex = this.buffers.Count == 0 ? -1 : 0;
      this.currentBufferReadCount = 0;
    }

    private IEnumerable<Task> BufferInputStream()
    {
      BufferedReadStream bufferedReadStream1 = this;
      while (bufferedReadStream1.inputStream != null)
      {
        BufferedReadStream bufferedReadStream = bufferedReadStream1;
        BufferedReadStream.DataBuffer currentBuffer = bufferedReadStream1.currentBufferIndex == -1 ? (BufferedReadStream.DataBuffer) null : bufferedReadStream1.buffers[bufferedReadStream1.currentBufferIndex];
        if (currentBuffer != null && currentBuffer.FreeBytes < 1024)
          currentBuffer = (BufferedReadStream.DataBuffer) null;
        if (currentBuffer == null)
          currentBuffer = bufferedReadStream1.AddNewBuffer();
        yield return bufferedReadStream1.inputStream.ReadAsync(currentBuffer.Buffer, currentBuffer.OffsetToWriteTo, currentBuffer.FreeBytes).ContinueWith((Action<Task<int>>) (t =>
        {
          try
          {
            int result = t.Result;
            if (result == 0)
              bufferedReadStream.inputStream = (Stream) null;
            else
              currentBuffer.MarkBytesAsWritten(result);
          }
          catch (Exception ex)
          {
            if (!ExceptionUtils.IsCatchableExceptionType(ex))
            {
              throw;
            }
            else
            {
              bufferedReadStream.inputStream = (Stream) null;
              throw;
            }
          }
        }));
      }
    }

    private BufferedReadStream.DataBuffer AddNewBuffer()
    {
      BufferedReadStream.DataBuffer dataBuffer = new BufferedReadStream.DataBuffer();
      this.buffers.Add(dataBuffer);
      this.currentBufferIndex = this.buffers.Count - 1;
      return dataBuffer;
    }

    private sealed class DataBuffer
    {
      internal const int MinReadBufferSize = 1024;
      private const int BufferSize = 65536;
      private readonly byte[] buffer;

      public DataBuffer()
      {
        this.buffer = new byte[65536];
        this.StoredCount = 0;
      }

      public byte[] Buffer => this.buffer;

      public int OffsetToWriteTo => this.StoredCount;

      public int StoredCount { get; private set; }

      public int FreeBytes => this.buffer.Length - this.StoredCount;

      public void MarkBytesAsWritten(int count) => this.StoredCount += count;
    }
  }
}
