// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.NonCloseableStream
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Core
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

    public override IAsyncResult BeginRead(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      return this.wrappedStream.BeginRead(buffer, offset, count, callback, state);
    }

    public override int EndRead(IAsyncResult asyncResult) => this.wrappedStream.EndRead(asyncResult);

    public override Task<int> ReadAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      return this.wrappedStream.ReadAsync(buffer, offset, count, cancellationToken);
    }

    public override IAsyncResult BeginWrite(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      return this.wrappedStream.BeginWrite(buffer, offset, count, callback, state);
    }

    public override void EndWrite(IAsyncResult asyncResult) => this.wrappedStream.EndWrite(asyncResult);

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
