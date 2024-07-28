// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.ByteBuffer
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common;
using System;
using System.Threading;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp
{
  internal sealed class ByteBuffer : IDisposable, ICloneable
  {
    private static readonly InternalBufferManager BufferManager = InternalBufferManager.Create(52428800L, int.MaxValue, false);
    private static InternalBufferManager TransportBufferManager;
    private static object syncRoot = new object();
    private byte[] buffer;
    private int start;
    private int read;
    private int write;
    private int end;
    private bool autoGrow;
    private int references;
    private InternalBufferManager bufferManager;

    public static void InitBufferManagers()
    {
      if (ByteBuffer.TransportBufferManager != null)
        return;
      lock (ByteBuffer.syncRoot)
      {
        if (ByteBuffer.TransportBufferManager != null)
          return;
        ByteBuffer.TransportBufferManager = InternalBufferManager.Create(50331648L, 65536, true);
      }
    }

    public ByteBuffer(byte[] buffer)
      : this(buffer, 0, 0, buffer.Length, false, (InternalBufferManager) null)
    {
    }

    public ByteBuffer(byte[] buffer, bool autoGrow)
      : this(buffer, 0, 0, buffer.Length, autoGrow, (InternalBufferManager) null)
    {
    }

    public ByteBuffer(ArraySegment<byte> array)
      : this(array.Array, array.Offset, array.Count, array.Count, false, (InternalBufferManager) null)
    {
    }

    public ByteBuffer(int size, bool autoGrow)
      : this(size, autoGrow, false)
    {
    }

    public ByteBuffer(int size, bool autoGrow, bool isTransportBuffer)
      : this(ByteBuffer.AllocateBufferFromPool(size, isTransportBuffer), autoGrow, size)
    {
    }

    public ByteBuffer(byte[] buffer, int offset, int count)
    {
      byte[] buffer1 = buffer;
      int offset1 = offset;
      int num = count;
      // ISSUE: explicit constructor call
      this.\u002Ector(buffer1, offset1, num, num, false, (InternalBufferManager) null);
    }

    private ByteBuffer(ByteBuffer.ManagedBuffer bufferReference, bool autoGrow, int size)
      : this(bufferReference.Buffer, 0, 0, size, autoGrow, bufferReference.BufferManager)
    {
    }

    private ByteBuffer(
      byte[] buffer,
      int offset,
      int count,
      int size,
      bool autoGrow,
      InternalBufferManager bufferManager)
    {
      this.buffer = buffer;
      this.start = offset;
      this.read = offset;
      this.write = offset + count;
      this.end = offset + size;
      this.autoGrow = autoGrow;
      this.bufferManager = bufferManager;
      this.references = 1;
    }

    private static ByteBuffer.ManagedBuffer AllocateBufferFromPool(int size, bool isTransportBuffer) => ByteBuffer.AllocateBuffer(size, isTransportBuffer ? ByteBuffer.TransportBufferManager : ByteBuffer.BufferManager);

    private static ByteBuffer.ManagedBuffer AllocateBuffer(
      int size,
      InternalBufferManager bufferManager)
    {
      if (bufferManager != null)
      {
        byte[] buffer = bufferManager.TakeBuffer(size);
        if (buffer != null)
          return new ByteBuffer.ManagedBuffer(buffer, bufferManager);
      }
      return new ByteBuffer.ManagedBuffer(ByteBuffer.BufferManager.TakeBuffer(size), ByteBuffer.BufferManager);
    }

    public byte[] Buffer => this.buffer;

    public int Capacity => this.end - this.start;

    public int Offset => this.read;

    public int Size => this.end - this.write;

    public int Length => this.write - this.read;

    public int WritePos => this.write;

    public void Validate(bool write, int dataSize)
    {
      bool flag;
      if (write)
      {
        if (this.Size < dataSize && this.autoGrow)
        {
          if (this.references != 1)
            throw new InvalidOperationException("Cannot grow the current buffer because it has more than one references");
          int size = Math.Max(this.Capacity * 2, this.Capacity + dataSize);
          ByteBuffer.ManagedBuffer managedBuffer = this.bufferManager == null ? new ByteBuffer.ManagedBuffer(new byte[size], (InternalBufferManager) null) : ByteBuffer.AllocateBuffer(size, this.bufferManager);
          System.Buffer.BlockCopy((Array) this.buffer, this.start, (Array) managedBuffer.Buffer, 0, this.Capacity);
          int num1 = this.read - this.start;
          int num2 = this.write - this.start;
          this.start = 0;
          this.read = num1;
          this.write = num2;
          this.end = size;
          if (this.bufferManager != null)
            this.bufferManager.ReturnBuffer(this.buffer);
          this.buffer = managedBuffer.Buffer;
          this.bufferManager = managedBuffer.BufferManager;
        }
        flag = this.Size >= dataSize;
      }
      else
        flag = this.Length >= dataSize;
      if (!flag)
        throw new AmqpException(AmqpError.DecodeError, SRAmqp.AmqpInsufficientBufferSize((object) dataSize, (object) (write ? this.Size : this.Length)));
    }

    public void Append(int size) => this.write += size;

    public void Complete(int size) => this.read += size;

    public void Seek(int seekPosition) => this.read = this.start + seekPosition;

    public void Reset()
    {
      this.read = this.start;
      this.write = this.start;
    }

    public object Clone()
    {
      this.AddReference();
      return (object) this;
    }

    public void AdjustPosition(int offset, int length)
    {
      this.read = offset;
      this.write = this.read + length;
    }

    public void Dispose() => this.RemoveReference();

    private void AddReference()
    {
      if (Interlocked.Increment(ref this.references) == 1)
      {
        Interlocked.Decrement(ref this.references);
        throw FxTrace.Exception.AsError((Exception) new InvalidOperationException(SRAmqp.AmqpBufferAlreadyReclaimed));
      }
    }

    private void RemoveReference()
    {
      if (this.references <= 0 || Interlocked.Decrement(ref this.references) != 0)
        return;
      byte[] buffer = this.buffer;
      this.buffer = (byte[]) null;
      if (this.bufferManager == null)
        return;
      this.bufferManager.ReturnBuffer(buffer);
    }

    private struct ManagedBuffer
    {
      public InternalBufferManager BufferManager;
      public byte[] Buffer;

      public ManagedBuffer(byte[] buffer, InternalBufferManager bufferManager)
      {
        this.Buffer = buffer;
        this.BufferManager = bufferManager;
      }
    }
  }
}
