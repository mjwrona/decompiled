// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.ParallelDownloadStream
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Blob
{
  internal sealed class ParallelDownloadStream : Stream
  {
    private readonly MemoryMappedViewStream downloadStream;
    private int maxIdleTimeInMs;
    private readonly CancellationTokenSource cts;

    public CancellationToken HangingCancellationToken => this.cts.Token;

    public ParallelDownloadStream(MemoryMappedViewStream downloadStream, int maxIdleTimeInMs)
    {
      this.downloadStream = downloadStream;
      this.maxIdleTimeInMs = maxIdleTimeInMs;
      this.cts = new CancellationTokenSource(this.maxIdleTimeInMs);
    }

    public override Task WriteAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      this.cts.CancelAfter(this.maxIdleTimeInMs);
      return this.downloadStream.WriteAsync(buffer, offset, count, cancellationToken);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      this.cts.CancelAfter(this.maxIdleTimeInMs);
      this.downloadStream.Write(buffer, offset, count);
    }

    public override void Flush() => this.downloadStream.Flush();

    public override void Close() => this.cts.Dispose();

    public override long Seek(long offset, SeekOrigin origin) => this.downloadStream.Seek(offset, origin);

    public override void SetLength(long value) => this.downloadStream.SetLength(value);

    public override int Read(byte[] buffer, int offset, int count) => this.downloadStream.Read(buffer, offset, count);

    public override bool CanRead => this.downloadStream.CanRead;

    public override bool CanSeek => this.downloadStream.CanSeek;

    public override bool CanWrite => this.downloadStream.CanWrite;

    public override long Length => this.downloadStream.Length;

    public override long Position
    {
      get => this.downloadStream.Position;
      set => this.downloadStream.Position = value;
    }
  }
}
