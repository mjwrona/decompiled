// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataNotificationStream
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal sealed class ODataNotificationStream : Stream
  {
    private readonly Stream stream;
    private IODataStreamListener listener;

    internal ODataNotificationStream(Stream underlyingStream, IODataStreamListener listener)
    {
      this.stream = underlyingStream;
      this.listener = listener;
    }

    public override bool CanRead => this.stream.CanRead;

    public override bool CanSeek => this.stream.CanSeek;

    public override bool CanWrite => this.stream.CanWrite;

    public override long Length => this.stream.Length;

    public override long Position
    {
      get => this.stream.Position;
      set => this.stream.Position = value;
    }

    public override bool CanTimeout => this.stream.CanTimeout;

    public override int ReadTimeout
    {
      get => this.stream.ReadTimeout;
      set => this.stream.ReadTimeout = value;
    }

    public override int WriteTimeout
    {
      get => this.stream.WriteTimeout;
      set => this.stream.WriteTimeout = value;
    }

    public override void Flush() => this.stream.Flush();

    public override int Read(byte[] buffer, int offset, int count) => this.stream.Read(buffer, offset, count);

    public override void SetLength(long value) => this.stream.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count) => this.stream.Write(buffer, offset, count);

    public override long Seek(long offset, SeekOrigin origin) => this.stream.Seek(offset, origin);

    public override int ReadByte() => this.stream.ReadByte();

    public override void WriteByte(byte value) => this.stream.WriteByte(value);

    public override string ToString() => this.stream.ToString();

    public override Task FlushAsync(CancellationToken cancellationToken) => this.stream.FlushAsync(cancellationToken);

    public override Task CopyToAsync(
      Stream destination,
      int bufferSize,
      CancellationToken cancellationToken)
    {
      return this.stream.CopyToAsync(destination, bufferSize, cancellationToken);
    }

    public override Task<int> ReadAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      return this.stream.ReadAsync(buffer, offset, count, cancellationToken);
    }

    public override Task WriteAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      return this.stream.WriteAsync(buffer, offset, count, cancellationToken);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.listener != null)
      {
        this.listener.StreamDisposed();
        this.listener = (IODataStreamListener) null;
      }
      base.Dispose(disposing);
    }
  }
}
