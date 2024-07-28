// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Util.ProgressIncrementingStream
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Core.Util
{
  internal class ProgressIncrementingStream : Stream
  {
    private Stream innerStream;
    private AggregatingProgressIncrementer incrementer;

    public ProgressIncrementingStream(Stream stream, AggregatingProgressIncrementer incrementer)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      if (incrementer == null)
        throw new ArgumentNullException(nameof (incrementer));
      this.innerStream = stream;
      this.incrementer = incrementer;
    }

    public override bool CanRead => this.innerStream.CanRead;

    public override bool CanSeek => this.innerStream.CanSeek;

    public override bool CanTimeout => this.innerStream.CanTimeout;

    public override bool CanWrite => this.innerStream.CanWrite;

    public override async Task CopyToAsync(
      Stream destination,
      int bufferSize,
      CancellationToken cancellationToken)
    {
      long oldPosition = this.innerStream.Position;
      await this.innerStream.CopyToAsync(destination, bufferSize, cancellationToken).ConfigureAwait(false);
      this.incrementer.Report(this.innerStream.Position - oldPosition);
    }

    protected override void Dispose(bool disposing) => this.innerStream.Dispose();

    public override async Task FlushAsync(CancellationToken cancellationToken)
    {
      long oldPosition = this.innerStream.Position;
      await this.innerStream.FlushAsync(cancellationToken).ConfigureAwait(false);
      this.incrementer.Report(this.innerStream.Position - oldPosition);
    }

    public override void Flush()
    {
      long position = this.innerStream.Position;
      this.innerStream.Flush();
      this.incrementer.Report(this.innerStream.Position - position);
    }

    public override long Length => this.innerStream.Length;

    public override long Position
    {
      get => this.innerStream.Position;
      set
      {
        long bytes = value - this.innerStream.Position;
        this.innerStream.Position = value;
        this.incrementer.Report(bytes);
      }
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      int bytes = this.innerStream.Read(buffer, offset, count);
      this.incrementer.Report((long) bytes);
      return bytes;
    }

    public override async Task<int> ReadAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      int bytes = await this.innerStream.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
      this.incrementer.Report((long) bytes);
      return bytes;
    }

    public override int ReadByte()
    {
      int num = this.innerStream.ReadByte();
      if (num == -1)
        return num;
      this.incrementer.Report(1L);
      return num;
    }

    public override int ReadTimeout
    {
      get => this.innerStream.ReadTimeout;
      set => this.innerStream.ReadTimeout = value;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      long position = this.innerStream.Position;
      long num = this.innerStream.Seek(offset, origin);
      this.incrementer.Report(num - position);
      return num;
    }

    public override void SetLength(long value) => this.innerStream.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count)
    {
      this.innerStream.Write(buffer, offset, count);
      this.incrementer.Report((long) count);
    }

    public override async Task WriteAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      await this.innerStream.WriteAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
      this.incrementer.Report((long) count);
    }

    public override void WriteByte(byte value)
    {
      this.innerStream.WriteByte(value);
      this.incrementer.Report(1L);
    }

    public override int WriteTimeout
    {
      get => this.innerStream.WriteTimeout;
      set => this.innerStream.WriteTimeout = value;
    }
  }
}
