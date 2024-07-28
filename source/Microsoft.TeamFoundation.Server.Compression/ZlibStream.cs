// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Compression.ZlibStream
// Assembly: Microsoft.TeamFoundation.Server.Compression, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E666AAE4-36CD-4581-80AF-1B631308AB46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.Compression.dll

using Microsoft.TeamFoundation.Server.Core;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Server.Compression
{
  public class ZlibStream : Stream, IZlibInflateStream
  {
    private DeflateStream _deflateStream;
    private const int Zlib_DefaultWindowBits = 15;

    public ZlibStream(Stream stream, CompressionMode mode)
      : this(stream, mode, false)
    {
    }

    public ZlibStream(Stream stream, CompressionMode mode, bool leaveOpen)
    {
      this.LeaveBaseOpen = leaveOpen;
      this._deflateStream = new DeflateStream(stream, mode, leaveOpen, 15, true);
    }

    public ZlibStream(Stream stream, CompressionLevel compressionLevel)
      : this(stream, compressionLevel, false)
    {
    }

    public ZlibStream(Stream stream, CompressionLevel compressionLevel, bool leaveOpen)
    {
      this.LeaveBaseOpen = leaveOpen;
      this._deflateStream = new DeflateStream(stream, compressionLevel, leaveOpen, 15, true);
    }

    public bool LeaveBaseOpen { get; private set; }

    public override bool CanRead => this._deflateStream != null && this._deflateStream.CanRead;

    public override bool CanWrite => this._deflateStream != null && this._deflateStream.CanWrite;

    public override bool CanSeek => this._deflateStream != null && this._deflateStream.CanSeek;

    public override long Length => throw new NotSupportedException(SR.NotSupported);

    public override long Position
    {
      get => throw new NotSupportedException(SR.NotSupported);
      set => throw new NotSupportedException(SR.NotSupported);
    }

    public override void Flush()
    {
      this.CheckDeflateStream();
      this._deflateStream.Flush();
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException(SR.NotSupported);

    public override void SetLength(long value) => throw new NotSupportedException(SR.NotSupported);

    public override int ReadByte()
    {
      this.CheckDeflateStream();
      return this._deflateStream.ReadByte();
    }

    public override int Read(byte[] array, int offset, int count)
    {
      this.CheckDeflateStream();
      return this._deflateStream.Read(array, offset, count);
    }

    public override void Write(byte[] array, int offset, int count)
    {
      this.CheckDeflateStream();
      this._deflateStream.Write(array, offset, count);
    }

    protected override void Dispose(bool disposing)
    {
      try
      {
        if (disposing && this._deflateStream != null)
          this._deflateStream.Dispose();
        this._deflateStream = (DeflateStream) null;
      }
      finally
      {
        base.Dispose(disposing);
      }
    }

    public Stream BaseStream => this._deflateStream != null ? this._deflateStream.BaseStream : (Stream) null;

    public int AvailableInputBytes => this._deflateStream.AvailableInputBytes;

    public override Task<int> ReadAsync(
      byte[] array,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      this.CheckDeflateStream();
      return this._deflateStream.ReadAsync(array, offset, count, cancellationToken);
    }

    public override Task WriteAsync(
      byte[] array,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      this.CheckDeflateStream();
      return this._deflateStream.WriteAsync(array, offset, count, cancellationToken);
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
      this.CheckDeflateStream();
      return this._deflateStream.FlushAsync(cancellationToken);
    }

    private void CheckDeflateStream()
    {
      if (this._deflateStream != null)
        return;
      ZlibStream.ThrowStreamClosedException();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowStreamClosedException() => throw new ObjectDisposedException((string) null, SR.ObjectDisposed_StreamClosed);
  }
}
