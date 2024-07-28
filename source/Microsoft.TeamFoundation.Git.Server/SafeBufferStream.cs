// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.SafeBufferStream
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class SafeBufferStream : Stream
  {
    private readonly SafeBuffer m_buf;
    private readonly unsafe byte* m_startPtr;
    private readonly bool m_leaveOpen;
    private long m_position;
    private bool m_disposed;
    private Stack<Action> m_onDispose;

    public unsafe SafeBufferStream(SafeBuffer buf, long offset, long length, bool leaveOpen = false)
    {
      ArgumentUtility.CheckForNull<SafeBuffer>(buf, nameof (buf));
      ArgumentUtility.CheckForOutOfRange(offset, nameof (offset), 0L);
      ArgumentUtility.CheckForOutOfRange(length, nameof (length), 0L);
      if (checked ((long) buf.ByteLength) - offset < length)
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0} + {1} > {2}.{3}", (object) nameof (offset), (object) nameof (length), (object) nameof (buf), (object) "ByteLength")));
      this.m_buf = buf;
      this.m_startPtr = (byte*) ((IntPtr) (void*) this.m_buf.DangerousGetHandle() + (IntPtr) offset);
      this.Length = length;
      this.m_leaveOpen = leaveOpen;
    }

    public void PushActionOnDispose(Action dispose)
    {
      this.m_onDispose = this.m_onDispose ?? new Stack<Action>();
      this.m_onDispose.Push(dispose);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && !this.m_disposed)
      {
        this.m_disposed = true;
        while (this.m_onDispose != null && this.m_onDispose.Count != 0)
          this.m_onDispose.Pop()();
        if (!this.m_leaveOpen)
          this.m_buf.Dispose();
      }
      base.Dispose(disposing);
    }

    public override bool CanRead => true;

    public override bool CanSeek => true;

    public override bool CanWrite => false;

    public override long Length { get; }

    public override long Position
    {
      get => this.m_position;
      set
      {
        ArgumentUtility.CheckForOutOfRange(value, nameof (value), 0L);
        this.m_position = value;
      }
    }

    public override void Flush()
    {
    }

    public override unsafe int Read(byte[] array, int offset, int count)
    {
      this.ThrowIfDisposed();
      ArgumentUtility.CheckForNull<byte[]>(array, nameof (array));
      ArgumentUtility.CheckForOutOfRange(offset, nameof (offset), 0);
      ArgumentUtility.CheckForOutOfRange(count, nameof (count), 0);
      if (array.Length - offset < count)
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("{0} + {1} > {2}.{3}", (object) nameof (offset), (object) nameof (count), (object) nameof (array), (object) "Length")));
      long num = Math.Min((long) count, this.Length - this.m_position);
      if (num <= 0L)
        return 0;
      count = (int) num;
      fixed (byte* destination = array)
        Buffer.MemoryCopy((void*) (this.m_startPtr + this.m_position), (void*) destination, (long) array.Length, (long) count);
      this.m_position += (long) count;
      return count;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      switch (origin)
      {
        case SeekOrigin.Begin:
          return this.Position = offset;
        case SeekOrigin.Current:
          return this.Position += offset;
        case SeekOrigin.End:
          return this.Position = this.Length + offset;
        default:
          throw new NotImplementedException();
      }
    }

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    private void ThrowIfDisposed()
    {
      if (this.m_disposed)
        throw new ObjectDisposedException(nameof (SafeBufferStream));
    }
  }
}
