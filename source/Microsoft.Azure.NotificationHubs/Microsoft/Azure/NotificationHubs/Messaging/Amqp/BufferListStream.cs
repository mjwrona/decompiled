// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.BufferListStream
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp
{
  internal sealed class BufferListStream : Stream, ICloneable
  {
    private IList<ArraySegment<byte>> bufferList;
    private int readArray;
    private int readOffset;
    private long length;
    private long position;
    private bool disposed;

    public BufferListStream(IList<ArraySegment<byte>> bufferList)
    {
      this.bufferList = bufferList;
      for (int index = 0; index < this.bufferList.Count; ++index)
        this.length += (long) this.bufferList[index].Count;
    }

    public override bool CanRead => true;

    public override bool CanSeek => true;

    public override bool CanWrite => false;

    public override long Length
    {
      get
      {
        this.ThrowIfDisposed();
        return this.length;
      }
    }

    public override long Position
    {
      get
      {
        this.ThrowIfDisposed();
        return this.position;
      }
      set
      {
        this.ThrowIfDisposed();
        this.SetPosition(value);
      }
    }

    public object Clone()
    {
      this.ThrowIfDisposed();
      return (object) new BufferListStream(this.bufferList);
    }

    public override void Flush() => throw new InvalidOperationException();

    public override int ReadByte()
    {
      this.ThrowIfDisposed();
      if (this.readArray == this.bufferList.Count)
        return -1;
      ArraySegment<byte> buffer = this.bufferList[this.readArray];
      int num = (int) buffer.Array[buffer.Offset + this.readOffset];
      this.Advance(1, buffer.Count);
      return num;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      this.ThrowIfDisposed();
      if (this.readArray == this.bufferList.Count)
        return 0;
      int num = 0;
      while (count > 0 && this.readArray < this.bufferList.Count)
      {
        ArraySegment<byte> buffer1 = this.bufferList[this.readArray];
        int count1 = Math.Min(buffer1.Count - this.readOffset, count);
        Buffer.BlockCopy((Array) buffer1.Array, buffer1.Offset + this.readOffset, (Array) buffer, offset, count1);
        this.Advance(count1, buffer1.Count);
        count -= count1;
        offset += count1;
        num += count1;
      }
      return num;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      this.ThrowIfDisposed();
      long pos = 0;
      switch (origin)
      {
        case SeekOrigin.Begin:
          pos = offset;
          break;
        case SeekOrigin.Current:
          pos += this.position + offset;
          break;
        case SeekOrigin.End:
          pos = this.length + offset;
          break;
      }
      this.SetPosition(pos);
      return pos;
    }

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    public ArraySegment<byte> ReadBytes(int count)
    {
      this.ThrowIfDisposed();
      if (this.readArray == this.bufferList.Count)
        return new ArraySegment<byte>();
      ArraySegment<byte> arraySegment = this.bufferList[this.readArray];
      if (arraySegment.Count - this.readOffset >= count)
      {
        int count1 = arraySegment.Count;
        arraySegment = new ArraySegment<byte>(arraySegment.Array, arraySegment.Offset + this.readOffset, count);
        this.Advance(count, count1);
        return arraySegment;
      }
      count = Math.Min(count, (int) (this.length - this.position));
      byte[] numArray = new byte[count];
      this.Read(numArray, 0, count);
      arraySegment = new ArraySegment<byte>(numArray);
      return arraySegment;
    }

    public ArraySegment<byte>[] ReadBuffers(int count, bool advance, out bool more)
    {
      this.ThrowIfDisposed();
      more = false;
      if (this.readArray == this.bufferList.Count)
        return (ArraySegment<byte>[]) null;
      List<ArraySegment<byte>> arraySegmentList = new List<ArraySegment<byte>>();
      int readArray = this.readArray;
      int readOffset = this.readOffset;
      long position = this.position;
      int count1;
      for (; count > 0 && this.readArray < this.bufferList.Count; count -= count1)
      {
        ArraySegment<byte> buffer = this.bufferList[this.readArray];
        count1 = Math.Min(buffer.Count - this.readOffset, count);
        arraySegmentList.Add(new ArraySegment<byte>(buffer.Array, buffer.Offset + this.readOffset, count1));
        this.Advance(count1, buffer.Count);
      }
      more = this.readArray < this.bufferList.Count;
      if (!advance)
      {
        this.readArray = readArray;
        this.readOffset = readOffset;
        this.position = position;
      }
      return arraySegmentList.ToArray();
    }

    protected override void Dispose(bool disposing)
    {
      try
      {
        if (!(!this.disposed & disposing))
          return;
        this.bufferList = (IList<ArraySegment<byte>>) null;
        this.disposed = true;
      }
      finally
      {
        base.Dispose(disposing);
      }
    }

    private void ThrowIfDisposed()
    {
      if (this.disposed)
        throw new ObjectDisposedException(this.GetType().Name);
    }

    private void SetPosition(long pos)
    {
      this.position = pos >= 0L ? pos : throw new ArgumentOutOfRangeException("position");
      int index;
      for (index = 0; index < this.bufferList.Count && pos > 0L; ++index)
      {
        long num1 = pos;
        ArraySegment<byte> buffer = this.bufferList[index];
        long count1 = (long) buffer.Count;
        if (num1 >= count1)
        {
          long num2 = pos;
          buffer = this.bufferList[index];
          long count2 = (long) buffer.Count;
          pos = num2 - count2;
        }
        else
          break;
      }
      this.readArray = index;
      this.readOffset = (int) pos;
    }

    private void Advance(int count, int segmentCount)
    {
      if (count > segmentCount)
        throw new ArgumentOutOfRangeException(nameof (count));
      this.position += (long) count;
      this.readOffset += count;
      if (this.readOffset != segmentCount)
        return;
      ++this.readArray;
      this.readOffset = 0;
    }

    public static BufferListStream Create(Stream stream, int segmentSize) => BufferListStream.Create(stream, segmentSize, false);

    public static BufferListStream Create(Stream stream, int segmentSize, bool forceCopyStream)
    {
      if (stream == null)
        throw FxTrace.Exception.ArgumentNull(nameof (stream));
      BufferListStream bufferListStream;
      if (stream is BufferListStream && !forceCopyStream)
      {
        bufferListStream = (BufferListStream) ((BufferListStream) stream).Clone();
      }
      else
      {
        stream.Position = 0L;
        bufferListStream = new BufferListStream((IList<ArraySegment<byte>>) BufferListStream.ReadStream(stream, segmentSize, out int _));
      }
      return bufferListStream;
    }

    public static ArraySegment<byte>[] ReadStream(Stream stream, int segmentSize, out int length)
    {
      if (stream == null)
        throw FxTrace.Exception.ArgumentNull(nameof (stream));
      length = 0;
      List<ArraySegment<byte>> arraySegmentList = new List<ArraySegment<byte>>();
      while (true)
      {
        byte[] numArray = new byte[segmentSize];
        int count = stream.Read(numArray, 0, numArray.Length);
        if (count != 0)
        {
          arraySegmentList.Add(new ArraySegment<byte>(numArray, 0, count));
          length += count;
        }
        else
          break;
      }
      return arraySegmentList.ToArray();
    }
  }
}
