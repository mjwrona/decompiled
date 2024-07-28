// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.CloneableStream
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class CloneableStream : Stream
  {
    private readonly MemoryStream internalStream;

    public CloneableStream Clone() => new CloneableStream(new MemoryStream(CustomTypeExtensions.GetBuffer(this.internalStream), 0, (int) this.internalStream.Length, false, true));

    public CloneableStream(MemoryStream internalStream) => this.internalStream = internalStream;

    public override bool CanRead => this.internalStream.CanRead;

    public override bool CanSeek => this.internalStream.CanSeek;

    public override bool CanTimeout => this.internalStream.CanTimeout;

    public override bool CanWrite => this.internalStream.CanWrite;

    public override long Length => this.internalStream.Length;

    public override long Position
    {
      get => this.internalStream.Position;
      set => this.internalStream.Position = value;
    }

    public override int ReadTimeout
    {
      get => this.internalStream.ReadTimeout;
      set => this.internalStream.ReadTimeout = value;
    }

    public override int WriteTimeout
    {
      get => this.internalStream.WriteTimeout;
      set => this.internalStream.WriteTimeout = value;
    }

    public override void Flush() => this.internalStream.Flush();

    public override Task FlushAsync(CancellationToken cancellationToken) => this.internalStream.FlushAsync(cancellationToken);

    public override int Read(byte[] buffer, int offset, int count) => this.internalStream.Read(buffer, offset, count);

    public override Task<int> ReadAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      return this.internalStream.ReadAsync(buffer, offset, count, cancellationToken);
    }

    public override int ReadByte() => this.internalStream.ReadByte();

    public override long Seek(long offset, SeekOrigin loc) => this.internalStream.Seek(offset, loc);

    public override void SetLength(long value) => this.internalStream.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count) => this.internalStream.Write(buffer, offset, count);

    public override Task WriteAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      return this.internalStream.WriteAsync(buffer, offset, count, cancellationToken);
    }

    public override void WriteByte(byte value) => this.internalStream.WriteByte(value);

    protected override void Dispose(bool disposing) => this.internalStream.Dispose();

    public void WriteTo(Stream target) => this.internalStream.WriteTo(target);
  }
}
