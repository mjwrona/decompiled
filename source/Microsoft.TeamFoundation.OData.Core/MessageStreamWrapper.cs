// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.MessageStreamWrapper
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal static class MessageStreamWrapper
  {
    internal static Stream CreateNonDisposingStream(Stream innerStream) => (Stream) new MessageStreamWrapper.MessageStreamWrappingStream(innerStream, true, -1L);

    internal static Stream CreateStreamWithMaxSize(Stream innerStream, long maxBytesToBeRead) => (Stream) new MessageStreamWrapper.MessageStreamWrappingStream(innerStream, false, maxBytesToBeRead);

    internal static Stream CreateNonDisposingStreamWithMaxSize(
      Stream innerStream,
      long maxBytesToBeRead)
    {
      return (Stream) new MessageStreamWrapper.MessageStreamWrappingStream(innerStream, true, maxBytesToBeRead);
    }

    internal static bool IsNonDisposingStream(Stream stream) => stream is MessageStreamWrapper.MessageStreamWrappingStream streamWrappingStream && streamWrappingStream.IgnoreDispose;

    private sealed class MessageStreamWrappingStream : Stream
    {
      private readonly long maxBytesToBeRead;
      private readonly bool ignoreDispose;
      private Stream innerStream;
      private long totalBytesRead;

      internal MessageStreamWrappingStream(
        Stream innerStream,
        bool ignoreDispose,
        long maxBytesToBeRead)
      {
        this.innerStream = innerStream;
        this.ignoreDispose = ignoreDispose;
        this.maxBytesToBeRead = maxBytesToBeRead;
      }

      public override bool CanRead => this.innerStream.CanRead;

      public override bool CanSeek => this.innerStream.CanSeek;

      public override bool CanWrite => this.innerStream.CanWrite;

      public override long Length => this.innerStream.Length;

      public override long Position
      {
        get => this.innerStream.Position;
        set => this.innerStream.Position = value;
      }

      internal bool IgnoreDispose => this.ignoreDispose;

      public override void Flush() => this.innerStream.Flush();

      public override Task FlushAsync(CancellationToken cancellationToken) => this.innerStream.FlushAsync(cancellationToken);

      public override int Read(byte[] buffer, int offset, int count)
      {
        int bytesRead = this.innerStream.Read(buffer, offset, count);
        this.IncreaseTotalBytesRead(bytesRead);
        return bytesRead;
      }

      public override async Task<int> ReadAsync(
        byte[] buffer,
        int offset,
        int count,
        CancellationToken cancellationToken)
      {
        int bytesRead = await this.innerStream.ReadAsync(buffer, offset, count, cancellationToken);
        this.IncreaseTotalBytesRead(bytesRead);
        return bytesRead;
      }

      public override long Seek(long offset, SeekOrigin origin) => this.innerStream.Seek(offset, origin);

      public override void SetLength(long value) => this.innerStream.SetLength(value);

      public override void Write(byte[] buffer, int offset, int count) => this.innerStream.Write(buffer, offset, count);

      public override Task WriteAsync(
        byte[] buffer,
        int offset,
        int count,
        CancellationToken cancellationToken)
      {
        return this.innerStream.WriteAsync(buffer, offset, count, cancellationToken);
      }

      protected override void Dispose(bool disposing)
      {
        if (disposing && !this.ignoreDispose && this.innerStream != null)
        {
          this.innerStream.Dispose();
          this.innerStream = (Stream) null;
        }
        base.Dispose(disposing);
      }

      private void IncreaseTotalBytesRead(int bytesRead)
      {
        if (this.maxBytesToBeRead <= 0L)
          return;
        this.totalBytesRead += bytesRead < 0 ? 0L : (long) bytesRead;
        if (this.totalBytesRead > this.maxBytesToBeRead)
          throw new ODataException(Strings.MessageStreamWrappingStream_ByteLimitExceeded((object) this.totalBytesRead, (object) this.maxBytesToBeRead));
      }
    }
  }
}
