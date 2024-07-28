// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.ReadLengthLimitingStream
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Core
{
  internal class ReadLengthLimitingStream : Stream
  {
    private readonly Stream wrappedStream;
    private long streamBeginIndex;
    private long position;
    private long length;
    private bool disposed;

    public ReadLengthLimitingStream(Stream wrappedStream, long length)
    {
      if (!wrappedStream.CanSeek || !wrappedStream.CanRead)
        throw new NotSupportedException();
      CommonUtility.AssertNotNull("wrappedSream", (object) wrappedStream);
      this.wrappedStream = wrappedStream;
      this.length = Math.Min(length, wrappedStream.Length - wrappedStream.Position);
      this.streamBeginIndex = wrappedStream.Position;
      this.Position = 0L;
    }

    public override bool CanRead => this.wrappedStream.CanRead;

    public override bool CanSeek => true;

    public override bool CanWrite => false;

    public override long Length => this.length;

    public override void SetLength(long value) => throw new NotSupportedException();

    public override long Position
    {
      get => this.position;
      set
      {
        CommonUtility.AssertInBounds<long>("position", value, 0L, this.Length);
        this.position = value;
        this.wrappedStream.Position = this.streamBeginIndex + value;
      }
    }

    public override void Flush() => this.wrappedStream.Flush();

    public override long Seek(long offset, SeekOrigin origin)
    {
      long num;
      switch (origin)
      {
        case SeekOrigin.Begin:
          num = 0L;
          break;
        case SeekOrigin.Current:
          num = this.Position;
          break;
        case SeekOrigin.End:
          num = this.length - offset;
          offset = 0L;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      this.Position = num + offset;
      return this.Position;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof (count));
      if (this.Position + (long) count > this.Length)
        count = (int) (this.Length - this.Position);
      int num = this.wrappedStream.Read(buffer, offset, count);
      this.Position += (long) num;
      return num;
    }

    public override IAsyncResult BeginRead(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof (count));
      if (this.Position + (long) count > this.Length)
        count = (int) (this.Length - this.Position);
      return this.wrappedStream.BeginRead(buffer, offset, count, callback, state);
    }

    public override int EndRead(IAsyncResult asyncResult)
    {
      int num = this.wrappedStream.EndRead(asyncResult);
      this.Position += (long) num;
      return num;
    }

    public override async Task<int> ReadAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      ReadLengthLimitingStream lengthLimitingStream = this;
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof (count));
      if (lengthLimitingStream.Position + (long) count > lengthLimitingStream.Length)
        count = (int) (lengthLimitingStream.Length - lengthLimitingStream.Position);
      int num = await lengthLimitingStream.wrappedStream.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
      lengthLimitingStream.Position += (long) num;
      return num;
    }

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    protected override void Dispose(bool disposing)
    {
      if (this.disposed)
        return;
      if (disposing)
        this.wrappedStream.Dispose();
      this.disposed = true;
      base.Dispose(disposing);
    }
  }
}
