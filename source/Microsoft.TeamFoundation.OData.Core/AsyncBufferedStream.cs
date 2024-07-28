// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.AsyncBufferedStream
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal sealed class AsyncBufferedStream : Stream
  {
    private readonly Stream innerStream;
    private Queue<AsyncBufferedStream.DataBuffer> bufferQueue;
    private AsyncBufferedStream.DataBuffer bufferToAppendTo;

    internal AsyncBufferedStream(Stream stream)
    {
      this.innerStream = stream;
      this.bufferQueue = new Queue<AsyncBufferedStream.DataBuffer>();
    }

    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
      get => throw new NotSupportedException();
      set => throw new NotSupportedException();
    }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (count <= 0)
        return;
      if (this.bufferToAppendTo == null)
        this.QueueNewBuffer();
      while (count > 0)
      {
        int num = this.bufferToAppendTo.Write(buffer, offset, count);
        if (num < count)
          this.QueueNewBuffer();
        count -= num;
        offset += num;
      }
    }

    internal void Clear()
    {
      this.bufferQueue.Clear();
      this.bufferToAppendTo = (AsyncBufferedStream.DataBuffer) null;
    }

    internal void FlushSync()
    {
      Queue<AsyncBufferedStream.DataBuffer> dataBufferQueue = this.PrepareFlushBuffers();
      if (dataBufferQueue == null)
        return;
      while (dataBufferQueue.Count > 0)
        dataBufferQueue.Dequeue().WriteToStream(this.innerStream);
    }

    internal new Task FlushAsync() => this.FlushAsyncInternal();

    internal Task FlushAsyncInternal()
    {
      Queue<AsyncBufferedStream.DataBuffer> buffers = this.PrepareFlushBuffers();
      return buffers == null ? TaskUtils.CompletedTask : Task.Factory.Iterate(this.FlushBuffersAsync(buffers));
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.bufferQueue.Count > 0)
        throw new ODataException(Strings.AsyncBufferedStream_WriterDisposedWithoutFlush);
      base.Dispose(disposing);
    }

    private void QueueNewBuffer()
    {
      this.bufferToAppendTo = new AsyncBufferedStream.DataBuffer();
      this.bufferQueue.Enqueue(this.bufferToAppendTo);
    }

    private Queue<AsyncBufferedStream.DataBuffer> PrepareFlushBuffers()
    {
      if (this.bufferQueue.Count == 0)
        return (Queue<AsyncBufferedStream.DataBuffer>) null;
      this.bufferToAppendTo = (AsyncBufferedStream.DataBuffer) null;
      Queue<AsyncBufferedStream.DataBuffer> bufferQueue = this.bufferQueue;
      this.bufferQueue = new Queue<AsyncBufferedStream.DataBuffer>();
      return bufferQueue;
    }

    private IEnumerable<Task> FlushBuffersAsync(Queue<AsyncBufferedStream.DataBuffer> buffers)
    {
      while (buffers.Count > 0)
        yield return buffers.Dequeue().WriteToStreamAsync(this.innerStream);
    }

    private sealed class DataBuffer
    {
      private const int BufferSize = 80896;
      private readonly byte[] buffer;
      private int storedCount;

      public DataBuffer()
      {
        this.buffer = new byte[80896];
        this.storedCount = 0;
      }

      public int Write(byte[] data, int index, int count)
      {
        int length = count;
        if (length > this.buffer.Length - this.storedCount)
          length = this.buffer.Length - this.storedCount;
        if (length > 0)
        {
          Array.Copy((Array) data, index, (Array) this.buffer, this.storedCount, length);
          this.storedCount += length;
        }
        return length;
      }

      public void WriteToStream(Stream stream) => stream.Write(this.buffer, 0, this.storedCount);

      public Task WriteToStreamAsync(Stream stream) => stream.WriteAsync(this.buffer, 0, this.storedCount);
    }
  }
}
