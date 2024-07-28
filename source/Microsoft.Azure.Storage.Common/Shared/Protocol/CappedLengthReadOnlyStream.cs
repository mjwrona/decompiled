// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Shared.Protocol.CappedLengthReadOnlyStream
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Shared.Protocol
{
  internal class CappedLengthReadOnlyStream : Stream
  {
    private readonly Stream wrappedStream;
    private long cappedLength;

    public CappedLengthReadOnlyStream(Stream wrappedStream, long cappedLength)
    {
      if (!wrappedStream.CanSeek || !wrappedStream.CanRead)
        throw new NotSupportedException();
      this.wrappedStream = wrappedStream;
      this.cappedLength = Math.Min(cappedLength, this.wrappedStream.Length);
    }

    public override bool CanRead => true;

    public override bool CanSeek => true;

    public override bool CanWrite => false;

    public override void Flush() => throw new NotSupportedException();

    public override long Length => this.cappedLength;

    public override long Position
    {
      get => this.wrappedStream.Position;
      set => this.wrappedStream.Position = value;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof (count));
      long num = this.cappedLength > this.Position ? this.cappedLength - this.Position : 0L;
      if ((long) count > num)
        count = (int) num;
      return this.wrappedStream.Read(buffer, offset, count);
    }

    public override Task<int> ReadAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof (count));
      long num = this.cappedLength > this.Position ? this.cappedLength - this.Position : 0L;
      if ((long) count > num)
        count = (int) num;
      return this.wrappedStream.ReadAsync(buffer, offset, count, cancellationToken);
    }

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    public override long Seek(long offset, SeekOrigin origin) => this.wrappedStream.Seek(offset, origin);

    public override void SetLength(long value) => this.cappedLength = value >= 0L && value <= this.Length ? value : throw new ArgumentOutOfRangeException(nameof (value));
  }
}
