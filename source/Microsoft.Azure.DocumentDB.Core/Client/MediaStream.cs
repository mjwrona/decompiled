// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Client.MediaStream
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Client
{
  internal sealed class MediaStream : Stream
  {
    private HttpResponseMessage responseMessage;
    private Stream contentStream;
    private bool isDisposed;

    public MediaStream(HttpResponseMessage responseMessage, Stream contentStream)
    {
      this.responseMessage = responseMessage;
      this.contentStream = contentStream;
      this.isDisposed = false;
    }

    public override bool CanRead => this.contentStream.CanRead;

    public override bool CanSeek => this.contentStream.CanSeek;

    public override bool CanTimeout => this.contentStream.CanTimeout;

    public override bool CanWrite => this.contentStream.CanWrite;

    public override long Length => this.contentStream.Length;

    public override int ReadTimeout
    {
      get => this.contentStream.ReadTimeout;
      set => this.contentStream.ReadTimeout = value;
    }

    public override int WriteTimeout
    {
      get => this.contentStream.WriteTimeout;
      set => this.contentStream.WriteTimeout = value;
    }

    public override long Position
    {
      get => this.contentStream.Position;
      set => this.contentStream.Position = value;
    }

    public override int Read(byte[] buffer, int offset, int count) => this.contentStream.Read(buffer, offset, count);

    public override void Write(byte[] buffer, int offset, int count) => this.contentStream.Write(buffer, offset, count);

    public override Task<int> ReadAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      return this.contentStream.ReadAsync(buffer, offset, count, cancellationToken);
    }

    public override Task WriteAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      return this.contentStream.WriteAsync(buffer, offset, count, cancellationToken);
    }

    public override Task CopyToAsync(
      Stream destination,
      int bufferSize,
      CancellationToken cancellationToken)
    {
      return this.contentStream.CopyToAsync(destination, bufferSize, cancellationToken);
    }

    public override void Flush() => this.contentStream.Flush();

    public override Task FlushAsync(CancellationToken cancellationToken) => this.contentStream.FlushAsync(cancellationToken);

    public override int ReadByte() => this.contentStream.ReadByte();

    public override void WriteByte(byte value) => this.contentStream.WriteByte(value);

    public override void SetLength(long value) => this.contentStream.SetLength(value);

    public override long Seek(long offset, SeekOrigin origin) => this.contentStream.Seek(offset, origin);

    protected override void Dispose(bool disposing)
    {
      if (!this.isDisposed & disposing)
      {
        this.responseMessage.Dispose();
        this.isDisposed = true;
      }
      base.Dispose(disposing);
    }
  }
}
