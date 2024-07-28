// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.NonDisposingStream
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal sealed class NonDisposingStream : Stream
  {
    private readonly Stream innerStream;

    internal NonDisposingStream(Stream innerStream) => this.innerStream = innerStream;

    public override bool CanRead => this.innerStream.CanRead;

    public override bool CanSeek => this.innerStream.CanSeek;

    public override bool CanWrite => this.innerStream.CanWrite;

    public override long Length => this.innerStream.Length;

    public override long Position
    {
      get => this.innerStream.Position;
      set => this.innerStream.Position = value;
    }

    public override void Flush() => this.innerStream.Flush();

    public override int Read(byte[] buffer, int offset, int count) => this.innerStream.Read(buffer, offset, count);

    public override async Task<int> ReadAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      return await this.innerStream.ReadAsync(buffer, offset, count, cancellationToken);
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
  }
}
