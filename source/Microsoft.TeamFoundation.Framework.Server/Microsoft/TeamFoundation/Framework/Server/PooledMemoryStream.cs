// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PooledMemoryStream
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.IO;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class PooledMemoryStream : Stream
  {
    private ByteArray m_byteArray;
    private MemoryStream m_memoryStream;
    private const string s_Area = "FileService";
    private const string s_Layer = "PooledMemoryStream";

    public PooledMemoryStream(int size)
      : this(size, DefaultBufferPoolsProvider.Instance)
    {
    }

    public PooledMemoryStream(int size, BufferPoolsProvider provider)
    {
      this.m_byteArray = new ByteArray(size, provider);
      this.m_memoryStream = new MemoryStream(this.m_byteArray.Bytes, 0, this.m_byteArray.Bytes.Length, true, true);
      this.m_memoryStream.SetLength(0L);
    }

    public override bool CanRead
    {
      get
      {
        this.CheckIfDisposed();
        return this.m_memoryStream.CanRead;
      }
    }

    public override bool CanSeek
    {
      get
      {
        this.CheckIfDisposed();
        return this.m_memoryStream.CanSeek;
      }
    }

    public override bool CanWrite
    {
      get
      {
        this.CheckIfDisposed();
        return this.m_memoryStream.CanWrite;
      }
    }

    public override void Flush()
    {
      this.CheckIfDisposed();
      this.m_memoryStream.Flush();
    }

    public override long Length
    {
      get
      {
        this.CheckIfDisposed();
        return this.m_memoryStream.Length;
      }
    }

    public override long Position
    {
      get
      {
        this.CheckIfDisposed();
        return this.m_memoryStream.Position;
      }
      set
      {
        this.CheckIfDisposed();
        this.m_memoryStream.Position = value;
      }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      this.CheckIfDisposed();
      return this.m_memoryStream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      this.CheckIfDisposed();
      return this.m_memoryStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
      this.CheckIfDisposed();
      this.m_memoryStream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      this.CheckIfDisposed();
      int capacity = this.m_memoryStream.Capacity;
      int position = (int) this.m_memoryStream.Position;
      if (this.m_memoryStream.Position + (long) count > (long) capacity)
      {
        if (TeamFoundationTracingService.IsRawTracingEnabled(15006, TraceLevel.Warning, "FileService", nameof (PooledMemoryStream), (string[]) null))
          TeamFoundationTracingService.TraceRaw(15006, TraceLevel.Warning, "FileService", nameof (PooledMemoryStream), "Reallocating MemoryStream {0}", (object) new StackTracer());
        ByteArray byteArray1 = (ByteArray) null;
        ByteArray byteArray2 = this.m_byteArray;
        MemoryStream memoryStream1 = this.m_memoryStream;
        try
        {
          byteArray1 = new ByteArray(Math.Max(capacity * 2, capacity + count));
          Array.Copy((Array) this.m_memoryStream.GetBuffer(), (Array) byteArray1.Bytes, position);
          MemoryStream memoryStream2 = new MemoryStream(byteArray1.Bytes, 0, byteArray1.Bytes.Length, true, true);
          memoryStream2.SetLength((long) position);
          memoryStream2.Position = (long) position;
          this.m_byteArray = byteArray1;
          this.m_memoryStream = memoryStream2;
        }
        finally
        {
          if (this.m_memoryStream == memoryStream1)
          {
            this.m_byteArray = byteArray2;
            byteArray1?.Dispose();
          }
          else
            byteArray2.Dispose();
        }
      }
      this.m_memoryStream.Write(buffer, offset, count);
    }

    protected virtual void CheckIfDisposed()
    {
      if (this.IsDisposed)
        throw new ObjectDisposedException(nameof (PooledMemoryStream));
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      this.m_memoryStream = (MemoryStream) null;
      if (this.m_byteArray != null)
      {
        this.m_byteArray.Dispose();
        this.m_byteArray = (ByteArray) null;
      }
      this.IsDisposed = true;
    }

    public bool IsDisposed { get; private set; }
  }
}
