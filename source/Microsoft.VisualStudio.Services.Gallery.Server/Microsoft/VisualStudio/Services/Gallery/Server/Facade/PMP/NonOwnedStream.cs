// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP.NonOwnedStream
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade.PMP
{
  internal class NonOwnedStream : Stream
  {
    protected NonOwnedStream()
    {
    }

    public NonOwnedStream(Stream innerStream) => this.InnerStream = innerStream != null ? innerStream : throw new ArgumentNullException(nameof (innerStream));

    protected Stream InnerStream { get; set; }

    protected bool IsDisposed { get; private set; }

    public override bool CanRead => !this.IsDisposed && this.InnerStream.CanRead;

    public override bool CanSeek => !this.IsDisposed && this.InnerStream.CanSeek;

    public override bool CanTimeout => this.InnerStream.CanTimeout;

    public override bool CanWrite => !this.IsDisposed && this.InnerStream.CanWrite;

    public override long Length
    {
      get
      {
        this.ThrowIfDisposed();
        return this.InnerStream.Length;
      }
    }

    public override long Position
    {
      get
      {
        this.ThrowIfDisposed();
        return this.InnerStream.Position;
      }
      set
      {
        this.ThrowIfDisposed();
        this.InnerStream.Position = value;
      }
    }

    public override int ReadTimeout
    {
      get
      {
        this.ThrowIfDisposed();
        return this.InnerStream.ReadTimeout;
      }
      set
      {
        this.ThrowIfDisposed();
        this.InnerStream.ReadTimeout = value;
      }
    }

    public override int WriteTimeout
    {
      get
      {
        this.ThrowIfDisposed();
        return this.InnerStream.WriteTimeout;
      }
      set
      {
        this.ThrowIfDisposed();
        this.InnerStream.WriteTimeout = value;
      }
    }

    public override IAsyncResult BeginRead(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      this.ThrowIfDisposed();
      return this.InnerStream.BeginRead(buffer, offset, count, callback, state);
    }

    public override IAsyncResult BeginWrite(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      this.ThrowIfDisposed();
      return this.InnerStream.BeginWrite(buffer, offset, count, callback, state);
    }

    public override void Close() => base.Close();

    public override Task CopyToAsync(
      Stream destination,
      int bufferSize,
      CancellationToken cancellationToken)
    {
      this.ThrowIfDisposed();
      return this.InnerStream.CopyToAsync(destination, bufferSize, cancellationToken);
    }

    [SuppressMessage("Microsoft.Usage", "CA2215:Dispose methods should call base class dispose", Justification = "We're intentionally preventing a double dispose here.")]
    protected override void Dispose(bool disposing)
    {
      if (this.IsDisposed)
        return;
      base.Dispose(disposing);
      this.IsDisposed = true;
    }

    public override int EndRead(IAsyncResult asyncResult)
    {
      this.ThrowIfDisposed();
      return this.InnerStream.EndRead(asyncResult);
    }

    public override void EndWrite(IAsyncResult asyncResult)
    {
      this.ThrowIfDisposed();
      this.InnerStream.EndWrite(asyncResult);
    }

    public override void Flush()
    {
      this.ThrowIfDisposed();
      this.InnerStream.Flush();
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
      this.ThrowIfDisposed();
      return this.InnerStream.FlushAsync(cancellationToken);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      this.ThrowIfDisposed();
      return this.InnerStream.Read(buffer, offset, count);
    }

    public override Task<int> ReadAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      this.ThrowIfDisposed();
      return this.InnerStream.ReadAsync(buffer, offset, count, cancellationToken);
    }

    public override int ReadByte()
    {
      this.ThrowIfDisposed();
      return this.InnerStream.ReadByte();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      this.ThrowIfDisposed();
      return this.InnerStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
      this.ThrowIfDisposed();
      this.InnerStream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      this.ThrowIfDisposed();
      this.InnerStream.Write(buffer, offset, count);
    }

    public override Task WriteAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      this.ThrowIfDisposed();
      return this.InnerStream.WriteAsync(buffer, offset, count, cancellationToken);
    }

    public override void WriteByte(byte value)
    {
      this.ThrowIfDisposed();
      this.InnerStream.WriteByte(value);
    }

    protected void ThrowIfDisposed()
    {
      if (this.IsDisposed)
        throw new ObjectDisposedException((string) null);
    }
  }
}
