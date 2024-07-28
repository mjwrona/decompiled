// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.Common.NonCloseableStream
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.Common
{
  internal class NonCloseableStream : Stream
  {
    private readonly Stream wrappedStream;

    public NonCloseableStream(Stream wrappedStream)
    {
      CommonUtility.AssertNotNull("WrappedStream", (object) wrappedStream);
      this.wrappedStream = wrappedStream;
    }

    public override bool CanRead => this.wrappedStream.CanRead;

    public override bool CanSeek => this.wrappedStream.CanSeek;

    public override bool CanTimeout => this.wrappedStream.CanTimeout;

    public override bool CanWrite => this.wrappedStream.CanWrite;

    public override long Length => this.wrappedStream.Length;

    public override long Position
    {
      get => this.wrappedStream.Position;
      set => this.wrappedStream.Position = value;
    }

    public override int ReadTimeout
    {
      get => this.wrappedStream.ReadTimeout;
      set => this.wrappedStream.ReadTimeout = value;
    }

    public override int WriteTimeout
    {
      get => this.wrappedStream.WriteTimeout;
      set => this.wrappedStream.WriteTimeout = value;
    }

    public override void Flush() => this.wrappedStream.Flush();

    public override void SetLength(long value) => this.wrappedStream.SetLength(value);

    public override long Seek(long offset, SeekOrigin origin) => this.wrappedStream.Seek(offset, origin);

    public override int Read(byte[] buffer, int offset, int count) => this.wrappedStream.Read(buffer, offset, count);

    public override Task<int> ReadAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      return this.wrappedStream.ReadAsync(buffer, offset, count, cancellationToken);
    }

    public override void Write(byte[] buffer, int offset, int count) => this.wrappedStream.Write(buffer, offset, count);

    public override Task WriteAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      return this.wrappedStream.WriteAsync(buffer, offset, count, cancellationToken);
    }

    protected override void Dispose(bool disposing)
    {
    }
  }
}
