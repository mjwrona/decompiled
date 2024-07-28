// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.CloneableStream
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class CloneableStream : Stream
  {
    private readonly MemoryStream internalStream;

    public CloneableStream Clone() => new CloneableStream(new MemoryStream(this.internalStream.GetBuffer(), 0, (int) this.internalStream.Length, false, true));

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

    public ArraySegment<byte> GetBuffer() => new ArraySegment<byte>(this.internalStream.GetBuffer(), 0, (int) this.internalStream.Length);

    public override IAsyncResult BeginRead(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      return this.internalStream.BeginRead(buffer, offset, count, callback, state);
    }

    public override IAsyncResult BeginWrite(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      return this.internalStream.BeginWrite(buffer, offset, count, callback, state);
    }

    public override void Close() => this.internalStream.Close();

    public override int EndRead(IAsyncResult asyncResult) => this.internalStream.EndRead(asyncResult);

    public override void EndWrite(IAsyncResult asyncResult) => this.internalStream.EndWrite(asyncResult);

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
